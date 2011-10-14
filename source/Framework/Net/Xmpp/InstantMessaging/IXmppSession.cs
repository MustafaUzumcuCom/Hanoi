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
using BabelIm.Net.Xmpp.InstantMessaging.EntityCaps;
using BabelIm.Net.Xmpp.InstantMessaging.MultiUserChat;
using BabelIm.Net.Xmpp.InstantMessaging.PersonalEventing;
using BabelIm.Net.Xmpp.InstantMessaging.ServiceDiscovery;
using Hanoi.Core;
using Hanoi.Core.Authentication;
using Hanoi.Serialization.Extensions.UserMood;

namespace BabelIm.Net.Xmpp.InstantMessaging {
    /// <summary>
    ///   Interface for XMPP session implementations
    /// </summary>
    public interface IXmppSession {
        IObservable<XmppMessage> MessageReceived { get; }

        IObservable<XmppSessionState> StateChanged { get; }

        XmppPersonalEventing PersonalEventing { get; }

        XmppPresence Presence { get; }

        XmppRoster Roster { get; }

        XmppServiceDiscovery ServiceDiscovery { get; }

        XmppSessionState State { get; }

        XmppJid UserId { get; }

        XmppActivity Activity { get; }

        AvatarStorage AvatarStorage { get; }

        XmppSessionEntityCapabilities Capabilities { get; }
        event EventHandler<XmppAuthenticationFailiureEventArgs> AuthenticationFailed;

        IXmppSession Open(string connectionString);

        IXmppSession PublishAvatar(string mimetype, string hash, System.Drawing.Image avatarImage);

        IXmppSession PublishDisplayName(string displayName);

        IXmppSession PublishMood(XmppUserMoodEvent moodEvent);

        IXmppSession PublishMood(MoodType mood, string description);

        IXmppSession PublishTune(XmppUserTuneEvent tuneEvent);

        IXmppSession SetPresence(XmppPresenceState newPresence);

        IXmppSession SetPresence(XmppPresenceState newPresence, string status);

        IXmppSession SetPresence(XmppPresenceState newPresence, string status, int priority);

        IXmppSession SetUnavailable();

        IXmppSession StopTunePublication();

        IXmppSession Close();

        XmppChat CreateChat(XmppJid contactId);

        XmppChat CreateChat(string contactId);

        XmppChatRoom EnterChatRoom();

        XmppChatRoom EnterChatRoom(string chatRoomName);

        bool HasOpenChat(XmppJid contactId);

        bool HasOpenChat(string contactId);
    }
}