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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using BabelIm.Contracts;
using BabelIm.Controls.PivotControl;
using BabelIm.IoC;
using BabelIm.Net.Xmpp.Core;
using BabelIm.Net.Xmpp.InstantMessaging;
using BabelIm.ViewModels;
using BabelIm.Views;

namespace BabelIm {
    /// <summary>
    ///   Chat View manager
    /// </summary>
    public sealed class ChatViewManager
        : DispatcherObject, IChatViewManager {
        /// <summary>
        ///   Identifies the IsDesktopCanvas dependency property.
        /// </summary>
        public static readonly DependencyProperty IsChatContainerProperty =
            DependencyProperty.RegisterAttached("IsChatContainer", typeof (bool), typeof (ChatViewManager),
                                                new FrameworkPropertyMetadata(false,
                                                                              OnIsChatContainer));

        private readonly object SyncObject = new object();

        private readonly IDictionary<string, PivotItem> chatViews;

        private IDisposable chatMessagesSubscription;
        private PivotControl container;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ChatViewManager" /> class
        /// </summary>
        public ChatViewManager() {
            chatViews = new Dictionary<string, PivotItem>();

            Subscribe();
        }

        #region IChatViewManager Members

        /// <summary>
        ///   Opens a new chat view for the given contact jid
        /// </summary>
        /// <param name = "jid">The contact jid</param>
        public void OpenChatView(XmppJid jid) {
            OpenChatView(jid.BareIdentifier);
        }

        /// <summary>
        ///   Opens a new chat view for the given contact jid
        /// </summary>
        /// <param name = "jid">The contact jid</param>
        public void OpenChatView(string jid) {
            lock (SyncObject)
            {
                if (!chatViews.ContainsKey(jid))
                {
                    OpenChatView(ServiceFactory.Current.Resolve<IXmppSession>().CreateChat(jid), false);
                }
                else
                {
                    if (chatViews.ContainsKey(jid))
                    {
                        OpenChatView(ServiceFactory.Current.Resolve<IXmppSession>().CreateChat(jid), true);
                    }
                }
            }
        }

        /// <summary>
        ///   Opens a new chat view for the given chat instance
        /// </summary>
        /// <param name = "chat">A <see cref = "XmppChat" /> instance</param>
        public void OpenChatView(XmppChat chat, bool activate) {
            Dispatcher.BeginInvoke
                (
                    DispatcherPriority.Normal,
                    new ThreadStart
                        (
                        delegate
                            {
                                lock (SyncObject)
                                {
                                    if (!activate)
                                    {
                                        var viewModel = new ChatViewModel(chat);
                                        var view = new ChatView();

                                        view.DataContext = viewModel;

                                        var chatViewItem = new PivotItem();

                                        chatViewItem.HorizontalAlignment = HorizontalAlignment.Stretch;
                                        chatViewItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                                        chatViewItem.Header = chat.Contact.DisplayName;
                                        chatViewItem.Content = view;

                                        chatViews.Add(chat.Contact.ContactId.BareIdentifier, chatViewItem);

                                        container.Items.Add(chatViewItem);
                                        container.Invalidate();
                                        container.SelectedItem = chatViewItem;
                                    }
                                    else
                                    {
                                        container.SelectedItem = chatViews[chat.Contact.ContactId.BareIdentifier];
                                    }
                                }
                            }
                        )
                );
        }

        /// <summary>
        ///   Closes the chat view for a given <see cref = "XmppChat" /> instance
        /// </summary>
        /// <param name = "chat">A <see cref = "XmppChat" /> instance</param>
        public void CloseChatView(string jid) {
            lock (SyncObject)
            {
                if (chatViews.ContainsKey(jid))
                {
                    PivotItem chatViewItem = chatViews[jid];
#warning Remove the view from the container
                    chatViews.Remove(jid);
                }
            }
        }

        public void RegisterContainer(DependencyObject d) {
            container = d as PivotControl;
        }

        #endregion

        /// <summary>
        ///   Gets the value of the IsDesktopCanvas attached property
        /// </summary>
        /// <param name = "d"></param>
        /// <returns></returns>
        public static bool GetIsChatContainer(DependencyObject d) {
            return (bool) d.GetValue(IsChatContainerProperty);
        }

        /// <summary>
        ///   Sets the value of the IsDesktopCanvas attached property
        /// </summary>
        /// <param name = "d"></param>
        /// <param name = "value"></param>
        public static void SetIsChatContainer(DependencyObject d, bool value) {
            d.SetValue(IsChatContainerProperty, value);
        }

        private static void OnIsChatContainer(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d != null)
            {
                var chatViewManager = ServiceFactory.Current.Resolve<IChatViewManager>();

                if (chatViewManager != null)
                {
                    chatViewManager.RegisterContainer(d);
                }
            }
        }

        /// <summary>
        ///   Opens a new chat view for the given chat instance
        /// </summary>
        /// <param name = "chat">A <see cref = "XmppChat" /> instance</param>
        public void OpenChatView(XmppChat chat) {
            OpenChatView(chat, false);
        }

        private void Subscribe() {
            ServiceFactory.Current.Resolve<IXmppSession>()
                .StateChanged
                .Where(state => state == XmppSessionState.LoggingOut)
                .Subscribe
                (
                    newState => { OnSessionStateChanged(newState); }
                );

            chatMessagesSubscription = ServiceFactory.Current.Resolve<IXmppSession>()
                .MessageReceived
                .Subscribe
                (
                    message => { OnChatMessageReceived(message); }
                );
        }

        private void Unsubscribe() {
            if (chatMessagesSubscription != null)
            {
                chatMessagesSubscription.Dispose();
                chatMessagesSubscription = null;
            }
        }

        private void OnSessionStateChanged(XmppSessionState newState) {
            lock (SyncObject)
            {
                var removedViews = new List<string>();
                IEnumerator<KeyValuePair<string, PivotItem>> enumerator = chatViews.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    removedViews.Add(enumerator.Current.Key);
                }

                foreach (string jid in removedViews)
                {
                    CloseChatView(jid);
                }
            }

            Unsubscribe();
        }

        private void OnChatMessageReceived(XmppMessage message) {
            OpenChatView(message.From);
        }
        }
}