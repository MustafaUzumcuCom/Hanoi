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

namespace Hanoi
{
    public sealed class ConnectionString
    {
        /// <summary>
        ///   Synonyms list
        /// </summary>
        private static readonly IDictionary<string, string> Synonyms = GetSynonyms();

        private readonly Dictionary<string, object> _options;

        public ConnectionString(string connectionString)
        {
            _options = new Dictionary<string, object>();
            Load(connectionString);
        }

        /// <summary>
        ///   Gets the login server.
        /// </summary>
        /// <value>The server.</value>
        public string HostName
        {
            get { return GetString("server"); }
        }

        /// <summary>
        ///   Gets a value indicating whether to resolve host names.
        /// </summary>
        /// <value>
        ///   <c>true</c> if host name should be resolved; otherwise, <c>false</c>.
        /// </value>
        public bool ResolveHostName
        {
            get { return GetBoolean("resolve host name"); }
        }

        /// <summary>
        ///   Gets the port number.
        /// </summary>
        /// <value>The port.</value>
        public int PortNumber
        {
            get { return GetInt32("port number"); }
        }

        /// <summary>
        ///   Gets the user id.
        /// </summary>
        /// <value>The user id.</value>
        public string UserId
        {
            get { return GetString("user id"); }
        }

        /// <summary>
        ///   Gets the password.
        /// </summary>
        /// <value>The password.</value>
        public string UserPassword
        {
            get { return GetString("user password"); }
        }

        /// <summary>
        ///   Gets the connection timeout.
        /// </summary>
        /// <value>The connection timeout.</value>
        public int ConnectionTimeout
        {
            get { return GetInt32("connection timeout"); }
        }

        /// <summary>
        ///   Gets a value that indicating whether the connection should be done throught a proxy
        /// </summary>
        public bool UseProxy
        {
            get { return GetBoolean("use proxy"); }
        }

        /// <summary>
        ///   Gets the proxy type
        /// </summary>
        public string ProxyType
        {
            get { return GetString("proxy type"); }
        }

        /// <summary>
        ///   Gets the proxy server
        /// </summary>
        public string ProxyServer
        {
            get { return GetString("proxy server"); }
        }

        /// <summary>
        ///   Gets the proxy port number
        /// </summary>
        public int ProxyPortNumber
        {
            get { return GetInt32("proxy port number"); }
        }

        /// <summary>
        ///   Gets the proxy user name
        /// </summary>
        public string ProxyUserName
        {
            get { return GetString("proxy user name"); }
        }

        /// <summary>
        ///   Gets the proxy password
        /// </summary>
        public string ProxyPassword
        {
            get { return GetString("proxy password"); }
        }

        /// <summary>
        ///   Gets a value that indicates if http binding should be used
        /// </summary>
        public bool UseHttpBinding
        {
            get { return GetBoolean("http binding"); }
        }

        /// <summary>
        ///   Gets a value indicating whether the given key value is a synonym
        /// </summary>
        /// <param name = "key"></param>
        /// <returns></returns>
        public static bool IsSynonym(string key)
        {
            return Synonyms.ContainsKey(key);
        }

        /// <summary>
        ///   Gets the synonym for the give key value
        /// </summary>
        /// <param name = "key"></param>
        /// <returns></returns>
        public static string GetSynonym(string key)
        {
            return Synonyms[key];
        }

        /// <summary>
        ///   Gets the synonyms.
        /// </summary>
        /// <returns></returns>
        private static IDictionary<string, string> GetSynonyms()
        {
            var synonyms = new Dictionary<string, string>();

            synonyms.Add("server", "server");
            synonyms.Add("port number", "port number");
            synonyms.Add("user id", "user id");
            synonyms.Add("uid", "user id");
            synonyms.Add("user password", "user password");
            synonyms.Add("resource", "resource");
            synonyms.Add("connection timeout", "connection timeout");
            synonyms.Add("use proxy", "use proxy");
            synonyms.Add("proxy type", "proxy type");
            synonyms.Add("proxy server", "proxy server");
            synonyms.Add("proxy port number", "proxy port number");
            synonyms.Add("proxy user name", "proxy user name");
            synonyms.Add("proxy password", "proxy password");
            synonyms.Add("http binding", "http binding");
            synonyms.Add("resolve host name", "resolve host name");

            return synonyms;
        }

        /// <summary>
        ///   Returns a <see cref = "T:System.String"></see> that represents the current <see cref = "T:System.Object"></see>.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.String"></see> that represents the current <see cref = "T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            var cs = new StringBuilder();
            var e = _options.GetEnumerator();

            while (e.MoveNext())
            {
                if (e.Current.Value == null) 
                    continue;

                if (cs.Length > 0)
                {
                    cs.Append(";");
                }

                var key = CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(e.Current.Key);
                cs.AppendFormat(CultureInfo.CurrentUICulture, "{0}={1}", key, e.Current.Value);
            }

            return cs.ToString();
        }

        private void SetDefaultOptions()
        {
            _options.Clear();

            _options.Add("login server", null);
            _options.Add("port number", 5222);
            _options.Add("user id", null);
            _options.Add("user password", null);
            _options.Add("resource", null);
            _options.Add("sasl", false);
            _options.Add("connection timeout", -1);
        }

        private void Load(string connectionString)
        {
            string[] keyPairs = connectionString.Split(';');

            SetDefaultOptions();

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
                        string key = Synonyms[values[0]];
                        _options[key] = values[1].Trim();
                    }
                }
            }

            Validate();
        }

        private void Validate()
        {
            return;
            if (string.IsNullOrEmpty(HostName) ||
                string.IsNullOrEmpty(UserId) ||
                string.IsNullOrEmpty(UserPassword))
            {
                throw new XmppException("Invalid connection string options.");
            }
        }

        private string GetString(string key)
        {
            if (_options.ContainsKey(key))
            {
                return (string)_options[key];
            }

            return null;
        }

        private int GetInt32(string key)
        {
            if (_options.ContainsKey(key))
            {
                return Convert.ToInt32(_options[key], CultureInfo.CurrentUICulture);
            }

            return 0;
        }

        private bool GetBoolean(string key)
        {
            if (_options.ContainsKey(key))
            {
                return Convert.ToBoolean(_options[key], CultureInfo.CurrentUICulture);
            }

            return false;
        }
    }
}