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
using BabelIm.Net.Xmpp.Core;
using BabelIm.Net.Xmpp.Serialization.Extensions.SimpleCommunicationsBlocking;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client.Presence;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Roster;

namespace BabelIm.Net.Xmpp.InstantMessaging
{
    /// <summary>
    /// Contact's Roster
    /// </summary>
    public sealed class XmppRoster
        : ObservableObject, IEnumerable<XmppContact>, INotifyCollectionChanged
    {
        #region · INotifyCollectionChanged Members ·

        /// <summary>
        /// Occurs when the roster changes ( contacts added, removed, updated, ... )
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region · Events ·

        /// <summary>
        /// Occurs when the presence of a contact changes
        /// </summary>
        public event EventHandler ContactPresenceChanged;

        /// <summary>
        /// Occurs when the Contact's Roster is recevied
        /// </summary>
        public event EventHandler RosterReceived;

        #endregion

        #region · Fields ·

        private ObservableCollection<XmppContact>   contacts;
        private XmppSession                         session;
        private XmppConnection                      connection;
        private List<string>                        pendingMessages;

        #region · Subscriptions ·

        private IDisposable sessionStateSubscription;
        private IDisposable rosterNotificationSubscription;
        private IDisposable infoQueryErrorSubscription;
        private IDisposable presenceSubscription;

        #endregion

        #endregion

        #region · Indexers ·

        /// <summary>
        /// Gets the contact with the given JID
        /// </summary>
        /// <param name="jid">The bare JID</param>
        /// <returns></returns>
        public XmppContact this[string jid]
        {
            get
            {
                return this.contacts.Where(contact => contact.ContactId.BareIdentifier == jid).SingleOrDefault();
            }
        }

        #endregion
       
        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="XmppRoster"/> class
        /// </summary>
        internal XmppRoster(XmppSession session)
        {
            this.session            = session;
            this.connection         = session.Connection;
            this.contacts           = new ObservableCollection<XmppContact>();
            this.pendingMessages    = new List<string>();

            this.SubscribeToSessionState();
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Adds the given contact to the roster
        /// </summary>
        /// <param name="jid">Contact JID</param>
        /// <param name="name">Contact name</param>
        public XmppRoster AddContact(string jid, string name)
        {
            RosterQuery query   = new RosterQuery();
            IQ          iq      = new IQ();

            iq.ID   = XmppIdentifierGenerator.Generate();
            iq.Type = IQType.Set;
            iq.From = this.connection.UserId;

            RosterItem item = new RosterItem();

            item.Subscription   = RosterSubscriptionType.None;
            item.Jid            = jid;
            item.Name           = name;

            query.Items.Add(item);

            iq.Items.Add(query);

            this.pendingMessages.Add(iq.ID);

            this.connection.Send(iq);

            return this;
        }

        /// <summary>
        /// Deletes a user from the roster list
        /// </summary>
        public XmppRoster RemoveContact(XmppJid jid)
        {
            RosterQuery query   = new RosterQuery();
            IQ          iq      = new IQ();

            iq.ID   = XmppIdentifierGenerator.Generate();
            iq.Type = IQType.Set;
            iq.From = this.connection.UserId;

            RosterItem item = new RosterItem();

            item.Jid            = jid.BareIdentifier;
            item.Subscription   = RosterSubscriptionType.Remove;

            query.Items.Add(item);

            iq.Items.Add(query);

            this.pendingMessages.Add(iq.ID);

            this.connection.Send(iq);

            return this;
        }

        /// <summary>
        /// Request Roster list to the XMPP Server
        /// </summary>
        public XmppRoster RequestRosterList()
        {
            RosterQuery query   = new RosterQuery();
            IQ          iq      = new IQ();

            iq.ID   = XmppIdentifierGenerator.Generate();
            iq.Type = IQType.Get;
            iq.From = this.connection.UserId;

            iq.Items.Add(query);

            this.connection.Send(iq);

            return this;
        }

        /// <summary>
        /// Refreshes the blocked contacts list
        /// </summary>
        /// <returns></returns>
        public XmppRoster RefreshBlockedContacts()
        {
#warning TODO: Check if contact list should be stored in a separated collection or the information should be injected into XmppContact class
            if (this.session.ServiceDiscovery.SupportsBlocking)
            {
                IQ iq = new IQ
                {
                    ID      = XmppIdentifierGenerator.Generate(),
                    Type    = IQType.Get
                };

                iq.Items.Add(new BlockList());

                this.session.Send(iq);
            }

            return this;
        }

        /// <summary>
        /// Unblocks all blocked contacts
        /// </summary>
        public XmppRoster UnBlockAll()
        {
            if (this.session.ServiceDiscovery.SupportsBlocking)
            {
                IQ iq = new IQ
                {
                    ID      = XmppIdentifierGenerator.Generate(),
                    From    = this.session.UserId,
                    Type    = IQType.Set
                };

                iq.Items.Add(new UnBlock());

                this.session.Send(iq);
            }

            return this;
        }

        #endregion

        #region · Internal Methods ·

        /// <summary>
        /// Called when the presence of a contact is changed
        /// </summary>
        internal void OnContactPresenceChanged()
        {
            if (this.ContactPresenceChanged != null)
            {
                this.ContactPresenceChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        #region · IEnumerable<XmppContact> Members ·

        IEnumerator<XmppContact> IEnumerable<XmppContact>.GetEnumerator()
        {
            return this.contacts.GetEnumerator();
        }

        #endregion

        #region · IEnumerable Members ·

        public IEnumerator GetEnumerator()
        {
            return this.contacts.GetEnumerator();
        }

        #endregion

        #region · Private Methods ·

        private void AddUserContact()
        {
            this.contacts.Add
            (
                new XmppContact
                (
                    this.session,
                    this.session.UserId.BareIdentifier,
                    "",
                    XmppContactSubscriptionType.Both,
                    new List<string>(new string[] { "Buddies" })
                )
            );
        }

        #endregion

        #region · Message Subscriptions ·

        private void SubscribeToSessionState()
        {
            this.sessionStateSubscription = this.session
                .StateChanged
                .Subscribe
            (
                newState =>
                {
                    if (newState == XmppSessionState.LoggingIn)
                    {
                        this.Subscribe();
                    }
                    else if (newState == XmppSessionState.LoggedIn)
                    {
                        this.AddUserContact();
                    }
                    else if (newState == XmppSessionState.LoggingOut)
                    {
                        this.Unsubscribe();
                        this.contacts.Clear();

                        if (this.CollectionChanged != null)
                        {
                            this.InvokeAsynchronouslyInBackground
                            (
                                () =>
                                {
                                    this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                                }
                            );
                        }
                    }
                }
            );
        }

        private void Subscribe()
        {
            this.rosterNotificationSubscription = this.connection
                .OnRosterMessage
                .Subscribe(mesasge => this.OnRosterNotification(mesasge));

            this.infoQueryErrorSubscription = this.connection
                .OnInfoQueryMessage
                .Where(iq => iq.Type == IQType.Error)
                .Subscribe(message => this.OnQueryErrorReceived(message));

            this.presenceSubscription = this.connection
                .OnPresenceMessage
                .Subscribe(message => this.OnPresenceMessageReceived(message));

            this.contacts.CollectionChanged         += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        private void Unsubscribe()
        {
            if (this.rosterNotificationSubscription != null)
            {
                this.rosterNotificationSubscription.Dispose();
                this.rosterNotificationSubscription = null;
            }
            
            if (this.infoQueryErrorSubscription != null)
            {
                this.infoQueryErrorSubscription.Dispose();
                this.infoQueryErrorSubscription = null;
            }

            if (this.presenceSubscription != null)
            {
                this.presenceSubscription.Dispose();
                this.presenceSubscription = null;
            }

            this.contacts.CollectionChanged         -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        #endregion

        #region · Message Handlers ·

        private void OnRosterNotification(RosterQuery message)
        {
            // It's a roster management related message
            foreach (RosterItem item in message.Items)
            {
                XmppContact contact = this.Where(c => c.ContactId.BareIdentifier == item.Jid).FirstOrDefault();

                if (contact == null)
                {
                    // Create the new contact
                    XmppContact newContact = new XmppContact
                    (
                        this.session,
                        item.Jid,
                        item.Name,
                        (XmppContactSubscriptionType)item.Subscription,
                        item.Groups
                    );

                    // Add the contact to the roster
                    this.contacts.Add(newContact);
                }
                else
                {
                    switch (item.Subscription)
                    {
                        case RosterSubscriptionType.Remove:
                            this.contacts.Remove(contact);
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

            if (this.RosterReceived != null)
            {
                this.RosterReceived(this, new EventArgs());
            }
        }

        private void OnPresenceMessageReceived(Presence message)
        {
            XmppJid jid = message.From;

            if (jid.BareIdentifier == this.session.UserId.BareIdentifier)
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
                                this.session.Presence.Subscribed(jid);
                                break;

                            case PresenceType.Unavailable:
                                contact.UpdatePresence(jid, message);
                                break;

                            case PresenceType.Unsubscribe:
                                this.session.Presence.Unsuscribed(jid);
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
                                this.session.Presence.Subscribed(jid);
                                break;

                            case PresenceType.Unsubscribe:
                                this.session.Presence.Unsuscribed(jid);
                                break;
                        }
                    }
                }
            }
        }

        private void OnQueryErrorReceived(IQ message)
        {
            if (this.pendingMessages.Contains(message.ID))
            {
                this.pendingMessages.Remove(message.ID);
            }
        }

        #endregion

        #region · Change Notification ·

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InvokeAsynchronouslyInBackground
            (
                () =>
                {
                    if (this.CollectionChanged != null)
                    {
                        this.CollectionChanged(this, e);
                    }
                }
            );
        }

        #endregion
    }
}
