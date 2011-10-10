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

using System;
using BabelIm.Net.Xmpp.Serialization.Extensions.UserTune;

namespace BabelIm.Net.Xmpp.InstantMessaging.PersonalEventing {
    /// <summary>
    ///   Activity for the user tune event
    /// </summary>
    public sealed class XmppUserTuneEvent
        : XmppUserEvent {
        private readonly string artist;
        private readonly ushort length;
        private readonly string rating;
        private readonly string source;
        private readonly string title;
        private readonly string track;
        private readonly string uri;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppUserTuneEvent">XmppUserTuneEvent</see> class.
        /// </summary>
        /// <param name = "user">User contact</param>
        /// <param name = "tune">User tune</param>
        public XmppUserTuneEvent(
            string artist,
            ushort length,
            string rating,
            string source,
            string title,
            string track,
            string uri) : base(null) {
            this.artist = artist;
            this.length = length;
            this.rating = rating;
            this.source = source;
            this.title = title;
            this.track = track;
            this.uri = uri;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmppUserTuneEvent">XmppUserTuneEvent</see> class.
        /// </summary>
        /// <param name = "user">User contact</param>
        /// <param name = "tune">User tune</param>
        public XmppUserTuneEvent(XmppContact user, Tune tune)
            : base(user) {
            artist = tune.Artist;
            length = tune.Length;
            rating = tune.Rating;
            source = tune.Source;
            title = tune.Title;
            track = tune.Track;
            uri = tune.Uri;
        }

        /// <remarks />
        public string Artist {
            get { return artist; }
        }

        /// <remarks />
        public ushort Length {
            get { return length; }
        }

        /// <remarks />
        public string Rating {
            get { return rating; }
        }

        /// <remarks />
        public string Source {
            get { return source; }
        }

        /// <remarks />
        public string Title {
            get { return title; }
        }

        /// <remarks />
        public string Track {
            get { return track; }
        }

        /// <remarks />
        public string Uri {
            get { return uri; }
        }

        public bool IsEmpty {
            get {
                return (String.IsNullOrEmpty(Artist) &&
                        String.IsNullOrEmpty(Title) &&
                        String.IsNullOrEmpty(Rating) &&
                        String.IsNullOrEmpty(Source) &&
                        String.IsNullOrEmpty(Track) &&
                        Length == 0);
            }
        }
        }
}