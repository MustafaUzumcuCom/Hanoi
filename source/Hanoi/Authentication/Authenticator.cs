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

namespace Hanoi.Authentication
{
    public abstract class Authenticator : IDisposable
    {
        public abstract string FeatureKey { get; }
        private List<string> _pendingMessages;

        protected Authenticator(Connection connection)
        {
            Connection = connection;
            Subscribe();
        }

        public string AuthenticationError { get; private set; }
        public bool AuthenticationFailed { get; protected set; }
        public Connection Connection { get; private set; }

        public List<string> PendingMessages
        {
            get { return _pendingMessages ?? (_pendingMessages = new List<string>()); }
        }

        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        
        ~Authenticator()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) 
                return;

            Unsubscribe();

            if (_pendingMessages != null)
            {
                _pendingMessages.Clear();
            }

            _pendingMessages = null;
            Connection = null;
            AuthenticationError = null;
            AuthenticationFailed = false;
        }

        
        public abstract void Authenticate();
        protected abstract void OnUnhandledMessage(object sender, UnhandledMessageEventArgs e);
        
        protected void Subscribe()
        {
            Connection.UnhandledMessage += OnUnhandledMessage;
            Connection.AuthenticationError += OnAuthenticationError;
        }

        protected void Unsubscribe()
        {
            Connection.UnhandledMessage -= OnUnhandledMessage;
            Connection.AuthenticationError -= OnAuthenticationError;
        }

        protected virtual void OnAuthenticationError(object sender, AuthenticationFailiureEventArgs e)
        {
            if (_pendingMessages != null)
            {
                _pendingMessages.Clear();
            }

            AuthenticationError = e.Message;
            AuthenticationFailed = true;
        }
    }
}