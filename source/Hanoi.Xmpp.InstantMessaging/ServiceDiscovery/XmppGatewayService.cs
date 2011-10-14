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

using Hanoi.Serialization.InstantMessaging.Client;
using Hanoi.Serialization.InstantMessaging.Register;

namespace Hanoi.Xmpp.InstantMessaging.ServiceDiscovery {
    /// <summary>
    ///   XMPP Gateway service
    /// </summary>
    public class XmppGatewayService
        : XmppService {
        private XmppGatewayType type;

        /// <summary>
        ///   Initiazes a new instance of the <see cref = "XmppGatewayService">XmppGatewayService</see> class.
        /// </summary>
        /// <param name = "session"></param>
        /// <param name = "serviceId"></param>
        public XmppGatewayService(Session session, string serviceId)
            : base(session, serviceId) {
            InferGatewayType();
        }

        /// <summary>
        ///   Gets the gateway type
        /// </summary>
        public XmppGatewayType Type {
            get { return type; }
        }

        /// <summary>
        ///   Sets the initial presence agains the XMPP Service.
        /// </summary>
        public void SetInitialPresence() {
            Session.Presence.SetInitialPresence(Identifier);
        }

        /// <summary>
        ///   Performs the gateway registration process
        /// </summary>
        /// <param name = "username"></param>
        /// <param name = "password"></param>
        public void Register(string username, string password) {
            var query = new RegisterQuery();
            var iq = new IQ();

            iq.ID = XmppIdentifierGenerator.Generate();
            iq.Type = IQType.Set;
            iq.From = Session.UserId;
            iq.To = Identifier;

            iq.Items.Add(query);

            query.UserName = username;
            query.Password = password;

            PendingMessages.Add(iq.ID);

            Session.Send(iq);
        }

        /// <summary>
        ///   Performs the gateway unregistration process
        /// </summary>
        public void Unregister() {
            var query = new RegisterQuery();
            var iq = new IQ();

            iq.ID = XmppIdentifierGenerator.Generate();
            iq.Type = IQType.Set;
            iq.From = Session.UserId;
            iq.To = Identifier;

            iq.Items.Add(query);

            query.Remove = "";

            PendingMessages.Add(iq.ID);

            Session.Send(iq);
        }

        private void InferGatewayType() {
            if (Identifier.BareIdentifier.StartsWith("aim"))
            {
                type = XmppGatewayType.Aim;
            }
            else if (Identifier.BareIdentifier.StartsWith("facebook"))
            {
                type = XmppGatewayType.Facebook;
            }
            else if (Identifier.BareIdentifier.StartsWith("gadugadu"))
            {
                type = XmppGatewayType.GaduGadu;
            }
            else if (Identifier.BareIdentifier.StartsWith("gtalk"))
            {
                type = XmppGatewayType.GTalk;
            }
            else if (Identifier.BareIdentifier.StartsWith("http-ws"))
            {
                type = XmppGatewayType.HttpWs;
            }
            else if (Identifier.BareIdentifier.StartsWith("icq"))
            {
                type = XmppGatewayType.Icq;
            }
            else if (Identifier.BareIdentifier.StartsWith("lcs"))
            {
                type = XmppGatewayType.Lcs;
            }
            else if (Identifier.BareIdentifier.StartsWith("mrim"))
            {
                type = XmppGatewayType.Mrim;
            }
            else if (Identifier.BareIdentifier.StartsWith("msn"))
            {
                type = XmppGatewayType.Msn;
            }
            else if (Identifier.BareIdentifier.StartsWith("myspaceim"))
            {
                type = XmppGatewayType.MySpaceIm;
            }
            else if (Identifier.BareIdentifier.StartsWith("ocs"))
            {
                type = XmppGatewayType.Ocs;
            }
            else if (Identifier.BareIdentifier.StartsWith("qq"))
            {
                type = XmppGatewayType.QQ;
            }
            else if (Identifier.BareIdentifier.StartsWith("sametime"))
            {
                type = XmppGatewayType.Sametime;
            }
            else if (Identifier.BareIdentifier.StartsWith("simple"))
            {
                type = XmppGatewayType.Simple;
            }
            else if (Identifier.BareIdentifier.StartsWith("skype"))
            {
                type = XmppGatewayType.Skype;
            }
            else if (Identifier.BareIdentifier.StartsWith("sms"))
            {
                type = XmppGatewayType.Sms;
            }
            else if (Identifier.BareIdentifier.StartsWith("smtp"))
            {
                type = XmppGatewayType.Smtp;
            }
            else if (Identifier.BareIdentifier.StartsWith("tlen"))
            {
                type = XmppGatewayType.Tlen;
            }
            else if (Identifier.BareIdentifier.StartsWith("xfire"))
            {
                type = XmppGatewayType.Xfire;
            }
            else if (Identifier.BareIdentifier.StartsWith("xmpp"))
            {
                type = XmppGatewayType.Xmpp;
            }
            else if (Identifier.BareIdentifier.StartsWith("yahoo"))
            {
                type = XmppGatewayType.Yahoo;
            }
        }
        }
}