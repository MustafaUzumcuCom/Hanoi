using System.Text;

namespace Hanoi.Authentication.Facebook
{
    public class FacebookPlatformAuthenticator : Authenticator
    {

        public FacebookPlatformAuthenticator(Connection connection) : base(connection)
        {
        }

        public override string FeatureKey
        {
            get { return "X-FACEBOOK-PLATFORM"; }
        }

        public override void Authenticate()
        {
            
        }

        protected override void OnUnhandledMessage(object sender, UnhandledMessageEventArgs e)
        {
            
        }

        private void BuildResponse()
        {
            var requestString = new StringBuilder();
            requestString.AppendFormat("method={0}", "");
            requestString.AppendFormat("&api_key={0}", "");
            requestString.AppendFormat("&access_token={0}", "");
            requestString.AppendFormat("&call_id={0}", 0.0f);
            requestString.AppendFormat("&v={0}", 1.0);
            requestString.AppendFormat("&nonce={0}", "");

            /*string method: Should be the same as the method specified by the server.
            string api_key: The application key associated with the calling application.
            string access_token: The access_token obtained in the above step.
            float call_id: The request's sequence number.
            string v: This must be set to 1.0 to use this version of the API.
            string format: Optional - Ignored.
            string cnonce: Optional - Client-selected nonce. Ignored.
            string nonce: Should be the same as the nonce specified by the server.*/
        }
    }
}
