using System.Collections.Generic;

namespace Hanoi.Authentication.Facebook
{
    public class FacebookFactory : IAuthenticatorFactory
    {
        private readonly string _apikey;
        public static string Key = "X-FACEBOOK-PLATFORM";

        public FacebookFactory(string apikey)
        {
            _apikey = apikey;
        }

        public Authenticator Create(IList<string> features, Connection connection)
        {
            return features.Contains(Key) ? new FacebookPlatformAuthenticator(connection, _apikey) : null;
        }
    }
}