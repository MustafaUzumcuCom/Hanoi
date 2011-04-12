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

using BabelIm.Net.Xmpp.Core;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Register;

namespace BabelIm.Net.Xmpp.InstantMessaging.ServiceDiscovery
{
    /// <summary>
    /// XMPP Gateway service
    /// </summary>
    public class XmppGatewayService
        : XmppService
    {
        #region · Fields ·
        
        private XmppGatewayType type;
        
        #endregion
        
        #region · Properties ·
        
        /// <summary>
        /// Gets the gateway type
        /// </summary>
        public XmppGatewayType Type
        {
            get { return this.type; }
        }
        
        #endregion
        
        #region · Constructors ·
        
        /// <summary>
        /// Initiazes a new instance of the <see cref="XmppGatewayService">XmppGatewayService</see> class.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="serviceId"></param>
        public XmppGatewayService(XmppSession session, string  serviceId)
            : base(session, serviceId)
        {
            this.InferGatewayType();
        }
        
        #endregion
        
        #region · Methods ·

        /// <summary>
        /// Sets the initial presence agains the XMPP Service.
        /// </summary>
        public void SetInitialPresence()
        {
            this.Session.Presence.SetInitialPresence(this.Identifier);
        }

        /// <summary>
        /// Performs the gateway registration process
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void Register(string username, string password)
        {
            RegisterQuery 	query 	= new RegisterQuery();
            IQ 				iq 		= new IQ();

            iq.ID   = XmppIdentifierGenerator.Generate();
            iq.Type = IQType.Set;
            iq.From = this.Session.UserId;
            iq.To   = this.Identifier;

            iq.Items.Add(query);

            query.UserName = username;
            query.Password = password;

            this.PendingMessages.Add(iq.ID);

            this.Session.Send(iq);
        }

        /// <summary>
        /// Performs the gateway unregistration process
        /// </summary>
        public void Unregister()
        {
            RegisterQuery 	query 	= new RegisterQuery();
            IQ 				iq 		= new IQ();

            iq.ID   = XmppIdentifierGenerator.Generate();
            iq.Type = IQType.Set;
            iq.From = this.Session.UserId;
            iq.To   = this.Identifier;

            iq.Items.Add(query);

            query.Remove = "";

            this.PendingMessages.Add(iq.ID);

            this.Session.Send(iq);
        }

        #endregion
        
        #region · Private Methods ·
        
        private void InferGatewayType()
        {
            if (this.Identifier.BareIdentifier.StartsWith("aim"))
            {
                this.type = XmppGatewayType.Aim;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("facebook"))
            {
                this.type = XmppGatewayType.Facebook;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("gadugadu"))
            {
                this.type = XmppGatewayType.GaduGadu;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("gtalk"))
            {
                this.type = XmppGatewayType.GTalk;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("http-ws"))
            {
                this.type = XmppGatewayType.HttpWs;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("icq"))
            {
                this.type = XmppGatewayType.Icq;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("lcs"))
            {
                this.type = XmppGatewayType.Lcs;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("mrim"))
            {
                this.type = XmppGatewayType.Mrim;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("msn"))
            {
                this.type = XmppGatewayType.Msn;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("myspaceim"))
            {
                this.type = XmppGatewayType.MySpaceIm;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("ocs"))
            {
                this.type = XmppGatewayType.Ocs;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("qq"))
            {
                this.type = XmppGatewayType.QQ;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("sametime"))
            {
                this.type = XmppGatewayType.Sametime;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("simple"))
            {
                this.type = XmppGatewayType.Simple;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("skype"))
            {
                this.type = XmppGatewayType.Skype;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("sms"))
            {
                this.type = XmppGatewayType.Sms;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("smtp"))
            {
                this.type = XmppGatewayType.Smtp;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("tlen"))
            {
                this.type = XmppGatewayType.Tlen;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("xfire"))
            {
                this.type = XmppGatewayType.Xfire;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("xmpp"))
            {
                this.type = XmppGatewayType.Xmpp;
            }
            else if (this.Identifier.BareIdentifier.StartsWith("yahoo"))
            {
                this.type = XmppGatewayType.Yahoo;
            }
        }
                
        #endregion
    }
}
