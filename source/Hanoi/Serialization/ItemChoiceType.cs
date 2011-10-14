using System;
using System.Xml.Serialization;

namespace Hanoi.Serialization {
    /// <remarks />
    [Serializable]
    public enum ItemChoiceType {
        /// <remarks />
        [XmlEnum("optional")] Optional,

        /// <remarks />
        [XmlEnum("required")] Required,
    }
}