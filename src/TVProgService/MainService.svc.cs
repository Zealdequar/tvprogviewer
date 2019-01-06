using System;
using System.Collections.Generic;
using System.ServiceModel;
using TVProgViewer.BusinessLogic.ProgObjs;
using TVProgViewer.BusinessLogic.Users;
using TVProgViewer.DataAccess.Adapters;

namespace TVProgViewer.TVProgService
{
    public class MainService : IMainService
    {
        private UsersAdapter _ua = new UsersAdapter();
        private TvProgAdapter _tpa = new TvProgAdapter();
        private GenresAdapter _ga = new GenresAdapter();
        private RatingsAdapter _ra = new RatingsAdapter();

        #region Пользователи

        /// <summary>
        /// Добавление пользователя
        /// </summary>
        /// <param name="userName">Логин (имя пользователя)</param>
        /// <param name="passHash">Хеш пароля</param>
        /// <param name="passExtend">Хеш соли</param>
        /// <param name="lastName">Фамилия</param>
        /// <param name="firstName">Имя</param>
        /// <param name="middleName">Отчество</param>
        /// <param name="birthDate">Дата рождения</param>
        /// <param name="gender">Пол</param>
        /// <param name="email">Адрес электронной почты</param>
        /// <param name="mobPhone">Номер моб. телефона</param>
        /// <param name="otherPhone1">Доп. номер 1</param>
        /// <param name="otherPhone2">Доп. номер 2</param>
        /// <param name="address">Адрес</param>
        /// <param name="gmtZone">Часовой пояс</param>
        public int AddUser(string userName, string passHash, string passExtend, string lastName,
            string firstName, string middleName, DateTime birthDate, bool? gender, string email, string mobPhone,
            string otherPhone1, string otherPhone2, string address, string gmtZone)
        {
            return _ua.UserStart(userName, passHash, passExtend, lastName, firstName,
                middleName, birthDate, gender, email, mobPhone, otherPhone1, otherPhone2, address, gmtZone);
        }

        /// <summary>
        /// Получение хешей
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        public SecureData GetHashes(string userName)
        {
            return _ua.GetHashes(userName);
        }

        /// <summary>
        /// Получение пользователя
        /// </summary>
        /// <param name="uid">Код пользователя</param>
        /// <param name="errCode">Код ошибки</param>
        public User GetUser(long uid, out int errCode)
        {
            return _ua.GetUser(uid, out errCode);
        }

        #endregion

        #region Телепрограмма
        /// <summary>
        /// Получение провайдера и типов телепередач
        /// </summary>
        public List<ProviderType> GetProviderTypeList()
        {
            return _tpa.GetTvProviderTypes();
        }

        /// <summary>
        /// Получение списка ситемных телепередач
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">Режим: 1 - сейчас, 2 - затем</param>
        /// <param name="category">Категория</param>
        public KeyValuePair<int, List<SystemProgramme>> GetSystemProgrammeList(int typeProgID, DateTimeOffset dateTimeOffset, int mode, string category, 
            string sidx, string sord, int page, int rows)
        {
            return _tpa.GetSystemProgrammes(typeProgID, dateTimeOffset, mode, category, sidx, sord, page, rows);
        }

        /// <summary>
        /// Получение периода телепрограммы
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        public ProgPeriod GetSystemProgrammePeriod(int typeProgID)
        {
            return _tpa.GetSystemProgrammePeriod(typeProgID);
        }

        /// <summary>
        /// Получение системной телепрограммы за день
        /// </summary>
        /// <param name="typeProgID">Тип телепрограммы</param>
        /// <param name="cid">Идентификатор телеканала</param>
        /// <param name="tsStart">Время начала выборки</param>
        /// <param name="tsStop">Время окончания выборки</param>
        public List<SystemProgramme> GetSystemProgrammeDayList(int typeProgID, int cid, DateTime tsStart, DateTime tsStop)
        {
            return _tpa.GetProgrammeOfDay(typeProgID, cid, tsStart, tsStop);
        }



        /// <summary>
        /// Получение пользователськой телепрограммы
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">Режми: 1 - сейчас, 2 - затем</param>
        /// <param name="category">Категория</param>
        public KeyValuePair<int, List<SystemProgramme>> GetUserProgrammeList(long uid, int typeProgID, DateTimeOffset dateTimeOffset, int mode, string category
                                    , string sidx, string sord, int page, int rows)
        {
            return _tpa.GetUserProgrammes(uid, typeProgID, dateTimeOffset, mode, category, sidx, sord, page, rows);
        }

        /// <summary>
        /// Получение пользовательской программы телепередач за день
        /// </summary>
        /// <param name="uid">Код пользователя</param>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        /// <param name="cid">Идентификатор телеканала</param>
        /// <param name="tsStart">Время начала выборки</param>
        /// <param name="tsStop">Время окончания выборки</param>
        /// <param name="category">Категория</param>
        public List<SystemProgramme> GetUserProgrammeDayList(long uid, int typeProgID, int cid, DateTime tsStart, DateTime tsStop, string category)
        {
            return _tpa.GetUserProgrammeOfDay(uid, typeProgID, cid, tsStart, tsStop, category);
        }

        /// <summary>
        /// Получение категорий
        /// </summary>
        public List<string> GetCategories()
        {
            return _tpa.GetCategories();
        }

        /// <summary>
        /// Поиск по системной программе телепередач
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public KeyValuePair<int,List<SystemProgramme>> SearchProgramme(int typeProgID, string findTitle, string sidx, string sord, int page, int rows)
        {
            return _tpa.SearchProgramme(typeProgID, findTitle, sidx, sord, page, rows);
        }

        /// <summary>
        /// Поиск по пользовательской программе телепередач
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        /// <param name="findTitle">Посиковая подстрока</param>
        public List<SystemProgramme> SearchUserProgramme(long uid, int typeProgID, string findTitle)
        {
            return _tpa.SearchUserProgramme(uid, typeProgID, findTitle);
        }

        /// <summary>
        /// Установка каталога существующим пользователям
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        public void UpdateCatalog(long uid)
        {
            _ua.UpdateCatalog(uid);
        }
        #endregion

        #region Телеканалы

        /// <summary>
        /// Получение системных телеканалов
        /// </summary>
        /// <param name="TVProgProviderID">Идентификатор провайдера программы телепередач</param>
        public List<SystemChannel> GetSystemChannelList(int tvProgProviderID)
        {
            return _tpa.GetSystemChannels(tvProgProviderID);
        }

        /// <summary>
        /// Получение пользовательских телеканалов
        /// </summary>
        /// <param name="uid">Код пользователя</param>
        /// <param name="typeProgID">Идентификатор типа телепередач</param>
        public List<UserChannel> GetUserChannelList(long uid, int typeProgID)
        {
            return _tpa.GetUserChannels(uid, typeProgID);
        }

        /// <summary>
        /// Добавление пользовательского телеканала
        /// </summary>
        /// <param name="userChannelID">Идентификатор пользовательского телеканала</param>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="tvProgProviderID">Идентификатор источника телепрограммы</param>
        /// <param name="cid">Идентификатор телеканала</param>
        /// <param name="displayName">Пользовательское название телеканала</param>
        /// <param name="orderCol">Порядковый номер телеканала</param>
        public void InsertUserChannel(int userChannelID, long uid, int tvProgProviderID, int cid, string displayName, int orderCol)
        {
            _tpa.InsertUserChannel(userChannelID, uid, tvProgProviderID, cid, displayName, orderCol);
        }

        /// <summary>
        /// Удаление пользовательского телеканала
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="cid">Мдентификатор телеканала</param>
        public void DeleteUserChannel(long uid, int cid)
        {
            _tpa.DeleteUserChannel(uid, cid);
        }

        /// <summary>
        /// Получение карты пользовательских телеканалов
        /// </summary>
        /// <param name="uid">Код пользователя</param>
        /// <param name="progProviderID">Идентификатор провайдера</param>
        /// <param name="typeProgID">Идентификатор типа телепередач</param>
        public List<SystemChannel> GetUserInSystemChannels(long uid, int progProviderID, int typeProgID)
        {
            return _tpa.GetUserInSystemChannels(uid, progProviderID, typeProgID);
        }

        /// <summary>
        /// Изменение пиктограммы телеканала
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="userChannelId">Идентификатор пользовательского телеканала</param>
        /// <param name="filename">Название файла</param>
        /// <param name="contentType">Тип содержимого</param>
        /// <param name="length">Размер большой пиктограммы в байтах</param>
        /// <param name="length25">Размер маленькой пиктограммы в байтах</param>
        /// <param name="pathOrig">Путь к большой пиктограмме</param>
        /// <param name="path25">Путь к маленькой пиктограмме</param>
        public void ChangeChannelImage(long uid, int userChannelId, string filename, string contentType,
          int length, int length25, string pathOrig, string path25)
        {
            _tpa.ChangeChannelImage(uid, userChannelId, filename, contentType, length, length25, pathOrig, path25);
        }
        #endregion

        #region Жанры
        /// <summary>
        /// Получение пользовательских жанров
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        public List<Genre> GetGenres(long? uid)
        {
            return _ga.GetGenres(uid);
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
            return _ga.AddGenre(uid, name, iconId, visible);
        }

        /// <summary>
        /// Обновление жанра
        /// </summary>
        /// <param name="genreId">Идентификатор жанра</param>
        /// <param name="name">Название</param>
        /// <param name="visible">Видимость</param>
        public void UpdateGenre(long genreId, string name, bool visible)
        {
            _ga.UpdateGenre(genreId, name, visible);
        }

        /// <summary>
        /// Удаление жанра
        /// </summary>
        /// <param name="genreId">Идентификатор жанра</param>
        public void DeleteGenre(long genreId)
        {
            _ga.DeleteGenre(genreId);
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
            _ga.ChangeGenreImage(uid, genreId, filename, contentType, length, pathOrig, path25);
        }
        #endregion

        #region Классификатор жанров
        /// <summary>
        /// Получение классификатора жанров
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        public List<GenreClassif> GetGenreClassificators(long? uid)
        {
            return _ga.GetGenreClassificators(uid);
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
            return _ga.AddGenreClassificator(gid, uid, containPhrases, nonContainPhrases, deleteAfterDate);
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
        public void UpdateGenreClassificator(long genreClassificatorId, long gid, string containPhrases,
                                      string nonContainPhrases, DateTime? deleteAfterDate)
        {
            _ga.UpdateGenreClassificator(genreClassificatorId, gid, containPhrases, nonContainPhrases, deleteAfterDate);
        }

        /// <summary>
        /// Удаление элемента классификации жанров
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор удаляемого элемента</param>
        public void DeleteGenreClassificator(long genreClassificatorId)
        {
            _ga.DeleteGenreClassificator(genreClassificatorId);
        }

        /// <summary>
        /// Поднять элемент классификации жанра выше по приоритету применимости
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор элемента классификации жанра</param>
        public void UpGenreClassificateElem(long genreClassificatorId)
        {
            _ga.UpGenreClassificateElem(genreClassificatorId);
        }

        /// <summary>
        /// Опустить элемент классификации жанра ниже по приоритету применимости
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор элемента классификации жанра</param>
        public void DownGenreClassificateElem(long genreClassificatorId)
        {
            _ga.DownGenreClassificateElem(genreClassificatorId);
        }
        #endregion

        #region Рейтинги

        /// <summary>
        /// Получение рейтингов
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <returns>Список рейтингов</returns>
        public List<Rating> GetRatings(long uid)
        {
            return _ra.GetRatings(uid);
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
            return _ra.AddRating(uid, name, iconId, visible);
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
            _ra.UpdateRating(ratingId, name, visible, deleteDate);
        }

        /// <summary>
        /// Удаление рейтинга
        /// </summary>
        /// <param name="ratingId">Идентификатор рейтинга</param>
        public void DeleteRating(long ratingId)
        {
            _ra.DeleteRating(ratingId);
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
            _ra.ChangeRatingImage(uid, ratingId, filename, contentType, length, pathOrig, path25);
        }

        #endregion

        #region Классификатор рейтингов

        /// <summary>
        /// Получение классификатора рейтингов
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        public List<RatingClassif> GetRatingClassificators(long uid)
        {
            return _ra.GetRatingClassificators(uid);
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
            return _ra.AddRatingClassificator(rid, uid, containPhrases, nonContainPhrases, deleteAfterDate);
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
            _ra.UpdateRatingClassificator(ratingClassificatorId, rid, containPhrases, nonContainPrhases, deleteAfterDate);
        }

        /// <summary>
        /// Удаление элемента классификации рейтингов
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор удаляемого элемента</param>
        public void DeleteRatingClassificator(long ratingClassificatorId)
        {
            _ra.DeleteRatingClassificator(ratingClassificatorId);
        }

        /// <summary>
        /// Поднять элемент классификации рейтинга выше по приоритету применимости
        /// </summary>
        /// <param name="ratingClassificatorId"></param>
        public void UpRatingClassificateElem(long ratingClassificatorId)
        {
            _ra.UpRatingClassificateElem(ratingClassificatorId);
        }

        /// <summary>
        /// Опустить элемент классификации рейтинга ниже по приоритету применимости 
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор элемента классификации рейтинга</param>
        public void DownRatingClassificateElem(long ratingClassificatorId)
        {
            _ra.DownRatingClassificateElem(ratingClassificatorId);
        }

        #endregion
    }
}