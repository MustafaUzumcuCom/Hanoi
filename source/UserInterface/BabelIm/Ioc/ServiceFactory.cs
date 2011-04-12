using System;
using System.Collections.Generic;
using BabelIm.Contracts;
using BabelIm.Net.Xmpp.InstantMessaging;
using Microsoft.Practices.Unity;

namespace BabelIm.IoC
{
    /// <summary>
    /// IServiceFactory implementation
    /// </summary>
    public sealed class ServiceFactory
        : IServiceFactory
    {
        #region · Singleton Instance ·

        private static readonly ServiceFactory Factory = new ServiceFactory();

        /// <summary>
        /// Current instance of ServiceFactory
        /// </summary>
        public static ServiceFactory Current
        {
            get { return Factory; }
        }

        /// <summary>
        /// Prevent "before field init" IL annotation
        /// </summary>
        static ServiceFactory()
        {
        }

        #endregion

        #region · Fields ·

        private IDictionary<string, IUnityContainer> containersDictionary;

        #endregion

        #region · Constructors ·

        /// <summary>
        /// default constructor
        /// </summary>
        private ServiceFactory()
        {
            containersDictionary = new Dictionary<string, IUnityContainer>();

            // Create root container
            IUnityContainer rootContainer = new UnityContainer();
            containersDictionary.Add("RootContext", rootContainer);

            ConfigureRootContainer(rootContainer);
        }

        #endregion

        #region · IServiceFactory Members ·

        /// <summary>
        /// <see cref="M:Microsoft.Samples.NLayerApp.Infrastructure.CrossCutting.IoC.IServiceFactory.Resolve{TService}"/>
        /// </summary>
        /// <typeparam name="TService"><see cref="M:Microsoft.Samples.NLayerApp.Infrastructure.CrossCutting.IoC.IServiceFactory.Resolve{TService}"/></typeparam>
        /// <returns><see cref="M:Microsoft.Samples.NLayerApp.Infrastructure.CrossCutting.IoC.IServiceFactory.Resolve{TService}"/></returns>
        public TService Resolve<TService>()
        {
            IUnityContainer container = this.containersDictionary["RootContext"];

            return container.Resolve<TService>();
        }

        /// <summary>
        /// Solve type construction and return the object as a TService instance
        /// </summary>
        /// <typeparam name="TService">Type of dependency to return</typeparam>
        /// <param name="type">Real Type of dependency to instantiate</param>
        /// <returns>instance of TService</returns>
        public object Resolve(Type type)
        {
            IUnityContainer container = this.containersDictionary["RootContext"];

            return container.Resolve(type, null);
        }

        /// <summary>
        /// <see cref="M:Microsoft.Samples.NLayerApp.Infrastructure.CrossCutting.IoC.IServiceFactory.Resolve{TService}"/>
        /// </summary>
        /// <param name="type"><see cref="M:Microsoft.Samples.NLayerApp.Infrastructure.CrossCutting.IoC.IServiceFactory.Resolve{TService}"/></param>
        public void RegisterType(Type type)
        {
            IUnityContainer container = this.containersDictionary["RootContext"];

            if (container != null)
            {
                container.RegisterType(type, new TransientLifetimeManager());
            }
        }

        #endregion

        #region · Private Methods ·

        /// <summary>
        /// Configure root container.Register types and life time managers for unity builder process
        /// </summary>
        /// <param name="container">Container to configure</param>
        private void ConfigureRootContainer(IUnityContainer container)
        {
            // Take into account that Types and Mappings registration could be also done using the UNITY XML configuration
            // But we prefer doing it here (C# code) because we'll catch errors at compiling time instead execution time, if any type has been written wrong.

            // Register Repositories mappings
            container.RegisterType<IXmppSession, XmppSession>(new ContainerControlledLifetimeManager());
            container.RegisterType<IChatViewManager, ChatViewManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<IConfigurationManager, ConfigurationManager>(new ContainerControlledLifetimeManager());
        }

        #endregion
    }
}
