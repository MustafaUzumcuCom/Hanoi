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
using Hanoi.Serialization.Extensions.UserTune;

namespace Hanoi.Xmpp.InstantMessaging.PersonalEventing
{
    public sealed class UserTuneEvent : UserEvent
    {
        public string Artist { get; private set; }
        public ushort Length { get; private set; }
        public string Rating { get; private set; }
        public string Source { get; private set; }
        public string Title { get; private set; }
        public string Track { get; private set; }
        public string Uri { get; private set; }
        
        public UserTuneEvent(string artist, ushort length, string rating, string source, string title, string track, string uri)
            : base(null)
        {
            Artist = artist;
            Length = length;
            Rating = rating;
            Source = source;
            Title = title;
            Track = track;
            Uri = uri;
        }

        public UserTuneEvent(Contact user, Tune tune)
            : base(user)
        {
            Artist = tune.Artist;
            Length = tune.Length;
            Rating = tune.Rating;
            Source = tune.Source;
            Title = tune.Title;
            Track = tune.Track;
            Uri = tune.Uri;
        }

        public bool IsEmpty
        {
            get
            {
                return (string.IsNullOrEmpty(Artist) &&
                        string.IsNullOrEmpty(Title) &&
                        string.IsNullOrEmpty(Rating) &&
                        string.IsNullOrEmpty(Source) &&
                        string.IsNullOrEmpty(Track) &&
                        Length == 0);
            }
        }
    }
}