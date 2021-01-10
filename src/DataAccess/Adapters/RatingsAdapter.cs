using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TVProgViewer.Data.TvProgMain.ProgObjs;
using TVProgViewer.DataAccess.Models;

namespace TVProgViewer.DataAccess.Adapters
{
    public class RatingsAdapter: AdapterBase
    {
        /// <summary>
        /// Получение рейтингов
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <returns>Список рейтингов</returns>
        public List<Rating> GetRatings(long uid)
        {
            List<Rating> ratings = new List<Rating>();
            try
            {
                ratings = (from r in dataContext.Ratings.AsNoTracking()
                           join mp in dataContext.MediaPic.AsNoTracking() on r.IconId equals mp.IconId into gmp
                           from mp in gmp.DefaultIfEmpty()
                           where r.Uid == uid && r.DeleteDate == null
                           select new
                           {
                               RatingID = r.RatingId,
                               UID = r.Uid,
                               IconID = r.IconId,
                               RatingPath = mp.Path25 + mp.FileName,
                               CreateDate = r.CreateDate,
                               RatingName = r.RatingName,
                               Visible = r.Visible,
                               DeleteDate = r.DeleteDate
                           }).Select(mapper.Map<Rating>).ToList();
            }
            catch (Exception ex)
            {

            }
            return ratings;
        }

        /// <summary>
        /// Добавление рейтинга
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="name">Название</param>
        /// <param name="iconId">Идентификатор пиктограммы</param>
        /// <param name="visible">Видимость</param>
        public long AddRating(long uid, string name, int? iconId, bool visible)
        {
            long ratingId = 0;
            try
            {
                Ratings rating = new Ratings
                {
                    CreateDate = DateTimeOffset.Now,
                    Uid = uid,
                    IconId = iconId,
                    RatingName = name,
                    Visible = visible
                };
                dataContext.Ratings.Add(rating);
                dataContext.SaveChanges();
                ratingId = rating.RatingId;
            }
            catch(Exception ex)
            {

            }
            return ratingId;
        }

        /// <summary>
        /// Обновление рейтинга
        /// </summary>
        /// <param name="ratingId">Идентификатор рейтинга</param>
        /// <param name="name">Название</param>
        /// <param name="visible">Видимость</param>
        /// <param name="deleteDate">Удалить после</param>
       /* public void UpdateRating(long ratingId, string name, bool visible, DateTimeOffset? deleteDate = null)
        {
            try
            {
                Ratings rating = dataContext.Ratings.Where(r => r.RatingId == ratingId && r.DeleteDate == null).Single();
                rating.RatingName = name;
                rating.Visible = visible;
                rating.DeleteDate = deleteDate;
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Удаление рейтинга
        /// </summary>
        /// <param name="ratingId">Идентификатор рейтинга</param>
        public void DeleteRating(long ratingId)
        {
            try
            {
                Ratings rating = dataContext.Ratings.Where(r => r.RatingId == ratingId && r.DeleteDate == null).Single();
                rating.DeleteDate = DateTimeOffset.Now;
                foreach(var rc in rating.RatingClassificator.ToList())
                {
                    DeleteRatingClassificator(rc.RatingClassificatorId);
                }
                dataContext.SaveChanges();
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Изменение пиктограммы рейтинга
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="ratingId">Идентификатор рейтинга</param>
        /// <param name="filename">Название файла</param>
        /// <param name="contentType">Тип содержимого</param>
        /// <param name="length">Размер большой пиктограммы в байтах</param>
        /// <param name="pathOrig">Путь к пиктограмме</param>
        /// <param name="path25">Путь к пиктограмме</param>
        public void ChangeRatingImage(long uid, long ratingId, string filename, string contentType,
            int length, string pathOrig, string path25)
        {
            try
            {
                MediaPic mp = new MediaPic()
                {
                    FileName = filename,
                    ContentType = contentType,
                    ContentCoding = "gzip",
                    Length = length,
                    IsSystem = false,
                    PathOrig = pathOrig,
                    Path25 = path25
                };
                dataContext.MediaPic.Add(mp);

                dataContext.SaveChanges();

                Ratings rating = dataContext.Ratings.Single(x => x.RatingId == ratingId && x.DeleteDate == null);
                rating.IconId = mp.IconId;
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Получение классификатора рейтингов
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        public List<RatingClassif> GetRatingClassificators (long uid)
        {
            List<RatingClassif> ratingClassifs = new List<RatingClassif>();
            try
            {
                ratingClassifs = (from rc in dataContext.RatingClassificator.AsNoTracking()
                                  join r in dataContext.Ratings.AsNoTracking() on rc.Rid equals r.RatingId
                                  join mp in dataContext.MediaPic.AsNoTracking() on r.IconId equals mp.IconId into gmp
                                  from mp in gmp.DefaultIfEmpty()
                                  where r.Uid == uid && rc.Uid == uid && r.DeleteDate == null && (rc.DeleteAfterDate == null || rc.DeleteAfterDate > DateTime.Now)
                                  orderby rc.OrderCol
                                  select new
                                  {
                                      RatingClassificatorID = rc.RatingClassificatorId,
                                      RID = rc.Rid,
                                      UID = rc.Uid,
                                      ContainPhrases = rc.ContainPhrases,
                                      NonContainPhrases = rc.NonContainPhrases,
                                      RatingPath = mp.Path25 + mp.FileName,
                                      RatingName = r.RatingName,
                                      OrderCol = rc.OrderCol,
                                      DeleteAfterDate = rc.DeleteAfterDate
                                  }).Select(mapper.Map<RatingClassif>).ToList();
            }
            catch(Exception ex)
            {
                
            }
            return ratingClassifs;
        }

        /// <summary>
        /// Добавление элемента классификации рейтнга
        /// </summary>
        /// <param name="rid">Идентификатор рейтинга</param>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="containPhrases">Содержащаяся фраза</param>
        /// <param name="nonContainPhrases">Не содержащаяся фраза</param>
        /// <param name="deleteAfterDate">Удаляется после наступления даты</param>
        /// <returns>Идентификатор добавленного элемента</returns>
        public long AddRatingClassificator(long rid, long uid, string containPhrases, string nonContainPhrases, DateTime? deleteAfterDate)
        {
            long ratingClassificatorId = 0;
            try
            {
                int? maxOrderCol = dataContext.RatingClassificator.AsNoTracking().Where(rc => rc.Uid == uid).Max(rc => rc.OrderCol);
                RatingClassificator ratingClassif = new RatingClassificator()
                {
                    Rid = rid,
                    Uid = uid,
                    ContainPhrases = containPhrases,
                    NonContainPhrases = nonContainPhrases,
                    OrderCol = (maxOrderCol ?? 0) + 1,
                    DeleteAfterDate = deleteAfterDate
                };
                dataContext.RatingClassificator.Add(ratingClassif);
                dataContext.SaveChanges();
                ratingClassificatorId = ratingClassif.RatingClassificatorId;
            }
            catch(Exception ex)
            {

            }
            return ratingClassificatorId;
        }

        /// <summary>
        /// Обновление элемента классификации рейтинга
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор классификации рейтнига</param>
        /// <param name="rid">Идентификатор рейтинга</param>
        /// <param name="containPhrases">Содержащаяся фраза</param>
        /// <param name="nonContainPrhases">Не содержащаяся фраза</param>
        /// <param name="deleteAfterDate">Будет удалена после наступления даты</param>
        public void UpdateRatingClassificator(long ratingClassificatorId, long rid, string containPhrases,
                                        string nonContainPrhases, DateTime? deleteAfterDate)
        {
            try
            {
                RatingClassificator ratingClassificator = dataContext.RatingClassificator.Single(rc => rc.RatingClassificatorId == ratingClassificatorId &&
                                                                  (deleteAfterDate == null || deleteAfterDate >= DateTime.Now));
                ratingClassificator.Rid = rid;
                ratingClassificator.ContainPhrases = containPhrases;
                ratingClassificator.NonContainPhrases = nonContainPrhases;
                ratingClassificator.DeleteAfterDate = deleteAfterDate;

                dataContext.SaveChanges();
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Удаление элемента классификации рейтингов
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор удаляемого элемента</param>
        public void DeleteRatingClassificator(long ratingClassificatorId)
        {
            try
            {
                RatingClassificator ratingClassificator = dataContext.RatingClassificator
                    .Single(rc => rc.RatingClassificatorId == ratingClassificatorId);
                IEnumerable<RatingClassificator> afterRcs = dataContext.RatingClassificator
                    .Where(rc => rc.Uid == ratingClassificator.Uid && rc.OrderCol > ratingClassificator.OrderCol)
                    .OrderBy(o => o.OrderCol);
                foreach (RatingClassificator rc in afterRcs)
                {
                    int? orderCol = rc.OrderCol;
                    rc.OrderCol = orderCol.HasValue ? --orderCol : null;
                }
                dataContext.RatingClassificator.Remove(ratingClassificator);
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Поднять элемент классификации рейтинга выше по приоритету применимости
        /// </summary>
        /// <param name="ratingClassificatorId"></param>
        public void UpRatingClassificateElem(long ratingClassificatorId)
        {
            try
            {
                RatingClassificator ratingClassificator = dataContext.RatingClassificator.Single(rc => rc.RatingClassificatorId == ratingClassificatorId);
                if (ratingClassificator.OrderCol > 1)
                {
                    RatingClassificator rcAbove = dataContext.RatingClassificator.Single(rc => rc.Uid == ratingClassificator.Uid &&
                                                                                                 rc.OrderCol == ratingClassificator.OrderCol - 1);
                    int? temp = ratingClassificator.OrderCol;
                    ratingClassificator.OrderCol = rcAbove.OrderCol;
                    rcAbove.OrderCol = temp;
                    dataContext.SaveChanges();
                }
            }
            catch(Exception ex)
            {
            }
        }

        /// <summary>
        /// Опустить элемент классификации рейтинга ниже по приоритету применимости 
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор элемента классификации рейтинга</param>
        public void DownRatingClassificateElem(long ratingClassificatorId)
        {
            try
            {
                RatingClassificator ratingClassificator = dataContext.RatingClassificator.Single(rc => rc.RatingClassificatorId == ratingClassificatorId);
                if (ratingClassificator.OrderCol < dataContext.RatingClassificator.AsNoTracking()
                                                    .Where(rc => rc.Uid == ratingClassificator.Uid).Max(rc => rc.OrderCol))
                {
                    RatingClassificator rcUnder = dataContext.RatingClassificator.Single(rc => rc.Uid == ratingClassificator.Uid &&
                                                                                           rc.OrderCol == ratingClassificator.OrderCol + 1);
                    int? temp = ratingClassificator.OrderCol;
                    ratingClassificator.OrderCol = rcUnder.OrderCol;
                    rcUnder.OrderCol = temp;
                    dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }*/
    }
}
