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
using System.Xml.Serialization;

namespace Hanoi.Serialization.Core.Streams {
    /// <remarks />
    [Serializable]
    [XmlType(TypeName = "error", Namespace = "http://etherx.jabber.org/streams")]
    [XmlRootAttribute("error", Namespace = "http://etherx.jabber.org/streams", IsNullable = false)]
    public class StreamError {
        private string badformatField;
        private string badnamespaceprefixField;
        private string conflictField;
        private string connectiontimeoutField;
        private string hostgoneField;
        private string hostunknownField;
        private string improperaddressingField;
        private string internalservererrorField;
        private string invalidfromField;
        private string invalididField;
        private string invalidnamespaceField;
        private string invalidxmlField;
        private string notauthorizedField;
        private string policyviolationField;
        private string remoteconnectionfailedField;
        private string resourceconstraintField;
        private string restrictedxmlField;
        private string seeotherhostField;
        private string systemshutdownField;
        private StreamErrorText textField;
        private string undefinedconditionField;
        private string unsupportedencodingField;
        private string unsupportedstanzatypeField;
        private string unsupportedversionField;
        private string xmlnotwellformedField;

        /// <remarks />
        [XmlElementAttribute("bad-format", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)]
        public string BadFormat {
            get { return badformatField; }
            set { badformatField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("bad-namespace-prefix", Namespace = "urn:ietf:params:xml:ns:xmpp-streams",
            IsNullable = true)]
        public string BadNamespacePrefix {
            get { return badnamespaceprefixField; }
            set { badnamespaceprefixField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("conflict", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)]
        public string Conflict {
            get { return conflictField; }
            set { conflictField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("connection-timeout", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)
        ]
        public string ConnectionTimeout {
            get { return connectiontimeoutField; }
            set { connectiontimeoutField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("host-gone", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)]
        public string HostGone {
            get { return hostgoneField; }
            set { hostgoneField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("host-unknown", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)]
        public string HostUnknown {
            get { return hostunknownField; }
            set { hostunknownField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("improper-addressing", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true
            )]
        public string ImproperAddressing {
            get { return improperaddressingField; }
            set { improperaddressingField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("internal-server-error", Namespace = "urn:ietf:params:xml:ns:xmpp-streams",
            IsNullable = true)]
        public string InternalServerError {
            get { return internalservererrorField; }
            set { internalservererrorField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("invalid-from", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)]
        public string InvalidFrom {
            get { return invalidfromField; }
            set { invalidfromField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("invalid-id", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)]
        public string InvalidID {
            get { return invalididField; }
            set { invalididField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("invalid-namespace", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)]
        public string InvalidNamespace {
            get { return invalidnamespaceField; }
            set { invalidnamespaceField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("invalid-xml", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)]
        public string InvalidXml {
            get { return invalidxmlField; }
            set { invalidxmlField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("not-authorized", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)]
        public string NotAuthorized {
            get { return notauthorizedField; }
            set { notauthorizedField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("policy-violation", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)]
        public string PolicyViolation {
            get { return policyviolationField; }
            set { policyviolationField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("remote-connection-failed", Namespace = "urn:ietf:params:xml:ns:xmpp-streams",
            IsNullable = true)]
        public string RemoteConnectionFailed {
            get { return remoteconnectionfailedField; }
            set { remoteconnectionfailedField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("resource-constraint", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true
            )]
        public string ResourceConstraint {
            get { return resourceconstraintField; }
            set { resourceconstraintField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("restricted-xml", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)]
        public string RestrictedXml {
            get { return restrictedxmlField; }
            set { restrictedxmlField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("see-other-host", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)]
        public string SeeOtherHost {
            get { return seeotherhostField; }
            set { seeotherhostField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("system-shutdown", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)]
        public string SystemShutdown {
            get { return systemshutdownField; }
            set { systemshutdownField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("undefined-condition", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true
            )]
        public string UndefinedCondition {
            get { return undefinedconditionField; }
            set { undefinedconditionField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("unsupported-encoding", Namespace = "urn:ietf:params:xml:ns:xmpp-streams",
            IsNullable = true)]
        public string UnsupportedEncoding {
            get { return unsupportedencodingField; }
            set { unsupportedencodingField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("unsupported-stanza-type", Namespace = "urn:ietf:params:xml:ns:xmpp-streams",
            IsNullable = true)]
        public string UnsupportedStanzaType {
            get { return unsupportedstanzatypeField; }
            set { unsupportedstanzatypeField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("unsupported-version", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true
            )]
        public string UnsupportedVersion {
            get { return unsupportedversionField; }
            set { unsupportedversionField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("xml-not-well-formed", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true
            )]
        public string XmlNotWellFormed {
            get { return xmlnotwellformedField; }
            set { xmlnotwellformedField = value; }
        }

        /// <remarks />
        [XmlElementAttribute("text", Namespace = "urn:ietf:params:xml:ns:xmpp-streams", IsNullable = true)]
        public StreamErrorText Text {
            get { return textField; }
            set { textField = value; }
        }
    }
}