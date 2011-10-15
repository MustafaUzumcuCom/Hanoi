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

using Hanoi.Serialization.Extensions.PubSub;

namespace Hanoi
{
    /// <summary>
    ///   Pub sub event message
    /// </summary>
    public sealed class EventMessage
    {
        internal EventMessage(Serialization.InstantMessaging.Client.Message message)
        {
            Identifier = message.ID;
            From = message.From;
            To = message.To;
            Event = (PubSubEvent)message.Items[0];
        }

        /// <summary>
        ///   Gets the XMPP Event Message ID
        /// </summary>
        public string Identifier { get; private set; }

        /// <summary>
        ///   Gets the Event Message source JID
        /// </summary>
        public Jid From { get; private set; }

        /// <summary>
        ///   Gets the Event Message target JID
        /// </summary>
        public Jid To { get; private set; }

        /// <summary>
        ///   Gets the XMPP Event Message data
        /// </summary>
        public PubSubEvent Event { get; private set; }
    }
}