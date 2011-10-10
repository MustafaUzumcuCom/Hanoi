using System;
using System.Xml.Serialization;

namespace BabelIm.Net.Xmpp.Serialization {
    /// <remarks />
    [Serializable]
    public enum ItemChoiceType {
        /// <remarks />
        [XmlEnum("optional")] Optional,

        /// <remarks />
        [XmlEnum("required")] Required,
    }
}