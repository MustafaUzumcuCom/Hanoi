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

using ShowType = BabelIm.Net.Xmpp.Serialization.InstantMessaging.Presence.ShowType;
using PresenceType = BabelIm.Net.Xmpp.Serialization.InstantMessaging.Presence.PresenceType;
using Presence = BabelIm.Net.Xmpp.Serialization.InstantMessaging.Presence.Presence;
using Status = BabelIm.Net.Xmpp.Serialization.InstantMessaging.Presence.Status;

namespace BabelIm.Net.Xmpp.InstantMessaging {
    /// <summary>
    ///   XMPP Contact presence information
    /// </summary>
    public sealed class XmppContactPresence
        : ObservableObject {
        private readonly XmppSession session;
        private XmppPresenceState presenceStatus;
        private int priority;
        private string statusMessage;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "">XmppContactPresence</see>
        /// </summary>
        /// <param name = "session"></param>
        internal XmppContactPresence(XmppSession session) {
            this.session = session;
            presenceStatus = XmppPresenceState.Offline;
        }

        /// <summary>
        ///   Gets or sets the presence priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority {
            get { return priority; }
            set {
                if (priority != value)
                {
                    priority = value;
                    NotifyPropertyChanged(() => Priority);
                }
            }
        }

        /// <summary>
        ///   Gets or sets the presence status.
        /// </summary>
        /// <value>The presence status.</value>
        public XmppPresenceState PresenceStatus {
            get { return presenceStatus; }
            set {
                if (presenceStatus != value)
                {
                    presenceStatus = value;
                    NotifyPropertyChanged(() => PresenceStatus);
                    session.Roster.OnContactPresenceChanged();
                }
            }
        }

        /// <summary>
        ///   Gets or sets the presence status message.
        /// </summary>
        /// <value>The presence status message.</value>
        public string StatusMessage {
            get { return statusMessage; }
            set {
                if (statusMessage != value)
                {
                    statusMessage = value;
                    NotifyPropertyChanged(() => StatusMessage);
                }
            }
        }

        internal void Update(Presence presence) {
            if (presence.TypeSpecified &&
                presence.Type == PresenceType.Unavailable)
            {
                PresenceStatus = XmppPresenceState.Offline;
            }
            else
            {
                PresenceStatus = XmppPresenceState.Available;
            }

            foreach (object item in presence.Items)
            {
                if (item is sbyte)
                {
                    Priority = (sbyte) item;
                }
                if (item is int)
                {
                    Priority = (int) item;
                }
                else if (item is ShowType)
                {
                    PresenceStatus = DecodeShowAs((ShowType) item);
                }
                else if (item is Status)
                {
                    StatusMessage = ((Status) item).Value;
                }
            }
        }

        private XmppPresenceState DecodeShowAs(ShowType showas) {
            switch (showas)
            {
                case ShowType.Away:
                    return XmppPresenceState.Away;

                case ShowType.Busy:
                    return XmppPresenceState.Busy;

                case ShowType.ExtendedAway:
                    return XmppPresenceState.Idle;

                case ShowType.Online:
                    return XmppPresenceState.Available;
            }

            return XmppPresenceState.Offline;
        }
        }
}