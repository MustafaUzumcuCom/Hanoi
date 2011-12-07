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
using System.Linq;
using Hanoi.Serialization.InstantMessaging.Client;

namespace Hanoi.Xmpp.InstantMessaging.EntityCaps
{
    /// <summary>
    ///   Client capabilities (XEP-0115)
    /// </summary>
    public abstract class EntityCapabilities : ClientCapabilities
    {
        public static readonly string DefaultHashAlgorithmName = "sha-1";

        private IDisposable _infoQueryErrorSubscription;
        private IDisposable _serviceDiscoverySubscription;
        private IDisposable _sessionStateSubscription;

        protected EntityCapabilities(Session session)
        {
            Session = session;

            SubscribeToSessionState();
        }

        protected Session Session { get; private set; }

        private void SubscribeToSessionState()
        {
            _sessionStateSubscription = Session
                .StateChanged
                .Where(s => s == SessionState.LoggingIn || s == SessionState.LoggingOut)
                .Subscribe
                (
                    newState =>
                    {
                        switch (newState)
                        {
                            case SessionState.LoggingIn:
                                Subscribe();
                                break;

                            case SessionState.LoggingOut:
                                Unsubscribe();
                                break;
                        }
                    }
                );
        }

        protected virtual void Subscribe()
        {
            _infoQueryErrorSubscription = Session.Connection
                .OnInfoQueryMessage
                .Where(message => message.Type == IQType.Error)
                .Subscribe(OnQueryErrorMessage);

            _serviceDiscoverySubscription = Session.Connection
                .OnServiceDiscoveryMessage
                .Subscribe(OnServiceDiscoveryMessage);
        }

        protected virtual void Unsubscribe()
        {
            if (_infoQueryErrorSubscription != null)
            {
                _infoQueryErrorSubscription.Dispose();
                _infoQueryErrorSubscription = null;
            }

            if (_serviceDiscoverySubscription != null)
            {
                _serviceDiscoverySubscription.Dispose();
                _serviceDiscoverySubscription = null;
            }
        }

        protected abstract void OnServiceDiscoveryMessage(IQ message);

        protected virtual void OnQueryErrorMessage(IQ message)
        {
        }
    }
}