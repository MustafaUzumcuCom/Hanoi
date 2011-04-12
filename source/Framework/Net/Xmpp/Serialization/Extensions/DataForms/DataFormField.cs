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

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BabelIm.Net.Xmpp.Serialization.Extensions.DataForms 
{                  
    /// <remarks/>
    [Serializable]
    [XmlTypeAttribute(Namespace="jabber:x:data")]
	[XmlRootAttribute("field", Namespace="jabber:x:data", IsNullable=false)]
	public class DataFormField 
	{
		#region · Fields ·

		private string				    description;
		private bool				    required;        
		private List<string>		    values;
		private List<DataFormOption>    options;
		private string				    label;
		private DataFormFieldType	    type;
		private string				    var;

		#endregion

		#region · Properties ·

		[XmlElementAttribute("desc")]
		public string Description
		{
			get { return this.description; }
			set { this.description = value; }
		}
        
		[XmlElementAttribute("required")]
		public bool Required
		{
			get { return this.required; }
			set { this.required	= value; }
		}
                
		/// <remarks/>
		[XmlArrayItemAttribute("value")]
		public List<string> Values
		{
			get 
			{
				if (this.values == null)
				{
					this.values = new List<string>();
				}

				return this.values; 
			}
		}
        
		/// <remarks/>
		[XmlArrayItemAttribute("option")]
		public List<DataFormOption> Options
		{
			get 
			{
				if (this.options == null)
				{
                    this.options = new List<DataFormOption>();
				}

				return this.options; 
			}
		}
        
		/// <remarks/>
		[XmlAttributeAttribute("label")]
		public string Label
		{
			get { return this.label; }
			set { this.label = value; }
		}
        
		/// <remarks/>
		[XmlAttributeAttribute("type")]
		public DataFormFieldType Type
		{
			get { return this.type; }
			set { this.type = value; }
		}
        
		/// <remarks/>
		[XmlAttributeAttribute("var")]
		public string Var
		{
			get { return this.var; }
			set { this.var = value; }
		}

		#endregion

		#region · Constructors ·

		public DataFormField()
		{
			this.type = DataFormFieldType.TextSingle;
		}

		#endregion        
    }   
}
