using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data.TvProgMain.ProgObjs;

namespace TVProgViewer.Services.TvProgMain
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
                                                         string sidx, string sord, int page, int rows, string genres);

        /// <summary>
        /// Поиск телепередачи
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public Task<KeyValuePair<int, List<SystemProgramme>>> SearchProgrammeAsync(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates);
    }
}
