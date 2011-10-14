/*
    Copyright (c) 2007 - 2009, Carlos Guzmán Álvarez

    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, 
    are permitted provided that the following conditions are met:

        * Redistributions of source code must retain the above copyright notice, 
          this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, 
          this list of conditions and the following disclaimer in the documentation and/or 
          other materials provided with the distribution.
        * Neither the name of the author nor the names of its contributors may be used to endorse or 
          promote products derived from this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
    CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
    EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Framework.Net.Xmpp.Serialization.InstantMessaging.Client;
using Framework.Net.Xmpp.Serialization.InstantMessaging.Jabber;

namespace Framework.Net.Xmpp.Core
{
    /// <summary>
    /// <see cref="XmppAuthenticator" /> implementation for the Jabber Authentication mechanism.
    /// (Non-SASL authentication)
    /// </summary>
    /// <remarks>
    /// References:
    ///     http://www.jabber.org/jeps/jep-0078.html
    /// </remarks>
    internal sealed class XmppJabberAuthenticator : XmppAuthenticator
    {
        #region · Fields ·

        private JabberAuthQuery authInfo;
        private AutoResetEvent	authInfoEvent;

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppJabberAuthenticator"/> class.
        /// </summary>
        public XmppJabberAuthenticator(XmppConnection connection) 
            : base(connection)
        {
            this.authInfoEvent = new AutoResetEvent(false);
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Performs the authentication using the Jabber mechanism.
        /// </summary>
        public override void Authenticate()
        {
            this.RequestAuthInformation();

            // Do authentication
            JabberAuthQuery auth = new JabberAuthQuery();

            if (authInfo.UserName != null)
            {
                auth.UserName = this.Connection.UserId.UserName;
            }
            if (authInfo.Resource != null)
            {
                auth.Resource = this.Connection.UserId.ResourceName;
            }
            if (authInfo.Digest != null)
            {
                auth.Digest = this.GenerateDigest(this.Connection.UserPassword);
            }
            else if (authInfo.Password != null)
            {
                auth.Password = this.Connection.UserPassword;
            }			
            
            IQ iq	= new IQ();
            iq.Type	= IQType.Set;
            iq.ID   = XmppIdentifierGenerator.Generate();

            iq.Items.Add(auth);

            this.PendingMessages.Add(iq.ID);

            this.Connection.Send(iq);		
        }

        #endregion

        #region · Protected Methods ·

        /// <summary>
        /// Called when an unhandled message is received
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:Framework.Net.Xmpp.Core.XmppUnhandledMessageEventArgs"/> instance containing the event data.</param>
        protected override void OnUnhandledMessage(object sender, XmppUnhandledMessageEventArgs e)
        {
            if (e.StanzaInstance is IQ)
            {
                IQ response = (IQ)e.StanzaInstance;

                if (this.PendingMessages.Contains(response.ID))
                {
                    this.PendingMessages.Remove(response.ID);

                    if (this.authInfo == null)
                    {
                        // Response to teh Authentication Information Request
                        this.authInfo = (JabberAuthQuery)response.Items[0];
                        this.authInfoEvent.Set();
                    }
                }
            }
        }

        #endregion

        #region · Private Methods ·

        private void RequestAuthInformation()
        {
            JabberAuthQuery auth    = new JabberAuthQuery();		            
            IQ              request	= new IQ();

            auth.UserName   = this.Connection.UserId.UserName;
            request.To		= this.Connection.Server;
            request.Type	= IQType.Get;
            request.ID      = XmppIdentifierGenerator.Generate();

            request.Items.Add(auth);

            this.Connection.Send(request);

            // Wait for Response. It will be received in async mode
            this.authInfoEvent.WaitOne();
        }

        /// <summary>
        /// Calculates the password digest.
        /// </summary>
        /// <param elementname="password">Password</param>
        /// <returns>Digest for the specified password.</returns>
        /// <remarks>
        /// References:
        ///		http://www.jabber.org/jeps/jep-0078.html#nt-id2602055
        /// </remarks>
        private string GenerateDigest(string password)
        {
            /*
            The value of the <digest/> element MUST be computed according 
            to the following algorithm:

            1. Concatenate the Stream ID received from the server with the password. [8]
            2. Hash the concatenated string according to the SHA1 algorithm, i.e., SHA1(concat(sid, password)).
            3. Ensure that the hash output is in hexidecimal format, not binary or base64.
            4. Convert the hash output to all lowercase characters.
            */

            byte[] buffer = Encoding.UTF8.GetBytes(this.Connection.StreamIdentifier + password);

            SHA1 sha1 = SHA1.Create();
            sha1.TransformFinalBlock(buffer, 0, buffer.Length);

            byte[] hash = sha1.Hash;

            StringBuilder digest = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                digest.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));
            }

            return digest.ToString();
        }

        #endregion
    }
}
