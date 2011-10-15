using System;
using System.Collections.Generic;
using Hanoi.Authentication;

namespace Hanoi
{
    public class AuthenticatorFactory : IAuthenticatorFactory
    {
        private readonly IDictionary<StreamFeatures, Func<Connection, Authenticator>> _custom =
            new Dictionary<StreamFeatures, Func<Connection, Authenticator>>();

        public AuthenticatorFactory() {
            Register(StreamFeatures.SaslDigestMD5, c => new SaslDigestAuthenticator(c));
            Register(StreamFeatures.XGoogleToken, c => new SaslXGoogleTokenAuthenticator(c));
            Register(StreamFeatures.SaslPlain, c => new SaslPlainAuthenticator(c));    
        }

        private static IAuthenticatorFactory _default;
        public static IAuthenticatorFactory Default
        {
            get { return _default ?? (_default = new AuthenticatorFactory()); }
        }

        public Authenticator Create(StreamFeatures features, Connection connection)
        {
            if (_custom.ContainsKey(features))
            {
                return _custom[features](connection);
            }

            return null;
        }

        public void Register(StreamFeatures streamFeatures, Func<Connection, Authenticator> createAuthenticator)
        {
            _custom.Add(streamFeatures, createAuthenticator);
        }
    }

    public interface IAuthenticatorFactory
    {
        Authenticator Create(StreamFeatures sessions, Connection connection);
    }
}
