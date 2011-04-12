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

namespace Org.Mentalis.Network.ProxySocket.Authentication
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// This class implements the 'username/password authentication' scheme.
    /// </summary>
    internal sealed class AuthUserPass 
        : AuthMethod
    {
        #region · Fields ·

        // private variables
        /// <summary>Holds the value of the Username property.</summary>
        private string username;
        
        /// <summary>Holds the value of the Password property.</summary>
        private string password;

        #endregion

        #region · Private Properties ·

        /// <summary>
        /// Gets or sets the username to use when authenticating with the proxy server.
        /// </summary>
        /// <value>The username to use when authenticating with the proxy server.</value>
        /// <exception cref="ArgumentNullException">The specified value is null.</exception>
        private string Username
        {
            get { return this.username; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                
                this.username = value;
            }
        }

        /// <summary>
        /// Gets or sets the password to use when authenticating with the proxy server.
        /// </summary>
        /// <value>The password to use when authenticating with the proxy server.</value>
        /// <exception cref="ArgumentNullException">The specified value is null.</exception>
        private string Password
        {
            get { return this.password; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                this.password = value;
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new AuthUserPass instance.
        /// </summary>
        /// <param name="server">The socket connection with the proxy server.</param>
        /// <param name="user">The username to use.</param>
        /// <param name="pass">The password to use.</param>
        /// <exception cref="ArgumentNullException"><c>user</c> -or- <c>pass</c> is null.</exception>
        public AuthUserPass(Socket server, string user, string pass)
            : base(server)
        {
            this.Username = user;
            this.Password = pass;
        }

        #endregion

        #region · Methods ·
        
        /// <summary>
        /// Starts the authentication process.
        /// </summary>
        public override void Authenticate()
        {
            byte[]  buffer      = new byte[2];
            int     received    = 0;

            this.Server.Send(this.GetAuthenticationBytes());

            while (received != 2)
            {
                received += this.Server.Receive(buffer, received, 2 - received, SocketFlags.None);
            }
            
            if (buffer[1] != 0)
            {
                Server.Close();
                throw new ProxyException("Username/password combination rejected.");
            }
        }

        /// <summary>
        /// Starts the asynchronous authentication process.
        /// </summary>
        /// <param name="callback">The method to call when the authentication is complete.</param>
        public override void BeginAuthenticate(HandShakeComplete callback)
        {
            this.CallBack = callback;
            this.Server.BeginSend(this.GetAuthenticationBytes(), 0, 3 + this.Username.Length + this.Password.Length, SocketFlags.None, new AsyncCallback(this.OnSent), this.Server);
        }

        #endregion

        #region · Private Methods ·

        /// <summary>
        /// Creates an array of bytes that has to be sent if the user wants to authenticate with the username/password authentication scheme.
        /// </summary>
        /// <returns>An array of bytes that has to be sent if the user wants to authenticate with the username/password authentication scheme.</returns>
        private byte[] GetAuthenticationBytes()
        {
            byte[] buffer = new byte[3 + Username.Length + Password.Length];

            buffer[0] = 1;
            buffer[1] = (byte)Username.Length;

            Array.Copy(Encoding.ASCII.GetBytes(this.Username), 0, buffer, 2, this.Username.Length);
            buffer[Username.Length + 2] = (byte)Password.Length;
            Array.Copy(Encoding.ASCII.GetBytes(this.Password), 0, buffer, this.Username.Length + 3, this.Password.Length);

            return buffer;
        }

        /// <summary>
        /// Called when the authentication bytes have been sent.
        /// </summary>
        /// <param name="ar">Stores state information for this asynchronous operation as well as any user-defined data.</param>
        private void OnSent(IAsyncResult ar)
        {
            try
            {
                this.Server.EndSend(ar);
                this.Buffer = new byte[2];
                this.Server.BeginReceive(this.Buffer, 0, 2, SocketFlags.None, new AsyncCallback(this.OnReceive), this.Server);
            }
            catch (Exception e)
            {
                this.CallBack(e);
            }
        }

        /// <summary>
        /// Called when the socket received an authentication reply.
        /// </summary>
        /// <param name="ar">Stores state information for this asynchronous operation as well as any user-defined data.</param>
        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                this.Received += this.Server.EndReceive(ar);

                if (this.Received == this.Buffer.Length)
                {
                    if (this.Buffer[1] == 0)
                    {
                        this.CallBack(null);
                    }
                    else
                    {
                        throw new ProxyException("Username/password combination not accepted.");
                    }
                }
                else
                {
                    this.Server.BeginReceive(this.Buffer, this.Received, this.Buffer.Length - this.Received, SocketFlags.None, new AsyncCallback(this.OnReceive), this.Server);
                }
            }
            catch (Exception e)
            {
                this.CallBack(e);
            }
        }

        #endregion
    }
}