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
using System.Drawing;
using System.IO;
using System.Linq;
using BabelIm.Net.Xmpp.Core;
using BabelIm.Net.Xmpp.InstantMessaging.ServiceDiscovery;
using BabelIm.Net.Xmpp.Serialization.Extensions.EntityCapabilities;
using BabelIm.Net.Xmpp.Serialization.Extensions.ServiceDiscovery;
using BabelIm.Net.Xmpp.Serialization.Extensions.VCard;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client;
using BabelIm.Net.Xmpp.Serialization.InstantMessaging.Client.Presence;

namespace BabelIm.Net.Xmpp.InstantMessaging
{
    /// <summary>
    /// Represents a contact resource
    /// </summary>
    public sealed class XmppContactResource 
        : ObservableObject
    {
        #region · Static Members ·

        internal static int DefaultPresencePriorityValue = -200;

        #endregion

        #region · Fields ·

        private XmppSession				session;
        private XmppContact             contact;
        private XmppJid					resourceId;
        private XmppContactPresence     presence;
        private XmppClientCapabilities	capabilities;
        private string					avatarHash;
        private Stream					avatar;
        private List<string>			pendingMessages;

        #region · Subscriptions ·

        private IDisposable sessionStateSubscription;
        private IDisposable infoQueryErrorSubscription;
        private IDisposable serviceDiscoverySubscription;
        private IDisposable vCardSubscription;

        #endregion

        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets a value that indicates wheter this resource is the default resource
        /// </summary>
        /// <value>The resource id.</value>
        public bool IsDefaultResource
        {
            get { return (this.Presence.Priority == DefaultPresencePriorityValue); }
        }

        /// <summary>
        /// Gets or sets the resource id.
        /// </summary>
        /// <value>The resource id.</value>
        public XmppJid ResourceId
        {
            get { return this.resourceId; }
        }

        /// <summary>
        /// Gets or sets the resource presence information.
        /// </summary>
        /// <value>The presence.</value>
        public XmppContactPresence Presence
        {
            get { return this.presence; }
        }

        /// <summary>
        /// Gets or sets the resource capabilities.
        /// </summary>
        /// <value>The capabilities.</value>
        public XmppClientCapabilities Capabilities
        {
            get { return this.capabilities; }
            private set
            {
                if (this.capabilities != value)
                {
                    this.capabilities = value;
                    this.NotifyPropertyChanged(() => Capabilities);
                }
            }
        }

        /// <summary>
        /// Gets the original avatar image
        /// </summary>
        public Stream Avatar
        {
            get { return this.avatar; }
            private set
            {
                if (this.avatar != value)
                {
                    this.avatar = value;
                    this.NotifyPropertyChanged(() => Avatar);
                }
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="XmppContactResource"/> class.
        /// </summary>
        internal XmppContactResource(XmppSession session, XmppContact contact, XmppJid resourceId)
        {
            this.session    		= session;
            this.contact    		= contact;
            this.resourceId 		= resourceId;
            this.presence			= new XmppContactPresence(this.session);
            this.capabilities 		= new XmppClientCapabilities();
            this.pendingMessages	= new List<string>();

            this.Subscribe();
        }

        #endregion

        #region · Methods ·

        public override string ToString()
        {
            return this.resourceId.ToString();
        }

        #endregion

        #region · Internal Methods ·

        internal void Update(Presence presence)
        {
            this.Presence.Update(presence);

            if (this.IsDefaultResource && this.Presence.PresenceStatus == XmppPresenceState.Offline)
            {
                string cachedHash = this.session.AvatarStorage.GetAvatarHash(this.ResourceId.BareIdentifier);

                // Grab stored images for offline users
                if (!String.IsNullOrEmpty(cachedHash))
                {
                    // Dipose Avatar Streams
                    this.DisposeAvatarStream();

                    // Update the avatar hash and file Paths
                    this.avatarHash = cachedHash;
                    this.Avatar     = this.session.AvatarStorage.ReadAvatar(this.ResourceId.BareIdentifier);
                }
            }

            foreach (object item in presence.Items)
            {
                if (item is Error)
                {
#warning TODO: Handle the error
                }
                else if (item is VCardAvatar)
                {
                    VCardAvatar vcard = (VCardAvatar)item;

                    if (vcard.Photo != null && vcard.Photo.Length > 0)
                    {
                        if (!String.IsNullOrEmpty(vcard.Photo))
                        {
                            // Check if we have the avatar cached
                            string cachedHash = this.session.AvatarStorage.GetAvatarHash(this.ResourceId.BareIdentifier);

                            if (cachedHash == vcard.Photo)
                            {
                                // Dispose Avatar Streams
                                this.DisposeAvatarStream();

                                // Update the avatar hash and file Paths
                                this.avatarHash = vcard.Photo;                                
                                this.Avatar     = this.session.AvatarStorage.ReadAvatar(this.ResourceId.BareIdentifier);
                            }
                            else
                            {
                                // Update the avatar hash
                                this.avatarHash = vcard.Photo;

                                // Avatar is not cached request the new avatar information
                                this.RequestAvatar();
                            }
                        }
                    }
                }
                else if (item is EntityCapabilities)
                {                    
                    EntityCapabilities caps = (EntityCapabilities)item;

                    // Request capabilities only if they aren't cached yet for this resource
                    // or the verfiication string differs from the one that is cached
                    if (this.Capabilities == null || this.Capabilities.VerificationString != caps.VerificationString)
                    {
                        this.Capabilities.Node 					= caps.Node;
                        this.Capabilities.HashAlgorithmName 	= caps.HashAlgorithmName;
                        this.Capabilities.VerificationString 	= caps.VerificationString;
                        this.Capabilities.Identities.Clear();
                        this.Capabilities.Features.Clear();
                        
                        // Check if we have the capabilities in the storage
                        if (this.session.ClientCapabilitiesStorage.Exists(caps.Node, caps.VerificationString))
                        {
                            this.Capabilities = this.session.ClientCapabilitiesStorage.Get(caps.Node, caps.VerificationString);
                        }
                        else if ((this.contact.Subscription == XmppContactSubscriptionType.Both ||
                             this.contact.Subscription == XmppContactSubscriptionType.To) &&
                             (!presence.TypeSpecified || presence.Type == PresenceType.Unavailable))
                        {
                            // Discover Entity Capabilities Extension Features
                            this.DiscoverCapabilities();
                        }
                        
                        this.NotifyPropertyChanged(() => Capabilities);
                    }
                }
            }
        }

        #endregion

        #region · Private Methods ·

        private void DiscoverCapabilities()
        {
            IQ              requestIq   = new IQ();
            ServiceQuery    request     = new ServiceQuery();

            request.Node    = this.Capabilities.DiscoveryInfoNode;
            requestIq.From  = this.session.UserId;
            requestIq.ID    = XmppIdentifierGenerator.Generate();
            requestIq.To    = this.ResourceId;
            requestIq.Type  = IQType.Get;

            requestIq.Items.Add(request);

            this.pendingMessages.Add(requestIq.ID);

            this.session.Send(requestIq);
        }

        private void RequestAvatar()
        {
            if (this.contact.Subscription == XmppContactSubscriptionType.Both ||
                this.contact.Subscription == XmppContactSubscriptionType.To)
            {
                IQ iq = new IQ();

                iq.ID   = XmppIdentifierGenerator.Generate();
                iq.Type = IQType.Get;
                iq.To   = this.ResourceId;
                iq.From = this.session.UserId;

                iq.Items.Add(new VCardData());

                this.session.Send(iq);
            }
        }

        private void DisposeAvatarStream()
        {
            if (this.Avatar != null)
            {
                this.Avatar.Dispose();
                this.Avatar = null;
            }
        }

        #endregion

        #region · Message Subscriptions ·

        private void SubscribeToSessionState()
        {
            this.sessionStateSubscription = this.session
                .StateChanged
                .Where(s => s == XmppSessionState.LoggingOut)
                .Subscribe
            (
                newState =>
                {
                    this.DisposeAvatarStream();
                    this.Unsubscribe();
                }
            );
        }

        private void Subscribe()
        {
            this.SubscribeToSessionState();

            this.infoQueryErrorSubscription = this.session.Connection
                .OnInfoQueryMessage
                .Where(message => message.Type == IQType.Error)
                .Subscribe(message => this.OnQueryErrorMessage(message));

            this.serviceDiscoverySubscription = this.session.Connection
                .OnServiceDiscoveryMessage
                .Where(message => message.Type == IQType.Result && this.pendingMessages.Contains(message.ID))
                .Subscribe(message => this.OnServiceDiscoveryMessage(message));

            this.vCardSubscription = this.session.Connection
                .OnVCardMessage
                .Where(message => message.From == this.ResourceId)
                .Subscribe(message => this.OnVCardMessage(message));
        }

        private void Unsubscribe()
        {
            if (this.sessionStateSubscription != null)
            {
                this.sessionStateSubscription.Dispose();
                this.sessionStateSubscription = null;
            }

            if (this.infoQueryErrorSubscription != null)
            {
                this.infoQueryErrorSubscription.Dispose();
                this.infoQueryErrorSubscription = null;
            }

            if (this.serviceDiscoverySubscription != null)
            {
                this.serviceDiscoverySubscription.Dispose();
                this.serviceDiscoverySubscription = null;
            }

            if (this.vCardSubscription != null)
            {
                this.vCardSubscription.Dispose();
                this.vCardSubscription = null;
            }
        }
        
        #endregion        

        #region · Message Handlers ·

        private void OnServiceDiscoveryMessage(IQ message)
        {
            this.pendingMessages.Remove(message.ID);

            this.Capabilities.Identities.Clear();
            this.Capabilities.Features.Clear();

            // Reponse to our capabilities query
            foreach (object item in message.Items)
            {
                if (item is ServiceQuery)
                {
                    ServiceQuery query = (ServiceQuery)item;

                    foreach (ServiceIdentity identity in query.Identities)
                    {
                        this.Capabilities.Identities.Add
                        (
                            new XmppServiceIdentity(identity.Name, identity.Category, identity.Type)
                        );
                    }

                    foreach (ServiceFeature supportedFeature in query.Features)
                    {
                        this.Capabilities.Features.Add(new XmppServiceFeature(supportedFeature.Name));
                    }
                }
            }

            if (!this.session.ClientCapabilitiesStorage.Exists(this.capabilities.Node, this.capabilities.VerificationString))
            {
                this.session.ClientCapabilitiesStorage.ClientCapabilities.Add(this.Capabilities);
                this.session.ClientCapabilitiesStorage.Save();
            }

            this.NotifyPropertyChanged(() => Capabilities);
        }

        private void OnQueryErrorMessage(IQ message)
        {
            if (this.pendingMessages.Contains(message.ID))
            {
                this.pendingMessages.Remove(message.ID);
            }
        }        

        private void OnVCardMessage(IQ message)
        {
            // Update the Avatar image
            VCardData vCard = (VCardData)message.Items[0];

            if (vCard.Photo.Photo != null && vCard.Photo.Photo.Length > 0)
            {
                Image avatarImage = null;

                try
                {
                    this.DisposeAvatarStream();

                    using (MemoryStream avatarStream = new MemoryStream(vCard.Photo.Photo))
                    {
                        // En sure it's a valid image
                        avatarImage = Image.FromStream(avatarStream);

                        // Save avatar
                        if (avatarStream != null && avatarStream.Length > 0)
                        {
                            this.session.AvatarStorage.SaveAvatar(this.ResourceId.BareIdentifier, this.avatarHash, avatarStream);
                        }
                    }

                    this.Avatar = this.session.AvatarStorage.ReadAvatar(this.ResourceId.BareIdentifier);
                }
                catch
                {
#warning TODO: Handle the exception
                }
                finally
                {
                    if (avatarImage != null)
                    {
                        avatarImage.Dispose();
                        avatarImage = null;
                    }
                }
            }
            else
            {
                this.session.AvatarStorage.RemoveAvatar(this.ResourceId.BareIdentifier);
            }

            this.session.AvatarStorage.Save();
        }
        
        #endregion
    }
}