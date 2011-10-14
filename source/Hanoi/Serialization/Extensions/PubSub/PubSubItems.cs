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

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Hanoi.Serialization.Extensions.PubSub {
    /// <remarks />
    [XmlType(AnonymousType = true, Namespace = "http://jabber.org/protocol/pubsub")]
    [XmlRootAttribute("items", Namespace = "http://jabber.org/protocol/pubsub", IsNullable = false)]
    public class PubSubItems {
        private List<PubSubItem> itemField;

        public PubSubItems() {
            if ((itemField == null))
            {
                itemField = new List<PubSubItem>();
            }
        }

        /// <remarks />
        [XmlElementAttribute("item")]
        public List<PubSubItem> Item {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks />
        [XmlAttributeAttribute("max_items", DataType = "positiveInteger")]
        public string MaxItems { get; set; }

        /// <remarks />
        [XmlAttributeAttribute("node")]
        public string Node { get; set; }

        /// <remarks />
        [XmlAttributeAttribute("subid")]
        public string Subid { get; set; }
    }
}