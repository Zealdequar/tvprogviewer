﻿namespace TvProgViewer.Data
{
    /// <summary>
    /// Represents a data provider manager
    /// </summary>
    public partial interface IDataProviderManager
    {
        #region Properties

        /// <summary>
        /// Gets data provider
        /// </summary>
        ITvProgDataProvider DataProvider { get; }

        #endregion
    }
}
