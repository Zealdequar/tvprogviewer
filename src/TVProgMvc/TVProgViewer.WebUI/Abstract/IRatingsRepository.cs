using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVProgViewer.BusinessLogic.ProgObjs;

namespace TVProgViewer.WebUI.Abstract
{
    public interface IRatingsRepository
    {
        /// <summary>
        /// Получение рейтингов
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <returns>Список рейтингов</returns>
        Task<Rating[]> GetRatings(long uid);

        /// <summary>
        /// Добавление рейтинга
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="name">Название</param>
        /// <param name="iconId">Идентификатор пиктограммы</param>
        /// <param name="visible">Видимость</param>
        long AddRating(long uid, string name, int? iconId, bool visible);

        /// <summary>
        /// Обновление рейтинга
        /// </summary>
        /// <param name="ratingId">Идентификатор рейтинга</param>
        /// <param name="name">Название</param>
        /// <param name="visible">Видимость</param>
        /// <param name="deleteDate">Удалить после</param>
        void UpdateRating(long ratingId, string name, bool visible, DateTimeOffset? deleteDate = null);

        /// <summary>
        /// Удаление рейтинга
        /// </summary>
        /// <param name="ratingId">Идентификатор рейтинга</param>
        void DeleteRating(long ratingId);

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
        void ChangeRatingImage(long uid, long ratingId, string filename, string contentType,
            int length, string pathOrig, string path25);

        /// <summary>
        /// Получение классификатора рейтингов
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        Task<RatingClassif[]> GetRatingClassificators(long uid);

        /// <summary>
        /// Добавление элемента классификации рейтнга
        /// </summary>
        /// <param name="rid">Идентификатор рейтинга</param>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="containPhrases">Содержащаяся фраза</param>
        /// <param name="nonContainPhrases">Не содержащаяся фраза</param>
        /// <param name="deleteAfterDate">Удаляется после наступления даты</param>
        /// <returns>Идентификатор добавленного элемента</returns>
        long AddRatingClassificator(long rid, long uid, string containPhrases, string nonContainPhrases, DateTime? deleteAfterDate);

        /// <summary>
        /// Обновление элемента классификации рейтинга
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор классификации рейтнига</param>
        /// <param name="rid">Идентификатор рейтинга</param>
        /// <param name="containPhrases">Содержащаяся фраза</param>
        /// <param name="nonContainPrhases">Не содержащаяся фраза</param>
        /// <param name="deleteAfterDate">Будет удалена после наступления даты</param>
        void UpdateRatingClassificator(long ratingClassificatorId, long rid, string containPhrases,
                                        string nonContainPrhases, DateTime? deleteAfterDate);

        /// <summary>
        /// Удаление элемента классификации рейтингов
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор удаляемого элемента</param>
        void DeleteRatingClassificator(long ratingClassificatorId);

        /// <summary>
        /// Поднять элемент классификации рейтинга выше по приоритету применимости
        /// </summary>
        /// <param name="ratingClassificatorId"></param>
        void UpRatingClassificateElem(long ratingClassificatorId);

        /// <summary>
        /// Опустить элемент классификации рейтинга ниже по приоритету применимости 
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор элемента классификации рейтинга</param>
        void DownRatingClassificateElem(long ratingClassificatorId);
    }
}
