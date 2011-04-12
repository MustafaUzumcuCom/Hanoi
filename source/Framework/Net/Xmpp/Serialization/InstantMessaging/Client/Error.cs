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
using BabelIm.Net.Xmpp.Serialization;
using BabelIm.Net.Xmpp.Serialization.Core.Stanzas;

namespace BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client
{
	/// <remarks/>
	[Serializable]
	[XmlTypeAttribute(Namespace = "jabber:client")]
	[XmlRootAttribute("error", Namespace = "jabber:client", IsNullable = false)]
	public class Error
	{
		#region · Fields ·

		private string badrequestField;
		private string conflictField;
		private string featurenotimplementedField;
		private string forbiddenField;
		private string goneField;
		private string internalservererrorField;
		private string itemnotfoundField;
		private string jidmalformedField;
		private string notacceptableField;
		private string notallowedField;
		private string paymentrequiredField;
		private string recipientunavailableField;
		private string redirectField;
		private string registrationrequiredField;
		private string remoteservernotfoundField;
		private string remoteservertimeoutField;
		private string resourceconstraintField;
		private string serviceunavailableField;
		private string subscriptionrequiredField;
		private string undefinedconditionField;
		private string unexpectedrequestField;
		private StanzaText textField;
		private short codeField;
		private bool codeFieldSpecified;
		private ErrorType typeField;

		#endregion

		#region · Properties ·

		/// <remarks/>
		[XmlElementAttribute("bad-request", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string Badrequest
		{
			get { return this.badrequestField; }
			set { this.badrequestField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("conflict", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string Conflict
		{
			get { return this.conflictField; }
			set { this.conflictField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("feature-not-implemented", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string FeatureNotImplemented
		{
			get { return this.featurenotimplementedField; }
			set { this.featurenotimplementedField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("forbidden", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string Forbidden
		{
			get { return this.forbiddenField; }
			set { this.forbiddenField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("gone", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string Gone
		{
			get { return this.goneField; }
			set { this.goneField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("internal-server-error", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string InternalServerError
		{
			get { return this.internalservererrorField; }
			set { this.internalservererrorField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("item-not-found", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string ItemNotFound
		{
			get { return this.itemnotfoundField; }
			set { this.itemnotfoundField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("jid-malformed", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string JidMalformed
		{
			get { return this.jidmalformedField; }
			set { this.jidmalformedField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("not-acceptable", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string NotAcceptable
		{
			get { return this.notacceptableField; }
			set { this.notacceptableField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("not-allowed", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string NotAllowed
		{
			get { return this.notallowedField; }
			set { this.notallowedField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("payment-required", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string PaymentRequired
		{
			get { return this.paymentrequiredField; }
			set { this.paymentrequiredField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("recipient-unavailable", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string RecipientUnavailable
		{
			get { return this.recipientunavailableField; }
			set { this.recipientunavailableField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("redirect", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string Redirect
		{
			get { return this.redirectField; }
			set { this.redirectField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("registration-required", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string RegistrationRequired
		{
			get { return this.registrationrequiredField; }
			set { this.registrationrequiredField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("remote-server-not-found", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string RemoteServerNotFound
		{
			get { return this.remoteservernotfoundField; }
			set { this.remoteservernotfoundField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("remote-server-timeout", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string RemoteServerTimeout
		{
			get { return this.remoteservertimeoutField; }
			set { this.remoteservertimeoutField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("resource-constraint", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string resourceconstraint
		{
			get { return this.resourceconstraintField; }
			set { this.resourceconstraintField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("service-unavailable", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string ServiceUnavailable
		{
			get { return this.serviceunavailableField; }
			set { this.serviceunavailableField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("subscription-required", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string SubscriptionRequired
		{
			get { return this.subscriptionrequiredField; }
			set { this.subscriptionrequiredField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("undefined-condition", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string UndefinedCondition
		{
			get { return this.undefinedconditionField; }
			set { this.undefinedconditionField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("unexpected-request", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public string UnexpectedRequest
		{
			get { return this.unexpectedrequestField; }
			set { this.unexpectedrequestField = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("text", Namespace = "urn:ietf:params:xml:ns:xmpp-stanzas")]
		public StanzaText Text
		{
			get { return this.textField; }
			set { this.textField = value; }
		}

		/// <remarks/>
		[XmlAttributeAttribute("code")]
		public short Code
		{
			get { return this.codeField; }
			set { this.codeField = value; }
		}

		/// <remarks/>
		[XmlIgnoreAttribute()]
		public bool CodeSpecified
		{
			get { return this.codeFieldSpecified; }
			set { this.codeFieldSpecified = value; }
		}

		/// <remarks/>
		[XmlAttributeAttribute("type")]
		public ErrorType Type
		{
			get { return this.typeField; }
			set { this.typeField = value; }
		}

		#endregion

		#region · Constructors ·

		public Error()
		{
		}

		#endregion
	}
}
