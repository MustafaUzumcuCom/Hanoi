/*
    Copyright (c) 2007-2010, Carlos Guzm�n �lvarez

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

namespace BabelIm.Net.Xmpp.Serialization.Extensions.Nickname {
    /// <summary>
    ///   XEP-0172: User Nickname
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "http://jabber.org/protocol/nick")]
    [XmlRootAttribute("nick", Namespace = "http://jabber.org/protocol/nick", IsNullable = false)]
    public class Nickname {
        private string nick;
        private string valueField;

        /// <remarks />
        [XmlAttributeAttribute("nick", Form = System.Xml.Schema.XmlSchemaForm.Qualified,
            Namespace = "http://jabber.org/protocol/nick")]
        public string Nick {
            get { return nick; }
            set { nick = value; }
        }

        /// <remarks />
        [XmlTextAttribute]
        public string Value {
            get { return valueField; }
            set { valueField = value; }
        }
    }
}