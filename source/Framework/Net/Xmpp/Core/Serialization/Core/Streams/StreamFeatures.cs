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
using Hanoi.Serialization.Core.Sasl;
using Hanoi.Serialization.InstantMessaging.Register;
using Hanoi.Xmpp.Serialization.Core.ResourceBinding;
using Hanoi.Xmpp.Serialization.Core.Streams;
using Hanoi.Xmpp.Serialization.Core.Tls;
using Hanoi.Xmpp.Serialization.Extensions.EntityCapabilities;
using Hanoi.Xmpp.Serialization.Extensions.RosterVersioning;

namespace Hanoi.Serialization.Core.Streams {
    /// <summary>
    ///   http://xmpp.org/rfcs/rfc3920.html
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "http://etherx.jabber.org/streams")]
    [XmlRootAttribute("features", Namespace = "http://etherx.jabber.org/streams", IsNullable = false)]
    public class StreamFeatures {
        private Bind bindField;
        private EntityCapabilities entityCapabilitiesField;
        private ArrayList itemsField;
        private Mechanisms mechanismsField;
        private RosterVersioningFeature rosterVersioningField;
        private Session sessionField;
        private bool sessionFieldSpecified;
        private StartTls starttlsField;

        public StreamFeatures() {
            itemsField = new ArrayList();
        }

        [XmlElementAttribute("c", Namespace = "http://jabber.org/protocol/caps")]
        public EntityCapabilities EntityCapabilities {
            get { return entityCapabilitiesField; }
            set { entityCapabilitiesField = value; }
        }

        [XmlElementAttribute("ver", Namespace = "urn:xmpp:features:rosterver")]
        public RosterVersioningFeature RosterVersioning {
            get { return rosterVersioningField; }
            set { rosterVersioningField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("starttls", Namespace = "urn:ietf:params:xml:ns:xmpp-tls")]
        public StartTls StartTls {
            get { return starttlsField; }
            set { starttlsField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("mechanisms", Namespace = "urn:ietf:params:xml:ns:xmpp-sasl")]
        public Mechanisms Mechanisms {
            get { return mechanismsField; }
            set { mechanismsField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("bind", Namespace = "urn:ietf:params:xml:ns:xmpp-bind")]
        public Bind Bind {
            get { return bindField; }
            set { bindField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("session", Namespace = "urn:ietf:params:xml:ns:xmpp-session")]
        public Session Session {
            get { return sessionField; }
            set {
                SessionSpecified = true;
                sessionField = value;
            }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool SessionSpecified {
            get { return sessionFieldSpecified; }
            set { sessionFieldSpecified = value; }
        }

        /// <remarks />
        [XmlElementAttribute("register", typeof (RegisterIQ), Namespace = "http://jabber.org/features/iq-register")]
        public ArrayList Items {
            get { return itemsField; }
        }
    }
}