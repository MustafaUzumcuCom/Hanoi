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

namespace Hanoi.Xmpp.InstantMessaging.EntityCaps
{
    /// <summary>
    ///   Client capabilities (XEP-0115)
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "")]
    [XmlRootAttribute("client", Namespace = "", IsNullable = false)]
    public class ClientCapabilities
    {
        private List<ServiceIdentity> _identities;
        private List<ServiceFeature> _supportedFeatures;

        [XmlAttribute("node")]
        public string Node { get; set; }

        [XmlAttribute("ver")]
        public string VerificationString { get; set; }

        [XmlAttribute("hash")]
        public string HashAlgorithmName { get; set; }

        [XmlArray("identities")]
        [XmlArrayItem("identity", typeof(ServiceIdentity))]
        public List<ServiceIdentity> Identities
        {
            get { return _identities ?? (_identities = new List<ServiceIdentity>()); }
        }

        [XmlArray("features")]
        [XmlArrayItem("feature", typeof(ServiceFeature))]
        public List<ServiceFeature> Features
        {
            get { return _supportedFeatures ?? (_supportedFeatures = new List<ServiceFeature>()); }
        }

        [XmlIgnore]
        public string DiscoveryInfoNode
        {
            get { return String.Format("{0}#{1}", Node, VerificationString); }
        }
    }
}