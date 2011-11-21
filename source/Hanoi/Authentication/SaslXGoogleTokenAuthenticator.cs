using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Hanoi.Serialization.Core.Sasl;

namespace Hanoi.Authentication
{
    /// <remarks>
    /// http://209.85.129.132/search?q=cache:AhT1kmNCYw4J:dystopics.dump.be/%3Fp%3D54+IssueAuthToken&cd=2&hl=es&ct=clnk&gl=es&client=firefox-a
    /// </remarks>
    internal sealed class SaslXGoogleTokenAuthenticator : Authenticator
    {
        private readonly AutoResetEvent _waitEvent;
        private string _auth;
        private string _lsid;
        private string _sid;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "SaslPlainAuthenticator" /> class.
        /// </summary>
        public SaslXGoogleTokenAuthenticator(Connection connection)
            : base(connection)
        {
            _waitEvent = new AutoResetEvent(false);
        }

        public override string FeatureKey
        {
            get { return "X-GOOGLE-TOKEN"; }
        }

        /// <summary>
        ///   Performs the authentication using the SASL Plain authentication mechanism.
        /// </summary>
        public override void Authenticate()
        {
            if (RequestToken())
            {
                // Send authentication mechanism
                var auth = new Auth
                               {
                                   Value = BuildMessage(),
                                   Mechanism = FeatureKey,
                               };

                Connection.Send(auth);

                _waitEvent.WaitOne();

                if (!AuthenticationFailed)
                {
                    // Re-Initialize XMPP Stream
                    Connection.InitializeXmppStream();

                    // Wait until we receive the Stream features
                    Connection.WaitForStreamFeatures();
                }
            }
            else
            {
                AuthenticationFailed = true;
            }
        }

        protected override void OnUnhandledMessage(object sender, UnhandledMessageEventArgs e)
        {
            if (e.StanzaInstance is Success)
            {
                _waitEvent.Set();
            }
        }

        protected override void OnAuthenticationError(object sender, AuthenticationFailiureEventArgs e)
        {
            base.OnAuthenticationError(sender, e);

            _waitEvent.Set();
        }

        private string BuildMessage()
        {
            var message = String.Format("\0{0}\0{1}", Connection.UserId.BareIdentifier, _auth);

            return Encoding.UTF8.GetBytes(message).ToBase64String();
        }

        /// <summary>
        ///   http://209.85.129.132/search?q=cache:AhT1kmNCYw4J:dystopics.dump.be/%3Fp%3D54+IssueAuthToken&cd=2&hl=es&ct=clnk&gl=es&client=firefox-a
        /// </summary>
        /// <returns></returns>
        private bool RequestToken()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://www.google.com/accounts/ClientAuth");
            var requestString = new StringBuilder();

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var stream = request.GetRequestStream();

            requestString.AppendFormat("Email={0}", Connection.UserId.BareIdentifier);
            requestString.AppendFormat("&Passwd={0}", Connection.UserPassword);
            requestString.AppendFormat("&source={0}", Connection.UserId.ResourceName);
            requestString.AppendFormat("&service={0}", "mail");
            requestString.AppendFormat("&PersistentCookie={0}", false);

            var buffer = Encoding.UTF8.GetBytes(requestString.ToString());

            stream.Write(buffer, 0, buffer.Length);
            stream.Dispose();

            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    var responseReader = new StreamReader(responseStream, true);

                    while (responseReader.Peek() != -1)
                    {
                        var data = responseReader.ReadLine();
                        if (data != null)
                        {
                            if (data.StartsWith("SID="))
                            {
                                _sid = data.Replace("SID=", "");
                            }
                            else if (data.StartsWith("LSID="))
                            {
                                _lsid = data.Replace("LSID=", "");
                            }
                            else if (data.StartsWith("Auth="))
                            {
                                _auth = data.Replace("Auth=", "");
                            }
                        }
                    }

                    responseStream.Dispose();
                    responseReader.Dispose();
                }

                return (!String.IsNullOrEmpty(_sid) &&
                        !String.IsNullOrEmpty(_lsid) &&
                        !String.IsNullOrEmpty(_auth));
            }

            return false;
        }
    }
}