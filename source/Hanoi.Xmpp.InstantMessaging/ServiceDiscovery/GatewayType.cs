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
    EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED2 TO,
    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

namespace Hanoi.Xmpp.InstantMessaging.ServiceDiscovery
{
    /// <summary>
    ///   XMPP Gateway types
    /// </summary>
    public enum GatewayType
    {
        /// <summary>
        ///   Gateway to AOL Instant Messenger
        /// </summary>
        Aim,

        /// <summary>
        ///   Gateway to the Facebook IM service
        /// </summary>
        Facebook,

        /// <summary>
        ///   Gateway to the Gadu-Gadu IM service
        /// </summary>
        GaduGadu,

        /// <summary>
        ///   Google talk
        /// </summary>
        GTalk,

        /// <summary>
        ///   Gateway that provides HTTP Web Services access
        /// </summary>
        HttpWs,

        /// <summary>
        ///   Gateway to ICQ
        /// </summary>
        Icq,

        /// <summary>
        ///   Gateway to Microsoft Live Communications Server
        /// </summary>
        Lcs,

        /// <summary>
        ///   Gateway to the mail.ru IM service
        /// </summary>
        Mrim,

        /// <summary>
        ///   Gateway to MSN Messenger
        /// </summary>
        Msn,

        /// <summary>
        ///   Gateway to the MySpace IM service
        /// </summary>
        MySpaceIm,

        /// <summary>
        ///   Gateway to Microsoft Office Communications Server
        /// </summary>
        Ocs,

        /// <summary>
        ///   Gateway to the QQ IM service
        /// </summary>
        QQ,

        /// <summary>
        ///   Gateway to IBM Lotus Sametime
        /// </summary>
        Sametime,

        /// <summary>
        ///   Gateway to SIP for Instant Messaging and Presence Leveraging Extensions (SIMPLE)
        /// </summary>
        Simple,

        /// <summary>
        ///   Gateway to the Skype service
        /// </summary>
        Skype,

        /// <summary>
        ///   Gateway to Short Message Service
        /// </summary>
        Sms,

        /// <summary>
        ///   Gateway to the SMTP (email) network
        /// </summary>
        Smtp,

        /// <summary>
        ///   Gateway to the Tlen IM service
        /// </summary>
        Tlen,

        /// <summary>
        ///   Gateway to the Xfire gaming and IM service
        /// </summary>
        Xfire,

        /// <summary>
        ///   Gateway to another XMPP service (NOT via native server-to-server communication)
        /// </summary>
        Xmpp,

        /// <summary>
        ///   Gateway to Yahoo! Instant Messenger
        /// </summary>
        Yahoo
    }
}