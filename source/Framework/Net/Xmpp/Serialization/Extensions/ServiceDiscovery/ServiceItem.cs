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

namespace Hanoi.Xmpp.Serialization.Extensions.ServiceDiscovery {
    /// <summary>
    ///   XEP-0030: Service Discovery
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "http://jabber.org/protocol/disco#items")]
    [XmlRootAttribute("item", Namespace = "http://jabber.org/protocol/disco#items", IsNullable = false)]
    public class ServiceItem {
        private ServiceActionType actionField;
        private bool actionFieldSpecified;
        private string jidField;
        private string nameField;
        private string nodeField;
        private string valueField;

        /// <remarks />
        [XmlAttributeAttribute("action")]
        public ServiceActionType Action {
            get { return actionField; }
            set { actionField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool actionSpecified {
            get { return actionFieldSpecified; }
            set { actionFieldSpecified = value; }
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
        [XmlAttributeAttribute("node")]
        public string Node {
            get { return nodeField; }
            set { nodeField = value; }
        }

        /// <remarks />
        [XmlTextAttribute]
        public string Value {
            get { return valueField; }
            set { valueField = value; }
        }
    }
}