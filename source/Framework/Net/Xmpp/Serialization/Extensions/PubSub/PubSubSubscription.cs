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
using System.Xml.Serialization;

namespace BabelIm.Net.Xmpp.Serialization.Extensions.PubSub
{
    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://jabber.org/protocol/pubsub")]
    [XmlRootAttribute("subscription", Namespace = "http://jabber.org/protocol/pubsub", IsNullable = false)]
    public class PubSubSubscription
    {
        #region · Fields ·

        private PubSubSubscribeOptions subscribeoptionsField;
        private string jidField;
        private string nodeField;
        private string subidField;
        private PubSubSubscriptionType subscriptionField;
        private bool subscriptionFieldSpecified;

        #endregion

        #region · Properties ·

        /// <remarks/>
        [XmlElementAttribute("subscribe-options")]
        public PubSubSubscribeOptions SubscribeOptions
        {
            get { return this.subscribeoptionsField; }
            set { this.subscribeoptionsField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("jid")]
        public string Jid
        {
            get
            {
                return this.jidField;
            }
            set
            {
                this.jidField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute("node")]
        public string Node
        {
            get { return this.nodeField; }
            set { this.nodeField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("subid")]
        public string Subid
        {
            get { return this.subidField; }
            set { this.subidField = value; }
        }

        /// <remarks/>
        [XmlAttributeAttribute("subscription")]
        public PubSubSubscriptionType Subscription
        {
            get { return this.subscriptionField; }
            set
            {
                this.subscriptionField = value;
                this.subscriptionFieldSpecified = true;
            }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool SubscriptionSpecified
        {
            get { return this.subscriptionFieldSpecified; }
            set { this.subscriptionFieldSpecified = value; }
        }

        #endregion

        #region · Constructors ·

        public PubSubSubscription()
        {
            if ((this.subscribeoptionsField == null))
            {
                this.subscribeoptionsField = new PubSubSubscribeOptions();
            }
        }

        #endregion
    }
}
