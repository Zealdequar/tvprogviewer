using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Data;

namespace TVProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Обеспечивает работу обновлений
    /// </summary>
    public class UpdaterService: IUpdaterService
    {
        #region Поля

        private readonly IDataProviderManager _dataProviderManager;
        private readonly IRepository<WebResources> _webResourcesRepository;
        private readonly IRepository<TypeProg> _typeProgRepository;
        

        #endregion

        #region Конструктор
        public UpdaterService(
            IDataProviderManager dataProviderManager,
            IRepository<WebResources> webResourcesRepository,
            IRepository<TypeProg> typeProgRepository)
        {
            _dataProviderManager = dataProviderManager;
            _webResourcesRepository = webResourcesRepository;
            _typeProgRepository = typeProgRepository;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получение всех веб-ресурсов
        /// </summary>
        /// <param name="showHidden">Показывать ли скрытые записи</param>
        public virtual async Task<IList<WebResources>> GetAllWebResourcesAsync(bool showHidden = false)
        {
            var webResources = await _webResourcesRepository.GetAllAsync(query =>
            {
                return query;
            }, default);
            return await webResources.ToListAsync();
        }

        /// <summary>
        /// Получение типа программы по идентификатору
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы</param>
        /// <returns>Тип программы</returns>
        public virtual async Task<TypeProg> GetTypeProgByIdAsync(int typeProgId)
        {
            return await _typeProgRepository.GetByIdAsync(typeProgId, cache => default);
        }

        /// <summary>
        /// Запуск хранимой процедуры обработки и обновления телепрограммы
        /// </summary>
        /// <param name="webResourceId">Идентификатор веб-ресурса</param>
        /// <param name="tvProgXml">Xml-формат телепрограммы</param>
        /// <returns></returns>
        public virtual async Task RunXmlToDbAsync(int webResourceId, string tvProgXml)
        {
            await _dataProviderManager.DataProvider.ExecuteProcAsync("spUpdateXmlChansAndProgs",
                new DataParameter[] {
                new DataParameter("@WRID", webResourceId),
                new DataParameter("@ChanAndProg", tvProgXml, DataType.Xml)
                });
        }

        /// <summary>
        /// Обновление данных о пиктограмме
        /// </summary>
        /// <param name="channelId">Идентификатор канала</param>
        /// <param name="iconWebSrc">Адрес пиктограммы в интернете</param>
        /// <param name="channelIconName">Название пиктограммы</param>
        /// <param name="contentType">Тип содержимого (ContentType) пиктограммы</param>
        /// <param name="contentCoding">Кодировка пиктограммы</param>
        /// <param name="channelOrigIcon">Оригинальная пиктограмма</param>
        public virtual async Task UpdateIconAsync(int channelId, string iconWebSrc, string channelIconName, string contentType, string contentCoding
            , byte[] channelOrigIcon)
        {
            await _dataProviderManager.DataProvider.ExecuteProcAsync("spUdtChannelImage",
                new DataParameter[] {
                    new DataParameter("@CID", channelId, DataType.Int32),
                    new DataParameter("@IconWebSrc", iconWebSrc, DataType.NVarChar),
                    new DataParameter("@ChannelIconName", channelIconName, DataType.NVarChar),
                    new DataParameter("@ContentType", contentType, DataType.NVarChar),
                    new DataParameter("@ContentCoding", contentCoding, DataType.NVarChar),
                    new DataParameter("@ChannelOrigIcon", channelOrigIcon, DataType.VarBinary),
                    new DataParameter("@IsSystem", 1, DataType.Boolean),
                    new DataParameter("@ErrCode", DataType.NVarChar){Direction = System.Data.ParameterDirection.Output}
                });
        }

        #endregion



    }
}
