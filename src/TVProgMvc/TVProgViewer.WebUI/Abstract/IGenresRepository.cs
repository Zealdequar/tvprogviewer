using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVProgViewer.WebUI.MainServiceReferences;
using TVProgViewer.BusinessLogic.ProgObjs;

namespace TVProgViewer.WebUI.Abstract
{
    public interface IGenresRepository
    {
        /// <summary>
        /// Получение пользовательских жанров
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        Task<Genre[]> GetGenres(long? uid);

        /// <summary>
        /// Добавление жанра
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="name">Название</param>
        /// <param name="iconId">Идентификатор пиктограммы</param>
        /// <param name="visible">Видимость</param>
        long AddGenre(long? uid,string name, int? iconId, bool visible);

        /// <summary>
        /// Обновление жанра
        /// </summary>
        /// <param name="genreId">Идентификатор жанра</param>
        /// <param name="name">Название</param>
        /// <param name="visible">Видимость</param>
        void UpdateGenre(long genreId, string name, bool visible);

        /// <summary>
        /// Удаление жанра
        /// </summary>
        /// <param name="genreId">Идентификатор жанра</param>
        void DeleteGenre(long genreId);

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
        void ChangeGenreImage(long uid, long genreId, string filename, string contentType,
            int length, string pathOrig, string path25);

        /// <summary>
        /// Получение классификатора жанров
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        Task<GenreClassif[]> GetGenreClassificators(long? uid);

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
        long AddGenreClassificator(long gid, long? uid, string containPhrases, string nonContainPhrases, DateTime? deleteAfterDate);

        /// <summary>
        /// Обновление элемента классификации жанра
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор классификации жанра</param>
        /// <param name="gid">Идентификатор жанра</param>
        /// <param name="containPhrases">Содержащаяся фраза</param>
        /// <param name="nonContainPhrases">Не содержащаяся фраза</param>
        /// <param name="orderCol">Порядковый номер</param>
        /// <param name="deleteAfterDate">Будет удалена после наступления даты</param>
        void UpdateGenreClassificator(long genreClassificatorId, long gid, string containPhrases,
                                      string nonContainPhrases, DateTime? deleteAfterDate);

        /// <summary>
        /// Удаление элемента классификации жанров
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор удаляемого элемента</param>
        void DeleteGenreClassificator(long genreClassificatorId);

        /// <summary>
        /// Поднять элемент классификации жанра выше по приоритету применимости
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор элемента классификации жанра</param>
        void UpGenreClassificateElem(long genreClassificatorId);

        /// <summary>
        /// Опустить элемент классификации жанра ниже по приоритету применимости
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор элемента классификации жанра</param>
        void DownGenreClassificateElem(long genreClassificatorId);
    }
}
