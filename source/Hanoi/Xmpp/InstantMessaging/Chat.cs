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
using Hanoi.Serialization.InstantMessaging.Client;

namespace Hanoi.Xmpp.InstantMessaging
{
    public sealed class Chat
    {
        private IDisposable _chatMessageSubscription;
        private Contact _contact;
        private Queue<Message> _pendingMessages;
        private Session _session;
        private IDisposable _sessionStateSubscription;

        internal Chat(Session session, Contact contact)
        {
            _session = session;
            _contact = contact;
            _pendingMessages = new Queue<Message>();

            Subscribe();
        }

        public Contact Contact
        {
            get { return _contact; }
        }

        public Queue<Message> PendingMessages
        {
            get { return _pendingMessages; }
        }

        private static object CreateChatStateNotification(ChatStateNotification notificationType)
        {
            switch (notificationType)
            {
                case ChatStateNotification.Active:
                    return new NotificationComposing();

                case ChatStateNotification.Composing:
                    return new NotificationComposing();

                case ChatStateNotification.Gone:
                    return new NotificationGone();

                case ChatStateNotification.Inactive:
                    return new NotificationInactive();

                case ChatStateNotification.Paused:
                    return new NotificationPaused();
            }

            return null;
        }

        public event EventHandler ReceivedMessage;
        public event EventHandler ChatClosing;
        public event EventHandler ChatClosed;

        public string SendMessage(string message)
        {
            if (_session == null)
            {
                throw new InvalidOperationException("Chat session is closed.");
            }

            var body = new MessageBody
                           {
                               Value = message
                           };

            var chatMessage = new Serialization.InstantMessaging.Client.Message
                                  {
                                      ID = IdentifierGenerator.Generate(),
                                      Type = MessageType.Chat,
                                      From = _session.UserId.ToString(),
                                      To = Contact.ContactId.ToString(),
                                  };

            if (Contact.SupportsChatStateNotifications)
            {
                chatMessage.Items.Add(CreateChatStateNotification(ChatStateNotification.Active));
            }

            chatMessage.Items.Add(body);

            _session.Send(chatMessage);

            return chatMessage.ID;
        }

        public void SendChatStateNotification(ChatStateNotification notificationType)
        {
            // Generate the notification only if the target entity supports it
            if (!Contact.SupportsChatStateNotifications) 
                return;

            var message = new Serialization.InstantMessaging.Client.Message
                              {
                                  ID = IdentifierGenerator.Generate(),
                                  Type = MessageType.Chat,
                                  From = _session.UserId.ToString(),
                                  To = Contact.ContactId.ToString(),
                              };

            message.Items.Add(CreateChatStateNotification(notificationType));

            _session.Send(message);
        }

        public void Close()
        {
            if (ChatClosing != null)
            {
                ChatClosing(this, new EventArgs());
            }

            SendChatStateNotification(ChatStateNotification.Gone);
            _pendingMessages.Clear();
            Unsubscribe();
            _pendingMessages = null;

            if (ChatClosed != null)
            {
                ChatClosed(this, new EventArgs());
            }

            _session = null;
            _contact = null;
        }

        private void Subscribe()
        {
            _chatMessageSubscription = _session
                .MessageReceived
                .Where(m => m.Type == MessageType.Chat && m.From.BareIdentifier == _contact.ContactId.BareIdentifier)
                .Subscribe
                (
                    message =>
                    {
                        _pendingMessages.Enqueue(message);

                        if (ReceivedMessage != null)
                        {
                            ReceivedMessage(message, new EventArgs());
                        }
                    }
                );
        }

        private void Unsubscribe()
        {
            if (_chatMessageSubscription != null)
            {
                _chatMessageSubscription.Dispose();
                _chatMessageSubscription = null;
            }

            UnsubscribeFromSessionState();
        }

        private void UnsubscribeFromSessionState()
        {
            if (_sessionStateSubscription != null)
            {
                _sessionStateSubscription.Dispose();
                _sessionStateSubscription = null;
            }
        }
    }
}