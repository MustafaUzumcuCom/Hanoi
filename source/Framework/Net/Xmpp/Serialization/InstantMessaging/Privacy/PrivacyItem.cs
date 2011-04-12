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

namespace BabelIm.Net.Xmpp.Serialization.InstantMessaging.Privacy
{
	/// <remarks/>
	[Serializable]
	[XmlTypeAttribute(Namespace = "jabber:iq:privacy")]
	[XmlRootAttribute("item", Namespace = "jabber:iq:privacy", IsNullable = false)]
	public class PrivacyItem
	{
		#region · Fields ·

		private Empty iqField;
		private bool iqFieldSpecified;
		private Empty messageField;
		private bool messageFieldSpecified;
		private Empty presenceinField;
		private bool presenceinFieldSpecified;
		private Empty presenceoutField;
		private bool presenceoutFieldSpecified;
		private PrivacyActionType actionField;
		private int orderField;
		private PrivacyType typeField;
		private bool typeFieldSpecified;
		private string valueField;

		#endregion

		#region · Properties ·

		/// <remarks/>
		[XmlElementAttribute("iq")]
		public Empty IQ
		{
			get { return this.iqField; }
			set { this.iqField = value; }
		}

		/// <remarks/>
		[XmlIgnoreAttribute()]
		public bool IQSpecified
		{
			get { return this.iqFieldSpecified; }
			set { this.iqFieldSpecified = value; }
		}

		/// <remarks/>
		[XmlElement("message")]
		public Empty Message
		{
			get { return this.messageField; }
			set { this.messageField = value; }
		}

		/// <remarks/>
		[XmlIgnoreAttribute()]
		public bool messageSpecified
		{
			get { return this.messageFieldSpecified; }
			set { this.messageFieldSpecified = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("presence-in")]
		public Empty PresenceIn
		{
			get { return this.presenceinField; }
			set { this.presenceinField = value; }
		}

		/// <remarks/>
		[XmlIgnoreAttribute()]
		public bool PresenceinSpecified
		{
			get { return this.presenceinFieldSpecified; }
			set { this.presenceinFieldSpecified = value; }
		}

		/// <remarks/>
		[XmlElementAttribute("presence-out")]
		public Empty PresenceOut
		{
			get { return this.presenceoutField; }
			set { this.presenceoutField = value; }
		}

		/// <remarks/>
		[XmlIgnoreAttribute()]
		public bool PresenceOutSpecified
		{
			get { return this.presenceoutFieldSpecified; }
			set { this.presenceoutFieldSpecified = value; }
		}

		/// <remarks/>
		[XmlAttributeAttribute("action")]
		public PrivacyActionType Action
		{
			get { return this.actionField; }
			set { this.actionField = value; }
		}

		/// <remarks/>
		[XmlAttributeAttribute("order")]
		public int order
		{
			get { return this.orderField; }
			set { this.orderField = value; }
		}

		/// <remarks/>
		[XmlAttributeAttribute("type")]
		public PrivacyType Type
		{
			get { return this.typeField; }
			set { this.typeField = value; }
		}

		/// <remarks/>
		[XmlIgnoreAttribute()]
		public bool TypeSpecified
		{
			get { return this.typeFieldSpecified; }
			set { this.typeFieldSpecified = value; }
		}

		/// <remarks/>
		[XmlAttributeAttribute("value")]
		public string Value
		{
			get { return this.valueField; }
			set { this.valueField = value; }
		}

		#endregion

		#region · Constructors ·

		public PrivacyItem()
		{
		}

		#endregion
	}
}
