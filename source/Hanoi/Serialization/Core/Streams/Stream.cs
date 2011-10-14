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
using Hanoi.Serialization.InstantMessaging.Client;
using Hanoi.Serialization.InstantMessaging.Presence;

namespace Hanoi.Serialization.Core.Streams {
    /// <remarks />
    [Serializable]
    [XmlType(Namespace = "http://etherx.jabber.org/streams")]
    [XmlRootAttribute("stream", Namespace = "http://etherx.jabber.org/streams", IsNullable = false)]
    public class Stream {
        private StreamError errorField;
        private StreamFeatures featuresField;
        private string fromField;
        private string idField;
        private ArrayList itemsField;
        private string langField;
        private string toField;
        private string versionField;

        public Stream() {
            itemsField = new ArrayList();
        }

        /// <remarks />
        [XmlElementAttribute("features")]
        public StreamFeatures Features {
            get { return featuresField; }
            set { featuresField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("presence", typeof (Presence), Namespace = "jabber:client")]
        [XmlElementAttribute("iq", typeof (IQ), Namespace = "jabber:client")]
        [XmlElementAttribute("message", typeof (Message), Namespace = "jabber:client")]
        public ArrayList Items {
            get { return itemsField; }
        }

        /// <remarks />
        [XmlElementAttribute("error")]
        public StreamError Error {
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
        [XmlAttributeAttribute("version")]
        public string Version {
            get { return versionField; }
            set { versionField = value; }
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