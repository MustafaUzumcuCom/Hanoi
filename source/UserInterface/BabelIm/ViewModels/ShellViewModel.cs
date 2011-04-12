/*
The MIT License

Copyright (c) 2009-2010 Carlos Guzmán Álvarez

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System.Windows;
using System.Windows.Input;
using BabelIm.Infrastructure;

namespace BabelIm.ViewModels
{
    /// <summary>
    /// Shell Window ViewModel
    /// </summary>
    public sealed class ShellViewModel 
        : ViewModelBase
    {
        #region · Fields ·

        private ICommand    shutdownCommand;
        private ICommand    minimizeCommand;
        private WindowState windowState;
        private ResizeMode  resizeMode;

        #endregion

        #region · Commands ·

        /// <summary>
        /// Gets the minimize command.
        /// </summary>
        /// <value>The minimize command.</value>
        public ICommand MinimizeCommand
        {
            get
            {
                if (this.minimizeCommand == null)
                {
                    this.minimizeCommand = new RelayCommand(() => OnMinimizeWindow());
                }

                return this.minimizeCommand;
            }
        }

        /// <summary>
        /// Gets the shutdown command
        /// </summary>
        public ICommand ShutdownCommand
        {
            get
            {
                if (this.shutdownCommand == null)
                {
                    this.shutdownCommand = new RelayCommand(() => OnShutdown());
                }

                return this.shutdownCommand;
            }
        }
        
        #endregion

        #region · Properties ·

        /// <summary>
        /// Gets the Window display name
        /// </summary>
        public string DisplayName
        {
            get { return "babel - XMPP IM Client"; }
        }

        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        /// <value>The state of the window.</value>
        public WindowState WindowState
        {
            get { return this.windowState; }
            set
            {
                if (this.windowState != value)
                {
                    this.windowState = value;
                    this.NotifyPropertyChanged(() => WindowState);
                }
            }
        }

        /// <summary>
        /// Gets or sets the resize mode.
        /// </summary>
        /// <value>The resize mode.</value>
        public ResizeMode ResizeMode
        {
            get { return this.resizeMode; }
            set
            {
                this.resizeMode = value;
                this.NotifyPropertyChanged(() => ResizeMode);
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        public ShellViewModel()
        {
        }

        #endregion

        #region · Command Actions ·

        private void OnShutdown()
        {
            Application.Current.Shutdown();
        }

        private void OnMinimizeWindow()
        {
            this.WindowState = WindowState.Minimized;
        }

        #endregion
    }
}
