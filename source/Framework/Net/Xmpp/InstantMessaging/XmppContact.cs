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
using BabelIm.Net.Xmpp.Core;
using BabelIm.Net.Xmpp.Serialization.Extensions.SimpleCommunicationsBlocking;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client.Presence;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Roster;

namespace BabelIm.Net.Xmpp.InstantMessaging
{
    /// <summary>
    /// Represents a <see cref="XmppRoster"/> contact.
    /// </summary>
    public sealed class XmppContact 
        : ObservableObject
    {
        #region · Fields ·

        private string                      name;
        private string                      displayName;
        private XmppJid                     contactId;
        private XmppSession                 session;
        private XmppContactSubscriptionType subscription;
        private List<XmppContactResource>   resources;
        private List<string>                groups;
        private object                      syncObject;

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the contact id.
        /// </summary>
        /// <value>The contact id.</value>
        public XmppJid ContactId
        {
            get { return this.contactId; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return this.name; }
            private set
            {
                if (this.name != value)
                {
                    this.name = value;

                    this.NotifyPropertyChanged(() => Name);
                }
            }
        }

        /// <summary>
        /// Gets the contact groups.
        /// </summary>
        /// <value>The groups.</value>
        public List<string> Groups
        {
            get
            {
                if (this.groups == null)
                {
                    this.groups = new List<string>();
                }

                return this.groups;
            }
        }

        /// <summary>
        /// Gets the contact Display Name
        /// </summary>
        public string DisplayName
        {
            get { return this.displayName; }
            set
            {
                if (this.displayName != value)
                {
                    if (!String.IsNullOrEmpty(value))
                    {
                        this.displayName = value;
                    }
                    else
                    {
                        this.displayName = this.contactId.UserName;
                    }

                    this.Update();

                    this.NotifyPropertyChanged(() => DisplayName);
                }
            }
        }

        /// <summary>
        /// Gets the list available resources.
        /// </summary>
        /// <value>The resources.</value>
        public IEnumerable<XmppContactResource> Resources
        {
            get
            {
                if (this.resources == null)
                {
                    this.resources = new List<XmppContactResource>();
                }

                foreach (XmppContactResource resource in this.resources)
                {
                    yield return resource;
                }
            }
        }

        /// <summary>
        /// Gets or sets the subscription.
        /// </summary>
        /// <value>The subscription.</value>
        public XmppContactSubscriptionType Subscription
        {
            get { return this.subscription; }
            set
            {
                if (this.subscription != value)
                {
                    this.subscription = value;

                    this.NotifyPropertyChanged(() => Subscription);
                }
            }
        }

        /// <summary>
        /// Gets the presence.
        /// </summary>
        /// <value>The presence.</value>
        public XmppContactResource Resource
        {
            get { return this.GetResource(); }
        }

        /// <summary>
        /// Gets the presence.
        /// </summary>
        /// <value>The presence.</value>
        public XmppContactPresence Presence
        {
            get { return this.GetResource().Presence; }
        }

        /// <summary>
        /// Gets a value that indicates if the contact supports File Transfer
        /// </summary>
        public bool SupportsFileTransfer
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value that indicates if the contact supports MUC
        /// </summary>
        public bool SupportsConference
        {
            get { return (this.Resource.Capabilities.Features.Where(f => f.Name == XmppFeatures.MultiUserChat).Count() > 0); }
        }

        /// <summary>
        /// Gets a value that indicates if the contact supports chat state notifications
        /// </summary>
        public bool SupportsChatStateNotifications
        {
            get { return (this.Resource.Capabilities.Features.Where(f => f.Name == XmppFeatures.ChatStateNotifications).Count() > 0); }
        }
        
        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppContact"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="contactId">The contact id.</param>
        /// <param name="name">The name.</param>
        /// <param name="subscription">The subscription.</param>
        /// <param name="groups">The groups.</param>
        internal XmppContact(XmppSession session, string contactId, string name, XmppContactSubscriptionType subscription, IList<string> groups)
        {
            this.session    = session;
            this.syncObject = new object();
            this.contactId  = contactId;
            this.resources  = new List<XmppContactResource>();

            this.RefreshData(name, subscription, groups);
            this.AddDefaultResource();
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Adds to group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        public void AddToGroup(string groupName)
        {
            IQ          iq      = new IQ();
            RosterQuery query   = new RosterQuery();
            RosterItem  item    = new RosterItem();

            if (!this.Groups.Contains(groupName))
            {
                this.Groups.Add(groupName);
            }

            iq.Type = IQType.Set;

            item.Jid            = this.ContactId.BareIdentifier;
            item.Name           = this.Name;
            item.Subscription   = (RosterSubscriptionType)this.Subscription;

            item.Groups.Add(groupName);

            query.Items.Add(item);
            iq.Items.Add(query);

            this.session.Send(iq);
        }

        /// <summary>
        /// Updates the contact data.
        /// </summary>
        public void Update()
        {
            IQ          iq      = new IQ();
            RosterQuery query   = new RosterQuery();
            RosterItem  item    = new RosterItem();

            iq.Type = IQType.Set;

            item.Jid    = this.ContactId.BareIdentifier;
            item.Name   = this.DisplayName;
            item.Subscription = (RosterSubscriptionType)this.Subscription;

            item.Groups.AddRange(this.Groups);

            query.Items.Add(item);
            iq.Items.Add(query);

            this.session.Send(iq);
        }

        /// <summary>
        /// Request subscription to he presence of the contanct
        /// </summary>
        public void RequestSubscription()
        {
            this.session.Presence.RequestSubscription(this.ContactId);
        }

        /// <summary>
        /// Block contact
        /// </summary>
        public void Block()
        {
            if (this.session.ServiceDiscovery.SupportsBlocking)
            {
                IQ iq = new IQ
                {
                    ID      = XmppIdentifierGenerator.Generate(),
                    From    = this.session.UserId,
                    Type    = IQType.Set
                };

                Block block = new Block();

                block.Items.Add
                (
                    new BlockItem
                    {
                        Jid = this.ContactId.BareIdentifier
                    }
                );

                iq.Items.Add(block);

                this.session.Send(iq);
            }
        }

        /// <summary>
        /// Unblock contact.
        /// </summary>
        public void UnBlock()
        {
            if (this.session.ServiceDiscovery.SupportsBlocking)
            {
                IQ iq = new IQ
                {
                    ID      = XmppIdentifierGenerator.Generate(),
                    From    = this.session.UserId,
                    Type    = IQType.Set
                };

                UnBlock unBlock = new UnBlock();

                unBlock.Items.Add
                (
                    new BlockItem
                    {
                        Jid = this.ContactId.BareIdentifier
                    }
                );

                iq.Items.Add(unBlock);

                this.session.Send(iq);
            }
        }

        #endregion

        #region · Overriden Methods ·

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return this.ContactId.ToString();
        }

        #endregion

        #region · Internal Methods ·

        internal void AddDefaultResource()
        {
            Presence            defaultPresence = new Presence();
            XmppContactResource contactResource = new XmppContactResource(this.session, this, this.ContactId);
            XmppJid             resourceJid     = new XmppJid(this.contactId.UserName, this.ContactId.DomainName, Guid.NewGuid().ToString());

            // Add a default resource
            defaultPresence.TypeSpecified   = true;
            defaultPresence.From            = resourceJid;
            defaultPresence.Type            = PresenceType.Unavailable;

            defaultPresence.Items.Add(XmppContactResource.DefaultPresencePriorityValue);

            contactResource.Update(defaultPresence);

            this.resources.Add(contactResource);
        }

        internal void RefreshData(string name, XmppContactSubscriptionType subscription, IList<string> groups)
        {
            this.Name = ((name == null) ? String.Empty : name);

            if (!String.IsNullOrEmpty(this.Name))
            {
                this.displayName = this.name;
            }
            else
            {
                this.displayName = this.contactId.UserName;
            }

            this.Subscription   = subscription;
            
            if (groups != null && groups.Count > 0)
            {
                this.Groups.AddRange(groups);
            }
            else
            {
                this.Groups.Add("Contacts");
            }

            this.NotifyPropertyChanged(() => DisplayName);
            this.NotifyPropertyChanged(() => Groups);
        }

        internal void UpdatePresence(XmppJid jid, Presence presence)
        {
            lock (this.syncObject)
            {
                XmppContactResource resource = this.resources
                    .Where(contactResource => contactResource.ResourceId.Equals(jid))
                    .SingleOrDefault();

                if (resource == null)
                {
                    resource = new XmppContactResource(this.session, this, jid);
                    this.resources.Add(resource);
                }

                resource.Update(presence);

                // Remove the resource information if the contact has gone offline
                if (!resource.IsDefaultResource &&
                    resource.Presence.PresenceStatus == XmppPresenceState.Offline)
                {
                    this.resources.Remove(resource);
                }

                this.NotifyPropertyChanged(() => Presence);
                this.NotifyPropertyChanged(() => Resource);
            }
        }

        #endregion

        #region · Private Methods ·

        private XmppContactResource GetResource()
        {
            var q = from resource in this.resources
                    orderby resource.Presence.Priority descending
                    select resource;

            return q.FirstOrDefault();
        }

        #endregion
    }
}