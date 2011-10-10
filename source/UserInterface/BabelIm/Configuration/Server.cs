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

namespace BabelIm.Configuration {
    [XmlType(TypeName = "server"), Serializable]
    public sealed class Server {
        private string domainName;
        private string name;
        private int portNumber;
        private string proxyPassword;
        private int proxyPortNumber;
        private string proxyServer;
        private string proxyType;
        private string proxyUserName;
        private bool resolveHostName;
        private string serverName;
        private int timeout;
        private bool useHttpBinding;
        private bool useProxy;

        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.Unqualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string Name {
            get { return name; }
            set { name = value; }
        }

        [XmlElement(ElementName = "server", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string ServerName {
            get { return serverName; }
            set { serverName = value; }
        }

        [XmlElement(ElementName = "domain", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string DomainName {
            get { return domainName; }
            set { domainName = value; }
        }

        [XmlElement(ElementName = "portnumber", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public int PortNumber {
            get { return portNumber; }
            set { portNumber = value; }
        }

        [XmlElement(ElementName = "timeout", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public int Timeout {
            get { return timeout; }
            set { timeout = value; }
        }

        [XmlElement(ElementName = "useproxy", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool UseProxy {
            get { return useProxy; }
            set { useProxy = value; }
        }

        [XmlElement(ElementName = "proxytype", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string ProxyType {
            get { return proxyType; }
            set { proxyType = value; }
        }

        [XmlElement(ElementName = "proxyserver", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string ProxyServer {
            get { return proxyServer; }
            set { proxyServer = value; }
        }

        [XmlElement(ElementName = "proxyportnumber", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public int ProxyPortNumber {
            get { return proxyPortNumber; }
            set { proxyPortNumber = value; }
        }

        [XmlElement(ElementName = "proxyusername", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string ProxyUserName {
            get { return proxyUserName; }
            set { proxyUserName = value; }
        }

        [XmlElement(ElementName = "proxypassword", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string ProxyPassword {
            get { return proxyPassword; }
            set { proxyPassword = value; }
        }

        [XmlElement(ElementName = "resolvehostname", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool ResolveHostName {
            get { return resolveHostName; }
            set { resolveHostName = value; }
        }

        [XmlElement(ElementName = "usehttpbinding", IsNullable = false, Form = XmlSchemaForm.Qualified)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool UseHttpBinding {
            get { return useHttpBinding; }
            set { useHttpBinding = value; }
        }
    }
}