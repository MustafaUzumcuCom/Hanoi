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

using Hanoi.Serialization.InstantMessaging.Presence;

namespace Hanoi.Xmpp.InstantMessaging
{
    /// <summary>
    ///   Presence handling
    /// </summary>
    public sealed class Presence
    {
        private readonly Session _session;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Presence" /> class using
        ///   the given session.
        /// </summary>
        /// <param name = "session"></param>
        internal Presence(Session session)
        {
            _session = session;
        }

        /// <summary>
        ///   Gets the presence of the given user.
        /// </summary>
        /// <param name = "targetJid">User JID</param>
        public void GetPresence(Jid targetJid)
        {
            var presence = new Serialization.InstantMessaging.Presence.Presence
                               {
                                   Id = IdentifierGenerator.Generate(),
                                   Type = PresenceType.Probe,
                                   From = _session.UserId,
                                   To = targetJid
                               };

            _session.Send(presence);
        }

        /// <summary>
        ///   Sets the initial presence status.
        /// </summary>
        public void SetInitialPresence()
        {
            SetInitialPresence(null);
        }

        /// <summary>
        ///   Sets the initial presence against the given user.
        /// </summary>
        /// <param name = "target">JID of the target user.</param>
        public void SetInitialPresence(Jid target)
        {
            var presence = new Serialization.InstantMessaging.Presence.Presence();

            if (target != null && target.ToString().Length > 0)
            {
                presence.To = target.ToString();
            }

            _session.Send(presence);
        }

        /// <summary>
        ///   Set the presence as <see cref = "PresenceState.Available" />
        /// </summary>
        public void SetPresence()
        {
            SetPresence(PresenceState.Available);
        }

        /// <summary>
        ///   Sets the presense state.
        /// </summary>
        /// <param name = "presenceState"></param>
        public void SetPresence(PresenceState presenceState)
        {
            SetPresence(presenceState, null);
        }

        /// <summary>
        ///   Sets the presence state with the given state and status message
        /// </summary>
        /// <param name = "showAs"></param>
        /// <param name = "statusMessage"></param>
        public void SetPresence(PresenceState showAs, string statusMessage)
        {
            SetPresence(showAs, statusMessage, 0);
        }

        /// <summary>
        ///   Sets the presence state with the given state, status message and priority
        /// </summary>
        /// <param name = "showAs"></param>
        /// <param name = "statusMessage"></param>
        /// <param name = "priority"></param>
        public void SetPresence(PresenceState showAs, string statusMessage, int priority)
        {
            var presence = new Serialization.InstantMessaging.Presence.Presence();
            var status = new Status();

            status.Value = statusMessage;
            presence.Id = IdentifierGenerator.Generate();

            presence.Items.Add((ShowType)showAs);
            presence.Items.Add(status);

            _session.Send(presence);
        }

        /// <summary>
        ///   Sets the presence as Unavailable
        /// </summary>
        public void SetUnavailable()
        {
            var presence = new Serialization.InstantMessaging.Presence.Presence
                               {
                                   Type = PresenceType.Unavailable
                               };

            _session.Send(presence);
        }

        /// <summary>
        ///   Request subscription to the given user
        /// </summary>
        /// <param name = "contactId"></param>
        public void RequestSubscription(Jid jid)
        {
            var presence = new Serialization.InstantMessaging.Presence.Presence
                               {
                                   Type = PresenceType.Subscribe,
                                   To = jid
                               };

            _session.Send(presence);
        }

        /// <summary>
        ///   Subscribes to presence updates of the current user
        /// </summary>
        /// <param name = "jid"></param>
        public void Subscribed(Jid jid)
        {
            var presence = new Serialization.InstantMessaging.Presence.Presence
                               {
                                   Type = PresenceType.Subscribed,
                                   To = jid
                               };

            _session.Send(presence);
        }

        /// <summary>
        ///   Subscribes to presence updates of the current user
        /// </summary>
        /// <param name = "jid"></param>
        public void Unsuscribed(Jid jid)
        {
            var presence = new Serialization.InstantMessaging.Presence.Presence
                               {
                                   Type = PresenceType.Unsubscribed,
                                   To = jid
                               };

            _session.Send(presence);
        }
    }
}