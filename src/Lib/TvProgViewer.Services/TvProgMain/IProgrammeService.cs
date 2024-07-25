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
        public Task<TvProgProviders> GetProviderByIdAsync(int providerId);

        /// <summary>
        /// Получение всех ТВ-провайдеров
        /// </summary>
        public Task<IList<TvProgProviders>> GetAllProvidersAsync();

        /// <summary>
        /// Получение типа ТВ-программы по его идентификатору
        /// </summary>
        /// <returns></returns>
        public Task<TypeProg> GetTypeProgByIdAsync(int typeProgId);

        /// <summary>
        /// Получение всех типов ТВ-программы
        /// </summary>
        /// <param name="showHidden">Показывать скрытые</param>
        public Task<IList<TypeProg>> GetAllTypeProgsAsync(bool showHidden);

        /// <summary>
        /// Получение провайдеров телепрограммы
        /// </summary>
        /// <returns></returns>
        public Task<IList<ProviderType>> GetProviderTypeListAsync();

        /// <summary>
        /// Получение периода действия программы телепередач
        /// </summary>
        /// <param name="TypeProgId">Идентификатор тип программы</param>
        /// <returns></returns>
        public Task<ProgPeriod> GetSystemProgrammePeriodAsync(int typeProgId);


        /// <summary>
        /// Получение дней доступной программы
        /// </summary>
        /// <param name="typeProg">Идентификатор типа программы</param>
        /// <returns>Список дней</returns>
        public Task<List<DaysItem>> GetDaysAsync(int typeProg);

        /// <summary>
        /// Получение категорий телепередач
        /// </summary>
        public Task<IList<string>> GetCategoriesAsync();

        /// <summary>
        /// Выборка телепрограммы
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">режимы выборки: 1 - сейчас; 2 - следом</param>
        public Task<KeyValuePair<int, List<SystemProgramme>>> GetSystemProgrammesAsync(int TypeProgId, DateTimeOffset dateTimeOffset, int mode, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels);

        /// <summary>
        /// Выборка телепрограммы для совершеннолетних
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">режимы выборки: 1 - сейчас; 2 - следом</param>
        public Task<KeyValuePair<int, List<SystemProgramme>>> GetUserAdultProgrammesAsync(int TypeProgId, DateTimeOffset dateTimeOffset, int mode, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels);
        
        /// <summary>
        /// Поиск телепередачи
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public Task<KeyValuePair<int, List<SystemProgramme>>> SearchProgrammeAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates, string channels);
        /// <summary>
        /// Поиск телепередачи для совершеннолетних
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public Task<KeyValuePair<int, List<SystemProgramme>>> SearchAdultProgrammeAsync(int typeProgId, string findTitle, string category
                                                               , string sidx, string sord, int page, int rows, string genres, string dates, string channels);
        /// <summary>
        /// Глобальный поиск телепередачи
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        /// <param name="category">Категория</param>
        /// <param name="genres">Жанры</param>
        /// <param name="channels">Каналы</param>
        public Task<KeyValuePair<int, List<SystemProgramme>>> SearchGlobalProgrammeAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels);

        /// <summary>
        /// Глобальный поиск телепередачи для совершеннолетних
        /// </summary>
        /// <param name="typeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        /// <param name="category">Категория</param>
        /// <param name="genres">Жанры</param>
        /// <param name="channels">Каналы</param>
        public Task<KeyValuePair<int, List<SystemProgramme>>> SearchAdultGlobalProgrammeAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string channels);

        /// <summary>
        /// Получение пользовательских телепередач за день
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа программы телепередач</param>
        /// <param name="channelId">Код канала</param>
        /// <param name="tsStart">Время начала выборки</param>
        /// <param name="tsEnd">Время завершения выборки</param>
        /// <param name="category">Категория</param>
        public Task<List<SystemProgramme>> GetUserProgrammesOfDayListAsync(int typeProgId, int channeldId, DateTime tsStart, DateTime tsEnd, string category);

        /// <summary>
        /// Получение пользовательских телепередач за день для совершеннолетних
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа программы телепередач</param>
        /// <param name="channelId">Код канала</param>
        /// <param name="tsStart">Время начала выборки</param>
        /// <param name="tsEnd">Время завершения выборки</param>
        /// <param name="category">Категория</param>
        public Task<List<SystemProgramme>> GetUserAdultProgrammesOfDayListAsync(int typeProgId, int channelId, DateTime tsStart, DateTime tsEnd, string category);

        /// <summary>
        /// Получение всей телепрограммы
        /// </summary>
        /// <param name="page">Страница</param>
        /// <param name="rows">Строки</param>
        public Task<List<GdProgramme>> GetAllProgrammes(int page, int rows);
    }
}
