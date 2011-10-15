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
using System.Collections.Generic;
using System.Linq;
using Hanoi.Serialization.Extensions.SimpleCommunicationsBlocking;
using Hanoi.Serialization.InstantMessaging.Client;
using Hanoi.Serialization.InstantMessaging.Presence;
using Hanoi.Serialization.InstantMessaging.Roster;

namespace Hanoi.Xmpp.InstantMessaging
{
    /// <summary>
    ///   Represents a <see cref = "Roster" /> contact.
    /// </summary>
    public sealed class Contact
    {
        private readonly Jid contactId;
        private readonly Session session;
        private readonly object syncObject;
        private string displayName;
        private List<string> groups;
        private string name;
        private List<ContactResource> resources;
        private ContactSubscriptionType subscription;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "T:Contact" /> class.
        /// </summary>
        /// <param name = "session">The session.</param>
        /// <param name = "contactId">The contact id.</param>
        /// <param name = "name">The name.</param>
        /// <param name = "subscription">The subscription.</param>
        /// <param name = "groups">The groups.</param>
        internal Contact(Session session, string contactId, string name,
                             ContactSubscriptionType subscription, IList<string> groups)
        {
            this.session = session;
            syncObject = new object();
            this.contactId = contactId;
            resources = new List<ContactResource>();

            RefreshData(name, subscription, groups);
            AddDefaultResource();
        }

        /// <summary>
        ///   Gets the contact id.
        /// </summary>
        /// <value>The contact id.</value>
        public Jid ContactId
        {
            get { return contactId; }
        }

        /// <summary>
        ///   Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
            private set
            {
                if (name != value)
                {
                    name = value;

                    // NotifyPropertyChanged(() => Name);
                }
            }
        }

        /// <summary>
        ///   Gets the contact groups.
        /// </summary>
        /// <value>The groups.</value>
        public List<string> Groups
        {
            get
            {
                if (groups == null)
                {
                    groups = new List<string>();
                }

                return groups;
            }
        }

        /// <summary>
        ///   Gets the contact Display Name
        /// </summary>
        public string DisplayName
        {
            get { return displayName; }
            set
            {
                if (displayName != value)
                {
                    if (!String.IsNullOrEmpty(value))
                    {
                        displayName = value;
                    }
                    else
                    {
                        displayName = contactId.UserName;
                    }

                    Update();

                    //NotifyPropertyChanged(() => DisplayName);
                }
            }
        }

        /// <summary>
        ///   Gets the list available resources.
        /// </summary>
        /// <value>The resources.</value>
        public IEnumerable<ContactResource> Resources
        {
            get
            {
                if (resources == null)
                {
                    resources = new List<ContactResource>();
                }

                foreach (ContactResource resource in resources)
                {
                    yield return resource;
                }
            }
        }

        /// <summary>
        ///   Gets or sets the subscription.
        /// </summary>
        /// <value>The subscription.</value>
        public ContactSubscriptionType Subscription
        {
            get { return subscription; }
            set
            {
                if (subscription != value)
                {
                    subscription = value;

                    //NotifyPropertyChanged(() => Subscription);
                }
            }
        }

        /// <summary>
        ///   Gets the presence.
        /// </summary>
        /// <value>The presence.</value>
        public ContactResource Resource
        {
            get { return GetResource(); }
        }

        /// <summary>
        ///   Gets the presence.
        /// </summary>
        /// <value>The presence.</value>
        public ContactPresence Presence
        {
            get { return GetResource().Presence; }
        }

        /// <summary>
        ///   Gets a value that indicates if the contact supports File Transfer
        /// </summary>
        public bool SupportsFileTransfer
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        ///   Gets a value that indicates if the contact supports MUC
        /// </summary>
        public bool SupportsConference
        {
            get { return (Resource.Capabilities.Features.Where(f => f.Name == Features.MultiUserChat).Count() > 0); }
        }

        /// <summary>
        ///   Gets a value that indicates if the contact supports chat state notifications
        /// </summary>
        public bool SupportsChatStateNotifications
        {
            get
            {
                return
                    (Resource.Capabilities.Features.Where(f => f.Name == Features.ChatStateNotifications).Count() >
                     0);
            }
        }

        /// <summary>
        ///   Adds to group.
        /// </summary>
        /// <param name = "groupName">Name of the group.</param>
        public void AddToGroup(string groupName)
        {
            var iq = new IQ();
            var query = new RosterQuery();
            var item = new RosterItem();

            if (!Groups.Contains(groupName))
            {
                Groups.Add(groupName);
            }

            iq.Type = IQType.Set;

            item.Jid = ContactId.BareIdentifier;
            item.Name = Name;
            item.Subscription = (RosterSubscriptionType)Subscription;

            item.Groups.Add(groupName);

            query.Items.Add(item);
            iq.Items.Add(query);

            session.Send(iq);
        }

        /// <summary>
        ///   Updates the contact data.
        /// </summary>
        public void Update()
        {
            var iq = new IQ();
            var query = new RosterQuery();
            var item = new RosterItem();

            iq.Type = IQType.Set;

            item.Jid = ContactId.BareIdentifier;
            item.Name = DisplayName;
            item.Subscription = (RosterSubscriptionType)Subscription;

            item.Groups.AddRange(Groups);

            query.Items.Add(item);
            iq.Items.Add(query);

            session.Send(iq);
        }

        /// <summary>
        ///   Request subscription to he presence of the contanct
        /// </summary>
        public void RequestSubscription()
        {
            session.Presence.RequestSubscription(ContactId);
        }

        /// <summary>
        ///   Block contact
        /// </summary>
        public void Block()
        {
            if (session.ServiceDiscovery.SupportsBlocking)
            {
                var iq = new IQ
                             {
                                 ID = IdentifierGenerator.Generate(),
                                 From = session.UserId,
                                 Type = IQType.Set
                             };

                var block = new Block();

                block.Items.Add
                    (
                        new BlockItem
                            {
                                Jid = ContactId.BareIdentifier
                            }
                    );

                iq.Items.Add(block);

                session.Send(iq);
            }
        }

        /// <summary>
        ///   Unblock contact.
        /// </summary>
        public void UnBlock()
        {
            if (session.ServiceDiscovery.SupportsBlocking)
            {
                var iq = new IQ
                             {
                                 ID = IdentifierGenerator.Generate(),
                                 From = session.UserId,
                                 Type = IQType.Set
                             };

                var unBlock = new UnBlock();

                unBlock.Items.Add
                    (
                        new BlockItem
                            {
                                Jid = ContactId.BareIdentifier
                            }
                    );

                iq.Items.Add(unBlock);

                session.Send(iq);
            }
        }

        /// <summary>
        ///   Returns a <see cref = "T:System.String"></see> that represents the current <see cref = "T:System.Object"></see>.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.String"></see> that represents the current <see cref = "T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return ContactId.ToString();
        }

        internal void AddDefaultResource()
        {
            var defaultPresence = new Serialization.InstantMessaging.Presence.Presence();
            var contactResource = new ContactResource(session, this, ContactId);
            var resourceJid = new Jid(contactId.UserName, ContactId.DomainName, Guid.NewGuid().ToString());

            // Add a default resource
            defaultPresence.TypeSpecified = true;
            defaultPresence.From = resourceJid;
            defaultPresence.Type = PresenceType.Unavailable;

            defaultPresence.Items.Add(ContactResource.DefaultPresencePriorityValue);

            contactResource.Update(defaultPresence);

            resources.Add(contactResource);
        }

        internal void RefreshData(string name, ContactSubscriptionType subscription, IList<string> groups)
        {
            Name = ((name == null) ? String.Empty : name);

            if (!String.IsNullOrEmpty(Name))
            {
                displayName = this.name;
            }
            else
            {
                displayName = contactId.UserName;
            }

            Subscription = subscription;

            if (groups != null && groups.Count > 0)
            {
                Groups.AddRange(groups);
            }
            else
            {
                Groups.Add("Contacts");
            }

            //NotifyPropertyChanged(() => DisplayName);
            //NotifyPropertyChanged(() => Groups);
        }

        internal void UpdatePresence(Jid jid, Serialization.InstantMessaging.Presence.Presence presence)
        {
            lock (syncObject)
            {
                ContactResource resource = resources
                    .Where(contactResource => contactResource.ResourceId.Equals(jid))
                    .SingleOrDefault();

                if (resource == null)
                {
                    resource = new ContactResource(session, this, jid);
                    resources.Add(resource);
                }

                resource.Update(presence);

                // Remove the resource information if the contact has gone offline
                if (!resource.IsDefaultResource &&
                    resource.Presence.PresenceStatus == PresenceState.Offline)
                {
                    resources.Remove(resource);
                }

                //NotifyPropertyChanged(() => Presence);
                //NotifyPropertyChanged(() => Resource);
            }
        }

        private ContactResource GetResource()
        {
            var q = from resource in resources
                    orderby resource.Presence.Priority descending
                    select resource;

            return q.FirstOrDefault();
        }
    }
}