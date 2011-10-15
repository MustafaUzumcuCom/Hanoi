using System;
using System.Collections.Generic;
using Hanoi.Authentication;

namespace Hanoi
{
    public class AuthenticatorFactory : IAuthenticatorFactory
    {
        private readonly IDictionary<StreamFeatures, Func<Connection, Authenticator>> _custom =
            new Dictionary<StreamFeatures, Func<Connection, Authenticator>>();

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

            if (HasFeature(features, StreamFeatures.SaslDigestMD5))
            {
                return new SaslDigestAuthenticator(connection);
            }
            if (HasFeature(features, StreamFeatures.XGoogleToken))
            {
                return new SaslXGoogleTokenAuthenticator(connection);
            }

            if (HasFeature(features, StreamFeatures.SaslPlain))
            {
                return new SaslPlainAuthenticator(connection);
            }

            return null;
        }

        private bool HasFeature(StreamFeatures features, StreamFeatures item)
        {
            return ((item & features) == features);
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
