using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Services.TvProgMain
{
    public static class TvProgProgrammeDefaults
    {
        /// <summary>
        /// Получение ключа для кеширования типов программы
        /// </summary>
        /// {0} : Показывать скрытые записи?
        public static CacheKey TypeProgAllCacheKey => new CacheKey("TvProg.typeprog.all.{0}", TvProgEntityCacheDefaults<TypeProg>.Prefix);

        /// <summary>
        /// Получение ключа для кеширования тв-провайдеров
        /// </summary>
        /// {0} : Показывать скрытые записи?
        public static CacheKey ProvidersAllCacheKey => new CacheKey("TvProg.provider.all.{0}", TvProgEntityCacheDefaults<TvProgProviders>.Prefix);

        /// <summary>
        /// Получение ключа для кеширования веб-ресурсов
        /// </summary>
        /// {0} : Показывать скрытые записи?
        public static CacheKey WebResourcesAllCacheKey => new CacheKey("TvProg.webresource.all.{0}", TvProgEntityCacheDefaults<WebResources>.Prefix);
    }
}
