using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace TvProgViewer.Services.Caching
{
    /// <summary>
    /// Представлена реализация оболочки подключения Redis
    /// </summary>
    public class RedisConnectionWrapper
    {
        #region Поля

        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private volatile IConnectionMultiplexer _connection;
        private readonly RedisCacheOptions _options;

        #endregion

        #region Свойства

        public string Instance => _options.InstanceName ?? string.Empty;

        #endregion

        #region Конструктор

        public RedisConnectionWrapper(IOptions<RedisCacheOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        #endregion

        #region Утилиты

        private async Task<IConnectionMultiplexer> ConnectAsync()
        {
            IConnectionMultiplexer connection;
            if (_options.ConnectionMultiplexerFactory is null)
            {
                if (_options.ConfigurationOptions is not null)
                    connection = await ConnectionMultiplexer.ConnectAsync(_options.ConfigurationOptions);
                else
                    connection = await ConnectionMultiplexer.ConnectAsync(_options.Configuration);
            }
            else
            {
                connection = await _options.ConnectionMultiplexerFactory();
            }

            if (_options.ProfilingSession != null)
                connection.RegisterProfiler(_options.ProfilingSession);
            return connection;
        }

        /// <summary>
        /// Получение соединения с серверами Redis и переподключение, если необходимо
        /// </summary>
        /// <returns></returns>
        protected async Task<IConnectionMultiplexer> GetConnectionAsync()
        {
            if (_connection?.IsConnected == true)
                return _connection;

            await _connectionLock.WaitAsync();
            try
            {
                if (_connection?.IsConnected == true)
                    return _connection;

                // Подключение отключено. Удаление подключения:
                _connection?.Dispose();

                // Создание экземпляра подключения Redis:
                _connection = await ConnectAsync();
            }
            finally
            {
                _connectionLock.Release();
            }

            return _connection;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получение интерактивного подключения к базе данных внутри Redis
        /// </summary>
        /// <returns>База данных кэша Redis</returns>
        public async Task<IDatabase> GetDatabaseAsync()
        {
            return (await GetConnectionAsync()).GetDatabase();
        }

        /// <summary>
        /// Получение конфигурации API для индивидуального сервера
        /// </summary>
        /// <param name="endPoint">Сетевя конечная точка</param>
        /// <returns>Сервер Redis</returns>
        public async Task<IServer> GetServerAsync(EndPoint endPoint)
        {
            return (await GetConnectionAsync()).GetServer(endPoint);
        }

        /// <summary>
        /// Получение всех конченых точек, определённых на сервере
        /// </summary>
        /// <returns>Массив конечных точек</returns>
        public async Task<EndPoint[]> GetEndPointsAsync()
        {
            return (await GetConnectionAsync()).GetEndPoints();
        }

        /// <summary>
        /// Получение подписчика на сервер
        /// </summary>
        /// <returns>Массив конечных точек</returns>
        public async Task<ISubscriber> GetSubscriberAsync()
        {
            return (await GetConnectionAsync()).GetSubscriber();
        }

        /// <summary>
        /// Удаление всех ключей базы данных
        /// </summary>
        public async Task FlushDatabaseAsync()
        {
            var endPoints = await GetEndPointsAsync();
            await Task.WhenAll(endPoints.Select(async endPoint =>
                await (await GetServerAsync(endPoint)).FlushDatabaseAsync()));
        }


        /// <summary>
        /// Освобождение всех ресурсов, ассоциированных с этим объектом
        /// </summary>
        public void Dispose()
        {
            // Удаление ConnectionMultiplexer
            _connection?.Dispose();
        }

        #endregion
    }
}