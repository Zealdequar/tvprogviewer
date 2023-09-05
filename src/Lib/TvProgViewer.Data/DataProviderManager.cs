﻿using TvProgViewer.Core;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data.Configuration;
using TvProgViewer.Data.DataProviders;

namespace TvProgViewer.Data
{
    /// <summary>
    /// Represents the data provider manager
    /// </summary>
    public partial class DataProviderManager : IDataProviderManager
    {
        #region Methods

        /// <summary>
        /// Gets data provider by specific type
        /// </summary>
        /// <param name="dataProviderType">Data provider type</param>
        /// <returns></returns>
        public static ITvProgDataProvider GetDataProvider(DataProviderType dataProviderType)
        {
            return dataProviderType switch
            {
                DataProviderType.SqlServer => new MsSqlTvProgDataProvider(),
                DataProviderType.MySql => new MySqlTvProgDataProvider(),
                DataProviderType.PostgreSQL => new PostgreSqlDataProvider(),
                _ => throw new TvProgException($"Not supported data provider name: '{dataProviderType}'"),
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets data provider
        /// </summary>
        public ITvProgDataProvider DataProvider
        {
            get
            {
                var dataProviderType = Singleton<DataConfig>.Instance.DataProvider;

                return GetDataProvider(dataProviderType);
            }
        }

        #endregion
    }
}
