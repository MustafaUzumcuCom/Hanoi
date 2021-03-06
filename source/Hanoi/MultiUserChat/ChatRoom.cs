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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Hanoi.Serialization.Extensions.MultiUserChat;
using Hanoi.Serialization.InstantMessaging.Client;
using Hanoi.Serialization.InstantMessaging.Presence;
using Hanoi.Xmpp.InstantMessaging.ServiceDiscovery;

namespace Hanoi.Xmpp.InstantMessaging.MultiUserChat
{
    public sealed class ChatRoom : ServiceDiscoveryObject
    {
        private readonly AutoResetEvent _createChatRoomEvent;
        private readonly AutoResetEvent _seekEnterChatRoomEvent;
        private Service _conferenceService;

        private IDisposable _messageReceivedSubscription;
        private IDisposable _presenceSubscription;
        private ObservableCollection<ChatRoomUser> _users;

        internal ChatRoom(Session session, Service conferenceService, Jid chatRoomId)
            : base(session, chatRoomId)
        {
            _conferenceService = conferenceService;
            _seekEnterChatRoomEvent = new AutoResetEvent(false);
            _createChatRoomEvent = new AutoResetEvent(false);
        }

        public ObservableCollection<ChatRoomUser> Users
        {
            get { return _users ?? (_users = new ObservableCollection<ChatRoomUser>()); }
        }

        public ChatRoom Enter()
        {
            var presence = new Serialization.InstantMessaging.Presence.Presence
                               {
                                   From = Session.UserId,
                                   To = Identifier
                               };

            presence.Items.Add(new Muc());

            Session.Send(presence);

            _createChatRoomEvent.WaitOne();

            return this;
        }

        public ChatRoom SendMessage(string message)
        {
            var chatMessage = new Serialization.InstantMessaging.Client.Message
                                  {
                                      ID = IdentifierGenerator.Generate(),
                                      Type = MessageType.GroupChat,
                                      From = Session.UserId.ToString(),
                                      To = Identifier
                                  };

            chatMessage.Items.Add
                (
                    new MessageBody
                        {
                            Value = message
                        }
                );

            Session.Send(chatMessage);

            return this;
        }

        public ChatRoom Invite(Contact contact)
        {
            var user = new MucUser();
            var message = new Serialization.InstantMessaging.Client.Message
                              {
                                  From = Session.UserId,
                                  To = Identifier.BareIdentifier,
                              };
            var invite = new MucUserInvite
                             {
                                 To = contact.ContactId.BareIdentifier,
                                 Reason = "Ninja invite"
                             };

            user.Items.Add(invite);
            message.Items.Add(user);

            Session.Send(message);

            return this;
        }

        public void Close()
        {
            var presence = new Serialization.InstantMessaging.Presence.Presence
                               {
                                   Id = IdentifierGenerator.Generate(),
                                   To = Identifier,
                                   Type = PresenceType.Unavailable
                               };

            PendingMessages.Add(presence.Id);

            Session.Send(presence);
        }

        protected override void Subscribe()
        {
            base.Subscribe();

            _messageReceivedSubscription = Session
                .MessageReceived
                .Where(m => m.Type == MessageType.GroupChat && m.From.BareIdentifier.Equals(Identifier.BareIdentifier))
                .Subscribe(OnMultiUserChatMessageReceived);

            _presenceSubscription = Session.Connection
                .OnPresenceMessage
                .Where(message => message.From.Equals(Identifier.ToString()))
                .Subscribe(OnPresenceMessageReceived);
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();

            if (_messageReceivedSubscription != null)
            {
                _messageReceivedSubscription.Dispose();
                _messageReceivedSubscription = null;
            }

            if (_presenceSubscription != null)
            {
                _presenceSubscription.Dispose();
                _presenceSubscription = null;
            }
        }

        private void OnMultiUserChatMessageReceived(Message message)
        {
            _createChatRoomEvent.Set();
            _seekEnterChatRoomEvent.Set();
        }

        private void OnPresenceMessageReceived(Serialization.InstantMessaging.Presence.Presence message)
        {
            if (message.Items != null && message.Items.Count > 0)
            {
                foreach (var item in message.Items)
                {
                    if (item is MucUser)
                    {
                        ProcessMucUser(item as MucUser);
                    }
                }
            }

            _createChatRoomEvent.Set();
            _seekEnterChatRoomEvent.Set();
        }

        private static void ProcessMucUser(MucUser item)
        {
            // Get the Status code
            var status = item.Items.OfType<MucUserStatus>().FirstOrDefault();

            if (status == null) 
                return;

            switch (status.Code)
            {
                case 100:
                    // stanza : message or presence
                    // context: Entering a room
                    // purpose: Inform user that any occupant is allowed to see the user's full JID
                    break;

                case 101:
                    // stanza : message (out of band)
                    // context: Affiliation change
                    // purpose: Inform user that his or her affiliation changed while not in the room
                    break;

                case 102:
                    // stanza : message
                    // context: Configuration change
                    // purpose: Inform occupants that room now shows unavailable members
                    break;

                case 103:
                    // stanza : message
                    // context: Configuration change
                    // purpose: Inform occupants that room now does not show unavailable members
                    break;

                case 104:
                    // stanza : message
                    // context: Configuration change
                    // purpose: Inform occupants that a non-privacy-related room configuration change has occurred
                    break;

                case 110:
                    // stanza : presence
                    // context: Any room presence
                    // purpose: Inform user that presence refers to one of its own room occupants</purpose>
                    break;

                case 170:
                    // stanza : message or initial presence
                    // context: Configuration change
                    // purpose: Inform occupants that room logging is now enabled
                    break;

                case 171:
                    // stanza : message
                    // context: Configuration change
                    // purpose: Inform occupants that room logging is now disabled
                    break;

                case 172:
                    // stanza : message
                    // context: Configuration change
                    // purpose: Inform occupants that the room is now non-anonymous
                    break;

                case 173:
                    // stanza : message
                    // context: Configuration change
                    // purpose: Inform occupants that the room is now semi-anonymous
                    break;

                case 174:
                    // stanza : message
                    // context: Configuration change
                    // purpose: Inform occupants that the room is now fully-anonymous
                    break;

                case 201:
                    // stanza : presence
                    // context: Entering a room
                    // purpose: Inform user that a new room has been created
                    break;

                case 210:
                    // stanza : presence
                    // context: Entering a room
                    // purpose: Inform user that service has assigned or modified occupant's roomnick
                    break;

                case 301:
                    // stanza : presence
                    // context: Removal from room
                    // purpose: Inform user that he or she has been banned from the room
                    break;

                case 303:
                    // stanza : presence
                    // context: Exiting a room
                    // purpose: Inform all occupants of new room nickname
                    break;

                case 307:
                    // stanza : presence
                    // context: Removal from room
                    // purpose: Inform user that he or she has been kicked from the room
                    break;

                case 321:
                    // stanza : presence
                    // context: Removal from room
                    // purpose: Inform user that he or she is being removed from the room
                    //          because of an affiliation change
                    break;

                case 322:
                    // stanza : presence
                    // context: Removal from room
                    // purpose: Inform user that he or she is being removed from the room
                    //          because the room has been changed to members-only and the user
                    //          is not a member
                    break;

                case 332:
                    // stanza : presence
                    // context: Removal from room
                    // purpose: Inform user that he or she is being removed from the room
                    //          because of a system shutdown
                    break;
            }
        }
    }
}