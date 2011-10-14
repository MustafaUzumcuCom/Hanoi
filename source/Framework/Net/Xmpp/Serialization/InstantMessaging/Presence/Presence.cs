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
using System.Collections;
using System.Xml.Serialization;
using Hanoi.Xmpp.Serialization.Extensions.EntityCapabilities;
using Hanoi.Xmpp.Serialization.Extensions.MultiUserChat;
using Hanoi.Xmpp.Serialization.Extensions.VCardAvatars;
using Hanoi.Xmpp.Serialization.InstantMessaging.Client;

namespace Hanoi.Xmpp.Serialization.InstantMessaging.Presence {
    /// <remarks />
    [Serializable]
    [XmlType(Namespace = "jabber:client")]
    [XmlRoot("presence", Namespace = "jabber:client", IsNullable = false)]
    public class Presence {
        private Error errorField;
        private string fromField;
        private string idField;
        private ArrayList itemsField;
        private string langField;
        private string toField;
        private PresenceType typeField;
        private bool typeFieldSpecified;

        public Presence() {
            itemsField = new ArrayList();
        }

        /// <remarks />
        [XmlElement("status", Type = typeof (Status))]
        [XmlElement("show", Type = typeof (ShowType))]
        [XmlElement("priority", Type = typeof (sbyte))]
        [XmlElement("c", Namespace = "http://jabber.org/protocol/caps", Type = typeof (EntityCapabilities))]
        [XmlElement("x", Namespace = "vcard-temp:x:update", Type = typeof (VCardAvatar))]
        [XmlElement("x", Type = typeof (Muc), Namespace = "http://jabber.org/protocol/muc")]
        [XmlElement("x", Type = typeof (MucUser), Namespace = "http://jabber.org/protocol/muc#user")]
        public ArrayList Items {
            get { return itemsField; }
        }

        /// <remarks />
        [XmlElement("error")]
        public Error Error {
            get { return errorField; }
            set { errorField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("from")]
        public string From {
            get { return fromField; }
            set { fromField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("id", DataType = "NMTOKEN")]
        public string Id {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("to")]
        public string To {
            get { return toField; }
            set { toField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("type")]
        public PresenceType Type {
            get { return typeField; }
            set {
                typeField = value;
                typeFieldSpecified = true;
            }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool TypeSpecified {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("lang", Form = System.Xml.Schema.XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Lang {
            get { return langField; }
            set { langField = value; }
        }
    }
}