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
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BabelIm.Net.Xmpp.Core
{
    /// <summary>
    /// Represents a Connection String
    /// </summary>
    public sealed class XmppConnectionString
    {
        #region · Static Fields ·

        /// <summary>
        /// Synonyms list
        /// </summary>
        static Dictionary<string, string> Synonyms = GetSynonyms();

        #endregion

        #region · Static Methods ·

        /// <summary>
        /// Gets a value indicating whether the given key value is a synonym
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsSynonym(string key)
        {
            return Synonyms.ContainsKey(key);
        }

        /// <summary>
        /// Gets the synonym for the give key value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSynonym(string key)
        {
            return Synonyms[key];
        }

        /// <summary>
        /// Gets the synonyms.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, string> GetSynonyms()
        {
            Dictionary<string, string> synonyms = new Dictionary<string, string>();

            synonyms.Add("server",              "server");
            synonyms.Add("port number",         "port number");
            synonyms.Add("user id",             "user id");
            synonyms.Add("uid",                 "user id");
            synonyms.Add("user password",       "user password");
            synonyms.Add("resource",            "resource");
            synonyms.Add("connection timeout",  "connection timeout");
            synonyms.Add("use proxy",           "use proxy");
            synonyms.Add("proxy type",          "proxy type");
            synonyms.Add("proxy server",        "proxy server");
            synonyms.Add("proxy port number",   "proxy port number");
            synonyms.Add("proxy user name",     "proxy user name");
            synonyms.Add("proxy password",      "proxy password");
            synonyms.Add("http binding",        "http binding");
            synonyms.Add("resolve host name",   "resolve host name");

            return synonyms;
        }

        #endregion

        #region · Fields ·

        private Dictionary<string, object> options;

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the login server.
        /// </summary>
        /// <value>The server.</value>
        public string HostName
        {
            get { return this.GetString("server"); }
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
        }

        /// <summary>
        /// Gets the port number.
        /// </summary>
        /// <value>The port.</value>
        public int PortNumber
        {
            get { return this.GetInt32("port number"); }
        }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        /// <value>The user id.</value>
        public string UserId
        {
            get { return this.GetString("user id"); }
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>The password.</value>
        public string UserPassword
        {
            get { return this.GetString("user password"); }
        }

        /// <summary>
        /// Gets the connection timeout.
        /// </summary>
        /// <value>The connection timeout.</value>
        public int ConnectionTimeout
        {
            get { return this.GetInt32("connection timeout"); }
        }

        /// <summary>
        /// Gets a value that indicating whether the connection should be done throught a proxy
        /// </summary>
        public bool UseProxy
        {
            get { return this.GetBoolean("use proxy"); }
        }

        /// <summary>
        /// Gets the proxy type
        /// </summary>
        public string ProxyType
        {
            get { return this.GetString("proxy type"); }
        }

        /// <summary>
        /// Gets the proxy server
        /// </summary>
        public string ProxyServer
        {
            get { return this.GetString("proxy server"); }
        }

        /// <summary>
        /// Gets the proxy port number
        /// </summary>
        public int ProxyPortNumber
        {
            get { return this.GetInt32("proxy port number"); }
        }

        /// <summary>
        /// Gets the proxy user name
        /// </summary>
        public string ProxyUserName
        {
            get { return this.GetString("proxy user name"); }
        }

        /// <summary>
        /// Gets the proxy password
        /// </summary>
        public string ProxyPassword
        {
            get { return this.GetString("proxy password"); }
        }

        /// <summary>
        /// Gets a value that indicates if http binding should be used
        /// </summary>
        public bool UseHttpBinding
        {
            get { return this.GetBoolean("http binding"); }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppConnectionString"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public XmppConnectionString(string connectionString)
        {
            this.options = new Dictionary<string, object>();
            this.Load(connectionString);
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
            Dictionary<string, object>.Enumerator e = this.options.GetEnumerator();

            while (e.MoveNext())
            {
                if (e.Current.Value != null)
                {
                    if (cs.Length > 0)
                    {
                        cs.Append(";");
                    }

                    string key = CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(e.Current.Key.ToString());
                    cs.AppendFormat(CultureInfo.CurrentUICulture, "{0}={1}", key, e.Current.Value);
                }
            }

            return cs.ToString();
        }

        #endregion

        #region · Private Methods ·

        private void SetDefaultOptions()
        {
            this.options.Clear();

            this.options.Add("login server",        null);
            this.options.Add("port number",         5222);
            this.options.Add("user id",             null);
            this.options.Add("user password",       null);
            this.options.Add("resource",            null);
            this.options.Add("sasl",                false);
            this.options.Add("connection timeout",  -1);
        }

        private void Load(string connectionString)
        {
            string[] keyPairs = connectionString.Split(';');

            this.SetDefaultOptions();

            foreach (string keyPair in keyPairs)
            {
                string[] values = keyPair.Split('=');

                if (values.Length == 2 &&
                    values[0] != null && values[0].Length > 0 &&
                    values[1] != null && values[1].Length > 0)
                {
                    values[0] = values[0].ToLower(CultureInfo.CurrentUICulture);

                    if (Synonyms.ContainsKey(values[0]))
                    {
                        string key = (string)Synonyms[values[0]];
                        this.options[key] = values[1].Trim();
                    }
                }
            }

            this.Validate();
        }

        private void Validate()
        {
            if ((this.HostName == null || this.HostName.Length == 0) ||
                (this.UserId == null || this.UserId.Length == 0) ||
                (this.UserPassword == null || this.UserPassword.Length == 0))
            {
                throw new XmppException("Invalid connection string options.");
            }
        }

        private string GetString(string key)
        {
            if (this.options.ContainsKey(key))
            {
                return (string)this.options[key];
            }
            
            return null;
        }

        private int GetInt32(string key)
        {
            if (this.options.ContainsKey(key))
            {
                return Convert.ToInt32(this.options[key], CultureInfo.CurrentUICulture);
            }
        
            return 0;
        }

        private bool GetBoolean(string key)
        {
            if (this.options.ContainsKey(key))
            {
                return Convert.ToBoolean(this.options[key], CultureInfo.CurrentUICulture);
            }

            return false;
        }

        #endregion
    }
}
