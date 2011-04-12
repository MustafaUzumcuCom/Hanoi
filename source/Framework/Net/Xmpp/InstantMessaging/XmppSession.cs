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
using BabelIm.Net.Xmpp.Core;
using BabelIm.Net.Xmpp.InstantMessaging.Configuration;
using BabelIm.Net.Xmpp.InstantMessaging.MultiUserChat;
using BabelIm.Net.Xmpp.InstantMessaging.PersonalEventing;
using BabelIm.Net.Xmpp.InstantMessaging.ServiceDiscovery;
using BabelIm.Net.Xmpp.Serialization.Extensions.PubSub;
using BabelIm.Net.Xmpp.Serialization.Extensions.UserMood;
using BabelIm.Net.Xmpp.Serialization.Extensions.UserTune;
using BabelIm.Net.Xmpp.Serialization.Extensions.VCard;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client;

namespace BabelIm.Net.Xmpp.InstantMessaging
{
    /// <summary>
    /// XMPP Instant Messaging Session
    /// </summary>
    public sealed class XmppSession 
        : IXmppSession
    {
        #region · Events ·

        /// <summary>
        /// Occurs when the authentications fails.
        /// </summary>
        public event EventHandler<XmppAuthenticationFailiureEventArgs> AuthenticationFailed;

        #endregion

        #region · Fields ·

        private XmppConnection                  connection;
        private XmppRoster                      roster;
        private XmppActivity                    activity;
        private XmppPresence                    presence;
        private XmppSessionEntityCapabilities   capabilities;
        private XmppSessionState                state;
        private XmppServiceDiscovery            serviceDiscovery;
        private XmppPersonalEventing            personalEventing;
        private XmppClientCapabilitiesStorage	clientCapabilitiesStorage;
        private AvatarStorage               	avatarStorage;
        private Dictionary<string, XmppChat>    chats;
        private object                          syncObject;

        #region · Subjects ·

        private readonly Subject<XmppMessage>       messageReceivedSubject  = new Subject<XmppMessage>();
        private readonly Subject<XmppSessionState>  stateChangedSubject     = new Subject<XmppSessionState>();

        #endregion

        #region · Subscriptions ·

        private IDisposable chatMessageSubscription;
        private IDisposable errorMessageSubscription;

        #endregion

        #endregion

        #region · IObservable<T> Action Properties ·

        /// <summary>
        /// Occurs when a message is received.
        /// </summary>
        public IObservable<XmppMessage> MessageReceived
        {
            get { return this.messageReceivedSubject.AsObservable(); }
        }

        /// <summary>
        /// Occurs when the session state changes.
        /// </summary>
        public IObservable<XmppSessionState> StateChanged
        {
            get { return this.stateChangedSubject.AsObservable(); }
        }

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the User <see cref="XmppJid">JID</see>
        /// </summary>
        public XmppJid UserId
        {
            get
            {
                if (this.connection != null)
                {
                    return this.connection.UserId;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the session <see cref="XmppRoster">Roster</see>
        /// </summary>
        public XmppRoster Roster
        {
            get 
            { 
                if (this.roster == null)
                {
                    this.roster = new XmppRoster(this);
                }

                return this.roster; 
            }
        }
        
        /// <summary>
        /// Gets the list of <see cref="XmppActivity">activities</see>
        /// </summary>
        public XmppActivity Activity
        {
            get
            {
                if (this.activity == null)
                {
                    this.activity = new XmppActivity(this);
                }
                
                return this.activity;
            }
        }

        /// <summary>
        /// Gets the client session supported features
        /// </summary>
        public XmppSessionEntityCapabilities Capabilities
        {
            get
            {
                if (this.capabilities == null)
                {
                    this.capabilities = new XmppSessionEntityCapabilities(this);
                }

                return this.capabilities;
            }
        }

        /// <summary>
        /// Gets the session state
        /// </summary>
        public XmppSessionState State
        {
            get { return this.state; }
            private set
            {
                if (this.state != value)
                {
                    this.state = value;
                    this.stateChangedSubject.OnNext(this.state);
                }
            }
        }

        /// <summary>
        /// Gets the presence
        /// </summary>
        public XmppPresence Presence
        {
            get
            {
                if (this.presence == null)
                {
                    this.presence = new XmppPresence(this);
                }

                return this.presence;
            }
        }

        /// <summary>
        /// Gets the <see cref="XmppSession">service discovery </see> instance associated to the session
        /// </summary>
        public XmppServiceDiscovery ServiceDiscovery
        {
            get { return this.serviceDiscovery; }
        }

        /// <summary>
        /// Gets the avatar storage
        /// </summary>
        public AvatarStorage AvatarStorage
        {
            get { return this.avatarStorage; }
        }

        /// <summary>
        /// Gets the <see cref="XmppPersonalEventing">personal eventing</see> instance associated to the session
        /// </summary>
        public XmppPersonalEventing PersonalEventing
        {
            get { return this.personalEventing; }
        }
        
        #endregion

        #region · Internal Properties ·

        /// <summary>
        /// Gets the <see cref="XmppConnection" /> instance associated to the session
        /// </summary>
        internal XmppConnection Connection
        {
            get { return this.connection; }
        }
        
        /// <summary>
        /// Gets the client capabilities storage
        /// </summary>
        internal XmppClientCapabilitiesStorage ClientCapabilitiesStorage
        {
            get { return this.clientCapabilitiesStorage; }
        }
                
        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="XmppSession"/> class
        /// </summary>
        public XmppSession()
        {
            this.State                      = XmppSessionState.LoggedOut;
            this.avatarStorage              = new AvatarStorage();
            this.chats                      = new Dictionary<string, XmppChat>();
            this.syncObject                 = new object();
            this.connection                 = new XmppConnection();
            this.serviceDiscovery           = new XmppServiceDiscovery(this);
            this.personalEventing           = new XmppPersonalEventing(this);
            this.activity                   = new XmppActivity(this);
            this.clientCapabilitiesStorage	= new XmppClientCapabilitiesStorage();
            this.roster                     = new XmppRoster(this);
            
            this.avatarStorage.Load();
            this.clientCapabilitiesStorage.Load();
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Opens a new Session with the given connection parameters
        /// </summary>
        /// <param name="connectionString">Connection parameters</param>
        public IXmppSession Open(string connectionString)
        {
            if (this.connection != null && this.connection.State == XmppConnectionState.Open)
            {
                throw new XmppException("The session is already open");
            }

            this.State = XmppSessionState.LoggingIn;

            // Wire XmppConnection events
            this.Subscribe();

            // Perform the authentication
            this.connection.Open(connectionString);

            if (this.connection != null && this.connection.State == XmppConnectionState.Open)
            {
                // Send Roster Request
                this.Roster.RequestRosterList();

                // Set initial Presence status
                this.Presence.SetInitialPresence();

                // Advertise Capabilities
                this.Capabilities.AdvertiseCapabilities();

                // Discover server services
                this.ServiceDiscovery.DiscoverServices();

                // Discover personal eventing support
                this.PersonalEventing.DiscoverSupport();

                // Set as Logged In
                this.State = XmppSessionState.LoggedIn;
            }
            else
            {
                this.State = XmppSessionState.Error;
            }

            return this;
        }

        /// <summary>
        /// Closes the Session
        /// </summary>
        public IXmppSession Close()
        {
            if (this.connection != null && 
                (this.connection.State == XmppConnectionState.Opening ||
                this.connection.State == XmppConnectionState.Open))
            {
                try
                {
                    this.State = XmppSessionState.LoggingOut;

                    if (this.connection.State == XmppConnectionState.Open)
                    {
                        // Save session configuration
                        this.AvatarStorage.Save();

                        // Change presence to unavailable
                        this.SetUnavailable();

                        // Clear all chats
                        this.chats.Clear();
                    }

                    // Close connection
                    this.connection.Close();

                    // Unwire XmppConnection events
                    this.Unsubscribe();
                }
                catch
                {
                }
                finally
                {
                    this.State = XmppSessionState.LoggedOut;
                }
            }

            return this;
        }
        
        #endregion
        
        #region · Chat Methods ·

        /// <summary>
        /// Checks if a given user has an open chat session
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public bool HasOpenChat(string contactId)
        {
            return (this.chats != null && this.chats.ContainsKey(contactId));
        }

        /// <summary>
        /// Checks if a given user has an open chat session
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public bool HasOpenChat(XmppJid contactId)
        {
            return this.HasOpenChat(contactId.BareIdentifier);
        }
        
        /// <summary>
        /// Creates the chat.
        /// </summary>
        /// <param name="contactId">The contact id.</param>
        /// <returns></returns>
        public XmppChat CreateChat(string contactId)
        {
            return this.CreateChat(new XmppJid(contactId));
        }

        /// <summary>
        /// Creates the chat.
        /// </summary>
        /// <param name="contactId">The contact id.</param>
        /// <returns></returns>
        public XmppChat CreateChat(XmppJid contactId)
        {
            this.CheckSessionState();

            XmppChat chat = null;

            lock (this.syncObject)
            {
                if (!this.chats.ContainsKey(contactId.BareIdentifier))
                {
                    chat = new XmppChat(this, this.Roster[contactId.BareIdentifier]);
                    this.chats.Add(contactId.BareIdentifier, chat);

                    chat.ChatClosed += new EventHandler(OnChatClosed);
                }
                else
                {
                    chat = this.chats[contactId.BareIdentifier];
                }
            }

            return chat;
        }
        
        #endregion
        
        #region · MUC Methods ·

        /// <summary>
        /// Creates the chat room.
        /// </summary>
        /// <param name="chatRoomName">Name of the chat room.</param>
        /// <returns></returns>
        public XmppChatRoom EnterChatRoom()
        {
            return this.EnterChatRoom(XmppIdentifierGenerator.Generate());
        }

        /// <summary>
        /// Creates the chat room.
        /// </summary>
        /// <param name="chatRoomName">Name of the chat room.</param>
        /// <returns></returns>
        public XmppChatRoom EnterChatRoom(string chatRoomName)
        {
            this.CheckSessionState();

            XmppService     service     = this.ServiceDiscovery.GetService(XmppServiceCategory.Conference);
            XmppChatRoom    chatRoom    = null;
            XmppJid         chatRoomId  = new XmppJid
            (
                chatRoomName,
                service.Identifier,
                this.UserId.UserName
            );

            if (service != null)
            {
                chatRoom = new XmppChatRoom(this, service, chatRoomId);
                chatRoom.Enter();
            }

            return chatRoom;
        }
        
        #endregion
                       
        #region · Publish Methods ·
        
        /// <summary>
        /// Publishes user tune information
        /// </summary>
        public IXmppSession PublishTune(XmppUserTuneEvent tuneEvent)
        {
            IQ 				iq 		= new IQ();
            PubSub			pubsub	= new PubSub();
            PubSubPublish 	publish = new PubSubPublish();
            PubSubItem		item	= new PubSubItem();
            Tune			tune	= new Tune();
            
            iq.Items.Add(pubsub);
            pubsub.Items.Add(publish);
            publish.Items.Add(item);       	
            
            iq.From			= this.UserId.ToString();
            iq.ID			= XmppIdentifierGenerator.Generate();
            iq.Type 		= IQType.Set;
            publish.Node	= XmppFeatures.UserMood;
            item.Item		= tune;
            tune.Artist		= tuneEvent.Artist;
            tune.Length		= tuneEvent.Length;
            tune.Rating		= tuneEvent.Rating;
            tune.Source		= tuneEvent.Source;
            tune.Title		= tuneEvent.Title;
            tune.Track		= tuneEvent.Track;
            tune.Uri		= tuneEvent.Uri;
            
            this.Send(iq);

            return this;
        }
        
        /// <summary>
        /// Stops user tune publications
        /// </summary>
        public IXmppSession StopTunePublication()
        {        
            IQ 				iq 		= new IQ();
            PubSub			pubsub	= new PubSub();
            PubSubPublish 	publish = new PubSubPublish();
            PubSubItem		item	= new PubSubItem();
            Tune			tune	= new Tune();
            
            iq.Items.Add(pubsub);
            pubsub.Items.Add(publish);
            publish.Items.Add(item);       	
            
            iq.From			= this.UserId.ToString();
            iq.ID			= XmppIdentifierGenerator.Generate();
            iq.Type 		= IQType.Set;
            publish.Node	= XmppFeatures.UserMood;
            item.Item		= tune;
            
            this.Send(iq);

            return this;
        }

        /// <summary>
        /// Publishes user mood information
        /// </summary>
        public IXmppSession PublishMood(MoodType mood, string description)
        {
            Mood instance = new Mood();

            instance.MoodType   = mood;
            instance.Text       = description;

            this.PublishMood(new XmppUserMoodEvent(null, instance));

            return this;
        }

        /// <summary>
        /// Publishes user mood information
        /// </summary>
        public IXmppSession PublishMood(XmppUserMoodEvent moodEvent)
        {
            IQ 				iq 		= new IQ();
            PubSub			pubsub	= new PubSub();
            PubSubPublish 	publish = new PubSubPublish();
            PubSubItem		item	= new PubSubItem();
            Mood 			mood 	= new Mood();
            
            iq.Items.Add(pubsub);
            pubsub.Items.Add(publish);
            publish.Items.Add(item);       	
            
            iq.From			= this.UserId.ToString();
            iq.ID			= XmppIdentifierGenerator.Generate();
            iq.Type 		= IQType.Set;
            publish.Node	= XmppFeatures.UserMood;
            item.Item		= mood;
            mood.MoodType	= (MoodType)Enum.Parse(typeof(MoodType), moodEvent.Mood);
            mood.Text		= moodEvent.Text;
            
            this.Send(iq);

            return this;
        }

        /// <summary>
        /// Publishes the display name.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        public IXmppSession PublishDisplayName(string displayName)
        {
            // Publish the display name ( nickname )
            IQ          iq      = new IQ();
            VCardData   vcard   = new VCardData();

            iq.ID   = XmppIdentifierGenerator.Generate();
            iq.Type = IQType.Set;
            iq.From = this.UserId.ToString();

            vcard.NickName = displayName;

            iq.Items.Add(vcard);

            this.Send(iq);

            return this;
        }

        /// <summary>
        /// Publishes the avatar.
        /// </summary>
        /// <param name="mimetype">The mimetype.</param>
        /// <param name="hash">The hash.</param>
        /// <param name="avatarImage">The avatar image.</param>
        public IXmppSession PublishAvatar(string mimetype, string hash, Image avatarImage)
        {
            MemoryStream avatarData = new MemoryStream();

            try
            {
                avatarImage.Save(avatarData, ImageFormat.Png);

                // Publish the avatar
                IQ          iq      = new IQ();
                VCardData   vcard   = new VCardData();

                iq.ID   = XmppIdentifierGenerator.Generate();
                iq.Type = IQType.Set;
                iq.From = this.UserId.ToString();

                vcard.Photo.Type    = mimetype;
                vcard.Photo.Photo   = avatarData.ToArray();

                iq.Items.Add(vcard);

                this.Send(iq);

                // Save the avatar
                this.avatarStorage.SaveAvatar(this.UserId.BareIdentifier, hash, avatarData);

                // Update session configuration
                this.avatarStorage.Save();
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

        #endregion

        #region · Presence Methods ·

        /// <summary>
        /// Sets as unavailable.
        /// </summary>
        public IXmppSession SetUnavailable()
        {
            this.CheckSessionState();

            this.Presence.SetUnavailable();

            return this;
        }

        /// <summary>
        /// Sets the presence.
        /// </summary>
        /// <param name="showAs">The show as.</param>
        public IXmppSession SetPresence(XmppPresenceState newPresence)
        {
            this.SetPresence(newPresence, null);

            return this;
        }

        /// <summary>
        /// Sets the presence.
        /// </summary>
        /// <param name="newPresence">The new presence state.</param>
        /// <param name="status">The status.</param>
        public IXmppSession SetPresence(XmppPresenceState newPresence, string status)
        {
            switch (newPresence)
            {
                case XmppPresenceState.Invisible:
                    throw new NotImplementedException();

                case XmppPresenceState.Offline:
                    this.Close();
                    break;

                default:
                    this.SetPresence(newPresence, status, 0);
                    break;
            }

            return this;
        }

        /// <summary>
        /// Sets the presence.
        /// </summary>
        /// <param name="newPresence">The new presence state.</param>
        /// <param name="status">The status.</param>
        /// <param name="priority">The priority.</param>
        public IXmppSession SetPresence(XmppPresenceState newPresence, string status, int priority)
        {
            this.CheckSessionState();

            this.Presence.SetPresence(newPresence, status);

            return this;
        }

        #endregion

        #region · Internal Methods ·

        /// <summary>
        /// Sends a XMPP message to the server
        /// </summary>
        /// <param name="message">The message to be sent</param>
        internal void Send(object message)
        {
            this.connection.Send(message);
        }

        #endregion

        #region · Private Methods ·

        private void CheckSessionState()
        {
            if (this.connection == null || this.connection.State != XmppConnectionState.Open)
            {
                throw new XmppException("The session is not valid.");
            }
        }

        #endregion

        #region · Message Subscriptions ·
        
        private void Subscribe()
        {
            this.chatMessageSubscription = this.connection.OnMessageReceived
                           .Where(m => m.Type == MessageType.Chat && m.ChatStateNotification != XmppChatStateNotification.None)
                           .Subscribe
            (
                message => { this.OnChatMessageReceived(message); }
            );
            this.errorMessageSubscription = this.connection.OnMessageReceived
                           .Where(m => m.Type == MessageType.Error)
                           .Subscribe
            (
                message => { this.OnChatMessageReceived(message); }
            );

            this.connection.AuthenticationFailiure	+= new EventHandler<XmppAuthenticationFailiureEventArgs>(OnAuthenticationFailiure);
            this.connection.ConnectionClosed        += new EventHandler(OnConnectionClosed);
        }

        private void Unsubscribe()
        {
            if (this.chatMessageSubscription != null)
            {
                this.chatMessageSubscription.Dispose();
                this.chatMessageSubscription = null;
            }
            if (this.errorMessageSubscription != null)
            {
                this.errorMessageSubscription.Dispose();
                this.errorMessageSubscription = null;
            }

            this.connection.AuthenticationFailiure  -= new EventHandler<XmppAuthenticationFailiureEventArgs>(OnAuthenticationFailiure);
            this.connection.ConnectionClosed        -= new EventHandler(OnConnectionClosed);
        }
        
        #endregion
        
        #region · Message Handlers ·

        private void OnAuthenticationFailiure(object sender, XmppAuthenticationFailiureEventArgs e)
        {
            this.Close();

            if (this.AuthenticationFailed != null)
            {
                this.AuthenticationFailed(this, e);
            }
        }

        private void OnChatMessageReceived(XmppMessage message)
        {
            XmppChat chat = null;

            if (String.IsNullOrEmpty(message.Body) &&
                !this.chats.ContainsKey(message.From.BareIdentifier))
            {
            }
            else
            {
                if (!this.chats.ContainsKey(message.From.BareIdentifier))
                {
                    chat = this.CreateChat(message.From);
                }
                else
                {
                    chat = this.chats[message.From.BareIdentifier];
                }

                this.messageReceivedSubject.OnNext(message);
            }
        }

        private void OnErrorMessageReceived(XmppMessage message)
        {
            this.messageReceivedSubject.OnNext(message);
        }

        private void OnChatClosed(object sender, EventArgs e)
        {
            XmppChat chat = (XmppChat)sender;

            if (chat != null)
            {
                chat.ChatClosed -= new EventHandler(OnChatClosed);

                if (chat.Contact != null)
                {
                    this.chats.Remove(chat.Contact.ContactId.BareIdentifier);
                }
            }
        }

        private void OnConnectionClosed(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
