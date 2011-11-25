using System.Text;
using System.Threading;
using System.Web;
using Hanoi.Serialization.Core.Sasl;

namespace Hanoi.Authentication.Facebook
{
    public class FacebookPlatformAuthenticator : Authenticator
    {
        private readonly string _apiKey;
        private readonly AutoResetEvent _waitEvent;

        public FacebookPlatformAuthenticator(Connection connection, string apiKey)
            : base(connection)
        {
            _apiKey = apiKey;
            _waitEvent = new AutoResetEvent(false);
        }

        public override string FeatureKey
        {
            get { return FacebookFactory.Key; }
        }

        public override void Authenticate()
        {
            Connection.Send(new Auth
            {
                Mechanism = FeatureKey,
            });
            _waitEvent.WaitOne();

            if (AuthenticationFailed)
                return;

            Connection.InitializeXmppStream();
            Connection.WaitForStreamFeatures();
        }

        protected override void OnUnhandledMessage(object sender, UnhandledMessageEventArgs e)
        {
            if (e.StanzaInstance is Challenge)
            {
                var challenge = ((Challenge)e.StanzaInstance);
                var value = HttpUtility.UrlDecode(DecodeFrom64(challenge.Value));

                var kv = value.Split('&');
                string method = "";
                string nonce = "";
                foreach (var k in kv)
                {
                    var values = k.Split('=');
                    switch (values[0])
                    {
                        case "method":
                            method = values[1];
                            break;

                        case "nonce":
                            nonce = values[1];
                            break;
                    }
                }

                var response = BuildResponse(method, nonce);
                Connection.Send(new Response { Value = response });
            }

            else if (e.StanzaInstance is Success)
            {
                _waitEvent.Set();
            }
        }

        private string BuildResponse(string method, string nonce)
        {
            var requestString = new StringBuilder();
            requestString.AppendFormat("method={0}", method);
            requestString.AppendFormat("&api_key={0}", _apiKey);
            requestString.AppendFormat("&access_token={0}", Connection.UserPassword);
            requestString.AppendFormat("&call_id={0}", "0");
            requestString.AppendFormat("&v={0}", "1.0");
            requestString.AppendFormat("&nonce={0}", nonce);

            var response = EncodeTo64(requestString.ToString());
            return response;
        }

        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = Encoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
    }
}
