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
using System.Threading;
using Hanoi.Serialization.Extensions.ServiceDiscovery;
using Hanoi.Serialization.InstantMessaging.Client;

namespace Hanoi.Xmpp.InstantMessaging.ServiceDiscovery {
    public abstract class XmppServiceDiscoveryObject
         {
        private readonly Jid identifier;
        private readonly Session session;
        private readonly AutoResetEvent waitEvent;
        private List<XmppServiceFeature> features;
        private bool featuresRequested;
        private List<XmppServiceIdentity> identities;
        private IDisposable infoQuerySubscription;
        private List<XmppServiceItem> items;
        private bool itemsRequested;
        private List<string> pendingMessages;
        private IDisposable serviceDiscoverySubscription;
        private IDisposable sessionStateSubscription;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppServiceDiscoveryObject" /> class.
        /// </summary>
        protected XmppServiceDiscoveryObject(Session session, string identifier) {
            this.session = session;
            this.identifier = identifier;
            waitEvent = new AutoResetEvent(false);

            Subscribe();
        }

        /// <summary>
        ///   Gets the XMPP Identifier (JID)
        /// </summary>
        public Jid Identifier {
            get { return identifier; }
        }

        /// <summary>
        ///   Gets the object identity.
        /// </summary>
        public List<XmppServiceIdentity> Identities {
            get {
                if (!featuresRequested)
                {
                    DiscoverFeatures();
                }

                return identities;
            }
        }

        /// <summary>
        ///   Gets the list of features supported by the XMPP Service
        /// </summary>
        public List<XmppServiceFeature> Features {
            get {
                if (!featuresRequested)
                {
                    DiscoverFeatures();
                }

                return features;
            }
        }

        /// <summary>
        ///   Gets the list of items for the XMPP Service
        /// </summary>
        public List<XmppServiceItem> Items {
            get {
                if (!itemsRequested)
                {
                    DiscoverItems();
                }

                return items;
            }
        }

        /// <summary>
        ///   Gets the Xmpp Session
        /// </summary>
        protected Session Session {
            get { return session; }
        }

        /// <summary>
        ///   Gets the list of message id's pending of response
        /// </summary>
        protected List<string> PendingMessages {
            get {
                if (pendingMessages == null)
                {
                    pendingMessages = new List<string>();
                }

                return pendingMessages;
            }
        }

        /// <summary>
        ///   Discover item features
        /// </summary>
        /// <param name = "serviceJid"></param>
        public virtual void DiscoverFeatures() {
            if (features == null)
            {
                features = new List<XmppServiceFeature>();
            }
            else
            {
                features.Clear();
            }

            // Get Service Info
            var iq = new IQ
                         {
                             ID = XmppIdentifierGenerator.Generate(),
                             Type = IQType.Get,
                             From = session.UserId,
                             To = Identifier
                         };

            iq.Items.Add(new ServiceQuery());

            featuresRequested = true;
            PendingMessages.Add(iq.ID);
            session.Send(iq);

            waitEvent.WaitOne();
        }

        /// <summary>
        ///   Discover item items.
        /// </summary>
        public virtual void DiscoverItems() {
            if (!featuresRequested)
            {
                DiscoverFeatures();
            }

            if (items == null)
            {
                items = new List<XmppServiceItem>();
            }
            else
            {
                items.Clear();
            }

            // Get Service Details
            var iq = new IQ
                         {
                             ID = XmppIdentifierGenerator.Generate(),
                             Type = IQType.Get,
                             From = session.UserId,
                             To = Identifier
                         };

            iq.Items.Add(new ServiceItemQuery());

            itemsRequested = true;
            PendingMessages.Add(iq.ID);
            session.Send(iq);

            waitEvent.WaitOne();
        }

        /// <summary>
        ///   Returns a <see cref = "T:System.String"></see> that represents the current <see cref = "T:System.Object"></see>.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.String"></see> that represents the current <see cref = "T:System.Object"></see>.
        /// </returns>
        public override string ToString() {
            return Identifier;
        }

        /// <summary>
        ///   Check if the item is on the given service category
        /// </summary>
        /// <param name = "category"></param>
        /// <returns></returns>
        public bool IsOnCategory(XmppServiceCategory category) {
            return (identities.Where(s => s.Category == XmppServiceCategory.Conference).Count() > 0);
        }

        private void SubscribeToSessionState() {
            sessionStateSubscription = session
                .StateChanged
                .Where(s => s == SessionState.LoggingIn || s == SessionState.LoggingOut)
                .Subscribe(newState => OnSessionStateChanged(newState));
        }

        protected virtual void Subscribe() {
            infoQuerySubscription = session.Connection
                .OnInfoQueryMessage
                .Where(message => message.Type == IQType.Error)
                .Subscribe(message => OnQueryErrorMessage(message));

            serviceDiscoverySubscription = session.Connection
                .OnServiceDiscoveryMessage
                .Where(message => PendingMessages.Contains(message.ID))
                .Subscribe(message => OnServiceDiscoveryMessage(message));
        }

        protected virtual void Unsubscribe() {
            if (sessionStateSubscription != null)
            {
                sessionStateSubscription.Dispose();
                sessionStateSubscription = null;
            }

            if (infoQuerySubscription != null)
            {
                infoQuerySubscription.Dispose();
                infoQuerySubscription = null;
            }

            if (serviceDiscoverySubscription != null)
            {
                serviceDiscoverySubscription.Dispose();
                serviceDiscoverySubscription = null;
            }
        }

        protected virtual void OnSessionStateChanged(SessionState newState) {
            SubscribeToSessionState();

            if (newState == SessionState.LoggingIn)
            {
                Subscribe();
            }
            else if (newState == SessionState.LoggingOut)
            {
                Unsubscribe();
            }

            //NotifyAllPropertiesChanged();
        }

        private void OnServiceDiscoveryMessage(IQ message) {
            if (PendingMessages.Contains(message.ID))
            {
                PendingMessages.Remove(message.ID);

                foreach (object item in message.Items)
                {
                    if (item is ServiceItemQuery)
                    {
                        foreach (ServiceItem serviceItem in ((ServiceItemQuery) item).Items)
                        {
                            items.Add(new XmppServiceItem(session, serviceItem.Jid));
                        }

                        //NotifyPropertyChanged(() => Items);
                    }
                    else if (item is ServiceQuery)
                    {
                        // Details of available services
                        var query = (ServiceQuery) item;

                        if (identities == null)
                        {
                            identities = new List<XmppServiceIdentity>();
                        }

                        identities.Clear();

                        foreach (ServiceIdentity identity in query.Identities)
                        {
                            identities.Add(new XmppServiceIdentity(identity.Name, identity.Category, identity.Type));
                        }

                        foreach (ServiceFeature feature in query.Features)
                        {
                            features.Add(new XmppServiceFeature(feature.Name));
                        }

                        //NotifyPropertyChanged(() => Identities);
                        //NotifyPropertyChanged(() => Features);
                    }
                }

                waitEvent.Set();
            }
        }

        private void OnQueryErrorMessage(IQ message) {
            if (PendingMessages.Contains(message.ID))
            {
                PendingMessages.Remove(message.ID);

                waitEvent.Set();
            }
        }
        }
}