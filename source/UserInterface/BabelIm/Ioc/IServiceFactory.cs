using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BabelIm.IoC
{
    /// <summary>
    /// Base contract for locator and register dependencies
    /// </summary>
    public interface IServiceFactory
    {
        #region · Methods ·

        /// <summary>
        /// Solve TService dependency
        /// </summary>
        /// <typeparam name="TService">Type of dependency</typeparam>
        /// <returns>instance of TService</returns>
        TService Resolve<TService>();

        /// <summary>
        /// Solve type construction and return the object as a TService instance
        /// </summary>
        /// <typeparam name="TService">Type of dependency</typeparam>
        /// <returns>instance of TService</returns>
        object Resolve(Type type);

        /// <summary>
        /// Register type into service locator
        /// </summary>
        /// <param name="type">Type to register</param>
        void RegisterType(Type type);

        #endregion
    }
}
