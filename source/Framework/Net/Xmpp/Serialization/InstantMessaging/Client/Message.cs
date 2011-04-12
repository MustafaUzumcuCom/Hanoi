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
using System.ComponentModel;
using System.Xml.Serialization;
using BabelIm.Net.Xmpp.Serialization.Extensions.MultiUserChat.User;
using BabelIm.Net.Xmpp.Serialization.Extensions.PubSub;

namespace BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client
{
    /// <remarks/>
    [Serializable]
    [XmlTypeAttribute(Namespace = "jabber:client")]
    [XmlRootAttribute("message", Namespace = "jabber:client", IsNullable = false)]
    public class Message
    {
        #region · Fields ·

        private ArrayList itemsField;
        private Error errorField;
        private string fromField;
        private string idField;
        private string toField;
        private MessageType typeField;
        private string langField;

        #endregion

        #region · Properties ·

        /// <remarks/>
        [XmlElementAttribute("subject", typeof(MessageSubject))]
        [XmlElementAttribute("body", typeof(MessageBody))]
        [XmlElementAttribute("thread", typeof(string), DataType = "NMTOKEN")]
        [XmlElementAttribute("active", typeof(NotificationActive), Namespace = "http://jabber.org/protocol/chatstates")]
        [XmlElementAttribute("composing", typeof(NotificationComposing), Namespace = "http://jabber.org/protocol/chatstates")]
        [XmlElementAttribute("gone", typeof(NotificationGone), Namespace = "http://jabber.org/protocol/chatstates")]
        [XmlElementAttribute("inactive", typeof(NotificationInactive), Namespace = "http://jabber.org/protocol/chatstates")]
        [XmlElementAttribute("paused", typeof(NotificationPaused), Namespace = "http://jabber.org/protocol/chatstates")]
        [XmlElementAttribute("event", typeof(PubSubEvent), Namespace = "http://jabber.org/protocol/pubsub#event")]
        [XmlElementAttribute("x", Type = typeof(MucUser), Namespace = "http://jabber.org/protocol/muc#user")]
        public ArrayList Items
        {
            get { return this.itemsField; }
        }

        /// <remarks/>
        [XmlElement("error")]
        public Error Error
        {
            get { return this.errorField; }
            set { this.errorField = value; }
        }
                
        /// <remarks/>
        [XmlAttributeAttribute("from")]
        public string From
        {
            get { return this.fromField; }
            set { this.fromField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("id", DataType = "NMTOKEN")]
        public string ID
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("to")]
        public string To
        {
            get { return this.toField; }
            set { this.toField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("type")]
        [DefaultValueAttribute(MessageType.Normal)]
        public MessageType Type
        {
            get { return this.typeField; }
            set { this.typeField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("lang", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Lang
        {
            get { return this.langField; }
            set { this.langField = value; } 
        }

        #endregion

        #region · Constructors ·

        public Message()
        {
            this.typeField	= MessageType.Normal;
            this.itemsField = new ArrayList();
        }

        #endregion
    }
}
