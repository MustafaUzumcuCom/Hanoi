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
using System.Xml.Serialization;

namespace Hanoi.Xmpp.Serialization.InstantMessaging.Privacy {
    /// <remarks />
    [Serializable]
    [XmlType(Namespace = "jabber:iq:privacy")]
    [XmlRootAttribute("item", Namespace = "jabber:iq:privacy", IsNullable = false)]
    public class PrivacyItem {
        private PrivacyActionType actionField;
        private Empty iqField;
        private bool iqFieldSpecified;
        private Empty messageField;
        private bool messageFieldSpecified;
        private int orderField;
        private Empty presenceinField;
        private bool presenceinFieldSpecified;
        private Empty presenceoutField;
        private bool presenceoutFieldSpecified;
        private PrivacyType typeField;
        private bool typeFieldSpecified;
        private string valueField;

        /// <remarks />
        [XmlElementAttribute("iq")]
        public Empty IQ {
            get { return iqField; }
            set { iqField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool IQSpecified {
            get { return iqFieldSpecified; }
            set { iqFieldSpecified = value; }
        }

        /// <remarks />
        [XmlElement("message")]
        public Empty Message {
            get { return messageField; }
            set { messageField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool messageSpecified {
            get { return messageFieldSpecified; }
            set { messageFieldSpecified = value; }
        }

        /// <remarks />
        [XmlElementAttribute("presence-in")]
        public Empty PresenceIn {
            get { return presenceinField; }
            set { presenceinField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool PresenceinSpecified {
            get { return presenceinFieldSpecified; }
            set { presenceinFieldSpecified = value; }
        }

        /// <remarks />
        [XmlElementAttribute("presence-out")]
        public Empty PresenceOut {
            get { return presenceoutField; }
            set { presenceoutField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool PresenceOutSpecified {
            get { return presenceoutFieldSpecified; }
            set { presenceoutFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("action")]
        public PrivacyActionType Action {
            get { return actionField; }
            set { actionField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("order")]
        public int order {
            get { return orderField; }
            set { orderField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("type")]
        public PrivacyType Type {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool TypeSpecified {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("value")]
        public string Value {
            get { return valueField; }
            set { valueField = value; }
        }
    }
}