using TvProgViewer.Core;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data;
using TvProgViewer.Data.Configuration;
using TvProgViewer.Data.DataProviders;

namespace TvProgViewer.Tests
{
    /// <summary>
    /// Represents the data provider manager
    /// </summary>
    public partial class TestDataProviderManager : IDataProviderManager
    {
        #region Properties

        /// <summary>
        /// Gets data provider
        /// </summary>
        public ITvProgDataProvider DataProvider
        {
            get
            {
                return Singleton<DataConfig>.Instance.DataProvider switch
                {
                    DataProviderType.SqlServer => new MsSqlTvProgDataProvider(),
                    DataProviderType.MySql => new MySqlTvProgDataProvider(),
                    DataProviderType.PostgreSQL => new PostgreSqlDataProvider(),
                    DataProviderType.Unknown => new SqLiteTvProgDataProvider(),
                    _ => throw new TvProgException($"Unknown [{Singleton<DataConfig>.Instance.DataProvider}] DataProvider")
                };
            }
        }

        #endregion
    }
}
