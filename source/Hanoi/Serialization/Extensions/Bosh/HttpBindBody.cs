/*
    Copyright (c) 2007-2010, Carlos Guzmán Álvarez

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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Hanoi.Serialization.Core.Sasl;
using Hanoi.Serialization.Core.Streams;
using Hanoi.Serialization.InstantMessaging.Client;
using Hanoi.Serialization.InstantMessaging.Presence;

namespace Hanoi.Serialization.Extensions.Bosh {
    /// <summary>
    ///   XEP-0124: Bidirectional-streams Over Synchronous HTTP (BOSH)
    ///   XEP-0206: XMPP Over BOSH
    /// </summary>
    [Serializable]
    [XmlType("body", AnonymousType = true, Namespace = "http://jabber.org/protocol/httpbind")]
    [XmlRootAttribute(ElementName = "body", Namespace = "http://jabber.org/protocol/httpbind", IsNullable = false)]
    public sealed class HttpBindBody {
        private string acceptField;
        private string ackField;
        private XmlAttribute[] anyAttrField;
        private string authidField;
        private string charsetsField;
        private BodyCondition conditionField;
        private bool conditionFieldSpecified;
        private string contentField;
        private string fromField;
        private byte holdField;
        private bool holdFieldSpecified;
        private short inactivityField;
        private bool inactivityFieldSpecified;
        private ArrayList itemsField;
        private string keyField;
        private string langField;
        private short maxpauseField;
        private bool maxpauseFieldSpecified;
        private string newkeyField;
        private short pauseField;
        private bool pauseFieldSpecified;
        private short pollingField;
        private bool pollingFieldSpecified;
        private string reportField;
        private byte requestsField;
        private bool requestsFieldSpecified;
        private bool restartField;
        private bool restartLogicField;
        private string ridField;
        private string routeField;
        private string sidField;
        private string streamField;
        private short timeField;
        private bool timeFieldSpecified;
        private string toField;
        private BodyType typeField;
        private bool typeFieldSpecified;
        private string verField;
        private string versionField;
        private short waitField;
        private bool waitFieldSpecified;

        public HttpBindBody() {
            itemsField = new ArrayList();
        }

        /// <remarks />
        [XmlElementAttribute("stream:error", Type = typeof (StreamError), Namespace = "jabber:client",
            Form = XmlSchemaForm.Qualified)]
        [XmlElementAttribute("failure", Type = typeof (Failure), Namespace = "urn:ietf:params:xml:ns:xmpp-sasl")]
        [XmlElementAttribute("auth", Type = typeof (Auth), Namespace = "urn:ietf:params:xml:ns:xmpp-sasl")]
        [XmlElementAttribute("challenge", Type = typeof (Challenge), Namespace = "urn:ietf:params:xml:ns:xmpp-sasl")]
        [XmlElementAttribute("response", Type = typeof (Response), Namespace = "urn:ietf:params:xml:ns:xmpp-sasl")]
        [XmlElementAttribute("success", Type = typeof (Success), Namespace = "urn:ietf:params:xml:ns:xmpp-sasl")]
        [XmlElementAttribute("features", Type = typeof (Core.Streams.StreamFeatures), Namespace = "http://etherx.jabber.org/streams",
            Form = XmlSchemaForm.Qualified)]
        [XmlElementAttribute("iq", Type = typeof (IQ), Namespace = "jabber:client")]
        [XmlElementAttribute("presence", Type = typeof (Presence), Namespace = "jabber:client")]
        [XmlElementAttribute("message", Type = typeof (InstantMessaging.Client.Message), Namespace = "jabber:client")]
        [XmlElementAttribute("uri", typeof (string))]
        public ArrayList Items {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("accept")]
        public string Accept {
            get { return acceptField; }
            set { acceptField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("ack", DataType = "positiveInteger")]
        public string Ack {
            get { return ackField; }
            set { ackField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("authid")]
        public string AuthId {
            get { return authidField; }
            set { authidField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("charsets", DataType = "NMTOKENS")]
        public string Charsets {
            get { return charsetsField; }
            set { charsetsField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("condition")]
        public BodyCondition Condition {
            get { return conditionField; }
            set { conditionField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool ConditionSpecified {
            get { return conditionFieldSpecified; }
            set { conditionFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("content")]
        public string Content {
            get { return contentField; }
            set { contentField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("from")]
        public string From {
            get { return fromField; }
            set { fromField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("hold")]
        public byte Hold {
            get { return holdField; }
            set { holdField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool HoldSpecified {
            get { return holdFieldSpecified; }
            set { holdFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("inactivity")]
        public short Inactivity {
            get { return inactivityField; }
            set { inactivityField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool InactivitySpecified {
            get { return inactivityFieldSpecified; }
            set { inactivityFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("key")]
        public string Key {
            get { return keyField; }
            set { keyField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("maxpause")]
        public short MaxPause {
            get { return maxpauseField; }
            set { maxpauseField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool MaxPauseSpecified {
            get { return maxpauseFieldSpecified; }
            set { maxpauseFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("newkey")]
        public string NewKey {
            get { return newkeyField; }
            set { newkeyField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("pause")]
        public short Pause {
            get { return pauseField; }
            set { pauseField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool PauseSpecified {
            get { return pauseFieldSpecified; }
            set { pauseFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("polling")]
        public short Polling {
            get { return pollingField; }
            set { pollingField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool PollingSpecified {
            get { return pollingFieldSpecified; }
            set { pollingFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("report", DataType = "positiveInteger")]
        public string Report {
            get { return reportField; }
            set { reportField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("requests")]
        public byte Requests {
            get { return requestsField; }
            set { requestsField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool RequestsSpecified {
            get { return requestsFieldSpecified; }
            set { requestsFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("rid", DataType = "positiveInteger")]
        public string Rid {
            get { return ridField; }
            set { ridField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("route")]
        public string Route {
            get { return routeField; }
            set { routeField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("sid")]
        public string Sid {
            get { return sidField; }
            set { sidField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("stream")]
        public string Stream {
            get { return streamField; }
            set { streamField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("time")]
        public short Time {
            get { return timeField; }
            set { timeField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool TimeSpecified {
            get { return timeFieldSpecified; }
            set { timeFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("to")]
        public string To {
            get { return toField; }
            set { toField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("type")]
        public BodyType Type {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool TypeSpecified {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("ver")]
        public string Ver {
            get { return verField; }
            set { verField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("wait")]
        public short Wait {
            get { return waitField; }
            set { waitField = value; }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool WaitSpecified {
            get { return waitFieldSpecified; }
            set { waitFieldSpecified = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("lang", Form = System.Xml.Schema.XmlSchemaForm.Qualified,
            Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Lang {
            get { return langField; }
            set { langField = value; }
        }

        /// <remarks />
        [XmlAnyAttributeAttribute]
        public System.Xml.XmlAttribute[] AnyAttr {
            get { return anyAttrField; }
            set { anyAttrField = value; }
        }

        [XmlAttributeAttribute("restart", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:xmpp:xbosh"
            )]
        public bool Restart {
            get { return restartField; }
            set { restartField = value; }
        }

        [XmlAttributeAttribute("restartlogic", Form = System.Xml.Schema.XmlSchemaForm.Qualified,
            Namespace = "urn:xmpp:xbosh")]
        public bool RestartLogicField {
            get { return restartLogicField; }
            set { restartLogicField = value; }
        }

        [XmlAttributeAttribute("version", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:xmpp:xbosh"
            )]
        public string VersionField {
            get { return versionField; }
            set { versionField = value; }
        }
    }
}