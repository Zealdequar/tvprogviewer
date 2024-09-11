using System;

namespace TvProgViewer.Core.Caching
{
    public static class CachingExtensions
    {
        /// <summary>
        /// Получить закэшированный элемент. Если его нет в кэше, тогда загрузить и кэшировать его.
        /// NOTE: этот метод сохранён только для обратной совместимости: асинхронная перегрузка предпочтительнее!
        /// </summary>
        /// <typeparam name="T">Тип кэшированного элемента</typeparam>
        /// <param name="cacheManager">Менеджер кэша</param>
        /// <param name="key">Ключ кэша</param>
        /// <param name="acquire">Функция для загрузки элемента если он ещё не закэширован</param>
        /// <returns>Кэшированное значение, ассоциированное с укзанным ключом</returns>
        public static T Get<T>(this IStaticCacheManager cacheManager, CacheKey key, Func<T> acquire)
        {
            return cacheManager.GetAsync(key, acquire).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Удаление элементов по префиксу ключа кэша
        /// </summary>
        /// <param name="cacheManager">Менеджер кэша</param>
        /// <param name="prefix">Префикс ключа кэша</param>
        /// <param name="prefixParameters">Параметры для создания префикса ключа кэша</param>
        public static void RemoveByPrefix(this IStaticCacheManager cacheManager, string prefix, params object[] prefixParameters)
        {
            cacheManager.RemoveByPrefixAsync(prefix, prefixParameters).Wait();
        }
    }
}