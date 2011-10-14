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
using Hanoi.Core.Authentication;
using Hanoi.Core.Transports;
using Hanoi.Xmpp.Serialization.Core.ResourceBinding;
using Hanoi.Xmpp.Serialization.Core.Sasl;
using Hanoi.Xmpp.Serialization.Core.Streams;
using Hanoi.Xmpp.Serialization.Extensions.PubSub;
using Hanoi.Xmpp.Serialization.Extensions.ServiceDiscovery;
using Hanoi.Xmpp.Serialization.Extensions.VCardTemp;
using Hanoi.Xmpp.Serialization.Extensions.XmppPing;
using Hanoi.Xmpp.Serialization.InstantMessaging.Client;
using Hanoi.Xmpp.Serialization.InstantMessaging.Presence;
using Hanoi.Xmpp.Serialization.InstantMessaging.Register;
using Hanoi.Xmpp.Serialization.InstantMessaging.Roster;

namespace Hanoi.Core {
    /// <summary>
    ///   Represents a connection to a XMPP server
    /// </summary>
    public sealed class XmppConnection
        : IDisposable {
        private readonly Subject<XmppEventMessage> onEventMessage = new Subject<XmppEventMessage>();
        private readonly Subject<IQ> onServiceDiscoveryMessage = new Subject<IQ>();
        private AutoResetEvent bindResourceEvent;
        private XmppConnectionString connectionString;
        private AutoResetEvent initializedStreamEvent;
        private bool isDisposed;
        private Subject<IQ> onInfoQueryMessage = new Subject<IQ>();
        private Subject<XmppMessage> onMessageReceived = new Subject<XmppMessage>();
        private Subject<Presence> onPresenceMessage = new Subject<Presence>();
        private Subject<RosterQuery> onRosterMessage = new Subject<RosterQuery>();
        private Subject<IQ> onVCardMessage = new Subject<IQ>();
        private XmppConnectionState state;
        private XmppStreamFeatures streamFeatures;
        private AutoResetEvent streamFeaturesEvent;
        private ITransport transport;

        private IDisposable transportMessageSubscription;
        private IDisposable transportStreamClosedSubscription;
        private IDisposable transportStreamInitializedSubscription;
        private XmppJid userId;

        /// <summary>
        ///   Occurs when a new message is received.
        /// </summary>
        public IObservable<XmppMessage> OnMessageReceived {
            get { return onMessageReceived.AsObservable(); }
        }

        /// <summary>
        ///   Occurs when an info/query message is recevied
        /// </summary>
        public IObservable<IQ> OnInfoQueryMessage {
            get { return onInfoQueryMessage.AsObservable(); }
        }

        /// <summary>
        ///   Occurs when a Service Discovery message is received from XMPP Server
        /// </summary>
        public IObservable<IQ> OnServiceDiscoveryMessage {
            get { return onServiceDiscoveryMessage.AsObservable(); }
        }

        /// <summary>
        ///   Occurs when an vCard message is received from the XMPP Server
        /// </summary>
        public IObservable<IQ> OnVCardMessage {
            get { return onVCardMessage.AsObservable(); }
        }

        /// <summary>
        ///   Occurs when a roster notification message is recevied
        /// </summary>
        public IObservable<RosterQuery> OnRosterMessage {
            get { return onRosterMessage.AsObservable(); }
        }

        /// <summary>
        ///   Occurs when a presence message is received
        /// </summary>
        public IObservable<Presence> OnPresenceMessage {
            get { return onPresenceMessage.AsObservable(); }
        }

        /// <summary>
        ///   Occurs when a event message ( pub-sub event ) is received
        /// </summary>
        public IObservable<XmppEventMessage> OnEventMessage {
            get { return onEventMessage.AsObservable(); }
        }

        /// <summary>
        ///   Gets the connection string used on authentication.
        /// </summary>
        public string ConnectionString {
            get {
                if (connectionString == null)
                {
                    return null;
                }

                return connectionString.ToString();
            }
        }

        /// <summary>
        ///   Gets the current state of the connection.
        /// </summary>
        /// <exception cref = "InvalidOperationException">
        ///   The connection is not open.
        /// </exception>
        public XmppConnectionState State {
            get { return state; }
        }

        /// <summary>
        ///   Gets the connection Host name
        /// </summary>
        public string HostName {
            get {
                if (transport == null)
                {
                    return String.Empty;
                }

                return transport.HostName;
            }
        }

        /// <summary>
        ///   Gets the User ID specified in the Connection String.
        /// </summary>
        public XmppJid UserId {
            get { return userId; }
        }

        /// <summary>
        ///   Gets the password specified in the Connection string.
        /// </summary>
        /// <remarks>
        ///   Needed for <see cref = "XmppAuthenticator" /> implementations.
        /// </remarks>
        internal string UserPassword {
            get {
                if (connectionString == null)
                {
                    return String.Empty;
                }

                return connectionString.UserPassword;
            }
        }

        #region IDisposable Members

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
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
        private static bool IsErrorMessage(object message) {
            bool isError = false;

            if (message is IQ)
            {
                isError = (((IQ) message).Type == IQType.Error);
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
        public event EventHandler<XmppAuthenticationFailiureEventArgs> AuthenticationFailiure;

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
        internal event EventHandler<XmppUnhandledMessageEventArgs> UnhandledMessage;

        /// <summary>
        ///   Occurs when a sasl failiure occurs.
        /// </summary>
        internal event EventHandler<XmppAuthenticationFailiureEventArgs> AuthenticationError;

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref = "T:Hanoi.Core.XmppConnection" /> is reclaimed by garbage collection.
        /// </summary>
        ~XmppConnection() {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        /// <summary>
        ///   Disposes the specified disposing.
        /// </summary>
        /// <param name = "disposing">if set to <c>true</c> [disposing].</param>
        private void Dispose(bool disposing) {
            if (!isDisposed)
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
                onMessageReceived = null;
                onInfoQueryMessage = null;
                onRosterMessage = null;
                onPresenceMessage = null;
                onVCardMessage = null;
            }

            isDisposed = true;
        }

        /// <summary>
        ///   Opens the connection
        /// </summary>
        /// <param name = "connectionString">The connection string used for authentication.</param>
        public void Open(string connectionString) {
            if (state == XmppConnectionState.Open)
            {
                throw new XmppException("Connection must be closed first.");
            }

            try
            {
                // Initialization
                Initialize();

                // Build the connection string
                this.connectionString = new XmppConnectionString(connectionString);
                userId = this.connectionString.UserId;

                // Connect to the server
                if (this.connectionString.UseHttpBinding)
                {
                    transport = new HttpTransport();
                }
                else
                {
                    transport = new TcpTransport();
                }

                transportMessageSubscription = transport
                    .OnMessageReceived
                    .Subscribe(message => OnTransportMessageReceived(message));

                transportStreamInitializedSubscription = transport
                    .OnXmppStreamInitialized
                    .Subscribe(message => OnTransportXmppStreamInitialized(message));

                transportStreamClosedSubscription = transport
                    .OnXmppStreamClosed
                    .Subscribe(message => OnTransportXmppStreamClosed(message));

                transport.Open(this.connectionString);

                // Initialize XMPP Stream
                InitializeXmppStream();

                // Wait until we receive the Stream features
                WaitForStreamFeatures();

                if (transport is ISecureTransport)
                {
                    if (this.connectionString.PortNumber != 443 &&
                        this.connectionString.PortNumber != 5223)
                    {
                        if (SupportsFeature(XmppStreamFeatures.SecureConnection))
                        {
                            ((ISecureTransport) transport).OpenSecureConnection();

                            // Wait until we receive the Stream features
                            WaitForStreamFeatures();
                        }
                    }
                }

                // Perform authentication
                bool authenticationDone = Authenticate();

                if (authenticationDone)
                {
                    // Resource Binding.
                    BindResource();

                    // Open the session
                    OpenSession();

                    // Update state
                    state = XmppConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                if (AuthenticationFailiure != null)
                {
                    AuthenticationFailiure(this, new XmppAuthenticationFailiureEventArgs(ex.ToString()));
                }

                Close();
            }
        }

        /// <summary>
        ///   Closes the connection
        /// </summary>
        public void Close() {
            if (state != XmppConnectionState.Closed)
            {
                if (ConnectionClosing != null)
                {
                    ConnectionClosing(this, new EventArgs());
                }

                try
                {
                    state = XmppConnectionState.Closing;

                    if (transport != null)
                    {
                        transport.Close();
                    }
                }
                catch
                {
                }
                finally
                {
                    if (initializedStreamEvent != null)
                    {
                        initializedStreamEvent.Set();
                        initializedStreamEvent = null;
                    }

                    if (streamFeaturesEvent != null)
                    {
                        streamFeaturesEvent.Set();
                        streamFeaturesEvent = null;
                    }

                    if (bindResourceEvent != null)
                    {
                        bindResourceEvent.Set();
                        bindResourceEvent = null;
                    }

                    if (initializedStreamEvent != null)
                    {
                        initializedStreamEvent.Close();
                        initializedStreamEvent = null;
                    }

                    if (streamFeaturesEvent != null)
                    {
                        streamFeaturesEvent.Close();
                        streamFeaturesEvent = null;
                    }

                    if (bindResourceEvent != null)
                    {
                        bindResourceEvent.Close();
                        bindResourceEvent = null;
                    }

                    if (transportMessageSubscription != null)
                    {
                        transportMessageSubscription.Dispose();
                        transportMessageSubscription = null;
                    }

                    if (transportStreamInitializedSubscription != null)
                    {
                        transportStreamInitializedSubscription.Dispose();
                        transportStreamInitializedSubscription = null;
                    }

                    if (transportStreamClosedSubscription != null)
                    {
                        transportStreamClosedSubscription.Dispose();
                        transportStreamClosedSubscription = null;
                    }

                    if (transport != null)
                    {
                        transport = null;
                    }

                    streamFeatures = streamFeatures & (~streamFeatures);
                    state = XmppConnectionState.Closed;
                    connectionString = null;
                    userId = null;
                }

                if (ConnectionClosed != null)
                {
                    ConnectionClosed(this, new EventArgs());
                }
            }
        }

        /// <summary>
        ///   Sends a new message.
        /// </summary>
        /// <param name = "message">The message to be sent</param>
        public void Send(object message) {
            transport.Send(message);
        }

        /// <summary>
        ///   Sends an XMPP message string to the XMPP Server
        /// </summary>
        /// <param name = "value"></param>
        public void Send(string value) {
            transport.Send(value);
        }

        /// <summary>
        ///   Sends an XMPP message buffer to the XMPP Server
        /// </summary>
        public void Send(byte[] buffer) {
            transport.Send(buffer);
        }

        /// <summary>
        ///   Initializes the XMPP stream.
        /// </summary>
        internal void InitializeXmppStream() {
            transport.InitializeXmppStream();

            initializedStreamEvent.WaitOne();
        }

        /// <summary>
        ///   Waits until the stream:features message is received
        /// </summary>
        internal void WaitForStreamFeatures() {
            streamFeaturesEvent.WaitOne();
        }

        /// <summary>
        ///   Process a XMPP message instance
        /// </summary>
        /// <param name = "message"></param>
        /// <returns></returns>
        private AutoResetEvent ProcessMessageInstance(object message) {
            if (message != null)
            {
                if (XmppConnection.IsErrorMessage(message))
                {
                    return ProcessErrorMessage(message);
                }
                else if (message is IQ)
                {
                    ProcessQueryMessage(message as IQ);
                }
                else if (message is Presence)
                {
                    ProcessPresenceMessage(message as Presence);
                }
                else if (message is Message)
                {
                    ProcessMessage(message as Message);
                }
                else if (message is StreamFeatures)
                {
                    ProcessStreamFeatures(message as StreamFeatures);
                }
                else if (UnhandledMessage != null)
                {
                    UnhandledMessage(this, new XmppUnhandledMessageEventArgs(message));
                }
            }

            return null;
        }

        /// <summary>
        ///   Process an XMPP Error message
        /// </summary>
        /// <param name = "message"></param>
        private AutoResetEvent ProcessErrorMessage(object message) {
            if (message is IQ)
            {
                onInfoQueryMessage.OnNext(message as IQ);
            }
            else if (message is StreamError)
            {
                throw new XmppException((StreamError) message);
            }
            else if (message is Failure)
            {
                if (AuthenticationError != null)
                {
                    string errorMessage = String.Format("SASL Authentication Failed ({0})",
                                                        ((Failure) message).FailiureType);

                    AuthenticationError(this, new XmppAuthenticationFailiureEventArgs(errorMessage));
                }
            }
            else if (message is PubSubErrorUnsupported)
            {
#warning TODO: Process PubSub Error
            }

            return null;
        }

        /// <summary>
        ///   Process an Stream Features XMPP message
        /// </summary>
        /// <param name = "features"></param>
        private void ProcessStreamFeatures(StreamFeatures features) {
            if (features.Mechanisms != null && features.Mechanisms.SaslMechanisms.Count > 0)
            {
                foreach (string mechanism in features.Mechanisms.SaslMechanisms)
                {
                    switch (mechanism)
                    {
                        case XmppCodes.SaslDigestMD5Mechanism:
                            streamFeatures |= XmppStreamFeatures.SaslDigestMD5;
                            break;

                        case XmppCodes.SaslPlainMechanism:
                            streamFeatures |= XmppStreamFeatures.SaslPlain;
                            break;

                        case XmppCodes.SaslXGoogleTokenMechanism:
                            streamFeatures |= XmppStreamFeatures.XGoogleToken;
                            break;
                    }
                }
            }

            if (features.Bind != null)
            {
                streamFeatures |= XmppStreamFeatures.ResourceBinding;
            }

            if (features.SessionSpecified)
            {
                streamFeatures |= XmppStreamFeatures.Sessions;
            }

            if (features.Items.Count > 0)
            {
                foreach (object item in features.Items)
                {
                    if (item is RegisterIQ)
                    {
                        streamFeatures |= XmppStreamFeatures.InBandRegistration;
                    }
                }
            }

            streamFeaturesEvent.Set();
        }

        /// <summary>
        ///   Process an XMPP Message
        /// </summary>
        /// <param name = "message"></param>
        private void ProcessMessage(Message message) {
            if (message.Items.Count > 0 &&
                message.Items[0] is PubSubEvent)
            {
                onEventMessage.OnNext(new XmppEventMessage(message));
            }
            else
            {
                onMessageReceived.OnNext(new XmppMessage(message));
            }
        }

        /// <summary>
        ///   Process an XMPP IQ message
        /// </summary>
        /// <param name = "iq"></param>
        private void ProcessQueryMessage(IQ iq) {
            if (iq.Items != null && iq.Items.Count > 0)
            {
                foreach (object item in iq.Items)
                {
                    if (iq.Type != IQType.Error)
                    {
                        if (item is Bind)
                        {
                            userId = ((Bind) item).Jid;

                            bindResourceEvent.Set();
                        }
                        else if (item is RosterQuery)
                        {
                            onRosterMessage.OnNext(item as RosterQuery);
                        }
                        else if (item is VCardData)
                        {
                            onVCardMessage.OnNext(iq);
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
                        onServiceDiscoveryMessage.OnNext(iq);
                    }
                }
            }
        }

        /// <summary>
        ///   Processes the presence message.
        /// </summary>
        /// <param name = "presence">The presence.</param>
        /// <returns></returns>
        private bool ProcessPresenceMessage(Presence presence) {
            onPresenceMessage.OnNext(presence);

            return true;
        }

        private bool Authenticate() {
            XmppAuthenticator authenticator = null;
            bool result = false;

            try
            {
                authenticator = CreateAuthenticator();

                if (authenticator != null)
                {
                    authenticator.Authenticate();

                    if (authenticator.AuthenticationFailed)
                    {
                        if (AuthenticationFailiure != null)
                        {
                            AuthenticationFailiure(this,
                                                   new XmppAuthenticationFailiureEventArgs(
                                                       authenticator.AuthenticationError));
                        }
                    }

                    result = !authenticator.AuthenticationFailed;
                }
                else
                {
                    if (AuthenticationFailiure != null)
                    {
                        AuthenticationFailiure(this,
                                               new XmppAuthenticationFailiureEventArgs(
                                                   "No valid authentication mechanism found."));
                    }
                    else
                    {
                        throw new XmppException("No valid authentication mechanism found.");
                    }
                }
            }
            catch
            {
                throw;
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

        private void BindResource() {
            if (SupportsFeature(XmppStreamFeatures.ResourceBinding))
            {
                var bind = new Bind();
                bind.Resource = UserId.ResourceName;

                var iq = new IQ();
                iq.Type = IQType.Set;
                iq.ID = XmppIdentifierGenerator.Generate();

                iq.Items.Add(bind);

                Send(iq);

                bindResourceEvent.WaitOne();
            }
        }

        private void OpenSession() {
            if (SupportsFeature(XmppStreamFeatures.Sessions))
            {
                var iq = new IQ();

                iq.Type = IQType.Set;
                iq.To = connectionString.HostName;
                iq.ID = XmppIdentifierGenerator.Generate();

                iq.Items.Add(new Session());

                Send(iq);
            }
        }

        private void OnTransportXmppStreamInitialized(string stanza) {
            // Wait until we have the stream:stream element readed
            initializedStreamEvent.Set();
        }

        private void OnTransportXmppStreamClosed(string stanza) {
            if (ConnectionClosed != null)
            {
                ConnectionClosed(this, EventArgs.Empty);
            }
        }

        private void OnTransportMessageReceived(object message) {
            AutoResetEvent resetEvent = ProcessMessageInstance(message);

            if (resetEvent != null)
            {
                resetEvent.Set();
            }
        }

        /// <summary>
        ///   Initializes the connection instance
        /// </summary>
        private void Initialize() {
            state = XmppConnectionState.Opening;
            initializedStreamEvent = new AutoResetEvent(false);
            streamFeaturesEvent = new AutoResetEvent(false);
            bindResourceEvent = new AutoResetEvent(false);
        }

        /// <summary>
        ///   Creates an instance of the <see cref = "XmppAuthenticator" /> used by the connection.
        /// </summary>
        /// <returns></returns>
        private XmppAuthenticator CreateAuthenticator() {
            XmppAuthenticator authenticator = null;

            if (SupportsFeature(XmppStreamFeatures.SaslDigestMD5))
            {
                authenticator = new XmppSaslDigestAuthenticator(this);
            }
            else if (SupportsFeature(XmppStreamFeatures.XGoogleToken))
            {
                authenticator = new XmppSaslXGoogleTokenAuthenticator(this);
            }
            else if (SupportsFeature(XmppStreamFeatures.SaslPlain))
            {
                authenticator = new XmppSaslPlainAuthenticator(this);
            }

            return authenticator;
        }

        /// <summary>
        ///   Checks if a speficic stream feature is supported by the XMPP server.
        /// </summary>
        /// <param elementname = "feature">Feature to check.</param>
        /// <returns><b>true</b> if the feature is supported by the server; or <b>false</b> if not</returns>
        private bool SupportsFeature(XmppStreamFeatures feature) {
            return ((streamFeatures & feature) == feature);
        }
        }
}