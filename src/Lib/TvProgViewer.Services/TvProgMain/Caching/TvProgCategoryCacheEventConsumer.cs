using System.Threading.Tasks;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.TvProgMain;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.TvProgMain.Caching
{
    /// <summary>
    /// Потребитель событий изменения категорий телепрограммы (сброс кеша списков категорий)
    /// </summary>
    public partial class TvProgCategoryCacheEventConsumer : CacheEventConsumer<TvProgCategory>
    {
        /// <summary>
        /// Сбрасывает кеш при изменении категории
        /// </summary>
        /// <param name="entity">Категория телепрограммы</param>
        /// <param name="entityEventType">Тип события изменения сущности</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected override Task ClearCacheAsync(TvProgCategory entity, EntityEventType entityEventType)
        {
            return RemoveByPrefixAsync(TvProgEntityCacheDefaults<TvProgCategory>.Prefix);
        }
    }
}
