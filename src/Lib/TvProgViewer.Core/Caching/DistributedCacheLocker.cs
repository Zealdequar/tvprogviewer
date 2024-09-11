using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TvProgViewer.Core.Caching
{
    public partial class DistributedCacheLocker : ILocker
    {
        #region Поля

        private static readonly string _running = JsonConvert.SerializeObject(TaskStatus.Running);
        private readonly IDistributedCache _distributedCache;

        #endregion

        #region Конструктор

        public DistributedCacheLocker(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Выполняет некоторую асинхронную задачу с эксклюзивной блокировкой
        /// </summary>
        /// <param name="resource">Ключ, который мы ищем</param>
        /// <param name="expirationTime">Время, по истечении которого блокировка будет автоматически снята</param>
        /// <param name="action">Асинхронная задача, выполняемая с блокировкой</param>
        /// <returns>Задача, которая принимает значение true, если блокировка была получена и действие было выполнено; в противном случае значение false</returns>
        public async Task<bool> PerformActionWithLockAsync(string resource, TimeSpan expirationTime, Func<Task> action)
        {
            // Убедиться, что установлена блокировка
            if (!string.IsNullOrEmpty(await _distributedCache.GetStringAsync(resource)))
                return false;

            try
            {
                await _distributedCache.SetStringAsync(resource, resource, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expirationTime
                });

                await action();

                return true;
            }
            finally
            {
                //снять блокировку, даже если действие выполнится неудачно
                await _distributedCache.RemoveAsync(resource);
            }
        }

        /// <summary>
        /// Запускает фоновую задачу с "сердцебиением": флагом состояния, который будет периодически обновляться, чтобы сигнализировать
        /// другим пользователям о том, что задача запущена, и помешать им запустить ту же задачу.
        /// </summary>
        /// <param name="key">Ключ задачи, выполняющейся в фоновом режиме</param>
        /// <param name="expirationTime">Время, по истечении которого срок действия ключа heartbeat автоматически истечет. Должно быть больше, чем <paramref name="heartbeatInterval"/></param>
        /// <param name="heartbeatInterval">Интервал, с которым следует обновлять частоту сердцебиений, если этого требует реализация</param>
        /// <param name="action">Выполняемая асинхронная фоновая задача</param>
        /// <param name="cancellationTokenSource">CancellationTokenSource для ручной отмены задачи</param>
        /// <returns>Задача, которая возвращает значение истина, если блокировка была получена и действие было выполнено; в противном случае значение ложно</returns>
        public async Task RunWithHeartbeatAsync(string key, TimeSpan expirationTime, TimeSpan heartbeatInterval, Func<CancellationToken, Task> action, CancellationTokenSource cancellationTokenSource = default)
        {
            if (!string.IsNullOrEmpty(await _distributedCache.GetStringAsync(key)))
                return;

            async Task heartbeat(CancellationToken token) => await _distributedCache.SetStringAsync(
                key,
                _running,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expirationTime },
                token: token);

            var tokenSource = cancellationTokenSource ?? new();
            var heartbeatTokenSource = CancellationTokenSource.CreateLinkedTokenSource(tokenSource.Token);
            try
            {
                // Запустить heartbeat на ранней стадии, чтобы свести к минимуму риск многократного выполнения:
                await heartbeat(heartbeatTokenSource.Token);

                using var timer = new Timer(
                    callback: async state =>
                    {
                        try
                        {
                            heartbeatTokenSource.Token.ThrowIfCancellationRequested();
                            var status = await _distributedCache.GetStringAsync(key, tokenSource.Token);
                            if (!string.IsNullOrEmpty(status) && JsonConvert.DeserializeObject<TaskStatus>(status) == TaskStatus.Canceled)
                            {
                                tokenSource.Cancel();
                                return;
                            }
                            await heartbeat(heartbeatTokenSource.Token);
                        }
                        catch (OperationCanceledException) { }
                    },
                    state: null,
                    dueTime: 0,
                    period: (int)heartbeatInterval.TotalMilliseconds);

                await action(tokenSource.Token);
            }
            catch (OperationCanceledException) { }
            finally
            {
                // Поскольку heartbeat вызывается таймером как асинхронный обратный вызов void, он не ожидается и не отменяется при удалении.
                // Поскольку это может привести к вызову heartbeat после удаления ключа, мы отменяем его перед удалением ключа.
                heartbeatTokenSource.Cancel();
                await _distributedCache.RemoveAsync(key);
            }
        }

        /// <summary>
        /// Пытается отменить фоновую задачу, пометив ее для отмены при следующем повторном запуске.
        /// </summary>
        /// <param name="key">Ключ задачи</param>
        /// <param name="expirationTime">Время, по истечении которого задача будет считаться остановленной из-за выключения системы 
        /// или по другим причинам, даже если она явно не отменена.</param>
        /// <returns>Задача, которая представляет собой запрос на отмену задачи. Обратите внимание, что завершение этой задачи не 
        /// обязательно означает, что задача была отменена, а только то, что была запрошена отмена.</returns>
        public async Task CancelTaskAsync(string key, TimeSpan expirationTime)
        {
            var status = await _distributedCache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(status) && JsonConvert.DeserializeObject<TaskStatus>(status) != TaskStatus.Canceled)
            {
                await _distributedCache.SetStringAsync(
                    key,
                    JsonConvert.SerializeObject(TaskStatus.Canceled),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = expirationTime
                    });
            }
        }

        /// <summary>
        /// Проверить, запущена ли сейчас фоновая задача
        /// </summary>
        /// <param name="key">Ключ задачи</param>
        /// <returns>Задача, значение которой равно true, если выполняется фоновая задача; в противном случае значение false</returns>
        public async Task<bool> IsTaskRunningAsync(string key)
        {
            return !string.IsNullOrEmpty(await _distributedCache.GetStringAsync(key));
        }

        #endregion
    }
}