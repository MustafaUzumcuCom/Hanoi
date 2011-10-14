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
using System.Windows.Threading;
using Hanoi.Serialization.InstantMessaging.Client;

namespace Hanoi.Xmpp.InstantMessaging.PersonalEventing
{
    /// <summary>
    ///   XMPP Activity
    /// </summary>
    public sealed class Activity : IEnumerable<Event>, INotifyCollectionChanged
    {
        private readonly ObservableCollection<Event> activities;
        private readonly Session session;

        private IDisposable eventMessageSubscription;
        private IDisposable messageSubscription;
        private IDisposable sessionStateSubscription;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Session" /> class
        /// </summary>
        internal Activity(Session session)
        {
            this.session = session;
            activities = new ObservableCollection<Event>();

            SubscribeToSessionState();
        }

        #region IEnumerable<Event> Members

        IEnumerator<Event> IEnumerable<Event>.GetEnumerator()
        {
            return activities.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return activities.GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        /// <summary>
        ///   Clears the activity list
        /// </summary>
        public void Clear()
        {
            activities.Clear();

            // TODO: necessary?
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                                                                  {
                                                                      if (CollectionChanged != null)
                                                                      {
                                                                          CollectionChanged(this,
                                                                                            new NotifyCollectionChangedEventArgs
                                                                                                (NotifyCollectionChangedAction.Reset));
                                                                      }
                                                                  }));

        }

        private void SubscribeToSessionState()
        {
            sessionStateSubscription = session
                .StateChanged
                .Where(s => s == SessionState.LoggingIn || s == SessionState.LoggingOut)
                .Subscribe
                (
                    newState =>
                    {
                        if (newState == SessionState.LoggingOut)
                        {
                            Subscribe();
                        }
                        else if (newState == SessionState.LoggingOut)
                        {
                            Clear();
                            Unsubscribe();
                        }
                    }
                );
        }

        private void Subscribe()
        {
            messageSubscription = session
                .MessageReceived
                .Where(m => m.Type == MessageType.Headline || m.Type == MessageType.Normal)
                .Subscribe(OnActivityMessage);

            eventMessageSubscription = session.Connection
                .OnEventMessage
                .Subscribe(OnEventMessage);

            activities.CollectionChanged += OnCollectionChanged;
        }

        private void Unsubscribe()
        {
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

        private void OnActivityMessage(Message message)
        {
            switch (message.Type)
            {
                case MessageType.Normal:
                    activities.Add(new MessageEvent(message));
                    break;

                case MessageType.Headline:
                    activities.Add(new MessageEvent(message));
                    break;
            }
        }

        private void OnEventMessage(EventMessage message)
        {
            // Add the activity based on the source event
            if (Event.IsActivityEvent(message.Event))
            {
                Event xmppevent = Event.Create(session.Roster[message.From.BareIdentifier], message.Event);

#warning TODO: see what to do when it's null
                if (xmppevent != null)
                {
#warning TODO: This needs to be changed
                    if (xmppevent is UserTuneEvent && ((UserTuneEvent)xmppevent).IsEmpty)
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

        // TODO: necessary?
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                        {
                            if (CollectionChanged != null)
                            {
                                CollectionChanged(this, e);
                            }
                        }
                ));
        }
    }
}