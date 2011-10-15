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
using Hanoi.Authentication;
using Hanoi.Serialization.Extensions.PubSub;
using Hanoi.Serialization.Extensions.UserMood;
using Hanoi.Serialization.Extensions.UserTune;
using Hanoi.Serialization.Extensions.VCardTemp;
using Hanoi.Serialization.InstantMessaging.Client;
using Hanoi.Serialization.InstantMessaging.Roster;
using Hanoi.Xmpp.InstantMessaging.EntityCaps;
using Hanoi.Xmpp.InstantMessaging.MultiUserChat;
using Hanoi.Xmpp.InstantMessaging.PersonalEventing;
using Hanoi.Xmpp.InstantMessaging.ServiceDiscovery;

namespace Hanoi.Xmpp.InstantMessaging
{
    /// <summary>
    ///   XMPP Instant Messaging Session
    /// </summary>
    public sealed class Session : ISession
    {
        private readonly AvatarStorage avatarStorage;
        private readonly Dictionary<string, Chat> chats;
        private readonly ClientCapabilitiesStorage clientCapabilitiesStorage;
        private readonly Connection connection;

        private readonly Subject<Message> messageReceivedSubject = new Subject<Message>();
        private readonly PersonalEventing.PersonalEventing personalEventing;
        private readonly ServiceDiscovery.ServiceDiscovery serviceDiscovery;
        private readonly Subject<SessionState> stateChangedSubject = new Subject<SessionState>();
        private readonly object syncObject;
        private Activity activity;
        private SessionEntityCapabilities capabilities;

        private IDisposable chatMessageSubscription;
        private IDisposable errorMessageSubscription;
        private Presence presence;
        private Roster roster;
        private SessionState state;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Session" /> class
        /// </summary>
        public Session()
        {
            State = SessionState.LoggedOut;
            avatarStorage = new AvatarStorage();
            chats = new Dictionary<string, Chat>();
            syncObject = new object();
            connection = new Connection();
            serviceDiscovery = new ServiceDiscovery.ServiceDiscovery(this);
            personalEventing = new PersonalEventing.PersonalEventing(this);
            activity = new Activity(this);
            clientCapabilitiesStorage = new ClientCapabilitiesStorage();
            roster = new Roster(this);

            avatarStorage.Load();
            clientCapabilitiesStorage.Load();
        }

        public Session(IFeatureDetection featureDetection, IAuthenticatorFactory authenticator) {
            State = SessionState.LoggedOut;
            avatarStorage = new AvatarStorage();
            chats = new Dictionary<string, Chat>();
            syncObject = new object();
            connection = new Connection(authenticator, featureDetection);
            serviceDiscovery = new ServiceDiscovery.ServiceDiscovery(this);
            personalEventing = new PersonalEventing.PersonalEventing(this);
            activity = new Activity(this);
            clientCapabilitiesStorage = new ClientCapabilitiesStorage();
            roster = new Roster(this);

            avatarStorage.Load();
            clientCapabilitiesStorage.Load();
        }

        /// <summary>
        ///   Gets the <see cref = "Hanoi.Connection" /> instance associated to the session
        /// </summary>
        internal Connection Connection
        {
            get { return connection; }
        }

        /// <summary>
        ///   Gets the client capabilities storage
        /// </summary>
        internal ClientCapabilitiesStorage ClientCapabilitiesStorage
        {
            get { return clientCapabilitiesStorage; }
        }

        /// <summary>
        ///   Occurs when the authentications fails.
        /// </summary>
        public event EventHandler<AuthenticationFailiureEventArgs> AuthenticationFailed;

        /// <summary>
        ///   Occurs when a message is received.
        /// </summary>
        public IObservable<Message> MessageReceived
        {
            get { return messageReceivedSubject.AsObservable(); }
        }

        public IObservable<Serialization.InstantMessaging.Presence.Presence> OnPresenceMessage
        {
            get { return connection.OnPresenceMessage; }
        }

        public IObservable<RosterQuery> OnRosterMessage
        {
            get { return connection.OnRosterMessage; }
        }

        /// <summary>
        ///   Occurs when the session state changes.
        /// </summary>
        public IObservable<SessionState> StateChanged
        {
            get { return stateChangedSubject.AsObservable(); }
        }

        /// <summary>
        ///   Gets the User <see cref = "Jid">JID</see>
        /// </summary>
        public Jid UserId
        {
            get
            {
                if (connection != null)
                {
                    return connection.UserId;
                }

                return null;
            }
        }

        /// <summary>
        ///   Gets the session <see cref = "InstantMessaging.Roster">Roster</see>
        /// </summary>
        public Roster Roster
        {
            get
            {
                if (roster == null)
                {
                    roster = new Roster(this);
                }

                return roster;
            }
        }

        /// <summary>
        ///   Gets the list of <see cref = "InstantMessaging.PersonalEventing.Activity">activities</see>
        /// </summary>
        public Activity Activity
        {
            get
            {
                if (activity == null)
                {
                    activity = new Activity(this);
                }

                return activity;
            }
        }

        /// <summary>
        ///   Gets the client session supported features
        /// </summary>
        public SessionEntityCapabilities Capabilities
        {
            get
            {
                if (capabilities == null)
                {
                    capabilities = new SessionEntityCapabilities(this);
                }

                return capabilities;
            }
        }

        /// <summary>
        ///   Gets the session state
        /// </summary>
        public SessionState State
        {
            get { return state; }
            private set
            {
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
        public Presence Presence
        {
            get
            {
                if (presence == null)
                {
                    presence = new Presence(this);
                }

                return presence;
            }
        }

        /// <summary>
        ///   Gets the <see cref = "Session">service discovery </see> instance associated to the session
        /// </summary>
        public ServiceDiscovery.ServiceDiscovery ServiceDiscovery
        {
            get { return serviceDiscovery; }
        }

        /// <summary>
        ///   Gets the avatar storage
        /// </summary>
        public AvatarStorage AvatarStorage
        {
            get { return avatarStorage; }
        }

        /// <summary>
        ///   Gets the <see cref = "InstantMessaging.PersonalEventing.PersonalEventing">personal eventing</see> instance associated to the session
        /// </summary>
        public PersonalEventing.PersonalEventing PersonalEventing
        {
            get { return personalEventing; }
        }

        /// <summary>
        ///   Opens a new Session with the given connection parameters
        /// </summary>
        /// <param name = "connectionString">Connection parameters</param>
        public ISession Open(string connectionString)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                throw new XmppException("The session is already open");
            }

            State = SessionState.LoggingIn;

            // Wire Connection events
            Subscribe();

            // Perform the authentication
            connection.Open(connectionString);

            if (connection != null && connection.State == ConnectionState.Open)
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
                State = SessionState.LoggedIn;
            }
            else
            {
                State = SessionState.Error;
            }

            return this;
        }

        /// <summary>
        ///   Closes the Session
        /// </summary>
        public ISession Close()
        {
            if (connection != null &&
                (connection.State == ConnectionState.Opening ||
                 connection.State == ConnectionState.Open))
            {
                try
                {
                    State = SessionState.LoggingOut;

                    if (connection.State == ConnectionState.Open)
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

                    // Unwire Connection events
                    Unsubscribe();
                }
                catch
                {
                }
                finally
                {
                    State = SessionState.LoggedOut;
                }
            }

            return this;
        }

        /// <summary>
        ///   Checks if a given user has an open chat session
        /// </summary>
        /// <param name = "contactId"></param>
        /// <returns></returns>
        public bool HasOpenChat(string contactId)
        {
            return (chats != null && chats.ContainsKey(contactId));
        }

        /// <summary>
        ///   Checks if a given user has an open chat session
        /// </summary>
        /// <param name = "contactId"></param>
        /// <returns></returns>
        public bool HasOpenChat(Jid contactId)
        {
            return HasOpenChat(contactId.BareIdentifier);
        }

        /// <summary>
        ///   Creates the chat.
        /// </summary>
        /// <param name = "contactId">The contact id.</param>
        /// <returns></returns>
        public Chat CreateChat(string contactId)
        {
            return CreateChat(new Jid(contactId));
        }

        /// <summary>
        ///   Creates the chat.
        /// </summary>
        /// <param name = "contactId">The contact id.</param>
        /// <returns></returns>
        public Chat CreateChat(Jid contactId)
        {
            CheckSessionState();

            Chat chat = null;

            lock (syncObject)
            {
                if (!chats.ContainsKey(contactId.BareIdentifier))
                {
                    chat = new Chat(this, Roster[contactId.BareIdentifier]);
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
        public ChatRoom EnterChatRoom()
        {
            return EnterChatRoom(IdentifierGenerator.Generate());
        }

        /// <summary>
        ///   Creates the chat room.
        /// </summary>
        /// <param name = "chatRoomName">Name of the chat room.</param>
        /// <returns></returns>
        public ChatRoom EnterChatRoom(string chatRoomName)
        {
            CheckSessionState();

            Service service = ServiceDiscovery.GetService(ServiceCategory.Conference);
            ChatRoom chatRoom = null;
            var chatRoomId = new Jid
                (
                chatRoomName,
                service.Identifier,
                UserId.UserName
                );

            if (service != null)
            {
                chatRoom = new ChatRoom(this, service, chatRoomId);
                chatRoom.Enter();
            }

            return chatRoom;
        }

        /// <summary>
        ///   Publishes user tune information
        /// </summary>
        public ISession PublishTune(UserTuneEvent tuneEvent)
        {
            var iq = new IQ();
            var pubsub = new PubSub();
            var publish = new PubSubPublish();
            var item = new PubSubItem();
            var tune = new Tune();

            iq.Items.Add(pubsub);
            pubsub.Items.Add(publish);
            publish.Items.Add(item);

            iq.From = UserId.ToString();
            iq.ID = IdentifierGenerator.Generate();
            iq.Type = IQType.Set;
            publish.Node = Features.UserMood;
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
        public ISession StopTunePublication()
        {
            var iq = new IQ();
            var pubsub = new PubSub();
            var publish = new PubSubPublish();
            var item = new PubSubItem();
            var tune = new Tune();

            iq.Items.Add(pubsub);
            pubsub.Items.Add(publish);
            publish.Items.Add(item);

            iq.From = UserId.ToString();
            iq.ID = IdentifierGenerator.Generate();
            iq.Type = IQType.Set;
            publish.Node = Features.UserMood;
            item.Item = tune;

            Send(iq);

            return this;
        }

        /// <summary>
        ///   Publishes user mood information
        /// </summary>
        public ISession PublishMood(MoodType mood, string description)
        {
            var instance = new Mood();

            instance.MoodType = mood;
            instance.Text = description;

            PublishMood(new UserMoodEvent(null, instance));

            return this;
        }

        /// <summary>
        ///   Publishes user mood information
        /// </summary>
        public ISession PublishMood(UserMoodEvent moodEvent)
        {
            var iq = new IQ();
            var pubsub = new PubSub();
            var publish = new PubSubPublish();
            var item = new PubSubItem();
            var mood = new Mood();

            iq.Items.Add(pubsub);
            pubsub.Items.Add(publish);
            publish.Items.Add(item);

            iq.From = UserId.ToString();
            iq.ID = IdentifierGenerator.Generate();
            iq.Type = IQType.Set;
            publish.Node = Features.UserMood;
            item.Item = mood;
            mood.MoodType = (MoodType)Enum.Parse(typeof(MoodType), moodEvent.Mood);
            mood.Text = moodEvent.Text;

            Send(iq);

            return this;
        }

        /// <summary>
        ///   Publishes the display name.
        /// </summary>
        /// <param name = "displayName">The display name.</param>
        public ISession PublishDisplayName(string displayName)
        {
            // Publish the display name ( nickname )
            var iq = new IQ();
            var vcard = new VCardData();

            iq.ID = IdentifierGenerator.Generate();
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
        public ISession PublishAvatar(string mimetype, string hash, Image avatarImage)
        {
            var avatarData = new MemoryStream();

            try
            {
                avatarImage.Save(avatarData, ImageFormat.Png);

                // Publish the avatar
                var iq = new IQ();
                var vcard = new VCardData();

                iq.ID = IdentifierGenerator.Generate();
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
        public ISession SetUnavailable()
        {
            CheckSessionState();

            Presence.SetUnavailable();

            return this;
        }

        /// <summary>
        ///   Sets the presence.
        /// </summary>
        /// <param name = "showAs">The show as.</param>
        public ISession SetPresence(PresenceState newPresence)
        {
            SetPresence(newPresence, null);

            return this;
        }

        /// <summary>
        ///   Sets the presence.
        /// </summary>
        /// <param name = "newPresence">The new presence state.</param>
        /// <param name = "status">The status.</param>
        public ISession SetPresence(PresenceState newPresence, string status)
        {
            switch (newPresence)
            {
                case PresenceState.Invisible:
                    throw new NotImplementedException();

                case PresenceState.Offline:
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
        public ISession SetPresence(PresenceState newPresence, string status, int priority)
        {
            CheckSessionState();

            Presence.SetPresence(newPresence, status);

            return this;
        }

        /// <summary>
        ///   Sends a XMPP message to the server
        /// </summary>
        /// <param name = "message">The message to be sent</param>
        internal void Send(object message)
        {
            connection.Send(message);
        }

        private void CheckSessionState()
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                throw new XmppException("The session is not valid.");
            }
        }

        private void Subscribe()
        {
            chatMessageSubscription = connection.OnMessageReceived
                .Where(m => m.Type == MessageType.Chat && m.ChatStateNotification != ChatStateNotification.None)
                .Subscribe(OnChatMessageReceived);

            errorMessageSubscription = connection.OnMessageReceived
                .Where(m => m.Type == MessageType.Error)
                .Subscribe(OnChatMessageReceived);

            connection.AuthenticationFailiure += OnAuthenticationFailiure;
            connection.ConnectionClosed += OnConnectionClosed;
        }

        private void Unsubscribe()
        {
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

        private void OnAuthenticationFailiure(object sender, AuthenticationFailiureEventArgs e)
        {
            Close();

            if (AuthenticationFailed != null)
            {
                AuthenticationFailed(this, e);
            }
        }

        private void OnChatMessageReceived(Message message)
        {
            Chat chat = null;

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

        private void OnErrorMessageReceived(Message message)
        {
            messageReceivedSubject.OnNext(message);
        }

        private void OnChatClosed(object sender, EventArgs e)
        {
            var chat = (Chat)sender;

            if (chat != null)
            {
                chat.ChatClosed -= OnChatClosed;

                if (chat.Contact != null)
                {
                    chats.Remove(chat.Contact.ContactId.BareIdentifier);
                }
            }
        }

        private void OnConnectionClosed(object sender, EventArgs e)
        {
            Close();
        }
    }
}