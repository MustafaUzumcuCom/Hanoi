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
using Hanoi.Serialization.Core.ResourceBinding;
using Hanoi.Serialization.Core.Streams;
using Hanoi.Serialization.Extensions.PubSub;
using Hanoi.Serialization.Extensions.ServiceDiscovery;
using Hanoi.Serialization.Extensions.SimpleCommunicationsBlocking;
using Hanoi.Serialization.Extensions.VCardTemp;
using Hanoi.Serialization.Extensions.XmppPing;
using Hanoi.Serialization.InstantMessaging.Register;
using Hanoi.Serialization.InstantMessaging.Roster;

namespace Hanoi.Serialization.InstantMessaging.Client {
    /// <remarks />
    [Serializable]
    [XmlType(Namespace = "jabber:client")]
    [XmlRoot("iq", Namespace = "jabber:client", IsNullable = false)]
    public class IQ {
        private Error errorField;
        private string fromField;
        private string idField;
        private ArrayList items;
        private string langField;
        private string toField;
        private IQType typeField;

        public IQ() {
            items = new ArrayList();
        }

        /// <remarks />
        [XmlElement("bind", Type = typeof (Bind), Namespace = "urn:ietf:params:xml:ns:xmpp-bind")]
        [XmlElement("session", Type = typeof (Session), Namespace = "urn:ietf:params:xml:ns:xmpp-session")]
        [XmlElement("ping", Type = typeof (Ping), Namespace = "urn:xmpp:ping")]
        [XmlElement("query", Type = typeof (Browse), Namespace = "jabber:iq:browse")]
        [XmlElement("query", Type = typeof (RegisterQuery), Namespace = "jabber:iq:register")]
        [XmlElement("query", Type = typeof (RosterQuery), Namespace = "jabber:iq:roster")]
        [XmlElement("pubsub", Type = typeof (PubSub), Namespace = "http://jabber.org/protocol/pubsub")]
        [XmlElement("query", Type = typeof (ServiceQuery), Namespace = "http://jabber.org/protocol/disco#info")]
        [XmlElement("query", Type = typeof (ServiceItemQuery), Namespace = "http://jabber.org/protocol/disco#items")]
        [XmlElement("vCard", Type = typeof (VCardData), Namespace = "vcard-temp")]
        [XmlElement("blocklist", Type = typeof (BlockList), Namespace = "urn:xmpp:blocking")]
        [XmlElement("block", Type = typeof (Block), Namespace = "urn:xmpp:blocking")]
        [XmlElement("unblock", Type = typeof (UnBlock), Namespace = "urn:xmpp:blocking")]
        public ArrayList Items {
            get { return items; }
        }

        /// <remarks />
        [XmlElementAttribute("error")]
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
        public string ID {
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
        public IQType Type {
            get { return typeField; }
            set { typeField = value; }
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