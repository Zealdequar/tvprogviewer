using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TVProgViewer.BusinessLogic.ProgObjs;
using TVProgViewer.WebUI.Abstract;

namespace TVProgViewer.WebUI.Concrete
{
    public class EFRatingsRepository : BaseEFRepository, IRatingsRepository
    {
        /// <summary>
        /// Получение рейтингов
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <returns>Список рейтингов</returns>
        public Task<Rating[]> GetRatings(long uid)
        {
            return Task<Rating[]>.Factory.StartNew(() => { return TvProgService.GetRatings(uid); });
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
            return TvProgService.AddRating(uid, name, iconId, visible);
        }

        /// <summary>
        /// Обновление рейтинга
        /// </summary>
        /// <param name="ratingId">Идентификатор рейтинга</param>
        /// <param name="name">Название</param>
        /// <param name="visible">Видимость</param>
        /// <param name="deleteDate">Удалить после</param>
        public void UpdateRating(long ratingId, string name, bool visible, DateTimeOffset? deleteDate = null)
        {
            TvProgService.UpdateRating(ratingId, name, visible, deleteDate);
        }

        /// <summary>
        /// Удаление рейтинга
        /// </summary>
        /// <param name="ratingId">Идентификатор рейтинга</param>
        public void DeleteRating(long ratingId)
        {
            TvProgService.DeleteRating(ratingId);
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
        public void ChangeRatingImage(long uid, long ratingId, string filename, string contentType, int length, string pathOrig, string path25)
        {
            TvProgService.ChangeRatingImage(uid, ratingId, filename, contentType, length, pathOrig, path25);
        }

        /// <summary>
        /// Получение классификатора рейтингов
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        public Task<RatingClassif[]> GetRatingClassificators(long uid)
        {
            return Task<RatingClassif[]>.Factory.StartNew(() => { return TvProgService.GetRatingClassificators(uid); });
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
            return TvProgService.AddRatingClassificator(rid, uid, containPhrases, nonContainPhrases, deleteAfterDate);
        }

        /// <summary>
        /// Обновление элемента классификации рейтинга
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор классификации рейтнига</param>
        /// <param name="rid">Идентификатор рейтинга</param>
        /// <param name="containPhrases">Содержащаяся фраза</param>
        /// <param name="nonContainPrhases">Не содержащаяся фраза</param>
        /// <param name="deleteAfterDate">Будет удалена после наступления даты</param>
        public void UpdateRatingClassificator(long ratingClassificatorId, long rid, string containPhrases, string nonContainPrhases, DateTime? deleteAfterDate)
        {
            TvProgService.UpdateRatingClassificator(ratingClassificatorId, rid, containPhrases, nonContainPrhases, deleteAfterDate);
        }

        /// <summary>
        /// Удаление элемента классификации рейтингов
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор удаляемого элемента</param>
        public void DeleteRatingClassificator(long ratingClassificatorId)
        {
            TvProgService.DeleteRatingClassificator(ratingClassificatorId);
        }

        /// <summary>
        /// Поднять элемент классификации рейтинга выше по приоритету применимости
        /// </summary>
        /// <param name="ratingClassificatorId"></param>
        public void UpRatingClassificateElem(long ratingClassificatorId)
        {
            TvProgService.UpRatingClassificateElem(ratingClassificatorId);
        }

        /// <summary>
        /// Опустить элемент классификации рейтинга ниже по приоритету применимости 
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор элемента классификации рейтинга</param>
        public void DownRatingClassificateElem(long ratingClassificatorId)
        {
            TvProgService.DownRatingClassificateElem(ratingClassificatorId);
        }
    }
}