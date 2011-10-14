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
using Hanoi.Xmpp.Serialization;

namespace Hanoi.Serialization.Core.ResourceBinding {
    /// <summary>
    ///   http://xmpp.org/rfcs/rfc3920.html
    ///   http://xmpp.org/extensions/xep-0193.html
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "urn:ietf:params:xml:ns:xmpp-bind")]
    [XmlRootAttribute("bind", Namespace = "urn:ietf:params:xml:ns:xmpp-bind", IsNullable = false)]
    public class Bind {
        private ItemChoiceType itemElementNameField;
        private Empty itemField;
        private string jidField;
        private string resourceField;

        /// <remarks />
        [XmlElementAttribute("optional", typeof (Empty))]
        [XmlElementAttribute("required", typeof (Empty))]
        [XmlChoiceIdentifierAttribute("ItemElementName")]
        public Empty Item {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public ItemChoiceType ItemElementName {
            get { return itemElementNameField; }
            set { itemElementNameField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("resource")]
        public string Resource {
            get { return resourceField; }
            set { resourceField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("jid")]
        public string Jid {
            get { return jidField; }
            set { jidField = value; }
        }
    }
}