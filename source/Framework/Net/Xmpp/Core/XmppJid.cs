/*
    Copyright (c) 2007 - 2010, Carlos Guzm�n �lvarez

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
using System.Text;
using System.Text.RegularExpressions;
using Gnu.Inet.Encoding;

namespace BabelIm.Net.Xmpp.Core
{
    /// <summary>
    /// Represents a XMPP JID
    /// </summary>
    public sealed class XmppJid
    {
        #region � Static Members �

        /// <summary>
        /// Regex used to parse jid strings
        /// </summary>
        private static readonly Regex JidRegex = new Regex
        (
            @"(?:(?<userid>[^\@]{1,1023})\@)?" +        // optional node
            @"(?<domain>[a-zA-Z0-9\.\-]{1,1023})" +     // domain
            @"(?:/(?<resource>.{1,1023}))?",            // resource
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled
        );

        #endregion

        #region � Fields �

        private string userName;
        private string domainName;
        private string resourceName;
        private string bareJid;
        private string fullJid;

        #endregion

        #region � Properties �

        /// <summary>
        /// Gets the Bare JID
        /// </summary>
        public string BareIdentifier
        {
            get { return this.bareJid; }
        }

        /// <summary>
        /// Gets the User Name
        /// </summary>
        public string UserName
        {
            get { return this.userName; }
        }

        /// <summary>
        /// Gets the Domain Name
        /// </summary>
        public string DomainName
        {
            get { return this.domainName; }
        }

        /// <summary>
        /// Gets the Resource Name
        /// </summary>
        public string ResourceName
        {
            get { return this.resourceName; }
        }

        #endregion

        #region � Constructors �

        /// <summary>
        /// Initializes a new instance of the <see cref="XmppJid"/> class with
        /// the given JID
        /// </summary>
        /// <param name="jid">The XMPP jid</param>
        public XmppJid(string jid)
        {
            this.Parse(jid);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmppJid"/> class with
        /// the given user name, domain name and resource name.
        /// </summary>
        /// <param name="userName">The user name</param>
        /// <param name="domainName">The domain name</param>
        /// <param name="resourceName">The resource name</param>
        public XmppJid(string userName, string domainName, string resourceName)
        {
            this.userName       = Stringprep.NamePrep(userName);
            this.domainName     = Stringprep.NodePrep(domainName);
            this.resourceName   = Stringprep.ResourcePrep(resourceName);

            this.BuildBareAndFullJid();
        }

        #endregion

        #region � Operator Overloading �

        // Implicit conversion from string to XmppJid. 
        public static implicit operator XmppJid(string x)
        {
            return new XmppJid(x);
        }

        // Explicit conversion from XmppJid to string. 
        public static implicit operator string(XmppJid x)
        {
            if (x == null)
            {
                throw new InvalidOperationException();
            }

            return x.fullJid;
        }

        #endregion

        #region � Overriden Methods �

        // Override the Object.Equals(object o) method:
        public override bool Equals(object o)
        {
            // If parameter is null return false.
            if (o == null)
            {
                return false;
            }
            if (!(o is XmppJid))
            {
                return false;
            }

            return (this.fullJid == ((XmppJid)o).fullJid);
        }

        // Override the Object.GetHashCode() method:
        public override int GetHashCode()
        {
            return this.fullJid.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return this.fullJid;
        }

        #endregion

        #region � Private Methods �

        private void Parse(string jid)
        {
            Match match = JidRegex.Match(jid);

            if (match != null)
            {
                if (match.Groups["userid"] != null)
                {
                    this.userName = Stringprep.NamePrep(match.Groups["userid"].Value);
                }
                if (match.Groups["domain"] != null)
                {
                    this.domainName = Stringprep.NodePrep(match.Groups["domain"].Value);
                }
                if (match.Groups["resource"] != null)
                {
                    this.resourceName = Stringprep.ResourcePrep(match.Groups["resource"].Value);
                }
            }

            this.BuildBareAndFullJid();
        }

        private void BuildBareAndFullJid()
        {
            StringBuilder jidBuffer = new StringBuilder();

            if (this.UserName != null && this.userName.Length > 0)
            {
                jidBuffer.Append(this.UserName);
            }
            if (this.DomainName != null && this.DomainName.Length > 0)
            {
                if (jidBuffer.Length > 0)
                {
                    jidBuffer.Append("@");
                }

                jidBuffer.Append(this.DomainName);
            }

            this.bareJid = jidBuffer.ToString();

            if (this.ResourceName != null && this.ResourceName.Length > 0)
            {
                jidBuffer.AppendFormat("/{0}", this.ResourceName);
            }

            this.fullJid = jidBuffer.ToString();
        }

        #endregion
    }
}
