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
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client;

namespace BabelIm.Net.Xmpp.Core
{
    /// <summary>
    /// Represents an XMPP message stanza
    /// </summary>
    public sealed class XmppMessage
    {
        #region · Fields ·

        private string                      identifier;
        private XmppJid                     from;
        private XmppJid                     to;
        private MessageType                 type;
        private string                      subject;
        private string                      body;
        private string                      thread;
        private string                      language;
        private XmppChatStateNotification   chatStateNotification;

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the message identifier
        /// </summary>
        public string Identifier
        {
            get { return this.identifier; }
        }

        /// <summary>
        /// Gets the message source JID .
        /// </summary>
        /// <value>From.</value>
        public XmppJid From
        {
            get { return this.from; }
        }

        /// <summary>
        /// Gets the message target JID .
        /// </summary>
        /// <value>From.</value>
        public XmppJid To
        {
            get { return this.to; }
        }

        /// <summary>
        /// Gets the message type
        /// </summary>
        public MessageType Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the message subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject
        {
            get { return this.subject; }
        }

        /// <summary>
        /// Gets the message body.
        /// </summary>
        /// <value>The body.</value>
        public string Body
        {
            get { return this.body; }
        }

        /// <summary>
        /// Gets the message thread.
        /// </summary>
        /// <value>The thread.</value>
        public string Thread
        {
            get { return this.thread; }
        }

        /// <summary>
        /// Gets the message language.
        /// </summary>
        /// <value>The language.</value>
        public string Language
        {
            get { return this.language; }
        }

        /// <summary>
        /// Gests the chat state notification type
        /// </summary>
        public XmppChatStateNotification ChatStateNotification
        {
            get { return chatStateNotification; }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        internal XmppMessage(Message message)
        {
            this.Initialize(message);
        }

        #endregion

        #region · Private methods ·

        private void Initialize(Message message)
        {
            this.identifier             = message.ID;
            this.from                   = message.From;
            this.to			            = message.To;
            this.language               = message.Lang;
            this.type                   = message.Type;
            this.thread                 = String.Empty;
            this.chatStateNotification  = XmppChatStateNotification.None;

            foreach (object item in message.Items)
            {
                if (item is MessageBody)
                {
                    this.body = ((MessageBody)item).Value;
                }
                else if (item is MessageSubject)
                {
                    this.subject = ((MessageSubject)item).Value;
                }
                else if (item is NotificationActive)
                {
                    this.chatStateNotification = XmppChatStateNotification.Active;
                }
                else if (item is NotificationComposing)
                {
                    this.chatStateNotification = XmppChatStateNotification.Composing;
                }
                else if (item is NotificationGone)
                {
                    this.chatStateNotification = XmppChatStateNotification.Gone;
                }
                else if (item is NotificationInactive)
                {
                    this.chatStateNotification = XmppChatStateNotification.Inactive;
                }
                else if (item is NotificationPaused)
                {
                    this.chatStateNotification = XmppChatStateNotification.Paused;
                }
            }
        }

        #endregion
    }
}