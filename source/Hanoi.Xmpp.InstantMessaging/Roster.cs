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
    public sealed class Roster : IEnumerable<XmppContact>, INotifyCollectionChanged
    {
        private readonly Connection connection;
        private readonly ObservableCollection<XmppContact> contacts;
        private readonly List<string> pendingMessages;
        private readonly Session session;

        private IDisposable infoQueryErrorSubscription;
        private IDisposable presenceSubscription;
        private IDisposable rosterNotificationSubscription;
        private IDisposable sessionStateSubscription;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Roster" /> class
        /// </summary>
        internal Roster(Session session)
        {
            this.session = session;
            connection = session.Connection;
            contacts = new ObservableCollection<XmppContact>();
            pendingMessages = new List<string>();

            SubscribeToSessionState();
        }

        /// <summary>
        ///   Gets the contact with the given JID
        /// </summary>
        /// <param name = "jid">The bare JID</param>
        /// <returns></returns>
        public XmppContact this[string jid]
        {
            get { return contacts.Where(contact => contact.ContactId.BareIdentifier == jid).SingleOrDefault(); }
        }

        #region IEnumerable<XmppContact> Members

        IEnumerator<XmppContact> IEnumerable<XmppContact>.GetEnumerator()
        {
            return contacts.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return contacts.GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChanged Members

        /// <summary>
        ///   Occurs when the roster changes ( contacts added, removed, updated, ... )
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        /// <summary>
        ///   Occurs when the presence of a contact changes
        /// </summary>
        public event EventHandler ContactPresenceChanged;

        /// <summary>
        ///   Occurs when the Contact's Roster is recevied
        /// </summary>
        public event EventHandler RosterReceived;

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
                             ID = XmppIdentifierGenerator.Generate(),
                             Type = IQType.Set,
                             From = connection.UserId
                         };

            var item = new RosterItem
                           {
                               Subscription = RosterSubscriptionType.None,
                               Jid = jid,
                               Name = name
                           };

            query.Items.Add(item);

            iq.Items.Add(query);

            pendingMessages.Add(iq.ID);

            connection.Send(iq);

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
                             ID = XmppIdentifierGenerator.Generate(), 
                             Type = IQType.Set, 
                             From = connection.UserId,
                         };

            var item = new RosterItem
                           {
                               Jid = jid.BareIdentifier,
                               Subscription = RosterSubscriptionType.Remove,
                           };

            query.Items.Add(item);

            iq.Items.Add(query);

            pendingMessages.Add(iq.ID);

            connection.Send(iq);

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
                             ID = XmppIdentifierGenerator.Generate(), 
                             Type = IQType.Get, 
                             From = connection.UserId
                         };

            iq.Items.Add(query);

            connection.Send(iq);

            return this;
        }

        /// <summary>
        ///   Refreshes the blocked contacts list
        /// </summary>
        /// <returns></returns>
        public Roster RefreshBlockedContacts()
        {
#warning TODO: Check if contact list should be stored in a separated collection or the information should be injected into XmppContact class
            if (session.ServiceDiscovery.SupportsBlocking)
            {
                var iq = new IQ
                             {
                                 ID = XmppIdentifierGenerator.Generate(),
                                 Type = IQType.Get
                             };

                iq.Items.Add(new BlockList());

                session.Send(iq);
            }

            return this;
        }

        /// <summary>
        ///   Unblocks all blocked contacts
        /// </summary>
        public Roster UnBlockAll()
        {
            if (session.ServiceDiscovery.SupportsBlocking)
            {
                var iq = new IQ
                             {
                                 ID = XmppIdentifierGenerator.Generate(),
                                 From = session.UserId,
                                 Type = IQType.Set
                             };

                iq.Items.Add(new UnBlock());

                session.Send(iq);
            }

            return this;
        }

        /// <summary>
        ///   Called when the presence of a contact is changed
        /// </summary>
        internal void OnContactPresenceChanged()
        {
            if (ContactPresenceChanged != null)
            {
                ContactPresenceChanged(this, EventArgs.Empty);
            }
        }

        private void AddUserContact()
        {
            contacts.Add
                (
                    new XmppContact
                        (
                        session,
                        session.UserId.BareIdentifier,
                        "",
                        XmppContactSubscriptionType.Both,
                        new List<string>(new[] { "Buddies" })
                        )
                );
        }

        private void SubscribeToSessionState()
        {
            sessionStateSubscription = session
                .StateChanged
                .Subscribe
                (
                    newState =>
                    {
                        if (newState == SessionState.LoggingIn)
                        {
                            Subscribe();
                        }
                        else if (newState == SessionState.LoggedIn)
                        {
                            AddUserContact();
                        }
                        else if (newState == SessionState.LoggingOut)
                        {
                            Unsubscribe();
                            contacts.Clear();

                            if (CollectionChanged != null)
                            {
                                Dispatcher.CurrentDispatcher.BeginInvoke(
                                    DispatcherPriority.Background,
                                    new Action(() => CollectionChanged(this,
                                                                       new NotifyCollectionChangedEventArgs(
                                                                           NotifyCollectionChangedAction.Reset))));
                            }
                        }
                    }
                );
        }

        private void Subscribe()
        {
            rosterNotificationSubscription = connection
                .OnRosterMessage
                .Subscribe(OnRosterNotification);

            infoQueryErrorSubscription = connection
                .OnInfoQueryMessage
                .Where(iq => iq.Type == IQType.Error)
                .Subscribe(OnQueryErrorReceived);

            presenceSubscription = connection
                .OnPresenceMessage
                .Subscribe(OnPresenceMessageReceived);

            contacts.CollectionChanged += OnCollectionChanged;
        }

        private void Unsubscribe()
        {
            if (rosterNotificationSubscription != null)
            {
                rosterNotificationSubscription.Dispose();
                rosterNotificationSubscription = null;
            }

            if (infoQueryErrorSubscription != null)
            {
                infoQueryErrorSubscription.Dispose();
                infoQueryErrorSubscription = null;
            }

            if (presenceSubscription != null)
            {
                presenceSubscription.Dispose();
                presenceSubscription = null;
            }

            contacts.CollectionChanged -= OnCollectionChanged;
        }

        private void OnRosterNotification(RosterQuery message)
        {
            // It's a roster management related message
            foreach (RosterItem item in message.Items)
            {
                XmppContact contact = this.Where(c => c.ContactId.BareIdentifier == item.Jid).FirstOrDefault();

                if (contact == null)
                {
                    // Create the new contact
                    var newContact = new XmppContact
                        (
                        session,
                        item.Jid,
                        item.Name,
                        (XmppContactSubscriptionType)item.Subscription,
                        item.Groups
                        );

                    // Add the contact to the roster
                    contacts.Add(newContact);
                }
                else
                {
                    switch (item.Subscription)
                    {
                        case RosterSubscriptionType.Remove:
                            contacts.Remove(contact);
                            break;

                        default:
                            // Update contact data
                            contact.RefreshData
                                (
                                    item.Name,
                                    (XmppContactSubscriptionType)item.Subscription,
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

            if (jid.BareIdentifier == session.UserId.BareIdentifier)
            {
#warning TODO: See how to handle our own presence changes from other resources
            }
            else
            {
                XmppContact contact = this[jid.BareIdentifier];

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
                                session.Presence.Subscribed(jid);
                                break;

                            case PresenceType.Unavailable:
                                contact.UpdatePresence(jid, message);
                                break;

                            case PresenceType.Unsubscribe:
                                session.Presence.Unsuscribed(jid);
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
                                session.Presence.Subscribed(jid);
                                break;

                            case PresenceType.Unsubscribe:
                                session.Presence.Unsuscribed(jid);
                                break;
                        }
                    }
                }
            }
        }

        private void OnQueryErrorReceived(IQ message)
        {
            if (pendingMessages.Contains(message.ID))
            {
                pendingMessages.Remove(message.ID);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                                {
                                    if (
                                        CollectionChanged !=
                                        null)
                                    {
                                        CollectionChanged
                                            (this, e);
                                    }
                                }
                        ));
        }
    }
}