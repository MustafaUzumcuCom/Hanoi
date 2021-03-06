/*
    Copyright (c) 2007 - 2010, Carlos Guzm�n �lvarez

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

using Hanoi.Serialization.Extensions.PubSub;
using Hanoi.Serialization.Extensions.UserMood;
using Hanoi.Serialization.Extensions.UserTune;

namespace Hanoi.Xmpp.InstantMessaging.PersonalEventing
{
    public abstract class Event
    {
        public static bool IsActivityEvent(PubSubEvent xmppevent)
        {
            if (xmppevent.Item is PubSubEventItems)
            {
                var items = (PubSubEventItems)xmppevent.Item;

                if (items.Items.Count == 1)
                {
                    var item = (PubSubItem)items.Items[0];

                    return (item.Item is Tune || item.Item is Mood);
                }
            }

            return false;
        }

        public static Event Create(Contact user, PubSubEvent xmppevent)
        {
            if (xmppevent.Item is PubSubEventItems)
            {
                var items = (PubSubEventItems)xmppevent.Item;

                if (items.Items.Count == 1)
                {
                    var item = (PubSubItem)items.Items[0];

                    if (item.Item is Tune)
                    {
                        return new UserTuneEvent(user, (Tune)item.Item);
                    }
                    if (item.Item is Mood)
                    {
                        return new UserMoodEvent(user, (Mood)item.Item);
                    }
                }
            }

            return null;
        }
    }
}