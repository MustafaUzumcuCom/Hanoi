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
using System.Text;
using Hanoi.Serialization.Extensions.ServiceDiscovery;
using Hanoi.Serialization.InstantMessaging.Client;
using ServiceFeature = Hanoi.Xmpp.InstantMessaging.ServiceDiscovery.ServiceFeature;
using ServiceIdentity = Hanoi.Xmpp.InstantMessaging.ServiceDiscovery.ServiceIdentity;

namespace Hanoi.Xmpp.InstantMessaging.EntityCaps
{
    /// <summary>
    ///   Client capabilities (XEP-0115)
    /// </summary>
    public sealed class SessionEntityCapabilities : EntityCapabilities
    {
        private Session _session;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "EntityCapabilities" /> class.
        /// </summary>
        internal SessionEntityCapabilities(Session session)
            : base(session)
        {
            _session = session;

            // Supported features
            Features.Add(new ServiceFeature(InstantMessaging.Features.ServiceDiscoveryInfo));
            // this.Features.Add(new ServiceFeature(Features.ServiceDiscoveryItems));
            Features.Add(new ServiceFeature(InstantMessaging.Features.EntityCapabilities));
            Features.Add(new ServiceFeature(InstantMessaging.Features.BidirectionalStreamsOverSynchronousHTTP));
            Features.Add(new ServiceFeature(InstantMessaging.Features.ChatStateNotifications));
            Features.Add(new ServiceFeature(InstantMessaging.Features.MultiUserChat));
            Features.Add(new ServiceFeature(InstantMessaging.Features.MultiUserChatUser));
            Features.Add(new ServiceFeature(InstantMessaging.Features.UserMood));
            Features.Add(new ServiceFeature(InstantMessaging.Features.UserMoodWithNotify));
            Features.Add(new ServiceFeature(InstantMessaging.Features.UserTune));
            Features.Add(new ServiceFeature(InstantMessaging.Features.UserTuneWithNotify));
            Features.Add(new ServiceFeature(InstantMessaging.Features.XmppPing));
        }

        public string Name { get; set; }
        public string Version { get; set; }
        public string ServiceDiscoveryName
        {
            get { return string.Format("{0} {1}", Name, Version); }
        }

        public void AdvertiseCapabilities()
        {
            if (string.IsNullOrEmpty(ServiceDiscoveryName) || string.IsNullOrEmpty(Node) || Identities.Count <= 0)
                return;

            var presence = new Serialization.InstantMessaging.Presence.Presence
                               {
                                   Id = IdentifierGenerator.Generate()
                               };

            if (_session.Capabilities != null)
            {
                presence.Items.Add(GetEntityCapabilities());
            }

            _session.Send(presence);
        }

        protected override void OnServiceDiscoveryMessage(IQ message)
        {
            // Answers to Entity Capabilities and service discovery info requests
            if (message.Type != IQType.Get) 
                return;

            if (!(message.Items[0] is ServiceQuery)) 
                return;

            var query = (ServiceQuery)message.Items[0];
            var response = new ServiceQuery
                               {
                                   Node = ((!string.IsNullOrEmpty(query.Node)) ? DiscoveryInfoNode : null)
                               };

            var responseIq = new IQ
                                 {
                                     From = _session.UserId,
                                     ID = message.ID,
                                     To = message.From,
                                     Type = IQType.Result
                                 };

            foreach (var identity in Identities)
            {
                var supportedIdentity = new Serialization.Extensions.ServiceDiscovery.ServiceIdentity
                                            {
                                                Name = identity.Name,
                                                Category = identity.Category.ToString().ToLower(),
                                                Type = identity.Type
                                            };

                response.Identities.Add(supportedIdentity);
            }

            foreach (var supportedFeature in Features)
            {
                var feature = new Serialization.Extensions.ServiceDiscovery.ServiceFeature
                                  {
                                      Name = supportedFeature.Name
                                  };

                response.Features.Add(feature);
            }

            responseIq.Items.Add(response);

            _session.Send(responseIq);

            //// Error response
            //IQ errorIq = new IQ();
            //ServiceQuery response = new ServiceQuery();
            //Error error = new Error();

            //errorIq.From = this.session.UserId.ToString();
            //errorIq.ID = e.QueryResult.ID;
            //errorIq.To = e.QueryResult.From;
            //errorIq.Type = IQType.Error;
            //errorIq.Error = error;
            //response.Node = query.Node;
            //error.Type = ErrorType.Cancel;
            //error.ItemNotFound = "";

            //errorIq.Items.Add(response);

            //this.session.Send(errorIq);
        }

        private Serialization.Extensions.EntityCapabilities.EntityCapabilities GetEntityCapabilities()
        {
            var capabilities = new Serialization.Extensions.EntityCapabilities.EntityCapabilities
                                   {
                                       HashAlgorithmName = DefaultHashAlgorithmName,
                                       Node = Node,
                                       VerificationString = BuildVerificationString()
                                   };

            VerificationString = capabilities.VerificationString;
            return capabilities;
        }

        /// <summary>
        ///   Builds the verification string.
        /// </summary>
        /// <returns>The encoded verification string</returns>
        /// <remarks>
        ///   5.1 In order to help prevent poisoning of entity capabilities information, 
        ///   the value of the verification string MUST be generated according to the following method.             
        /// 
        ///   1. Initialize an empty string S.
        ///   2. Sort the service discovery identities [15] by category and then by type and then by xml:lang (if it exists), 
        ///   formatted as CATEGORY '/' [TYPE] '/' [LANG] '/' [NAME]. [16] Note that each slash is included even if the TYPE, LANG, 
        ///   or NAME is not included.
        ///   3. For each identity, append the 'category/type/lang/name' to S, followed by the '<' character.
        /// 4. Sort the supported service discovery features. [17]
        /// 5. For each feature, append the feature to S, followed by the '<' character.
        /// 6. If the service discovery information response includes XEP-0128 data forms, 
        ///    sort the forms by the FORM_TYPE (i.e., by the XML character data of the <value /> element).
        ///   7. For each extended service discovery information form:
        ///   1. Append the XML character data of the FORM_TYPE field's <value /> element, followed by the '<' character.
        ///     2. Sort the fields by the value of the "var" attribute.
        ///     3. For each field:
        ///         1. Append the value of the "var" attribute, followed by the '<' character.
        ///         2. Sort values by the XML character data of the <value /> element.
        ///   3. For each <value /> element, append the XML character data, followed by the '<' character.
        /// 8. Ensure that S is encoded according to the UTF-8 encoding (RFC 3269 [18]).
        /// 9. Compute the verification string by hashing S using the algorithm specified in the 'hash' attribute 
        ///    (e.g., SHA-1 as defined in RFC 3174 [19]). The hashed data MUST be generated with binary output and 
        ///    encoded using Base64 as specified in Section 4 of RFC 4648 [20] (note: the Base64 output MUST NOT 
        ///    include whitespace and MUST set padding bits to zero). [21]
        ///    
        /// 5.2 Simple Generation Example
        /// 
        /// Consider an entity whose category is "client", whose service discovery type is "pc", whose service discovery name is "Exodus 0.9.1", and whose supported features are "http://jabber.org/protocol/disco#info", "http://jabber.org/protocol/disco#items", and "http://jabber.org/protocol/muc". Using the SHA-1 algorithm, the verification string would be generated as follows (note: line breaks in the verification string are included only for the purposes of readability):
        /// 1. S = ''
        /// 2. Only one identity: "client/pc"
        /// 3. S = 'client/pc//Exodus 0.9.1<'
        /// 4. Sort the features: "http://jabber.org/protocol/caps", "http://jabber.org/protocol/disco#info", "http://jabber.org/protocol/disco#items", "http://jabber.org/protocol/muc".
        /// 5. S = 'client/pc//Exodus 0.9.1<http://
        ///   jabber.org/protocol/caps<http://jabber.org/protocol/disco#info<
        ///    http://jabber.org/protocol/disco#items<http://jabber.org/protocol/muc<'
        /// 6. ver = QgayPKawpkPSDYmwT/WM94uAlu0=
        /// 
        /// </remarks>
        private string BuildVerificationString()
        {
            var verificationString = new StringBuilder();

            foreach (ServiceIdentity identity in Identities)
            {
                verificationString.AppendFormat("{0}/{1}//{2}<", identity.Category, identity.Type, ServiceDiscoveryName);
            }

            foreach (ServiceFeature supportedFeature in Features)
            {
                verificationString.AppendFormat("{0}<", supportedFeature.Name);
            }

            return verificationString.ComputeSHA1Hash().ToBase64String();
        }
    }
}