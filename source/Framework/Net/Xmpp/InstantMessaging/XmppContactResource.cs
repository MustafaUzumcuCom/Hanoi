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
using BabelIm.Net.Xmpp.InstantMessaging.EntityCaps;
using BabelIm.Net.Xmpp.InstantMessaging.ServiceDiscovery;
using Hanoi.Core;
using Hanoi.Serialization.Extensions.EntityCapabilities;
using Hanoi.Serialization.Extensions.VCardAvatars;
using Hanoi.Serialization.Extensions.VCardTemp;
using Hanoi.Serialization.InstantMessaging.Client;
using Hanoi.Xmpp;
using Hanoi.Xmpp.Serialization.Extensions.ServiceDiscovery;
using Presence = Hanoi.Serialization.InstantMessaging.Presence.Presence;
using PresenceType = Hanoi.Serialization.InstantMessaging.Presence.PresenceType;

namespace BabelIm.Net.Xmpp.InstantMessaging {
    /// <summary>
    ///   Represents a contact resource
    /// </summary>
    public sealed class XmppContactResource
        : ObservableObject {
        internal static int DefaultPresencePriorityValue = -200;

        private readonly XmppContact contact;
        private readonly List<string> pendingMessages;
        private readonly XmppContactPresence presence;
        private readonly XmppJid resourceId;
        private readonly XmppSession session;
        private Stream avatar;
        private string avatarHash;
        private XmppClientCapabilities capabilities;
        private IDisposable infoQueryErrorSubscription;
        private IDisposable serviceDiscoverySubscription;
        private IDisposable sessionStateSubscription;
        private IDisposable vCardSubscription;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppContactResource" /> class.
        /// </summary>
        internal XmppContactResource(XmppSession session, XmppContact contact, XmppJid resourceId) {
            this.session = session;
            this.contact = contact;
            this.resourceId = resourceId;
            presence = new XmppContactPresence(this.session);
            capabilities = new XmppClientCapabilities();
            pendingMessages = new List<string>();

            Subscribe();
        }

        /// <summary>
        ///   Gets a value that indicates wheter this resource is the default resource
        /// </summary>
        /// <value>The resource id.</value>
        public bool IsDefaultResource {
            get { return (Presence.Priority == DefaultPresencePriorityValue); }
        }

        /// <summary>
        ///   Gets or sets the resource id.
        /// </summary>
        /// <value>The resource id.</value>
        public XmppJid ResourceId {
            get { return resourceId; }
        }

        /// <summary>
        ///   Gets or sets the resource presence information.
        /// </summary>
        /// <value>The presence.</value>
        public XmppContactPresence Presence {
            get { return presence; }
        }

        /// <summary>
        ///   Gets or sets the resource capabilities.
        /// </summary>
        /// <value>The capabilities.</value>
        public XmppClientCapabilities Capabilities {
            get { return capabilities; }
            private set {
                if (capabilities != value)
                {
                    capabilities = value;
                    NotifyPropertyChanged(() => Capabilities);
                }
            }
        }

        /// <summary>
        ///   Gets the original avatar image
        /// </summary>
        public Stream Avatar {
            get { return avatar; }
            private set {
                if (avatar != value)
                {
                    avatar = value;
                    NotifyPropertyChanged(() => Avatar);
                }
            }
        }

        public override string ToString() {
            return resourceId.ToString();
        }

        internal void Update(Presence presence) {
            Presence.Update(presence);

            if (IsDefaultResource && Presence.PresenceStatus == XmppPresenceState.Offline)
            {
                string cachedHash = session.AvatarStorage.GetAvatarHash(ResourceId.BareIdentifier);

                // Grab stored images for offline users
                if (!String.IsNullOrEmpty(cachedHash))
                {
                    // Dipose Avatar Streams
                    DisposeAvatarStream();

                    // Update the avatar hash and file Paths
                    avatarHash = cachedHash;
                    Avatar = session.AvatarStorage.ReadAvatar(ResourceId.BareIdentifier);
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
                    var vcard = (VCardAvatar) item;

                    if (vcard.Photo != null && vcard.Photo.Length > 0)
                    {
                        if (!String.IsNullOrEmpty(vcard.Photo))
                        {
                            // Check if we have the avatar cached
                            string cachedHash = session.AvatarStorage.GetAvatarHash(ResourceId.BareIdentifier);

                            if (cachedHash == vcard.Photo)
                            {
                                // Dispose Avatar Streams
                                DisposeAvatarStream();

                                // Update the avatar hash and file Paths
                                avatarHash = vcard.Photo;
                                Avatar = session.AvatarStorage.ReadAvatar(ResourceId.BareIdentifier);
                            }
                            else
                            {
                                // Update the avatar hash
                                avatarHash = vcard.Photo;

                                // Avatar is not cached request the new avatar information
                                RequestAvatar();
                            }
                        }
                    }
                }
                else if (item is EntityCapabilities)
                {
                    var caps = (EntityCapabilities) item;

                    // Request capabilities only if they aren't cached yet for this resource
                    // or the verfiication string differs from the one that is cached
                    if (Capabilities == null || Capabilities.VerificationString != caps.VerificationString)
                    {
                        Capabilities.Node = caps.Node;
                        Capabilities.HashAlgorithmName = caps.HashAlgorithmName;
                        Capabilities.VerificationString = caps.VerificationString;
                        Capabilities.Identities.Clear();
                        Capabilities.Features.Clear();

                        // Check if we have the capabilities in the storage
                        if (session.ClientCapabilitiesStorage.Exists(caps.Node, caps.VerificationString))
                        {
                            Capabilities = session.ClientCapabilitiesStorage.Get(caps.Node, caps.VerificationString);
                        }
                        else if ((contact.Subscription == XmppContactSubscriptionType.Both ||
                                  contact.Subscription == XmppContactSubscriptionType.To) &&
                                 (!presence.TypeSpecified || presence.Type == PresenceType.Unavailable))
                        {
                            // Discover Entity Capabilities Extension Features
                            DiscoverCapabilities();
                        }

                        NotifyPropertyChanged(() => Capabilities);
                    }
                }
            }
        }

        private void DiscoverCapabilities() {
            var requestIq = new IQ();
            var request = new ServiceQuery();

            request.Node = Capabilities.DiscoveryInfoNode;
            requestIq.From = session.UserId;
            requestIq.ID = XmppIdentifierGenerator.Generate();
            requestIq.To = ResourceId;
            requestIq.Type = IQType.Get;

            requestIq.Items.Add(request);

            pendingMessages.Add(requestIq.ID);

            session.Send(requestIq);
        }

        private void RequestAvatar() {
            if (contact.Subscription == XmppContactSubscriptionType.Both ||
                contact.Subscription == XmppContactSubscriptionType.To)
            {
                var iq = new IQ();

                iq.ID = XmppIdentifierGenerator.Generate();
                iq.Type = IQType.Get;
                iq.To = ResourceId;
                iq.From = session.UserId;

                iq.Items.Add(new VCardData());

                session.Send(iq);
            }
        }

        private void DisposeAvatarStream() {
            if (Avatar != null)
            {
                Avatar.Dispose();
                Avatar = null;
            }
        }

        private void SubscribeToSessionState() {
            sessionStateSubscription = session
                .StateChanged
                .Where(s => s == XmppSessionState.LoggingOut)
                .Subscribe
                (
                    newState =>
                        {
                            DisposeAvatarStream();
                            Unsubscribe();
                        }
                );
        }

        private void Subscribe() {
            SubscribeToSessionState();

            infoQueryErrorSubscription = session.Connection
                .OnInfoQueryMessage
                .Where(message => message.Type == IQType.Error)
                .Subscribe(message => OnQueryErrorMessage(message));

            serviceDiscoverySubscription = session.Connection
                .OnServiceDiscoveryMessage
                .Where(message => message.Type == IQType.Result && pendingMessages.Contains(message.ID))
                .Subscribe(message => OnServiceDiscoveryMessage(message));

            vCardSubscription = session.Connection
                .OnVCardMessage
                .Where(message => message.From == ResourceId)
                .Subscribe(message => OnVCardMessage(message));
        }

        private void Unsubscribe() {
            if (sessionStateSubscription != null)
            {
                sessionStateSubscription.Dispose();
                sessionStateSubscription = null;
            }

            if (infoQueryErrorSubscription != null)
            {
                infoQueryErrorSubscription.Dispose();
                infoQueryErrorSubscription = null;
            }

            if (serviceDiscoverySubscription != null)
            {
                serviceDiscoverySubscription.Dispose();
                serviceDiscoverySubscription = null;
            }

            if (vCardSubscription != null)
            {
                vCardSubscription.Dispose();
                vCardSubscription = null;
            }
        }

        private void OnServiceDiscoveryMessage(IQ message) {
            pendingMessages.Remove(message.ID);

            Capabilities.Identities.Clear();
            Capabilities.Features.Clear();

            // Reponse to our capabilities query
            foreach (object item in message.Items)
            {
                if (item is ServiceQuery)
                {
                    var query = (ServiceQuery) item;

                    foreach (ServiceIdentity identity in query.Identities)
                    {
                        Capabilities.Identities.Add
                            (
                                new XmppServiceIdentity(identity.Name, identity.Category, identity.Type)
                            );
                    }

                    foreach (ServiceFeature supportedFeature in query.Features)
                    {
                        Capabilities.Features.Add(new XmppServiceFeature(supportedFeature.Name));
                    }
                }
            }

            if (!session.ClientCapabilitiesStorage.Exists(capabilities.Node, capabilities.VerificationString))
            {
                session.ClientCapabilitiesStorage.ClientCapabilities.Add(Capabilities);
                session.ClientCapabilitiesStorage.Save();
            }

            NotifyPropertyChanged(() => Capabilities);
        }

        private void OnQueryErrorMessage(IQ message) {
            if (pendingMessages.Contains(message.ID))
            {
                pendingMessages.Remove(message.ID);
            }
        }

        private void OnVCardMessage(IQ message) {
            // Update the Avatar image
            var vCard = (VCardData) message.Items[0];

            if (vCard.Photo.Photo != null && vCard.Photo.Photo.Length > 0)
            {
                Image avatarImage = null;

                try
                {
                    DisposeAvatarStream();

                    using (var avatarStream = new MemoryStream(vCard.Photo.Photo))
                    {
                        // En sure it's a valid image
                        avatarImage = Image.FromStream(avatarStream);

                        // Save avatar
                        if (avatarStream != null && avatarStream.Length > 0)
                        {
                            session.AvatarStorage.SaveAvatar(ResourceId.BareIdentifier, avatarHash, avatarStream);
                        }
                    }

                    Avatar = session.AvatarStorage.ReadAvatar(ResourceId.BareIdentifier);
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
                session.AvatarStorage.RemoveAvatar(ResourceId.BareIdentifier);
            }

            session.AvatarStorage.Save();
        }
        }
}