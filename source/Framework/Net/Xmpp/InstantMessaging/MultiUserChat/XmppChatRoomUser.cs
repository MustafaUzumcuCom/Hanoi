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

using BabelIm.Net.Xmpp.Serialization.Extensions.MultiUserChat;

namespace BabelIm.Net.Xmpp.InstantMessaging.MultiUserChat {
    public sealed class XmppChatRoomUser
        : ObservableObject {
        private readonly MucUserItem userItem;

        internal XmppChatRoomUser(MucUserItem userItem) {
            this.userItem = userItem;
        }

        public MucUserActor Actor {
            get { return userItem.Actor; }
        }

        public MucUserItemAffiliation Affiliation {
            get { return userItem.Affiliation; }
        }

        public string Identifier {
            get { return userItem.Jid; }
        }

        public string Nick {
            get { return userItem.Nick; }
        }

        public MucUserItemRole Role {
            get { return userItem.Role; }
        }
        }
}