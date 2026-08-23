using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.TvProgMain;
using TvProgViewer.Data.TvProgMain;
using TvProgViewer.Data.TvProgMain.ProgObjs;

namespace TvProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Интерфейс для работы с телепередачами
    /// </summary>
    public partial interface IProgrammeService
    {
        /// <summary>
        /// Получение ТВ-провайдера по идентификатору
        /// </summary>
        /// <param name="providerId">Идентификатор ТВ-провайдера</param>
        /// <returns>ТВ-провайдер</returns>
        Task<TvProgProviders> GetProviderByIdAsync(int providerId);

        /// <summary>
        /// Получение всех ТВ-провайдеров
        /// </summary>
        /// <returns>Список ТВ-провайдеров</returns>
        Task<IList<TvProgProviders>> GetAllProvidersAsync();

        /// <summary>
        /// Получение типа ТВ-программы по его идентификатору
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы</param>
        /// <returns>Тип программы</returns>
        Task<TypeProg> GetTypeProgByIdAsync(int typeProgId);

        /// <summary>
        /// Получение всех типов ТВ-программы
        /// </summary>
        /// <param name="showHidden">Показывать скрытые</param>
        /// <returns>Список типов программ</returns>
        Task<IList<TypeProg>> GetAllTypeProgsAsync(bool showHidden = false);

        /// <summary>
        /// Получение провайдеров телепрограммы вместе с их типами программ
        /// </summary>
        /// <returns>Список пар "провайдер - тип программы"</returns>
        Task<IList<ProviderType>> GetProviderTypeListAsync();

        /// <summary>
        /// Получение периода действия программы телепередач
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы</param>
        /// <returns>Период действия программы</returns>
        Task<ProgPeriod> GetSystemProgrammePeriodAsync(int typeProgId);

        /// <summary>
        /// Получение дней доступной программы
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы</param>
        /// <returns>Список дней</returns>
        Task<List<DaysItem>> GetDaysAsync(int typeProgId);

        /// <summary>
        /// Получение категорий телепередач
        /// </summary>
        /// <returns>Список категорий</returns>
        Task<IList<string>> GetCategoriesAsync();

        /// <summary>
        /// Выборка телепрограммы
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">Режимы выборки: 1 - сейчас; 2 - следом</param>
        /// <param name="category">Категория</param>
        /// <param name="sidx">Поле сортировки</param>
        /// <param name="sord">Порядок сортировки</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="rows">Количество строк на странице</param>
        /// <param name="genres">Идентификаторы жанров через ;</param>
        /// <param name="channels">Идентификаторы каналов через ;</param>
        /// <param name="isAuthorized">Авторизован ли пользователь</param>
        /// <returns>Общее количество записей и страница телепрограммы</returns>
        Task<KeyValuePair<int, List<SystemProgramme>>> GetSystemProgrammesAsync(int typeProgId, DateTimeOffset dateTimeOffset, int mode, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels, bool isAuthorized);

        /// <summary>
        /// Выборка телепрограммы для совершеннолетних
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">Режимы выборки: 1 - сейчас; 2 - следом</param>
        /// <param name="category">Категория</param>
        /// <param name="sidx">Поле сортировки</param>
        /// <param name="sord">Порядок сортировки</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="rows">Количество строк на странице</param>
        /// <param name="genres">Идентификаторы жанров через ;</param>
        /// <param name="channels">Идентификаторы каналов через ;</param>
        /// <param name="isAuthorized">Авторизован ли пользователь</param>
        /// <returns>Общее количество записей и страница телепрограммы</returns>
        Task<KeyValuePair<int, List<SystemProgramme>>> GetUserAdultProgrammesAsync(int typeProgId, DateTimeOffset dateTimeOffset, int mode, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels, bool isAuthorized);

        /// <summary>
        /// Поиск телепередачи
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        /// <param name="category">Категория</param>
        /// <param name="sidx">Поле сортировки</param>
        /// <param name="sord">Порядок сортировки</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="rows">Количество строк на странице</param>
        /// <param name="genres">Идентификаторы жанров через ;</param>
        /// <param name="dates">Даты в формате yyyyMMdd через ;</param>
        /// <param name="channels">Идентификаторы каналов через ;</param>
        /// <param name="isAuthorized">Авторизован ли пользователь</param>
        /// <returns>Общее количество записей и страница телепрограммы</returns>
        Task<KeyValuePair<int, List<SystemProgramme>>> SearchProgrammeAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates, string channels, bool isAuthorized);

        /// <summary>
        /// Поиск телепередачи для совершеннолетних
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        /// <param name="category">Категория</param>
        /// <param name="sidx">Поле сортировки</param>
        /// <param name="sord">Порядок сортировки</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="rows">Количество строк на странице</param>
        /// <param name="genres">Идентификаторы жанров через ;</param>
        /// <param name="dates">Даты в формате yyyyMMdd через ;</param>
        /// <param name="channels">Идентификаторы каналов через ;</param>
        /// <param name="isAuthorized">Авторизован ли пользователь</param>
        /// <returns>Общее количество записей и страница телепрограммы</returns>
        Task<KeyValuePair<int, List<SystemProgramme>>> SearchAdultProgrammeAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates, string channels, bool isAuthorized);

        /// <summary>
        /// Глобальный поиск телепередачи
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        /// <param name="category">Категория</param>
        /// <param name="sidx">Поле сортировки</param>
        /// <param name="sord">Порядок сортировки</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="rows">Количество строк на странице</param>
        /// <param name="genres">Идентификаторы жанров через ;</param>
        /// <param name="channels">Идентификаторы каналов через ;</param>
        /// <param name="isAuthorized">Авторизован ли пользователь</param>
        /// <returns>Общее количество записей и страница телепрограммы</returns>
        Task<KeyValuePair<int, List<SystemProgramme>>> SearchGlobalProgrammeAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels, bool isAuthorized);

        /// <summary>
        /// Глобальный поиск телепередачи для совершеннолетних
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        /// <param name="category">Категория</param>
        /// <param name="sidx">Поле сортировки</param>
        /// <param name="sord">Порядок сортировки</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="rows">Количество строк на странице</param>
        /// <param name="genres">Идентификаторы жанров через ;</param>
        /// <param name="channels">Идентификаторы каналов через ;</param>
        /// <param name="isAuthorized">Авторизован ли пользователь</param>
        /// <returns>Общее количество записей и страница телепрограммы</returns>
        Task<KeyValuePair<int, List<SystemProgramme>>> SearchAdultGlobalProgrammeAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels, bool isAuthorized);

        /// <summary>
        /// Получение пользовательских телепередач за день
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы телепередач</param>
        /// <param name="channelId">Код канала</param>
        /// <param name="tsStart">Время начала выборки</param>
        /// <param name="tsEnd">Время завершения выборки</param>
        /// <param name="category">Категория</param>
        /// <param name="isAuthorized">Авторизован ли пользователь</param>
        /// <returns>Список телепередач за день</returns>
        Task<List<SystemProgramme>> GetUserProgrammesOfDayListAsync(int typeProgId, int channelId, DateTime tsStart, DateTime tsEnd, string category, bool isAuthorized);

        /// <summary>
        /// Получение пользовательских телепередач за день для совершеннолетних
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы телепередач</param>
        /// <param name="channelId">Код канала</param>
        /// <param name="tsStart">Время начала выборки</param>
        /// <param name="tsEnd">Время завершения выборки</param>
        /// <param name="category">Категория</param>
        /// <param name="isAuthorized">Авторизован ли пользователь</param>
        /// <returns>Список телепередач за день</returns>
        Task<List<SystemProgramme>> GetUserAdultProgrammesOfDayListAsync(int typeProgId, int channelId, DateTime tsStart, DateTime tsEnd, string category, bool isAuthorized);

        /// <summary>
        /// Получение всей телепрограммы
        /// </summary>
        /// <param name="page">Страница</param>
        /// <param name="rows">Строки</param>
        /// <returns>Страница телепрограммы</returns>
        Task<List<GdProgramme>> GetAllProgrammes(int page, int rows);

        /// <summary>
        /// Получение состояния телепрограммы (количества передач и дату/время последней передачи)
        /// </summary>
        /// <returns>Состояние телепрограммы</returns>
        Task<GdProgrammeStatus> GetProgrammeStatus();
    }
}
