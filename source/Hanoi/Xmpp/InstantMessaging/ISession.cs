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
    EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED2 TO,
    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using Hanoi.Authentication;
using Hanoi.Serialization.Extensions.UserMood;
using Hanoi.Xmpp.InstantMessaging.EntityCaps;
using Hanoi.Xmpp.InstantMessaging.MultiUserChat;
using Hanoi.Xmpp.InstantMessaging.PersonalEventing;

namespace Hanoi.Xmpp.InstantMessaging
{
    /// <summary>
    ///   Interface for XMPP session implementations
    /// </summary>
    public interface ISession
    {
        IObservable<Message> MessageReceived { get; }

        IObservable<SessionState> StateChanged { get; }

        PersonalEventing.PersonalEventing PersonalEventing { get; }

        Presence Presence { get; }

        Roster Roster { get; }

        ServiceDiscovery.ServiceDiscovery ServiceDiscovery { get; }

        SessionState State { get; }

        Jid UserId { get; }

        Activity Activity { get; }

        AvatarStorage AvatarStorage { get; }

        SessionEntityCapabilities Capabilities { get; }

        event EventHandler<AuthenticationFailiureEventArgs> AuthenticationFailed;

        ISession Open(ConnectionStringBuilder connectionString);

        ISession PublishAvatar(string mimetype, string hash, System.Drawing.Image avatarImage);

        ISession PublishDisplayName(string displayName);

        ISession PublishMood(UserMoodEvent moodEvent);

        ISession PublishMood(MoodType mood, string description);

        ISession PublishTune(UserTuneEvent tuneEvent);

        ISession SetPresence(PresenceState newPresence);

        ISession SetPresence(PresenceState newPresence, string status);

        ISession SetPresence(PresenceState newPresence, string status, int priority);

        ISession SetUnavailable();

        ISession StopTunePublication();

        ISession Close();

        Chat CreateChat(Jid contactId);

        Chat CreateChat(string contactId);

        ChatRoom EnterChatRoom();

        ChatRoom EnterChatRoom(string chatRoomName);

        bool HasOpenChat(Jid contactId);

        bool HasOpenChat(string contactId);
    }
}