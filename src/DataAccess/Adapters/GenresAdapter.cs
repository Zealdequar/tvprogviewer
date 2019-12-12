using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVProgViewer.BusinessLogic.ProgObjs;

namespace TVProgViewer.DataAccess.Adapters
{
    public class GenresAdapter: AdapterBase
    {
        /// <summary>
        /// Получение жанров
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <returns>Список жанров</returns>
        public List<Genre> GetGenres (long? uid, bool all)
        {
            List<Genre> genres = new List<Genre>();
            try
            {
                genres = (from g in dataContext.Genres.AsNoTracking()
                          join mp in dataContext.MediaPic.AsNoTracking() on g.IconID equals mp.IconID into gmp
                          from mp in gmp.DefaultIfEmpty()
                          where g.UID == uid && g.DeleteDate == null
                          select new
                          {
                              GenreID = g.GenreID,
                              UID = g.UID,
                              IconID = g.IconID,
                              GenrePath = mp.Path25 + mp.FileName,
                              CreateDate = g.CreateDate,
                              GenreName = g.GenreName,
                              Visible = g.Visible,
                              DeleteDate = g.DeleteDate
                          }).Select(mapper.Map<Genre>).ToList() ;
                if (!all)
                    genres = genres.Where(g => g.Visible).ToList();

            }
            catch (Exception ex)
            {

            }
            return genres;
        }

        /// <summary>
        /// Добавление жанра
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="name">Название</param>
        /// <param name="iconId">Идентификатор пиктограммы</param>
        /// <param name="visible">Видимость</param>
        public long AddGenre(long? uid, string name, int? iconId, bool visible)
        {
            long genreId = 0;
            try
            {
                Genres genre = new Genres
                {
                    CreateDate = DateTimeOffset.Now,
                    UID = uid,
                    IconID = iconId,
                    GenreName = name,
                    Visible = visible
                };

                genre = dataContext.Genres.Add(genre);
                dataContext.SaveChanges();
                genreId = genre.GenreID;
            }
            catch (Exception ex)
            {

            }
            return genreId;
        }

        /// <summary>
        /// Обновление жанра 
        /// </summary>
        /// <param name="genreId">Идентификатор жанра</param>
        /// <param name="name">Название</param>
        /// <param name="visible">Видимость</param>
        /// <param name="deleteDate">Удалить после</param>
        public void UpdateGenre(long genreId, string name, bool visible, DateTimeOffset? deleteDate = null)
        {
            try
            {
                Genres genre = dataContext.Genres.Where(g => g.GenreID == genreId && g.DeleteDate == null).Single();
                genre.GenreName = name;
                genre.Visible = visible;
                genre.DeleteDate = deleteDate;
                dataContext.SaveChanges();
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Удаление жанра
        /// </summary>
        /// <param name="genreId">Идентификатор жанра</param>
        public void DeleteGenre(long genreId)
        {
            try
            {
                Genres genre = dataContext.Genres.Where(g => g.GenreID == genreId && g.DeleteDate == null).Single();
                genre.DeleteDate = DateTimeOffset.Now;
                foreach (var gc in genre.GenreClassificator.ToList())
                       DeleteGenreClassificator(gc.GenreClassificatorID);
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Изменение пиктограммы жанра
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="genreId">Идентификатор пользовательского жанра</param>
        /// <param name="filename">Название файла</param>
        /// <param name="contentType">Тип содержимого</param>
        /// <param name="length">Размер большой пиктограммы в байтах</param>
        /// <param name="pathOrig">Путь к пиктограмме</param>
        /// <param name="path25">Путь к пиктограмме</param>
        public void ChangeGenreImage(long uid, long genreId, string filename, string contentType,
            int length, string pathOrig, string path25)
        {
            try
            {
                MediaPic mp = dataContext.MediaPic.Add(new MediaPic()
                {
                    FileName = filename,
                    ContentType = contentType,
                    ContentCoding = "gzip",
                    Length = length,
                    IsSystem = false,
                    PathOrig = pathOrig,
                    Path25 = path25
                });

                dataContext.SaveChanges();

                Genres genre = dataContext.Genres.Single(x => x.GenreID == genreId && x.DeleteDate == null);
                genre.IconID = mp.IconID;
                dataContext.SaveChanges();

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Получение классификатора жанров
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        public List<GenreClassif> GetGenreClassificators (long? uid)
        {
            List<GenreClassif> genreClassifs = new List<GenreClassif>();
            try
            {
                genreClassifs = (from gc in dataContext.GenreClassificator.AsNoTracking()
                                 join g in dataContext.Genres.AsNoTracking() on gc.GID equals g.GenreID
                                 join mp in dataContext.MediaPic.AsNoTracking() on g.IconID equals mp.IconID into gmp
                                 from mp in gmp.DefaultIfEmpty()
                                 where g.UID == uid && g.Visible && gc.UID == uid && g.DeleteDate == null && (gc.DeleteAfterDate == null || gc.DeleteAfterDate >= DateTime.Now)
                                 orderby gc.OrderCol
                                 select new
                                 {
                                     GenreClassificatorID = gc.GenreClassificatorID,
                                     GID = gc.GID,
                                     UID = gc.UID,
                                     ContainPhrases = gc.ContainPhrases,
                                     NonContainPhrases = gc.NonContainPhrases,
                                     GenrePath = mp.Path25 + mp.FileName,
                                     GenreName = g.GenreName,
                                     OrderCol = gc.OrderCol,
                                     DeleteAfterDate = gc.DeleteAfterDate
                                 }
                                 ).Select(mapper.Map<GenreClassif>).ToList();
            }
            catch (Exception ex)
            {

            }
            return genreClassifs;
        }

        /// <summary>
        /// Добавление элемента классификации жанра
        /// </summary>
        /// <param name="gid">Идентификатор жанра</param>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="containPhrases">Содержащаяся фраза</param>
        /// <param name="nonContainPhrases">Не содержащаяся фраза</param>
        /// <param name="orderCol">Порядковый номер применения</param>
        /// <param name="deleteAfterDate">Удаляется после наступления даты</param>
        /// <returns>Идентификатор добавленного элемента</returns>
        public long AddGenreClassificator(long gid, long? uid, string containPhrases, string nonContainPhrases, DateTime? deleteAfterDate)
        {
            long genreClassificatorId = 0;
            try
            {
                int? maxOrderCol = dataContext.GenreClassificator.AsNoTracking().Where(gc => gc.UID == uid).Max(gc => gc.OrderCol);
                GenreClassificator genreClassif = new GenreClassificator()
                {
                    GID = gid,
                    UID = uid,
                    ContainPhrases = containPhrases,
                    NonContainPhrases = nonContainPhrases,
                    OrderCol = (maxOrderCol ?? 0) + 1,
                    DeleteAfterDate = deleteAfterDate
                };
                genreClassif = dataContext.GenreClassificator.Add(genreClassif);
                dataContext.SaveChanges();
                genreClassificatorId = genreClassif.GenreClassificatorID;
            }
            catch(Exception ex)
            {

            }
            return genreClassificatorId;
        }

        /// <summary>
        /// Обновление элемента классификации жанра
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор классификации жанра</param>
        /// <param name="gid">Идентификатор жанра</param>
        /// <param name="containPhrases">Содержащаяся фраза</param>
        /// <param name="nonContainPhrases">Не содержащаяся фраза</param>
        /// <param name="deleteAfterDate">Будет удалена после наступления даты</param>
        public void UpdateGenreClassificator(long genreClassificatorId, long gid, string containPhrases, 
                                      string nonContainPhrases, DateTime? deleteAfterDate)
        {
            try
            {
                GenreClassificator genreClassificator = dataContext.GenreClassificator.Single(gc => gc.GenreClassificatorID == genreClassificatorId &&
                                                           (deleteAfterDate == null || deleteAfterDate >= DateTime.Now));
                genreClassificator.GID = gid;
                genreClassificator.ContainPhrases = containPhrases;
                genreClassificator.NonContainPhrases = nonContainPhrases;
                genreClassificator.DeleteAfterDate = deleteAfterDate;

                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Удаление элемента классификации жанров
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор удаляемого элемента</param>
        public void DeleteGenreClassificator(long genreClassificatorId)
        {
            try
            {
                GenreClassificator genreClassificator = dataContext.GenreClassificator
                               .Single(gc => gc.GenreClassificatorID == genreClassificatorId);
                IEnumerable<GenreClassificator> afterGcs = dataContext.GenreClassificator
                               .Where(gc => gc.UID == genreClassificator.UID && gc.OrderCol > genreClassificator.OrderCol)
                               .OrderBy(o => o.OrderCol);
                foreach(GenreClassificator gc in afterGcs)
                {
                    int? orderCol = gc.OrderCol;
                    gc.OrderCol = orderCol.HasValue ? --orderCol : null;
                }
                dataContext.GenreClassificator.Remove(genreClassificator);
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Поднять элемент классификации жанра выше по приоритету применимости
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор элемента классификации жанра</param>
        public void UpGenreClassificateElem(long genreClassificatorId)
        {
            try
            {
                GenreClassificator genreClassificator = dataContext.GenreClassificator.Single(gc => gc.GenreClassificatorID == genreClassificatorId);
                if (genreClassificator.OrderCol > 1)
                {
                    GenreClassificator gcAbove = dataContext.GenreClassificator.Single(gc => gc.UID == genreClassificator.UID && 
                                                                                              gc.OrderCol == genreClassificator.OrderCol - 1);
                    int? temp = genreClassificator.OrderCol;
                    genreClassificator.OrderCol = gcAbove.OrderCol;
                    gcAbove.OrderCol = temp;
                    dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Опустить элемент классификации жанра ниже по приоритету применимости
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор элемента классификации жанра</param>
        public void DownGenreClassificateElem(long genreClassificatorId)
        {
            try
            {
                GenreClassificator genreClassificator = dataContext.GenreClassificator.Single(gc => gc.GenreClassificatorID == genreClassificatorId);
                if (genreClassificator.OrderCol < dataContext.GenreClassificator.AsNoTracking()
                                                  .Where(gc => gc.UID == genreClassificator.UID).Max(gc => gc.OrderCol))
                {
                    GenreClassificator gcUnder = dataContext.GenreClassificator.Single(gc => gc.UID == genreClassificator.UID &&
                                                                                              gc.OrderCol == genreClassificator.OrderCol + 1);
                    int? temp = genreClassificator.OrderCol;
                    genreClassificator.OrderCol = gcUnder.OrderCol;
                    gcUnder.OrderCol = temp;
                    dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
