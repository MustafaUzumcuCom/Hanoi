using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Hanoi.Serialization;
using Hanoi.Serialization.Core.Tls;
using Hanoi.Sockets;

namespace Hanoi.Transports
{
    /// <summary>
    ///   TCP/IP Transport implementation
    /// </summary>
    internal sealed class TcpTransport : BaseTransport, ISecureTransport
    {
        private const string StreamNamespace = "jabber:client";
        private const string StreamURI = "http://etherx.jabber.org/streams";
        private const string StreamVersion = "1.0";
        private const string StreamFormat = "<?xml version='1.0' encoding='UTF-8' ?><stream:stream xmlns='{0}' xmlns:stream='{1}' to='{2}' version='{3}'>";
        private const string EndStream = "</stream:stream>";

        private XmppMemoryStream _inputBuffer;
        private Stream _networkStream;
        private XmppStreamParser _parser;
        private ProxySocket _socket;
        private AutoResetEvent _tlsProceedEvent;

        #region ISecureTransport Members

        /// <summary>
        ///   Opens the connection
        /// </summary>
        /// <param name = "connectionString">The connection string used for authentication.</param>
        public override void Open(ConnectionString connectionString)
        {
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
        public override void Close()
        {
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
                    if (_tlsProceedEvent != null)
                    {
                        _tlsProceedEvent.Set();
                        _tlsProceedEvent = null;
                    }

                    if (_networkStream != null)
                    {
                        _networkStream.Dispose();
                        _networkStream = null;
                    }

                    if (_socket != null)
                    {
                        _socket.Close();
                        _socket = null;
                    }

                    if (_inputBuffer != null)
                    {
                        _inputBuffer.Dispose();
                        _inputBuffer = null;
                    }

                    if (_parser != null)
                    {
                        _parser.Dispose();
                        _parser = null;
                    }
                }

                base.Close();
            }
        }

        /// <summary>
        ///   Sends a new message.
        /// </summary>
        /// <param name = "message">The message to be sent</param>
        public override void Send(object message)
        {
            Send(XmppSerializer.Serialize(message));
        }

        /// <summary>
        ///   Sends an XMPP message string to the XMPP Server
        /// </summary>
        /// <param name = "value"></param>
        public override void Send(string value)
        {
            Send(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        ///   Sends an XMPP message buffer to the XMPP Server
        /// </summary>
        public override void Send(byte[] buffer)
        {
            lock (SyncWrites)
            {
                Debug.WriteLine(Encoding.UTF8.GetString(buffer, 0, buffer.Length));

                _networkStream.Write(buffer, 0, buffer.Length);
                _networkStream.Flush();
            }
        }

        /// <summary>
        ///   Initializes the XMPP stream.
        /// </summary>
        public override void InitializeXmppStream()
        {
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
        public void OpenSecureConnection()
        {
            // Send Start TLS message
            var tlsMsg = new StartTls();
            Send(tlsMsg);

            _tlsProceedEvent.WaitOne();

            OpenSecureStream();

            // Clear the Input Buffer
            _inputBuffer.Clear();

            // Re-Start Async Reads
            BeginReceiveData();

            // Re-Initialize XMPP Stream
            InitializeXmppStream();
        }

        #endregion

        private void OpenSecureStream()
        {
            if (_networkStream != null)
            {
                _networkStream.Close();
                _networkStream = null;
            }

            // Initialize the Ssl Stream
            _networkStream = new SslStream
                (
                new NetworkStream(_socket, false),
                false,
                RemoteCertificateValidation
                );

            // Perform SSL Authentication
            ((SslStream) _networkStream).AuthenticateAsClient
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
        private void Connect()
        {
            try
            {
                if (ConnectionString.ResolveHostName)
                {
                    // ReSharper disable RedundantBaseQualifier
                    base.ResolveHostName();
                    // ReSharper restore RedundantBaseQualifier
                }

                IPAddress hostadd = Dns.GetHostEntry(HostName).AddressList[0];
                var hostEndPoint = new IPEndPoint(hostadd, ConnectionString.PortNumber);

                _socket = new ProxySocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                if (ConnectionString.UseProxy)
                {
                    IPAddress proxyadd = Dns.GetHostEntry(ConnectionString.ProxyServer).AddressList[0];
                    var proxyEndPoint = new IPEndPoint(proxyadd, ConnectionString.ProxyPortNumber);

                    switch (ConnectionString.ProxyType)
                    {
                        case "SOCKS4":
                            _socket.ProxyType = ProxyTypes.Socks4;
                            break;

                        case "SOCKS5":
                            _socket.ProxyType = ProxyTypes.Socks5;
                            break;

                        default:
                            _socket.ProxyType = ProxyTypes.None;
                            break;
                    }

                    _socket.ProxyEndPoint = proxyEndPoint;
                    _socket.ProxyUser = ConnectionString.ProxyUserName;

                    if (!String.IsNullOrWhiteSpace(ConnectionString.ProxyPassword))
                    {
                        _socket.ProxyPass = ConnectionString.ProxyPassword;
                    }
                }

                // Disables the Nagle algorithm for send coalescing.
                _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);

                // Make the socket to connect to the Server
                _socket.Connect(hostEndPoint);

                // Create the Stream Instance
                _networkStream = new NetworkStream(_socket, false);
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
        private void BeginReceiveData()
        {
            var state = new StateObject(_networkStream);
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
        private void ReceiveCallback(IAsyncResult ar)
        {
            if (!ar.CompletedSynchronously)
            {
                ProcessAsyncRead(ar);
            }
        }

        /// <summary>
        ///   Assync read processing.
        /// </summary>
        /// <param name = "ar"></param>
        private void ProcessAsyncRead(IAsyncResult ar)
        {
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

                    long currentPosition = _inputBuffer.Position;

                    _inputBuffer.Seek(0, SeekOrigin.End);
                    _inputBuffer.Write(state.Buffer, 0, bytesRead);
                    _inputBuffer.Flush();
                    _inputBuffer.Seek(currentPosition, SeekOrigin.Begin);

                    Monitor.Exit(SyncReads);

                    List<AutoResetEvent> resetEvents = ProcessPendingMessages();

                    if (resetEvents != null && resetEvents.Count > 0)
                    {
                        foreach (AutoResetEvent resetEvent in resetEvents)
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
        private List<AutoResetEvent> ProcessPendingMessages()
        {
            var resetEvents = new List<AutoResetEvent>();

            while (_parser != null && !_parser.EOF)
            {
                AutoResetEvent resetEvent = ProcessXmppMessage(_parser.ReadNextNode());

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
        private AutoResetEvent ProcessXmppMessage(XmppStreamElement node)
        {
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
                    object message = XmppSerializer.Deserialize(node.Name, node.ToString());

                    if (message != null)
                    {
                        if (message is Proceed)
                        {
                            return _tlsProceedEvent;
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
        private bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // TODO: Give the option to make this handled by the application using the library
            return true;
        }

        /// <summary>
        ///   Initializes the connection instance
        /// </summary>
        private void Initialize()
        {
            _inputBuffer = new XmppMemoryStream();
            _parser = new XmppStreamParser(_inputBuffer);
            _tlsProceedEvent = new AutoResetEvent(false);
        }
    }
}