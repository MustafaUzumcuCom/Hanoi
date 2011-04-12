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
using System.Xml.Serialization;

namespace BabelIm.Net.Xmpp.Serialization.Extensions.MultiUserChat 
{
    /// <summary>
    /// XEP-0045: Multi-User Chat
    /// </summary>
    [Serializable]
    [XmlTypeAttribute(Namespace="http://jabber.org/protocol/muc")]
    [XmlRootAttribute("history", Namespace="http://jabber.org/protocol/muc", IsNullable=false)]
    public class MucHistory 
    {
    	#region · Fields ·

    	private int 		maxchars;
        private bool 		maxcharsSpecified;
        private int 		maxstanzas;
        private bool 		maxstanzasSpecified;
        private int 		seconds;
        private bool 		secondsSpecified;
        private DateTime 	since;
        private bool 		sinceSpecified;
        private string 		value;
    	
    	#endregion
    	
    	#region · Properties ·
    	
        /// <remarks/>
        [XmlAttributeAttribute("maxchars")]
        public int Maxchars
        {
        	get { return this.maxchars; }
        	set 
        	{
        		this.maxchars 			= value; 
        		this.maxcharsSpecified 	= true;
        	}
        }
        
        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool MaxcharsSpecified
        {
        	get { return this.maxcharsSpecified; }
        }
        
        /// <remarks/>
        [XmlAttributeAttribute("maxstanzas")]
        public int Maxstanzas
        {
        	get { return this.maxstanzas; }
        	set 
        	{
        		this.maxstanzas = value;
        		this.maxstanzasSpecified = true;
        	}
        }
        
        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool MaxstanzasSpecified
        {
        	get { return this.maxstanzasSpecified; }
        }
        
        /// <remarks/>
        [XmlAttributeAttribute("seconds")]
        public int Seconds
        {
        	get { return this.seconds; }
        	set 
        	{
        		this.seconds 			= value;
        		this.secondsSpecified 	= true;
        	}
        }
        
        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool SecondsSpecified
        {
        	get { return this.secondsSpecified; }
        }
        
        /// <remarks/>
        [XmlAttributeAttribute("since")]
        public DateTime Since
        {
        	get { return this.since; }
        	set 
        	{
        		this.since 			= value;
        		this.sinceSpecified = true;
        	}
        }
        
        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool SinceSpecified
        {
        	get { return this.sinceSpecified; }
        }
        
        /// <remarks/>
        [XmlTextAttribute()]
        public string Value
        {
        	get { return this.value; }
        	set { this.value = value; }
        }
        
        #endregion
        
        #region · Constructors ·
        
        public MucHistory()
        {
        }
        
        #endregion
    }
}
