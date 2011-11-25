using System.Runtime.Serialization;

namespace Hanoi.Authentication.Messenger
{
    [DataContract]
    public class OAuthToken
    {
        [DataMember(Name = OAuthConstants.AccessToken)]
        public string AccessToken { get; set; }

        [DataMember(Name = OAuthConstants.RefreshToken)]
        public string RefreshToken { get; set; }

        [DataMember(Name = OAuthConstants.ExpiresIn)]
        public string ExpiresIn { get; set; }

        [DataMember(Name = OAuthConstants.Scope)]
        public string Scope { get; set; }
    }
}