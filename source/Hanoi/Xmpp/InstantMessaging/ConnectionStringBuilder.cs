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

namespace Hanoi.Xmpp.InstantMessaging
{
    public sealed class ConnectionStringBuilder
    {
        private readonly Hashtable _options = new Hashtable();
        public ConnectionStringBuilder()
            : this(null)
        {
        }

        public ConnectionStringBuilder(string connectionString)
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                Load(connectionString);
            }
        }

        public string ConnectionString
        {
            get { return ToString(); }
            set { Load(value); }
        }

        public string HostName
        {
            get { return GetString("server"); }
            set { SetValue("server", value); }
        }

        public bool ResolveHostName
        {
            get { return GetBoolean("resolve host name"); }
            set { SetValue("resolve host name", value); }
        }

        public int Port
        {
            get { return GetInt32("port number"); }
            set { SetValue("port number", value); }
        }

        public string UserId
        {
            get { return GetString("user id"); }
            set { SetValue("user id", value); }
        }

        public string Password
        {
            get { return GetString("user password"); }
            set { SetValue("user password", value); }
        }

        public int ConnectionTimeout
        {
            get { return GetInt32("connection timeout"); }
            set { SetValue("connection timeout", value); }
        }

        public bool UseProxy
        {
            get { return GetBoolean("use proxy"); }
            set { SetValue("use proxy", value); }
        }

        public string ProxyType
        {
            get { return GetString("proxy type"); }
            set { SetValue("proxy type", value); }
        }

        public string ProxyServer
        {
            get { return GetString("proxy server"); }
            set { SetValue("proxy server", value); }
        }

        public int ProxyPortNumber
        {
            get { return GetInt32("proxy port number"); }
            set { SetValue("proxy port number", value); }
        }

        public string ProxyUserName
        {
            get { return GetString("proxy user name"); }
            set { SetValue("proxy user name", value); }
        }

        public string ProxyPassword
        {
            get { return GetString("proxy password"); }
            set { SetValue("proxy password", value); }
        }


        public bool UseHttpBinding
        {
            get { return GetBoolean("http binding"); }
            set { SetValue("http binding", value); }
        }

        public override string ToString()
        {
            var cs = new StringBuilder();
            var e = _options.GetEnumerator();

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

        private void Load(string connectionString)
        {
            var keyPairs = connectionString.Split(';');

            _options.Clear();

            foreach (var keyPair in keyPairs)
            {
                var values = keyPair.Split('=');

                if (values.Length == 2 &&
                    values[0] != null && values[0].Length > 0 &&
                    values[1] != null && values[1].Length > 0)
                {
                    values[0] = values[0].ToLower(CultureInfo.CurrentUICulture);

                    if (Hanoi.ConnectionString.IsSynonym(values[0]))
                    {
                        _options[Hanoi.ConnectionString.GetSynonym(values[0])] = values[1].Trim();
                    }
                }
            }
        }

        private string GetString(string key)
        {
            if (_options.Contains(key))
            {
                return (string)_options[key];
            }

            return null;
        }

        private int GetInt32(string key)
        {
            return _options.Contains(key) ? Convert.ToInt32(_options[key], CultureInfo.CurrentCulture) : 0;
        }

        private bool GetBoolean(string key)
        {
            return _options.Contains(key) && Convert.ToBoolean(_options[key], CultureInfo.CurrentCulture);
        }

        private void SetValue(string key, object value)
        {
            if (_options.Contains(key))
            {
                _options[key] = value;
            }
            else
            {
                _options.Add(key, value);
            }
        }
    }
}