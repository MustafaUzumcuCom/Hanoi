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
using BabelIm.Net.Xmpp.Serialization;
using BabelIm.Net.Xmpp.Serialization.Core.Tls;
using Org.Mentalis.Network.ProxySocket;

namespace BabelIm.Net.Xmpp.Core.Transports
{
    /// <summary>
    /// TCP/IP Transport implementation
    /// </summary>
    internal sealed class TcpTransport
        : BaseTransport, ISecureTransport
    {
        #region · Constants ·

        const string StreamNamespace    = "jabber:client";
        const string StreamURI          = "http://etherx.jabber.org/streams";
        const string StreamVersion      = "1.0";
        const string StreamFormat       = "<?xml version='1.0' encoding='UTF-8' ?><stream:stream xmlns='{0}' xmlns:stream='{1}' to='{2}' version='{3}'>";
        const string EndStream          = "</stream:stream>";

        #endregion

        #region · Fields ·

        private ProxySocket         socket;
        private System.IO.Stream    networkStream;
        private XmppMemoryStream	inputBuffer;
        private XmppStreamParser	parser;
        private AutoResetEvent      tlsProceedEvent;

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <b>XmppConnection</b> class.
        /// </summary>
        public TcpTransport()
        {
        }

        #endregion
        
        #region · Methods ·

        /// <summary>
        /// Opens the connection
        /// </summary>
        /// <param name="connectionString">The connection string used for authentication.</param>
        public override void Open(XmppConnectionString connectionString)
        {
            // Connection string
            this.ConnectionString   = connectionString;
            this.UserId             = this.ConnectionString.UserId;

            // Initialization
            this.Initialize();

            // Connect to the server
            this.Connect();

            // Begin Receiving Data
            this.BeginReceiveData();
        }

        /// <summary>
        /// Closes the connection
        /// </summary>
        public override void Close()
        {
            if (!this.IsDisposed)
            {
                try
                {
                    this.Send(EndStream);
                }
                catch
                {
                }
                finally
                {
                    if (this.tlsProceedEvent != null)
                    {
                        this.tlsProceedEvent.Set();
                        this.tlsProceedEvent = null;
                    }

                    if (this.networkStream != null)
                    {
                        this.networkStream.Dispose();
                        this.networkStream = null;
                    }

                    if (this.socket != null)
                    {
                        this.socket.Close();
                        this.socket = null;
                    }

                    if (this.inputBuffer != null)
                    {
                        this.inputBuffer.Dispose();
                        this.inputBuffer = null;
                    }

                    if (this.parser != null)
                    {
                        this.parser.Dispose();
                        this.parser = null;
                    }
                }

                base.Close();
            }
        }

        /// <summary>
        /// Sends a new message.
        /// </summary>
        /// <param elementname="message">The message to be sent</param>
        public override void Send(object message)
        {
            this.Send(XmppSerializer.Serialize(message));
        }

        /// <summary>
        /// Sends an XMPP message string to the XMPP Server
        /// </summary>
        /// <param name="value"></param>
        public override void Send(string value)
        {
            this.Send(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// Sends an XMPP message buffer to the XMPP Server
        /// </summary>
        public override void Send(byte[] buffer)
        {
            lock (this.SyncWrites)
            {
                Debug.WriteLine(Encoding.UTF8.GetString(buffer, 0, buffer.Length));

                this.networkStream.Write(buffer, 0, buffer.Length);
                this.networkStream.Flush();
            }
        }

        /// <summary>
        /// Initializes the XMPP stream.
        /// </summary>
        public override void InitializeXmppStream()
        {
            // Serialization can't be used in this case
            string xml = String.Format
            (
                StreamFormat,
                StreamNamespace,
                StreamURI,
                this.UserId.DomainName,
                StreamVersion
            );

            this.Send(xml);
        }

        #endregion

        #region · Secure Connection Methods ·

        /// <summary>
        /// Opens a secure connection against the XMPP server using TLS
        /// </summary>
        public void OpenSecureConnection()
        {
            // Send Start TLS message
            StartTls tlsMsg = new StartTls();
            this.Send(tlsMsg);

            this.tlsProceedEvent.WaitOne();

            this.OpenSecureStream();

            // Clear the Input Buffer
            this.inputBuffer.Clear();

            // Re-Start Async Reads
            this.BeginReceiveData();

            // Re-Initialize XMPP Stream
            this.InitializeXmppStream();
        }

        private void OpenSecureStream()
        {
            if (this.networkStream != null)
            {
                this.networkStream.Close();
                this.networkStream = null;
            }

            // Initialize the Ssl Stream
            this.networkStream = new SslStream
            (
                new NetworkStream(this.socket, false),
                false,
                new RemoteCertificateValidationCallback(RemoteCertificateValidation)
            );

            // Perform SSL Authentication
            ((SslStream)this.networkStream).AuthenticateAsClient
            (
                this.ConnectionString.HostName,
                null,
                SslProtocols.Tls,
                false
            );
        }

        #endregion

        #region · Connect Method ·
        
        /// <summary>
        /// Opens the connection to the XMPP server.
        /// </summary>
        private void Connect()
        {
            try
            {
                if (this.ConnectionString.ResolveHostName)
                {
                    base.ResolveHostName();
                }

                IPAddress   hostadd         = Dns.GetHostEntry(this.HostName).AddressList[0];
                IPEndPoint  hostEndPoint    = new IPEndPoint(hostadd, this.ConnectionString.PortNumber);

                this.socket = new ProxySocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                if (this.ConnectionString.UseProxy)
                {
                    IPAddress   proxyadd        = Dns.GetHostEntry(this.ConnectionString.ProxyServer).AddressList[0];
                    IPEndPoint  proxyEndPoint   = new IPEndPoint(proxyadd, this.ConnectionString.ProxyPortNumber);

                    switch (this.ConnectionString.ProxyType)
                    {
                        case "SOCKS4":
                            this.socket.ProxyType = ProxyTypes.Socks4;
                            break;

                        case "SOCKS5":
                            this.socket.ProxyType = ProxyTypes.Socks5;
                            break;

                        default:
                            this.socket.ProxyType = ProxyTypes.None;
                            break;
                    }

                    this.socket.ProxyEndPoint   = proxyEndPoint;
                    this.socket.ProxyUser       = this.ConnectionString.ProxyUserName;

                    if (!String.IsNullOrWhiteSpace(this.ConnectionString.ProxyPassword))
                    {
                        this.socket.ProxyPass = this.ConnectionString.ProxyPassword;
                    }
                }                

                // Disables the Nagle algorithm for send coalescing.
                this.socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);

                // Make the socket to connect to the Server
                this.socket.Connect(hostEndPoint);

                // Create the Stream Instance
                this.networkStream = new NetworkStream(this.socket, false);
            }
            catch (Exception ex)
            {
                throw new XmppException(String.Format("Unable to connect to XMPP server {0}", this.ConnectionString.HostName), ex);
            }
        }

        #endregion

        #region · Private Read Methods ·

        /// <summary>
        /// Startds async readings on the socket connected to the server
        /// </summary>
        private void BeginReceiveData() 
        {
            StateObject     state           = new StateObject(this.networkStream);
            AsyncCallback   asyncCallback   = new AsyncCallback(ReceiveCallback);

            // Begin receiving the data from the remote device.
            IAsyncResult ar = state.WorkStream.BeginRead(state.Buffer, 0, state.Buffer.Length, asyncCallback, state);

            if (ar.CompletedSynchronously)
            {
                this.ProcessAsyncRead(ar);
            }
        }

        /// <summary>
        /// Async read callback
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            if (!ar.CompletedSynchronously)
            {
                this.ProcessAsyncRead(ar);
            }
        }

        /// <summary>
        /// Assync read processing.
        /// </summary>
        /// <param name="ar"></param>
        private void ProcessAsyncRead(IAsyncResult ar)
        {
            // Retrieve the state object and the client socket 
            // from the asynchronous state object.
            StateObject             state       = (StateObject)ar.AsyncState;
            List<AutoResetEvent>    resetEvents = null;

            if (state.WorkStream != null && state.WorkStream.CanRead)
            {
                // Read data from the remote device.
                int bytesRead = state.WorkStream.EndRead(ar);

                if (bytesRead > 0)
                {
                    Monitor.Enter(this.SyncReads);

                    long currentPosition = this.inputBuffer.Position;

                    this.inputBuffer.Seek(0, SeekOrigin.End);
                    this.inputBuffer.Write(state.Buffer, 0, bytesRead);
                    this.inputBuffer.Flush();
                    this.inputBuffer.Seek(currentPosition, SeekOrigin.Begin);
                    
                    Monitor.Exit(this.SyncReads);

                    resetEvents = this.ProcessPendingMessages();

                    if (resetEvents != null && resetEvents.Count > 0)
                    {
                        foreach (AutoResetEvent resetEvent in resetEvents)
                        {
                            resetEvent.Set();
                        }
                    }
                    else if (!this.IsDisposed)
                    {
                        this.BeginReceiveData();
                    }
                }
            }
        }

        #endregion

        #region · Message Handling ·

        /// <summary>
        /// Process all pending XMPP messages
        /// </summary>
        /// <returns></returns>
        private List<AutoResetEvent> ProcessPendingMessages()
        {
            List<AutoResetEvent> resetEvents = new List<AutoResetEvent>();

            while (this.parser != null && !this.parser.EOF)
            {
                AutoResetEvent resetEvent = this.ProcessXmppMessage(this.parser.ReadNextNode());

                if (resetEvent != null)
                {
                    resetEvents.Add(resetEvent);
                }
            }

            return resetEvents;
        }

        /// <summary>
        /// Procesa an arbitrary XMPP Message
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private AutoResetEvent ProcessXmppMessage(XmppStreamElement node)
        {
            if (node != null)
            {
                Debug.WriteLine(node.ToString());

                if (node.OpensXmppStream)
                {
                    this.OnXmppStreamInitializedSubject.OnNext(node.ToString());
                }
                else if (node.ClosesXmppStream)
                {
                    this.OnXmppStreamClosedSubject.OnNext(node.ToString());
                }
                else
                {
                    object message = XmppSerializer.Deserialize(node.Name, node.ToString());

                    if (message != null)
                    {
                        if (message is Proceed)
                        {
                            return this.tlsProceedEvent;
                        }
                        else
                        {
                            this.OnMessageReceivedSubject.OnNext(message);
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region · SSL/TLS Callbacks ·

        /// <summary>
        /// Validation of the remote X509 Certificate ( on SSL/TLS connections only )
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private bool RemoteCertificateValidation(object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
#warning Give the option to make this handled by the application using the library
            return true;
        }

        #endregion

        #region · Private Methods ·

        /// <summary>
        /// Initializes the connection instance
        /// </summary>
        private void Initialize()
        {
            this.inputBuffer        = new XmppMemoryStream();
            this.parser             = new XmppStreamParser(this.inputBuffer);
            this.tlsProceedEvent    = new AutoResetEvent(false);
        }

        #endregion
    }
}
