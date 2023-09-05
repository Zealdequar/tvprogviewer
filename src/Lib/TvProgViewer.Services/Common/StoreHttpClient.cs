using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;
using System.Xml.Linq;
using TvProgViewer.Core;
using TvProgViewer.Data;
using TvProgViewer.Core.Domain.TvProgMain;
using System.Linq;
using LinqToDB.Data;
using LinqToDB;
using System.Diagnostics;
using TvProgViewer.Services.TvProgMain;
using System.Xml.XPath;
using TVProgViewer.Services.TvProgMain;
using System.Data;

namespace TvProgViewer.Services.Common
{
    /// <summary>
    /// Represents the HTTP client to request current store
    /// </summary>
    public partial class StoreHttpClient
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly IDataProviderManager _dataProviderManager;
        private readonly IRepository<WebResources> _webResourcesRepository;
        private readonly IRepository<TypeProg> _typeProgRepository;
        private static Logger _logger = LogManager.GetLogger("dbupdate");
        private static Dictionary<string, string> _dictChanCodeOld = new Dictionary<string, string>();
        private static Dictionary<string, string> _dictChanCodeNew = new Dictionary<string, string>();

        #endregion

        #region Ctor


        public StoreHttpClient(HttpClient client
        , IWebHelper webHelper
        , IDataProviderManager dataProviderManager
        , IRepository<WebResources> webResourcesRepository
        , IRepository<TypeProg> typeProgRepository)
        {
            //configure client
            client.BaseAddress = new Uri(webHelper.GetStoreLocation());

            _httpClient = client;

            _dataProviderManager = dataProviderManager;
            _webResourcesRepository = webResourcesRepository;
            _typeProgRepository = typeProgRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Keep the current store site alive
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the asynchronous task whose result determines that request completed
        /// </returns>
        public virtual async Task KeepAliveAsync()
        {
            await _httpClient.GetStringAsync(TvProgCommonDefaults.KeepAlivePath);
        }

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
            await _dataProviderManager.DataProvider.QueryProcAsync<int>("dbo.spUpdateXmlChansAndProgs",
                new DataParameter[] {
                new DataParameter("@WRID", webResourceId, DataType.Int32),
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
            await _dataProviderManager.DataProvider.ExecuteNonQueryAsync("spUdtChannelImage",
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

        public virtual async Task UpdateTvProgrammes()
        {
            if (!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday))
                return;
             
            _logger.Info(" =========== Начало обновления ============ ");
            Stopwatch sw = Stopwatch.StartNew();
            IList<WebResources> webResources = await GetAllWebResourcesAsync();

            _logger.Trace("Ресурсы из базы успешно получены: {0}", string.Join(";", webResources.ToList().Select<WebResources, string>(
                   wr => string.Format("WRID = '{0}', FileName = '{1}', ResourceName = '{2}', ResourceUrl ='{3}'",
                   wr.Id, wr.FileName, wr.ResourceName, wr.ResourceUrl))));

            foreach (WebResources webResource in webResources)
            {
                try
                {
                    _logger.Info("Обновление для ID='{0}'", webResource.Id);
                    Stopwatch swLoc = Stopwatch.StartNew();
                    string fileName = webResource.FileName;
                    WebPart.GetWebTVProgramm(new Uri(webResource.ResourceUrl), ref fileName);
                    webResource.FileName = fileName;
                    XDocument xDoc = new XDocument();
                    var sourceType = (await GetTypeProgByIdAsync(webResource.TypeProgId))
                        .TypeName == "Формат XMLTV" ? Enums.TypeProg.XMLTV : Enums.TypeProg.InterTV;
                    if (sourceType == Enums.TypeProg.XMLTV)
                    {
                        if (webResource.Id == 6)
                        {
                            string tempFile = Path.GetTempFileName();

                            using (var streamReader = new StreamReader(webResource.FileName))
                            using (var streamWriter = new StreamWriter(tempFile))
                            {
                                string line;

                                while ((line = streamReader.ReadLine()) != null)
                                {
                                    if (!line.Contains("}{отт@бь)ч"))
                                        streamWriter.WriteLine(line);
                                }
                            }

                            File.Delete(webResource.FileName);
                            File.Move(tempFile, webResource.FileName);
                        }

                        xDoc = XDocument.Load(webResource.FileName, LoadOptions.SetLineInfo);
                        xDoc = WebPart.ReformatXDoc(xDoc);
                        if (webResource.Id == 1)
                        {
                            foreach (XElement xChan in xDoc.XPathSelectElements("tv/channel"))
                            {
                                string id = xChan.Attribute("id").Value;
                                _dictChanCodeOld[xChan.Element("display-name").Value] = id;
                            }
                        }
                        else if (webResource.Id == 2)
                        {
                            foreach (XElement xChan in xDoc.XPathSelectElements("tv/channel"))
                            {
                                string id = xChan.Attribute("id").Value;
                                _dictChanCodeNew[xChan.Element("display-name").Value] = xChan.Attribute("id").Value;
                            }
                        }
                    }
                    else if (sourceType == Enums.TypeProg.InterTV)
                    {
                        if (webResource.Id == 3)
                            xDoc = ConverterProg.InterToXml(webResource.FileName, _dictChanCodeOld);
                        else if (webResource.Id == 4)
                            xDoc = ConverterProg.InterToXml(webResource.FileName, _dictChanCodeNew);
                    }

                    _logger.Info("Каналы и тв-программа ID='{0}' подготовлены к запуску обработки в базе", webResource.Id);
                    await RunXmlToDbAsync(webResource.Id, xDoc.ToString());

                    if (webResource.Id == 2)
                    {
                        _logger.Info("Обработка каналов и тв-программы ID='{0}' завершена. Начало загрузки пиктограмм.", webResource.Id);
                        Stopwatch swPict = Stopwatch.StartNew();
                        foreach (XElement xChan in xDoc.XPathSelectElements("tv/channel"))
                        {
                            string id = xChan.Attribute("id").Value;
                            if (!string.IsNullOrWhiteSpace(id) && xChan.Element("icon") != null)
                            {
                                string src = xChan.Element("icon").Attribute("src").Value;
                                if (!string.IsNullOrWhiteSpace(src))
                                {
                                    byte[] channelIcons = WebPart.GetAndSaveIcon(id, src);
                                    await UpdateIconAsync(Convert.ToInt32(id), src, id + ".gif", "image/gif", "gzip",
                                        channelIcons);
                                }
                            }
                        }
                        swPict.Stop();
                        _logger.Info("Загрузка пиктограмм завершена. Потребовалось {0} сек.", swPict.ElapsedMilliseconds / 1000.0);
                    }
                    swLoc.Stop();
                    _logger.Info("Обновление для ID='{0}' успешно завершено. Затрачено '{1}' сек.", webResource.Id, swLoc.ElapsedMilliseconds / 1000.0);
                }
                catch (Exception ex)
                {
                    _logger.Error("Обновление тв-программы прошло неуспешно: ErrMessage='{0}', StackTrace='{1}'", ex.Message, ex.StackTrace);
                }
            }
            sw.Stop();
            _logger.Info(" ========== Обновление завершено ========== (Затрачено: {0} сек.)", sw.ElapsedMilliseconds / 1000.0);
        }
    }
    #endregion
}
