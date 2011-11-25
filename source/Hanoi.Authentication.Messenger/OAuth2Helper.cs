using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Hanoi.Authentication.Messenger
{
    public static class OAuth2Helper
    {
        public static Uri GenerateRequestUrl(string clientId, Scope scopes, string type = "token", string callback = "https://oauth.live.com/desktop", string display = "touch")
        {
            string scopeString = "";
            var y = Enum.GetValues(typeof(Scope)).Cast<Scope>();
            foreach (var s in y)
            {
                if ((scopes & s) == s)
                    scopeString += GetDescription(s) + "%20";
            }

            return new Uri(String.Format(@"https://oauth.live.com/authorize?client_id={0}&display={2}&redirect_uri={4}&response_type={3}&scope={1}", clientId, scopeString, display, type, callback));
        }

        public static OAuthToken RefreshToken(string clientId, string clientSecret, string refreshToken, string callback = "https://oauth.live.com/desktop")
        {
            var wc = new WebClient();
            var url = string.Format("https://oauth.live.com/token?client_id={0}&redirect_uri={1}&client_secret={2}&refresh_token={3}&grant_type=refresh_token", clientId, callback, clientSecret, refreshToken);
            var response = wc.DownloadString(url);
            var ms = new MemoryStream(Encoding.Unicode.GetBytes(response));
            var serializer = new DataContractJsonSerializer(typeof(OAuthToken));
            return serializer.ReadObject(ms) as OAuthToken;
        }

        public static string CodeFromUriResponse(Uri response)
        {
            if (!response.Query.Contains("code"))
                return null;

            var nvPair = Regex.Split(response.Query.Substring(1), "=");
            return nvPair[0] == "code" ? nvPair[1] : null;
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

        public static OAuthToken AuthCodeToToken(string clientId, string clientSecret, string auth, string callback = "https://oauth.live.com/desktop")
        {
            var wc = new WebClient();
            var url = string.Format("https://oauth.live.com/token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}&grant_type=authorization_code", clientId, callback, clientSecret, auth);
            var response = wc.DownloadString(url);
            var ms = new MemoryStream(Encoding.Unicode.GetBytes(response));
            var serializer = new DataContractJsonSerializer(typeof(OAuthToken));
            return serializer.ReadObject(ms) as OAuthToken;
        }

        public static string GetDescription<T>(T value) where T : struct
        {
            string name = value.ToString();

            object[] attrs =
                value.GetType().GetField(name).GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

            return (attrs.Length > 0) ? ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description : name;
        }

        public static T GetEnumFromDescription<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("value must be non-empty");
            }

            var field = typeof(T).GetFields().FirstOrDefault(f =>
            {
                var attrs = f.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    if (((System.ComponentModel.DescriptionAttribute)attrs[0]).Description == value)
                    {
                        return true;
                    }
                }

                return false;
            });

            if (field != null)
            {
                return (T)field.GetValue(null);
            }
            else
            {
                return (T)Enum.Parse(typeof(T), value);
            }
        }
    }
}
