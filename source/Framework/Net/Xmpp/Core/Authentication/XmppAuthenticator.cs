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
using System.Collections.Generic;
using Hanoi.Authentication;

namespace Hanoi.Core.Authentication {

    // TODO: introduce IAuthenticator interface

    /// <summary>
    ///   Base class for authentication mechanims implementations.
    /// </summary>
    internal abstract class XmppAuthenticator : IDisposable {
        private List<string> pendingMessages;

        /// <summary>
        ///   Initializes a new instance of the <see cref="XmppAuthenticator" /> class.
        /// </summary>
        /// <param name = "connection">A <see cref="XmppConnection" /> instance</param>
        protected XmppAuthenticator(XmppConnection connection) {
            Connection = connection;

            Subscribe();
        }

        /// <summary>
        ///   Gets the authentication error.
        /// </summary>
        /// <value>The authentication error.</value>
        public string AuthenticationError { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether the authentication has failed.
        /// </summary>
        /// <value><c>true</c> if authentication failed; otherwise, <c>false</c>.</value>
        public bool AuthenticationFailed { get; protected set; }

        /// <summary>
        ///   Gets the connection associated with the authenticator class.
        /// </summary>
        public XmppConnection Connection { get; private set; }

        /// <summary>
        ///   Gets the list of message ID's pending of response
        /// </summary>
        public List<string> PendingMessages {
            get {
                if (pendingMessages == null) {
                    pendingMessages = new List<string>();
                }

                return pendingMessages;
            }
        }

        #region IDisposable Members

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref = "T:Hanoi.Core.Authentication.XmppAuthenticator" /> is reclaimed by garbage collection.
        /// </summary>
        ~XmppAuthenticator() {
            Dispose(false);
        }

        /// <summary>
        ///   Disposes the specified disposing.
        /// </summary>
        /// <param name = "disposing">if set to <c>true</c> [disposing].</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                Unsubscribe();

                if (pendingMessages != null){
                    pendingMessages.Clear();
                }

                pendingMessages = null;
                Connection = null;
                AuthenticationError = null;
                AuthenticationFailed = false;
            }
        }

        /// <summary>
        ///   Performs the authentication.
        /// </summary>
        public abstract void Authenticate();

        protected void Subscribe() {
            Connection.UnhandledMessage += OnUnhandledMessage;
            Connection.AuthenticationError += OnAuthenticationError;
        }

        protected void Unsubscribe() {
            Connection.UnhandledMessage -= OnUnhandledMessage;
            Connection.AuthenticationError -= OnAuthenticationError;
        }

        /// <summary>
        ///   Called when an unhandled message is received
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "T:Hanoi.Core.XmppUnhandledMessageEventArgs" /> instance containing the event data.</param>
        protected abstract void OnUnhandledMessage(object sender, XmppUnhandledMessageEventArgs e);

        /// <summary>
        ///   Called when an authentication failiure occurs.
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "XmppAuthenticationFailiureEventArgs" /> instance containing the event data.</param>
        protected virtual void OnAuthenticationError(object sender, XmppAuthenticationFailiureEventArgs e) {
            if (pendingMessages != null) {
                pendingMessages.Clear();
            }

            AuthenticationError = e.Message;
            AuthenticationFailed = true;
        }
    }
}