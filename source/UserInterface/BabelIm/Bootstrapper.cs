/*
    Copyright (c) 2008 - 2009, Carlos Guzmán Álvarez

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

using System.Windows;
using BabelIm.Interfaces;
using CompositeWPFContrib.Composite.Wpf.Regions;
using Framework.Net.Xmpp.InstantMessaging;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Presentation.Regions;
using Microsoft.Practices.Composite.UnityExtensions;
using Microsoft.Practices.Unity;

namespace BabelIm
{
    /// <summary>
    /// Application Bootstrapper
    /// </summary>
    /// <typeparam name="TShell">The type of the shell.</typeparam>
    /// <typeparam name="TPresenter">The type of the presenter.</typeparam>
    public class Bootstrapper : UnityBootstrapper
    {
        #region · Static Members ·

        public static Bootstrapper Instance = new Bootstrapper();

        #endregion

        #region · Protected Methods ·

        /// <summary>
        /// Configures the <see cref="T:Microsoft.Practices.Unity.IUnityContainer"/>. May be overwritten in a derived class to add specific
        /// type mappings required by the application.
        /// </summary>
        protected override void ConfigureContainer()
        {
            this.Container.RegisterType<IShellView, Shell>();
            this.Container.RegisterType<XmppSession, XmppSession>(new ContainerControlledLifetimeManager());
            this.Container.RegisterType<IChatViewManager, ChatViewManager>(new ContainerControlledLifetimeManager());
            this.Container.RegisterType<ConfigurationManager, ConfigurationManager>(new ContainerControlledLifetimeManager());
            this.Container.RegisterType<NotificationManager, NotificationManager>(new ContainerControlledLifetimeManager());

            base.ConfigureContainer();           
        }

        /// <summary>
        /// Creates the shell or main window of the application.
        /// </summary>
        /// <returns>The shell of the application.</returns>
        /// <remarks>
        /// If the returned instance is a <see cref="T:System.Windows.DependencyObject"/>, the
        /// <see cref="T:Microsoft.Practices.Composite.UnityExtensions.UnityBootstrapper"/> will attach the default <seealso cref="T:Microsoft.Practices.Composite.Regions.IRegionManager"/> of
        /// the application in its <see cref="F:Microsoft.Practices.Composite.Wpf.Regions.RegionManager.RegionManagerProperty"/> attached property
        /// in order to be able to add regions by using the <seealso cref="F:Microsoft.Practices.Composite.Wpf.Regions.RegionManager.RegionNameProperty"/>
        /// attached property from XAML.
        /// </remarks>
        protected override DependencyObject CreateShell()
        {
             // If all goes well show the Shell
            ShellPresenter  presenter   = this.Container.Resolve<ShellPresenter>();
            IShellView      view        = presenter.View;

            view.ShowView();

            return view as DependencyObject;
        }

        protected override Microsoft.Practices.Composite.Modularity.IModuleCatalog GetModuleCatalog()
        {
            return new ModuleCatalog();
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings regionAdapterMappings = Container.TryResolve<RegionAdapterMappings>();

            if (regionAdapterMappings != null)
            {
                regionAdapterMappings.RegisterMapping(
                    typeof(Window), 
                    this.Container.Resolve<WindowRegionAdapter>());
            }

            return base.ConfigureRegionAdapterMappings();
        }

        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            return base.ConfigureDefaultRegionBehaviors();
        }

        #endregion
    }
}