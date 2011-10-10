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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BabelIm.Net.Xmpp.Serialization.InstantMessaging.Roster {
    /// <remarks />
    [Serializable]
    [XmlType(Namespace = "jabber:iq:roster")]
    [XmlRootAttribute("item", Namespace = "jabber:iq:roster", IsNullable = false)]
    public class RosterItem {
        private RosterAskType askField;
        private bool askFieldSpecified;
        private List<String> groupsField;
        private string jidField;
        private string nameField;
        private RosterSubscriptionType subscriptionField;
        private bool subscriptionFieldSpecified;

        public RosterItem() {
            groupsField = new List<string>();
        }

        /// <remarks />
        [XmlElementAttribute("group")]
        public List<string> Groups {
            get { return groupsField; }
        }

        /// <remarks />
        [XmlAttributeAttribute("ask")]
        public RosterAskType Ask {
            get { return askField; }
            set {
                askField = value;
                askFieldSpecified = true;
            }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool AskSpecified {
            get { return askFieldSpecified; }
            set { askFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("jid")]
        public string Jid {
            get { return jidField; }
            set { jidField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("name")]
        public string Name {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("subscription")]
        public RosterSubscriptionType Subscription {
            get { return subscriptionField; }
            set {
                SubscriptionSpecified = true;
                subscriptionField = value;
            }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool SubscriptionSpecified {
            get { return subscriptionFieldSpecified; }
            set { subscriptionFieldSpecified = value; }
        }
    }
}