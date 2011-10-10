/*
    Copyright (c) 2008 - 2010, Carlos Guzmán Álvarez

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

using BabelIm.Infrastructure;
using BabelIm.IoC;
using BabelIm.Net.Xmpp.InstantMessaging;
using BabelIm.Net.Xmpp.InstantMessaging.PersonalEventing;
using BabelIm.Net.Xmpp.Serialization.Extensions.UserMood;

namespace BabelIm.ViewModels {
    /// <summary>
    ///   ViewModel for activity views
    /// </summary>
    public sealed class ActivityViewModel
        : ViewModel<XmppSession> {
        private string moodText;
        private MoodType moodType;

        /// <summary>
        ///   Gets or sets the <see cref = "MoodType">mood type</see>
        /// </summary>
        public MoodType MoodType {
            get { return moodType; }
            set {
                if (moodType != value)
                {
                    moodType = value;
                    NotifyPropertyChanged(() => MoodType);
                }
            }
        }

        /// <summary>
        ///   Gets or sets the <see cref = "MoodType">mood text</see>
        /// </summary>
        public string MoodText {
            get { return moodText; }
            set {
                if (moodText != value)
                {
                    moodText = value;
                    NotifyPropertyChanged(() => MoodText);
                }
            }
        }

        /// <summary>
        ///   Gets the activity list
        /// </summary>
        public XmppActivity Activity {
            get { return ServiceFactory.Current.Resolve<IXmppSession>().Activity; }
        }

        /// <summary>
        ///   Gets the personal eventing instance associated to the session
        /// </summary>
        public XmppPersonalEventing PersonalEventing {
            get { return ServiceFactory.Current.Resolve<IXmppSession>().PersonalEventing; }
        }

        /// <summary>
        ///   Publishes the mood information
        /// </summary>
        public void PublishMood() {
            ServiceFactory.Current.Resolve<IXmppSession>().PublishMood(MoodType, MoodText);
        }
        }
}