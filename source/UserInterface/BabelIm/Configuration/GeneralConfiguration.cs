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
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BabelIm.Configuration
{
    [XmlType(TypeName = "general"), Serializable]
    public sealed class GeneralConfiguration
    {
        #region · Fields ·

        private int             inactiveafter;
        private bool			userTuneEnabled;
        private Capabilities    capabilities;

        #endregion

        #region · Properties ·

        [XmlElement(ElementName = "usertune", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool UserTuneEnabled
        {
        	get { return this.userTuneEnabled; }
        	set { this.userTuneEnabled = value; }
        }
        
        [XmlElement(ElementName = "inactiveafter", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public int Inactiveafter
        {
            get { return this.inactiveafter; }
            set { this.inactiveafter = value; }
        }

        [XmlElement(Type = typeof(Capabilities), ElementName = "capabilities", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Capabilities Capabilities
        {
            get
            {
                if (this.capabilities == null)
                {
                    this.capabilities = new Capabilities();
                }

                return this.capabilities;
            }
            set { this.capabilities = value; }
        }

        #endregion

        #region · Constructors ·

        public GeneralConfiguration()
        {
        }

        #endregion
    }
}
