using System;
using System.Threading;
using System.Threading.Tasks;

namespace TvProgViewer.Core.Caching
{
    public interface ILocker
    {
        /// <summary>
        /// Выполняет некоторую асинхронную задачу с эксклюзивной блокировкой
        /// </summary>
        /// <param name="resource">Ключ, который мы ищем</param>
        /// <param name="expirationTime">Время, по истечении которого блокировка будет автоматически снята</param>
        /// <param name="action">Асинхронная задача, выполняемая с блокировкой</param>
        /// <returns>Задача, которая разрешает значение true, если блокировка была получена и действие было выполнено; в противном случае значение false</returns>
        Task<bool> PerformActionWithLockAsync(string resource, TimeSpan expirationTime, Func<Task> action);

        /// <summary>
        /// Запускает фоновую задачу с "сердцебиением": флагом состояния, который будет периодически обновляться, чтобы сигнализировать 
        /// другим пользователям о том, что задача запущена, и помешать им запустить ту же задачу.
        /// </summary>
        /// <param name="key">Ключ фоновой задачи</param>
        /// <param name="expirationTime">Время, после которого срок ключа сердцебиения автоматически истечёт. Должно быть больше, чем <paramref name="heartbeatInterval"/></param>
        /// <param name="heartbeatInterval">Интервал с которым происходит обновление сердцебиения, если требуется реализацией</param>
        /// <param name="action">Выполняемая асинхронная фоновая задача</param>
        /// <param name="cancellationTokenSource">CancellationTokenSource для ручной отмены задачи</param>
        /// <returns>Задача, которая разрешает значение true если блокировка была получена и действие было выполнено; в противном случае значение false</returns>
        Task RunWithHeartbeatAsync(string key, TimeSpan expirationTime, TimeSpan heartbeatInterval, Func<CancellationToken, Task> action, CancellationTokenSource cancellationTokenSource = default);

        /// <summary>
        /// Пытается отменить фоновую задачу, пометив ее для отмены при следующем повторном запуске.
        /// </summary>
        /// <param name="key">Ключ фоновой задачи</param>
        /// <param name="expirationTime">Время, по истечении которого задача будет считаться остановленной из-за выключения системы 
        /// или по другим причинам, даже если она явно не отменена.</param>
        /// <returns>Задача, которая представляет собой запрос на отмену задания. Обратите внимание, что завершение этой задачи
        /// не обязательно означает, что задание было отменено, а только то, что была запрошена отмена.</returns>

        Task CancelTaskAsync(string key, TimeSpan expirationTime);

        /// <summary>
        /// Проверка, запущена ли фоновая задача.
        /// </summary>
        /// <param name="key">Ключ фоновой задачи</param>
        /// <returns>Задача, значение которой равно true, если выполняется фоновая задача; в противном случае значение false</returns>
        Task<bool> IsTaskRunningAsync(string key);
    }
}
