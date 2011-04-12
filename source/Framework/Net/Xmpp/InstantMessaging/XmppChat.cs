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
using BabelIm.Net.Xmpp.Core;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client;

namespace BabelIm.Net.Xmpp.InstantMessaging
{
    /// <summary>
    /// Represents a chat conversation with a contact.
    /// </summary>
    public sealed class XmppChat
    {
        #region · Static Methods ·

        private static object CreateChatStateNotification(XmppChatStateNotification notificationType)
        {
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

        #endregion

        #region · Events ·

        /// <summary>
        /// Occurs when a new message is received
        /// </summary>
        public event EventHandler ReceivedMessage;

        /// <summary>
        /// Occurs before the chat is closed
        /// </summary>
        public event EventHandler ChatClosing;

        /// <summary>
        /// Occurs when the chat is closed
        /// </summary>
        public event EventHandler ChatClosed;

        #endregion

        #region · Fields ·

        private XmppContact         contact;
        private XmppSession         session;
        private Queue<XmppMessage>  pendingMessages;

        #region · Subscriptions ·

        private IDisposable chatMessageSubscription;
        private IDisposable sessionStateSubscription;

        #endregion

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the contact.
        /// </summary>
        /// <value>The contact.</value>
        public XmppContact Contact
        {
            get { return this.contact; }
        }

        /// <summary>
        /// Gets the pending messages
        /// </summary>
        public Queue<XmppMessage> PendingMessages
        {
            get { return this.pendingMessages; }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppChat"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="contact">The contact.</param>
        internal XmppChat(XmppSession session, XmppContact contact)
        {
            this.session            = session;
            this.contact            = contact;
            this.pendingMessages    = new Queue<XmppMessage>();

            this.Subscribe();
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public string SendMessage(string message)
        {
            if (this.session == null)
            {
                throw new InvalidOperationException("Chat session is closed.");
            }

            MessageBody body = new MessageBody
            {
                Value = message
            };

            Message chatMessage = new Message
            {
                ID      = XmppIdentifierGenerator.Generate(),
                Type    = MessageType.Chat,
                From    = this.session.UserId.ToString(),
                To      = this.Contact.ContactId.ToString(),
            };

            if (this.Contact.SupportsChatStateNotifications)
            {
                chatMessage.Items.Add(CreateChatStateNotification(XmppChatStateNotification.Active));
            }

            chatMessage.Items.Add(body);

            this.session.Send(chatMessage);

            return chatMessage.ID;
        }
        
        /// <summary>
        /// Sends a chat state notification
        /// </summary>
        /// <param name="notificationType"></param>
        public void SendChatStateNotification(XmppChatStateNotification notificationType)
        {
            // Generate the notification only if the target entity supports it
            if (this.Contact.SupportsChatStateNotifications)
            {
                Message message = new Message
                {
                    ID      = XmppIdentifierGenerator.Generate(),
                    Type    = MessageType.Chat,
                    From    = this.session.UserId.ToString(),
                    To      = this.Contact.ContactId.ToString(),
                };

                message.Items.Add(CreateChatStateNotification(notificationType));

                this.session.Send(message);
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            if (this.ChatClosing != null)
            {
                this.ChatClosing(this, new EventArgs());
            }

            this.SendChatStateNotification(XmppChatStateNotification.Gone);
            this.pendingMessages.Clear();
            this.Unsubscribe();
            this.pendingMessages = null;
           
            if (this.ChatClosed != null)
            {
                this.ChatClosed(this, new EventArgs());
            }

            this.session = null;
            this.contact = null;
        }

        #endregion

        #region · Message Subscriptions ·

        private void SubscribeToSessionState()
        {
            this.sessionStateSubscription = this.session
                .StateChanged
                .Where(s => s == XmppSessionState.LoggingOut)
                .Subscribe
            (
                newState =>
                {
                    this.Close();
                    this.Unsubscribe();
                }
            );
        }
        
        private void Subscribe()
        {
            this.chatMessageSubscription = this.session
                .MessageReceived
                .Where(m => m.Type == MessageType.Chat && m.From.BareIdentifier == this.contact.ContactId.BareIdentifier)
                .Subscribe
            (
                message =>
                {
                    this.pendingMessages.Enqueue(message);

                    if (this.ReceivedMessage != null)
                    {
                        this.ReceivedMessage(message, new EventArgs());
                    }
                }
            );
        }

        private void Unsubscribe()
        {
            if (this.chatMessageSubscription != null)
            {
                this.chatMessageSubscription.Dispose();
                this.chatMessageSubscription = null;
            }

            this.UnsubscribeFromSessionState();
        }

        private void UnsubscribeFromSessionState()
        {
            if (this.sessionStateSubscription != null)
            {
                this.sessionStateSubscription.Dispose();
                this.sessionStateSubscription = null;
            }
        }

        #endregion
    }
}
