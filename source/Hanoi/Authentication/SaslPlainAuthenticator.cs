/*
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
using Hanoi.Serialization.Core.Sasl;

namespace Hanoi.Authentication
{
    /// <summary>
    ///   <see cref = "Authenticator" /> implementation for the SASL Plain Authentication mechanism.
    /// </summary>
    /// <remarks>
    ///   References:
    ///   http://www.ietf.org/html.charters/sasl-charter.html
    ///   http://www.ietf.org/internet-drafts/draft-ietf-sasl-plain-09.txt
    /// </remarks>
    internal sealed class SaslPlainAuthenticator : Authenticator
    {
        private readonly AutoResetEvent _waitEvent;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SaslPlainAuthenticator" /> class.
        /// </summary>
        public SaslPlainAuthenticator(Connection connection)
            : base(connection)
        {
            _waitEvent = new AutoResetEvent(false);
        }

        public override string FeatureKey
        {
            get { return "PLAIN"; }
        }

        /// <summary>
        ///   Performs the authentication using the SASL Plain authentication mechanism.
        /// </summary>
        public override void Authenticate()
        {
            // Send authentication mechanism
            var auth = new Auth
                           {
                               Mechanism = XmppCodes.SaslPlainMechanism, 
                               Value = BuildMessage(),
                           };
            
            Connection.Send(auth);

            _waitEvent.WaitOne();

            if (!AuthenticationFailed)
            {
                // Re-Initialize XMPP Stream
                Connection.InitializeXmppStream();

                // Wait until we receive the Stream features
                Connection.WaitForStreamFeatures();
            }
        }

        protected override void OnUnhandledMessage(object sender, UnhandledMessageEventArgs e)
        {
            if (e.StanzaInstance is Success)
            {
                _waitEvent.Set();
            }
        }

        protected override void OnAuthenticationError(object sender, AuthenticationFailiureEventArgs e)
        {
            base.OnAuthenticationError(sender, e);

            _waitEvent.Set();
        }

        private string BuildMessage()
        {
            string message = String.Format("\0{0}\0{1}", Connection.UserId.BareIdentifier, Connection.UserPassword);

            return Encoding.UTF8.GetBytes(message).ToBase64String();
        }
    }
}