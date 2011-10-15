using System;

namespace Hanoi {
    public static class StreamCodes {
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