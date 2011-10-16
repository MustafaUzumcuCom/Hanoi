/*
    Copyright (c) 2007 - 2010, Carlos Guzmán Álvarez

    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, 
    are permitted provided that the following conditions are met:

        * Redistributions of source code must retain the above copyright notice, 
          this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, 
          this list of conditions and the following disclaimer in the documentation and/or 
          other materials provided with the distribution.
        * Neither the name of the author nor the names of its contributors may be used to endorse or 
          promote products derived from this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
    CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
    EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Hanoi.Authentication;
using Hanoi.Serialization.Core.ResourceBinding;
using Hanoi.Serialization.Core.Sasl;
using Hanoi.Serialization.Core.Streams;
using Hanoi.Serialization.Extensions.PubSub;
using Hanoi.Serialization.Extensions.ServiceDiscovery;
using Hanoi.Serialization.Extensions.VCardTemp;
using Hanoi.Serialization.Extensions.XmppPing;
using Hanoi.Serialization.InstantMessaging.Client;
using Hanoi.Serialization.InstantMessaging.Presence;
using Hanoi.Serialization.InstantMessaging.Roster;
using Hanoi.Transports;

namespace Hanoi
{
    // NOTE: unsealed this class for testing

    /// <summary>
    /// Represents a connection to a XMPP server
    /// </summary>
    public class Connection : IDisposable
    {
        private readonly Subject<EventMessage> _onEventMessage = new Subject<EventMessage>();
        private readonly Subject<IQ> _onServiceDiscoveryMessage = new Subject<IQ>();
        private ConnectionString _connectionString;
        private AutoResetEvent _bindResourceEvent;
        private AutoResetEvent _initializedStreamEvent;
        private bool _isDisposed;
        private Subject<IQ> _onInfoQueryMessage = new Subject<IQ>();
        private Subject<Message> _onMessageReceived = new Subject<Message>();
        private Subject<Presence> _onPresenceMessage = new Subject<Presence>();
        private Subject<RosterQuery> _onRosterMessage = new Subject<RosterQuery>();
        private Subject<IQ> _onVCardMessage = new Subject<IQ>();
        private StreamFeatures _streamFeatures;
        private AutoResetEvent _streamFeaturesEvent;
        private ITransport _transport;

        private IDisposable _transportMessageSubscription;
        private IDisposable _transportStreamClosedSubscription;
        private IDisposable _transportStreamInitializedSubscription;

        public Connection() : this(AuthenticatorFactory.Default, FeatureDetection.Default, ConnectionFactory.Default)
        {

        }

        public Connection(IAuthenticatorFactory authenticator, IFeatureDetection featureDetection, IConnectionFactory factory) {
            Authenticator = authenticator;
            Features = featureDetection;
            Factory = factory;
        }

        internal IConnectionFactory Factory { get; private set; }
        
        internal IAuthenticatorFactory Authenticator { get; private set; }

        internal IFeatureDetection Features { get; private set; }

        /// <summary>
        ///   Occurs when a new message is received.
        /// </summary>
        public IObservable<Message> OnMessageReceived
        {
            get { return _onMessageReceived.AsObservable(); }
        }

        /// <summary>
        ///   Occurs when an info/query message is recevied
        /// </summary>
        public IObservable<IQ> OnInfoQueryMessage
        {
            get { return _onInfoQueryMessage.AsObservable(); }
        }

        /// <summary>
        ///   Occurs when a Service Discovery message is received from XMPP Server
        /// </summary>
        public IObservable<IQ> OnServiceDiscoveryMessage
        {
            get { return _onServiceDiscoveryMessage.AsObservable(); }
        }

        /// <summary>
        ///   Occurs when an vCard message is received from the XMPP Server
        /// </summary>
        public IObservable<IQ> OnVCardMessage
        {
            get { return _onVCardMessage.AsObservable(); }
        }

        /// <summary>
        ///   Occurs when a roster notification message is recevied
        /// </summary>
        public IObservable<RosterQuery> OnRosterMessage
        {
            get { return _onRosterMessage.AsObservable(); }
        }

        /// <summary>
        ///   Occurs when a presence message is received
        /// </summary>
        public IObservable<Presence> OnPresenceMessage
        {
            get { return _onPresenceMessage.AsObservable(); }
        }

        /// <summary>
        ///   Occurs when a event message ( pub-sub event ) is received
        /// </summary>
        public IObservable<EventMessage> OnEventMessage
        {
            get { return _onEventMessage.AsObservable(); }
        }

        /// <summary>
        ///   Gets the connection string used on authentication.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    return null;
                }

                return _connectionString.ToString();
            }
        }

        /// <summary>
        ///   Gets the current state of the connection.
        /// </summary>
        /// <exception cref = "InvalidOperationException">
        ///   The connection is not open.
        /// </exception>
        public ConnectionState State { get; private set; }

        /// <summary>
        ///   Gets the connection Host name
        /// </summary>
        public string HostName
        {
            get
            {
                if (_transport == null)
                {
                    return String.Empty;
                }

                return _transport.HostName;
            }
        }

        /// <summary>
        ///   Gets the User ID specified in the Connection String.
        /// </summary>
        public Jid UserId { get; private set; }

        /// <summary>
        ///   Gets the password specified in the Connection string.
        /// </summary>
        /// <remarks>
        ///   Needed for <see cref = "Authenticator" /> implementations.
        /// </remarks>
        internal string UserPassword
        {
            get
            {
                if (_connectionString == null)
                {
                    return String.Empty;
                }

                return _connectionString.UserPassword;
            }
        }

        #region IDisposable Members

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        ///   Checks if the given XMPP message instance is an error message
        /// </summary>
        /// <param name = "message">The XMPP Message instance</param>
        /// <returns><b>true</b> if it's an error message; otherwise <b>false</b></returns>
        private static bool IsErrorMessage(object message)
        {
            var isError = false;

            if (message is IQ)
            {
                isError = (((IQ)message).Type == IQType.Error);
            }
            else if (message is StreamError)
            {
                isError = true;
            }
            else if (message is Failure)
            {
                isError = true;
            }

            return isError;
        }

        /// <summary>
        ///   Occurs when the authentication fails
        /// </summary>
        public event EventHandler<AuthenticationFailiureEventArgs> AuthenticationFailiure;

        /// <summary>
        ///   Occurs before the connection to the XMPP Server is closed.
        /// </summary>
        public event EventHandler ConnectionClosing;

        /// <summary>
        ///   Occurs when the Connection to the XMPP Server is already closed.
        /// </summary>
        public event EventHandler ConnectionClosed;

        /// <summary>
        ///   Occurs when a new message received from the XMPP server has no handler.
        /// </summary>
        internal event EventHandler<UnhandledMessageEventArgs> UnhandledMessage;

        /// <summary>
        ///   Occurs when a sasl failiure occurs.
        /// </summary>
        internal event EventHandler<AuthenticationFailiureEventArgs> AuthenticationError;

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref = "T:Hanoi.Connection" /> is reclaimed by garbage collection.
        /// </summary>
        ~Connection()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        /// <summary>
        ///   Disposes the specified disposing.
        /// </summary>
        /// <param name = "disposing">if set to <c>true</c> [disposing].</param>
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // Release managed resources here
                    Close();
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.
                _onMessageReceived = null;
                _onInfoQueryMessage = null;
                _onRosterMessage = null;
                _onPresenceMessage = null;
                _onVCardMessage = null;
            }

            _isDisposed = true;
        }

        /// <summary>
        ///   Opens the connection
        /// </summary>
        /// <param name = "connectionString">The connection string used for authentication.</param>
        public void Open(string connectionString)
        {
            if (State == ConnectionState.Open)
            {
                throw new XmppException("Connection must be closed first.");
            }

            try
            {
                Initialize();

                _connectionString = new ConnectionString(connectionString);
                UserId = _connectionString.UserId;

                _transport = Factory.Create(_connectionString);

                _transportMessageSubscription = _transport
                    .OnMessageReceived
                    .Subscribe(OnTransportMessageReceived);

                _transportStreamInitializedSubscription = _transport
                    .OnXmppStreamInitialized
                    .Subscribe(message => OnTransportXmppStreamInitialized());

                _transportStreamClosedSubscription = _transport
                    .OnXmppStreamClosed
                    .Subscribe(OnTransportXmppStreamClosed);

                _transport.Open(_connectionString);

                InitializeXmppStream();

                WaitForStreamFeatures();

                if (_transport is ISecureTransport)
                {
                    if (_connectionString.PortNumber != 443 &&
                        _connectionString.PortNumber != 5223)
                    {
                        if (SupportsFeature(StreamFeatures.SecureConnection))
                        {
                            ((ISecureTransport)_transport).OpenSecureConnection();
                            WaitForStreamFeatures();
                        }
                    }
                }

                if (Authenticate())
                {
                    BindResource();
                    OpenSession();
                    State = ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                if (AuthenticationFailiure != null)
                {
                    AuthenticationFailiure(this, new AuthenticationFailiureEventArgs(ex.ToString()));
                }

                Close();
            }
        }

        /// <summary>
        ///   Closes the connection
        /// </summary>
        public void Close()
        {
            if (State == ConnectionState.Closed)
                return;

            if (ConnectionClosing != null)
            {
                ConnectionClosing(this, new EventArgs());
            }

            try
            {
                State = ConnectionState.Closing;

                if (_transport != null)
                {
                    _transport.Close();
                }
            }
            catch
            {
            }
            finally
            {
                if (_initializedStreamEvent != null)
                {
                    _initializedStreamEvent.Set();
                    _initializedStreamEvent = null;
                }

                if (_streamFeaturesEvent != null)
                {
                    _streamFeaturesEvent.Set();
                    _streamFeaturesEvent = null;
                }

                if (_bindResourceEvent != null)
                {
                    _bindResourceEvent.Set();
                    _bindResourceEvent = null;
                }

                if (_initializedStreamEvent != null)
                {
                    _initializedStreamEvent.Close();
                    _initializedStreamEvent = null;
                }

                if (_streamFeaturesEvent != null)
                {
                    _streamFeaturesEvent.Close();
                    _streamFeaturesEvent = null;
                }

                if (_bindResourceEvent != null)
                {
                    _bindResourceEvent.Close();
                    _bindResourceEvent = null;
                }

                if (_transportMessageSubscription != null)
                {
                    _transportMessageSubscription.Dispose();
                    _transportMessageSubscription = null;
                }

                if (_transportStreamInitializedSubscription != null)
                {
                    _transportStreamInitializedSubscription.Dispose();
                    _transportStreamInitializedSubscription = null;
                }

                if (_transportStreamClosedSubscription != null)
                {
                    _transportStreamClosedSubscription.Dispose();
                    _transportStreamClosedSubscription = null;
                }

                if (_transport != null)
                {
                    _transport = null;
                }

                _streamFeatures = _streamFeatures & (~_streamFeatures);
                State = ConnectionState.Closed;
                _connectionString = null;
                UserId = null;
            }

            if (ConnectionClosed != null)
            {
                ConnectionClosed(this, new EventArgs());
            }
        }

        /// <summary>
        ///   Sends a new message.
        /// </summary>
        /// <param name = "message">The message to be sent</param>
        public void Send(object message)
        {
            _transport.Send(message);
        }

        /// <summary>
        ///   Sends an XMPP message string to the XMPP Server
        /// </summary>
        /// <param name = "value"></param>
        public void Send(string value)
        {
            _transport.Send(value);
        }

        /// <summary>
        ///   Sends an XMPP message buffer to the XMPP Server
        /// </summary>
        public void Send(byte[] buffer)
        {
            _transport.Send(buffer);
        }

        /// <summary>
        ///   Initializes the XMPP stream.
        /// </summary>
        internal void InitializeXmppStream()
        {
            _transport.InitializeXmppStream();

            _initializedStreamEvent.WaitOne();
        }

        /// <summary>
        ///   Waits until the stream:features message is received
        /// </summary>
        internal void WaitForStreamFeatures()
        {
            _streamFeaturesEvent.WaitOne();
        }

        /// <summary>
        ///   Process a XMPP message instance
        /// </summary>
        /// <param name = "message"></param>
        /// <returns></returns>
        private AutoResetEvent ProcessMessageInstance(object message)
        {
            if (message != null)
            {
                if (IsErrorMessage(message))
                {
                    return ProcessErrorMessage(message);
                }
                if (message is IQ)
                {
                    ProcessQueryMessage(message as IQ);
                }
                else if (message is Presence)
                {
                    ProcessPresenceMessage(message as Presence);
                }
                else if (message is Serialization.InstantMessaging.Client.Message)
                {
                    ProcessMessage(message as Serialization.InstantMessaging.Client.Message);
                }
                else if (message is Serialization.Core.Streams.StreamFeatures)
                {
                    var features = message as Serialization.Core.Streams.StreamFeatures;
                    _streamFeatures = Features.Process(features);
                    return _streamFeaturesEvent;
                }
                else if (UnhandledMessage != null)
                {
                    UnhandledMessage(this, new UnhandledMessageEventArgs(message));
                }
            }

            return null;
        }

        /// <summary>
        ///   Process an XMPP Error message
        /// </summary>
        /// <param name = "message"></param>
        private AutoResetEvent ProcessErrorMessage(object message)
        {
            if (message is IQ)
            {
                _onInfoQueryMessage.OnNext(message as IQ);
            }
            else if (message is StreamError)
            {
                throw new XmppException((StreamError)message);
            }
            else if (message is Failure)
            {
                if (AuthenticationError != null)
                {
                    string errorMessage = String.Format("SASL Authentication Failed ({0})",
                                                        ((Failure)message).FailiureType);

                    AuthenticationError(this, new AuthenticationFailiureEventArgs(errorMessage));
                }
            }
            else if (message is PubSubErrorUnsupported)
            {
                //  TODO: Process PubSub Error
            }

            return null;
        }

        /// <summary>
        ///   Process an XMPP Message
        /// </summary>
        /// <param name = "message"></param>
        private void ProcessMessage(Serialization.InstantMessaging.Client.Message message)
        {
            if (message.Items.Count > 0 &&
                message.Items[0] is PubSubEvent)
            {
                _onEventMessage.OnNext(new EventMessage(message));
            }
            else
            {
                _onMessageReceived.OnNext(new Message(message));
            }
        }

        /// <summary>
        ///   Process an XMPP IQ message
        /// </summary>
        /// <param name = "iq"></param>
        private void ProcessQueryMessage(IQ iq)
        {
            if (iq.Items != null && iq.Items.Count > 0)
            {
                foreach (object item in iq.Items)
                {
                    if (iq.Type != IQType.Error)
                    {
                        if (item is Bind)
                        {
                            UserId = ((Bind)item).Jid;

                            _bindResourceEvent.Set();
                        }
                        else if (item is RosterQuery)
                        {
                            _onRosterMessage.OnNext(item as RosterQuery);
                        }
                        else if (item is VCardData)
                        {
                            _onVCardMessage.OnNext(iq);
                        }
                        else if (item is Ping)
                        {
                            if (iq.Type == IQType.Get)
                            {
                                // Send the "pong" response
                                Send
                                    (
                                        new IQ
                                            {
                                                ID = iq.ID,
                                                To = iq.From,
                                                From = UserId.ToString(),
                                                Type = IQType.Result
                                            }
                                    );
                            }
                        }
                    }

                    if (item is ServiceQuery || item is ServiceItemQuery)
                    {
                        _onServiceDiscoveryMessage.OnNext(iq);
                    }
                }
            }
        }

        /// <summary>
        ///   Processes the presence message.
        /// </summary>
        /// <param name = "presence">The presence.</param>
        /// <returns></returns>
        private bool ProcessPresenceMessage(Presence presence)
        {
            _onPresenceMessage.OnNext(presence);

            return true;
        }

        private bool Authenticate()
        {
            Authenticator authenticator = null;
            var result = false;

            try
            {
                authenticator = Authenticator.Create(_streamFeatures, this);

                if (authenticator != null)
                {
                    authenticator.Authenticate();

                    if (authenticator.AuthenticationFailed)
                    {
                        OnAuthenticationFailure(authenticator.AuthenticationError);
                    }

                    result = !authenticator.AuthenticationFailed;
                }
                else
                {
                    var message = "No valid authentication mechanism found.";
                    if (AuthenticationFailiure == null)
                    {
                        throw new XmppException(message);
                    }

                    OnAuthenticationFailure(message);
                }
            }
            finally
            {
                if (authenticator != null)
                {
                    authenticator.Dispose();
                    authenticator = null;
                }
            }

            return result;
        }

        private void OnAuthenticationFailure(string error)
        {
            if (AuthenticationFailiure != null)
            {
                AuthenticationFailiure(this, new AuthenticationFailiureEventArgs(error));
            }
        }

        private void BindResource()
        {
            if (!SupportsFeature(StreamFeatures.ResourceBinding)) 
                return;

            var bind = new Bind { Resource = UserId.ResourceName };

            var iq = new IQ
                         {
                             Type = IQType.Set,
                             ID = IdentifierGenerator.Generate()
                         };

            iq.Items.Add(bind);

            Send(iq);

            _bindResourceEvent.WaitOne();
        }

        private void OpenSession()
        {
            if (!SupportsFeature(StreamFeatures.Sessions)) 
                return;

            var iq = new IQ
                         {
                             Type = IQType.Set,
                             To = _connectionString.HostName,
                             ID = IdentifierGenerator.Generate()
                         };


            iq.Items.Add(new Session());

            Send(iq);
        }

        private void OnTransportXmppStreamInitialized()
        {
            // Wait until we have the stream:stream element readed
            _initializedStreamEvent.Set();
        }

        private void OnTransportXmppStreamClosed(string stanza)
        {
            if (ConnectionClosed != null)
            {
                ConnectionClosed(this, EventArgs.Empty);
            }
        }

        private void OnTransportMessageReceived(object message)
        {
            var resetEvent = ProcessMessageInstance(message);

            if (resetEvent != null)
            {
                resetEvent.Set();
            }
        }

        /// <summary>
        ///   Initializes the connection instance
        /// </summary>
        private void Initialize()
        {
            State = ConnectionState.Opening;
            _initializedStreamEvent = new AutoResetEvent(false);
            _streamFeaturesEvent = new AutoResetEvent(false);
            _bindResourceEvent = new AutoResetEvent(false);
        }

        /// <summary>
        ///   Checks if a speficic stream feature is supported by the XMPP server.
        /// </summary>
        /// <param elementname = "feature">Feature to check.</param>
        /// <param name="feature"></param>
        /// <returns><b>true</b> if the feature is supported by the server; or <b>false</b> if not</returns>
        /// 
        private bool SupportsFeature(StreamFeatures feature)
        {
            return ((_streamFeatures & feature) == feature);
        }
    }
}