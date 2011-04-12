using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using BabelIm.Net.Xmpp.Serialization.Core.Sasl;

namespace BabelIm.Net.Xmpp.Core
{
    /// <summary>
    /// http://209.85.129.132/search?q=cache:AhT1kmNCYw4J:dystopics.dump.be/%3Fp%3D54+IssueAuthToken&cd=2&hl=es&ct=clnk&gl=es&client=firefox-a
    /// </remarks>
    internal sealed class XmppSaslXGoogleTokenAuthenticator 
        : XmppAuthenticator
    {
        #region · Fields ·

        private AutoResetEvent  waitEvent;
        private string          sid;
        private string          lsid;
        private string          auth;

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmppSaslPlainAuthenticator"/> class.
        /// </summary>
        public XmppSaslXGoogleTokenAuthenticator(XmppConnection connection)
            : base(connection)
        {
            this.waitEvent = new AutoResetEvent(false);
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Performs the authentication using the SASL Plain authentication mechanism.
        /// </summary>
        public override void Authenticate()
        {
            if (this.RequestToken())
            {
                // Send authentication mechanism
                Auth auth   = new Auth();
                auth.Value  = this.BuildMessage();

                auth.Mechanism = XmppCodes.SaslXGoogleTokenMechanism;

                this.Connection.Send(auth);

                this.waitEvent.WaitOne();

                if (!this.AuthenticationFailed)
                {
                    // Re-Initialize XMPP Stream
                    this.Connection.InitializeXmppStream();

                    // Wait until we receive the Stream features
                    this.Connection.WaitForStreamFeatures();
                }
            }
            else
            {
                this.AuthenticationFailed = true;
            }
        }

        #endregion

        #region · Protected Methods ·

        protected override void OnUnhandledMessage(object sender, XmppUnhandledMessageEventArgs e)
        {
            if (e.StanzaInstance is Success)
            {
                this.waitEvent.Set();
            }
        }

        protected override void OnAuthenticationError(object sender, XmppAuthenticationFailiureEventArgs e)
        {
            base.OnAuthenticationError(sender, e);

            this.waitEvent.Set();
        }

        #endregion

        #region · Private Methods ·

        private string BuildMessage()
        {
            string message = String.Format("\0{0}\0{1}", this.Connection.UserId.BareIdentifier, this.auth);

            return Encoding.UTF8.GetBytes(message).ToBase64String();
        }

        /// <summary>
        /// http://209.85.129.132/search?q=cache:AhT1kmNCYw4J:dystopics.dump.be/%3Fp%3D54+IssueAuthToken&cd=2&hl=es&ct=clnk&gl=es&client=firefox-a
        /// </summary>
        /// <returns></returns>
        private bool RequestToken()
        {
            HttpWebRequest  request         = (HttpWebRequest)HttpWebRequest.Create("https://www.google.com/accounts/ClientAuth");
            StringBuilder   requestString   = new StringBuilder();

            request.Method      = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            System.IO.Stream stream = request.GetRequestStream();

            requestString.AppendFormat("Email={0}", this.Connection.UserId.BareIdentifier);
            requestString.AppendFormat("&Passwd={0}", this.Connection.UserPassword);
            requestString.AppendFormat("&source={0}", this.Connection.UserId.ResourceName);
            requestString.AppendFormat("&service={0}", "mail");
            requestString.AppendFormat("&PersistentCookie={0}", false);

            byte[] buffer = Encoding.UTF8.GetBytes(requestString.ToString());

            stream.Write(buffer, 0, buffer.Length);
            stream.Dispose();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                System.IO.Stream    responseStream  = response.GetResponseStream();
                StreamReader        responseReader  = new StreamReader(responseStream, true);

                while (responseReader.Peek() != -1)
                {
                    string data = responseReader.ReadLine();

                    if (data.StartsWith("SID="))
                    {
                        this.sid = data.Replace("SID=", "");
                    }
                    else if (data.StartsWith("LSID="))
                    {
                        this.lsid = data.Replace("LSID=", "");
                    }
                    else if (data.StartsWith("Auth="))
                    {
                        this.auth = data.Replace("Auth=", "");
                    }
                }

                responseStream.Dispose();
                responseReader.Dispose();

                return (!String.IsNullOrEmpty(this.sid) && 
                            !String.IsNullOrEmpty(this.lsid) && 
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

        #endregion
    }
}
