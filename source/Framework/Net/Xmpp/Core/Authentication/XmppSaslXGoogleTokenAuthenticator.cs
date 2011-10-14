using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Hanoi.Authentication;
using Hanoi.Xmpp.Serialization.Core.Sasl;

namespace Hanoi.Core.Authentication {
    /// <summary>
    ///   http://209.85.129.132/search?q=cache:AhT1kmNCYw4J:dystopics.dump.be/%3Fp%3D54+IssueAuthToken&cd=2&hl=es&ct=clnk&gl=es&client=firefox-a
    ///   </remarks>
    internal sealed class XmppSaslXGoogleTokenAuthenticator
        : XmppAuthenticator {
        private readonly AutoResetEvent waitEvent;
        private string auth;
        private string lsid;
        private string sid;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "T:XmppSaslPlainAuthenticator" /> class.
        /// </summary>
        public XmppSaslXGoogleTokenAuthenticator(XmppConnection connection)
            : base(connection) {
            waitEvent = new AutoResetEvent(false);
        }

        /// <summary>
        ///   Performs the authentication using the SASL Plain authentication mechanism.
        /// </summary>
        public override void Authenticate() {
            if (RequestToken())
            {
                // Send authentication mechanism
                var auth = new Auth();
                auth.Value = BuildMessage();

                auth.Mechanism = XmppCodes.SaslXGoogleTokenMechanism;

                Connection.Send(auth);

                waitEvent.WaitOne();

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

        protected override void OnUnhandledMessage(object sender, XmppUnhandledMessageEventArgs e) {
            if (e.StanzaInstance is Success)
            {
                waitEvent.Set();
            }
        }

        protected override void OnAuthenticationError(object sender, XmppAuthenticationFailiureEventArgs e) {
            base.OnAuthenticationError(sender, e);

            waitEvent.Set();
        }

        private string BuildMessage() {
            string message = String.Format("\0{0}\0{1}", Connection.UserId.BareIdentifier, auth);

            return Encoding.UTF8.GetBytes(message).ToBase64String();
        }

        /// <summary>
        ///   http://209.85.129.132/search?q=cache:AhT1kmNCYw4J:dystopics.dump.be/%3Fp%3D54+IssueAuthToken&cd=2&hl=es&ct=clnk&gl=es&client=firefox-a
        /// </summary>
        /// <returns></returns>
        private bool RequestToken() {
            var request = (HttpWebRequest) WebRequest.Create("https://www.google.com/accounts/ClientAuth");
            var requestString = new StringBuilder();

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            System.IO.Stream stream = request.GetRequestStream();

            requestString.AppendFormat("Email={0}", Connection.UserId.BareIdentifier);
            requestString.AppendFormat("&Passwd={0}", Connection.UserPassword);
            requestString.AppendFormat("&source={0}", Connection.UserId.ResourceName);
            requestString.AppendFormat("&service={0}", "mail");
            requestString.AppendFormat("&PersistentCookie={0}", false);

            byte[] buffer = Encoding.UTF8.GetBytes(requestString.ToString());

            stream.Write(buffer, 0, buffer.Length);
            stream.Dispose();

            var response = (HttpWebResponse) request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                System.IO.Stream responseStream = response.GetResponseStream();
                var responseReader = new StreamReader(responseStream, true);

                while (responseReader.Peek() != -1)
                {
                    string data = responseReader.ReadLine();

                    if (data.StartsWith("SID="))
                    {
                        sid = data.Replace("SID=", "");
                    }
                    else if (data.StartsWith("LSID="))
                    {
                        lsid = data.Replace("LSID=", "");
                    }
                    else if (data.StartsWith("Auth="))
                    {
                        auth = data.Replace("Auth=", "");
                    }
                }

                responseStream.Dispose();
                responseReader.Dispose();

                return (!String.IsNullOrEmpty(sid) &&
                        !String.IsNullOrEmpty(lsid) &&
                        !String.IsNullOrEmpty(auth));
            }

            return false;
        }

        /*
        private bool IssueToken()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://www.google.com/accounts/IssueAuthToken");
            StringBuilder requestString = new StringBuilder();

            request.Method      = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            System.IO.Stream stream = request.GetRequestStream();

            requestString.AppendFormat("SID={0}", this.sid);
            requestString.AppendFormat("&service={0}", "mail");
            requestString.AppendFormat("&Session={0}", true);

            byte[] buffer = Encoding.UTF8.GetBytes(requestString.ToString());

            stream.Write(buffer, 0, buffer.Length);
            stream.Dispose();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                System.IO.Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream, true);

                this.authToken = responseReader.ReadToEnd();

                responseStream.Dispose();
                responseReader.Dispose();

                return true;
            }

            return false;
        }
        */
        }
}