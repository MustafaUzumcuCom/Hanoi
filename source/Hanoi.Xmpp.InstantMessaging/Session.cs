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
        private readonly AvatarStorage _avatarStorage;
        private readonly Dictionary<string, Chat> _chats;
        private readonly ClientCapabilitiesStorage _clientCapabilitiesStorage;
        private readonly Connection _connection;

        private readonly Subject<Message> _messageReceivedSubject = new Subject<Message>();
        private readonly PersonalEventing.PersonalEventing _personalEventing;
        private readonly ServiceDiscovery.ServiceDiscovery _serviceDiscovery;
        private readonly Subject<SessionState> _stateChangedSubject = new Subject<SessionState>();
        private readonly object _syncObject;
        private Activity _activity;
        private SessionEntityCapabilities _capabilities;

        private IDisposable _chatMessageSubscription;
        private IDisposable _errorMessageSubscription;
        private Presence _presence;
        private Roster _roster;
        private SessionState _state;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Session" /> class
        /// </summary>
        public Session()
        {
            State = SessionState.LoggedOut;
            _avatarStorage = new AvatarStorage();
            _chats = new Dictionary<string, Chat>();
            _syncObject = new object();
            _connection = new Connection();
            _serviceDiscovery = new ServiceDiscovery.ServiceDiscovery(this);
            _personalEventing = new PersonalEventing.PersonalEventing(this);
            _activity = new Activity(this);
            _clientCapabilitiesStorage = new ClientCapabilitiesStorage();
            _roster = new Roster(this);

            _avatarStorage.Load();
            _clientCapabilitiesStorage.Load();
        }

        public Session(IFeatureDetection featureDetection, IAuthenticatorFactory authenticator, IConnectionFactory factory)
        {
            State = SessionState.LoggedOut;
            _avatarStorage = new AvatarStorage();
            _chats = new Dictionary<string, Chat>();
            _syncObject = new object();

            _connection = new Connection(authenticator, featureDetection, factory);

            _serviceDiscovery = new ServiceDiscovery.ServiceDiscovery(this);
            _personalEventing = new PersonalEventing.PersonalEventing(this);
            _activity = new Activity(this);
            _clientCapabilitiesStorage = new ClientCapabilitiesStorage();
            _roster = new Roster(this);

            _avatarStorage.Load();
            _clientCapabilitiesStorage.Load();
        }

        /// <summary>
        ///   Gets the <see cref = "Hanoi.Connection" /> instance associated to the session
        /// </summary>
        internal Connection Connection
        {
            get { return _connection; }
        }

        /// <summary>
        ///   Gets the client capabilities storage
        /// </summary>
        internal ClientCapabilitiesStorage ClientCapabilitiesStorage
        {
            get { return _clientCapabilitiesStorage; }
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
            get { return _messageReceivedSubject.AsObservable(); }
        }

        public IObservable<Serialization.InstantMessaging.Presence.Presence> OnPresenceMessage
        {
            get { return _connection.OnPresenceMessage; }
        }

        public IObservable<RosterQuery> OnRosterMessage
        {
            get { return _connection.OnRosterMessage; }
        }

        /// <summary>
        ///   Occurs when the session state changes.
        /// </summary>
        public IObservable<SessionState> StateChanged
        {
            get { return _stateChangedSubject.AsObservable(); }
        }

        /// <summary>
        ///   Gets the User <see cref = "Jid">JID</see>
        /// </summary>
        public Jid UserId
        {
            get
            {
                if (_connection != null)
                {
                    return _connection.UserId;
                }

                return null;
            }
        }

        /// <summary>
        ///   Gets the session <see cref = "InstantMessaging.Roster">Roster</see>
        /// </summary>
        public Roster Roster
        {
            get { return _roster ?? (_roster = new Roster(this)); }
        }

        /// <summary>
        ///   Gets the list of <see cref = "InstantMessaging.PersonalEventing.Activity">activities</see>
        /// </summary>
        public Activity Activity
        {
            get { return _activity ?? (_activity = new Activity(this)); }
        }

        /// <summary>
        ///   Gets the client session supported features
        /// </summary>
        public SessionEntityCapabilities Capabilities
        {
            get { return _capabilities ?? (_capabilities = new SessionEntityCapabilities(this)); }
        }

        /// <summary>
        ///   Gets the session state
        /// </summary>
        public SessionState State
        {
            get { return _state; }
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    _stateChangedSubject.OnNext(_state);
                }
            }
        }

        /// <summary>
        ///   Gets the presence
        /// </summary>
        public Presence Presence
        {
            get { return _presence ?? (_presence = new Presence(this)); }
        }

        /// <summary>
        ///   Gets the <see cref = "Session">service discovery </see> instance associated to the session
        /// </summary>
        public ServiceDiscovery.ServiceDiscovery ServiceDiscovery
        {
            get { return _serviceDiscovery; }
        }

        /// <summary>
        ///   Gets the avatar storage
        /// </summary>
        public AvatarStorage AvatarStorage
        {
            get { return _avatarStorage; }
        }

        /// <summary>
        ///   Gets the <see cref = "InstantMessaging.PersonalEventing.PersonalEventing">personal eventing</see> instance associated to the session
        /// </summary>
        public PersonalEventing.PersonalEventing PersonalEventing
        {
            get { return _personalEventing; }
        }

        /// <summary>
        ///   Opens a new Session with the given connection parameters
        /// </summary>
        /// <param name = "connectionString">Connection parameters</param>
        public ISession Open(string connectionString)
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                throw new XmppException("The session is already open");
            }

            State = SessionState.LoggingIn;

            Subscribe();
            Connection.Open(connectionString);

            if (Connection != null && Connection.State == ConnectionState.Open)
            {
                Roster.RequestRosterList();

                Presence.SetInitialPresence();
                Capabilities.AdvertiseCapabilities();
                ServiceDiscovery.DiscoverServices();
                PersonalEventing.DiscoverSupport();

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
            if (_connection != null &&
                (_connection.State == ConnectionState.Opening ||
                 _connection.State == ConnectionState.Open))
            {
                try
                {
                    State = SessionState.LoggingOut;

                    if (_connection.State == ConnectionState.Open)
                    {
                        // Save session configuration
                        AvatarStorage.Save();

                        // Change presence to unavailable
                        SetUnavailable();

                        // Clear all chats
                        _chats.Clear();
                    }

                    // Close connection
                    _connection.Close();

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
            return (_chats != null && _chats.ContainsKey(contactId));
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

            Chat chat;

            lock (_syncObject)
            {
                if (!_chats.ContainsKey(contactId.BareIdentifier))
                {
                    chat = new Chat(this, Roster[contactId.BareIdentifier]);
                    _chats.Add(contactId.BareIdentifier, chat);

                    chat.ChatClosed += OnChatClosed;
                }
                else
                {
                    chat = _chats[contactId.BareIdentifier];
                }
            }

            return chat;
        }

        /// <summary>
        /// Creates the chat room.
        /// </summary>
        public ChatRoom EnterChatRoom()
        {
            return EnterChatRoom(IdentifierGenerator.Generate());
        }

        /// <summary>
        ///   Creates the chat room.
        /// </summary>
        /// <param name="chatRoomName">Name of the chat room.</param>
        public ChatRoom EnterChatRoom(string chatRoomName)
        {
            CheckSessionState();

            var service = ServiceDiscovery.GetService(ServiceCategory.Conference);

            ChatRoom chatRoom = null;
            var chatRoomId = new Jid(chatRoomName, service.Identifier, UserId.UserName);

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
                _avatarStorage.SaveAvatar(UserId.BareIdentifier, hash, avatarData);

                // Update session configuration
                _avatarStorage.Save();
            }
            finally
            {
                avatarData.Close();
                avatarData.Dispose();
            }

            return this;
        }

        /// <summary>
        /// Sets as unavailable.
        /// </summary>
        public ISession SetUnavailable()
        {
            CheckSessionState();

            Presence.SetUnavailable();

            return this;
        }

        /// <summary>
        /// Sets the presence.
        /// </summary>
        /// <param name="newPresence">The Presence state to show.</param>
        public ISession SetPresence(PresenceState newPresence)
        {
            SetPresence(newPresence, null);

            return this;
        }

        /// <summary>
        ///   Sets the presence.
        /// </summary>
        /// <param name="newPresence">The new presence state.</param>
        /// <param name="status">The status.</param>
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
            _connection.Send(message);
        }

        private void CheckSessionState()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                throw new XmppException("The session is not valid.");
            }
        }

        private void Subscribe()
        {
            _chatMessageSubscription = _connection.OnMessageReceived
                .Where(m => m.Type == MessageType.Chat)
                .Subscribe(OnChatMessageReceived);

            _errorMessageSubscription = _connection.OnMessageReceived
                .Where(m => m.Type == MessageType.Error)
                .Subscribe(OnChatMessageReceived);

            _connection.AuthenticationFailiure += OnAuthenticationFailiure;
            _connection.ConnectionClosed += OnConnectionClosed;
        }

        private void Unsubscribe()
        {
            if (_chatMessageSubscription != null)
            {
                _chatMessageSubscription.Dispose();
                _chatMessageSubscription = null;
            }
            if (_errorMessageSubscription != null)
            {
                _errorMessageSubscription.Dispose();
                _errorMessageSubscription = null;
            }

            _connection.AuthenticationFailiure -= OnAuthenticationFailiure;
            _connection.ConnectionClosed -= OnConnectionClosed;
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
            if (string.IsNullOrEmpty(message.Body) && !_chats.ContainsKey(message.From.BareIdentifier)) 
                return;

            if (!_chats.ContainsKey(message.From.BareIdentifier))
            {
                CreateChat(message.From);
            }
            
            _messageReceivedSubject.OnNext(message);
        }

        private void OnChatClosed(object sender, EventArgs e)
        {
            var chat = (Chat)sender;

            if (chat != null)
            {
                chat.ChatClosed -= OnChatClosed;

                if (chat.Contact != null)
                {
                    _chats.Remove(chat.Contact.ContactId.BareIdentifier);
                }
            }
        }

        private void OnConnectionClosed(object sender, EventArgs e)
        {
            Close();
        }
    }
}