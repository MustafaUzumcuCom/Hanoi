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
using Hanoi.Serialization.Extensions.ServiceDiscovery;
using Hanoi.Serialization.InstantMessaging.Client;

namespace Hanoi.Xmpp.InstantMessaging.PersonalEventing
{
    /// <summary>
    ///   XMPP Personal Eventing
    /// </summary>
    public sealed class PersonalEventing
    {
        private readonly List<string> _features;
        private readonly List<string> _pendingMessages;
        private readonly Session _session;
        private IDisposable _infoQueryErrorSubscription;
        private bool _isUserTuneEnabled;
        private IDisposable _serviceDiscoverySubscription;
        private IDisposable _sessionStateSubscription;

        internal PersonalEventing(Session session)
        {
            _session = session;
            _pendingMessages = new List<string>();
            _features = new List<string>();

            SubscribeToSessionState();
        }

        public IEnumerable<string> Features
        {
            get { return _features; }
        }

        public bool SupportsUserTune
        {
            get { return SupportsFeature(InstantMessaging.Features.UserTune); }
        }

        public bool SupportsUserMood
        {
            get { return SupportsFeature(InstantMessaging.Features.UserMood); }
        }

        public bool IsUserTuneEnabled
        {
            get { return _isUserTuneEnabled; }
            set
            {
                if (_isUserTuneEnabled == value)
                    return;

                _isUserTuneEnabled = value;
            }
        }

        internal void DiscoverSupport()
        {
            var query = new ServiceItemQuery();
            var iq = new IQ
                         {
                             ID = IdentifierGenerator.Generate(),
                             Type = IQType.Get,
                             From = _session.UserId,
                             To = _session.UserId.BareIdentifier
                         };

            iq.Items.Add(query);
            _pendingMessages.Add(iq.ID);
            _features.Clear();
            _session.Send(iq);
        }

        private bool SupportsFeature(string featureName)
        {
            var q = from feature in _features
                    where feature == featureName
                    select feature;

            return (q.Count() > 0);
        }

        private void SubscribeToSessionState()
        {
            _sessionStateSubscription = _session
                .StateChanged
                .Where(s => s == SessionState.LoggingIn || s == SessionState.LoggingOut)
                .Subscribe
                (
                    newState =>
                    {
                        if (newState == SessionState.LoggingOut)
                        {
                            Subscribe();
                        }
                        else if (newState == SessionState.LoggingOut)
                        {
                            _features.Clear();
                            _pendingMessages.Clear();
                            Unsubscribe();
                        }
                    }
                );
        }

        private void Subscribe()
        {
            _infoQueryErrorSubscription = _session.Connection
                .OnInfoQueryMessage
                .Where(iq => iq.Type == IQType.Error)
                .Subscribe(OnQueryErrorMessage);

            _infoQueryErrorSubscription = _session.Connection
                .OnServiceDiscoveryMessage
                .Where(message => message.Type == IQType.Result && _pendingMessages.Contains(message.ID))
                .Subscribe(OnServiceDiscoveryMessage);
        }

        private void Unsubscribe()
        {
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
        }

        private void OnServiceDiscoveryMessage(IQ message)
        {
            _pendingMessages.Remove(message.ID);

            foreach (ServiceItemQuery item in message.Items)
            {
                foreach (ServiceItem sitem in item.Items)
                {
                    _features.Add(sitem.Node);
                }

                //if (SupportsUserTune)
                //{

                //}
            }
        }

        private void OnQueryErrorMessage(IQ message)
        {
            if (_pendingMessages.Contains(message.ID))
            {
                _pendingMessages.Remove(message.ID);
            }
        }
    }
}