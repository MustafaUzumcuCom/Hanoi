using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using Hanoi.Serialization;
using Hanoi.Serialization.Core.Tls;
using Hanoi.Sockets;

namespace Hanoi.Transports {
    /// <summary>
    ///   TCP/IP Transport implementation
    /// </summary>
    internal sealed class TcpTransport : BaseTransport, ISecureTransport {
        private const string StreamNamespace = "jabber:client";
        private const string StreamURI = "http://etherx.jabber.org/streams";
        private const string StreamVersion = "1.0";

        private const string StreamFormat =
            "<?xml version='1.0' encoding='UTF-8' ?><stream:stream xmlns='{0}' xmlns:stream='{1}' to='{2}' version='{3}'>";

        private const string EndStream = "</stream:stream>";

        private XmppMemoryStream inputBuffer;
        private Stream networkStream;
        private XmppStreamParser parser;
        private ProxySocket socket;
        private AutoResetEvent tlsProceedEvent;

        #region ISecureTransport Members

        /// <summary>
        ///   Opens the connection
        /// </summary>
        /// <param name = "connectionString">The connection string used for authentication.</param>
        public override void Open(XmppConnectionString connectionString) {
            // Connection string
            ConnectionString = connectionString;
            UserId = ConnectionString.UserId;

            // Initialization
            Initialize();

            // Connect to the server
            Connect();

            // Begin Receiving Data
            BeginReceiveData();
        }

        /// <summary>
        ///   Closes the connection
        /// </summary>
        public override void Close() {
            if (!IsDisposed)
            {
                try
                {
                    Send(EndStream);
                }
                    // TODO: empty try-catch bad, martial arts good
// ReSharper disable EmptyGeneralCatchClause
                catch
// ReSharper restore EmptyGeneralCatchClause
                {
                }
                finally
                {
                    if (tlsProceedEvent != null)
                    {
                        tlsProceedEvent.Set();
                        tlsProceedEvent = null;
                    }

                    if (networkStream != null)
                    {
                        networkStream.Dispose();
                        networkStream = null;
                    }

                    if (socket != null)
                    {
                        socket.Close();
                        socket = null;
                    }

                    if (inputBuffer != null)
                    {
                        inputBuffer.Dispose();
                        inputBuffer = null;
                    }

                    if (parser != null)
                    {
                        parser.Dispose();
                        parser = null;
                    }
                }

                base.Close();
            }
        }

        /// <summary>
        ///   Sends a new message.
        /// </summary>
        /// <param name = "message">The message to be sent</param>
        public override void Send(object message) {
            Send(XmppSerializer.Serialize(message));
        }

        /// <summary>
        ///   Sends an XMPP message string to the XMPP Server
        /// </summary>
        /// <param name = "value"></param>
        public override void Send(string value) {
            Send(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        ///   Sends an XMPP message buffer to the XMPP Server
        /// </summary>
        public override void Send(byte[] buffer) {
            lock (SyncWrites)
            {
                Debug.WriteLine(Encoding.UTF8.GetString(buffer, 0, buffer.Length));

                networkStream.Write(buffer, 0, buffer.Length);
                networkStream.Flush();
            }
        }

        /// <summary>
        ///   Initializes the XMPP stream.
        /// </summary>
        public override void InitializeXmppStream() {
            // Serialization can't be used in this case
            string xml = String.Format
                (
                    StreamFormat,
                    StreamNamespace,
                    StreamURI,
                    UserId.DomainName,
                    StreamVersion
                );

            Send(xml);
        }

        /// <summary>
        ///   Opens a secure connection against the XMPP server using TLS
        /// </summary>
        public void OpenSecureConnection() {
            // Send Start TLS message
            var tlsMsg = new StartTls();
            Send(tlsMsg);

            tlsProceedEvent.WaitOne();

            OpenSecureStream();

            // Clear the Input Buffer
            inputBuffer.Clear();

            // Re-Start Async Reads
            BeginReceiveData();

            // Re-Initialize XMPP Stream
            InitializeXmppStream();
        }

        #endregion

        private void OpenSecureStream() {
            if (networkStream != null)
            {
                networkStream.Close();
                networkStream = null;
            }

            // Initialize the Ssl Stream
            networkStream = new SslStream
                (
                new NetworkStream(socket, false),
                false,
                RemoteCertificateValidation
                );

            // Perform SSL Authentication
            ((SslStream) networkStream).AuthenticateAsClient
                (
                    ConnectionString.HostName,
                    null,
                    SslProtocols.Tls,
                    false
                );
        }

        /// <summary>
        ///   Opens the connection to the XMPP server.
        /// </summary>
        private void Connect() {
            try
            {
                if (ConnectionString.ResolveHostName)
                {
                    // ReSharper disable RedundantBaseQualifier
                    base.ResolveHostName();
                    // ReSharper restore RedundantBaseQualifier
                }

                var hostadd = Dns.GetHostEntry(HostName).AddressList[0];
                var hostEndPoint = new IPEndPoint(hostadd, ConnectionString.PortNumber);

                socket = new ProxySocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                if (ConnectionString.UseProxy)
                {
                    IPAddress proxyadd = Dns.GetHostEntry(ConnectionString.ProxyServer).AddressList[0];
                    var proxyEndPoint = new IPEndPoint(proxyadd, ConnectionString.ProxyPortNumber);

                    switch (ConnectionString.ProxyType)
                    {
                        case "SOCKS4":
                            socket.ProxyType = ProxyTypes.Socks4;
                            break;

                        case "SOCKS5":
                            socket.ProxyType = ProxyTypes.Socks5;
                            break;

                        default:
                            socket.ProxyType = ProxyTypes.None;
                            break;
                    }

                    socket.ProxyEndPoint = proxyEndPoint;
                    socket.ProxyUser = ConnectionString.ProxyUserName;

                    if (!String.IsNullOrWhiteSpace(ConnectionString.ProxyPassword))
                    {
                        socket.ProxyPass = ConnectionString.ProxyPassword;
                    }
                }

                // Disables the Nagle algorithm for send coalescing.
                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);

                // Make the socket to connect to the Server
                socket.Connect(hostEndPoint);

                // Create the Stream Instance
                networkStream = new NetworkStream(socket, false);
            }
            catch (Exception ex)
            {
                throw new XmppException(
                    String.Format("Unable to connect to XMPP server {0}", ConnectionString.HostName), ex);
            }
        }

        /// <summary>
        ///   Startds async readings on the socket connected to the server
        /// </summary>
        private void BeginReceiveData() {
            var state = new StateObject(networkStream);
            AsyncCallback asyncCallback = ReceiveCallback;

            // Begin receiving the data from the remote device.
            IAsyncResult ar = state.WorkStream.BeginRead(state.Buffer, 0, state.Buffer.Length, asyncCallback, state);

            if (ar.CompletedSynchronously)
            {
                ProcessAsyncRead(ar);
            }
        }

        /// <summary>
        ///   Async read callback
        /// </summary>
        /// <param name = "ar"></param>
        private void ReceiveCallback(IAsyncResult ar) {
            if (!ar.CompletedSynchronously)
            {
                ProcessAsyncRead(ar);
            }
        }

        /// <summary>
        ///   Assync read processing.
        /// </summary>
        /// <param name = "ar"></param>
        private void ProcessAsyncRead(IAsyncResult ar) {
            // Retrieve the state object and the client socket 
            // from the asynchronous state object.
            var state = (StateObject) ar.AsyncState;

            if (state.WorkStream != null && state.WorkStream.CanRead)
            {
                // Read data from the remote device.
                int bytesRead = state.WorkStream.EndRead(ar);

                if (bytesRead > 0)
                {
                    Monitor.Enter(SyncReads);

                    long currentPosition = inputBuffer.Position;

                    inputBuffer.Seek(0, SeekOrigin.End);
                    inputBuffer.Write(state.Buffer, 0, bytesRead);
                    inputBuffer.Flush();
                    inputBuffer.Seek(currentPosition, SeekOrigin.Begin);

                    Monitor.Exit(SyncReads);

                    var resetEvents = ProcessPendingMessages();

                    if (resetEvents != null && resetEvents.Count > 0)
                    {
                        foreach (var resetEvent in resetEvents)
                        {
                            resetEvent.Set();
                        }
                    }
                    else if (!IsDisposed)
                    {
                        BeginReceiveData();
                    }
                }
            }
        }

        /// <summary>
        ///   Process all pending XMPP messages
        /// </summary>
        /// <returns></returns>
        private List<AutoResetEvent> ProcessPendingMessages() {
            var resetEvents = new List<AutoResetEvent>();

            while (parser != null && !parser.EOF)
            {
                AutoResetEvent resetEvent = ProcessXmppMessage(parser.ReadNextNode());

                if (resetEvent != null)
                {
                    resetEvents.Add(resetEvent);
                }
            }

            return resetEvents;
        }

        /// <summary>
        ///   Procesa an arbitrary XMPP Message
        /// </summary>
        /// <param name = "node"></param>
        /// <returns></returns>
        private AutoResetEvent ProcessXmppMessage(XmppStreamElement node) {
            if (node != null)
            {
                Debug.WriteLine(node.ToString());

                if (node.OpensXmppStream)
                {
                    OnXmppStreamInitializedSubject.OnNext(node.ToString());
                }
                else if (node.ClosesXmppStream)
                {
                    OnXmppStreamClosedSubject.OnNext(node.ToString());
                }
                else
                {
                    var message = XmppSerializer.Deserialize(node.Name, node.ToString());

                    if (message != null)
                    {
                        if (message is Proceed)
                        {
                            return tlsProceedEvent;
                        }

                        OnMessageReceivedSubject.OnNext(message);
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///   Validation of the remote X509 Certificate ( on SSL/TLS connections only )
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "certificate"></param>
        /// <param name = "chain"></param>
        /// <param name = "sslPolicyErrors"></param>
        /// <returns></returns>
        private bool RemoteCertificateValidation(object sender,
                                                 System.Security.Cryptography.X509Certificates.X509Certificate
                                                     certificate,
                                                 System.Security.Cryptography.X509Certificates.X509Chain chain,
                                                 SslPolicyErrors sslPolicyErrors) {
            // TODO: Give the option to make this handled by the application using the library
            return true;
        }

        /// <summary>
        ///   Initializes the connection instance
        /// </summary>
        private void Initialize() {
            inputBuffer = new XmppMemoryStream();
            parser = new XmppStreamParser(inputBuffer);
            tlsProceedEvent = new AutoResetEvent(false);
        }
    }
}