/*
    Copyright (c) 2008 - 2010, Carlos Guzmán Álvarez

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
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using BabelIm.Contracts;
using BabelIm.Infrastructure;
using BabelIm.IoC;
using BabelIm.Net.Xmpp.Core;
using BabelIm.Net.Xmpp.InstantMessaging;

namespace BabelIm.ViewModels {
    /// <summary>
    ///   View model for chat views
    /// </summary>
    /// <remarks>
    ///   Bring paragraphs into view seeing here:
    ///   http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/858200c8-6dc8-4a26-a1a3-9bbe7b6ea106
    /// </remarks>
    public sealed class ChatViewModel
        : ViewModelBase {
        private readonly FlowDocument conversation;
        private XmppChat chat;
        private XmppChatStateNotification chatStateNotification;
        private RelayCommand closeCommand;
        private bool hasUnreadMessages;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ChatViewModel" /> class
        /// </summary>
        /// <param name = "container"></param>
        /// <param name = "chat"></param>
        public ChatViewModel(XmppChat chat) {
            this.chat = chat;
            // this.emoticonPackage    = this.ResolveEmoticonPackage();

            conversation = new FlowDocument();
            this.chat.ChatClosed += chat_ChatClosed;
            this.chat.ReceivedMessage += chat_ReceivedMessage;

            NotifyPropertyChanged(() => Contact);

            ShowMessages();
        }

        /// <summary>
        ///   Gets the close chat command.
        /// </summary>
        /// <value>The add new command.</value>
        public RelayCommand CloseCommand {
            get {
                if (closeCommand == null)
                {
                    closeCommand = new RelayCommand
                        (
                        () => OnClose()
                        );
                }

                return closeCommand;
            }
        }

        /// <summary>
        ///   Gets the chat conversation as a <see cref = "FlowDocument" />
        /// </summary>
        public FlowDocument Conversation {
            get { return conversation; }
        }

        /// <summary>
        ///   Gets the chat contact information
        /// </summary>
        public XmppContact Contact {
            get {
                if (chat != null)
                {
                    return chat.Contact;
                }

                return null;
            }
        }

        /// <summary>
        ///   Gets a value that indicates if there are messages unreaded
        /// </summary>
        public bool HasUnreadMessages {
            get { return hasUnreadMessages; }
            private set {
                if (hasUnreadMessages != value)
                {
                    hasUnreadMessages = value;
                    NotifyPropertyChanged(() => HasUnreadMessages);
                }
            }
        }

        /// <summary>
        ///   Gets chat state notification description
        /// </summary>
        public XmppChatStateNotification ChatStateNotification {
            get { return chatStateNotification; }
            set {
                if (chatStateNotification != value)
                {
                    chatStateNotification = value;
                    NotifyPropertyChanged(() => ChatStateNotification);
                }
            }
        }

        /// <summary>
        ///   Sends the given message
        /// </summary>
        /// <param name = "message">The message</param>
        public void SendMessage(string message) {
            if (!String.IsNullOrEmpty(message))
            {
                message = message.Replace(Environment.NewLine, "");

                Task.Factory.StartNew
                    (
                        () => { chat.SendMessage(message); }
                    );

                ComposeOutgoingMessage(message);
            }
        }

        /// <summary>
        ///   Sends a chat state notification
        /// </summary>
        /// <param name = "notificationType">Chat state notification type</param>
        public void SendChatStateNotification(XmppChatStateNotification notificationType) {
            Task.Factory.StartNew
                (
                    () => { chat.SendChatStateNotification(notificationType); }
                );
        }

        /// <summary>
        ///   Closes the view
        /// </summary>
        /// <param name = "obj"></param>
        private void OnClose() {
            if (chat != null)
            {
                chat.Close();
                ServiceFactory.Current.Resolve<IChatViewManager>().CloseChatView(chat.Contact.ContactId);
            }
        }

        private void ShowMessages() {
            if (chat != null)
            {
                InvokeAsynchronously
                    (
                        () =>
                            {
                                while (chat.PendingMessages.Count > 0)
                                {
                                    XmppMessage currentMessage = chat.PendingMessages.Dequeue();

                                    if (!String.IsNullOrEmpty(currentMessage.Body))
                                    {
                                        ComposeIncomingMessage(currentMessage.Body);
                                    }

                                    ChatStateNotification = currentMessage.ChatStateNotification;
                                }

#warning TODO: Review this
                                //if (!this.IsActive)
                                //{
                                //    this.HasUnreadMessages = true;
                                //}
                            }
                    );
            }
        }

        private void ComposeOutgoingMessage(string message) {
            var headerParagraph = new Paragraph();
            var messageParagraph = new Paragraph();

            // Paragraph settings
            headerParagraph.FontFamily = new FontFamily("Segoe WP Light");
            headerParagraph.FontSize = 21.333;
            headerParagraph.Foreground = new SolidColorBrush(Color.FromArgb(255, 27, 161, 226));

            headerParagraph.Inlines.Add(new Bold(new Run(DateTime.Now.ToShortTimeString())));
            headerParagraph.Inlines.Add(new Bold(new Run(" ")));
            headerParagraph.Inlines.Add(
                new Bold(new Run(ServiceFactory.Current.Resolve<IConfigurationManager>().SelectedAccount.DisplayName)));

            messageParagraph.FontFamily = new FontFamily("Segoe WP");
            messageParagraph.FontSize = 14;
            messageParagraph.Inlines.Add(new Run(message));

            conversation.Blocks.Add(headerParagraph);
            conversation.Blocks.Add(messageParagraph);

            Action<Paragraph> x = r => r.BringIntoView();
            Dispatcher.BeginInvoke(x, DispatcherPriority.SystemIdle, messageParagraph);

            NotifyPropertyChanged(() => Conversation);
        }

        private void ComposeIncomingMessage(string message) {
            var headerParagraph = new Paragraph();
            var messageParagraph = new Paragraph();

            // Paragraph settings
            headerParagraph.FontFamily = new FontFamily("Segoe WP Light");
            headerParagraph.FontSize = 21.333;
            headerParagraph.Foreground = new SolidColorBrush(Color.FromArgb(255, 51, 153, 51));

            headerParagraph.Inlines.Add(new Bold(new Run(DateTime.Now.ToShortTimeString())));
            headerParagraph.Inlines.Add(new Bold(new Run(" ")));
            headerParagraph.Inlines.Add(new Bold(new Run(chat.Contact.DisplayName)));

            messageParagraph.FontFamily = new FontFamily("Segoe WP");
            messageParagraph.FontSize = 14;
            messageParagraph.Inlines.Add(new Run(message));

            conversation.Blocks.Add(headerParagraph);
            conversation.Blocks.Add(messageParagraph);

            Action<Paragraph> x = r => r.BringIntoView();
            Dispatcher.BeginInvoke(x, DispatcherPriority.SystemIdle, messageParagraph);

            NotifyPropertyChanged(() => Conversation);
        }

        private void chat_ReceivedMessage(object sender, EventArgs e) {
            ShowMessages();
        }

        private void chat_ChatClosed(object sender, EventArgs e) {
            // Unbind event handlers
            chat.ChatClosed -= chat_ChatClosed;
            chat.ReceivedMessage -= chat_ReceivedMessage;
            chat = null;
        }
        }
}