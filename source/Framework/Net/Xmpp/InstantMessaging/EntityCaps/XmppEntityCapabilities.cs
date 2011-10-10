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
using System.Linq;
using BabelIm.Net.Xmpp.Core;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client;

namespace BabelIm.Net.Xmpp.InstantMessaging
{
    /// <summary>
    /// Client capabilities (XEP-0115)
    /// </summary>
    public abstract class XmppEntityCapabilities 
        : XmppClientCapabilities
    {
        #region · Consts ·

        /// <summary>
        /// Default hash algorithm name
        /// </summary>
        /// <remarks>
        /// SHA-1
        /// </remarks>
        public static readonly string DefaultHashAlgorithmName = "sha-1";

        #endregion

        #region · Fields ·

        private XmppSession	session;

        #region · Subscriptions ·

        private IDisposable sessionStateSubscription;
        private IDisposable infoQueryErrorSubscription;
        private IDisposable serviceDiscoverySubscription;

        #endregion

        #endregion

        #region · Protected Properties ·

        /// <summary>
        /// Gets the <see cref="XmppSession">Session</see>
        /// </summary>
        protected XmppSession Session
        {
            get { return this.session; }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="XmppEntityCapabilities"/> class.
        /// </summary>
        protected XmppEntityCapabilities(XmppSession session)
        	: base()
        {
            this.session = session;

            this.SubscribeToSessionState();
        }

        #endregion

        #region · Message Subscriptions ·

        private void SubscribeToSessionState()
        {
            this.sessionStateSubscription = this.session
                .StateChanged
                .Where(s => s == XmppSessionState.LoggingIn || s == XmppSessionState.LoggingOut)
                .Subscribe
            (
                newState =>
                {
                    if (newState == XmppSessionState.LoggingOut)
                    {
                        this.Subscribe();
                    }
                    else if (newState == XmppSessionState.LoggingOut)
                    {
                        this.Unsubscribe();
                    }
                }
            );
        }

        protected virtual void Subscribe()
        {
            this.infoQueryErrorSubscription = this.session.Connection
                .OnInfoQueryMessage
                .Where(message => message.Type == IQType.Error)
                .Subscribe(message => this.OnQueryErrorMessage(message));

            this.serviceDiscoverySubscription = this.session.Connection
                .OnServiceDiscoveryMessage
                .Subscribe(message => this.OnServiceDiscoveryMessage(message));
        }

        protected virtual void Unsubscribe()
        {
            if (this.infoQueryErrorSubscription != null)
            {
                this.infoQueryErrorSubscription.Dispose();
                this.infoQueryErrorSubscription = null;
            }

            if (this.serviceDiscoverySubscription != null)
            {
                this.serviceDiscoverySubscription.Dispose();
                this.serviceDiscoverySubscription = null;
            }
        }
                
        #endregion

        #region · Message Handlers ·

        protected abstract void OnServiceDiscoveryMessage(IQ message);

        protected virtual void OnQueryErrorMessage(IQ message)
        {
        }

        #endregion
    }
}