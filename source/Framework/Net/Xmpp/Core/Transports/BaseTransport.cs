using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using DnDns.Enums;
using DnDns.Query;
using DnDns.Records;

namespace BabelIm.Net.Xmpp.Core.Transports
{
    /// <summary>
    /// Base class for transport implementations
    /// </summary>
    internal abstract class BaseTransport
        : ITransport
    {
        #region · Fields ·

        private bool                    isDisposed;
        private XmppConnectionString    connectionString;
        private string                  hostName;
        private XmppJid                 userId;
        private object                  syncReads;
        private object                  syncWrites;

        #region · Observable Subjects ·

        private Subject<object> onMessageReceivedSubject        = new Subject<object>();
        private Subject<string> onXmppStreamInitializedSubject  = new Subject<string>();
        private Subject<string> onXmppStreamClosedSubject       = new Subject<string>();

        #endregion

        #endregion

        #region · Properties ·

        /// <summary>
        /// XMPP server Host name
        /// </summary>
        public string HostName
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(this.hostName))
                {
                    return this.hostName;
                }

                return this.connectionString.HostName;
            }
            protected set { this.hostName = value; }
        }

        #endregion

        #region · Observable Properties ·

        public IObservable<object> OnMessageReceived
        {
            get { return this.onMessageReceivedSubject.AsObservable(); }
        }

        public IObservable<string> OnXmppStreamInitialized
        {
            get { return this.onXmppStreamInitializedSubject.AsObservable(); }
        }

        public IObservable<string> OnXmppStreamClosed
        {
            get { return this.onXmppStreamClosedSubject.AsObservable(); }
        }

        #endregion

        #region · Protected Subject Properties ·

        protected Subject<object> OnMessageReceivedSubject
        {
            get { return this.onMessageReceivedSubject; }
        }

        protected Subject<string> OnXmppStreamInitializedSubject
        {
            get { return this.onXmppStreamInitializedSubject; }
        }

        protected Subject<string> OnXmppStreamClosedSubject
        {
            get { return this.onXmppStreamClosedSubject; }
        }

        #endregion

        #region · Protected Properties ·

        protected bool IsDisposed
        {
            get { return this.isDisposed; }
        }
        
        protected XmppConnectionString ConnectionString
        {
            get { return this.connectionString; }
            set { this.connectionString = value; }
        }
        
        protected XmppJid UserId
        {
            get { return this.userId; }            
            set { this.userId = value; }
        }
        
        protected object SyncReads
        {
            get { return this.syncReads; }
        }

        protected object SyncWrites
        {
            get { return this.syncWrites; }
        }

        #endregion

        #region · Constructors ·

        protected BaseTransport()
        {
            this.syncReads  = new object();
            this.syncWrites = new object();
        }

        #endregion

        #region · Finalizer ·

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="T:BabelIm.Net.Xmpp.Core.XmppConnection"/> is reclaimed by garbage collection.
        /// </summary>
        ~BaseTransport()
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
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.Close();
                }

                this.userId             = null;
                this.connectionString   = null;
                this.syncReads          = null;
                this.syncWrites         = null;

                this.onMessageReceivedSubject       = null;
                this.onXmppStreamInitializedSubject = null;
                this.onXmppStreamClosedSubject      = null;

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.               
            }

            this.isDisposed = true;
        }

        #endregion

        #region · Methods ·

        public abstract void Open(XmppConnectionString connectionString);

        public abstract void InitializeXmppStream();

        public abstract void Send(string message);

        public abstract void Send(object message);

        public abstract void Send(byte[] message);

        public virtual void Close()
        {
            this.userId                         = null;
            this.connectionString               = null;
            this.syncReads                      = null;
            this.syncWrites                     = null;
            this.onMessageReceivedSubject       = null;
            this.onXmppStreamInitializedSubject = null;
            this.onXmppStreamClosedSubject      = null;
        }

        #endregion

        #region · DNS Lookup Methods ·

        protected void ResolveHostName()
        {
            try
            {
                DnsQueryRequest     request     = new DnsQueryRequest();
                DnsQueryResponse    response    = request.Resolve
                (
                    String.Format("{0}.{1}", XmppCodes.XmppSrvRecordPrefix, this.ConnectionString.HostName),
                    NsType.SRV,
                    NsClass.INET,
                    ProtocolType.Tcp
                );

                foreach (SrvRecord record in response.Answers.OfType<SrvRecord>())
                {
                    this.HostName = record.HostName;
                }
            }
            catch
            {
            }
        }

        #endregion
    }
}
