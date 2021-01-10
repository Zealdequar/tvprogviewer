using System.Collections.Generic;
using TVProgViewer.Data.TvProgMain.ProgObjs;

namespace TVProgViewer.Services.TvProgMain
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
        public List<Genre> GetGenres(int? uid, bool all);
    }
}
