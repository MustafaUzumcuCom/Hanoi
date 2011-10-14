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

using System.Xml.Serialization;

namespace Hanoi.Xmpp.InstantMessaging.ServiceDiscovery {
    /// <summary>
    ///   XMPP Service Identity
    /// </summary>
    [XmlType(Namespace = "")]
    [XmlRootAttribute("identity", Namespace = "", IsNullable = false)]
    public sealed class XmppServiceIdentity {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppServiceIdentity" /> class.
        /// </summary>
        public XmppServiceIdentity() {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppServiceIdentity" /> class.
        /// </summary>
        /// <param name = "name">Identity name</param>
        /// <param name = "category">Identity category</param>
        /// <param name = "type">Identity type</param>
        public XmppServiceIdentity(string name, string category, string type)
            : this(name, InferCategory(category), type) {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppServiceIdentity" /> class.
        /// </summary>
        /// <param name = "name">Identity name</param>
        /// <param name = "category">Identity category</param>
        /// <param name = "type">Identity type</param>
        public XmppServiceIdentity(string name, XmppServiceCategory category, string type) {
            this.Name = name;
            this.Category = category;
            this.Type = type;
        }

        /// <summary>
        ///   Gets the identity name
        /// </summary>
        [XmlAttributeAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        ///   Gets the identity category
        /// </summary>
        [XmlAttributeAttribute("category")]
        public XmppServiceCategory Category { get; set; }

        /// <summary>
        ///   Gets the identity type
        /// </summary>
        [XmlAttributeAttribute("type")]
        public string Type { get; set; }

        private static XmppServiceCategory InferCategory(string category) {
            switch (category)
            {
                case "account":
                    return XmppServiceCategory.Account;

                case "auth":
                    return XmppServiceCategory.Auth;

                case "automation":
                    return XmppServiceCategory.Automation;

                case "client":
                    return XmppServiceCategory.Client;

                case "collaboration":
                    return XmppServiceCategory.Collaboration;

                case "component":
                    return XmppServiceCategory.Component;

                case "conference":
                    return XmppServiceCategory.Conference;

                case "directory":
                    return XmppServiceCategory.Directory;

                case "gateway":
                    return XmppServiceCategory.Gateway;

                case "headline":
                    return XmppServiceCategory.Headline;

                case "hierarchy":
                    return XmppServiceCategory.Hierarchy;

                case "proxy":
                    return XmppServiceCategory.Proxy;

                case "pubsub":
                    return XmppServiceCategory.Pubsub;

                case "server":
                    return XmppServiceCategory.Server;

                case "store":
                    return XmppServiceCategory.Store;

                default:
                    return XmppServiceCategory.Unknown;
            }
        }
    }
}