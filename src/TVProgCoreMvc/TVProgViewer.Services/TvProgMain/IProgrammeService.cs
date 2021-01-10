using System;
using System.Collections.Generic;
using System.Text;
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
        public TvProgProviders GetProviderById(int providerId);

        /// <summary>
        /// Получение всех ТВ-провайдеров
        /// </summary>
        public List<TvProgProviders> GetAllProviders();

        /// <summary>
        /// Получение типа ТВ-программы по его идентификатору
        /// </summary>
        /// <returns></returns>
        public TypeProg GetTypeProgById(int typeProgId);

        /// <summary>
        /// Получение всех типов ТВ-программы
        /// </summary>
        public List<TypeProg> GetAllTypeProgs();

        /// <summary>
        /// Получение провайдеров телепрограммы
        /// </summary>
        /// <returns></returns>
        public List<ProviderType> GetProviderTypeList();

        /// <summary>
        /// Получение периода действия программы телепередач
        /// </summary>
        /// <param name="TypeProgId">Идентификатор тип программы</param>
        /// <returns></returns>
        public ProgPeriod GetSystemProgrammePeriod(int typeProgId);

        /// <summary>
        /// Получение категорий телепередач
        /// </summary>
        public List<string> GetCategories();

        /// <summary>
        /// Выборка телепрограммы
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">режимы выборки: 1 - сейчас; 2 - следом</param>
        public KeyValuePair<int, List<SystemProgramme>> GetSystemProgrammes(int TypeProgId, DateTimeOffset dateTimeOffset, int mode, string category,
                                                         string sidx, string sord, int page, int rows, string genres);

        /// <summary>
        /// Поиск телепередачи
        /// </summary>
        /// <param name="TypeProgId">Тип программы телепередач</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public KeyValuePair<int, List<SystemProgramme>> SearchProgramme(int typeProgId, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates);
    }
}
