using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Distributed;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Configuration;

namespace TvProgViewer.Services.Caching
{
    /// <summary>
    /// Представляет mssqlserver распределённый кэш 
    /// </summary>
    public class MsSqlServerCacheManager : DistributedCacheManager
    {
        #region Поля

        private readonly DistributedCacheConfig _distributedCacheConfig;

        #endregion

        #region Конструктор

        public MsSqlServerCacheManager(AppSettings appSettings, 
            IDistributedCache distributedCache,
            CacheKeyManager cacheKeyManager)
            : base(appSettings, distributedCache, cacheKeyManager)
        {
            _distributedCacheConfig = appSettings.Get<DistributedCacheConfig>();
        }

        #endregion

        #region Утилиты

        protected async Task PerformActionAsync(SqlCommand command, params SqlParameter[] parameters)
        {
            var conn = new SqlConnection(_distributedCacheConfig.ConnectionString);
            try
            {
                await conn.OpenAsync();
                command.Connection = conn;
                if (parameters.Any())
                    command.Parameters.AddRange(parameters);

                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                await conn.CloseAsync();
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Удалить элементы по префиксу ключа кэша
        /// </summary>
        /// <param name="prefix">Префикс ключа кэша</param>
        /// <param name="prefixParameters">Параметры для создания префикса ключа кэша</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public override async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
        {
            prefix = PrepareKeyPrefix(prefix, prefixParameters);

            var command =
                new SqlCommand(
                    $"DELETE FROM {_distributedCacheConfig.SchemaName}.{_distributedCacheConfig.TableName} WHERE Id LIKE @Prefix + '%'");

            await PerformActionAsync(command, new SqlParameter("Prefix", SqlDbType.NVarChar) { Value = prefix });

            RemoveByPrefixInstanceData(prefix);
        }

        /// <summary>
        /// Очистить все кэшированные данные
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public override async Task ClearAsync()
        {
            var command =
                new SqlCommand(
                    $"TRUNCATE TABLE {_distributedCacheConfig.SchemaName}.{_distributedCacheConfig.TableName}");

            await PerformActionAsync(command);

            ClearInstanceData();
        }

        #endregion
    }
}