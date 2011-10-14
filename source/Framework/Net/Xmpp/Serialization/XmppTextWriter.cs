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

using System.IO;
using System.Text;
using System.Xml;

namespace Hanoi.Xmpp.Serialization {
    /// <summary>
    ///   Custom <see cref = "XmlTextWriter" /> implementation for writing XMPP stanzas
    /// </summary>
    public sealed class XmppTextWriter
        : XmlTextWriter {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppTextWriter" /> class.
        /// </summary>
        /// <param name = "w">The TextWriter to write to. It is assumed that the TextWriter is already set to the correct encoding.</param>
        public XmppTextWriter(TextWriter w)
            : base(w) {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppTextWriter" /> class.
        /// </summary>
        /// <param name = "w">The w.</param>
        public XmppTextWriter(Stream w)
            : this(w, Encoding.UTF8) {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppTextWriter" /> class.
        /// </summary>
        /// <param name = "w">The w.</param>
        /// <param name = "encoding">The encoding.</param>
        public XmppTextWriter(Stream w, Encoding encoding)
            : base(w, encoding) {
        }

        /// <summary>
        ///   Writes the XML declaration with the version "1.0".
        /// </summary>
        /// <exception cref = "T:System.InvalidOperationException">This is not the first write method called after the constructor. </exception>
        public override void WriteStartDocument() {
        }

        /// <summary>
        ///   Writes the XML declaration with the version "1.0" and the standalone attribute.
        /// </summary>
        /// <param name = "standalone">If true, it writes "standalone=yes"; if false, it writes "standalone=no".</param>
        /// <exception cref = "T:System.InvalidOperationException">This is not the first write method called after the constructor. </exception>
        public override void WriteStartDocument(bool standalone) {
        }
        }
}