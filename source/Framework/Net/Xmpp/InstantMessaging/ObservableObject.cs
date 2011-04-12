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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Threading;

namespace BabelIm.Net.Xmpp.InstantMessaging
{
    /// <summary>
    /// Base class for observable objects viewmodels
    /// </summary>
    public abstract class ObservableObject 
        : INotifyPropertyChanged
    {
        #region · Static Methods ·

        public static PropertyChangedEventArgs CreateArgs<T>(
            Expression<Func<T, object>> propertyExpression)
        {
            return new PropertyChangedEventArgs(propertyExpression.GetPropertyName());
        }

        #endregion

        #region · INotifyPropertyChanged Members ·

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region · Fields ·

        private Dispatcher dispatcher;

        #endregion

        #region · Protected Properties ·

        protected Dispatcher Dispatcher
        {
            get { return this.dispatcher; }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableObject"/> class.
        /// </summary>
        protected ObservableObject()
        {
            this.dispatcher = Dispatcher.CurrentDispatcher;
        }

        #endregion

        #region · Protected Methods ·

        /// <summary>
        /// Executes the specified <see cref="Action "/> at the <see cref="DispatcherPriority.ApplicationIdle"/> priority 
        /// on the thread on which the DispatcherObject is associated with. 
        /// </summary>
        /// <param name="dispatcherObject">The dispatcher object.</param>
        /// <param name="action">The action.</param>
        protected void InvokeAsynchronously(Action action)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }

        /// <summary>
        /// Executes the specified <see cref="Action "/> at the <see cref="DispatcherPriority.ApplicationIdle"/> priority 
        /// on the thread on which the DispatcherObject is associated with. 
        /// </summary>
        /// <param name="dispatcherObject">The dispatcher object.</param>
        /// <param name="action">The action.</param>
        protected void InvokeAsynchronouslyInBackground(Action action)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }

        /// <summary>
        /// Executes the specified <see cref="Action "/> at the <see cref="DispatcherPriority.ApplicationIdle"/> priority 
        /// on the thread on which the DispatcherObject is associated with. 
        /// </summary>
        /// <param name="dispatcherObject">The dispatcher object.</param>
        /// <param name="action">The action.</param>
        protected void Invoke(Action action)
        {
            if (this.Dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                this.Dispatcher.Invoke(action);
            }
        }

        /// <summary>
        /// Notifies all properties changed.
        /// </summary>
        protected void NotifyAllPropertiesChanged()
        {
            this.NotifyPropertyChanged((string)null);
        }

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        protected virtual void NotifyPropertyChanged<T>(Expression<Func<T>> property)
        {
            this.NotifyPropertyChanged(property.CreateChangeEventArgs());
        }

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            this.NotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            if (this.PropertyChanged != null)
            {
                this.InvokeAsynchronouslyInBackground
                (
                    () =>
                    {
                        this.PropertyChanged(this, args);
                    }
                );
            }
        }

        #endregion
    }
}
