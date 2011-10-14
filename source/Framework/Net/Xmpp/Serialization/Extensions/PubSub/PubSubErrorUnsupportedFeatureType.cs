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

using System.Xml.Serialization;

namespace Hanoi.Xmpp.Serialization.Extensions.PubSub {
    /// <remarks />
    [XmlType(AnonymousType = true, Namespace = "http://jabber.org/protocol/pubsub#errors")]
    public enum PubSubErrorUnsupportedFeatureType {
        /// <remarks />
        [XmlEnumAttribute("access-authorize")] AccessAuthorize,

        /// <remarks />
        [XmlEnumAttribute("access-open")] AccessOpen,

        /// <remarks />
        [XmlEnumAttribute("access-presence")] AccessPresence,

        /// <remarks />
        [XmlEnumAttribute("access-roster")] AccessRoster,

        /// <remarks />
        [XmlEnumAttribute("access-whitelist")] AccessWhitelist,

        /// <remarks />
        [XmlEnumAttribute("auto-create")] AutoCreate,

        /// <remarks />
        [XmlEnumAttribute("auto-subscribe")] AutoSubscribe,

        /// <remarks />
        [XmlEnumAttribute("collections")] Collections,

        /// <remarks />
        [XmlEnumAttribute("config-node")] ConfigNode,

        /// <remarks />
        [XmlEnumAttribute("create-and-configure")] CreateAndConfigure,

        /// <remarks />
        [XmlEnumAttribute("create-nodes")] CreateNodes,

        /// <remarks />
        [XmlEnumAttribute("delete-any")] DeleteAny,

        /// <remarks />
        [XmlEnumAttribute("delete-nodes")] DeleteNodes,

        /// <remarks />
        [XmlEnumAttribute("filtered-notifications")] FilteredNotifications,

        /// <remarks />
        [XmlEnumAttribute("get-pending")] GetPending,

        /// <remarks />
        [XmlEnumAttribute("instant-nodes")] InstantNodes,

        /// <remarks />
        [XmlEnumAttribute("item-ids")] ItemIds,

        /// <remarks />
        [XmlEnumAttribute("last-published")] LastPublished,

        /// <remarks />
        [XmlEnumAttribute("leased-subscription")] LeasedSubscription,

        /// <remarks />
        [XmlEnumAttribute("manage-subscriptions")] ManageSubscriptions,

        /// <remarks />
        [XmlEnumAttribute("member-affiliation")] MemberAffiliation,

        /// <remarks />
        [XmlEnumAttribute("meta-data")] MetaData,

        /// <remarks />
        [XmlEnumAttribute("modify-affiliations")] ModifyAffiliations,

        /// <remarks />
        [XmlEnumAttribute("multi-collection")] MultiCollection,

        /// <remarks />
        [XmlEnumAttribute("multi-subscribe")] MultiSubscribe,

        /// <remarks />
        [XmlEnumAttribute("outcast-affiliation")] OutcastAffiliation,

        /// <remarks />
        [XmlEnumAttribute("persistent-items")] PersistentItems,

        /// <remarks />
        [XmlEnumAttribute("presence-notifications")] PresenceNotifications,

        /// <remarks />
        [XmlEnumAttribute("presence-subscribe")] PresenceSubscribe,

        /// <remarks />
        [XmlEnumAttribute("publish")] Publish,

        /// <remarks />
        [XmlEnumAttribute("publish-options")] PublishOptions,

        /// <remarks />
        [XmlEnumAttribute("publisher-affiliation")] PublisherAffiliation,

        /// <remarks />
        [XmlEnumAttribute("purge-nodes")] PurgeNodes,

        /// <remarks />
        [XmlEnumAttribute("retract-items")] RetractItems,

        /// <remarks />
        [XmlEnumAttribute("retrieve-affiliations")] RetrieveAffiliations,

        /// <remarks />
        [XmlEnumAttribute("retrieve-default")] RetrieveDefault,

        /// <remarks />
        [XmlEnumAttribute("retrieve-items")] RetrieveItems,

        /// <remarks />
        [XmlEnumAttribute("retrieve-subscriptions")] RetrieveSubscriptions,

        /// <remarks />
        [XmlEnumAttribute("subscribe")] Subscribe,

        /// <remarks />
        [XmlEnumAttribute("subscription-options")] SubscriptionOptions,

        /// <remarks />
        [XmlEnumAttribute("subscription-notifications")] SubscriptionNotifications,
    }
}