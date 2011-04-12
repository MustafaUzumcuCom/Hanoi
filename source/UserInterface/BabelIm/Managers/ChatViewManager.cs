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

namespace BabelIm
{
    /// <summary>
    /// Chat View manager
    /// </summary>
    public sealed class ChatViewManager 
        : DispatcherObject, IChatViewManager
    {
        #region · Attached Properties ·

        /// <summary>
        /// Identifies the IsDesktopCanvas dependency property.
        /// </summary>
        public static readonly DependencyProperty IsChatContainerProperty =
            DependencyProperty.RegisterAttached("IsChatContainer", typeof(bool), typeof(ChatViewManager),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnIsChatContainer)));

        #endregion

        #region · Attached Property Get/Set Methods ·

        /// <summary>
        /// Gets the value of the IsDesktopCanvas attached property
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool GetIsChatContainer(DependencyObject d)
        {
            return (bool)d.GetValue(IsChatContainerProperty);
        }

        /// <summary>
        /// Sets the value of the IsDesktopCanvas attached property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        public static void SetIsChatContainer(DependencyObject d, bool value)
        {
            d.SetValue(IsChatContainerProperty, value);
        }

        #endregion

        #region · Attached Properties Callbacks ·

        private static void OnIsChatContainer(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                IChatViewManager chatViewManager = ServiceFactory.Current.Resolve<IChatViewManager>();

                if (chatViewManager != null)
                {
                    chatViewManager.RegisterContainer(d);
                }
            }
        }

        #endregion

        #region · Sync Object ·

        object SyncObject = new object();

        #endregion

        #region · Fields ·

        private IDictionary<string, PivotItem>  chatViews;
        private PivotControl                    container;

        #region · Subscriptions ·

        private IDisposable chatMessagesSubscription;

        #endregion

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatViewManager"/> class
        /// </summary>
        public ChatViewManager()
        {
            this.chatViews = new Dictionary<string, PivotItem>();

            this.Subscribe();
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Opens a new chat view for the given contact jid 
        /// </summary>
        /// <param name="jid">The contact jid</param>
        public void OpenChatView(XmppJid jid)
        {
            this.OpenChatView(jid.BareIdentifier);
        }

        /// <summary>
        /// Opens a new chat view for the given contact jid 
        /// </summary>
        /// <param name="jid">The contact jid</param>
        public void OpenChatView(string jid)
        {
            lock (SyncObject)
            {
                if (!this.chatViews.ContainsKey(jid))
                {
                    OpenChatView(ServiceFactory.Current.Resolve<IXmppSession>().CreateChat(jid), false);
                }
                else
                {
                    if (this.chatViews.ContainsKey(jid))
                    {
                        OpenChatView(ServiceFactory.Current.Resolve<IXmppSession>().CreateChat(jid), true);
                    }
                }
            }
        }

        /// <summary>
        /// Opens a new chat view for the given chat instance
        /// </summary>
        /// <param name="chat">A <see cref="XmppChat"/> instance</param>
        public void OpenChatView(XmppChat chat)
        {
            this.OpenChatView(chat, false);
        }

        /// <summary>
        /// Opens a new chat view for the given chat instance
        /// </summary>
        /// <param name="chat">A <see cref="XmppChat"/> instance</param>
        public void OpenChatView(XmppChat chat, bool activate)
        {
            this.Dispatcher.BeginInvoke
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
                                ChatViewModel viewModel = new ChatViewModel(chat);
                                ChatView view = new ChatView();

                                view.DataContext = viewModel;

                                PivotItem chatViewItem = new PivotItem();

                                chatViewItem.HorizontalAlignment    = HorizontalAlignment.Stretch;
                                chatViewItem.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                                chatViewItem.Header                 = chat.Contact.DisplayName;
                                chatViewItem.Content                = view;

                                this.chatViews.Add(chat.Contact.ContactId.BareIdentifier, chatViewItem);

                                this.container.Items.Add(chatViewItem);
                                this.container.Invalidate();
                                this.container.SelectedItem = chatViewItem;
                            }
                            else
                            {
                                this.container.SelectedItem = this.chatViews[chat.Contact.ContactId.BareIdentifier];
                            }
                        }
                    }
                )
            );
        }

        /// <summary>
        /// Closes the chat view for a given <see cref="XmppChat"/> instance
        /// </summary>
        /// <param name="chat">A <see cref="XmppChat"/> instance</param>
        public void CloseChatView(string jid)
        {
            lock (SyncObject)
            {
                if (this.chatViews.ContainsKey(jid))
                {
                    PivotItem chatViewItem = this.chatViews[jid];
#warning Remove the view from the container
                    this.chatViews.Remove(jid);
                }
            }
        }

        public void RegisterContainer(DependencyObject d)
        {
            this.container = d as PivotControl;
        }

        #endregion

        #region · Private Methods ·
        
        private void Subscribe()
        {
            ServiceFactory.Current.Resolve<IXmppSession>()
                .StateChanged
                .Where(state => state == XmppSessionState.LoggingOut)
                .Subscribe
            (
                newState =>
                {
                    this.OnSessionStateChanged(newState);
                }
            );
            
            this.chatMessagesSubscription = ServiceFactory.Current.Resolve<IXmppSession>()
                .MessageReceived
                .Subscribe
            (
                message => 
                {
                    this.OnChatMessageReceived(message);
                }
            );
        }

        private void Unsubscribe()
        {
            if (chatMessagesSubscription != null)
            {
                this.chatMessagesSubscription.Dispose();
                this.chatMessagesSubscription = null;
            }
        }

        #endregion

        #region · Event Handlers ·

        private void OnSessionStateChanged(XmppSessionState newState)
        {
            lock (SyncObject)
            {
                List<string>                                    removedViews    = new List<string>();
                IEnumerator<KeyValuePair<string, PivotItem>>    enumerator      = this.chatViews.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    removedViews.Add(enumerator.Current.Key);
                }

                foreach (string jid in removedViews)
                {
                    this.CloseChatView(jid);
                }
            }

            this.Unsubscribe();
        }

        private void OnChatMessageReceived(XmppMessage message)
        {
            this.OpenChatView(message.From);
        }

        #endregion
    }
}