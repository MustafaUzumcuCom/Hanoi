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
    [XmlType(TypeName = "account"), Serializable]
    public sealed class Account
    {
        #region · Fields ·

        private string  name;
        private string  server;
        private Login   login;
        private string  nickname;
        private string  avatar;
        private string  status;
        private string  presence;
        private string  resource;

        #endregion

        #region · Properties ·

        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.Unqualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        [XmlAttribute(AttributeName = "server", Form = XmlSchemaForm.Unqualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string Server
        {
            get { return this.server; }
            set { this.server = value; }
        }

        [XmlElement(Type = typeof(Login), ElementName = "login", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Login Login
        {
            get
            {
                if (this.login == null)
                {
                    this.login = new Login();
                }
                return this.login;
            }
            set { this.login = value; }
        }

        [XmlElement(ElementName = "displayName", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string DisplayName
        {
            get { return this.nickname; }
            set { this.nickname = value; }
        }

        [XmlElement(ElementName = "avatar", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string Avatar
        {
            get { return this.avatar; }
            set { this.avatar = value; }
        }

        [XmlElement(ElementName = "presence", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string Presence
        {
            get { return this.presence; }
            set { this.presence = value; }
        }

        [XmlElement(ElementName = "status", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        [XmlElement(ElementName = "resource", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string Resource
        {
            get { return this.resource; }
            set { this.resource = value; }
        }

        #endregion

        #region · Constructors ·

        public Account()
        {
        }

        #endregion
    }
}
