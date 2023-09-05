using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Data.TvProgMain.ProgObjs;

namespace TvProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Интерфейс для работы с жанрами
    /// </summary>
    public partial interface IGenreService
    {
        /// <summary>
        /// Получение всех жанров
        /// </summary>
        /// <param name="uid">Идентификатор пользователя; null — для всех пользователей</param>
        /// <param name="all">Все жанры</param>
        public Task<List<Genre>> GetGenresAsync(int? uid, bool all);
    }
}
