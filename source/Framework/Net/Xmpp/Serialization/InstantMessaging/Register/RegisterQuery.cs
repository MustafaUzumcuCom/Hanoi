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

namespace Hanoi.Xmpp.Serialization.InstantMessaging.Register {
    /// <remarks />
    [Serializable]
    [XmlType(Namespace = "jabber:iq:register")]
    [XmlRootAttribute("query", Namespace = "jabber:iq:register", IsNullable = false)]
    public class RegisterQuery {
        private string passwordField;
        private string removeField;
        private string usernameField;
        // private object itemField;
        // private RegisterType itemElementNameField;

        /// <remarks />
        [XmlElementAttribute("username")]
        public string UserName {
            get { return usernameField; }
            set { usernameField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("password", typeof (string))]
        public string Password {
            get { return passwordField; }
            set { passwordField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("remove", typeof (string))]
        public string Remove {
            get { return removeField; }
            set { removeField = value; }
        }

        /*
        /// <remarks/>
        [XmlElementAttribute("misc", typeof(string))]
        [XmlElementAttribute("username", typeof(string))]
        [XmlElementAttribute("first", typeof(string))]
        [XmlElementAttribute("url", typeof(string))]
        [XmlElementAttribute("remove", typeof(Empty))]
        [XmlElementAttribute("phone", typeof(string))]
        [XmlElementAttribute("name", typeof(string))]
        [XmlElementAttribute("state", typeof(string))]
        [XmlElementAttribute("registered", typeof(Empty))]
        [XmlElementAttribute("date", typeof(string))]
        [XmlElementAttribute("key", typeof(string))]
        [XmlElementAttribute("city", typeof(string))]
        [XmlElementAttribute("instructions", typeof(string))]
        [XmlElementAttribute("zip", typeof(string))]
        [XmlElementAttribute("nick", typeof(string))]
        [XmlElementAttribute("password", typeof(string))]
        [XmlElementAttribute("email", typeof(string))]
        [XmlElementAttribute("address", typeof(string))]
        [XmlElementAttribute("text", typeof(string))]
        [XmlElementAttribute("last", typeof(string))]
        [XmlChoiceIdentifierAttribute("ItemElementName")]
        public object Item
        {
            get { return this.itemField; }
            set { this.itemField = value; }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public RegisterType Type
        {
            get { return this.itemElementNameField; }
            set { this.itemElementNameField = value; }
        }
        */
    }
}