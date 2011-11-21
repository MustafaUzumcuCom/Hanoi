using System.Collections.Generic;

namespace Hanoi.Authentication.Facebook
{
    public class FacebookFactory : IAuthenticatorFactory
    {
        public Authenticator Create(IList<string> features, Connection connection)
        {
            return new FacebookPlatformAuthenticator(connection);
        }
    }
}