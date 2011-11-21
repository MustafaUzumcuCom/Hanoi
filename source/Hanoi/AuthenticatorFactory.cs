using System;
using System.Collections.Generic;
using System.Linq;
using Hanoi.Authentication;

namespace Hanoi
{
    public class AuthenticatorFactory : IAuthenticatorFactory
    {
        private readonly IDictionary<string, Func<Connection, Authenticator>> _custom = new Dictionary<string, Func<Connection, Authenticator>>();

        public AuthenticatorFactory()
        {
            Register("DIGEST-MD5", c => new SaslDigestAuthenticator(c));
            Register("X-GOOGLE-TOKEN", c => new SaslXGoogleTokenAuthenticator(c));
            Register("PLAIN", c => new SaslPlainAuthenticator(c));    
        }

        private static IAuthenticatorFactory _default;
        public static IAuthenticatorFactory Default
        {
            get { return _default ?? (_default = new AuthenticatorFactory()); }
        }

        public Authenticator Create(IList<string> features, Connection connection)
        {
            return (from k in _custom.Keys where features.Contains(k) select _custom[k](connection)).FirstOrDefault();
        }

        public void Register(string streamFeatures, Func<Connection, Authenticator> createAuthenticator)
        {
            _custom.Add(streamFeatures, createAuthenticator);
        }
    }
}