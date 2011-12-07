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
    public sealed class ContactPresence
    {
        private readonly Session _session;
        private readonly Contact _contact;
        private PresenceState _presenceStatus;
        private int _priority;
        private string _statusMessage;

        internal ContactPresence(Session session, Contact contact)
        {
            _session = session;
            _contact = contact;
            _presenceStatus = PresenceState.Offline;
        }

        public int Priority
        {
            get { return _priority; }
            set
            {
                if (_priority != value)
                    return;

                _priority = value;
            }
        }

        public PresenceState PresenceStatus
        {
            get { return _presenceStatus; }
            set
            {
                if (_presenceStatus == value)
                    return;

                _presenceStatus = value;
                _session.Roster.ContactPresence(_contact);
            }
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (_statusMessage == value) 
                    return;

                _statusMessage = value;
                _session.Roster.ContactPresence(_contact);
            }
        }

        internal void Update(Serialization.InstantMessaging.Presence.Presence presence)
        {
            if (presence.TypeSpecified && presence.Type == PresenceType.Unavailable)
            {
                PresenceStatus = PresenceState.Offline;
            }
            else
            {
                PresenceStatus = PresenceState.Available;
            }

            foreach (var item in presence.Items)
            {
                if (item is sbyte)
                {
                    Priority = (sbyte)item;
                }
                if (item is int)
                {
                    Priority = (int)item;
                }
                else if (item is ShowType)
                {
                    PresenceStatus = DecodeShowAs((ShowType)item);
                }
                else if (item is Status)
                {
                    StatusMessage = ((Status)item).Value;
                }
            }
        }

        private static PresenceState DecodeShowAs(ShowType showas)
        {
            switch (showas)
            {
                case ShowType.Away:
                    return PresenceState.Away;

                case ShowType.Busy:
                    return PresenceState.Busy;

                case ShowType.ExtendedAway:
                    return PresenceState.Idle;

                case ShowType.Online:
                    return PresenceState.Available;
            }

            return PresenceState.Offline;
        }
    }
}