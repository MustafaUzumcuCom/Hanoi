using System;
using System.Collections.Generic;
using Hanoi.Authentication;

namespace Hanoi
{
    public class AuthenticatorFactory : IConnectionFactory {
        
        private IDictionary<StreamFeatures, Func<Connection, Authenticator>> custom =
            new Dictionary<StreamFeatures, Func<Connection, Authenticator>>();

        public Authenticator Create(StreamFeatures features, Connection connection)
        {
            if (custom.ContainsKey(features))
            {
                return custom[features](connection);
            }

            if (SupportsFeature(features, StreamFeatures.SaslDigestMD5))
            {
                return new SaslDigestAuthenticator(connection);
            }
            if (SupportsFeature(features, StreamFeatures.XGoogleToken))
            {
                return new SaslXGoogleTokenAuthenticator(connection);
            }

            if (SupportsFeature(features, StreamFeatures.SaslPlain))
            {
                return new SaslPlainAuthenticator(connection);
            }

            return null;
        }

        private bool SupportsFeature(StreamFeatures feature, StreamFeatures reference)
        {
            return ((reference & feature) == feature);
        }

        public void Register(StreamFeatures streamFeatures, Func<Connection, Authenticator> createAuthenticator)
        {
            custom.Add(streamFeatures, createAuthenticator);
        }
    }

    public interface IConnectionFactory
    {
        Authenticator Create(StreamFeatures sessions, Connection connection);
    }
}
