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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Threading;
using Hanoi.Serialization.Extensions.SimpleCommunicationsBlocking;
using Hanoi.Serialization.InstantMessaging.Client;
using Hanoi.Serialization.InstantMessaging.Presence;
using Hanoi.Serialization.InstantMessaging.Roster;

namespace Hanoi.Xmpp.InstantMessaging
{
    /// <summary>
    ///   Contact's Roster
    /// </summary>
    public sealed class Roster : IEnumerable<Contact>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event EventHandler ContactPresenceChanged;
        public event EventHandler RosterReceived;

        private readonly Connection _connection;
        private readonly ObservableCollection<Contact> _contacts;
        private readonly List<string> _pendingMessages;
        private readonly Session _session;

        private IDisposable _infoQueryErrorSubscription;
        private IDisposable _presenceSubscription;
        private IDisposable _rosterNotificationSubscription;

        private Subject<Contact> _onContactPresence = new Subject<Contact>();

        internal Roster(Session session)
        {
            _session = session;
            _connection = session.Connection;
            _contacts = new ObservableCollection<Contact>();
            _pendingMessages = new List<string>();

            SubscribeToSessionState();
        }

        /// <summary>
        ///   Gets the contact with the given JID
        /// </summary>
        /// <param name = "jid">The bare JID</param>
        /// <returns></returns>
        public Contact this[string jid]
        {
            get { return _contacts.SingleOrDefault(contact => contact.ContactId.BareIdentifier == jid); }
        }

        IEnumerator<Contact> IEnumerable<Contact>.GetEnumerator()
        {
            return _contacts.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _contacts.GetEnumerator();
        }


        /// <summary>
        ///   Adds the given contact to the roster
        /// </summary>
        /// <param name = "jid">Contact JID</param>
        /// <param name = "name">Contact name</param>
        public Roster AddContact(string jid, string name)
        {
            var query = new RosterQuery();
            var iq = new IQ
                         {
                             ID = IdentifierGenerator.Generate(),
                             Type = IQType.Set,
                             From = _connection.UserId
                         };

            var item = new RosterItem
                           {
                               Subscription = RosterSubscriptionType.None,
                               Jid = jid,
                               Name = name
                           };

            query.Items.Add(item);

            iq.Items.Add(query);

            _pendingMessages.Add(iq.ID);

            _connection.Send(iq);

            return this;
        }

        /// <summary>
        ///   Deletes a user from the roster list
        /// </summary>
        public Roster RemoveContact(Jid jid)
        {
            var query = new RosterQuery();
            var iq = new IQ
                         {
                             ID = IdentifierGenerator.Generate(),
                             Type = IQType.Set,
                             From = _connection.UserId,
                         };

            var item = new RosterItem
                           {
                               Jid = jid.BareIdentifier,
                               Subscription = RosterSubscriptionType.Remove,
                           };

            query.Items.Add(item);

            iq.Items.Add(query);

            _pendingMessages.Add(iq.ID);

            _connection.Send(iq);

            return this;
        }

        /// <summary>
        ///   Request Roster list to the XMPP Server
        /// </summary>
        public Roster RequestRosterList()
        {
            var query = new RosterQuery();
            var iq = new IQ
                         {
                             ID = IdentifierGenerator.Generate(),
                             Type = IQType.Get,
                             From = _connection.UserId
                         };

            iq.Items.Add(query);

            _connection.Send(iq);

            return this;
        }

        /// <summary>
        ///   Refreshes the blocked contacts list
        /// </summary>
        /// <returns></returns>
        public Roster RefreshBlockedContacts()
        {
            // TODO: Check if contact list should be stored in a separated collection 
            // TODO: or the information should be injected into Contact class
            if (_session.ServiceDiscovery.SupportsBlocking)
            {
                var iq = new IQ
                             {
                                 ID = IdentifierGenerator.Generate(),
                                 Type = IQType.Get
                             };

                iq.Items.Add(new BlockList());

                _session.Send(iq);
            }

            return this;
        }

        /// <summary>
        ///   Unblocks all blocked contacts
        /// </summary>
        public Roster UnBlockAll()
        {
            if (_session.ServiceDiscovery.SupportsBlocking)
            {
                var iq = new IQ
                             {
                                 ID = IdentifierGenerator.Generate(),
                                 From = _session.UserId,
                                 Type = IQType.Set
                             };

                iq.Items.Add(new UnBlock());

                _session.Send(iq);
            }

            return this;
        }

        /// <summary>
        ///   Called when the presence of a contact is changed
        /// </summary>
        [Obsolete]
        internal void OnContactPresenceChanged()
        {
            if (ContactPresenceChanged != null)
            {
                ContactPresenceChanged(this, EventArgs.Empty);
            }
        }

        private void AddUserContact()
        {
            _contacts.Add
                (
                    new Contact(_session, _session.UserId.BareIdentifier, "", ContactSubscriptionType.Both, new List<string>(new[] { "Buddies" }))
                );
        }

        private void SubscribeToSessionState()
        {
            _session
                .StateChanged
                .Subscribe
                (
                    newState =>
                        {
                            switch (newState)
                            {
                                case SessionState.LoggingIn:
                                    Subscribe();
                                    break;
                                case SessionState.LoggedIn:
                                    AddUserContact();
                                    break;
                                case SessionState.LoggingOut:
                                    Unsubscribe();
                                    _contacts.Clear();
                                    if (CollectionChanged != null)
                                    {
                                        Dispatcher.CurrentDispatcher.BeginInvoke(
                                            DispatcherPriority.Background,
                                            new Action(() => CollectionChanged(this,
                                                                               new NotifyCollectionChangedEventArgs(
                                                                                   NotifyCollectionChangedAction.Reset))));
                                    }
                                    break;
                            }
                        }
                );
        }

        private void Subscribe()
        {
            _rosterNotificationSubscription = _connection
                .OnRosterMessage
                .Subscribe(OnRosterNotification);

            _infoQueryErrorSubscription = _connection
                .OnInfoQueryMessage
                .Where(iq => iq.Type == IQType.Error)
                .Subscribe(OnQueryErrorReceived);

            _presenceSubscription = _connection
                .OnPresenceMessage
                .Subscribe(OnPresenceMessageReceived);

            _contacts.CollectionChanged += OnCollectionChanged;
        }

        private void Unsubscribe()
        {
            if (_rosterNotificationSubscription != null)
            {
                _rosterNotificationSubscription.Dispose();
                _rosterNotificationSubscription = null;
            }

            if (_infoQueryErrorSubscription != null)
            {
                _infoQueryErrorSubscription.Dispose();
                _infoQueryErrorSubscription = null;
            }

            if (_presenceSubscription != null)
            {
                _presenceSubscription.Dispose();
                _presenceSubscription = null;
            }

            _contacts.CollectionChanged -= OnCollectionChanged;
        }

        private void OnRosterNotification(RosterQuery message)
        {
            // It's a roster management related message
            foreach (RosterItem item in message.Items)
            {
                Contact contact = this.Where(c => c.ContactId.BareIdentifier == item.Jid).FirstOrDefault();

                if (contact == null)
                {
                    // Create the new contact
                    var newContact = new Contact
                        (
                        _session,
                        item.Jid,
                        item.Name,
                        (ContactSubscriptionType)item.Subscription,
                        item.Groups
                        );

                    // Add the contact to the roster
                    _contacts.Add(newContact);
                }
                else
                {
                    switch (item.Subscription)
                    {
                        case RosterSubscriptionType.Remove:
                            _contacts.Remove(contact);
                            break;

                        default:
                            // Update contact data
                            contact.RefreshData
                                (
                                    item.Name,
                                    (ContactSubscriptionType)item.Subscription,
                                    item.Groups
                                );
                            break;
                    }
                }
            }

            if (RosterReceived != null)
            {
                RosterReceived(this, new EventArgs());
            }
        }

        private void OnPresenceMessageReceived(Serialization.InstantMessaging.Presence.Presence message)
        {
            Jid jid = message.From;

            if (jid.BareIdentifier == _session.UserId.BareIdentifier)
            {
                // TODO: See how to handle our own presence changes from other resources
            }
            else
            {
                Contact contact = this[jid.BareIdentifier];

                if (contact != null)
                {
                    if (message.TypeSpecified)
                    {
                        switch (message.Type)
                        {
                            case PresenceType.Probe:
                                break;

                            case PresenceType.Subscribe:
                                // auto-accept subscription requests
                                _session.Presence.Subscribed(jid);
                                break;

                            case PresenceType.Unavailable:
                                contact.UpdatePresence(jid, message);
                                break;

                            case PresenceType.Unsubscribe:
                                _session.Presence.Unsuscribed(jid);
                                break;
                        }
                    }
                    else
                    {
                        contact.UpdatePresence(jid, message);
                    }
                }
                else
                {
                    if (message.TypeSpecified)
                    {
                        switch (message.Type)
                        {
                            case PresenceType.Probe:
                                break;

                            case PresenceType.Subscribe:
                                // auto-accept subscription requests
                                _session.Presence.Subscribed(jid);
                                break;

                            case PresenceType.Unsubscribe:
                                _session.Presence.Unsuscribed(jid);
                                break;
                        }
                    }
                }
            }
        }

        private void OnQueryErrorReceived(IQ message)
        {
            if (_pendingMessages.Contains(message.ID))
            {
                _pendingMessages.Remove(message.ID);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged(this, e);
            return;
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    if (CollectionChanged != null)
                                    {
                                        CollectionChanged(this, e);
                                    }
                                }
                        ));
        }

        internal void ContactPresence(Contact contact)
        {
            _onContactPresence.OnNext(contact);
        }

        public IObservable<Contact> OnContactPresence
        {
            get { return _onContactPresence.AsObservable(); }
        }
    }
}