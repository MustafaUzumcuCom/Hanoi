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
    public sealed class Presence
    {
        private readonly Session _session;
        internal Presence(Session session)
        {
            _session = session;
        }

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

        public void SetInitialPresence()
        {
            SetInitialPresence(null);
        }

        public void SetInitialPresence(Jid target)
        {
            var presence = new Serialization.InstantMessaging.Presence.Presence();

            if (target != null && target.ToString().Length > 0)
            {
                presence.To = target.ToString();
            }

            _session.Send(presence);
        }

        public void SetPresence()
        {
            SetPresence(PresenceState.Available);
        }

        public void SetPresence(PresenceState presenceState)
        {
            SetPresence(presenceState, null);
        }

        public void SetPresence(PresenceState showAs, string statusMessage)
        {
            SetPresence(showAs, statusMessage, 0);
        }

        public void SetPresence(PresenceState showAs, string statusMessage, int priority)
        {
            var presence = new Serialization.InstantMessaging.Presence.Presence();
            var status = new Status
                             {
                                 Value = statusMessage
                             };

            presence.Id = IdentifierGenerator.Generate();
            presence.Items.Add((ShowType)showAs);
            presence.Items.Add(status);
            _session.Send(presence);
        }

        public void SetUnavailable()
        {
            var presence = new Serialization.InstantMessaging.Presence.Presence
                               {
                                   Type = PresenceType.Unavailable
                               };

            _session.Send(presence);
        }

        public void RequestSubscription(Jid jid)
        {
            var presence = new Serialization.InstantMessaging.Presence.Presence
                               {
                                   Type = PresenceType.Subscribe,
                                   To = jid
                               };

            _session.Send(presence);
        }

        public void Subscribed(Jid jid)
        {
            var presence = new Serialization.InstantMessaging.Presence.Presence
                               {
                                   Type = PresenceType.Subscribed,
                                   To = jid
                               };

            _session.Send(presence);
        }

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