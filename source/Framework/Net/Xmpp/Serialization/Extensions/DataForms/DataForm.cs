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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Hanoi.Xmpp.Serialization.Extensions.DataForms {
    /// <remarks />
    [Serializable]
    [XmlType(Namespace = "jabber:x:data")]
    [XmlRootAttribute("x", Namespace = "jabber:x:data", IsNullable = false)]
    public class DataForm {
        private List<DataFormField> fields;
        private List<string> instructions;
        private List<DataFormItem> items;
        private DataFormReported reported;
        private string title;
        private DataFormType type;

        /// <remarks />
        [XmlElementAttribute("instructions")]
        public List<string> Instructions {
            get {
                if (instructions == null)
                {
                    instructions = new List<string>();
                }

                return instructions;
            }
        }

        /// <remarks />
        [XmlElementAttribute("title")]
        public string Title {
            get { return title; }
            set { title = value; }
        }

        /// <remarks />
        [XmlElementAttribute("field")]
        public List<DataFormField> Fields {
            get {
                if (fields == null)
                {
                    fields = new List<DataFormField>();
                }

                return fields;
            }
        }

        /// <remarks />
        [XmlElementAttribute("reported", IsNullable = false)]
        public DataFormReported Reported {
            get { return reported; }
            set { reported = value; }
        }

        /// <remarks />
        [XmlArrayItemAttribute("item", IsNullable = false)]
        public List<DataFormItem> Items {
            get {
                if (items == null)
                {
                    items = new List<DataFormItem>();
                }

                return items;
            }
        }

        /// <remarks />
        [XmlAttributeAttribute("type")]
        public DataFormType Type {
            get { return type; }
            set { type = value; }
        }
    }
}