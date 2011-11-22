using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hanoi.Authentication.Messenger
{
    public static class OAuth2Helper
    {
        public static Uri GenerateRequestUrl(string clientId, string scope, string display = "touch")
        {
            return new Uri(String.Format(@"https://oauth.live.com/authorize?client_id={0}&display={2}&redirect_uri=https://oauth.live.com/desktop&response_type=token&scope={1}", clientId, scope, display));
        }

        public static string AccessTokenFromUriResponse(Uri response)
        {
            if (!response.Fragment.Contains("access_token"))
                return string.Empty;

            var responseAll = Regex.Split(response.Fragment.Remove(0, 1), "&");
            return (from t in responseAll 
                    select Regex.Split(t, "=") into nvPair 
                    where nvPair[0] == "access_token" 
                    select nvPair[1]).FirstOrDefault();
        }
    }
}
