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

using System.Xml.Serialization;
using Hanoi.Xmpp.Serialization;

namespace Hanoi.Serialization.Extensions.UserMood {
    /// <summary>
    ///   XEP-0107: User Mood
    /// </summary>
    [XmlType(AnonymousType = true, Namespace = "http://jabber.org/protocol/mood")]
    [XmlRootAttribute("mood", Namespace = "http://jabber.org/protocol/mood", IsNullable = false)]
    public sealed class Mood {
        /// <remarks />
        [XmlElementAttribute("afraid", typeof (Empty))]
        [XmlElementAttribute("amazed", typeof (Empty))]
        [XmlElementAttribute("amorous", typeof (Empty))]
        [XmlElementAttribute("angry", typeof (Empty))]
        [XmlElementAttribute("annoyed", typeof (Empty))]
        [XmlElementAttribute("anxious", typeof (Empty))]
        [XmlElementAttribute("aroused", typeof (Empty))]
        [XmlElementAttribute("ashamed", typeof (Empty))]
        [XmlElementAttribute("bored", typeof (Empty))]
        [XmlElementAttribute("brave", typeof (Empty))]
        [XmlElementAttribute("calm", typeof (Empty))]
        [XmlElementAttribute("cautious", typeof (Empty))]
        [XmlElementAttribute("cold", typeof (Empty))]
        [XmlElementAttribute("confident", typeof (Empty))]
        [XmlElementAttribute("confused", typeof (Empty))]
        [XmlElementAttribute("contemplative", typeof (Empty))]
        [XmlElementAttribute("contented", typeof (Empty))]
        [XmlElementAttribute("cranky", typeof (Empty))]
        [XmlElementAttribute("crazy", typeof (Empty))]
        [XmlElementAttribute("creative", typeof (Empty))]
        [XmlElementAttribute("curious", typeof (Empty))]
        [XmlElementAttribute("dejected", typeof (Empty))]
        [XmlElementAttribute("depressed", typeof (Empty))]
        [XmlElementAttribute("disappointed", typeof (Empty))]
        [XmlElementAttribute("disgusted", typeof (Empty))]
        [XmlElementAttribute("dismayed", typeof (Empty))]
        [XmlElementAttribute("distracted", typeof (Empty))]
        [XmlElementAttribute("embarrassed", typeof (Empty))]
        [XmlElementAttribute("envious", typeof (Empty))]
        [XmlElementAttribute("excited", typeof (Empty))]
        [XmlElementAttribute("flirtatious", typeof (Empty))]
        [XmlElementAttribute("frustrated", typeof (Empty))]
        [XmlElementAttribute("grumpy", typeof (Empty))]
        [XmlElementAttribute("guilty", typeof (Empty))]
        [XmlElementAttribute("happy", typeof (Empty))]
        [XmlElementAttribute("hopeful", typeof (Empty))]
        [XmlElementAttribute("hot", typeof (Empty))]
        [XmlElementAttribute("humbled", typeof (Empty))]
        [XmlElementAttribute("humiliated", typeof (Empty))]
        [XmlElementAttribute("hungry", typeof (Empty))]
        [XmlElementAttribute("hurt", typeof (Empty))]
        [XmlElementAttribute("impressed", typeof (Empty))]
        [XmlElementAttribute("in_awe", typeof (Empty))]
        [XmlElementAttribute("in_love", typeof (Empty))]
        [XmlElementAttribute("indignant", typeof (Empty))]
        [XmlElementAttribute("interested", typeof (Empty))]
        [XmlElementAttribute("intoxicated", typeof (Empty))]
        [XmlElementAttribute("invincible", typeof (Empty))]
        [XmlElementAttribute("jealous", typeof (Empty))]
        [XmlElementAttribute("lonely", typeof (Empty))]
        [XmlElementAttribute("lucky", typeof (Empty))]
        [XmlElementAttribute("mean", typeof (Empty))]
        [XmlElementAttribute("moody", typeof (Empty))]
        [XmlElementAttribute("nervous", typeof (Empty))]
        [XmlElementAttribute("neutral", typeof (Empty))]
        [XmlElementAttribute("offended", typeof (Empty))]
        [XmlElementAttribute("outraged", typeof (Empty))]
        [XmlElementAttribute("playful", typeof (Empty))]
        [XmlElementAttribute("proud", typeof (Empty))]
        [XmlElementAttribute("relaxed", typeof (Empty))]
        [XmlElementAttribute("relieved", typeof (Empty))]
        [XmlElementAttribute("remorseful", typeof (Empty))]
        [XmlElementAttribute("restless", typeof (Empty))]
        [XmlElementAttribute("sad", typeof (Empty))]
        [XmlElementAttribute("sarcastic", typeof (Empty))]
        [XmlElementAttribute("serious", typeof (Empty))]
        [XmlElementAttribute("shocked", typeof (Empty))]
        [XmlElementAttribute("shy", typeof (Empty))]
        [XmlElementAttribute("sick", typeof (Empty))]
        [XmlElementAttribute("sleepy", typeof (Empty))]
        [XmlElementAttribute("spontaneous", typeof (Empty))]
        [XmlElementAttribute("stressed", typeof (Empty))]
        [XmlElementAttribute("strong", typeof (Empty))]
        [XmlElementAttribute("surprised", typeof (Empty))]
        [XmlElementAttribute("thankful", typeof (Empty))]
        [XmlElementAttribute("thirsty", typeof (Empty))]
        [XmlElementAttribute("tired", typeof (Empty))]
        [XmlElementAttribute("undefined", typeof (Empty))]
        [XmlElementAttribute("weak", typeof (Empty))]
        [XmlElementAttribute("worried", typeof (Empty))]
        [XmlChoiceIdentifierAttribute("MoodType")]
        public Empty Item { get; set; }

        /// <remarks />
        [XmlIgnoreAttribute]
        public MoodType MoodType { get; set; }

        /// <remarks />
        [XmlElementAttribute("text")]
        public string Text { get; set; }
    }
}