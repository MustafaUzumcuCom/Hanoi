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
    EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED2 TO,
    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

namespace BabelIm.Net.Xmpp.InstantMessaging {
    /// <summary>
    ///   XMPP Features as described in XMPP Registrar (disco-features.xml)
    /// </summary>
    public static class XmppFeatures {
        /// <summary>
        ///   RFC 3920: XMPP Core, RFC 3921: XMPP IM. Support for DNS SRV lookups of XMPP services.
        /// </summary>
        public const string DnsSrvLookups = "dnssrv";

        /// <summary>
        ///   Support for Unicode characters, including in displayed text, JIDs, and passwords.
        /// </summary>
        public const string FullUnicode = "fullunicode";

        /// <summary>
        ///   XEP-0108: User Activity
        /// </summary>
        public const string UserActivity = "http://jabber.org/protocol/activity";

        /// <summary>
        ///   XEP-0033: Extended Stanza Addressing
        /// </summary>
        public const string ExtendedStanzaAddressing = "http://jabber.org/protocol/address";

        /// <summary>
        ///   XEP-0079: Advanced Message Processing
        /// </summary>
        public const string AdvancedMessageProcessing = "http://jabber.org/protocol/amp";

        /// <summary>
        ///   XEP-0065: SOCKS5 Bytestreams
        /// </summary>
        public const string SOCKS5Bytestreams = "http://jabber.org/protocol/bytestreams";

        /// <summary>
        ///   XEP-0115: Entity Capabilities
        /// </summary>
        public const string EntityCapabilities = "http://jabber.org/protocol/caps";

        /// <summary>
        ///   XEP-0085: Chat State Notifications
        /// </summary>
        public const string ChatStateNotifications = "http://jabber.org/protocol/chatstates";

        /// <summary>
        ///   XEP-0050: Ad-Hoc Commands
        /// </summary>
        public const string AdHocCommands = "http://jabber.org/protocol/commands";

        /// <summary>
        ///   XEP-0138: Stream Compression
        /// </summary>
        public const string StreamCompression = "http://jabber.org/protocol/compress";

        /// <summary>
        ///   XEP-0030: Service Discovery
        /// </summary>
        public const string ServiceDiscoveryInfo = "http://jabber.org/protocol/disco#info";

        /// <summary>
        ///   XEP-0030: Service Discovery
        /// </summary>
        public const string ServiceDiscoveryItems = "http://jabber.org/protocol/disco#items";

        /// <summary>
        ///   XEP-0020: Feature Negotiation
        /// </summary>
        public const string FeatureNegotiation = "http://jabber.org/protocol/feature-neg";

        /// <summary>
        ///   XEP-0080: User Geolocation
        /// </summary>
        public const string UserGeolocation = "http://jabber.org/protocol/geoloc";

        /// <summary>
        ///   XEP-0072: SOAP Over XMPP
        /// </summary>
        public const string SoapOverXmppHttpAuth = "http://jabber.org/protocol/http-auth";

        /// <summary>
        ///   XEP-0124: Bidirectional-streams Over Synchronous HTTP
        /// </summary>
        public const string BidirectionalStreamsOverSynchronousHTTP = "http://jabber.org/protocol/httpbind";

        /// <summary>
        ///   XEP-0047: In-Band Bytestreams
        /// </summary>
        public const string InBandBytestreams = "http://jabber.org/protocol/ibb";

        /// <summary>
        ///   XEP-0107: User Mood
        /// </summary>
        public const string UserMood = "http://jabber.org/protocol/mood";

        /// <summary>
        ///   XEP-0107: User Mood
        /// </summary>
        public const string UserMoodWithNotify = "http://jabber.org/protocol/mood+notify";

        /// <summary>
        ///   XEP-0118: User Tune
        /// </summary>
        public const string UserTune = "http://jabber.org/protocol/tune";

        /// <summary>
        ///   XEP-0118: User Tune
        /// </summary>
        public const string UserTuneWithNotify = "http://jabber.org/protocol/tune+notify";

        /// <summary>
        ///   XEP-0045: Multi-User Chat
        /// </summary>
        public const string MultiUserChat = "http://jabber.org/protocol/muc";

        /// <summary>
        ///   XEP-0045: Multi-User Chat
        /// </summary>
        public const string MultiUserChatAdmin = "http://jabber.org/protocol/muc#admin";

        /// <summary>
        ///   XEP-0045: Multi-User Chat
        /// </summary>
        public const string MultiUserChatOwner = "http://jabber.org/protocol/muc#owner";

        /// <summary>
        ///   XEP-0045: Multi-User Chat. Support for the muc#register FORM_TYPE in Multi-User Chat.
        /// </summary>
        public const string MultiUserChatRegister = "http://jabber.org/protocol/muc#register";

        /// <summary>
        ///   XEP-0045: Multi-User Chat
        /// </summary>
        public const string MultiUserChatRoomConfig = "http://jabber.org/protocol/muc#roomconfig";

        /// <summary>
        ///   XEP-0045: Multi-User Chat
        /// </summary>
        public const string MultiUserChatRoomInfo = "http://jabber.org/protocol/muc#roominfo";

        /// <summary>
        ///   XEP-0045: Multi-User Chat
        /// </summary>
        public const string MultiUserChatUser = "http://jabber.org/protocol/muc#user";

        /// <summary>
        ///   XEP-0013: Flexible Offline Message Retrieval
        /// </summary>
        public const string FlexibleOfflineMessageRetrieval = "http://jabber.org/protocol/offline";

        /// <summary>
        ///   XEP-0144: Roster Item Exchange
        /// </summary>
        public const string RosterItemExchange = "http://jabber.org/protocol/rosterx";

        /// <summary>
        ///   XEP-0137: Publishing SI Requests
        /// </summary>
        public const string PublishingSIRequests = "http://jabber.org/protocol/sipub";

        /// <summary>
        ///   XEP-0072: SOAP Over XMPP
        /// </summary>
        public const string SoapOverXmpp = "http://jabber.org/protocol/soap";

        /// <summary>
        ///   XEP-0072: SOAP Over XMPP
        /// </summary>
        public const string SOAPOverXmppFaults = "http://jabber.org/protocol/soap#fault";

        /// <summary>
        ///   XEP-0130: Waiting Lists
        /// </summary>
        public const string WaitingLists = "http://jabber.org/protocol/waitinglist";

        /// <summary>
        ///   XEP-0130: Waiting Lists
        /// </summary>
        public const string WaitingListsMailTo = "http://jabber.org/protocol/waitinglist/schemes/mailto";

        /// <summary>
        ///   XEP-0130: Waiting Lists
        /// </summary>
        public const string WaitingListsTel = "http://jabber.org/protocol/waitinglist/schemes/tel";

        /// <summary>
        ///   XEP-0071: XHTML-IM
        /// </summary>
        public const string XHTMLIM = "http://jabber.org/protocol/xhtml-im";

        /// <summary>
        ///   XEP-0141: Data Forms Layout
        /// </summary>
        public const string DataFormsLayout = "http://jabber.org/protocol/xdata-layout";

        /// <summary>
        ///   XEP-0122: Data Forms Validation
        /// </summary>
        public const string DataFormsValidation = "http://jabber.org/protocol/xdata-validate";

        /// <summary>
        ///   N/A
        /// </summary>
        public const string IPv6 = "ipv6";

        /// <summary>
        ///   RFC 3921: XMPP IM
        /// </summary>
        public const string XmppImClient = "jabber:client";

        /// <summary>
        ///   XEP-0114: Existing Component Protocol
        /// </summary>
        public const string ExistingComponentProtocolAccept = "jabber:component:accept";

        /// <summary>
        ///   XEP-0114: Existing Component Protocol
        /// </summary>
        public const string ExistingComponentProtocolConnect = "jabber:component:connect";

        /// <summary>
        ///   XEP-0078: Non-SASL Authentication
        /// </summary>
        public const string NonSaslAuthentication = "jabber:iq:auth";

        /// <summary>
        ///   XEP-0100: Gateway Interaction
        /// </summary>
        public const string GatewayInteraction = "jabber:iq:gateway";

        /// <summary>
        ///   XEP-0012: Last Activity
        /// </summary>
        public const string LastActivity = "jabber:iq:last";

        /// <summary>
        ///   XEP-0066: Out of Band Data
        /// </summary>
        public const string OutOfBandDataBasic = "jabber:iq:oob";

        /// <summary>
        ///   RFC 3921: XMPP IM
        /// </summary>
        public const string XmppImPrivacy = "jabber:iq:privacy";

        /// <summary>
        ///   XEP-0049: Private XML Storage
        /// </summary>
        public const string PrivateXMLStorage = "jabber:iq:private";

        /// <summary>
        ///   XEP-0077: In-Band Registration
        /// </summary>
        public const string InbandRegistration = "jabber:iq:register";

        /// <summary>
        ///   RFC 3921: XMPP IM
        /// </summary>
        public const string XmppImRoster = "jabber:iq:roster";

        /// <summary>
        ///   XEP-0009: Jabber-RPC
        /// </summary>
        public const string JabberRpc = "jabber:iq:rpc";

        /// <summary>
        ///   XEP-0055: Jabber Search
        /// </summary>
        public const string JabberSearch = "jabber:iq:search";

        /// <summary>
        ///   XEP-0092: Software Version
        /// </summary>
        public const string SoftwareVersion = "jabber:iq:version";

        /// <summary>
        ///   RFC 3921: XMPP IM
        /// </summary>
        public const string XmppImServer = "jabber:server";

        /// <summary>
        ///   XEP-0004: Data Forms
        /// </summary>
        public const string DataForms = "jabber:x:data";

        /// <summary>
        ///   XEP-0027: Current OpenPGP Usage
        /// </summary>
        public const string CurrentOpenPgpUsageEncrypted = "jabber:x:encrypted";

        /// <summary>
        ///   XEP-0066: Out of Band Data
        /// </summary>
        public const string OutOfBandDataExtended = "jabber:x:oob";

        /// <summary>
        ///   XEP-0027: Current OpenPGP Usage
        /// </summary>
        public const string CurrentOpenPgpUsageSigned = "jabber:x:signed";

        /// <summary>
        ///   Application performs logging or archiving of messages.
        /// </summary>
        public const string MessageLogging = "msglog";

        /// <summary>
        ///   XEP-0160: Best Practices for Handling Offline Messages
        /// </summary>
        public const string BestPracticesForHandlingOfflineMessages = "msgoffline";

        /// <summary>
        ///   XEP-0083: Nested Roster Groups
        /// </summary>
        public const string NestedRosterGroups = "roster:delimiter";

        /// <summary>
        ///   Application supports old-style (pre-TLS) SSL connections on a dedicated port.
        /// </summary>
        public const string PreTlsConnections = "sslc2s";

        /// <summary>
        ///   RFC 3920: XMPP Core. Application supports the nameprep, nodeprep, and resourceprep profiles of stringprep.
        /// </summary>
        public const string XmppCoreStringprep = "stringprep";

        /// <summary>
        ///   RFC 3920: XMPP Core
        /// </summary>
        public const string XmppCoreBind = "urn:ietf:params:xml:ns:xmpp-bind";

        /// <summary>
        ///   RFC 3923: XMPP E2E
        /// </summary>
        public const string XmppE2E = "urn:ietf:params:xml:ns:xmpp-e2e";

        /// <summary>
        ///   RFC 3920: XMPP Core
        /// </summary>
        public const string XmppCoreSasl = "urn:ietf:params:xml:ns:xmpp-sasl";

        /// <summary>
        ///   RFC 3920: XMPP Core. Application supports client-to-server SASL.
        /// </summary>
        public const string XmppCoreSaslC2S = "urn:ietf:params:xml:ns:xmpp-sasl#c2s";

        /// <summary>
        ///   RFC 3920: XMPP Core. Application supports server-to-server SASL.
        /// </summary>
        public const string XmppCoreSaslS2S = "urn:ietf:params:xml:ns:xmpp-sasl#s2s";

        /// <summary>
        ///   RFC 3921: XMPP IM
        /// </summary>
        public const string XmppImSession = "urn:ietf:params:xml:ns:xmpp-session";

        /// <summary>
        ///   RFC 3920: XMPP Core
        /// </summary>
        public const string XmppCoreStanzas = "urn:ietf:params:xml:ns:xmpp-stanzas";

        /// <summary>
        ///   RFC 3920: XMPP Core
        /// </summary>
        public const string XmppCoreStreams = "urn:ietf:params:xml:ns:xmpp-streams";

        /// <summary>
        ///   RFC 3920: XMPP Core
        /// </summary>
        public const string XmppCoreTls = "urn:ietf:params:xml:ns:xmpp-tls";

        /// <summary>
        ///   RFC 3920: XMPP Core. Application supports client-to-server TLS.
        /// </summary>
        public const string XmppCoreTlsC2S = "urn:ietf:params:xml:ns:xmpp-tls#c2s";

        /// <summary>
        ///   RFC 3920: XMPP Core. Application supports server-to-server TLS.
        /// </summary>
        public const string XmppCoreTlsS2S = "urn:ietf:params:xml:ns:xmpp-tls#s2s";

        /// <summary>
        ///   XEP-0136: Message Archiving
        /// </summary>
        public const string MessageArchiving = "urn:xmpp:archive:auto";

        /// <summary>
        ///   XEP-0136: Message Archiving
        /// </summary>
        public const string MessageArchivingManage = "urn:xmpp:archive:manage";

        /// <summary>
        ///   XEP-0136: Message Archiving
        /// </summary>
        public const string MessageArchivingManual = "urn:xmpp:archive:manual";

        /// <summary>
        ///   XEP-0136: Message Archiving
        /// </summary>
        public const string MessageArchivingPreferences = "urn:xmpp:archive:pref";

        /// <summary>
        ///   XEP-0084: User Avatars
        /// </summary>
        public const string UserAvatars = "urn:xmpp:avatar:data";

        /// <summary>
        ///   XEP-0084: User Avatars
        /// </summary>
        public const string UserAvatarsMetadata = "urn:xmpp:avatar:metadata";

        /// <summary>
        ///   XEP-0203: Delayed Delivery
        /// </summary>
        public const string DelayedDelivery = "urn:xmpp:delay";

        /// <summary>
        ///   XEP-0199: XMPP Ping
        /// </summary>
        public const string XmppPing = "urn:xmpp:ping";

        /// <summary>
        ///   XEP-0199: XMPP Ping
        /// </summary>
        public const string XmppPingReceipts = "urn:xmpp:receipts";

        /// <summary>
        ///   XEP-0155: Stanza Session Negotiation
        /// </summary>
        public const string StanzaSessionNegotiation = "urn:xmpp:ssn";

        /// <summary>
        ///   XEP-0202: Entity Time
        /// </summary>
        public const string EntityTime = "urn:xmpp:time";

        /// <summary>
        ///   RFC 3920: XMPP Core. Application supports the 'xml:lang' attribute as described in RFC 3920.
        /// </summary>
        public const string XMPPCore = "xmllang";

        /// <summary>
        ///   See XEP-0054
        /// </summary>
        public const string VCardTemp = "vcard-temp";

        /// <summary>
        ///   XEP-0191: Simple Communications Blocking
        /// </summary>
        public const string SimpleCommunicationsBlocking = "urn:xmpp:blocking";
    }
}