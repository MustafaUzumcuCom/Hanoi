using System;

namespace Hanoi.Authentication.Facebook
{
    public static class OAuth2Helper
    {
        public static Uri GenerateRequestUrl(string clientId, string redirect = "https://www.facebook.com/connect/login_success.html", string scope = "offline_access,xmpp_login")
        {
            return new Uri(string.Format("https://www.facebook.com/dialog/oauth?client_id={2}&redirect_uri={1}&scope={0}&display=touch&response_type=token", scope, redirect, clientId));
        }
    }
}
