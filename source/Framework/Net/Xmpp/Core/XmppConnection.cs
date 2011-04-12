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
using BabelIm.Net.Xmpp.Core.Transports;
using BabelIm.Net.Xmpp.Serialization.Core.ResourceBinding;
using BabelIm.Net.Xmpp.Serialization.Core.Sasl;
using BabelIm.Net.Xmpp.Serialization.Core.Streams;
using BabelIm.Net.Xmpp.Serialization.Extensions.PubSub;
using BabelIm.Net.Xmpp.Serialization.Extensions.ServiceDiscovery;
using BabelIm.Net.Xmpp.Serialization.Extensions.VCard;
using BabelIm.Net.Xmpp.Serialization.Extensions.XmppPing;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client.Presence;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Roster;

namespace BabelIm.Net.Xmpp.Core
{
    /// <summary>
    /// Represents a connection to a XMPP server
    /// </summary>
    public sealed class XmppConnection
        : IDisposable
    {
        #region · Static Methods ·

        /// <summary>
        /// Checks if the given XMPP message instance is an error message
        /// </summary>
        /// <param name="message">The XMPP Message instance</param>
        /// <returns><b>true</b> if it's an error message; otherwise <b>false</b></returns>
        private static bool IsErrorMessage(object message)
        {
            bool isError = false;

            if (message is IQ)
            {
                isError = (((IQ)message).Type == IQType.Error);
            }
            else if (message is StreamError)
            {
                isError = true;
            }
            else if (message is Xmpp.Serialization.Core.Sasl.Failure)
            {
                isError = true;
            }

            return isError;
        }

        #endregion

        #region · Events ·

        /// <summary>
        /// Occurs when the authentication fails
        /// </summary>
        public event EventHandler<XmppAuthenticationFailiureEventArgs> AuthenticationFailiure;

        /// <summary>
        /// Occurs before the connection to the XMPP Server is closed.
        /// </summary>
        public event EventHandler ConnectionClosing;

        /// <summary>
        /// Occurs when the Connection to the XMPP Server is already closed.
        /// </summary>
        public event EventHandler ConnectionClosed;
        
        #endregion

        #region · Internal Events ·

        /// <summary>
        /// Occurs when a new message received from the XMPP server has no handler.
        /// </summary>
        internal event EventHandler<XmppUnhandledMessageEventArgs> UnhandledMessage;

        /// <summary>
        /// Occurs when a sasl failiure occurs.
        /// </summary>
        internal event EventHandler<XmppAuthenticationFailiureEventArgs> AuthenticationError;

        #endregion

        #region · Fields ·

        private XmppConnectionString	connectionString;
        private XmppConnectionState		state;
        private XmppStreamFeatures		streamFeatures;
        private XmppJid                 userId;
        private AutoResetEvent          initializedStreamEvent;
        private AutoResetEvent          streamFeaturesEvent;
        private AutoResetEvent          bindResourceEvent;
        private ITransport              transport;
        private bool                    isDisposed;

        #region · Observable Subscriptions ·
        
        private IDisposable transportMessageSubscription;
        private IDisposable transportStreamInitializedSubscription;
        private IDisposable transportStreamClosedSubscription;

        #endregion

        #region · Subject Fields ·

        private Subject<XmppMessage>        onMessageReceived           = new Subject<XmppMessage>();
        private Subject<IQ>                 onInfoQueryMessage          = new Subject<IQ>();
        private Subject<IQ>                 onServiceDiscoveryMessage   = new Subject<IQ>();
        private Subject<IQ>                 onVCardMessage              = new Subject<IQ>();
        private Subject<RosterQuery>        onRosterMessage             = new Subject<RosterQuery>();
        private Subject<Presence>           onPresenceMessage           = new Subject<Presence>();
        private Subject<XmppEventMessage>   onEventMessage              = new Subject<XmppEventMessage>();
        
        #endregion

        #endregion

        #region · Observable Properties ·

        /// <summary>
        /// Occurs when a new message is received.
        /// </summary>
        public IObservable<XmppMessage> OnMessageReceived
        {
            get { return this.onMessageReceived.AsObservable(); }
        }

        /// <summary>
        /// Occurs when an info/query message is recevied
        /// </summary>
        public IObservable<IQ> OnInfoQueryMessage
        {
            get { return this.onInfoQueryMessage.AsObservable(); }
        }

        /// <summary>
        /// Occurs when a Service Discovery message is received from XMPP Server
        /// </summary>
        public IObservable<IQ> OnServiceDiscoveryMessage
        {
            get { return this.onServiceDiscoveryMessage.AsObservable(); }
        }

        /// <summary>
        /// Occurs when an vCard message is received from the XMPP Server
        /// </summary>
        public IObservable<IQ> OnVCardMessage
        {
            get { return this.onVCardMessage.AsObservable(); }
        }

        /// <summary>
        /// Occurs when a roster notification message is recevied
        /// </summary>
        public IObservable<RosterQuery> OnRosterMessage
        {
            get { return this.onRosterMessage.AsObservable(); }
        }

        /// <summary>
        /// Occurs when a presence message is received
        /// </summary>
        public IObservable<Presence> OnPresenceMessage
        {
            get { return this.onPresenceMessage.AsObservable(); }
        }

        /// <summary>
        /// Occurs when a event message ( pub-sub event ) is received
        /// </summary>
        public IObservable<XmppEventMessage> OnEventMessage
        {
            get { return this.onEventMessage.AsObservable(); }
        }

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the connection string used on authentication.
        /// </summary>
        public string ConnectionString
        {
            get 
            {
                if (this.connectionString == null)
                {
                    return null;
                }

                return this.connectionString.ToString(); 
            }
        }

        /// <summary>
        /// Gets the current state of the connection.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The connection is not open.
        /// </exception>
        public XmppConnectionState State
        {
            get { return this.state; }
        }

        /// <summary>
        /// Gets the connection Host name
        /// </summary>
        public string HostName
        {
            get 
            { 
                if (this.transport == null)
                {
                    return String.Empty;
                }

                return this.transport.HostName; 
            }
        }

        /// <summary>
        /// Gets the User ID specified in the Connection String.
        /// </summary>
        public XmppJid UserId
        {
            get  { return this.userId; }
        }

        #endregion

        #region · Internal Properties ·

        /// <summary>
        /// Gets the password specified in the Connection string.
        /// </summary>
        /// <remarks>
        /// Needed for <see cref="XmppAuthenticator"/> implementations.
        /// </remarks>
        internal string UserPassword
        {
            get 
            {
                if (this.connectionString == null)
                {
                    return String.Empty;
                }

                return this.connectionString.UserPassword; 
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="XmppConnection"/> class.
        /// </summary>
        public XmppConnection()
        {
        }

        #endregion
        
        #region · Finalizer ·

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="T:BabelIm.Net.Xmpp.Core.XmppConnection"/> is reclaimed by garbage collection.
        /// </summary>
        ~XmppConnection()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            this.Dispose(false);
        }

        #endregion

        #region · IDisposable Methods ·

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the specified disposing.
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> [disposing].</param>
        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    // Release managed resources here
                    this.Close();
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.
                this.onMessageReceived  = null;
                this.onInfoQueryMessage = null;
                this.onRosterMessage    = null;
                this.onPresenceMessage  = null;
                this.onVCardMessage     = null;
            }

            this.isDisposed = true;
        }

        #endregion

        #region · Methods ·

        /// <summary>
        /// Opens the connection
        /// </summary>
        /// <param name="connectionString">The connection string used for authentication.</param>
        public void Open(string connectionString)
        {
            if (this.state == XmppConnectionState.Open)
            {
                throw new XmppException("Connection must be closed first.");
            }

            try
            {
                // Initialization
                this.Initialize();

                // Build the connection string
                this.connectionString   = new XmppConnectionString(connectionString);
                this.userId             = this.connectionString.UserId;

                // Connect to the server
                if (this.connectionString.UseHttpBinding)
                {
                    this.transport = new HttpTransport();
                }
                else
                {
                    this.transport = new TcpTransport();
                }

                this.transportMessageSubscription = this.transport
                    .OnMessageReceived
                    .Subscribe(message => this.OnTransportMessageReceived(message));

                this.transportStreamInitializedSubscription = this.transport
                    .OnXmppStreamInitialized
                    .Subscribe(message => this.OnTransportXmppStreamInitialized(message));

                this.transportStreamClosedSubscription = this.transport
                    .OnXmppStreamClosed
                    .Subscribe(message => this.OnTransportXmppStreamClosed(message));

                this.transport.Open(this.connectionString);

                // Initialize XMPP Stream
                this.InitializeXmppStream();

                // Wait until we receive the Stream features
                this.WaitForStreamFeatures();

                if (this.transport is ISecureTransport)
                {
                    if (this.connectionString.PortNumber != 443 &&
                        this.connectionString.PortNumber != 5223)
                    {
                        if (this.SupportsFeature(XmppStreamFeatures.SecureConnection))
                        {
                            ((ISecureTransport)this.transport).OpenSecureConnection();

                            // Wait until we receive the Stream features
                            this.WaitForStreamFeatures();
                        }
                    }
                }

                // Perform authentication
                bool authenticationDone = this.Authenticate();

                if (authenticationDone)
                {
                    // Resource Binding.
                    this.BindResource();

                    // Open the session
                    this.OpenSession();

                    // Update state
                    this.state = XmppConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                if (this.AuthenticationFailiure != null)
                {
                    this.AuthenticationFailiure(this, new XmppAuthenticationFailiureEventArgs(ex.ToString()));
                }

                this.Close();
            }
        }

        /// <summary>
        /// Closes the connection
        /// </summary>
        public void Close()
        {
            if (this.state != XmppConnectionState.Closed)
            {
                if (this.ConnectionClosing != null)
                {
                    this.ConnectionClosing(this, new EventArgs());
                }

                try
                {
                    this.state = XmppConnectionState.Closing;

                    if (this.transport != null)
                    { 
                        this.transport.Close();
                    }                    
                }
                catch
                {
                }
                finally
                {
                    if (this.initializedStreamEvent != null)
                    { 
                        this.initializedStreamEvent.Set();
                        this.initializedStreamEvent = null;
                    }

                    if (this.streamFeaturesEvent != null)
                    { 
                        this.streamFeaturesEvent.Set();
                        this.streamFeaturesEvent = null;
                    }

                    if (this.bindResourceEvent != null)
                    { 
                        this.bindResourceEvent.Set();
                        this.bindResourceEvent = null;
                    }

                    if (this.initializedStreamEvent != null)
                    {
                        this.initializedStreamEvent.Close();
                        this.initializedStreamEvent = null;
                    }

                    if (this.streamFeaturesEvent != null)
                    {
                        this.streamFeaturesEvent.Close();
                        this.streamFeaturesEvent = null;
                    }

                    if (this.bindResourceEvent != null)
                    {
                        this.bindResourceEvent.Close();
                        this.bindResourceEvent = null;
                    }
                    
                    if (this.transportMessageSubscription != null)
                    {
                        this.transportMessageSubscription.Dispose();
                        this.transportMessageSubscription = null;
                    }

                    if (this.transportStreamInitializedSubscription != null)
                    {
                        this.transportStreamInitializedSubscription.Dispose();
                        this.transportStreamInitializedSubscription = null;
                    }

                    if (this.transportStreamClosedSubscription != null)
                    {
                        this.transportStreamClosedSubscription.Dispose();
                        this.transportStreamClosedSubscription = null;
                    }

                    if (this.transport != null)
                    {
                        this.transport = null;
                    }

                    this.streamFeatures         = this.streamFeatures & (~this.streamFeatures);
                    this.state                  = XmppConnectionState.Closed;
                    this.connectionString		= null;
                    this.userId                 = null;
                }

                if (this.ConnectionClosed != null)
                {
                    this.ConnectionClosed(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Sends a new message.
        /// </summary>
        /// <param elementname="message">The message to be sent</param>
        public void Send(object message)
        {
            this.transport.Send(message);
        }

        /// <summary>
        /// Sends an XMPP message string to the XMPP Server
        /// </summary>
        /// <param name="value"></param>
        public void Send(string value)
        {
            this.transport.Send(value);
        }

        /// <summary>
        /// Sends an XMPP message buffer to the XMPP Server
        /// </summary>
        public void Send(byte[] buffer)
        {
            this.transport.Send(buffer);
        }

        #endregion

        #region · Stream Initialization ·

        /// <summary>
        /// Initializes the XMPP stream.
        /// </summary>
        internal void InitializeXmppStream()
        {
            this.transport.InitializeXmppStream();

            this.initializedStreamEvent.WaitOne();
        }

        /// <summary>
        /// Waits until the stream:features message is received
        /// </summary>
        internal void WaitForStreamFeatures()
        {
            this.streamFeaturesEvent.WaitOne();
        }

        #endregion

        #region · Message Handling ·

        /// <summary>
        /// Process a XMPP message instance
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private AutoResetEvent ProcessMessageInstance(object message)
        {
            if (message != null)
            {
                if (XmppConnection.IsErrorMessage(message))
                {
                    return this.ProcessErrorMessage(message);
                }
                else if (message is IQ)
                {
                    this.ProcessQueryMessage(message as IQ);
                }
                else if (message is Presence)
                {
                    this.ProcessPresenceMessage(message as Presence);
                }
                else if (message is Message)
                {
                    this.ProcessMessage(message as Message);
                }
                else if (message is StreamFeatures)
                {
                    this.ProcessStreamFeatures(message as StreamFeatures);
                }
                else if (this.UnhandledMessage != null)
                {
                    this.UnhandledMessage(this, new XmppUnhandledMessageEventArgs(message));
                }
            }

            return null;
        }

        /// <summary>
        /// Process an XMPP Error message
        /// </summary>
        /// <param name="message"></param>
        private AutoResetEvent ProcessErrorMessage(object message)
        {
            if (message is IQ)
            {
                this.onInfoQueryMessage.OnNext(message as IQ);
            }
            else if (message is StreamError)
            {
                throw new XmppException((StreamError)message);
            }
            else if (message is Failure)
            {
                if (this.AuthenticationError != null)
                {
                    string errorMessage = String.Format("SASL Authentication Failed ({0})", ((Failure)message).FailiureType);

                    this.AuthenticationError(this, new XmppAuthenticationFailiureEventArgs(errorMessage));
                }
            }
            else if (message is PubSubErrorUnsupported)
            {
#warning TODO: Process PubSub Error
            }

            return null;
        }

        /// <summary>
        /// Process an Stream Features XMPP message
        /// </summary>
        /// <param name="features"></param>
        private void ProcessStreamFeatures(StreamFeatures features)
        {
            if (features.Mechanisms != null && features.Mechanisms.SaslMechanisms.Count > 0)
            {
                foreach (string mechanism in features.Mechanisms.SaslMechanisms)
                {
                    switch (mechanism)
                    {
                        case XmppCodes.SaslDigestMD5Mechanism:
                            this.streamFeatures |= XmppStreamFeatures.SaslDigestMD5;
                            break;

                        case XmppCodes.SaslPlainMechanism:
                            this.streamFeatures |= XmppStreamFeatures.SaslPlain;
                            break;

                        case XmppCodes.SaslXGoogleTokenMechanism:
                            this.streamFeatures |= XmppStreamFeatures.XGoogleToken;
                            break;
                    }
                }
            }

            if (features.Bind != null)
            {
                this.streamFeatures |= XmppStreamFeatures.ResourceBinding;
            }

            if (features.SessionSpecified)
            {
                this.streamFeatures |= XmppStreamFeatures.Sessions;
            }

            if (features.Items.Count > 0)
            {
                foreach (object item in features.Items)
                {
                    if (item is BabelIm.Net.Xmpp.Serialization.InstantMessaging.RegisterIQ)
                    {
                        this.streamFeatures |= XmppStreamFeatures.InBandRegistration;
                    }
                }
            }

            this.streamFeaturesEvent.Set();
        }

        /// <summary>
        /// Process an XMPP Message
        /// </summary>
        /// <param name="message"></param>
        private void ProcessMessage(Message message)
        {
            if (message.Items.Count > 0 && 
                message.Items[0] is PubSubEvent)
            {
                this.onEventMessage.OnNext(new XmppEventMessage(message));
            }
            else
            {
                this.onMessageReceived.OnNext(new XmppMessage(message));
            }
        }

        /// <summary>
        /// Process an XMPP IQ message
        /// </summary>
        /// <param name="iq"></param>
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
                            this.userId = ((Bind)item).Jid;

                            this.bindResourceEvent.Set();
                        }
                        else if (item is RosterQuery)
                        {
                            this.onRosterMessage.OnNext(item as RosterQuery);
                        }
                        else if (item is VCardData)
                        {
                            this.onVCardMessage.OnNext(iq);
                        }
                        else if (item is Ping)
                        {
                            if (iq.Type == IQType.Get)
                            {
                                // Send the "pong" response
                                this.Send
                                (
                                    new IQ
                                    {
                                        ID      = iq.ID,
                                        To      = iq.From,
                                        From    = this.UserId.ToString(),
                                        Type    = IQType.Result
                                    }
                                );
                            }
                        }
                    }

                    if (item is ServiceQuery || item is ServiceItemQuery)
                    {
                        this.onServiceDiscoveryMessage.OnNext(iq);
                    }
                }
            }
        }

        /// <summary>
        /// Processes the presence message.
        /// </summary>
        /// <param name="presence">The presence.</param>
        /// <returns></returns>
        private bool ProcessPresenceMessage(Presence presence)
        {
            this.onPresenceMessage.OnNext(presence);

            return true;
        }

        #endregion

        #region · Authentication ·

        private bool Authenticate()
        {
            XmppAuthenticator   authenticator   = null;
            bool                result          = false;
            
            try
            {
                authenticator = this.CreateAuthenticator();

                if (authenticator != null)
                {
                    authenticator.Authenticate();

                    if (authenticator.AuthenticationFailed)
                    {
                        if (this.AuthenticationFailiure != null)
                        {
                            this.AuthenticationFailiure(this, new XmppAuthenticationFailiureEventArgs(authenticator.AuthenticationError));
                        }
                    }

                    result = !authenticator.AuthenticationFailed;
                }
                else
                {
                    if (this.AuthenticationFailiure != null)
                    {
                        this.AuthenticationFailiure(this, new XmppAuthenticationFailiureEventArgs("No valid authentication mechanism found."));
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

        #endregion

        #region · Feature negotiation ·

        private void BindResource()
        {
            if (this.SupportsFeature(XmppStreamFeatures.ResourceBinding))
            {
                Bind bind		= new Bind();
                bind.Resource	= this.UserId.ResourceName;

                IQ iq   = new IQ();
                iq.Type = IQType.Set;
                iq.ID   = XmppIdentifierGenerator.Generate();

                iq.Items.Add(bind);

                this.Send(iq);

                this.bindResourceEvent.WaitOne();
            }
        }

        private void OpenSession()
        {
            if (this.SupportsFeature(XmppStreamFeatures.Sessions))
            {
                IQ iq = new IQ();
            
                iq.Type = IQType.Set;
                iq.To   = this.connectionString.HostName;
                iq.ID   = XmppIdentifierGenerator.Generate();

                iq.Items.Add(new Session());

                this.Send(iq);
            }
        }

        #endregion

        #region · Connection Manager Event Handlers ·

        private void OnTransportXmppStreamInitialized(string stanza)
        {
            // Wait until we have the stream:stream element readed
            this.initializedStreamEvent.Set();
        }

        private void OnTransportXmppStreamClosed(string stanza)
        {
            if (this.ConnectionClosed != null)
            {
                this.ConnectionClosed(this, EventArgs.Empty);
            }
        }

        private void OnTransportMessageReceived(object message)
        {
            AutoResetEvent resetEvent = this.ProcessMessageInstance(message);

            if (resetEvent != null)
            {
                resetEvent.Set();
            }
        }

        #endregion

        #region · Private Methods ·

        /// <summary>
        /// Initializes the connection instance
        /// </summary>
        private void Initialize()
        {
            this.state                  = XmppConnectionState.Opening;
            this.initializedStreamEvent = new AutoResetEvent(false);
            this.streamFeaturesEvent    = new AutoResetEvent(false);
            this.bindResourceEvent      = new AutoResetEvent(false);
        }

        /// <summary>
        /// Creates an instance of the <see cref="XmppAuthenticator"/> used by the connection.
        /// </summary>
        /// <returns></returns>
        private XmppAuthenticator CreateAuthenticator()
        {
            XmppAuthenticator authenticator = null;

            if (this.SupportsFeature(XmppStreamFeatures.SaslDigestMD5))
            {
                authenticator = new XmppSaslDigestAuthenticator(this);
            }
            else if (this.SupportsFeature(XmppStreamFeatures.XGoogleToken))
            {
                authenticator = new XmppSaslXGoogleTokenAuthenticator(this);
            }
            else if (this.SupportsFeature(XmppStreamFeatures.SaslPlain))
            {
                authenticator = new XmppSaslPlainAuthenticator(this);
            }

            return authenticator;
        }

        /// <summary>
        /// Checks if a speficic stream feature is supported by the XMPP server.
        /// </summary>
        /// <param elementname="feature">Feature to check.</param>
        /// <returns><b>true</b> if the feature is supported by the server; or <b>false</b> if not</returns>
        private bool SupportsFeature(XmppStreamFeatures feature)
        {
            return ((this.streamFeatures & feature) == feature);
        }

        #endregion
    }
}
