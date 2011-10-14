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

namespace Hanoi.Xmpp {
    /// <summary>
    ///   Internal constants
    /// </summary>
    internal static class XmppCodes {
        /// <summary>
        ///   Namespace URI of the XMPP stream
        /// </summary>
        public const string XmppNamespaceURI = "http://etherx.jabber.org/streams";

        /// <summary>
        ///   Code for the SASL Digest authentication mechanism
        /// </summary>
        public const string SaslDigestMD5Mechanism = "DIGEST-MD5";

        /// <summary>
        ///   Code for the SASL PLAIN authentication mechanism
        /// </summary>
        public const string SaslPlainMechanism = "PLAIN";

        /// <summary>
        ///   Code for the SASL X-GOOGLE-Token authentication mechanism
        /// </summary>
        public const string SaslXGoogleTokenMechanism = "X-GOOGLE-TOKEN";

        /// <summary>
        ///   XMPP Stream XML root node name
        /// </summary>
        internal static readonly string XmppStreamName = "stream:stream";

        /// <summary>
        ///   XMPP Stream XML open node tag
        /// </summary>
        internal static readonly string XmppStreamOpen = String.Format("<{0} ", XmppStreamName);

        /// <summary>
        ///   XMPP Stream XML close node tag
        /// </summary>
        internal static readonly string XmppStreamClose = String.Format("</{0}>", XmppStreamName);

        /// <summary>
        ///   XMPP DNS SRV Record prefix
        /// </summary>
        internal static string XmppSrvRecordPrefix = "_xmpp-client._tcp";
    }
}