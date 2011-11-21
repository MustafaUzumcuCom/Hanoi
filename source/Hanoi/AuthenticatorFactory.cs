using System;
using System.Collections.Generic;
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

        public Authenticator Create(List<string> features, Connection connection)
        {
            foreach (var k in _custom.Keys)
            {
                if (features.Contains(k))
                    return _custom[k](connection);
            }

            return null;
        }

        public void Register(string streamFeatures, Func<Connection, Authenticator> createAuthenticator)
        {
            _custom.Add(streamFeatures, createAuthenticator);
        }
    }

    public interface IAuthenticatorFactory
    {
        Authenticator Create(List<string> features, Connection connection);
    }
}