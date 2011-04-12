﻿/*
    Copyright (c) 2007 - 2010, Carlos Guzmán Álvarez

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

using System;
using System.Text;
using System.Threading;
using BabelIm.Net.Xmpp.Serialization.Core.Sasl;

namespace BabelIm.Net.Xmpp.Core
{
    /// <summary>
    /// <see cref="XmppAuthenticator" /> implementation for the SASL Plain Authentication mechanism.
    /// </summary>
    /// <remarks>
    /// References:
    ///     http://www.ietf.org/html.charters/sasl-charter.html
    ///     http://www.ietf.org/internet-drafts/draft-ietf-sasl-plain-09.txt
    /// </remarks>
    internal sealed class XmppSaslPlainAuthenticator 
        : XmppAuthenticator
    {
        #region · Fields ·

        private AutoResetEvent waitEvent;

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppSaslPlainAuthenticator"/> class.
        /// </summary>
        public XmppSaslPlainAuthenticator(XmppConnection connection) 
            : base(connection)
        {
            this.waitEvent = new AutoResetEvent(false);
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Performs the authentication using the SASL Plain authentication mechanism.
        /// </summary>
        public override void Authenticate()
        {
            // Send authentication mechanism
            Auth auth = new Auth();

            auth.Mechanism  = XmppCodes.SaslPlainMechanism;
            auth.Value      = this.BuildMessage();

            this.Connection.Send(auth);

            this.waitEvent.WaitOne();

            if (!this.AuthenticationFailed)
            {
                // Re-Initialize XMPP Stream
                this.Connection.InitializeXmppStream();

                // Wait until we receive the Stream features
                this.Connection.WaitForStreamFeatures();
            }
        }

        #endregion

        #region · Protected Methods ·

        protected override void OnUnhandledMessage(object sender, XmppUnhandledMessageEventArgs e)
        {
            if (e.StanzaInstance is Success)
            {
                this.waitEvent.Set();
            }
        }

        protected override void OnAuthenticationError(object sender, XmppAuthenticationFailiureEventArgs e)
        {
            base.OnAuthenticationError(sender, e);

            this.waitEvent.Set();
        }

        #endregion

        #region · Private Methods ·

        private string BuildMessage()
        {
            string message  = String.Format("\0{0}\0{1}", this.Connection.UserId.BareIdentifier, this.Connection.UserPassword);

            return Encoding.UTF8.GetBytes(message).ToBase64String();
        }

        #endregion
    }
}
