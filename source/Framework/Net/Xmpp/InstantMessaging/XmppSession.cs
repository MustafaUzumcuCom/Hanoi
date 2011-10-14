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
    EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED2 TO,
    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using BabelIm.Net.Xmpp.InstantMessaging.EntityCaps;
using BabelIm.Net.Xmpp.InstantMessaging.MultiUserChat;
using BabelIm.Net.Xmpp.InstantMessaging.PersonalEventing;
using BabelIm.Net.Xmpp.InstantMessaging.ServiceDiscovery;
using Hanoi.Core;
using Hanoi.Core.Authentication;
using Hanoi.Serialization.Extensions.VCardTemp;
using Hanoi.Serialization.InstantMessaging.Client;
using Hanoi.Xmpp;
using Hanoi.Xmpp.Serialization.Extensions.PubSub;
using Hanoi.Xmpp.Serialization.Extensions.UserMood;
using Hanoi.Xmpp.Serialization.Extensions.UserTune;

namespace BabelIm.Net.Xmpp.InstantMessaging {
    /// <summary>
    ///   XMPP Instant Messaging Session
    /// </summary>
    public sealed class XmppSession
        : IXmppSession {
        private readonly AvatarStorage avatarStorage;
        private readonly Dictionary<string, XmppChat> chats;
        private readonly XmppClientCapabilitiesStorage clientCapabilitiesStorage;
        private readonly XmppConnection connection;

        private readonly Subject<XmppMessage> messageReceivedSubject = new Subject<XmppMessage>();
        private readonly XmppPersonalEventing personalEventing;
        private readonly XmppServiceDiscovery serviceDiscovery;
        private readonly Subject<XmppSessionState> stateChangedSubject = new Subject<XmppSessionState>();
        private readonly object syncObject;
        private XmppActivity activity;
        private XmppSessionEntityCapabilities capabilities;

        private IDisposable chatMessageSubscription;
        private IDisposable errorMessageSubscription;
        private XmppPresence presence;
        private XmppRoster roster;
        private XmppSessionState state;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppSession" /> class
        /// </summary>
        public XmppSession() {
            State = XmppSessionState.LoggedOut;
            avatarStorage = new AvatarStorage();
            chats = new Dictionary<string, XmppChat>();
            syncObject = new object();
            connection = new XmppConnection();
            serviceDiscovery = new XmppServiceDiscovery(this);
            personalEventing = new XmppPersonalEventing(this);
            activity = new XmppActivity(this);
            clientCapabilitiesStorage = new XmppClientCapabilitiesStorage();
            roster = new XmppRoster(this);

            avatarStorage.Load();
            clientCapabilitiesStorage.Load();
        }

        /// <summary>
        ///   Gets the <see cref = "XmppConnection" /> instance associated to the session
        /// </summary>
        internal XmppConnection Connection {
            get { return connection; }
        }

        /// <summary>
        ///   Gets the client capabilities storage
        /// </summary>
        internal XmppClientCapabilitiesStorage ClientCapabilitiesStorage {
            get { return clientCapabilitiesStorage; }
        }

        #region IXmppSession Members

        /// <summary>
        ///   Occurs when the authentications fails.
        /// </summary>
        public event EventHandler<XmppAuthenticationFailiureEventArgs> AuthenticationFailed;

        /// <summary>
        ///   Occurs when a message is received.
        /// </summary>
        public IObservable<XmppMessage> MessageReceived {
            get { return messageReceivedSubject.AsObservable(); }
        }

        /// <summary>
        ///   Occurs when the session state changes.
        /// </summary>
        public IObservable<XmppSessionState> StateChanged {
            get { return stateChangedSubject.AsObservable(); }
        }

        /// <summary>
        ///   Gets the User <see cref = "XmppJid">JID</see>
        /// </summary>
        public XmppJid UserId {
            get {
                if (connection != null)
                {
                    return connection.UserId;
                }

                return null;
            }
        }

        /// <summary>
        ///   Gets the session <see cref = "XmppRoster">Roster</see>
        /// </summary>
        public XmppRoster Roster {
            get {
                if (roster == null)
                {
                    roster = new XmppRoster(this);
                }

                return roster;
            }
        }

        /// <summary>
        ///   Gets the list of <see cref = "XmppActivity">activities</see>
        /// </summary>
        public XmppActivity Activity {
            get {
                if (activity == null)
                {
                    activity = new XmppActivity(this);
                }

                return activity;
            }
        }

        /// <summary>
        ///   Gets the client session supported features
        /// </summary>
        public XmppSessionEntityCapabilities Capabilities {
            get {
                if (capabilities == null)
                {
                    capabilities = new XmppSessionEntityCapabilities(this);
                }

                return capabilities;
            }
        }

        /// <summary>
        ///   Gets the session state
        /// </summary>
        public XmppSessionState State {
            get { return state; }
            private set {
                if (state != value)
                {
                    state = value;
                    stateChangedSubject.OnNext(state);
                }
            }
        }

        /// <summary>
        ///   Gets the presence
        /// </summary>
        public XmppPresence Presence {
            get {
                if (presence == null)
                {
                    presence = new XmppPresence(this);
                }

                return presence;
            }
        }

        /// <summary>
        ///   Gets the <see cref = "XmppSession">service discovery </see> instance associated to the session
        /// </summary>
        public XmppServiceDiscovery ServiceDiscovery {
            get { return serviceDiscovery; }
        }

        /// <summary>
        ///   Gets the avatar storage
        /// </summary>
        public AvatarStorage AvatarStorage {
            get { return avatarStorage; }
        }

        /// <summary>
        ///   Gets the <see cref = "XmppPersonalEventing">personal eventing</see> instance associated to the session
        /// </summary>
        public XmppPersonalEventing PersonalEventing {
            get { return personalEventing; }
        }

        /// <summary>
        ///   Opens a new Session with the given connection parameters
        /// </summary>
        /// <param name = "connectionString">Connection parameters</param>
        public IXmppSession Open(string connectionString) {
            if (connection != null && connection.State == XmppConnectionState.Open)
            {
                throw new XmppException("The session is already open");
            }

            State = XmppSessionState.LoggingIn;

            // Wire XmppConnection events
            Subscribe();

            // Perform the authentication
            connection.Open(connectionString);

            if (connection != null && connection.State == XmppConnectionState.Open)
            {
                // Send Roster Request
                Roster.RequestRosterList();

                // Set initial Presence status
                Presence.SetInitialPresence();

                // Advertise Capabilities
                Capabilities.AdvertiseCapabilities();

                // Discover server services
                ServiceDiscovery.DiscoverServices();

                // Discover personal eventing support
                PersonalEventing.DiscoverSupport();

                // Set as Logged In
                State = XmppSessionState.LoggedIn;
            }
            else
            {
                State = XmppSessionState.Error;
            }

            return this;
        }

        /// <summary>
        ///   Closes the Session
        /// </summary>
        public IXmppSession Close() {
            if (connection != null &&
                (connection.State == XmppConnectionState.Opening ||
                 connection.State == XmppConnectionState.Open))
            {
                try
                {
                    State = XmppSessionState.LoggingOut;

                    if (connection.State == XmppConnectionState.Open)
                    {
                        // Save session configuration
                        AvatarStorage.Save();

                        // Change presence to unavailable
                        SetUnavailable();

                        // Clear all chats
                        chats.Clear();
                    }

                    // Close connection
                    connection.Close();

                    // Unwire XmppConnection events
                    Unsubscribe();
                }
                catch
                {
                }
                finally
                {
                    State = XmppSessionState.LoggedOut;
                }
            }

            return this;
        }

        /// <summary>
        ///   Checks if a given user has an open chat session
        /// </summary>
        /// <param name = "contactId"></param>
        /// <returns></returns>
        public bool HasOpenChat(string contactId) {
            return (chats != null && chats.ContainsKey(contactId));
        }

        /// <summary>
        ///   Checks if a given user has an open chat session
        /// </summary>
        /// <param name = "contactId"></param>
        /// <returns></returns>
        public bool HasOpenChat(XmppJid contactId) {
            return HasOpenChat(contactId.BareIdentifier);
        }

        /// <summary>
        ///   Creates the chat.
        /// </summary>
        /// <param name = "contactId">The contact id.</param>
        /// <returns></returns>
        public XmppChat CreateChat(string contactId) {
            return CreateChat(new XmppJid(contactId));
        }

        /// <summary>
        ///   Creates the chat.
        /// </summary>
        /// <param name = "contactId">The contact id.</param>
        /// <returns></returns>
        public XmppChat CreateChat(XmppJid contactId) {
            CheckSessionState();

            XmppChat chat = null;

            lock (syncObject)
            {
                if (!chats.ContainsKey(contactId.BareIdentifier))
                {
                    chat = new XmppChat(this, Roster[contactId.BareIdentifier]);
                    chats.Add(contactId.BareIdentifier, chat);

                    chat.ChatClosed += OnChatClosed;
                }
                else
                {
                    chat = chats[contactId.BareIdentifier];
                }
            }

            return chat;
        }

        /// <summary>
        ///   Creates the chat room.
        /// </summary>
        /// <param name = "chatRoomName">Name of the chat room.</param>
        /// <returns></returns>
        public XmppChatRoom EnterChatRoom() {
            return EnterChatRoom(XmppIdentifierGenerator.Generate());
        }

        /// <summary>
        ///   Creates the chat room.
        /// </summary>
        /// <param name = "chatRoomName">Name of the chat room.</param>
        /// <returns></returns>
        public XmppChatRoom EnterChatRoom(string chatRoomName) {
            CheckSessionState();

            XmppService service = ServiceDiscovery.GetService(XmppServiceCategory.Conference);
            XmppChatRoom chatRoom = null;
            var chatRoomId = new XmppJid
                (
                chatRoomName,
                service.Identifier,
                UserId.UserName
                );

            if (service != null)
            {
                chatRoom = new XmppChatRoom(this, service, chatRoomId);
                chatRoom.Enter();
            }

            return chatRoom;
        }

        /// <summary>
        ///   Publishes user tune information
        /// </summary>
        public IXmppSession PublishTune(XmppUserTuneEvent tuneEvent) {
            var iq = new IQ();
            var pubsub = new PubSub();
            var publish = new PubSubPublish();
            var item = new PubSubItem();
            var tune = new Tune();

            iq.Items.Add(pubsub);
            pubsub.Items.Add(publish);
            publish.Items.Add(item);

            iq.From = UserId.ToString();
            iq.ID = XmppIdentifierGenerator.Generate();
            iq.Type = IQType.Set;
            publish.Node = XmppFeatures.UserMood;
            item.Item = tune;
            tune.Artist = tuneEvent.Artist;
            tune.Length = tuneEvent.Length;
            tune.Rating = tuneEvent.Rating;
            tune.Source = tuneEvent.Source;
            tune.Title = tuneEvent.Title;
            tune.Track = tuneEvent.Track;
            tune.Uri = tuneEvent.Uri;

            Send(iq);

            return this;
        }

        /// <summary>
        ///   Stops user tune publications
        /// </summary>
        public IXmppSession StopTunePublication() {
            var iq = new IQ();
            var pubsub = new PubSub();
            var publish = new PubSubPublish();
            var item = new PubSubItem();
            var tune = new Tune();

            iq.Items.Add(pubsub);
            pubsub.Items.Add(publish);
            publish.Items.Add(item);

            iq.From = UserId.ToString();
            iq.ID = XmppIdentifierGenerator.Generate();
            iq.Type = IQType.Set;
            publish.Node = XmppFeatures.UserMood;
            item.Item = tune;

            Send(iq);

            return this;
        }

        /// <summary>
        ///   Publishes user mood information
        /// </summary>
        public IXmppSession PublishMood(MoodType mood, string description) {
            var instance = new Mood();

            instance.MoodType = mood;
            instance.Text = description;

            PublishMood(new XmppUserMoodEvent(null, instance));

            return this;
        }

        /// <summary>
        ///   Publishes user mood information
        /// </summary>
        public IXmppSession PublishMood(XmppUserMoodEvent moodEvent) {
            var iq = new IQ();
            var pubsub = new PubSub();
            var publish = new PubSubPublish();
            var item = new PubSubItem();
            var mood = new Mood();

            iq.Items.Add(pubsub);
            pubsub.Items.Add(publish);
            publish.Items.Add(item);

            iq.From = UserId.ToString();
            iq.ID = XmppIdentifierGenerator.Generate();
            iq.Type = IQType.Set;
            publish.Node = XmppFeatures.UserMood;
            item.Item = mood;
            mood.MoodType = (MoodType) Enum.Parse(typeof (MoodType), moodEvent.Mood);
            mood.Text = moodEvent.Text;

            Send(iq);

            return this;
        }

        /// <summary>
        ///   Publishes the display name.
        /// </summary>
        /// <param name = "displayName">The display name.</param>
        public IXmppSession PublishDisplayName(string displayName) {
            // Publish the display name ( nickname )
            var iq = new IQ();
            var vcard = new VCardData();

            iq.ID = XmppIdentifierGenerator.Generate();
            iq.Type = IQType.Set;
            iq.From = UserId.ToString();

            vcard.NickName = displayName;

            iq.Items.Add(vcard);

            Send(iq);

            return this;
        }

        /// <summary>
        ///   Publishes the avatar.
        /// </summary>
        /// <param name = "mimetype">The mimetype.</param>
        /// <param name = "hash">The hash.</param>
        /// <param name = "avatarImage">The avatar image.</param>
        public IXmppSession PublishAvatar(string mimetype, string hash, Image avatarImage) {
            var avatarData = new MemoryStream();

            try
            {
                avatarImage.Save(avatarData, ImageFormat.Png);

                // Publish the avatar
                var iq = new IQ();
                var vcard = new VCardData();

                iq.ID = XmppIdentifierGenerator.Generate();
                iq.Type = IQType.Set;
                iq.From = UserId.ToString();

                vcard.Photo.Type = mimetype;
                vcard.Photo.Photo = avatarData.ToArray();

                iq.Items.Add(vcard);

                Send(iq);

                // Save the avatar
                avatarStorage.SaveAvatar(UserId.BareIdentifier, hash, avatarData);

                // Update session configuration
                avatarStorage.Save();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (avatarData != null)
                {
                    avatarData.Close();
                    avatarData.Dispose();
                    avatarData = null;
                }
            }

            return this;
        }

        /// <summary>
        ///   Sets as unavailable.
        /// </summary>
        public IXmppSession SetUnavailable() {
            CheckSessionState();

            Presence.SetUnavailable();

            return this;
        }

        /// <summary>
        ///   Sets the presence.
        /// </summary>
        /// <param name = "showAs">The show as.</param>
        public IXmppSession SetPresence(XmppPresenceState newPresence) {
            SetPresence(newPresence, null);

            return this;
        }

        /// <summary>
        ///   Sets the presence.
        /// </summary>
        /// <param name = "newPresence">The new presence state.</param>
        /// <param name = "status">The status.</param>
        public IXmppSession SetPresence(XmppPresenceState newPresence, string status) {
            switch (newPresence)
            {
                case XmppPresenceState.Invisible:
                    throw new NotImplementedException();

                case XmppPresenceState.Offline:
                    Close();
                    break;

                default:
                    SetPresence(newPresence, status, 0);
                    break;
            }

            return this;
        }

        /// <summary>
        ///   Sets the presence.
        /// </summary>
        /// <param name = "newPresence">The new presence state.</param>
        /// <param name = "status">The status.</param>
        /// <param name = "priority">The priority.</param>
        public IXmppSession SetPresence(XmppPresenceState newPresence, string status, int priority) {
            CheckSessionState();

            Presence.SetPresence(newPresence, status);

            return this;
        }

        #endregion

        /// <summary>
        ///   Sends a XMPP message to the server
        /// </summary>
        /// <param name = "message">The message to be sent</param>
        internal void Send(object message) {
            connection.Send(message);
        }

        private void CheckSessionState() {
            if (connection == null || connection.State != XmppConnectionState.Open)
            {
                throw new XmppException("The session is not valid.");
            }
        }

        private void Subscribe() {
            chatMessageSubscription = connection.OnMessageReceived
                .Where(m => m.Type == MessageType.Chat && m.ChatStateNotification != XmppChatStateNotification.None)
                .Subscribe
                (
                    message => { OnChatMessageReceived(message); }
                );
            errorMessageSubscription = connection.OnMessageReceived
                .Where(m => m.Type == MessageType.Error)
                .Subscribe
                (
                    message => { OnChatMessageReceived(message); }
                );

            connection.AuthenticationFailiure += OnAuthenticationFailiure;
            connection.ConnectionClosed += OnConnectionClosed;
        }

        private void Unsubscribe() {
            if (chatMessageSubscription != null)
            {
                chatMessageSubscription.Dispose();
                chatMessageSubscription = null;
            }
            if (errorMessageSubscription != null)
            {
                errorMessageSubscription.Dispose();
                errorMessageSubscription = null;
            }

            connection.AuthenticationFailiure -= OnAuthenticationFailiure;
            connection.ConnectionClosed -= OnConnectionClosed;
        }

        private void OnAuthenticationFailiure(object sender, XmppAuthenticationFailiureEventArgs e) {
            Close();

            if (AuthenticationFailed != null)
            {
                AuthenticationFailed(this, e);
            }
        }

        private void OnChatMessageReceived(XmppMessage message) {
            XmppChat chat = null;

            if (String.IsNullOrEmpty(message.Body) &&
                !chats.ContainsKey(message.From.BareIdentifier))
            {
            }
            else
            {
                if (!chats.ContainsKey(message.From.BareIdentifier))
                {
                    chat = CreateChat(message.From);
                }
                else
                {
                    chat = chats[message.From.BareIdentifier];
                }

                messageReceivedSubject.OnNext(message);
            }
        }

        private void OnErrorMessageReceived(XmppMessage message) {
            messageReceivedSubject.OnNext(message);
        }

        private void OnChatClosed(object sender, EventArgs e) {
            var chat = (XmppChat) sender;

            if (chat != null)
            {
                chat.ChatClosed -= OnChatClosed;

                if (chat.Contact != null)
                {
                    chats.Remove(chat.Contact.ContactId.BareIdentifier);
                }
            }
        }

        private void OnConnectionClosed(object sender, EventArgs e) {
            Close();
        }
        }
}