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
using System.Collections.ObjectModel;
using System.Linq;
using BabelIm.Net.Xmpp.Core;
using BabelIm.Net.Xmpp.Serialization.Extensions.ServiceDiscovery;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client;

namespace BabelIm.Net.Xmpp.InstantMessaging.ServiceDiscovery
{
    /// <summary>
    /// XMPP Service Discovery
    /// </summary>
    public sealed class XmppServiceDiscovery 
        : ObservableObject
    {
        #region · Fields ·

        private XmppSession     session;
        private List<string>    pendingMessages;
        private string			domainName;
        private bool            supportsServiceDiscovery;

        private ObservableCollection<XmppService> services;

        #region · Subscriptions ·

        private IDisposable sessionStateSubscription;
        private IDisposable infoQuerySubscription;
        private IDisposable serviceDiscoverySubscription;

        #endregion

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the collection of discovered services
        /// </summary>
        public ObservableCollection<XmppService> Services
        {
            get
            {
                if (this.services == null)
                {
                    this.services = new ObservableCollection<XmppService>();
                }

                return this.services;
            }
        }

        /// <summary>
        /// Gets a value that indicates if it supports multi user chat
        /// </summary>
        public bool SupportsMultiuserChat
        {
            get { return this.SupportsFeature(XmppFeatures.MultiUserChat); }
        }

        /// <summary>
        /// Gets a value that indicates whether service discovery is supported
        /// </summary>
        public bool SupportsServiceDiscovery
        {
            get { return this.supportsServiceDiscovery; }
            private set
            {
                if (this.supportsServiceDiscovery != value)
                {
                    this.supportsServiceDiscovery = value;
                    this.NotifyPropertyChanged(() => SupportsServiceDiscovery);
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether user tunes are supported
        /// </summary>
        public bool SupportsUserTune
        {
            get { return this.SupportsFeature(XmppFeatures.UserTune); }
        }

        /// <summary>
        /// Gets a value that indicates whether user moods are supported
        /// </summary>
        public bool SupportsUserMood
        {
            get { return this.SupportsFeature(XmppFeatures.UserMood); }
        }

        /// <summary>
        /// Gets a value that indicates whether simple communications blocking is supported
        /// </summary>
        public bool SupportsBlocking
        {
            get { return this.SupportsFeature(XmppFeatures.SimpleCommunicationsBlocking); }
        }
        
        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppServiceDiscovery"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public XmppServiceDiscovery(XmppSession session)
        {
            this.session            = session;
            this.pendingMessages    = new List<string>();

            this.SubscribeToSessionState();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppServiceDiscovery"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public XmppServiceDiscovery(XmppSession session, string domainName)
            : this(session)
        {
            this.domainName = domainName;
        }
        
        #endregion

        #region · Methods ·

        /// <summary>
        /// Clears service discovery data
        /// </summary>
        public void Clear()
        {
            this.pendingMessages.Clear();
            this.Services.Clear();

            this.NotifyAllPropertiesChanged();
        }

        public XmppService GetService(XmppServiceCategory category)
        {
            return this.Services
                .Where(s => s.IsOnCategory(XmppServiceCategory.Conference))
                .FirstOrDefault();
        }

        /// <summary>
        /// Discover existing services provided by the XMPP Server
        /// </summary>
        /// <returns></returns>
        public void DiscoverServices()
        {
            this.Clear();

            string  domain      = ((String.IsNullOrEmpty(this.domainName)) ? this.session.UserId.DomainName : this.domainName);
            string  messageId   = XmppIdentifierGenerator.Generate();
            IQ      iq          = new IQ
            {
                ID   = messageId,
                Type = IQType.Get,
                From = this.session.UserId,
                To   = domain
            };

            iq.Items.Add(new ServiceItemQuery());

            this.pendingMessages.Add(messageId);

            this.session.Send(iq);
        }

        #endregion
        
        #region · Private Methods ·
        
        private bool SupportsFeature(string featureName)
        {
            var q = from service in this.Services
                    where service.Features.Where(f => f.Name == featureName).Count() > 0
                    select service;
            
            return (q.Count() > 0);
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
                    if (newState == XmppSessionState.LoggingIn)
                    {
                        this.Subscribe();
                    }
                    else if (newState == XmppSessionState.LoggingOut)
                    {
                        this.Unsubscribe();
                        this.Clear();
                    }
                }
            );
        }

        private void Subscribe()
        {
            this.infoQuerySubscription = this.session.Connection
                .OnInfoQueryMessage
                .Where(message => message.Type == IQType.Error && this.pendingMessages.Contains(message.ID))
                .Subscribe(message => this.OnQueryErrorReceived(message));

            this.serviceDiscoverySubscription = this.session.Connection
                .OnServiceDiscoveryMessage
                .Where(message => message.Type == IQType.Result && this.pendingMessages.Contains(message.ID))
                .Subscribe(message => this.OnServiceDiscoveryMessageReceived(message));
        }

        private void Unsubscribe()
        {
            if (this.infoQuerySubscription != null)
            {
                this.infoQuerySubscription.Dispose();
                this.infoQuerySubscription = null;
            }

            if (this.serviceDiscoverySubscription != null)
            {
                this.serviceDiscoverySubscription.Dispose();
                this.serviceDiscoverySubscription = null;
            }
        }

        #endregion

        #region · Message Handlers ·

        private void OnServiceDiscoveryMessageReceived(IQ message)
        {
            this.pendingMessages.Remove(message.ID);
            this.SupportsServiceDiscovery = true;

            foreach (object item in message.Items)
            {
                if (item is ServiceItemQuery)
                {
                    // List of availabl services
                    foreach (ServiceItem serviceItem in ((ServiceItemQuery)item).Items)
                    {
                        this.services.Add(new XmppService(this.session, serviceItem.Jid));
                    }
                } 
            }

            this.NotifyPropertyChanged(() => Services);
        }

        private void OnQueryErrorReceived(IQ message)
        {
            this.pendingMessages.Remove(message.ID);

            if (message.Error.FeatureNotImplemented != null)
            {
                this.SupportsServiceDiscovery = false;
            }
        }

        #endregion
    }
}
