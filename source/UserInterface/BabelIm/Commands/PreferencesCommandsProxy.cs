/*
    Copyright (c) 2009, Carlos Guzmán Álvarez

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
using Microsoft.Practices.Composite.Presentation.Commands;

namespace BabelIm.Commands
{
    /// <summary>
    /// Preferences commands proxy class
    /// </summary>
    public class PreferencesCommandsProxy
    {
        #region · Properties ·

        /// <summary>
        /// Gets the open preferences command.
        /// </summary>
        /// <value>The add new command.</value>
        public virtual CompositeCommand OpenPreferencesCommand
        {
            get { return PreferencesCommands.OpenPreferences; }
        }

        /// <summary>
        /// Gets the save preferences command.
        /// </summary>
        /// <value>The add new command.</value>
        public virtual CompositeCommand SavePreferencesCommand
        {
            get { return PreferencesCommands.SavePreferences; }
        }

        /// <summary>
        /// Gets the open server preferences command.
        /// </summary>
        /// <value>The add new command.</value>
        public virtual CompositeCommand OpenServerPreferencesCommand
        {
            get { return PreferencesCommands.OpenServerPreferences; }
        }

        /// <summary>
        /// Gets the open account preferences command.
        /// </summary>
        /// <value>The add new command.</value>
        public virtual CompositeCommand OpenAccountPreferencesCommand
        {
            get { return PreferencesCommands.OpenAccountPreferences; }
        }

        #endregion
    }
}
