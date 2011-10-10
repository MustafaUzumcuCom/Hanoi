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

using BabelIm.Net.Xmpp.Core;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client.Presence;

namespace BabelIm.Net.Xmpp.InstantMessaging
{
    /// <summary>
    /// Presence handling
    /// </summary>
    public sealed class XmppPresence
    {
        #region · Fields ·

        private XmppSession session;

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="XmppPresence"/> class using
        /// the given session.
        /// </summary>
        /// <param name="session"></param>
        internal XmppPresence(XmppSession session)
        {
            this.session = session;
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Gets the presence of the given user.
        /// </summary>
        /// <param name="targetJid">User JID</param>
        public void GetPresence(XmppJid targetJid)
        {
            Presence presence = new Presence
            {
                Id      = XmppIdentifierGenerator.Generate(),
                Type    = PresenceType.Probe,
                From    = this.session.UserId,
                To      = targetJid
            };

            this.session.Send(presence);
        }

        /// <summary>
        /// Sets the initial presence status.
        /// </summary>
        public void SetInitialPresence()
        {
            this.SetInitialPresence(null);
        }

        /// <summary>
        /// Sets the initial presence against the given user.
        /// </summary>
        /// <param name="target">JID of the target user.</param>
        public void SetInitialPresence(XmppJid target)
        {
            Presence presence = new Presence();

            if (target != null && target.ToString().Length > 0)
            {
                presence.To = target.ToString();
            }

            this.session.Send(presence);
        }

        /// <summary>
        /// Set the presence as <see cref="XmppPresenceState.Available"/>
        /// </summary>
        public void SetPresence()
        {
            this.SetPresence(XmppPresenceState.Available);
        }

        /// <summary>
        /// Sets the presense state.
        /// </summary>
        /// <param name="presenceState"></param>
        public void SetPresence(XmppPresenceState presenceState)
        {
            this.SetPresence(presenceState, null);
        }

        /// <summary>
        /// Sets the presence state with the given state and status message
        /// </summary>
        /// <param name="showAs"></param>
        /// <param name="statusMessage"></param>
        public void SetPresence(XmppPresenceState showAs, string statusMessage)
        {
            this.SetPresence(showAs, statusMessage, 0);
        }

        /// <summary>
        /// Sets the presence state with the given state, status message and priority
        /// </summary>
        /// <param name="showAs"></param>
        /// <param name="statusMessage"></param>
        /// <param name="priority"></param>
        public void SetPresence(XmppPresenceState showAs, string statusMessage, int priority)
        {
            Presence    presence    = new Presence();
            Status      status      = new Status();

            status.Value    = statusMessage;
            presence.Id     = XmppIdentifierGenerator.Generate();

            presence.Items.Add((ShowType)showAs);
            presence.Items.Add(status);

            this.session.Send(presence);
        }

        /// <summary>
        /// Sets the presence as Unavailable
        /// </summary>
        public void SetUnavailable()
        {
            Presence presence = new Presence
            {
                Type = PresenceType.Unavailable
            };

            this.session.Send(presence);
        }

        /// <summary>
        /// Request subscription to the given user
        /// </summary>
        /// <param name="contactId"></param>
        public void RequestSubscription(XmppJid jid)
        {
            Presence presence = new Presence
            {
                Type   = PresenceType.Subscribe,
                To     = jid
            };
            
            this.session.Send(presence);
        }

        /// <summary>
        /// Subscribes to presence updates of the current user
        /// </summary>
        /// <param name="jid"></param>
        public void Subscribed(XmppJid jid)
        {
            Presence presence = new Presence
            {
                Type    = PresenceType.Subscribed,
                To      = jid
            };

            this.session.Send(presence);
        }

        /// <summary>
        /// Subscribes to presence updates of the current user
        /// </summary>
        /// <param name="jid"></param>
        public void Unsuscribed(XmppJid jid)
        {
            Presence presence = new Presence
            {
                Type    = PresenceType.Unsubscribed,
                To      = jid
            };

            this.session.Send(presence);
        }

        #endregion
    }
}