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
using Hanoi.Serialization.Extensions.ServiceDiscovery;
using Hanoi.Serialization.Extensions.VCardAvatars;
using Hanoi.Serialization.Extensions.VCardTemp;
using Hanoi.Serialization.InstantMessaging.Client;
using Hanoi.Serialization.InstantMessaging.Presence;
using Hanoi.Xmpp.InstantMessaging.EntityCaps;
using EntityCapabilities = Hanoi.Serialization.Extensions.EntityCapabilities.EntityCapabilities;
using ServiceFeature = Hanoi.Xmpp.InstantMessaging.ServiceDiscovery.ServiceFeature;
using ServiceIdentity = Hanoi.Xmpp.InstantMessaging.ServiceDiscovery.ServiceIdentity;

namespace Hanoi.Xmpp.InstantMessaging
{
    public sealed class ContactResource
    {
        internal static int DefaultPresencePriorityValue = -200;
        private readonly Contact _contact;
        private readonly List<string> _pendingMessages;
        private readonly ContactPresence _presence;
        private readonly Jid _resourceId;
        private readonly Session _session;
        private Stream _avatar;
        private string _avatarHash;
        private ClientCapabilities _capabilities;
        private IDisposable _infoQueryErrorSubscription;
        private IDisposable _serviceDiscoverySubscription;
        private IDisposable _sessionStateSubscription;
        private IDisposable _vCardSubscription;

        internal ContactResource(Session session, Contact contact, Jid resourceId)
        {
            _session = session;
            _contact = contact;
            _resourceId = resourceId;
            _presence = new ContactPresence(_session, contact);
            _capabilities = new ClientCapabilities();
            _pendingMessages = new List<string>();

            Subscribe();
        }

        /// <summary>
        ///   Gets a value that indicates wheter this resource is the default resource
        /// </summary>
        /// <value>The resource id.</value>
        public bool IsDefaultResource
        {
            get { return (Presence.Priority == DefaultPresencePriorityValue); }
        }

        /// <summary>
        ///   Gets or sets the resource id.
        /// </summary>
        /// <value>The resource id.</value>
        public Jid ResourceId
        {
            get { return _resourceId; }
        }

        /// <summary>
        ///   Gets or sets the resource presence information.
        /// </summary>
        /// <value>The presence.</value>
        public ContactPresence Presence
        {
            get { return _presence; }
        }

        /// <summary>
        ///   Gets or sets the resource capabilities.
        /// </summary>
        /// <value>The capabilities.</value>
        public ClientCapabilities Capabilities
        {
            get { return _capabilities; }
            private set
            {
                if (_capabilities != value)
                {
                    _capabilities = value;
                    // NotifyPropertyChanged(() => Capabilities);
                }
            }
        }

        /// <summary>
        ///   Gets the original avatar image
        /// </summary>
        public Stream Avatar
        {
            get { return _avatar; }
            private set
            {
                if (_avatar != value)
                {
                    _avatar = value;
                    // NotifyPropertyChanged(() => Avatar);
                }
            }
        }

        public override string ToString()
        {
            return _resourceId.ToString();
        }

        internal void Update(Serialization.InstantMessaging.Presence.Presence presence)
        {
            Presence.Update(presence);

            if (IsDefaultResource && Presence.PresenceStatus == PresenceState.Offline)
            {
                string cachedHash = _session.AvatarStorage.GetAvatarHash(ResourceId.BareIdentifier);

                // Grab stored images for offline users
                if (!String.IsNullOrEmpty(cachedHash))
                {
                    // Dipose Avatar Streams
                    DisposeAvatarStream();

                    // Update the avatar hash and file Paths
                    _avatarHash = cachedHash;
                    Avatar = _session.AvatarStorage.ReadAvatar(ResourceId.BareIdentifier);
                }
            }

            foreach (object item in presence.Items)
            {
                if (item is Error)
                {
                    // TODO: Handle the error
                }
                else if (item is VCardAvatar)
                {
                    var vcard = (VCardAvatar)item;

                    if (!string.IsNullOrEmpty(vcard.Photo))
                    {
                        if (!String.IsNullOrEmpty(vcard.Photo))
                        {
                            // Check if we have the avatar cached
                            string cachedHash = _session.AvatarStorage.GetAvatarHash(ResourceId.BareIdentifier);

                            if (cachedHash == vcard.Photo)
                            {
                                // Dispose Avatar Streams
                                DisposeAvatarStream();

                                // Update the avatar hash and file Paths
                                _avatarHash = vcard.Photo;
                                Avatar = _session.AvatarStorage.ReadAvatar(ResourceId.BareIdentifier);
                            }
                            else
                            {
                                // Update the avatar hash
                                _avatarHash = vcard.Photo;

                                // Avatar is not cached request the new avatar information
                                RequestAvatar();
                            }
                        }
                    }
                }
                else if (item is EntityCapabilities)
                {
                    var caps = (EntityCapabilities)item;

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
                        if (_session.ClientCapabilitiesStorage.Exists(caps.Node, caps.VerificationString))
                        {
                            Capabilities = _session.ClientCapabilitiesStorage.Get(caps.Node, caps.VerificationString);
                        }
                        else if ((_contact.Subscription == ContactSubscriptionType.Both ||
                                  _contact.Subscription == ContactSubscriptionType.To) &&
                                 (!presence.TypeSpecified || presence.Type == PresenceType.Unavailable))
                        {
                            // Discover Entity Capabilities Extension Features
                            DiscoverCapabilities();
                        }

                        // NotifyPropertyChanged(() => Capabilities);
                    }
                }
            }
        }

        private void DiscoverCapabilities()
        {
            var requestIq = new IQ();
            var request = new ServiceQuery
                              {
                                  Node = Capabilities.DiscoveryInfoNode
                              };

            requestIq.From = _session.UserId;
            requestIq.ID = IdentifierGenerator.Generate();
            requestIq.To = ResourceId;
            requestIq.Type = IQType.Get;
            requestIq.Items.Add(request);
            _pendingMessages.Add(requestIq.ID);
            _session.Send(requestIq);
        }

        private void RequestAvatar()
        {
            if (_contact.Subscription == ContactSubscriptionType.Both ||
                _contact.Subscription == ContactSubscriptionType.To)
            {
                var iq = new IQ
                             {
                                 ID = IdentifierGenerator.Generate(),
                                 Type = IQType.Get,
                                 To = ResourceId,
                                 From = _session.UserId
                             };
                
                iq.Items.Add(new VCardData());

                _session.Send(iq);
            }
        }

        private void DisposeAvatarStream()
        {
            if (Avatar == null) 
                return;

            Avatar.Dispose();
            Avatar = null;
        }

        private void SubscribeToSessionState()
        {
            _sessionStateSubscription = _session
                .StateChanged
                .Where(s => s == SessionState.LoggingOut)
                .Subscribe
                (
                    newState =>
                    {
                        DisposeAvatarStream();
                        Unsubscribe();
                    }
                );
        }

        private void Subscribe()
        {
            SubscribeToSessionState();

            _infoQueryErrorSubscription = _session.Connection
                .OnInfoQueryMessage
                .Where(message => message.Type == IQType.Error)
                .Subscribe(message => OnQueryErrorMessage(message));

            _serviceDiscoverySubscription = _session.Connection
                .OnServiceDiscoveryMessage
                .Where(message => message.Type == IQType.Result && _pendingMessages.Contains(message.ID))
                .Subscribe(message => OnServiceDiscoveryMessage(message));

            _vCardSubscription = _session.Connection
                .OnVCardMessage
                .Where(message => message.From == ResourceId)
                .Subscribe(message => OnVCardMessage(message));
        }

        private void Unsubscribe()
        {
            if (_sessionStateSubscription != null)
            {
                _sessionStateSubscription.Dispose();
                _sessionStateSubscription = null;
            }

            if (_infoQueryErrorSubscription != null)
            {
                _infoQueryErrorSubscription.Dispose();
                _infoQueryErrorSubscription = null;
            }

            if (_serviceDiscoverySubscription != null)
            {
                _serviceDiscoverySubscription.Dispose();
                _serviceDiscoverySubscription = null;
            }

            if (_vCardSubscription != null)
            {
                _vCardSubscription.Dispose();
                _vCardSubscription = null;
            }
        }

        private void OnServiceDiscoveryMessage(IQ message)
        {
            _pendingMessages.Remove(message.ID);

            Capabilities.Identities.Clear();
            Capabilities.Features.Clear();

            // Reponse to our capabilities query
            foreach (var item in message.Items)
            {
                if (!(item is ServiceQuery)) 
                    continue;

                var query = (ServiceQuery)item;

                foreach (Serialization.Extensions.ServiceDiscovery.ServiceIdentity identity in query.Identities)
                {
                    Capabilities.Identities.Add
                        (
                            new ServiceIdentity(identity.Name, identity.Category, identity.Type)
                        );
                }

                foreach (Serialization.Extensions.ServiceDiscovery.ServiceFeature supportedFeature in query.Features)
                {
                    Capabilities.Features.Add(new ServiceFeature(supportedFeature.Name));
                }
            }

            if (_session.ClientCapabilitiesStorage.Exists(_capabilities.Node, _capabilities.VerificationString))
                return;

            _session.ClientCapabilitiesStorage.ClientCapabilities.Add(Capabilities);
            _session.ClientCapabilitiesStorage.Save();

            // NotifyPropertyChanged(() => Capabilities);
        }

        private void OnQueryErrorMessage(IQ message)
        {
            if (_pendingMessages.Contains(message.ID))
            {
                _pendingMessages.Remove(message.ID);
            }
        }

        private void OnVCardMessage(IQ message)
        {
            // Update the Avatar image
            var vCard = (VCardData)message.Items[0];

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
                        if (avatarStream.Length > 0)
                        {
                            _session.AvatarStorage.SaveAvatar(ResourceId.BareIdentifier, _avatarHash, avatarStream);
                        }
                    }

                    Avatar = _session.AvatarStorage.ReadAvatar(ResourceId.BareIdentifier);
                }
                catch
                {
                    // TODO: Handle the exception
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
                _session.AvatarStorage.RemoveAvatar(ResourceId.BareIdentifier);
            }

            _session.AvatarStorage.Save();
        }
    }
}