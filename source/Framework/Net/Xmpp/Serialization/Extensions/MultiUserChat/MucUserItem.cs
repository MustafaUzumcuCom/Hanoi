/*
    Copyright (c) 2007-2010, Carlos Guzmán Álvarez

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
using System.Xml.Serialization;

namespace BabelIm.Net.Xmpp.Serialization.Extensions.MultiUserChat {
    /// <summary>
    ///   XEP-0045: Multi-User Chat
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "http://jabber.org/protocol/muc#user")]
    [XmlRootAttribute("item", Namespace = "http://jabber.org/protocol/muc#user", IsNullable = false)]
    public class MucUserItem {
        private MucUserActor actor;
        private MucUserItemAffiliation affiliation;
        private bool affiliationSpecified;
        private string jid;
        private MucUserContinue muccontinue;
        private string nick;
        private string reason;
        private MucUserItemRole role;
        private bool roleSpecified;

        /// <remarks />
        [XmlElement("actor")]
        public MucUserActor Actor {
            get { return actor; }
            set { actor = value; }
        }

        /// <remarks />
        [XmlElement("reason")]
        public string Reason {
            get { return reason; }
            set { reason = value; }
        }

        /// <remarks />
        [XmlElement("continue")]
        public MucUserContinue Continue {
            get { return muccontinue; }
            set { muccontinue = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("affiliation")]
        public MucUserItemAffiliation Affiliation {
            get { return affiliation; }
            set {
                affiliation = value;
                affiliationSpecified = true;
            }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool AffiliationSpecified {
            get { return affiliationSpecified; }
        }

        /// <remarks />
        [XmlAttributeAttribute("jid")]
        public string Jid {
            get { return jid; }
            set { jid = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("nick")]
        public string Nick {
            get { return nick; }
            set { nick = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("role")]
        public MucUserItemRole Role {
            get { return role; }
            set {
                role = value;
                roleSpecified = true;
            }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool RoleSpecified {
            get { return roleSpecified; }
        }
    }
}