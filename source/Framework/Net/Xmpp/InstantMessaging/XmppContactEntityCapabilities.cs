/*
    Copyright (c) 2007 - 2009, Carlos Guzmán Álvarez

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
using Framework.Net.Xmpp.Core;
using Framework.Net.Xmpp.Serialization.Extensions.ServiceDiscovery;
using Framework.Net.Xmpp.Serialization.InstantMessaging.Client;

namespace Framework.Net.Xmpp.InstantMessaging
{
    /// <summary>
    /// Contact entity capabilities
    /// </summary>
    /// <remarks>XEP-0115</remarks>
    public sealed class XmppContactEntityCapabilities : XmppEntityCapabilities
    {
        #region · Fields ·

        private List<string>        pendingMessages;
        private XmppContactResource resource;
        
        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="XmppContactEntityCapabilities"/> class.
        /// </summary>
        internal XmppContactEntityCapabilities(XmppSession session, XmppContactResource resource)
            : base(session)
        {
            this.resource           = resource;
            this.pendingMessages    = new List<string>();
        }

        #endregion

        #region · Methods ·

        public void DiscoverCapabilities()
        {
            IQ              requestIq 	= new IQ();
            ServiceQuery    request 	= new ServiceQuery();

            request.Node    = this.DiscoveryInfoNode;
            requestIq.From  = this.Session.UserId.ToString();
            requestIq.ID    = XmppIdentifierGenerator.Generate();
            requestIq.To    = this.resource.ResourceId.ToString();
            requestIq.Type  = IQType.Get;
            
            requestIq.Items.Add(request);

            this.pendingMessages.Add(requestIq.ID);

            this.Session.Send(requestIq);
        }

        #endregion
        
        #region · Internal Methods ·
        
        internal void Fill(XmppClientCapabilities caps)
        {
        	this.VerificationString = caps.VerificationString;
        	this.Node				= caps.Node;
        	
            foreach (XmppServiceIdentity identity in caps.Identities)
            {
                this.Identities.Add(new XmppServiceIdentity(identity.Category, identity.Type));
            }

            foreach (XmppServiceFeature supportedFeature in caps.Features)
            {
                this.Features.Add(new XmppServiceFeature(supportedFeature.Name));
            }
        }
        
        #endregion

        #region · Protected Methods ·

        protected override void OnServiceDiscoveryMessageReceived(object sender, XmppServiceDiscoveryMessageEventArgs e)
        {
            if (this.pendingMessages.Contains(e.QueryResult.ID) &&
                e.QueryResult.Type == IQType.Result)
            {
                this.pendingMessages.Remove(e.QueryResult.ID);

                // Reponse to our capabilities query
                foreach (object item in e.QueryResult.Items)
                {
                    if (item is ServiceQuery)
                    {
                        ServiceQuery query = (ServiceQuery)item;

                        foreach (ServiceIdentity identity in query.Identities)
                        {
                            this.Identities.Add(new XmppServiceIdentity(identity.Category, identity.Type));
                        }

                        foreach (ServiceFeature supportedFeature in query.Features)
                        {
                            this.Features.Add(new XmppServiceFeature(supportedFeature.Name));
                        }
                    }
                }

                if (!this.Session.ClientCapabilitiesStorage.Exists(this.Node, this.VerificationString))
                {
                    this.Session.ClientCapabilitiesStorage.Add(this);
                    this.Session.ClientCapabilitiesStorage.Save();
                }
            }
        }

        protected override void OnQueryErrorReceived(object sender, XmppQueryErrorReceivedEventArgs e)
        {
            if (this.pendingMessages.Contains(e.IQError.ID))
            {
                this.pendingMessages.Remove(e.IQError.ID);
            }
        }

        #endregion
    }
}
