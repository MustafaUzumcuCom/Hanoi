using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using DnDns.Enums;
using DnDns.Query;
using DnDns.Records;

namespace Hanoi.Transports {
    /// <summary>
    ///   Base class for transport implementations
    /// </summary>
    internal abstract class BaseTransport : ITransport {
        private ConnectionString connectionString;
        private string hostName;

        private Subject<object> onMessageReceivedSubject = new Subject<object>();
        private Subject<string> onXmppStreamClosedSubject = new Subject<string>();
        private Subject<string> onXmppStreamInitializedSubject = new Subject<string>();
        private Jid userId;

        protected BaseTransport() {
            SyncReads = new object();
            SyncWrites = new object();
        }

        protected Subject<object> OnMessageReceivedSubject {
            get { return onMessageReceivedSubject; }
        }

        protected Subject<string> OnXmppStreamInitializedSubject {
            get { return onXmppStreamInitializedSubject; }
        }

        protected Subject<string> OnXmppStreamClosedSubject {
            get { return onXmppStreamClosedSubject; }
        }

        protected bool IsDisposed { get; private set; }

        protected ConnectionString ConnectionString {
            get { return connectionString; }
            set { connectionString = value; }
        }

        protected Jid UserId {
            get { return userId; }
            set { userId = value; }
        }

        protected object SyncReads { get; private set; }

        protected object SyncWrites { get; private set; }

        #region ITransport Members

        /// <summary>
        ///   XMPP server Host name
        /// </summary>
        public string HostName {
            get {
                if (!String.IsNullOrWhiteSpace(hostName))
                {
                    return hostName;
                }

                return connectionString.HostName;
            }
            protected set { hostName = value; }
        }

        public IObservable<object> OnMessageReceived {
            get { return onMessageReceivedSubject.AsObservable(); }
        }

        public IObservable<string> OnXmppStreamInitialized {
            get { return onXmppStreamInitializedSubject.AsObservable(); }
        }

        public IObservable<string> OnXmppStreamClosed {
            get { return onXmppStreamClosedSubject.AsObservable(); }
        }

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

        public abstract void Open(ConnectionString connectionString);

        public abstract void InitializeXmppStream();

        public abstract void Send(string message);

        public abstract void Send(object message);

        public abstract void Send(byte[] message);

        public virtual void Close() {
            userId = null;
            connectionString = null;
            SyncReads = null;
            SyncWrites = null;
            onMessageReceivedSubject = null;
            onXmppStreamInitializedSubject = null;
            onXmppStreamClosedSubject = null;
        }

        #endregion

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref = "T:Hanoi.Connection" /> is reclaimed by garbage collection.
        /// </summary>
        ~BaseTransport() {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        /// <summary>
        ///   Disposes the specified disposing.
        /// </summary>
        /// <param name = "disposing">if set to <c>true</c> [disposing].</param>
        protected virtual void Dispose(bool disposing) {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    Close();
                }

                userId = null;
                connectionString = null;
                SyncReads = null;
                SyncWrites = null;

                onMessageReceivedSubject = null;
                onXmppStreamInitializedSubject = null;
                onXmppStreamClosedSubject = null;

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.               
            }

            IsDisposed = true;
        }

        protected void ResolveHostName() {
            try
            {
                var request = new DnsQueryRequest();
                var response = request.Resolve
                    (
                        String.Format("{0}.{1}", StreamCodes.XmppSrvRecordPrefix, ConnectionString.HostName),
                        NsType.SRV,
                        NsClass.INET,
                        ProtocolType.Tcp
                    );

                foreach (var record in response.Answers.OfType<SrvRecord>())
                {
                    HostName = record.HostName;
                }
            }
            catch // TODO: logging of exceptions?
            {
            }
        }
    }
}