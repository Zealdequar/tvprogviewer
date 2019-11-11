using System;
using System.Collections.Generic;
using System.ServiceModel;
using TVProgViewer.BusinessLogic.ProgObjs;
using TVProgViewer.BusinessLogic.Users;

namespace TVProgViewer.TVProgService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IMainService
    {
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
        [OperationContract]
        int AddUser(string userName, string passHash, string passExtend, string lastName,
            string firstName, string middleName, DateTime birthDate, bool? gender, string email, string mobPhone,
            string otherPhone1, string otherPhone2, string address, string gmtZone);

        /// <summary>
        /// Получение хешей
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        [OperationContract]
        SecureData GetHashes(string username);

        /// <summary>
        /// Получение пользователя
        /// </summary>
        /// <param name="uid">Код пользователя</param>
        /// <param name="errCode">Код ошибки</param>
        [OperationContract]
        User GetUser(long uid, out int errCode);

        /// <summary>
        /// Получение провайдера и типов телепередач
        /// </summary>
        [OperationContract]
        List<ProviderType> GetProviderTypeList();

        /// <summary>
        /// Получение системных телеканалов
        /// </summary>
        /// <param name="TVProgProviderID">Идентификатор провайдера программы телепередач</param>
        [OperationContract]
        List<SystemChannel> GetSystemChannelList(int TVProgProviderID);

        /// <summary>
        /// Получение пользовательских телеканалов
        /// </summary>
        /// <param name="uid">Код пользователя</param>
        /// <param name="typeProgID">Идентификатор типа телепередач</param>
        [OperationContract]
        List<UserChannel> GetUserChannelList(long uid, int typeProgID);

        /// <summary>
        /// Получение карты пользовательских телеканалов
        /// </summary>
        /// <param name="uid">Код пользователя</param>
        /// <param name="progProviderID">Идентификатор провайдера</param>
        /// <param name="typeProgID">Идентификатор типа телепередач</param>
        [OperationContract]
        List<SystemChannel> GetUserInSystemChannels(long uid, int progProviderID, int typeProgID);

        /// <summary>
        /// Получение списка ситемных телепередач
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">Режим: 1 - сейчас, 2 - затем</param>
        /// <param name="category">Категория</param>
        [OperationContract]
        KeyValuePair<int, List<SystemProgramme>> GetSystemProgrammeList(int typeProgID, DateTimeOffset dateTimeOffset, int mode, string category, string sidx, string sord, int page, int rows, string genres);

        /// <summary>
        /// Получение периода телепрограммы
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        [OperationContract]
        ProgPeriod GetSystemProgrammePeriod(int typeProgID);

        /// <summary>
        /// Получение системной телепрограммы за день
        /// </summary>
        /// <param name="typeProgID">Тип телепрограммы</param>
        /// <param name="cid">Идентификатор телеканала</param>
        /// <param name="tsStart">Время начала выборки</param>
        /// <param name="tsStop">Время окончания выборки</param>
        [OperationContract]
        List<SystemProgramme> GetSystemProgrammeDayList(int typeProgID, int cid, DateTime tsStart, DateTime tsStop);

        /// <summary>
        /// Добавление пользовательского телеканала
        /// </summary>
        /// <param name="userChannelID">Идентификатор пользовательского телеканала</param>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="tvProgProviderID">Идентификатор источника телепрограммы</param>
        /// <param name="cid">Идентификатор телеканала</param>
        /// <param name="displayName">Пользовательское название телеканала</param>
        /// <param name="orderCol">Порядковый номер телеканала</param>
        [OperationContract]
        void InsertUserChannel(int userChannelID, long uid, int tvProgProviderID, int cid, string displayName, int orderCol);

        /// <summary>
        /// Удаление пользовательского телеканала
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="cid">Мдентификатор телеканала</param>
        [OperationContract]
        void DeleteUserChannel(long uid, int cid);

        /// <summary>
        /// Получение пользователськой телепрограммы
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">Режми: 1 - сейчас, 2 - затем</param>
        /// <param name="category">Категория</param>
        [OperationContract]
        List<SystemProgramme> GetUserProgrammeList(long uid, int typeProgID, DateTimeOffset dateTimeOffset, int mode, string category, string sidx, string sord, int page, int rows, string genres);

        /// <summary>
        /// Получение пользовательской программы телепередач за день
        /// </summary>
        /// <param name="uid">Код пользователя</param>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        /// <param name="cid">Идентификатор телеканала</param>
        /// <param name="tsStart">Время начала выборки</param>
        /// <param name="tsStop">Время окончания выборки</param>
        /// <param name="category">Категория</param>
        [OperationContract]
        List<SystemProgramme> GetUserProgrammeDayList(long uid, int typeProgID, int cid, DateTime tsStart, DateTime tsStop, string category);

        /// <summary>
        /// Получение категорий
        /// </summary>
        [OperationContract]
        List<string> GetCategories();

        /// <summary>
        /// Поиск по системной программе телепередач
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        [OperationContract]
        List<SystemProgramme> SearchProgramme(int typeProgID, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates);

        /// <summary>
        /// Поиск по пользовательской программе телепередач
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        /// <param name="findTitle">Посиковая подстрока</param>
        [OperationContract]
        List<SystemProgramme> SearchUserProgramme(long uid, int typeProgID, string findTitle);

        /// <summary>
        /// Установка каталога существующим пользователям
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        [OperationContract]
        void UpdateCatalog(long uid);

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
        [OperationContract]
        void ChangeChannelImage(long uid, int userChannelId, string filename, string contentType,
           int length, int length25, string pathOrig, string path25);

        /// <summary>
        /// Получение пользовательских жанров
        /// </summary>
        /// <param name="uid">Идентифкатор пользователя</param>
        /// <returns></returns>
        [OperationContract]
        List<Genre> GetGenres(long? uid);

        /// <summary>
        /// Добавление жанра
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="name">Название</param>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="iconId">Идентификатор пиктограммы</param>
        /// <param name="visible">Видимость</param>
        [OperationContract]
        long AddGenre(long? uid, string name, int? iconId, bool visible);

        /// <summary>
        /// Обновление жанра
        /// </summary>
        /// <param name="genreId">Идентификатор жанра</param>
        /// <param name="name">Название жанра</param>
        /// <param name="visible">Видимость</param>
        [OperationContract]
        void UpdateGenre(long genreId, string name, bool visible);

        /// <summary>
        /// Удаление жанра
        /// </summary>
        /// <param name="genreId">Идентификатор жанра</param>
        [OperationContract]
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
        [OperationContract]
        void ChangeGenreImage(long uid, long genreId, string filename, string contentType,
            int length, string pathOrig, string path25);

        /// <summary>
        /// Получение классификатора жанров
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        [OperationContract]
        List<GenreClassif> GetGenreClassificators(long? uid);

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
        [OperationContract]
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
        [OperationContract]
        void UpdateGenreClassificator(long genreClassificatorId, long gid, string containPhrases,
                                      string nonContainPhrases, DateTime? deleteAfterDate);

        /// <summary>
        /// Удаление элемента классификации жанров
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор удаляемого элемента</param>
        [OperationContract]
        void DeleteGenreClassificator(long genreClassificatorId);

        /// <summary>
        /// Поднять элемент классификации жанра выше по приоритету применимости
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор элемента классификации жанра</param>
        [OperationContract]
        void UpGenreClassificateElem(long genreClassificatorId);

        /// <summary>
        /// Опустить элемент классификации жанра ниже по приоритету применимости
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор элемента классификации жанра</param>
        [OperationContract]
        void DownGenreClassificateElem(long genreClassificatorId);

        /// <summary>
        /// Получение рейтингов
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <returns>Список рейтингов</returns>
        [OperationContract]
        List<Rating> GetRatings(long uid);

        /// <summary>
        /// Добавление рейтинга
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="name">Название</param>
        /// <param name="iconId">Идентификатор пиктограммы</param>
        /// <param name="visible">Видимость</param>
        [OperationContract]
        long AddRating(long uid, string name, int? iconId, bool visible);

        /// <summary>
        /// Обновление рейтинга
        /// </summary>
        /// <param name="ratingId">Идентификатор рейтинга</param>
        /// <param name="name">Название</param>
        /// <param name="visible">Видимость</param>
        /// <param name="deleteDate">Удалить после</param>
        [OperationContract]
        void UpdateRating(long ratingId, string name, bool visible, DateTimeOffset? deleteDate = null);

        /// <summary>
        /// Удаление рейтинга
        /// </summary>
        /// <param name="ratingId">Идентификатор рейтинга</param>
        [OperationContract]
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
        [OperationContract]
        void ChangeRatingImage(long uid, long ratingId, string filename, string contentType,
            int length, string pathOrig, string path25);

        /// <summary>
        /// Получение классификатора рейтингов
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        [OperationContract]
        List<RatingClassif> GetRatingClassificators(long uid);

        /// <summary>
        /// Добавление элемента классификации рейтнга
        /// </summary>
        /// <param name="rid">Идентификатор рейтинга</param>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="containPhrases">Содержащаяся фраза</param>
        /// <param name="nonContainPhrases">Не содержащаяся фраза</param>
        /// <param name="deleteAfterDate">Удаляется после наступления даты</param>
        /// <returns>Идентификатор добавленного элемента</returns>
        [OperationContract]
        long AddRatingClassificator(long rid, long uid, string containPhrases, string nonContainPhrases, DateTime? deleteAfterDate);

        /// <summary>
        /// Обновление элемента классификации рейтинга
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор классификации рейтнига</param>
        /// <param name="rid">Идентификатор рейтинга</param>
        /// <param name="containPhrases">Содержащаяся фраза</param>
        /// <param name="nonContainPrhases">Не содержащаяся фраза</param>
        /// <param name="deleteAfterDate">Будет удалена после наступления даты</param>
        [OperationContract]
        void UpdateRatingClassificator(long ratingClassificatorId, long rid, string containPhrases,
                                        string nonContainPrhases, DateTime? deleteAfterDate);

        /// <summary>
        /// Удаление элемента классификации рейтингов
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор удаляемого элемента</param>
        [OperationContract]
        void DeleteRatingClassificator(long ratingClassificatorId);

        /// <summary>
        /// Поднять элемент классификации рейтинга выше по приоритету применимости
        /// </summary>
        /// <param name="ratingClassificatorId"></param>
        [OperationContract]
        void UpRatingClassificateElem(long ratingClassificatorId);

        /// <summary>
        /// Опустить элемент классификации рейтинга ниже по приоритету применимости 
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор элемента классификации рейтинга</param>
        [OperationContract]
        void DownRatingClassificateElem(long ratingClassificatorId);
    }
}
