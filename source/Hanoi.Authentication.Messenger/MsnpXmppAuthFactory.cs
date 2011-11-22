using System.Collections.Generic;

namespace Hanoi.Authentication.Messenger
{
    public class MsnpXmppAuthFactory : IAuthenticatorFactory
    {
        public static string Key = "X-MESSENGER-OAUTH2";
        public Authenticator Create(IList<string> features, Connection connection)
        {
            return features.Contains(Key) ? new MsnpXmppAuthenticator(connection) : null;
        }
    }
}