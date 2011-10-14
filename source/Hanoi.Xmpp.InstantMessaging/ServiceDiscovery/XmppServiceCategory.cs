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

namespace BabelIm.Net.Xmpp.InstantMessaging.ServiceDiscovery {
    /// <summary>
    ///   Service Categories enumeration
    /// </summary>
    [Serializable]
    public enum XmppServiceCategory {
        /// <summary>
        ///   Support for DNS SRV lookups of XMPP services
        /// </summary>
        Account,

        /// <summary>
        ///   The "auth" category consists of server components that 
        ///   provide authentication services within a server implementation.
        /// </summary>
        Auth,

        /// <summary>
        ///   The "automation" category consists of entities and nodes that 
        ///   provide automated or programmed interaction.
        /// </summary>
        Automation,

        /// <summary>
        ///   The "client" category consists of different types of clients, 
        ///   mostly for instant messaging.
        /// </summary>
        Client,

        /// <summary>
        ///   The "collaboration" category consists of services that enable 
        ///   multiple individuals to work together in real time.
        /// </summary>
        Collaboration,

        /// <summary>
        ///   The "component" category consists of services that are internal 
        ///   to server implementations and not normally exposed outside a server.
        /// </summary>
        Component,

        /// <summary>
        ///   The "conference" category consists of online conference services such 
        ///   as multi-user chatroom services.
        /// </summary>
        Conference,

        /// <summary>
        ///   The "directory" category consists of information retrieval services that 
        ///   enable users to search online directories or otherwise be informed about 
        ///   the existence of other XMPP entities.
        /// </summary>
        Directory,

        /// <summary>
        ///   The "gateway" category consists of translators between Jabber/XMPP services 
        ///   and non-XMPP services.
        /// </summary>
        Gateway,

        /// <summary>
        ///   The "headline" category consists of services that provide real-time news or 
        ///   information (often but not necessarily in a message of type "headline").
        /// </summary>
        Headline,

        /// <summary>
        ///   The "hierarchy" category is used to describe nodes within a hierarchy of nodes; 
        ///   the "branch" and "leaf" types are exhaustive.
        /// </summary>
        Hierarchy,

        /// <summary>
        ///   The "proxy" category consists of servers or services that act as special-purpose 
        ///   proxies or intermediaries between two or more XMPP endpoints.
        /// </summary>
        Proxy,

        /// <summary>
        ///   Services and nodes that adhere to XEP-0060.
        /// </summary>
        Pubsub,

        /// <summary>
        ///   The "server" category consists of any Jabber/XMPP server.
        /// </summary>
        Server,

        /// <summary>
        ///   The "store" category consists of internal server components that provide data 
        ///   storage and retrieval services.
        /// </summary>
        Store,

        /// <summary>
        ///   Unknown service category
        /// </summary>
        Unknown
    }
}