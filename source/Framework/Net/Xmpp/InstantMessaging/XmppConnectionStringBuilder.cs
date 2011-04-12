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
using System.Collections;
using System.Globalization;
using System.Text;
using BabelIm.Net.Xmpp.Core;

namespace BabelIm.Net.Xmpp.InstantMessaging
{
    /// <summary>
    /// XMPP Connection String Builder
    /// </summary>
    public sealed class XmppConnectionStringBuilder
    {
        #region · Fields ·

        private Hashtable options;

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get { return this.ToString(); }
            set { this.Load(value); }
        }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        public string HostName
        {
            get { return this.GetString("server"); }
            set { this.SetValue("server", value); }
        }

        /// <summary>
        /// Gets a value indicating whether to resolve host names.
        /// </summary>
        /// <value>
        ///   <c>true</c> if host name should be resolved; otherwise, <c>false</c>.
        /// </value>
        public bool ResolveHostName
        {
            get { return this.GetBoolean("resolve host name"); }
            set { this.SetValue("resolve host name", value); }
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port
        {
            get { return this.GetInt32("port number"); }
            set { this.SetValue("port number", value); }
        }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        /// <value>The user ID.</value>
        public string UserId
        {
            get { return this.GetString("user id"); }
            set { this.SetValue("user id", value); }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password
        {
            get { return this.GetString("user password"); }
            set { this.SetValue("user password", value); }
        }

        /// <summary>
        /// Gets or sets the connection timeout.
        /// </summary>
        /// <value>The connection timeout.</value>
        public int ConnectionTimeout
        {
            get { return this.GetInt32("connection timeout"); }
            set { this.SetValue("connection timeout", value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the connection should be done throught a proxy
        /// </summary>
        public bool UseProxy
        {
            get { return this.GetBoolean("use proxy"); }
            set { this.SetValue("use proxy", value); }
        }

        /// <summary>
        /// Gets or sets the proxy type
        /// </summary>
        public string ProxyType
        {
            get { return this.GetString("proxy type"); }
            set { this.SetValue("proxy type", value); }
        }

        /// <summary>
        /// Gets or sets the proxy server
        /// </summary>
        public string ProxyServer
        {
            get { return this.GetString("proxy server"); }
            set { this.SetValue("proxy server", value); }
        }

        /// <summary>
        /// Gets or sets the proxy port number
        /// </summary>
        public int ProxyPortNumber
        {
            get { return this.GetInt32("proxy port number"); }
            set { this.SetValue("proxy port number", value); }
        }

        /// <summary>
        /// Gets or sets the proxy user name
        /// </summary>
        public string ProxyUserName
        {
            get { return this.GetString("proxy user name"); }
            set { this.SetValue("proxy user name", value); }
        }

        /// <summary>
        /// Gets or sets the proxy password
        /// </summary>
        public string ProxyPassword
        {
            get { return this.GetString("proxy password"); }
            set { this.SetValue("proxy password", value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether http binding should be used
        /// </summary>
        public bool UseHttpBinding
        {
            get { return this.GetBoolean("http binding"); }
            set { this.SetValue("http binding", value); }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppConnectionStringBuilder"/> class.
        /// </summary>
        public XmppConnectionStringBuilder() 
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppConnectionStringBuilder"/> class with
        /// the given connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public XmppConnectionStringBuilder(string connectionString)
        {
            this.options = new Hashtable();

            if (connectionString != null)
            {
                this.Load(connectionString);
            }
        }

        #endregion

        #region · Overriden methods ·

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder cs = new StringBuilder();

            IDictionaryEnumerator e = this.options.GetEnumerator();

            while (e.MoveNext())
            {
                if (e.Value != null)
                {
                    if (cs.Length > 0)
                    {
                        cs.Append(";");
                    }

                    string key = CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(e.Key.ToString());
                    cs.AppendFormat(CultureInfo.CurrentUICulture, "{0}={1}", key, e.Value);
                }
            }

            return cs.ToString();
        }

        #endregion

        #region · Private Methods ·

        private void Load(string connectionString)
        {
            string[] keyPairs = connectionString.Split(';');

            if (this.options != null)
            {
                this.options.Clear();
            }

            foreach (string keyPair in keyPairs)
            {
                string[] values = keyPair.Split('=');

                if (values.Length == 2 &&
                    values[0] != null && values[0].Length > 0 &&
                    values[1] != null && values[1].Length > 0)
                {
                    values[0] = values[0].ToLower(CultureInfo.CurrentUICulture);

                    if (XmppConnectionString.IsSynonym(values[0]))
                    {
                        this.options[XmppConnectionString.GetSynonym(values[0])] = values[1].Trim();
                    }
                }
            }
        }

        private string GetString(string key)
        {
            if (this.options.Contains(key))
            {
                return (string)this.options[key];
            }
            
            return null;
        }

        private int GetInt32(string key)
        {
            if (this.options.Contains(key))
            {
                return Convert.ToInt32(this.options[key], CultureInfo.CurrentUICulture);
            }
            
            return 0;
        }

        private bool GetBoolean(string key)
        {
            if (this.options.Contains(key))
            {
                return Convert.ToBoolean(this.options[key], CultureInfo.CurrentUICulture);
            }

            return false;
        }

        private void SetValue(string key, object value)
        {
            if (this.options.Contains(key))
            {
                this.options[key] = value;
            }
            else
            {
                this.options.Add(key, value);
            }
        }

        #endregion
    }
}
