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
    EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Xml.Serialization;

namespace Hanoi.Serialization.Core.Sasl {
    /// <remarks />
    [Serializable]
    [XmlType(Namespace = "urn:ietf:params:xml:ns:xmpp-sasl")]
    [XmlRootAttribute("failure", Namespace = "urn:ietf:params:xml:ns:xmpp-sasl", IsNullable = false)]
    public class Failure {
        private FailiureType itemElementNameField;
        private Empty itemField;

        /// <remarks />
        [XmlElementAttribute("not-authorized", typeof (Empty))]
        [XmlElementAttribute("mechanism-too-weak", typeof (Empty))]
        [XmlElementAttribute("temporary-auth-failure", typeof (Empty))]
        [XmlElementAttribute("invalid-authzid", typeof (Empty))]
        [XmlElementAttribute("aborted", typeof (Empty))]
        [XmlElementAttribute("incorrect-encoding")]
        [XmlElementAttribute("invalid-mechanism", typeof (Empty))]
        [XmlChoiceIdentifierAttribute("FailiureType")]
        public Empty Item {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks />
        [XmlIgnore]
        public FailiureType FailiureType {
            get { return itemElementNameField; }
            set { itemElementNameField = value; }
        }
    }
}