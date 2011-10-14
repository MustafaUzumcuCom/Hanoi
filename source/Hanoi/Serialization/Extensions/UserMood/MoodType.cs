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

namespace Hanoi.Serialization.Extensions.UserMood {
    /// <summary>
    ///   XEP-0107: User Mood
    /// </summary>
    [XmlType(Namespace = "http://jabber.org/protocol/mood", IncludeInSchema = false)]
    public enum MoodType {
        /// <remarks />
        [XmlEnumAttribute("afraid")] Afraid,

        /// <remarks />
        [XmlEnumAttribute("amazed")] Amazed,

        /// <remarks />
        [XmlEnumAttribute("amorous")] Amorous,

        /// <remarks />
        [XmlEnumAttribute("angry")] Angry,

        /// <remarks />
        [XmlEnumAttribute("annoyed")] Annoyed,

        /// <remarks />
        [XmlEnumAttribute("anxious")] Anxious,

        /// <remarks />
        [XmlEnumAttribute("aroused")] Aroused,

        /// <remarks />
        [XmlEnumAttribute("ashamed")] Ashamed,

        /// <remarks />
        [XmlEnumAttribute("bored")] Bored,

        /// <remarks />
        [XmlEnumAttribute("brave")] Brave,

        /// <remarks />
        [XmlEnumAttribute("calm")] Calm,

        /// <remarks />
        [XmlEnumAttribute("cautious")] Cautious,

        /// <remarks />
        [XmlEnumAttribute("cold")] Cold,

        /// <remarks />
        [XmlEnumAttribute("confident")] Confident,

        /// <remarks />
        [XmlEnumAttribute("confused")] Confused,

        /// <remarks />
        [XmlEnumAttribute("contemplative")] Contemplative,

        /// <remarks />
        [XmlEnumAttribute("contented")] Contented,

        /// <remarks />
        [XmlEnumAttribute("cranky")] Cranky,

        /// <remarks />
        [XmlEnumAttribute("crazy")] Crazy,

        /// <remarks />
        [XmlEnumAttribute("creative")] Creative,

        /// <remarks />
        [XmlEnumAttribute("curious")] Curious,

        /// <remarks />
        [XmlEnumAttribute("dejected")] Dejected,

        /// <remarks />
        [XmlEnumAttribute("depressed")] Depressed,

        /// <remarks />
        [XmlEnumAttribute("disappointed")] Disappointed,

        /// <remarks />
        [XmlEnumAttribute("disgusted")] Disgusted,

        /// <remarks />
        [XmlEnumAttribute("dismayed")] Dismayed,

        /// <remarks />
        [XmlEnumAttribute("distracted")] Distracted,

        /// <remarks />
        [XmlEnumAttribute("embarrassed")] Embarrassed,

        /// <remarks />
        [XmlEnumAttribute("envious")] Envious,

        /// <remarks />
        [XmlEnumAttribute("excited")] Excited,

        /// <remarks />
        [XmlEnumAttribute("flirtatious")] Flirtatious,

        /// <remarks />
        [XmlEnumAttribute("frustrated")] Frustrated,

        /// <remarks />
        [XmlEnumAttribute("grumpy")] Grumpy,

        /// <remarks />
        [XmlEnumAttribute("guilty")] Guilty,

        /// <remarks />
        [XmlEnumAttribute("happy")] Happy,

        /// <remarks />
        [XmlEnumAttribute("hopeful")] Hopeful,

        /// <remarks />
        [XmlEnumAttribute("hot")] Hot,

        /// <remarks />
        [XmlEnumAttribute("humbled")] Humbled,

        /// <remarks />
        [XmlEnumAttribute("humiliated")] Humiliated,

        /// <remarks />
        [XmlEnumAttribute("hungry")] Hungry,

        /// <remarks />
        [XmlEnumAttribute("hurt")] Hurt,

        /// <remarks />
        [XmlEnumAttribute("impressed")] Impressed,

        /// <remarks />
        [XmlEnumAttribute("in_awe")] InAwe,

        /// <remarks />
        [XmlEnumAttribute("in_love")] Inlove,

        /// <remarks />
        [XmlEnumAttribute("indignant")] Indignant,

        /// <remarks />
        [XmlEnumAttribute("interested")] Interested,

        /// <remarks />
        [XmlEnumAttribute("intoxicated")] Intoxicated,

        /// <remarks />
        [XmlEnumAttribute("invincible")] Invincible,

        /// <remarks />
        [XmlEnumAttribute("jealous")] Jealous,

        /// <remarks />
        [XmlEnumAttribute("lonely")] Lonely,

        /// <remarks />
        [XmlEnumAttribute("lucky")] Lucky,

        /// <remarks />
        [XmlEnumAttribute("mean")] Mean,

        /// <remarks />
        [XmlEnumAttribute("moody")] Moody,

        /// <remarks />
        [XmlEnumAttribute("nervous")] Nervous,

        /// <remarks />
        [XmlEnumAttribute("neutral")] Neutral,

        /// <remarks />
        [XmlEnumAttribute("offended")] Offended,

        /// <remarks />
        [XmlEnumAttribute("outraged")] Outraged,

        /// <remarks />
        [XmlEnumAttribute("playful")] Playful,

        /// <remarks />
        [XmlEnumAttribute("proud")] Proud,

        /// <remarks />
        [XmlEnumAttribute("relaxed")] Relaxed,

        /// <remarks />
        [XmlEnumAttribute("relieved")] Relieved,

        /// <remarks />
        [XmlEnumAttribute("remorseful")] Remorseful,

        /// <remarks />
        [XmlEnumAttribute("restless")] Restless,

        /// <remarks />
        [XmlEnumAttribute("sad")] Sad,

        /// <remarks />
        [XmlEnumAttribute("sarcastic")] Sarcastic,

        /// <remarks />
        [XmlEnumAttribute("serious")] Serious,

        /// <remarks />
        [XmlEnumAttribute("shocked")] Shocked,

        /// <remarks />
        [XmlEnumAttribute("shy")] Shy,

        /// <remarks />
        [XmlEnumAttribute("sick")] Sick,

        /// <remarks />
        [XmlEnumAttribute("sleepy")] Sleepy,

        /// <remarks />
        [XmlEnumAttribute("spontaneous")] Spontaneous,

        /// <remarks />
        [XmlEnumAttribute("stressed")] Stressed,

        /// <remarks />
        [XmlEnumAttribute("strong")] Strong,

        /// <remarks />
        [XmlEnumAttribute("surprised")] Surprised,

        /// <remarks />
        [XmlEnumAttribute("thankful")] Thankful,

        /// <remarks />
        [XmlEnumAttribute("thirsty")] Thirsty,

        /// <remarks />
        [XmlEnumAttribute("tired")] Tired,

        /// <remarks />
        [XmlEnumAttribute("undefined")] Undefined,

        /// <remarks />
        [XmlEnumAttribute("weak")] Weak,

        /// <remarks />
        [XmlEnumAttribute("worried")] Worried,
    }
}