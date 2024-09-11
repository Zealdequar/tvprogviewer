using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TvProgViewer.Core.Caching;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace TvProgViewer.Services.Caching
{
    public class RedisSynchronizedMemoryCache : IMemoryCache
    {
        private static readonly string _processId;

        private RedisConnectionWrapper _connection;

        private bool _disposed;
        private readonly IMemoryCache _memoryCache;
        private readonly string _ignorePrefix;
        private readonly CacheKeyManager _keyManager;

        static RedisSynchronizedMemoryCache()
        {
            var machineId = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                .Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

            if (string.IsNullOrEmpty(machineId))
                machineId = Environment.MachineName;

            _processId = machineId + Environment.ProcessId.ToString();
        }


        public RedisSynchronizedMemoryCache(IMemoryCache memoryCache,
            RedisConnectionWrapper connectionWrapper,
            CacheKeyManager cacheKeyManager,
            string ignorePrefix = null)
        {
            _memoryCache = memoryCache;
            _ignorePrefix = ignorePrefix;
            _keyManager = cacheKeyManager;
            _connection = connectionWrapper;
            SubscribeAsync().Wait();
        }

        private async Task<string> GetChannelAsync()
        {
            var db = await _connection.GetDatabaseAsync();
            return $"__change@{db.Database}__{_connection.Instance}__:";
        }

        private async Task SubscribeAsync()
        {
            var channel = await GetChannelAsync();
            (await _connection.GetSubscriberAsync()).Subscribe(channel + "*", (redisChannel, value) =>
            {
                if (value != _processId)
                {
                    var key = ((string)redisChannel).Replace(channel, "");
                    _memoryCache.Remove(key);
                    _keyManager.RemoveKey(key);
                }
            });
        }

        private async Task PublishChangeEventAsync(object key)
        {
            var channel = await GetChannelAsync();
            var stringKey = key.ToString();
            if (string.IsNullOrEmpty(_ignorePrefix) || !stringKey.StartsWith(_ignorePrefix))
                await (await _connection.GetSubscriberAsync()).PublishAsync($"{channel}{stringKey}", _processId, CommandFlags.FireAndForget);
        }

        private void OnEviction(object key, object value, EvictionReason reason, object state)
        {
            switch (reason)
            {
                case EvictionReason.Replaced:
                case EvictionReason.TokenExpired: // например, событие очистки кэша
                    _ = PublishChangeEventAsync(key);
                    break;
                // не публикуйте здесь информацию об удалении, так как это может быть вызвано самим событием redis
                default:
                    break;
            }
        }

        public ICacheEntry CreateEntry(object key)
        {
            return _memoryCache.CreateEntry(key).RegisterPostEvictionCallback(OnEviction);
        }

        public void Remove(object key)
        {
            _memoryCache.Remove(key);
            // публикуйте событие вручную, а не с помощью обратного вызова для удаления, чтобы избежать циклов обратной связи
            _ = PublishChangeEventAsync(key);
        }

        public bool TryGetValue(object key, out object value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        protected virtual async Task DisposeAsync(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    var channel = await GetChannelAsync();
                    (await _connection.GetSubscriberAsync()).Unsubscribe(channel + "*");
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            DisposeAsync(disposing: true).Wait();
            GC.SuppressFinalize(this);
        }
    }
}