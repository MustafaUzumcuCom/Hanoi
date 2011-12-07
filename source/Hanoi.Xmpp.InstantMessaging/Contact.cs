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
using Hanoi.Serialization.Extensions.SimpleCommunicationsBlocking;
using Hanoi.Serialization.InstantMessaging.Client;
using Hanoi.Serialization.InstantMessaging.Presence;
using Hanoi.Serialization.InstantMessaging.Roster;

namespace Hanoi.Xmpp.InstantMessaging
{
    public sealed class Contact
    {
        private readonly Jid _contactId;
        private readonly Session _session;
        private readonly object _syncObject;
        private string _displayName;
        private List<string> _groups;
        private string _name;
        private List<ContactResource> _resources;
        private ContactSubscriptionType _subscription;

        internal Contact(Session session, string contactId, string name, ContactSubscriptionType subscription, IList<string> groups)
        {
            _session = session;
            _syncObject = new object();
            _contactId = contactId;
            _resources = new List<ContactResource>();

            RefreshData(name, subscription, groups);
            AddDefaultResource();
        }

        public Jid ContactId
        {
            get { return _contactId; }
        }

        public string Name
        {
            get { return _name; }
            private set
            {
                if (_name == value) 
                    return;

                _name = value;
                _session.OnContactMessage(this);
            }
        }

        public List<string> Groups
        {
            get { return _groups ?? (_groups = new List<string>()); }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (_displayName == value) 
                    return;

                _displayName = !string.IsNullOrEmpty(value) ? value : _contactId.UserName;

                Update();

                _session.OnContactMessage(this);
            }
        }

        public IEnumerable<ContactResource> Resources
        {
            get { return _resources ?? (_resources = new List<ContactResource>()); }
        }

        public ContactSubscriptionType Subscription
        {
            get { return _subscription; }
            set
            {
                if (_subscription == value) 
                    return;

                _subscription = value;
                _session.OnContactMessage(this);
            }
        }

        public ContactResource Resource
        {
            get { return GetResource(); }
        }

        public ContactPresence Presence
        {
            get { return GetResource().Presence; }
        }

        //TODO: Implement indicator if contact supports file transfer
        public bool SupportsFileTransfer
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsConference
        {
            get { return (Resource.Capabilities.Features.Where(f => f.Name == Features.MultiUserChat).Count() > 0); }
        }

        public bool SupportsChatStateNotifications
        {
            get
            {
                return
                    (Resource.Capabilities.Features.Where(f => f.Name == Features.ChatStateNotifications).Count() >
                     0);
            }
        }

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

            _session.Send(iq);
        }

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

            _session.Send(iq);
        }

        public void RequestSubscription()
        {
            _session.Presence.RequestSubscription(ContactId);
        }

        public void Block()
        {
            if (_session.ServiceDiscovery.SupportsBlocking)
            {
                var iq = new IQ
                             {
                                 ID = IdentifierGenerator.Generate(),
                                 From = _session.UserId,
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

                _session.Send(iq);
            }
        }

        /// <summary>
        ///   Unblock contact.
        /// </summary>
        public void UnBlock()
        {
            if (!_session.ServiceDiscovery.SupportsBlocking) 
                return;

            var iq = new IQ
                         {
                             ID = IdentifierGenerator.Generate(),
                             From = _session.UserId,
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

            _session.Send(iq);
        }

        public override string ToString()
        {
            return ContactId.ToString();
        }

        internal void AddDefaultResource()
        {
            var defaultPresence = new Serialization.InstantMessaging.Presence.Presence();
            var contactResource = new ContactResource(_session, this, ContactId);
            var resourceJid = new Jid(_contactId.UserName, ContactId.DomainName, Guid.NewGuid().ToString());

            // Add a default resource
            defaultPresence.TypeSpecified = true;
            defaultPresence.From = resourceJid;
            defaultPresence.Type = PresenceType.Unavailable;

            defaultPresence.Items.Add(ContactResource.DefaultPresencePriorityValue);

            contactResource.Update(defaultPresence);

            _resources.Add(contactResource);
        }

        internal void RefreshData(string name, ContactSubscriptionType subscription, IList<string> groups)
        {
            Name = (name ?? string.Empty);

            _displayName = !string.IsNullOrEmpty(Name) ? _name : _contactId.UserName;

            Subscription = subscription;

            if (groups != null && groups.Count > 0)
            {
                Groups.AddRange(groups);
            }
            else
            {
                Groups.Add("Contacts");
            }

            _session.OnContactMessage(this);
        }

        internal void UpdatePresence(Jid jid, Serialization.InstantMessaging.Presence.Presence presence)
        {
            lock (_syncObject)
            {
                var resource = _resources
                    .Where(contactResource => contactResource.ResourceId.Equals(jid))
                    .SingleOrDefault();

                if (resource == null)
                {
                    resource = new ContactResource(_session, this, jid);
                    _resources.Add(resource);
                }

                resource.Update(presence);

                if (!resource.IsDefaultResource && resource.Presence.PresenceStatus == PresenceState.Offline)
                    _resources.Remove(resource);

                _session.OnContactMessage(this);
            }
        }

        private ContactResource GetResource()
        {
            var q = from resource in _resources
                    orderby resource.Presence.Priority descending
                    select resource;

            return q.FirstOrDefault();
        }
    }
}