using System;
using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.Web.Framework.Factories
{
    /// <summary>
    /// Represents the localized model factory
    /// </summary>
    public partial interface ILocalizedModelFactory
    {
        /// <summary>
        /// Prepare localized model for localizable entities
        /// </summary>
        /// <typeparam name="T">Localized model type</typeparam>
        /// <param name="configure">Model configuration action</param>
        /// <returns>List of localized model</returns>
        IList<T> PrepareLocalizedModels<T>(Action<T, int> configure = null) where T : ILocalizedLocaleModel;
    }
}