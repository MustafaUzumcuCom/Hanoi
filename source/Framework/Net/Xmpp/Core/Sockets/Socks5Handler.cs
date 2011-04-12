/*
    Copyright © 2002, The KPD-Team
    All rights reserved.
    http://www.mentalis.org/

  Redistribution and use in source and binary forms, with or without
  modification, are permitted provided that the following conditions
  are met:

    - Redistributions of source code must retain the above copyright
       notice, this list of conditions and the following disclaimer. 

    - Neither the name of the KPD-Team, nor the names of its contributors
       may be used to endorse or promote products derived from this
       software without specific prior written permission. 

  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
  FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
  THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
  HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
  STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
  OF THE POSSIBILITY OF SUCH DAMAGE.
*/

namespace Org.Mentalis.Network.ProxySocket
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using Org.Mentalis.Network.ProxySocket.Authentication;

    /// <summary>
    /// Implements the SOCKS5 protocol.
    /// </summary>
    internal sealed class Socks5Handler
        : SocksHandler
    {
        #region · Fields ·

        /// <summary>Holds the value of the Password property.</summary>
        private string m_Password;

        /// <summary>Holds the value of the HandShake property.</summary>
        private byte[] m_HandShake;

        #endregion

        #region · Private Properties ·

        /// <summary>
        /// Gets or sets the password to use when authenticating with the SOCKS5 server.
        /// </summary>
        /// <value>The password to use when authenticating with the SOCKS5 server.</value>
        private string Password
        {
            get { return this.m_Password; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                this.m_Password = value;
            }
        }

        /// <summary>
        /// Gets or sets the bytes to use when sending a connect request to the proxy server.
        /// </summary>
        /// <value>The array of bytes to use when sending a connect request to the proxy server.</value>
        private byte[] HandShake
        {
            get { return this.m_HandShake; }
            set { this.m_HandShake = value; }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initiliazes a new Socks5Handler instance.
        /// </summary>
        /// <param name="server">The socket connection with the proxy server.</param>
        /// <exception cref="ArgumentNullException"><c>server</c>  is null.</exception>
        public Socks5Handler(Socket server)
            : this(server, "")
        {
        }

        /// <summary>
        /// Initiliazes a new Socks5Handler instance.
        /// </summary>
        /// <param name="server">The socket connection with the proxy server.</param>
        /// <param name="user">The username to use.</param>
        /// <exception cref="ArgumentNullException"><c>server</c> -or- <c>user</c> is null.</exception>
        public Socks5Handler(Socket server, string user)
            : this(server, user, "")
        {
        }

        /// <summary>
        /// Initiliazes a new Socks5Handler instance.
        /// </summary>
        /// <param name="server">The socket connection with the proxy server.</param>
        /// <param name="user">The username to use.</param>
        /// <param name="pass">The password to use.</param>
        /// <exception cref="ArgumentNullException"><c>server</c> -or- <c>user</c> -or- <c>pass</c> is null.</exception>
        public Socks5Handler(Socket server, string user, string pass)
            : base(server, user)
        {
            this.Password = pass;
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Starts negotiating with the SOCKS server.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <exception cref="ArgumentNullException"><c>host</c> is null.</exception>
        /// <exception cref="ArgumentException"><c>port</c> is invalid.</exception>
        /// <exception cref="ProxyException">The proxy rejected the request.</exception>
        /// <exception cref="SocketException">An operating system error occurs while accessing the Socket.</exception>
        /// <exception cref="ObjectDisposedException">The Socket has been closed.</exception>
        /// <exception cref="ProtocolViolationException">The proxy server uses an invalid protocol.</exception>
        public override void Negotiate(string host, int port)
        {
            this.Negotiate(GetHostPortBytes(host, port));
        }

        /// <summary>
        /// Starts negotiating with the SOCKS server.
        /// </summary>
        /// <param name="remoteEP">The IPEndPoint to connect to.</param>
        /// <exception cref="ArgumentNullException"><c>remoteEP</c> is null.</exception>
        /// <exception cref="ProxyException">The proxy rejected the request.</exception>
        /// <exception cref="SocketException">An operating system error occurs while accessing the Socket.</exception>
        /// <exception cref="ObjectDisposedException">The Socket has been closed.</exception>
        /// <exception cref="ProtocolViolationException">The proxy server uses an invalid protocol.</exception>
        public override void Negotiate(IPEndPoint remoteEP)
        {
            this.Negotiate(GetEndPointBytes(remoteEP));
        }

        /// <summary>
        /// Starts negotiating asynchronously with the SOCKS server. 
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="callback">The method to call when the negotiation is complete.</param>
        /// <param name="proxyEndPoint">The IPEndPoint of the SOCKS proxy server.</param>
        /// <returns>An IAsyncProxyResult that references the asynchronous connection.</returns>
        public override IAsyncProxyResult BeginNegotiate(string host, int port, HandShakeComplete callback, IPEndPoint proxyEndPoint)
        {
            this.ProtocolComplete   = callback;
            this.HandShake          = this.GetHostPortBytes(host, port);

            this.Server.BeginConnect(proxyEndPoint, new AsyncCallback(this.OnConnect), this.Server);

            this.AsyncResult = new IAsyncProxyResult();

            return this.AsyncResult;
        }

        /// <summary>
        /// Starts negotiating asynchronously with the SOCKS server. 
        /// </summary>
        /// <param name="remoteEP">An IPEndPoint that represents the remote device.</param>
        /// <param name="callback">The method to call when the negotiation is complete.</param>
        /// <param name="proxyEndPoint">The IPEndPoint of the SOCKS proxy server.</param>
        /// <returns>An IAsyncProxyResult that references the asynchronous connection.</returns>
        public override IAsyncProxyResult BeginNegotiate(IPEndPoint remoteEP, HandShakeComplete callback, IPEndPoint proxyEndPoint)
        {
            this.ProtocolComplete   = callback;
            this.HandShake          = this.GetEndPointBytes(remoteEP);

            this.Server.BeginConnect(proxyEndPoint, new AsyncCallback(this.OnConnect), this.Server);

            this.AsyncResult = new IAsyncProxyResult();

            return this.AsyncResult;
        }

        #endregion

        #region · Private Methods ·

        /// <summary>
        /// Starts the synchronous authentication process.
        /// </summary>
        /// <exception cref="ProxyException">Authentication with the proxy server failed.</exception>
        /// <exception cref="ProtocolViolationException">The proxy server uses an invalid protocol.</exception>
        /// <exception cref="SocketException">An operating system error occurs while accessing the Socket.</exception>
        /// <exception cref="ObjectDisposedException">The Socket has been closed.</exception>
        private void Authenticate()
        {
            this.Server.Send(new byte[] { 5, 2, 0, 2 });
            byte[] buffer = this.ReadBytes(2);

            if (buffer[1] == 255)
            {
                throw new ProxyException("No authentication method accepted.");
            }

            AuthMethod authenticate;

            switch (buffer[1])
            {
                case 0:
                    authenticate = new AuthNone(this.Server);
                    break;

                case 2:
                    authenticate = new AuthUserPass(this.Server, this.Username, this.Password);
                    break;

                default:
                    throw new ProtocolViolationException();
            }

            authenticate.Authenticate();
        }

        /// <summary>
        /// Creates an array of bytes that has to be sent when the user wants to connect to a specific host/port combination.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <returns>An array of bytes that has to be sent when the user wants to connect to a specific host/port combination.</returns>
        /// <exception cref="ArgumentNullException"><c>host</c> is null.</exception>
        /// <exception cref="ArgumentException"><c>port</c> or <c>host</c> is invalid.</exception>
        private byte[] GetHostPortBytes(string host, int port)
        {
            if (host == null)
            {
                throw new ArgumentNullException();
            }
            if (port <= 0 || port > 65535 || host.Length > 255)
            {
                throw new ArgumentException();
            }

            byte[] connect = new byte[7 + host.Length];
            connect[0] = 5;
            connect[1] = 1;
            connect[2] = 0; //reserved
            connect[3] = 3;
            connect[4] = (byte)host.Length;
            Array.Copy(Encoding.ASCII.GetBytes(host), 0, connect, 5, host.Length);
            Array.Copy(this.PortToBytes(port), 0, connect, host.Length + 5, 2);
            
            return connect;
        }

        /// <summary>
        /// Creates an array of bytes that has to be sent when the user wants to connect to a specific IPEndPoint.
        /// </summary>
        /// <param name="remoteEP">The IPEndPoint to connect to.</param>
        /// <returns>An array of bytes that has to be sent when the user wants to connect to a specific IPEndPoint.</returns>
        /// <exception cref="ArgumentNullException"><c>remoteEP</c> is null.</exception>
        private byte[] GetEndPointBytes(IPEndPoint remoteEP)
        {
            if (remoteEP == null)
            {
                throw new ArgumentNullException();
            }

            byte[] connect = new byte[10];
            connect[0] = 5;
            connect[1] = 1;
            connect[2] = 0; //reserved
            connect[3] = 1;
            Array.Copy(this.AddressToBytes(remoteEP.Address.Address), 0, connect, 4, 4);
            Array.Copy(this.PortToBytes(remoteEP.Port), 0, connect, 8, 2);
            
            return connect;
        }

        /// <summary>
        /// Starts negotiating with the SOCKS server.
        /// </summary>
        /// <param name="connect">The bytes to send when trying to authenticate.</param>
        /// <exception cref="ArgumentNullException"><c>connect</c> is null.</exception>
        /// <exception cref="ArgumentException"><c>connect</c> is too small.</exception>
        /// <exception cref="ProxyException">The proxy rejected the request.</exception>
        /// <exception cref="SocketException">An operating system error occurs while accessing the Socket.</exception>
        /// <exception cref="ObjectDisposedException">The Socket has been closed.</exception>
        /// <exception cref="ProtocolViolationException">The proxy server uses an invalid protocol.</exception>
        private void Negotiate(byte[] connect)
        {
            this.Authenticate();
            this.Server.Send(connect);

            byte[] buffer = this.ReadBytes(4);

            if (buffer[1] != 0)
            {
                this.Server.Close();
                throw new ProxyException(buffer[1]);
            }

            switch (buffer[3])
            {
                case 1:
                    buffer = this.ReadBytes(6); //IPv4 address with port
                    break;

                case 3:
                    buffer = this.ReadBytes(1);
                    buffer = this.ReadBytes(buffer[0] + 2); //domain name with port
                    break;

                case 4:
                    buffer = this.ReadBytes(18); //IPv6 address with port
                    break;

                default:
                    this.Server.Close();
                    throw new ProtocolViolationException();
            }
        }

        /// <summary>
        /// Called when the socket is connected to the remote server.
        /// </summary>
        /// <param name="ar">Stores state information for this asynchronous operation as well as any user-defined data.</param>
        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                this.Server.EndConnect(ar);
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
                return;
            }

            try
            {
                this.Server.BeginSend(new byte[] { 5, 2, 0, 2 }, 0, 4, SocketFlags.None, new AsyncCallback(this.OnAuthSent), this.Server);
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
            }
        }

        /// <summary>
        /// Called when the authentication bytes have been sent.
        /// </summary>
        /// <param name="ar">Stores state information for this asynchronous operation as well as any user-defined data.</param>
        private void OnAuthSent(IAsyncResult ar)
        {
            try
            {
                this.Server.EndSend(ar);
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
                return;
            }

            try
            {
                this.Buffer = new byte[1024];
                this.Received = 0;
                this.Server.BeginReceive(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, new AsyncCallback(this.OnAuthReceive), this.Server);
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
            }
        }

        /// <summary>
        /// Called when an authentication reply has been received.
        /// </summary>
        /// <param name="ar">Stores state information for this asynchronous operation as well as any user-defined data.</param>
        private void OnAuthReceive(IAsyncResult ar)
        {
            try
            {
                this.Received += this.Server.EndReceive(ar);

                if (Received <= 0)
                {
                    throw new SocketException();
                }
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
                return;
            }

            try
            {
                if (this.Received < 2)
                {
                    this.Server.BeginReceive(this.Buffer, this.Received, this.Buffer.Length - this.Received, SocketFlags.None, new AsyncCallback(this.OnAuthReceive), this.Server);
                }
                else
                {
                    AuthMethod authenticate;

                    switch (this.Buffer[1])
                    {
                        case 0:
                            authenticate = new AuthNone(this.Server);
                            break;

                        case 2:
                            authenticate = new AuthUserPass(this.Server, this.Username, this.Password);
                            break;

                        default:
                            this.ProtocolComplete(new SocketException());
                            return;
                    }

                    authenticate.BeginAuthenticate(new HandShakeComplete(this.OnAuthenticated));
                }
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
            }
        }

        /// <summary>
        /// Called when the socket has been successfully authenticated with the server.
        /// </summary>
        /// <param name="e">The exception that has occured while authenticating, or <em>null</em> if no error occured.</param>
        private void OnAuthenticated(Exception e)
        {
            if (e != null)
            {
                this.ProtocolComplete(e);
                return;
            }
            try
            {
                this.Server.BeginSend(this.HandShake, 0, this.HandShake.Length, SocketFlags.None, new AsyncCallback(this.OnSent), this.Server);
            }
            catch (Exception ex)
            {
                this.ProtocolComplete(ex);
            }
        }

        /// <summary>
        /// Called when the connection request has been sent.
        /// </summary>
        /// <param name="ar">Stores state information for this asynchronous operation as well as any user-defined data.</param>
        private void OnSent(IAsyncResult ar)
        {
            try
            {
                this.Server.EndSend(ar);
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
                return;
            }

            try
            {
                this.Buffer = new byte[5];
                this.Received = 0;
                this.Server.BeginReceive(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, new AsyncCallback(this.OnReceive), this.Server);
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
            }
        }

        /// <summary>
        /// Called when a connection reply has been received.
        /// </summary>
        /// <param name="ar">Stores state information for this asynchronous operation as well as any user-defined data.</param>
        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                this.Received += this.Server.EndReceive(ar);
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
                return;
            }

            try
            {
                if (this.Received == this.Buffer.Length)
                {
                    this.ProcessReply(Buffer);
                }
                else
                {
                    this.Server.BeginReceive(this.Buffer, this.Received, this.Buffer.Length - this.Received, SocketFlags.None, new AsyncCallback(this.OnReceive), this.Server);
                }
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
            }
        }

        /// <summary>
        /// Processes the received reply.
        /// </summary>
        /// <param name="buffer">The received reply</param>
        /// <exception cref="ProtocolViolationException">The received reply is invalid.</exception>
        private void ProcessReply(byte[] buffer)
        {
            switch (buffer[3])
            {
                case 1:
                    this.Buffer = new byte[5]; //IPv4 address with port - 1 byte
                    break;

                case 3:
                    this.Buffer = new byte[buffer[4] + 2]; //domain name with port
                    break;

                case 4:
                    buffer = new byte[17]; //IPv6 address with port - 1 byte
                    break;

                default:
                    throw new ProtocolViolationException();
            }
            
            this.Received = 0;
            this.Server.BeginReceive(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, new AsyncCallback(this.OnReadLast), this.Server);
        }

        /// <summary>
        /// Called when the last bytes are read from the socket.
        /// </summary>
        /// <param name="ar">Stores state information for this asynchronous operation as well as any user-defined data.</param>
        private void OnReadLast(IAsyncResult ar)
        {
            try
            {
                this.Received += this.Server.EndReceive(ar);
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
                return;
            }

            try
            {
                if (this.Received == this.Buffer.Length)
                {
                    this.ProtocolComplete(null);
                }
                else
                {
                    this.Server.BeginReceive(this.Buffer, this.Received, this.Buffer.Length - this.Received, SocketFlags.None, new AsyncCallback(this.OnReadLast), this.Server);
                }
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
            }
        }

        #endregion
    }
}