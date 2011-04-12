using System;
using System.Windows;

namespace BabelIm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
        : Application
    {
        #region · Static Members ·

        /// <summary>
        /// Determines if the application is running on Windows 7
        /// </summary>
        public static bool RunningOnWin7
        {
            get
            {
                return (Environment.OSVersion.Version.Major > 6) ||
                    (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1);
            }
        }

        #endregion
    }
}
