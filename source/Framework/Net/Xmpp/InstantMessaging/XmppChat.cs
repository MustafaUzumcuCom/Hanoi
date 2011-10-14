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
using System.Collections.Generic;
using System.Linq;
using Hanoi.Core;
using Hanoi.Xmpp;
using Hanoi.Xmpp.Serialization.InstantMessaging.Client;

namespace BabelIm.Net.Xmpp.InstantMessaging {
    /// <summary>
    ///   Represents a chat conversation with a contact.
    /// </summary>
    public sealed class XmppChat {
        private IDisposable chatMessageSubscription;
        private XmppContact contact;
        private Queue<XmppMessage> pendingMessages;
        private XmppSession session;

        private IDisposable sessionStateSubscription;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "T:XmppChat" /> class.
        /// </summary>
        /// <param name = "session">The session.</param>
        /// <param name = "contact">The contact.</param>
        internal XmppChat(XmppSession session, XmppContact contact) {
            this.session = session;
            this.contact = contact;
            pendingMessages = new Queue<XmppMessage>();

            Subscribe();
        }

        /// <summary>
        ///   Gets the contact.
        /// </summary>
        /// <value>The contact.</value>
        public XmppContact Contact {
            get { return contact; }
        }

        /// <summary>
        ///   Gets the pending messages
        /// </summary>
        public Queue<XmppMessage> PendingMessages {
            get { return pendingMessages; }
        }

        private static object CreateChatStateNotification(XmppChatStateNotification notificationType) {
            switch (notificationType)
            {
                case XmppChatStateNotification.Active:
                    return new NotificationComposing();

                case XmppChatStateNotification.Composing:
                    return new NotificationComposing();

                case XmppChatStateNotification.Gone:
                    return new NotificationGone();

                case XmppChatStateNotification.Inactive:
                    return new NotificationInactive();

                case XmppChatStateNotification.Paused:
                    return new NotificationPaused();
            }

            return null;
        }

        /// <summary>
        ///   Occurs when a new message is received
        /// </summary>
        public event EventHandler ReceivedMessage;

        /// <summary>
        ///   Occurs before the chat is closed
        /// </summary>
        public event EventHandler ChatClosing;

        /// <summary>
        ///   Occurs when the chat is closed
        /// </summary>
        public event EventHandler ChatClosed;

        /// <summary>
        ///   Sends the message.
        /// </summary>
        /// <param name = "message">The message.</param>
        public string SendMessage(string message) {
            if (session == null)
            {
                throw new InvalidOperationException("Chat session is closed.");
            }

            var body = new MessageBody
                           {
                               Value = message
                           };

            var chatMessage = new Message
                                  {
                                      ID = XmppIdentifierGenerator.Generate(),
                                      Type = MessageType.Chat,
                                      From = session.UserId.ToString(),
                                      To = Contact.ContactId.ToString(),
                                  };

            if (Contact.SupportsChatStateNotifications)
            {
                chatMessage.Items.Add(CreateChatStateNotification(XmppChatStateNotification.Active));
            }

            chatMessage.Items.Add(body);

            session.Send(chatMessage);

            return chatMessage.ID;
        }

        /// <summary>
        ///   Sends a chat state notification
        /// </summary>
        /// <param name = "notificationType"></param>
        public void SendChatStateNotification(XmppChatStateNotification notificationType) {
            // Generate the notification only if the target entity supports it
            if (Contact.SupportsChatStateNotifications)
            {
                var message = new Message
                                  {
                                      ID = XmppIdentifierGenerator.Generate(),
                                      Type = MessageType.Chat,
                                      From = session.UserId.ToString(),
                                      To = Contact.ContactId.ToString(),
                                  };

                message.Items.Add(CreateChatStateNotification(notificationType));

                session.Send(message);
            }
        }

        /// <summary>
        ///   Closes this instance.
        /// </summary>
        public void Close() {
            if (ChatClosing != null)
            {
                ChatClosing(this, new EventArgs());
            }

            SendChatStateNotification(XmppChatStateNotification.Gone);
            pendingMessages.Clear();
            Unsubscribe();
            pendingMessages = null;

            if (ChatClosed != null)
            {
                ChatClosed(this, new EventArgs());
            }

            session = null;
            contact = null;
        }

        private void SubscribeToSessionState() {
            sessionStateSubscription = session
                .StateChanged
                .Where(s => s == XmppSessionState.LoggingOut)
                .Subscribe
                (
                    newState =>
                        {
                            Close();
                            Unsubscribe();
                        }
                );
        }

        private void Subscribe() {
            chatMessageSubscription = session
                .MessageReceived
                .Where(m => m.Type == MessageType.Chat && m.From.BareIdentifier == contact.ContactId.BareIdentifier)
                .Subscribe
                (
                    message =>
                        {
                            pendingMessages.Enqueue(message);

                            if (ReceivedMessage != null)
                            {
                                ReceivedMessage(message, new EventArgs());
                            }
                        }
                );
        }

        private void Unsubscribe() {
            if (chatMessageSubscription != null)
            {
                chatMessageSubscription.Dispose();
                chatMessageSubscription = null;
            }

            UnsubscribeFromSessionState();
        }

        private void UnsubscribeFromSessionState() {
            if (sessionStateSubscription != null)
            {
                sessionStateSubscription.Dispose();
                sessionStateSubscription = null;
            }
        }
    }
}