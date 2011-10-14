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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Hanoi.Core;
using Hanoi.Serialization.InstantMessaging.Client;
using Hanoi.Xmpp;

namespace BabelIm.Net.Xmpp.InstantMessaging.PersonalEventing {
    /// <summary>
    ///   XMPP Activity
    /// </summary>
    public sealed class XmppActivity
        : ObservableObject, IEnumerable<XmppEvent>, INotifyCollectionChanged {
        private readonly ObservableCollection<XmppEvent> activities;
        private readonly XmppSession session;

        private IDisposable eventMessageSubscription;
        private IDisposable messageSubscription;
        private IDisposable sessionStateSubscription;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppSession" /> class
        /// </summary>
        internal XmppActivity(XmppSession session) {
            this.session = session;
            activities = new ObservableCollection<XmppEvent>();

            SubscribeToSessionState();
        }

        #region IEnumerable<XmppEvent> Members

        IEnumerator<XmppEvent> IEnumerable<XmppEvent>.GetEnumerator() {
            return activities.GetEnumerator();
        }

        public IEnumerator GetEnumerator() {
            return activities.GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        /// <summary>
        ///   Clears the activity list
        /// </summary>
        public void Clear() {
            activities.Clear();

            InvokeAsynchronously
                (
                    () =>
                        {
                            if (CollectionChanged != null)
                            {
                                CollectionChanged(this,
                                                  new NotifyCollectionChangedEventArgs(
                                                      NotifyCollectionChangedAction.Reset));
                            }
                        }
                );
        }

        private void SubscribeToSessionState() {
            sessionStateSubscription = session
                .StateChanged
                .Where(s => s == XmppSessionState.LoggingIn || s == XmppSessionState.LoggingOut)
                .Subscribe
                (
                    newState =>
                        {
                            if (newState == XmppSessionState.LoggingOut)
                            {
                                Subscribe();
                            }
                            else if (newState == XmppSessionState.LoggingOut)
                            {
                                Clear();
                                Unsubscribe();
                            }
                        }
                );
        }

        private void Subscribe() {
            messageSubscription = session
                .MessageReceived
                .Where(m => m.Type == MessageType.Headline || m.Type == MessageType.Normal)
                .Subscribe
                (
                    message => { OnActivityMessage(message); }
                );

            eventMessageSubscription = session.Connection
                .OnEventMessage
                .Subscribe(message => OnEventMessage(message));

            activities.CollectionChanged += OnCollectionChanged;
        }

        private void Unsubscribe() {
            if (messageSubscription != null)
            {
                messageSubscription.Dispose();
                messageSubscription = null;
            }

            if (eventMessageSubscription != null)
            {
                eventMessageSubscription.Dispose();
                eventMessageSubscription = null;
            }

            activities.CollectionChanged -= OnCollectionChanged;
        }

        private void OnActivityMessage(XmppMessage message) {
            switch (message.Type)
            {
                case MessageType.Normal:
                    activities.Add(new XmppMessageEvent(message));
                    break;

                case MessageType.Headline:
                    activities.Add(new XmppMessageEvent(message));
                    break;
            }
        }

        private void OnEventMessage(XmppEventMessage message) {
            // Add the activity based on the source event
            if (XmppEvent.IsActivityEvent(message.Event))
            {
                XmppEvent xmppevent = XmppEvent.Create(session.Roster[message.From.BareIdentifier], message.Event);

#warning TODO: see what to do when it's null
                if (xmppevent != null)
                {
#warning TODO: This needs to be changed
                    if (xmppevent is XmppUserTuneEvent && ((XmppUserTuneEvent) xmppevent).IsEmpty)
                    {
                        // And empty tune means no info available or that the user
                        // cancelled the tune notifications ??
                    }
                    else
                    {
                        activities.Add(xmppevent);
                    }
                }
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            InvokeAsynchronouslyInBackground
                (
                    () =>
                        {
                            if (CollectionChanged != null)
                            {
                                CollectionChanged(this, e);
                            }
                        }
                );
        }
        }
}