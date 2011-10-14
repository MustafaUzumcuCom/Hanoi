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
using System.Linq;
using Hanoi.Core;
using Hanoi.Serialization.Extensions.ServiceDiscovery;
using Hanoi.Serialization.InstantMessaging.Client;

namespace BabelIm.Net.Xmpp.InstantMessaging.PersonalEventing {
    /// <summary>
    ///   XMPP Personal Eventing
    /// </summary>
    public sealed class XmppPersonalEventing
        : ObservableObject {
        private readonly List<string> features;
        private readonly List<string> pendingMessages;
        private readonly XmppSession session;
        private IDisposable infoQueryErrorSubscription;
        private bool isUserTuneEnabled;
        private IDisposable serviceDiscoverySubscription;
        private IDisposable sessionStateSubscription;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "T:XmppServiceDiscovery" /> class.
        /// </summary>
        /// <param name = "session">The session.</param>
        internal XmppPersonalEventing(XmppSession session) {
            this.session = session;
            pendingMessages = new List<string>();
            features = new List<string>();

            SubscribeToSessionState();
        }

        /// <summary>
        ///   Gets the collection of features ( if personal eventing is supported )
        /// </summary>
        public IEnumerable<string> Features {
            get {
                foreach (string feature in features)
                {
                    yield return feature;
                }
            }
        }

        /// <summary>
        ///   Gets a value that indicates if it supports user tunes
        /// </summary>
        public bool SupportsUserTune {
            get { return SupportsFeature(XmppFeatures.UserTune); }
        }

        /// <summary>
        ///   Gets a value that indicates if it supports user moods
        /// </summary>
        public bool SupportsUserMood {
            get { return SupportsFeature(XmppFeatures.UserMood); }
        }

        /// <summary>
        ///   Gets or sets a value that indicates if user tune is enabled
        /// </summary>
        public bool IsUserTuneEnabled {
            get { return isUserTuneEnabled; }
            set {
                if (isUserTuneEnabled != value)
                {
                    isUserTuneEnabled = value;
                    NotifyPropertyChanged(() => IsUserTuneEnabled);
                }
            }
        }

        /// <summary>
        ///   Discover if we have personal eventing support enabled
        /// </summary>
        /// <returns></returns>
        internal void DiscoverSupport() {
            var query = new ServiceItemQuery();
            var iq = new IQ();

            iq.ID = XmppIdentifierGenerator.Generate();
            iq.Type = IQType.Get;
            iq.From = session.UserId;
            iq.To = session.UserId.BareIdentifier;

            iq.Items.Add(query);

            pendingMessages.Add(iq.ID);
            features.Clear();

            session.Send(iq);
        }

        private bool SupportsFeature(string featureName) {
            var q = from feature in features
                    where feature == featureName
                    select feature;

            return (q.Count() > 0);
        }

        private void SubscribeToSessionState() {
            sessionStateSubscription = session
                .StateChanged
                .Where(s => s == XmppSessionState.LoggingIn || s == XmppSessionState.LoggingOut)
                .Subscribe
                (
                    newState =>
                        {
                            if (newState == XmppSessionState.LoggingOut)
                            {
                                Subscribe();
                            }
                            else if (newState == XmppSessionState.LoggingOut)
                            {
                                features.Clear();
                                pendingMessages.Clear();
                                Unsubscribe();
                            }
                        }
                );
        }

        private void Subscribe() {
            infoQueryErrorSubscription = session.Connection
                .OnInfoQueryMessage
                .Where(iq => iq.Type == IQType.Error)
                .Subscribe(message => OnQueryErrorMessage(message));

            infoQueryErrorSubscription = session.Connection
                .OnServiceDiscoveryMessage
                .Where(message => message.Type == IQType.Result && pendingMessages.Contains(message.ID))
                .Subscribe(message => OnServiceDiscoveryMessage(message));
        }

        private void Unsubscribe() {
            if (infoQueryErrorSubscription != null)
            {
                infoQueryErrorSubscription.Dispose();
                infoQueryErrorSubscription = null;
            }

            if (serviceDiscoverySubscription != null)
            {
                serviceDiscoverySubscription.Dispose();
                serviceDiscoverySubscription = null;
            }
        }

        private void OnServiceDiscoveryMessage(IQ message) {
            pendingMessages.Remove(message.ID);

            foreach (ServiceItemQuery item in message.Items)
            {
                foreach (ServiceItem sitem in item.Items)
                {
                    features.Add(sitem.Node);
                }

                NotifyPropertyChanged(() => SupportsUserTune);
                NotifyPropertyChanged(() => SupportsUserMood);

                if (SupportsUserTune)
                {

                }
            }
        }

        private void OnQueryErrorMessage(IQ message) {
            if (pendingMessages.Contains(message.ID))
            {
                pendingMessages.Remove(message.ID);
            }
        }
        }
}