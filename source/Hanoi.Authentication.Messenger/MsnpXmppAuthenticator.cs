using System.Threading;
using System.Web;
using Hanoi.Serialization.Core.Sasl;

namespace Hanoi.Authentication.Messenger
{
    public class MsnpXmppAuthenticator : Authenticator
    {
        private readonly AutoResetEvent _waitEvent;
        private readonly Connection _connection;

        public MsnpXmppAuthenticator(Connection connection)
            : base(connection)
        {
            _connection = connection;
            _waitEvent = new AutoResetEvent(false);
        }

        public override string FeatureKey
        {
            get { return MsnpXmppAuthFactory.Key; }
        }

        public override void Authenticate()
        {
            var auth = new Auth
                           {
                               Mechanism = MsnpXmppAuthFactory.Key,
                               Value = HttpUtility.UrlDecode(Connection.UserPassword),
                           };

            Connection.Send(auth);
            _waitEvent.WaitOne();
            if (!AuthenticationFailed)
            {
                _connection.InitializeXmppStream();
                _connection.WaitForStreamFeatures();
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
        }
    }
}