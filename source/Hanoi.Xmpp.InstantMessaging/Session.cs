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
    public sealed class Session : ISession
    {
        private readonly Dictionary<string, Chat> _chats;
        private readonly Connection _connection;
        private readonly Subject<Message> _messageReceivedSubject = new Subject<Message>();
        private readonly Subject<Contact> _contactChanged = new Subject<Contact>();
        private readonly Subject<SessionState> _stateChangedSubject = new Subject<SessionState>();

        private readonly PersonalEventing.PersonalEventing _personalEventing;
        private readonly ServiceDiscovery.ServiceDiscovery _serviceDiscovery;
        private readonly object _syncObject;
        private ClientCapabilitiesStorage _clientCapabilitiesStorage;
        private AvatarStorage _avatarStorage;
        private Activity _activity;
        private SessionEntityCapabilities _capabilities;
        private IDisposable _chatMessageSubscription;
        private IDisposable _errorMessageSubscription;
        private Presence _presence;
        private Roster _roster;
        private SessionState _state;
        

        public event EventHandler<AuthenticationFailiureEventArgs> AuthenticationFailed;

        public Session() 
            : this(FeatureDetection.Default, AuthenticatorFactory.Default, ConnectionFactory.Default, "")
        {
            
        }

        public Session(IFeatureDetection featureDetection, IAuthenticatorFactory authenticator, IConnectionFactory factory, string storagePath)
        {
            State = SessionState.LoggedOut;
            _chats = new Dictionary<string, Chat>();
            _syncObject = new object();
            _connection = new Connection(authenticator, featureDetection, factory);
            _serviceDiscovery = new ServiceDiscovery.ServiceDiscovery(this);
            _personalEventing = new PersonalEventing.PersonalEventing(this);
            _activity = new Activity(this);
            _roster = new Roster(this);

            FilePath.Directory = storagePath;
        }

        internal Connection Connection
        {
            get { return _connection; }
        }

        internal ClientCapabilitiesStorage ClientCapabilitiesStorage
        {
            get { return _clientCapabilitiesStorage; }
        }
        public IObservable<Contact>  ContactChanged
        {
            get { return _contactChanged.AsObservable(); }
        }
        public IObservable<Message> MessageReceived
        {
            get { return _messageReceivedSubject.AsObservable(); }
        }

        public IObservable<Contact> OnPresenceMessage
        {
            get { return _roster.OnContactPresence; }
        }

        public IObservable<RosterQuery> OnRosterMessage
        {
            get { return _connection.OnRosterMessage; }
        }

        public IObservable<SessionState> StateChanged
        {
            get { return _stateChangedSubject.AsObservable(); }
        }

        public Jid UserId
        {
            get { return _connection != null ? _connection.UserId : null; }
        }

        public Roster Roster
        {
            get { return _roster ?? (_roster = new Roster(this)); }
        }

        public Activity Activity
        {
            get { return _activity ?? (_activity = new Activity(this)); }
        }

        public SessionEntityCapabilities Capabilities
        {
            get { return _capabilities ?? (_capabilities = new SessionEntityCapabilities(this)); }
        }

        public SessionState State
        {
            get { return _state; }
            private set
            {
                if (_state == value)
                    return;
                _state = value;
                _stateChangedSubject.OnNext(_state);
            }
        }

        public Presence Presence
        {
            get { return _presence ?? (_presence = new Presence(this)); }
        }

        public ServiceDiscovery.ServiceDiscovery ServiceDiscovery
        {
            get { return _serviceDiscovery; }
        }

        public AvatarStorage AvatarStorage
        {
            get { return _avatarStorage; }
        }

        public PersonalEventing.PersonalEventing PersonalEventing
        {
            get { return _personalEventing; }
        }

        public ISession Open(ConnectionStringBuilder connectionString)
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                throw new XmppException("The session is already open");
            }

            _avatarStorage = new AvatarStorage(connectionString.UserId);
            _avatarStorage.Load();

            _clientCapabilitiesStorage = new ClientCapabilitiesStorage(connectionString.UserId);
            _clientCapabilitiesStorage.Load();
            State = SessionState.LoggingIn;

            Subscribe();
            Connection.Open(connectionString.ToString());

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
                        AvatarStorage.Save();
                        SetUnavailable();
                        _chats.Clear();
                    }

                    _connection.Close();
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

        public bool HasOpenChat(string contactId)
        {
            return (_chats != null && _chats.ContainsKey(contactId));
        }

        public bool HasOpenChat(Jid contactId)
        {
            return HasOpenChat(contactId.BareIdentifier);
        }

        public Chat CreateChat(string contactId)
        {
            return CreateChat(new Jid(contactId));
        }

        public Chat CreateChat(Jid contactId)
        {
            if (CheckSessionState())
            {
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
            return null;
        }

        public ChatRoom EnterChatRoom()
        {
            return EnterChatRoom(IdentifierGenerator.Generate());
        }

        public ChatRoom EnterChatRoom(string chatRoomName)
        {
            if (CheckSessionState())
            {
                var service = ServiceDiscovery.GetService(ServiceCategory.Conference);
                var chatRoomId = new Jid(chatRoomName, service.Identifier, UserId.UserName);
                var chatRoom = new ChatRoom(this, service, chatRoomId);
                chatRoom.Enter();

                return chatRoom;
            } 
            return null;
        }

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

        public ISession PublishMood(MoodType mood, string description)
        {
            var instance = new Mood
                               {
                                   MoodType = mood,
                                   Text = description
                               };

            PublishMood(new UserMoodEvent(null, instance));
            return this;
        }

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

        public ISession PublishDisplayName(string displayName)
        {
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

                _avatarStorage.SaveAvatar(UserId.BareIdentifier, hash, avatarData);
                _avatarStorage.Save();
            }
            finally
            {
                avatarData.Close();
                avatarData.Dispose();
            }

            return this;
        }

        public ISession SetUnavailable()
        {
            if (CheckSessionState())
                Presence.SetUnavailable();

            return this;
        }

        public ISession SetPresence(PresenceState newPresence)
        {
            SetPresence(newPresence, null);

            return this;
        }

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

        public ISession SetPresence(PresenceState newPresence, string status, int priority)
        {
            if (CheckSessionState())
                Presence.SetPresence(newPresence, status);
            return this;
        }

        internal void Send(object message)
        {
            _connection.Send(message);
        }

        private bool CheckSessionState()
        {
            return _connection != null && _connection.State == ConnectionState.Open;
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

            if (chat == null) 
                return;

            chat.ChatClosed -= OnChatClosed;

            if (chat.Contact != null)
            {
                _chats.Remove(chat.Contact.ContactId.BareIdentifier);
            }
        }

        private void OnConnectionClosed(object sender, EventArgs e)
        {
            Close();
        }

        internal void OnContactMessage(Contact contact)
        {
            _contactChanged.OnNext(contact);
        }
    }
}