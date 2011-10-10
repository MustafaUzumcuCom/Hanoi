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

using System.Xml.Serialization;

namespace BabelIm.Net.Xmpp.Serialization.Extensions.UserTune {
    /// <summary>
    ///   XEP-0118: User Tune
    /// </summary>
    [XmlType(AnonymousType = true, Namespace = "http://jabber.org/protocol/tune")]
    [XmlRootAttribute("tune", Namespace = "http://jabber.org/protocol/tune", IsNullable = false)]
    public class Tune {
        private ushort lengthField;
        private bool lengthFieldSpecified;

        /// <remarks />
        [XmlElementAttribute("artist")]
        public string Artist { get; set; }

        /// <remarks />
        [XmlElementAttribute("length", DataType = "unsignedShort")]
        public ushort Length {
            get { return lengthField; }
            set {
                lengthField = value;
                lengthFieldSpecified = true;
            }
        }

        /// <remarks />
        [XmlIgnoreAttribute]
        public bool LengthSpecified {
            get { return lengthFieldSpecified; }
        }

        /// <remarks />
        [XmlElementAttribute("rating", DataType = "positiveInteger")]
        public string Rating { get; set; }

        /// <remarks />
        [XmlElementAttribute("source")]
        public string Source { get; set; }

        /// <remarks />
        [XmlElementAttribute("title")]
        public string Title { get; set; }

        /// <remarks />
        [XmlElementAttribute("track")]
        public string Track { get; set; }

        /// <remarks />
        [XmlElementAttribute("uri", DataType = "anyURI")]
        public string Uri { get; set; }
    }
}