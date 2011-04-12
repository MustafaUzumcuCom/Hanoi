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
using System.CodeDom.Compiler;
using System.Collections;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using BabelIm.Net.Xmpp.Serialization.Core.Sasl;
using BabelIm.Net.Xmpp.Serialization.Core.Streams;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client.Presence;

namespace BabelIm.Net.Xmpp.Serialization.Extensions.Bosh
{
    /// <summary>
    /// XEP-0124: Bidirectional-streams Over Synchronous HTTP (BOSH)
    /// XEP-0206: XMPP Over BOSH
    /// </summary>
    [SerializableAttribute()]
    [XmlTypeAttribute("body", AnonymousType = true, Namespace = "http://jabber.org/protocol/httpbind")]
    [XmlRootAttribute(ElementName="body", Namespace = "http://jabber.org/protocol/httpbind", IsNullable = false)]
    public sealed class HttpBindBody
    {
        #region · Fields ·

        private ArrayList       itemsField;
        private string          acceptField;
        private string          ackField;
        private string          authidField;
        private string          charsetsField;
        private BodyCondition   conditionField;
        private bool            conditionFieldSpecified;
        private string          contentField;
        private string          fromField;
        private byte            holdField;
        private bool            holdFieldSpecified;
        private short           inactivityField;
        private bool            inactivityFieldSpecified;
        private string          keyField;
        private short           maxpauseField;
        private bool            maxpauseFieldSpecified;
        private string          newkeyField;
        private short           pauseField;
        private bool            pauseFieldSpecified;
        private short           pollingField;
        private bool            pollingFieldSpecified;
        private string          reportField;
        private byte            requestsField;
        private bool            requestsFieldSpecified;
        private string          ridField;
        private string          routeField;
        private string          sidField;
        private string          streamField;
        private short           timeField;
        private bool            timeFieldSpecified;
        private string          toField;
        private BodyType        typeField;
        private bool            typeFieldSpecified;
        private string          verField;
        private short           waitField;
        private bool            waitFieldSpecified;
        private string          langField;
        private bool            restartField;
        private bool            restartLogicField;
        private string          versionField;
        private XmlAttribute[]  anyAttrField;

        #endregion

        #region · Properties ·

        /// <remarks/>
        [XmlElementAttribute("stream:error", Type = typeof(StreamError), Namespace = "jabber:client", Form = XmlSchemaForm.Qualified)]
        [XmlElementAttribute("failure", Type = typeof(Failure), Namespace = "urn:ietf:params:xml:ns:xmpp-sasl")]
        [XmlElementAttribute("auth", Type = typeof(Auth), Namespace = "urn:ietf:params:xml:ns:xmpp-sasl")]
        [XmlElementAttribute("challenge", Type = typeof(Challenge), Namespace = "urn:ietf:params:xml:ns:xmpp-sasl")]
        [XmlElementAttribute("response", Type = typeof(Response), Namespace = "urn:ietf:params:xml:ns:xmpp-sasl")]
        [XmlElementAttribute("success", Type = typeof(Success), Namespace = "urn:ietf:params:xml:ns:xmpp-sasl")]
        [XmlElementAttribute("features", Type = typeof(StreamFeatures), Namespace = "http://etherx.jabber.org/streams", Form = XmlSchemaForm.Qualified)]
        [XmlElementAttribute("iq", Type = typeof(IQ), Namespace = "jabber:client")]
        [XmlElementAttribute("presence", Type = typeof(Presence), Namespace = "jabber:client")]
        [XmlElementAttribute("message", Type = typeof(Message), Namespace = "jabber:client")]
        [XmlElementAttribute("uri", typeof(string))]
        public ArrayList Items
        {
            get { return this.itemsField; }
            set { this.itemsField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("accept")]
        public string Accept
        {
            get { return this.acceptField; }
            set { this.acceptField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("ack", DataType = "positiveInteger")]
        public string Ack
        {
            get { return this.ackField; }
            set { this.ackField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("authid")]
        public string AuthId
        {
            get { return this.authidField; }
            set { this.authidField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("charsets", DataType = "NMTOKENS")]
        public string Charsets
        {
            get { return this.charsetsField; }
            set { this.charsetsField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("condition")]
        public BodyCondition Condition
        {
            get { return this.conditionField; }
            set { this.conditionField = value; }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool ConditionSpecified
        {
            get { return this.conditionFieldSpecified; }
            set { this.conditionFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("content")]
        public string Content
        {
            get { return this.contentField; }
            set { this.contentField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("from")]
        public string From
        {
            get { return this.fromField; }
            set { this.fromField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("hold")]
        public byte Hold
        {
            get { return this.holdField; }
            set { this.holdField = value; }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool HoldSpecified
        {
            get { return this.holdFieldSpecified; }
            set { this.holdFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("inactivity")]
        public short Inactivity
        {
            get { return this.inactivityField; }
            set { this.inactivityField = value; }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool InactivitySpecified
        {
            get { return this.inactivityFieldSpecified; }
            set { this.inactivityFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("key")]
        public string Key
        {
            get { return this.keyField; }
            set { this.keyField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("maxpause")]
        public short MaxPause
        {
            get { return this.maxpauseField; }
            set { this.maxpauseField = value; }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool MaxPauseSpecified
        {
            get { return this.maxpauseFieldSpecified; }
            set { this.maxpauseFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("newkey")]
        public string NewKey
        {
            get { return this.newkeyField; }
            set { this.newkeyField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("pause")]
        public short Pause
        {
            get { return this.pauseField; }
            set { this.pauseField = value; }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool PauseSpecified
        {
            get { return this.pauseFieldSpecified; }
            set { this.pauseFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("polling")]
        public short Polling
        {
            get { return this.pollingField; }
            set { this.pollingField = value; }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool PollingSpecified
        {
            get { return this.pollingFieldSpecified; }
            set { this.pollingFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("report", DataType = "positiveInteger")]
        public string Report
        {
            get { return this.reportField; }
            set { this.reportField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("requests")]
        public byte Requests
        {
            get { return this.requestsField; }
            set { this.requestsField = value; }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool RequestsSpecified
        {
            get { return this.requestsFieldSpecified; }
            set { this.requestsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("rid", DataType = "positiveInteger")]
        public string Rid
        {
            get { return this.ridField; }
            set { this.ridField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("route")]
        public string Route
        {
            get { return this.routeField; }
            set { this.routeField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("sid")]
        public string Sid
        {
            get { return this.sidField; }
            set { this.sidField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("stream")]
        public string Stream
        {
            get { return this.streamField; }
            set { this.streamField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("time")]
        public short Time
        {
            get { return this.timeField; }
            set { this.timeField = value; }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool TimeSpecified
        {
            get { return this.timeFieldSpecified; }
            set { this.timeFieldSpecified = value; }
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
        public BodyType Type
        {
            get { return this.typeField; }
            set { this.typeField = value; }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool TypeSpecified
        {
            get { return this.typeFieldSpecified; }
            set { this.typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("ver")]
        public string Ver
        {
            get { return this.verField; }
            set { this.verField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("wait")]
        public short Wait
        {
            get { return this.waitField; }
            set { this.waitField = value; }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool WaitSpecified
        {
            get { return this.waitFieldSpecified; }
            set { this.waitFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("lang", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Lang
        {
            get { return this.langField; }
            set { this.langField = value; }
        }

        /// <remarks/>
        [XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get { return this.anyAttrField; }
            set { this.anyAttrField = value; }
        }

        [XmlAttributeAttribute("restart", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:xmpp:xbosh")]
        public bool Restart
        {
            get { return this.restartField; }
            set { this.restartField = value; }
        }

        [XmlAttributeAttribute("restartlogic", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:xmpp:xbosh")]
        public bool RestartLogicField
        {
            get { return this.restartLogicField; }
            set { this.restartLogicField = value; }
        }

        [XmlAttributeAttribute("version", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:xmpp:xbosh")]
        public string VersionField
        {
            get { return this.versionField; }
            set { this.versionField = value; }
        }

        #endregion

        #region · Constructors ·

        public HttpBindBody()
        {
            this.itemsField = new ArrayList();
        }

        #endregion
    }
}