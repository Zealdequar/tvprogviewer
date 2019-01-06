using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TVProgViewer.WebUI.Abstract;
using TVProgViewer.WebUI.MainServiceReferences;
using TVProgViewer.BusinessLogic.ProgObjs;

namespace TVProgViewer.WebUI.Concrete
{
    public class EFGenresRepository : BaseEFRepository, IGenresRepository
    {
        /// <summary>
        /// Получение пользовательских жанров
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        public Task<Genre[]> GetGenres(long? uid)
        {
            return Task<Genre[]>.Factory.StartNew(() => { return TvProgService.GetGenres(uid); });
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
            return TvProgService.AddGenre(uid, name, iconId, visible); 
        }

        /// <summary>
        /// Обновление жанра
        /// </summary>
        /// <param name="genreId">Идентификатор жанра</param>
        /// <param name="name">Название</param>
        /// <param name="visible">Видимость</param>
        public void UpdateGenre(long genreId, string name, bool visible)
        {
            TvProgService.UpdateGenre(genreId, name, visible);
        }

        /// <summary>
        /// Удаление жанра
        /// </summary>
        /// <param name="genreId">Идентификатор жанра</param>
        public void DeleteGenre(long genreId)
        {
            TvProgService.DeleteGenre(genreId);
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
            TvProgService.ChangeGenreImage(uid, genreId, filename, contentType, length, pathOrig, path25);
        }
        
        /// <summary>
        /// Получение классификатора жанров
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        public Task<GenreClassif[]> GetGenreClassificators(long? uid)
        {
            return Task<GenreClassif[]>.Factory.StartNew(() => { return TvProgService.GetGenreClassificators(uid); });
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
            return TvProgService.AddGenreClassificator(gid, uid, containPhrases, nonContainPhrases, deleteAfterDate);
        }

        /// <summary>
        /// Обновление элемента классификации жанра
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор классификации жанра</param>
        /// <param name="gid">Идентификатор жанра</param>
        /// <param name="containPhrases">Содержащаяся фраза</param>
        /// <param name="nonContainPhrases">Не содержащаяся фраза</param>
        /// <param name="orderCol">Порядковый номер</param>
        /// <param name="deleteAfterDate">Будет удалена после наступления даты</param>
        public void UpdateGenreClassificator(long genreClassificatorId, long gid,string containPhrases,
                                      string nonContainPhrases, DateTime? deleteAfterDate)
        {
            TvProgService.UpdateGenreClassificator(genreClassificatorId, gid, containPhrases, nonContainPhrases,deleteAfterDate);
        }


        /// <summary>
        /// Удаление элемента классификации жанров
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор удаляемого элемента</param>
        public void DeleteGenreClassificator(long genreClassificatorId)
        {
            TvProgService.DeleteGenreClassificator(genreClassificatorId);
        }

        /// <summary>
        /// Поднять элемент классификации жанра выше по приоритету применимости
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор элемента классификации жанра</param>
        public void UpGenreClassificateElem(long genreClassificatorId)
        {
            TvProgService.UpGenreClassificateElem(genreClassificatorId);  
        }

        /// <summary>
        /// Опустить элемент классификации жанра ниже по приоритету применимости
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор элемента классификации жанра</param>
        public void DownGenreClassificateElem(long genreClassificatorId)
        {
            TvProgService.DownGenreClassificateElem(genreClassificatorId);
        }
    }
}