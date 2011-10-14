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
using System.Collections.Generic;
using System.Xml.Serialization;
using Hanoi.Xmpp.InstantMessaging.ServiceDiscovery;

namespace Hanoi.Xmpp.InstantMessaging.EntityCaps {
    /// <summary>
    ///   Client capabilities (XEP-0115)
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "")]
    [XmlRootAttribute("client", Namespace = "", IsNullable = false)]
    public class XmppClientCapabilities {
        private string hashAlgorithmName;
        private List<XmppServiceIdentity> identities;
        private string node;
        private List<XmppServiceFeature> supportedFeatures;
        private string verificationString;

        /// <summary>
        ///   Gets or sets the client node
        /// </summary>
        [XmlAttribute("node")]
        public string Node {
            get { return node; }
            set { node = value; }
        }

        /// <summary>
        ///   Gets or sets the client version
        /// </summary>
        [XmlAttribute("ver")]
        public string VerificationString {
            get { return verificationString; }
            set { verificationString = value; }
        }

        /// <summary>
        ///   Gets or sets the hash algorithm name
        /// </summary>
        [XmlAttribute("hash")]
        public string HashAlgorithmName {
            get { return hashAlgorithmName; }
            set { hashAlgorithmName = value; }
        }

        /// <summary>
        ///   Gets or sets the identity.
        /// </summary>
        /// <value>The identity.</value>
        [XmlArray("identities")]
        [XmlArrayItem("identity", typeof (XmppServiceIdentity))]
        public List<XmppServiceIdentity> Identities {
            get {
                if (identities == null)
                {
                    identities = new List<XmppServiceIdentity>();
                }

                return identities;
            }
        }

        /// <summary>
        ///   Gets the list of supported features
        /// </summary>
        [XmlArray("features")]
        [XmlArrayItem("feature", typeof (XmppServiceFeature))]
        public List<XmppServiceFeature> Features {
            get {
                if (supportedFeatures == null)
                {
                    supportedFeatures = new List<XmppServiceFeature>();
                }

                return supportedFeatures;
            }
        }

        /// <summary>
        ///   Gets the discovery info node
        /// </summary>
        [XmlIgnore]
        public string DiscoveryInfoNode {
            get { return String.Format("{0}#{1}", Node, VerificationString); }
        }
    }
}