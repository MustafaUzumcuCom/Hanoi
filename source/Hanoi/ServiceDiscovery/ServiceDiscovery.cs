/*
    Copyright (c) 2007 - 2010, Carlos Guzm�n �lvarez

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
using System.Collections.ObjectModel;
using System.Linq;
using Hanoi.Serialization.Extensions.ServiceDiscovery;
using Hanoi.Serialization.InstantMessaging.Client;

namespace Hanoi.Xmpp.InstantMessaging.ServiceDiscovery
{
    /// <summary>
    ///   XMPP Service Discovery
    /// </summary>
    public sealed class ServiceDiscovery
    {
        private readonly string domainName;
        private readonly List<string> pendingMessages;
        private readonly Session session;
        private IDisposable infoQuerySubscription;
        private IDisposable serviceDiscoverySubscription;
        private ObservableCollection<Service> services;

        private IDisposable sessionStateSubscription;
        private bool supportsServiceDiscovery;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "T:ServiceDiscovery" /> class.
        /// </summary>
        /// <param name = "session">The session.</param>
        public ServiceDiscovery(Session session)
        {
            this.session = session;
            pendingMessages = new List<string>();

            SubscribeToSessionState();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "T:ServiceDiscovery" /> class.
        /// </summary>
        /// <param name = "session">The session.</param>
        public ServiceDiscovery(Session session, string domainName)
            : this(session)
        {
            this.domainName = domainName;
        }

        /// <summary>
        ///   Gets the collection of discovered services
        /// </summary>
        public ObservableCollection<Service> Services
        {
            get
            {
                if (services == null)
                {
                    services = new ObservableCollection<Service>();
                }

                return services;
            }
        }

        /// <summary>
        ///   Gets a value that indicates if it supports multi user chat
        /// </summary>
        public bool SupportsMultiuserChat
        {
            get { return SupportsFeature(Features.MultiUserChat); }
        }

        /// <summary>
        ///   Gets a value that indicates whether service discovery is supported
        /// </summary>
        public bool SupportsServiceDiscovery
        {
            get { return supportsServiceDiscovery; }
            private set
            {
                if (supportsServiceDiscovery != value)
                {
                    supportsServiceDiscovery = value;
                    //NotifyPropertyChanged(() => SupportsServiceDiscovery);
                }
            }
        }

        /// <summary>
        ///   Gets a value that indicates whether user tunes are supported
        /// </summary>
        public bool SupportsUserTune
        {
            get { return SupportsFeature(Features.UserTune); }
        }

        /// <summary>
        ///   Gets a value that indicates whether user moods are supported
        /// </summary>
        public bool SupportsUserMood
        {
            get { return SupportsFeature(Features.UserMood); }
        }

        /// <summary>
        ///   Gets a value that indicates whether simple communications blocking is supported
        /// </summary>
        public bool SupportsBlocking
        {
            get { return SupportsFeature(Features.SimpleCommunicationsBlocking); }
        }

        /// <summary>
        ///   Clears service discovery data
        /// </summary>
        public void Clear()
        {
            pendingMessages.Clear();
            Services.Clear();

            //NotifyAllPropertiesChanged();
        }

        public Service GetService(ServiceCategory category)
        {
            return Services
                .Where(s => s.IsOnCategory(ServiceCategory.Conference))
                .FirstOrDefault();
        }

        /// <summary>
        ///   Discover existing services provided by the XMPP Server
        /// </summary>
        /// <returns></returns>
        public void DiscoverServices()
        {
            Clear();

            string domain = ((String.IsNullOrEmpty(domainName)) ? session.UserId.DomainName : domainName);
            string messageId = IdentifierGenerator.Generate();
            var iq = new IQ
                         {
                             ID = messageId,
                             Type = IQType.Get,
                             From = session.UserId,
                             To = domain
                         };

            iq.Items.Add(new ServiceItemQuery());

            pendingMessages.Add(messageId);

            session.Send(iq);
        }

        private bool SupportsFeature(string featureName)
        {
            var q = from service in Services
                    where service.Features.Where(f => f.Name == featureName).Count() > 0
                    select service;

            return (q.Count() > 0);
        }

        private void SubscribeToSessionState()
        {
            sessionStateSubscription = session
                .StateChanged
                .Where(s => s == SessionState.LoggingIn || s == SessionState.LoggingOut)
                .Subscribe
                (
                    newState =>
                    {
                        if (newState == SessionState.LoggingIn)
                        {
                            Subscribe();
                        }
                        else if (newState == SessionState.LoggingOut)
                        {
                            Unsubscribe();
                            Clear();
                        }
                    }
                );
        }

        private void Subscribe()
        {
            infoQuerySubscription = session.Connection
                .OnInfoQueryMessage
                .Where(message => message.Type == IQType.Error && pendingMessages.Contains(message.ID))
                .Subscribe(message => OnQueryErrorReceived(message));

            serviceDiscoverySubscription = session.Connection
                .OnServiceDiscoveryMessage
                .Where(message => message.Type == IQType.Result && pendingMessages.Contains(message.ID))
                .Subscribe(message => OnServiceDiscoveryMessageReceived(message));
        }

        private void Unsubscribe()
        {
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

        private void OnServiceDiscoveryMessageReceived(IQ message)
        {
            pendingMessages.Remove(message.ID);
            SupportsServiceDiscovery = true;

            foreach (object item in message.Items)
            {
                if (item is ServiceItemQuery)
                {
                    // List of availabl services
                    foreach (Serialization.Extensions.ServiceDiscovery.ServiceItem serviceItem in ((ServiceItemQuery)item).Items)
                    {
                        services.Add(new Service(session, serviceItem.Jid));
                    }
                }
            }

            //NotifyPropertyChanged(() => Services);
        }

        private void OnQueryErrorReceived(IQ message)
        {
            pendingMessages.Remove(message.ID);

            if (message.Error.FeatureNotImplemented != null)
            {
                SupportsServiceDiscovery = false;
            }
        }
    }
}