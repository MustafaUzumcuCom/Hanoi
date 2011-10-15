using Hanoi.Authentication;

namespace Hanoi
{
    public class ConnectionFactory : IConnectionFactory
    {
        public Authenticator Create(StreamFeatures features, Connection connection)
        {
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
    }

    public interface IConnectionFactory
    {
        Authenticator Create(StreamFeatures sessions, Connection connection);
    }
}
