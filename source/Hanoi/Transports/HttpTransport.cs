using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Hanoi.Serialization;
using Hanoi.Serialization.Extensions.Bosh;

namespace Hanoi.Core.Transports {
    /// <summary>
    ///   XMPP over Bosh (HTTP) Transport implementation
    /// </summary>
    /// <remarks>
    ///   XEP-0124 - Bidirectional-streams Over Synchronous HTTP (BOSH) - http://xmpp.org/extensions/xep-0124.pdf
    ///   XEP-0206 - XMPP over BOSH - http://xmpp.org/extensions/xep-0206.pdf
    /// </remarks>
    internal sealed class HttpTransport : BaseTransport {
        private const string ContentType = "text/xml; charset=utf-8";
        private const string BoshVersion = "1.10";
        private const string RouteFormat = "xmpp:{0}:9999";
        private const string DefaultLanguage = "en";

        private long rid;
        private HttpBindBody streamResponse;

        public override void Open(XmppConnectionString connectionString) {
            // Connection string
            ConnectionString = connectionString;
            UserId = ConnectionString.UserId;

            // Generate initial RID
            var rng = new RNGCryptoServiceProvider();
            var bytes = new byte[32/8];

            rng.GetBytes(bytes);

            rid = BitConverter.ToInt32(bytes, 0);
            rid = (rid < 0) ? -rid : rid;

            // HTTP Configuration
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
        }

        public override void InitializeXmppStream() {
            var message = new HttpBindBody();

            message.Rid = (rid++).ToString();
            message.To = ConnectionString.HostName;
            message.Lang = DefaultLanguage;

            if (streamResponse == null)
            {
                message.Content = ContentType;
                message.From = UserId.BareIdentifier;
                message.Hold = 1;
                message.HoldSpecified = true;
                message.Route = String.Format(RouteFormat, ConnectionString.HostName);
                message.Ver = BoshVersion;
                message.Wait = 60;
                message.WaitSpecified = true;
                message.Ack = "1";
            }
            else
            {
                message.Sid = streamResponse.Sid;
                message.Restart = true;
            }

            var response = SendSync(XmppSerializer.Serialize(message));

#warning TODO: If no <stream:features/> element is included in the connection manager's session creation response, then the client SHOULD send empty request elements until it receives a response containing a <stream:features/> element.

            if (response != null)
            {
                streamResponse = response;

                ProcessResponse(response);

#warning TODO: Check if the response has an stream-features element
                OnXmppStreamInitializedSubject.OnNext(String.Empty);
            }
            else
            {
#warning TODO: Review how to handle this case
                throw new Exception("");
            }
        }

        /// <summary>
        ///   Sends a new message.
        /// </summary>
        /// <param name = "message">The message to be sent</param>
        public override void Send(object message) {
            var body = new HttpBindBody
                           {
                               Rid = (rid++).ToString(),
                               Sid = streamResponse.Sid
                           };

            body.Items.Add(message);

            Send(XmppSerializer.Serialize(body));
        }

        /// <summary>
        ///   Sends an XMPP message string to the XMPP Server
        /// </summary>
        /// <param name = "value"></param>
        public override void Send(string value) {
            Send(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        ///   Sends an XMPP message buffer to the XMPP Server
        /// </summary>
        public override void Send(byte[] buffer) {
            var response = SendSync(buffer);

            ProcessResponse(response);
        }

        public override void Close() {
            base.Close();

            ServicePointManager.ServerCertificateValidationCallback -= ValidateRemoteCertificate;

            streamResponse = null;
            rid = 0;
        }

        private static bool ValidateRemoteCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors policyErrors) {
            // allow any old dodgy certificate...
            return true;
        }

        /// <summary>
        ///   Sends an XMPP message buffer to the XMPP Server
        /// </summary>
        public HttpBindBody SendSync(byte[] buffer) {
            Debug.WriteLine(Encoding.UTF8.GetString(buffer));

            lock (SyncWrites)
            {
                HttpWebRequest webRequest = CreateWebRequest();

                using (var stream = webRequest.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);

                    var webResponse = (HttpWebResponse) webRequest.GetResponse();

                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var responseStream = webResponse.GetResponseStream())
                        {
                            using (var responseReader = new StreamReader(responseStream, true))
                            {
                                var response = responseReader.ReadToEnd();

                                // TODO: necessary?
                                Debug.WriteLine(response);

                                return XmppSerializer.Deserialize("body", response) as HttpBindBody;
                            }
                        }
                    }
                }

                return null;
            }
        }

        private void ProcessResponse(HttpBindBody response) {
            foreach (var item in response.Items)
            {
                OnMessageReceivedSubject.OnNext(item);
            }
        }

        private HttpWebRequest CreateWebRequest() {
            var webRequest = (HttpWebRequest) WebRequest.Create
                                                  (
                                                      String.Format("https://{0}/http-bind", ConnectionString.HostName)
                                                  );

            webRequest.ContentType = ContentType;
            webRequest.Method = "POST";
            webRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            return webRequest;
        }

        /// <summary>
        ///   Sends a new message.
        /// </summary>
        /// <param name = "message">The message to be sent</param>
        [Obsolete("R# tells me this is never used")]
        private void Send(HttpBindBody message) {
            Send(XmppSerializer.Serialize(message));
        }
    }
}