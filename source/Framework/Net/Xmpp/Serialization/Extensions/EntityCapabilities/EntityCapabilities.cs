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

namespace BabelIm.Net.Xmpp.Serialization.Extensions.EntityCapabilities
{
    /// <summary>
    /// XEP-0115: Entity Capabilities
    /// </summary>
    [SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "http://jabber.org/protocol/caps", IsNullable = false)]
    public sealed class EntityCapabilities
    {
        #region · Fields ·

        private string  nametokens;        
        private string  hashAlgorithmName;
        private string  node;
        private string  verificationString;
        private Empty   valueField;

        #endregion

        #region · Properties ·

        /// <summary>
        /// A set of nametokens specifying additional feature bundles; this attribute is deprecated 
        /// (see the Legacy Format section of this document).
        /// </summary>
        [Obsolete]
        [XmlAttributeAttribute("ext", DataType = "NMTOKENS")]
        public string Nametokens
        {
            get { return this.nametokens; }
            set { this.nametokens = value; }
        }

        /// <summary>
        /// The hashing algorithm used to generate the verification string; 
        /// see Mandatory-to-Implement Technologies regarding supported hashing algorithms.
        /// </summary>
        [XmlAttributeAttribute("hash", DataType = "NMTOKEN")]
        public string HashAlgorithmName
        {
            get { return this.hashAlgorithmName; }
            set { this.hashAlgorithmName = value; }
        }

        /// <summary>
        /// A URI that uniquely identifies a software application, typically a URL at the website 
        /// of the project or company that produces the software
        /// </summary>
        /// <remarks>
        /// It is RECOMMENDED for the value of the 'node' attribute to be an HTTP URL at which a user 
        /// could find further information about the software product, such as "http://psi-im.org" 
        /// for the Psi client; this enables a processing application to also determine a unique string 
        /// for the generating application, which it could maintain in a list of known software 
        /// implementations (e.g., associating the name received via the disco#info reply with the URL 
        /// found in the caps data).
        /// </remarks>
        [XmlAttributeAttribute("node")]
        public string Node
        {
            get { return this.node; }
            set { this.node = value; }
        }

        /// <summary>
        /// A string that is used to verify the identity and supported features of the entity.
        /// </summary>
        /// <remarks>
        /// Before version 1.4 of this specification, the 'ver' attribute was used to specify the released 
        /// version of the software; while the values of the 'ver' attribute that result from use of the 
        /// algorithm specified herein are backwards-compatible, 
        /// applications SHOULD appropriately handle the Legacy Format.
        /// </remarks>
        [XmlAttributeAttribute("ver")]
        public string VerificationString
        {
            get { return this.verificationString; }
            set { this.verificationString = value; }
        }

        /// <comentarios/>
        [XmlTextAttribute()]
        public Empty Value
        {
            get { return this.valueField; }
            set { this.valueField = value; }
        }

        #endregion

        #region · Constructors ·

        public EntityCapabilities()
        {
        }

        #endregion
    }
}
