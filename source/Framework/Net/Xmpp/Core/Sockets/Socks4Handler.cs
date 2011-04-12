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

    /// <summary>
    /// Implements the SOCKS4[A] protocol.
    /// </summary>
    internal sealed class Socks4Handler
        : SocksHandler
    {
        #region · Constructors ·

        /// <summary>
        /// Initilizes a new instance of the SocksHandler class.
        /// </summary>
        /// <param name="server">The socket connection with the proxy server.</param>
        /// <param name="user">The username to use when authenticating with the server.</param>
        /// <exception cref="ArgumentNullException"><c>server</c> -or- <c>user</c> is null.</exception>
        public Socks4Handler(Socket server, string user)
            : base(server, user)
        {
        }
        
        #endregion

        #region · Private Methods ·

        /// <summary>
        /// Creates an array of bytes that has to be sent when the user wants to connect to a specific host/port combination.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <returns>An array of bytes that has to be sent when the user wants to connect to a specific host/port combination.</returns>
        /// <remarks>Resolving the host name will be done at server side. Do note that some SOCKS4 servers do not implement this functionality.</remarks>
        /// <exception cref="ArgumentNullException"><c>host</c> is null.</exception>
        /// <exception cref="ArgumentException"><c>port</c> is invalid.</exception>
        private byte[] GetHostPortBytes(string host, int port)
        {
            if (host == null)
            {
                throw new ArgumentNullException();
            }
            if (port <= 0 || port > 65535)
            {
                throw new ArgumentException();
            }

            byte[] connect = new byte[10 + this.Username.Length + host.Length];
            connect[0] = 4;
            connect[1] = 1;
            Array.Copy(this.PortToBytes(port), 0, connect, 2, 2);
            connect[4] = connect[5] = connect[6] = 0;
            connect[7] = 1;
            Array.Copy(Encoding.ASCII.GetBytes(this.Username), 0, connect, 8, this.Username.Length);
            connect[8 + Username.Length] = 0;
            Array.Copy(Encoding.ASCII.GetBytes(host), 0, connect, 9 + this.Username.Length, host.Length);
            connect[9 + this.Username.Length + host.Length] = 0;

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

            byte[] connect = new byte[9 + this.Username.Length];
            connect[0] = 4;
            connect[1] = 1;
            Array.Copy(PortToBytes(remoteEP.Port), 0, connect, 2, 2);
            Array.Copy(AddressToBytes(remoteEP.Address.Address), 0, connect, 4, 4);
            Array.Copy(Encoding.ASCII.GetBytes(this.Username), 0, connect, 8, this.Username.Length);
            connect[8 + this.Username.Length] = 0;

            return connect;
        }

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
        public override void Negotiate(IPEndPoint remoteEP)
        {
            this.Negotiate(GetEndPointBytes(remoteEP));
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
        private void Negotiate(byte[] connect)
        {
            if (connect == null)
            {
                throw new ArgumentNullException();
            }
            if (connect.Length < 2)
            {
                throw new ArgumentException();
            }

            this.Server.Send(connect);
            byte[] buffer = this.ReadBytes(8);

            if (buffer[1] != 90)
            {
                this.Server.Close();
                throw new ProxyException("Negotiation failed.");
            }
        }

        /// <summary>
        /// Starts negotiating asynchronously with a SOCKS proxy server.
        /// </summary>
        /// <param name="host">The remote server to connect to.</param>
        /// <param name="port">The remote port to connect to.</param>
        /// <param name="callback">The method to call when the connection has been established.</param>
        /// <param name="proxyEndPoint">The IPEndPoint of the SOCKS proxy server.</param>
        /// <returns>An IAsyncProxyResult that references the asynchronous connection.</returns>
        public override IAsyncProxyResult BeginNegotiate(string host, int port, HandShakeComplete callback, IPEndPoint proxyEndPoint)
        {
            this.ProtocolComplete   = callback;
            this.Buffer             = this.GetHostPortBytes(host, port);

            this.Server.BeginConnect(proxyEndPoint, new AsyncCallback(this.OnConnect), this.Server);

            this.AsyncResult = new IAsyncProxyResult();

            return this.AsyncResult;
        }

        /// <summary>
        /// Starts negotiating asynchronously with a SOCKS proxy server.
        /// </summary>
        /// <param name="remoteEP">An IPEndPoint that represents the remote device.</param>
        /// <param name="callback">The method to call when the connection has been established.</param>
        /// <param name="proxyEndPoint">The IPEndPoint of the SOCKS proxy server.</param>
        /// <returns>An IAsyncProxyResult that references the asynchronous connection.</returns>
        public override IAsyncProxyResult BeginNegotiate(IPEndPoint remoteEP, HandShakeComplete callback, IPEndPoint proxyEndPoint)
        {
            this.ProtocolComplete   = callback;
            this.Buffer             = this.GetEndPointBytes(remoteEP);

            this.Server.BeginConnect(proxyEndPoint, new AsyncCallback(this.OnConnect), this.Server);

            this.AsyncResult = new IAsyncProxyResult();
            
            return this.AsyncResult;
        }

        /// <summary>
        /// Called when the Socket is connected to the remote proxy server.
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
                this.Server.BeginSend(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, new AsyncCallback(this.OnSent), this.Server);
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
            }
        }

        /// <summary>
        /// Called when the Socket has sent the handshake data.
        /// </summary>
        /// <param name="ar">Stores state information for this asynchronous operation as well as any user-defined data.</param>
        private void OnSent(IAsyncResult ar)
        {
            try
            {
                if (this.Server.EndSend(ar) < this.Buffer.Length)
                {
                    this.ProtocolComplete(new SocketException());
                    return;
                }
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
                return;
            }

            try
            {
                this.Buffer     = new byte[8];
                this.Received   = 0;

                this.Server.BeginReceive(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, new AsyncCallback(this.OnReceive), this.Server);
            }
            catch (Exception e)
            {
                this.ProtocolComplete(e);
            }
        }

        /// <summary>
        /// Called when the Socket has received a reply from the remote proxy server.
        /// </summary>
        /// <param name="ar">Stores state information for this asynchronous operation as well as any user-defined data.</param>
        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                int received = this.Server.EndReceive(ar);

                if (received <= 0)
                {
                    this.ProtocolComplete(new SocketException());
                    return;
                }
                
                this.Received += received;

                if (this.Received == 8)
                {
                    if (this.Buffer[1] == 90)
                    {
                        this.ProtocolComplete(null);
                    }
                    else
                    {
                        this.Server.Close();
                        this.ProtocolComplete(new ProxyException("Negotiation failed."));
                    }
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

        #endregion
    }
}