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
using System.Runtime.InteropServices;

namespace BabelIm.Infrastructure {
    public static class Win32NativeMethods {
        /// <summary>
        ///   Stop flashing. The system restores the window to its original state.
        /// </summary>
        public const UInt32 FLASHW_STOP = 0;

        /// <summary>
        ///   Flash the window caption.
        /// </summary>
        public const UInt32 FLASHW_CAPTION = 1;

        /// <summary>
        ///   Flash the taskbar button.
        /// </summary>
        public const UInt32 FLASHW_TRAY = 2;

        /// <summary>
        ///   Flash both the window caption and taskbar button. 
        ///   This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
        /// </summary>
        public const UInt32 FLASHW_ALL = 3;

        /// <summary>
        ///   Flash continuously, until the FLASHW_STOP flag is set.
        /// </summary>
        public const UInt32 FLASHW_TIMER = 4;

        /// <summary>
        ///   Flash continuously until the window comes to the foreground.
        /// </summary>
        public const UInt32 FLASHW_TIMERNOFG = 12;

        [DllImport("user32.dll")]
        private static extern Int32 FlashWindowEx(ref FLASHWINFO pwfi);

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public static void FlashWindow(IntPtr handle) {
            IntPtr hWnd = handle;
            var fInfo = new Win32NativeMethods.FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            fInfo.dwFlags = Win32NativeMethods.FLASHW_ALL |
                            Win32NativeMethods.FLASHW_TIMERNOFG |
                            Win32NativeMethods.FLASHW_CAPTION;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            FlashWindowEx(ref fInfo);
        }

        /// <summary>
        ///   Gets the application idle time
        /// </summary>
        /// <remarks>
        ///   http://www.geekpedia.com/tutorial210_Retrieving-the-Operating-System-Idle-Time-Uptime-and-Last-Input-Time.html
        /// </remarks>
        /// <returns></returns>
        public static int GetIdleTime() {
            // Get the system uptime		
            int systemUptime = Environment.TickCount;
            // The tick at which the last input was recorded		
            int lastInputTicks = 0;
            // The number of ticks that passed since last input
            int idleTicks = 0;

            // Set the struct		
            var lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint) Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            // If we have a value from the function		
            if (GetLastInputInfo(ref lastInputInfo))
            {
                // Get the number of ticks at the point when the last activity was seen		
                lastInputTicks = (int) lastInputInfo.dwTime;
                // Number of idle ticks = system uptime ticks - number of ticks at last input		
                idleTicks = systemUptime - lastInputTicks;
            }

            return (idleTicks/1000);
        }

        #region Nested type: FLASHWINFO

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        #endregion

        // Struct we'll need to pass to the function

        #region Nested type: LASTINPUTINFO

        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO {
            public UInt32 cbSize;
            public UInt32 dwTime;
        }

        #endregion
    }
}