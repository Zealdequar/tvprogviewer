using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Data.TvProgMain.ProgObjs;
using TVProgViewer.Data;
using TVProgViewer.Core.Domain.TvProgMain;
using LinqToDB;
using System.Threading.Tasks;

namespace TVProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Обеспечивает выборку жанров
    /// </summary>
    public class GenreService: IGenreService
    {

        #region Поля

        private readonly IRepository<Genres> _genreRepository;
        private readonly IRepository<MediaPic> _mediaPicRepository;

        #endregion

        #region Конструктор

        public GenreService (IRepository<Genres> genreRepository,
            IRepository<MediaPic> mediaPicRepository)
        {
            _genreRepository = genreRepository;
            _mediaPicRepository = mediaPicRepository;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получение всех жанров
        /// </summary>
        /// <param name="uid">Идентификатор пользователя; null — для всех пользователей</param>
        /// <param name="all">Все жанры</param>
        public virtual async Task<List<Genre>> GetGenresAsync(int? uid, bool all)
        {
           
            List<Genre> genres = await (from g in _genreRepository.Table
                                  join mp in _mediaPicRepository.Table on g.IconId equals mp.Id into gmp
                                  from mp in gmp.DefaultIfEmpty()
                                  where g.UserId == uid && g.DeleteDate == null
                                  select new Genre
                                  {
                                      GenreId = g.Id,
                                      Uid = g.UserId,
                                      IconId = g.IconId,
                                      GenrePath = mp.Path25 + mp.FileName,
                                      CreateDate = g.CreateDate,
                                      GenreName = g.GenreName,
                                      Visible = g.Visible,
                                      DeleteDate = g.DeleteDate
                                  }).ToListAsync();
                if (!all)
                    genres = genres.Where(g => g.Visible).ToList();
            return genres;
        }
        #endregion
    }
}
