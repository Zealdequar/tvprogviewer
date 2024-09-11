using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace TvProgViewer.Core.Caching
{
    /// <summary>
    /// Диспетчер распределённого кэша, который блокирует полученную задачу
    /// </summary>
    public partial class MemoryCacheLocker : ILocker 
    {
        #region Поля

        private readonly IMemoryCache _memoryCache;

        #endregion

        #region Конструктор

        public MemoryCacheLocker(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        #endregion

        #region Утилиты

        private async Task<bool> RunAsync(string key, TimeSpan? expirationTime, Func<CancellationToken, Task> action, CancellationTokenSource cancellationTokenSource = default)
        {
            var started = false;
            try
            {
                var tokenSource = _memoryCache.GetOrCreate(key, entry => new Lazy<CancellationTokenSource>(() =>
                {
                    entry.AbsoluteExpirationRelativeToNow = expirationTime;
                    entry.SetPriority(CacheItemPriority.NeverRemove);
                    started = true;
                    return cancellationTokenSource ?? new();
                }, true)).Value;

                if (started)
                    await action(tokenSource.Token);
            }
            catch (OperationCanceledException) { }
            finally
            {
                if (started)
                    _memoryCache.Remove(key);
            }
            return started;
        }

        #endregion

        #region Методы

        public async Task<bool> PerformActionWithLockAsync(string resource, TimeSpan expirationTime, Func<Task> action)
        {
            return await RunAsync(resource, expirationTime, _ => action());
        }

        public async Task RunWithHeartbeatAsync(string key, TimeSpan expirationTime, TimeSpan heartbeatInterval, Func<CancellationToken, Task> action, CancellationTokenSource cancellationTokenSource = default)
        {
            // Здесь игнорируются expirationTime и heartbeatInterval, т.к. кэш не используется совместно с другими экземплярами и в любом случае будет удален при сбое системы.
            // Гарантируется, что задача будет выполняться до тех пор, пока она находится в кэше.
            await RunAsync(key, null, action, cancellationTokenSource);
        }

        public Task CancelTaskAsync(string key, TimeSpan expirationTime)
        {
            if (_memoryCache.TryGetValue(key, out Lazy<CancellationTokenSource> tokenSource))
                tokenSource.Value.Cancel();
            return Task.CompletedTask;
        }

        public Task<bool> IsTaskRunningAsync(string key)
        {
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }

        #endregion
    }
}
