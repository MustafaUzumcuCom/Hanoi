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
    [XmlType(TypeName = "server"), Serializable]
    public sealed class Server
    {
        #region · Fields ·

        private string  name;
        private string  serverName;
        private string  domainName;
        private int     portNumber;
        private int     timeout;
        private bool    useProxy;
        private string  proxyType;
        private string  proxyServer;
        private int     proxyPortNumber;
        private string  proxyUserName;
        private string  proxyPassword;
        private bool    resolveHostName;
        private bool    useHttpBinding;

        #endregion

        #region · Properties ·

        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.Unqualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        [XmlElement(ElementName = "server", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string ServerName
        {
            get { return this.serverName; }
            set { this.serverName = value; }
        }

        [XmlElement(ElementName = "domain", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string DomainName
        {
            get { return this.domainName; }
            set { this.domainName = value; }
        }

        [XmlElement(ElementName = "portnumber", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public int PortNumber
        {
            get { return this.portNumber; }
            set { this.portNumber = value; }
        }

        [XmlElement(ElementName = "timeout", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public int Timeout
        {
            get { return this.timeout; }
            set { this.timeout = value; }
        }

        [XmlElement(ElementName = "useproxy", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool UseProxy
        {
            get { return this.useProxy; }
            set { this.useProxy = value; }
        }

        [XmlElement(ElementName = "proxytype", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string ProxyType
        {
            get { return this.proxyType; }
            set { this.proxyType = value; }
        }

        [XmlElement(ElementName = "proxyserver", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string ProxyServer
        {
            get { return this.proxyServer; }
            set { this.proxyServer = value; }
        }

        [XmlElement(ElementName = "proxyportnumber", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public int ProxyPortNumber
        {
            get { return this.proxyPortNumber; }
            set { this.proxyPortNumber = value; }
        }

        [XmlElement(ElementName = "proxyusername", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string ProxyUserName
        {
            get { return this.proxyUserName; }
            set { this.proxyUserName = value; }
        }

        [XmlElement(ElementName = "proxypassword", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string ProxyPassword
        {
            get { return this.proxyPassword; }
            set { this.proxyPassword = value; }
        }

        [XmlElement(ElementName = "resolvehostname", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool ResolveHostName
        {
            get { return this.resolveHostName; }
            set { this.resolveHostName = value; }
        }

        [XmlElement(ElementName = "usehttpbinding", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool UseHttpBinding
        {
            get { return this.useHttpBinding; }
            set { this.useHttpBinding = value; }
        }

        #endregion

        #region · Constructors ·

        public Server()
        {
        }

        #endregion
    }
}