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
using System.ComponentModel;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client.Presence;

namespace BabelIm.Net.Xmpp.InstantMessaging
{
    /// <summary>
    /// XMPP Contact presence information
    /// </summary>
    public sealed class XmppContactPresence 
        : ObservableObject
    {
        #region · Fields ·

        private XmppPresenceState   presenceStatus;
        private XmppSession         session;
        private int                 priority;
        private string              statusMessage;

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets or sets the presence priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority
        {
            get { return this.priority; }
            set
            {
                if (this.priority != value)
                {
                    this.priority = value;
                    this.NotifyPropertyChanged(() => Priority);
                }
            }
        }

        /// <summary>
        /// Gets or sets the presence status.
        /// </summary>
        /// <value>The presence status.</value>
        public XmppPresenceState PresenceStatus
        {
            get { return this.presenceStatus; }
            set
            {
                if (this.presenceStatus != value)
                {
                    this.presenceStatus = value;
                    this.NotifyPropertyChanged(() => PresenceStatus);
                    this.session.Roster.OnContactPresenceChanged();
                }                
            }
        }

        /// <summary>
        /// Gets or sets the presence status message.
        /// </summary>
        /// <value>The presence status message.</value>
        public string StatusMessage
        {
            get { return this.statusMessage; }
            set
            {
                if (this.statusMessage != value)
                {
                    this.statusMessage = value;
                    this.NotifyPropertyChanged(() => StatusMessage);
                }
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="">XmppContactPresence</see>
        /// </summary>
        /// <param name="session"></param>
        internal XmppContactPresence(XmppSession session)
        {
            this.session        = session;
            this.presenceStatus = XmppPresenceState.Offline;
        }

        #endregion

        #region · Internal Methods ·

        internal void Update(Presence presence)
        {
            if (presence.TypeSpecified &&
                presence.Type == PresenceType.Unavailable)
            {
                this.PresenceStatus = XmppPresenceState.Offline;
            }
            else
            {
                this.PresenceStatus = XmppPresenceState.Available;
            }

            foreach (object item in presence.Items)
            {
                if (item is sbyte)
                {
                    this.Priority = (sbyte)item;
                }
                if (item is int)
                {
                    this.Priority = (int)item;
                }
                else if (item is ShowType)
                {
                    this.PresenceStatus = this.DecodeShowAs((ShowType)item);
                }
                else if (item is Status)
                {
                    this.StatusMessage = ((Status)item).Value;
                }
            }
        }

        #endregion

        #region · Private Methods ·

        private XmppPresenceState DecodeShowAs(ShowType showas)
        {
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

        #endregion
    }
}
