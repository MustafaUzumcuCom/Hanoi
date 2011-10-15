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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Hanoi.Serialization.Core.Sasl;

namespace Hanoi.Authentication
{
    /// <summary>
    ///   <see cref="Authenticator" /> implementation for the SASL Digest Authentication mechanism.
    /// </summary>
    /// <remarks>
    ///   References:
    ///   http://www.ietf.org/html.charters/sasl-charter.html
    ///   http://www.ietf.org/internet-drafts/draft-ietf-sasl-rfc2831bis-09.txt
    /// </remarks>
    internal sealed class SaslDigestAuthenticator : Authenticator
    {
        private readonly string _cnonce;
        private readonly AutoResetEvent _successEvent;
        private IDictionary<string, string> _digestChallenge;

        /// <summary>
        ///   Initializes a new instance of the <see cref="SaslDigestAuthenticator" /> class.
        /// </summary>
        public SaslDigestAuthenticator(Connection connection)
            : base(connection)
        {
            _cnonce = Convert.ToBase64String(Encoding.UTF8.GetBytes(IdentifierGenerator.Generate()));
            _successEvent = new AutoResetEvent(false);
        }

        private static IDictionary<string, string> DecodeDigestChallenge(Challenge challenge)
        {
            var table = new Dictionary<string, string>();
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(challenge.Value));
            var keyPairs = Regex.Matches(decoded, @"([\w\s\d]*)\s*=\s*([^,]*)");

            foreach (Match match in keyPairs)
            {
                if (match.Success && match.Groups.Count == 3)
                {
                    var key = match.Groups[1].Value.Trim();
                    var value = match.Groups[2].Value.Trim();

                    // Strip quotes from the value
                    if (value.StartsWith("\"", StringComparison.OrdinalIgnoreCase) ||
                        value.StartsWith("'", StringComparison.OrdinalIgnoreCase))
                    {
                        value = value.Remove(0, 1);
                    }

                    if (value.EndsWith("\"", StringComparison.OrdinalIgnoreCase) ||
                        value.EndsWith("'", StringComparison.OrdinalIgnoreCase))
                    {
                        value = value.Remove(value.Length - 1, 1);
                    }

                    if (key == "nonce" && table.ContainsKey(key))
                    {
                        return null;
                    }

                    table.Add(key, value);
                }
            }

            return table;
        }

        /// <summary>
        ///   Performs the authentication using the SASL digest authentication mechanism.
        /// </summary>
        public override void Authenticate()
        {
            // Send authentication mechanism
            var auth = new Auth { Mechanism = XmppCodes.SaslDigestMD5Mechanism };

            Connection.Send(auth);

            _successEvent.WaitOne();

            // Verify received Digest-Challenge

            // Check that the nonce setting is pressent
            if (!_digestChallenge.ContainsKey("nonce"))
            {
                throw new XmppException("SASL Authrization failed. Incorrect challenge received from server");
            }

            // Check that the charset is correct
            if (_digestChallenge.ContainsKey("charset")
                && _digestChallenge["charset"] != "utf-8")
            {
                throw new XmppException("SASL Authrization failed. Incorrect challenge received from server");
            }

            // Check that the mechanims is correct
            if (!_digestChallenge.ContainsKey("algorithm")
                || _digestChallenge["algorithm"] != "md5-sess")
            {
                throw new XmppException("SASL Authrization failed. Incorrect challenge received from server");
            }

            // Send the Digest-Reponse
            var digestResponse = new Response();

            digestResponse.Value = BuildDigestRespose();

            Connection.Send(digestResponse);

            _successEvent.WaitOne();

            if (_digestChallenge.ContainsKey("rspauth"))
            {
                digestResponse = new Response();
                Connection.Send(digestResponse);

                _successEvent.WaitOne();
            }

            if (!AuthenticationFailed)
            {
                // Re-Initialize XMPP Stream
                Connection.InitializeXmppStream();

                // Wait until we receive the Stream features
                Connection.WaitForStreamFeatures();
            }
        }

        protected override void OnUnhandledMessage(object sender, UnhandledMessageEventArgs e)
        {
            if (e.StanzaInstance is Challenge)
            {
                // Response to teh Authentication Information Request
                _digestChallenge = DecodeDigestChallenge((Challenge)e.StanzaInstance);
                _successEvent.Set();
            }
            else if (e.StanzaInstance is Success)
            {
                _successEvent.Set();
            }
        }

        protected override void OnAuthenticationError(object sender, AuthenticationFailiureEventArgs e)
        {
            base.OnAuthenticationError(sender, e);

            _successEvent.Set();
        }

        private string BuildDigestRespose()
        {
            var response = new StringBuilder();
            string digestUri;

            response.AppendFormat("username=\"{0}\",", Connection.UserId.UserName);

            if (_digestChallenge.ContainsKey("realm"))
            {
                response.AppendFormat("realm=\"{0}\",", _digestChallenge["realm"]);

                digestUri = String.Format("xmpp/{0}", _digestChallenge["realm"]);
            }
            else
            {
                digestUri = String.Format("xmpp/{0}", Connection.HostName);
            }

            response.AppendFormat("nonce=\"{0}\",", _digestChallenge["nonce"]);
            response.AppendFormat("cnonce=\"{0}\",", _cnonce);
            response.AppendFormat("nc={0},", "00000001");
            response.AppendFormat("qop={0},", SelectProtectionQuality());
            response.AppendFormat("digest-uri=\"{0}\",", digestUri);
            response.AppendFormat("response={0},", GenerateResponseValue());
            response.AppendFormat("charset={0}", _digestChallenge["charset"]);

            return Encoding.UTF8.GetBytes(response.ToString()).ToBase64String();
        }

        private string GenerateResponseValue()
        {
            var realm = ((_digestChallenge.ContainsKey("realm")) ? _digestChallenge["realm"] : Connection.HostName);
            var nonce = _digestChallenge["nonce"];
            var userId = Connection.UserId.UserName;
            var password = Connection.UserPassword;
            var digestURI = String.Format("xmpp/{0}", realm);
            var quop = SelectProtectionQuality();

            // TODO: is this necessary?
            /*
            If authzid is specified, then A1 is
                A1 = { H( { username-value, ":", realm-value, ":", passwd } ),
                    ":", nonce-value, ":", cnonce-value, ":", authzid-value }

            If authzid is not specified, then A1 is
                A1 = { H( { username-value, ":", realm-value, ":", passwd } ),
                    ":", nonce-value, ":", cnonce-value }
            */

            var a1 = new MemoryStream();

            var a1Hash = (new[] { userId, ":", realm, ":", password }).ComputeMD5Hash();
            var temp = Encoding.UTF8.GetBytes(String.Format(":{0}:{1}", nonce, _cnonce));

            // There are no authzid-value
            a1.Write(a1Hash, 0, a1Hash.Length);
            a1.Write(temp, 0, temp.Length);

            /*
            HEX(H(A2))

            If the "qop" directive's value is "auth", then A2 is:
                A2       = { "AUTHENTICATE:", digest-uri-value }

            If the "qop" value is "auth-int" or "auth-conf" then A2 is:
                A2       = { "AUTHENTICATE:", digest-uri-value,
                        ":00000000000000000000000000000000" }
            */

            string a2 = "AUTHENTICATE:" + digestURI +
                        ((quop == "auth") ? null : ":00000000000000000000000000000000");

            /*
            KD(k, s) = H({k, ":", s}), 

            HEX( KD ( HEX(H(A1)),
                 { nonce-value, ":" nc-value, ":",
                   cnonce-value, ":", qop-value, ":", HEX(H(A2)) }))
            */
            /*
            return SaslDigestAuthenticator.ConvertToHex(
                    SaslDigestAuthenticator.ComputeHash(
                        SaslDigestAuthenticator.ConvertToHex(SaslDigestAuthenticator.ComputeHash(a1.ToArray())), ":",
                        nonce, ":", "00000001", ":", cnonce, ":", quop, ":",
                        SaslDigestAuthenticator.ConvertToHex(SaslDigestAuthenticator.ComputeHash(a2)))
                        );
             */

            var hexA1 = a1.ToArray().ComputeMD5Hash().ToHexString();
            var hexA2 = (new[] { a2 }).ComputeMD5Hash().ToHexString();

            return
                (new[] { hexA1, ":", nonce, ":", "00000001", ":", _cnonce, ":", quop, ":", hexA2 }).ComputeMD5Hash().
                    ToHexString();
        }

        private string SelectProtectionQuality()
        {
            if (_digestChallenge.ContainsKey("qop-options"))
            {
                var quopOptions = _digestChallenge["qop-options"].Split(',');

                foreach (var quo in quopOptions)
                {
                    switch (quo)
                    {
                        case "auth-int":
                        case "auth-conf":
                            break;

                        // case "auth":
                        default:
                            return quo;
                    }
                }
            }

            return "auth";
        }
    }
}