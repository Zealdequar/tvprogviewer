using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
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
using System.Data;
using System.Collections.Concurrent;
using System.Threading;
using TVProgViewer.Services.TvProgMain;

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
        private readonly IRepository<Channels> _channelsRepository;
        private readonly IRepository<Programmes> _programmesRepository;
        private readonly IRepository<UpdateProgLog> _updateProgLogRepository;
        private readonly IRepository<TvProgCategory> _tvProgCategoryRepository;
        private readonly IRepository<ProgrammesTvProgCategoryMapping> _programmesTvProgCategoryMappingRepository;
        private static readonly Logger _logger = LogManager.GetLogger("dbupdate");
        // Используем ConcurrentDictionary для потокобезопасности
        private static readonly ConcurrentDictionary<string, string> _dictChanCodeOld = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> _dictChanCodeNew = new ConcurrentDictionary<string, string>();
        private static readonly SemaphoreSlim _updateSemaphore = new SemaphoreSlim(1, 1);
        
        // Константы для магических чисел
        private const int WebResourceIdForSpecialProcessing = 6;
        private const int WebResourceIdForOldChannelMapping = 1;
        private const int WebResourceIdForNewChannelMapping = 2;
        private const int TypeProgIdToSkip = 2;
        private const int DaysToKeepProgrammes = 14;
        private const int DaysToDeleteOldProgrammes = 21;
        private const int MinProgrammesPerFutureDay = 100;

        // Транслитерация кириллицы для формирования системного идентификатора категории
        private static readonly Dictionary<char, string> _translitMap = new Dictionary<char, string>
        {
            {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "g"}, {'д', "d"}, {'е', "e"}, {'ё', "e"},
            {'ж', "zh"}, {'з', "z"}, {'и', "i"}, {'й', "y"}, {'к', "k"}, {'л', "l"}, {'м', "m"},
            {'н', "n"}, {'о', "o"}, {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"}, {'у', "u"},
            {'ф', "f"}, {'х', "h"}, {'ц', "c"}, {'ч', "ch"}, {'ш', "sh"}, {'щ', "sch"}, {'ъ', ""},
            {'ы', "y"}, {'ь', ""}, {'э', "e"}, {'ю', "yu"}, {'я', "ya"}
        };

        #endregion

        #region Ctor


        public StoreHttpClient(HttpClient client
        , IWebHelper webHelper
        , IDataProviderManager dataProviderManager
        , IRepository<WebResources> webResourcesRepository
        , IRepository<TypeProg> typeProgRepository
        , IRepository<Channels> channelsRepository
        , IRepository<Programmes> programmesRepository
        , IRepository<UpdateProgLog> updateProgLogRepository
        , IRepository<TvProgCategory> tvProgCategoryRepository
        , IRepository<ProgrammesTvProgCategoryMapping> programmesTvProgCategoryMappingRepository)
        {
            //configure client
            client.BaseAddress = new Uri(webHelper.GetStoreLocation());

            _httpClient = client;

            _dataProviderManager = dataProviderManager;
            _webResourcesRepository = webResourcesRepository;
            _typeProgRepository = typeProgRepository;
            _channelsRepository = channelsRepository;
            _programmesRepository = programmesRepository;
            _updateProgLogRepository = updateProgLogRepository;
            _tvProgCategoryRepository = tvProgCategoryRepository;
            _programmesTvProgCategoryMappingRepository = programmesTvProgCategoryMappingRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Keep the current store site alive
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
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
        /// Получение типов программы передач
        /// </summary>
        /// <param name="showHidden">Показывать ли скрытые записи</param>
        /// <returns></returns>
        public virtual async Task<IList<TypeProg>> GetAllTypeProgAsync(bool showHidden = false)
        {
            var typeProgs = await _typeProgRepository.GetAllAsync(query =>
            {
                return query;
            }, default);
            return await typeProgs.ToListAsync();
        }

        /// <summary>
        /// Получение всех неудалённых каналов
        /// </summary>
        /// <param name="showHidden">Показывать ли скрытые записи</param>
        /// <returns></returns>
        public virtual async Task<IList<Channels>> GetAllChannelsAsync(bool showHidden = false)
        {
            var channels = await _channelsRepository.GetAllAsync(query =>
            {
                return query.Where(x => x.Deleted == null);
            }, default);
            return await channels.ToListAsync();
        }

        /// <summary>
        /// Получение всей телепрограммы
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public virtual async Task<IList<Programmes>> GetAllProgrammesAsync(bool showHidden = false)
        {
            var programmes = await _programmesRepository.GetAllAsync(query =>
            {
                return query;
            }, default);
            return await programmes.ToListAsync();
        }

        /// <summary>
        /// Преобразование строки к дате и времени
        /// </summary>
        /// <param name="rawDateTime">Сырая строка в формате yyyyMMddHHmmss +HHMM</param>
        /// <returns>Дата и время в UTC</returns>
        /// <exception cref="ArgumentException">Если формат строки неверный</exception>
        public DateTime GetDateTimeValue(string rawDateTime)
        {
            if (string.IsNullOrWhiteSpace(rawDateTime))
                throw new ArgumentException("Строка даты и времени не может быть пустой", nameof(rawDateTime));

            if (rawDateTime.Length < 20)
                throw new ArgumentException($"Неверный формат строки даты. Ожидается минимум 20 символов, получено: {rawDateTime.Length}", nameof(rawDateTime));

            try
            {
                int year = int.Parse(rawDateTime.Substring(0, 4));
                int month = int.Parse(rawDateTime.Substring(4, 2));
                int day = int.Parse(rawDateTime.Substring(6, 2));
                int hour = int.Parse(rawDateTime.Substring(8, 2));
                int minute = int.Parse(rawDateTime.Substring(10, 2));
                int second = int.Parse(rawDateTime.Substring(12, 2));
                int hourOffset = int.Parse(rawDateTime.Substring(15, 3));
                int minuteOffset = int.Parse(rawDateTime.Substring(18, 2));
                
                var timeOffset = new TimeSpan(hourOffset, minuteOffset, 0);
                var dateTimeOffset = new DateTimeOffset(year, month, day, hour, minute, second, timeOffset);
                return dateTimeOffset.ToUniversalTime().UtcDateTime;
            }
            catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is FormatException || ex is ArgumentException)
            {
                throw new ArgumentException($"Неверный формат строки даты: '{rawDateTime}'", nameof(rawDateTime), ex);
            }
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
        /// Обновление телеканалов программы передач
        /// </summary>
        public virtual async Task UpdateTvProgrammes()
        {
            if (!await _updateSemaphore.WaitAsync(0))
            {
                _logger.Info("Обновление уже выполняется, повторный запуск пропущен");
                return;
            }

            try
            {
                await UpdateTvProgrammesCore();
            }
            finally
            {
                _updateSemaphore.Release();
            }
        }

        private async Task UpdateTvProgrammesCore()
        {
            _logger.Info(" =========== Начало обновления ============ ");
            var sw = Stopwatch.StartNew();
            
            try
            {
                var webResources = await GetAllWebResourcesAsync();
                var typeProgs = await GetAllTypeProgAsync();
                
                _logger.Trace("Ресурсы из базы успешно получены: {0}", 
                    string.Join(";", webResources.Select(wr => 
                        $"WRID = '{wr.Id}', FileName = '{wr.FileName}', ResourceName = '{wr.ResourceName}', ResourceUrl ='{wr.ResourceUrl}'")));

                foreach (var webResource in webResources)
                {
                    if (webResource.TypeProgId == TypeProgIdToSkip)
                        continue;

                    await ProcessWebResourceAsync(webResource, typeProgs);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Критическая ошибка при обновлении телепрограммы");
                throw;
            }
            finally
            {
                sw.Stop();
                _logger.Info(" ========== Обновление завершено ========== (Затрачено: {0} сек.)", sw.ElapsedMilliseconds / 1000.0);
            }
        }

        /// <summary>
        /// Обработка одного веб-ресурса
        /// </summary>
        private async Task ProcessWebResourceAsync(WebResources webResource, IList<TypeProg> typeProgs)
        {
            var swLoc = Stopwatch.StartNew();
            int channelQty = 0;
            int newChannelQty = 0;
            int programmeQty = 0;
            DateTime minProgDate = DateTime.MinValue;
            DateTime maxProgDate = DateTime.MaxValue;

            try
            {
                _logger.Info("Обновление для ID='{0}'", webResource.Id);
                
                var xDoc = await LoadAndPrepareXmlDocumentAsync(webResource);
                if (xDoc == null)
                {
                    _logger.Warn("Не удалось загрузить XML документ для ресурса ID='{0}'", webResource.Id);
                    return;
                }

                var sourceType = await GetSourceTypeAsync(webResource.TypeProgId);
                if (sourceType == Enums.TypeProg.XMLTV)
                {
                    ProcessChannelMappings(xDoc, webResource.Id);
                }

                _logger.Info("Каналы и тв-программа ID='{0}' подготовлены к запуску обработки", webResource.Id);
                
                var tsUpdateStart = DateTime.Now;
                var channels = await GetAllChannelsAsync();
                
                // Обработка каналов
                (channelQty, newChannelQty) = await ProcessChannelsAsync(xDoc, webResource, typeProgs, channels);
                
                // Обработка программы передач
                (programmeQty, minProgDate, maxProgDate) = await ProcessProgrammesAsync(xDoc, webResource, typeProgs, channels);
                
                // Очистка дубликатов и старых записей
                await CleanupProgrammesAsync();
                
                // Сохранение отчёта
                await SaveUpdateLogAsync(webResource.Id, tsUpdateStart, channelQty, newChannelQty, programmeQty, minProgDate, maxProgDate);
                
                swLoc.Stop();
                _logger.Info("Обновление для ID='{0}' успешно завершено. Затрачено '{1}' сек.", 
                    webResource.Id, swLoc.ElapsedMilliseconds / 1000.0);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Обновление тв-программы для ресурса ID='{0}' прошло неуспешно", webResource.Id);
                await SaveErrorLogAsync(webResource.Id, ex);
            }
        }

        /// <summary>
        /// Загрузка и подготовка XML документа
        /// </summary>
        private async Task<XDocument> LoadAndPrepareXmlDocumentAsync(WebResources webResource)
        {
            string fileName = webResource.FileName;
            var (success, fullFileName) = await WebPart.GetWebTVProgramm(new Uri(webResource.ResourceUrl), fileName);
            
            if (!success)
            {
                _logger.Error("Не удалось загрузить телепрограмму для ресурса ID='{0}'", webResource.Id);
                return null;
            }
            
            webResource.FileName = fullFileName;
            fileName = fullFileName;

            // Специальная обработка для определённого ресурса
            if (webResource.Id == WebResourceIdForSpecialProcessing)
            {
                await CleanupSpecialResourceFileAsync(fileName);
            }

            var xDoc = XDocument.Load(fileName, LoadOptions.SetLineInfo);
            return WebPart.ReformatXDoc(xDoc);
        }

        /// <summary>
        /// Очистка специального файла ресурса от нежелательных строк
        /// </summary>
        private async Task CleanupSpecialResourceFileAsync(string fileName)
        {
            string tempFile = Path.GetTempFileName();
            const string unwantedString = "}{отт@бь)ч";

            try
            {
                using (var streamReader = new StreamReader(fileName))
                using (var streamWriter = new StreamWriter(tempFile))
                {
                    string line;
                    while ((line = await streamReader.ReadLineAsync()) != null)
                    {
                        if (!line.Contains(unwantedString))
                            await streamWriter.WriteLineAsync(line);
                    }
                }

                File.Delete(fileName);
                File.Move(tempFile, fileName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при очистке файла ресурса: {0}", fileName);
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
                throw;
            }
        }

        /// <summary>
        /// Получение типа источника программы
        /// </summary>
        private async Task<Enums.TypeProg> GetSourceTypeAsync(int typeProgId)
        {
            var typeProg = await GetTypeProgByIdAsync(typeProgId);
            return typeProg?.TypeName == "Формат XMLTV" ? Enums.TypeProg.XMLTV : Enums.TypeProg.InterTV;
        }

        /// <summary>
        /// Обработка маппингов каналов
        /// </summary>
        private void ProcessChannelMappings(XDocument xDoc, long webResourceId)
        {
            var channels = xDoc.XPathSelectElements("tv/channel");
            
            if (webResourceId == WebResourceIdForOldChannelMapping)
            {
                foreach (var xChan in channels)
                {
                    var id = xChan.Attribute("id")?.Value;
                    var displayName = xChan.Element("display-name")?.Value;
                    if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(displayName))
                    {
                        _dictChanCodeOld.TryAdd(displayName, id);
                    }
                }
            }
            else if (webResourceId == WebResourceIdForNewChannelMapping)
            {
                foreach (var xChan in channels)
                {
                    var id = xChan.Attribute("id")?.Value;
                    var displayName = xChan.Element("display-name")?.Value;
                    if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(displayName))
                    {
                        _dictChanCodeNew.TryAdd(displayName, id);
                    }
                }
            }
        }

        /// <summary>
        /// Обработка каналов
        /// </summary>
        private async Task<(int channelQty, int newChannelQty)> ProcessChannelsAsync(
            XDocument xDoc, WebResources webResource, IList<TypeProg> typeProgs, IList<Channels> existingChannels)
        {
            var docChannels = xDoc.XPathSelectElements("tv/channel").ToList();
            var channelQty = docChannels.Count;
            
            var internalIds = docChannels
                .Select(c => c.Attribute("id")?.Value)
                .Where(id => !string.IsNullOrWhiteSpace(id) && int.TryParse(id, out _))
                .Select(int.Parse)
                .ToList();
            
            var existingInternalIds = existingChannels
                .Where(c => c.Deleted == null && c.InternalId.HasValue)
                .Select(c => c.InternalId.Value)
                .ToHashSet();
            
            var newInternalIds = internalIds.Except(existingInternalIds).ToList();
            var newChannelQty = newInternalIds.Count;
            
            if (newChannelQty > 0)
            {
                var typeProg = typeProgs.FirstOrDefault(s => s.Id == webResource.TypeProgId);
                if (typeProg == null)
                {
                    _logger.Warn("Тип программы не найден для TypeProgId={0}", webResource.TypeProgId);
                    return (channelQty, 0);
                }
                
                var channelsToInsert = new List<Channels>();
                foreach (var newInternalId in newInternalIds)
                {
                    var xChannel = docChannels.FirstOrDefault(x => 
                        x.Attribute("id")?.Value == newInternalId.ToString());
                    
                    if (xChannel != null)
                    {
                        var displayName = xChannel.Element("display-name")?.Value ?? $"Канал {newInternalId}";
                        channelsToInsert.Add(new Channels
                        {
                            TvProgProviderId = typeProg.TvProgProviderId,
                            InternalId = newInternalId,
                            IconId = null,
                            CreateDate = DateTime.Now,
                            TitleChannel = displayName,
                            IconWebSrc = null,
                            Deleted = null,
                            SysOrderCol = 1000000000
                        });
                    }
                }
                
                if (channelsToInsert.Count > 0)
                {
                    await _channelsRepository.InsertAsync(channelsToInsert);
                }
            }
            
            return (channelQty, newChannelQty);
        }

        /// <summary>
        /// Обработка программы передач
        /// </summary>
        private async Task<(int programmeQty, DateTime minProgDate, DateTime maxProgDate)> ProcessProgrammesAsync(
            XDocument xDoc, WebResources webResource, IList<TypeProg> typeProgs, IList<Channels> channels)
        {
            var docProgrammes = xDoc.XPathSelectElements("tv/programme").ToList();
            var programmeQty = docProgrammes.Count;
            
            if (programmeQty == 0)
                return (0, DateTime.MinValue, DateTime.MaxValue);
            
            var startDates = docProgrammes
                .Select(p => p.Attribute("start")?.Value)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(GetDateTimeValue)
                .ToList();
            
            if (!startDates.Any())
                return (0, DateTime.MinValue, DateTime.MaxValue);
            
            var minProgDate = startDates.Min();
            var maxProgDate = startDates.Max();
            
            // Удаление программ в диапазоне загружаемых дат (вместе с их сопоставлениями категорий)
            var programmes = await GetAllProgrammesAsync();
            var programmesToDelete = programmes
                .Where(x => x.TsStartMo >= minProgDate && x.TsStartMo <= maxProgDate)
                .ToList();
            
            if (programmesToDelete.Count > 0)
            {
                await DeleteProgrammeMappingsAsync(programmesToDelete.Select(p => p.Id).ToList());
                await _programmesRepository.DeleteAsync(programmesToDelete);
            }
            
            // Подготовка новых программ для вставки
            var typeProg = typeProgs.FirstOrDefault(s => s.Id == webResource.TypeProgId);
            if (typeProg == null)
            {
                _logger.Warn("Тип программы не найден для TypeProgId={0}", webResource.TypeProgId);
                return (programmeQty, minProgDate, maxProgDate);
            }
            
            var channelsDict = channels
                .Where(c => c.Deleted == null && c.InternalId.HasValue)
                .ToDictionary(c => c.InternalId.Value, c => c);
            
            var programmeCategoryPairs = new List<(Programmes programme, List<string> categories)>();
            
            foreach (var programmeElement in docProgrammes)
            {
                try
                {
                    var channelAttr = programmeElement.Attribute("channel")?.Value;
                    if (string.IsNullOrWhiteSpace(channelAttr) || !int.TryParse(channelAttr, out var internalChanId))
                        continue;
                    
                    if (!channelsDict.TryGetValue(internalChanId, out var channel))
                        continue;
                    
                    var startAttr = programmeElement.Attribute("start")?.Value;
                    var stopAttr = programmeElement.Attribute("stop")?.Value;
                    
                    if (string.IsNullOrWhiteSpace(startAttr) || string.IsNullOrWhiteSpace(stopAttr))
                        continue;
                    
                    var tsStart = GetDateTimeValue(startAttr);
                    var tsStop = GetDateTimeValue(stopAttr);
                    
                    var title = GetElementValueByLang(programmeElement, "title", "ru");
                    var descr = GetElementValueByLang(programmeElement, "desc", "ru");
                    var categories = GetElementValuesByLang(programmeElement, "category", "ru")
                        .Select(NormalizeCategoryName)
                        .ToList();
                    var firstCategory = categories.Count > 0 ? categories[0] : null;
                    
                    programmeCategoryPairs.Add((new Programmes
                    {
                        TypeProgId = typeProg.Id,
                        ChannelId = channel.Id,
                        InternalChanId = internalChanId,
                        TsStart = tsStart,
                        TsStop = tsStop,
                        TsStartMo = tsStart,
                        TsStopMo = tsStop,
                        Title = title,
                        Descr = descr,
                        Category = firstCategory
                    }, categories));
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Ошибка при обработке элемента программы передач");
                }
            }
            
            if (programmeCategoryPairs.Count == 0)
                return (programmeQty, minProgDate, maxProgDate);
            
            var programmesToInsert = programmeCategoryPairs.Select(p => p.programme).ToList();
            await _programmesRepository.InsertAsync(programmesToInsert);

            // Обеспечиваем наличие категорий в справочнике TvProgCategory (создаём отсутствующие)
            var allCategoryNames = programmeCategoryPairs
                .SelectMany(p => p.categories)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (allCategoryNames.Count > 0)
            {
                var existingCategories = await _tvProgCategoryRepository.GetAllAsync(query => query, default);
                var existingNames = new HashSet<string>(
                    existingCategories.Select(c => c.Name?.Trim()).Where(n => n != null),
                    StringComparer.OrdinalIgnoreCase);

                var toCreate = allCategoryNames.Where(n => !existingNames.Contains(n)).ToList();
                if (toCreate.Count > 0)
                {
                    var maxOrder = existingCategories
                        .Where(c => c.Order.HasValue && c.Order.Value < 1000000)
                        .Select(c => c.Order.Value)
                        .DefaultIfEmpty(5)
                        .Max();

                    var nextOrder = Math.Max(maxOrder + 1, 6);

                    var newCategories = toCreate.Select(name => new TvProgCategory
                    {
                        Name = name,
                        Ident = GenerateCategoryIdent(name),
                        Active = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Order = nextOrder++
                    }).ToList();

                    await _tvProgCategoryRepository.InsertAsync(newCategories);
                }
            }

            // Получаем актуальные идентификаторы из БД, не полагаясь на возврат identity при bulk-вставке,
            // который в данной реализации репозитория не гарантируется.
            var categoryIdByName = (await _tvProgCategoryRepository.GetAllAsync(query => query, default))
                .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                .GroupBy(c => c.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

            var savedProgrammes = await _programmesRepository.GetAllAsync(
                query => query.Where(p => p.TsStartMo >= minProgDate && p.TsStartMo <= maxProgDate), default);

            var programmeByKey = savedProgrammes
                .GroupBy(p => (p.TypeProgId, p.InternalChanId, p.ChannelId, p.TsStartMo, p.TsStopMo, p.Title))
                .ToDictionary(g => g.Key, g => g.First());

            // Создаём сопоставления многие-ко-многим
            var mappings = new List<ProgrammesTvProgCategoryMapping>();
            foreach (var (programme, categories) in programmeCategoryPairs)
            {
                var key = (programme.TypeProgId, programme.InternalChanId, programme.ChannelId, programme.TsStartMo, programme.TsStopMo, programme.Title);
                if (!programmeByKey.TryGetValue(key, out var savedProgramme))
                    continue;

                foreach (var categoryName in categories)
                {
                    if (categoryIdByName.TryGetValue(categoryName, out var categoryId))
                    {
                        mappings.Add(new ProgrammesTvProgCategoryMapping
                        {
                            ProgrammesId = savedProgramme.Id,
                            TvProgCategoryId = categoryId
                        });
                    }
                }
            }

            if (mappings.Count > 0)
            {
                await _programmesTvProgCategoryMappingRepository.InsertAsync(mappings);
            }

            return (programmeQty, minProgDate, maxProgDate);
        }

        /// <summary>
        /// Получение значения элемента по языку
        /// </summary>
        private string GetElementValueByLang(XElement parent, string elementName, string lang)
        {
            return parent.Elements(elementName)
                .FirstOrDefault(x => x.Attribute("lang")?.Value == lang)?.Value;
        }

        /// <summary>
        /// Приводит наименование категории к виду с заглавной первой буквой
        /// </summary>
        private static string NormalizeCategoryName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            name = name.Trim();
            if (name.Length == 1)
                return char.ToUpper(name[0], CultureInfo.InvariantCulture).ToString();

            return char.ToUpper(name[0], CultureInfo.InvariantCulture) + name.Substring(1);
        }

        /// <summary>
        /// Получение значений элемента по языку (для множественных категорий)
        /// </summary>
        private List<string> GetElementValuesByLang(XElement parent, string elementName, string lang)
        {
            if (parent == null)
                return new List<string>();

            return parent.Elements(elementName)
                .Where(x =>
                {
                    var langAttr = x.Attribute("lang")?.Value;
                    return langAttr == null || langAttr == lang;
                })
                .Select(x => x.Value?.Trim())
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .ToList();
        }

        /// <summary>
        /// Формирует системный идентификатор категории (транслитерация в латиницу)
        /// </summary>
        private static string GenerateCategoryIdent(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var ident = new StringBuilder();
            foreach (var ch in name.ToLowerInvariant())
            {
                if (_translitMap.TryGetValue(ch, out var latin) && !string.IsNullOrEmpty(latin))
                    ident.Append(latin);
                else if (char.IsLetterOrDigit(ch))
                    ident.Append(ch);
                else if (ident.Length > 0 && ident[ident.Length - 1] != '-')
                    ident.Append('-');
            }

            var result = ident.ToString().Trim('-');
            return string.IsNullOrEmpty(result) ? null : result.ToUpperInvariant();
        }

        /// <summary>
        /// Удаление сопоставлений категорий для указанных передач
        /// </summary>
        private async Task DeleteProgrammeMappingsAsync(IList<int> programmeIds)
        {
            if (programmeIds == null || programmeIds.Count == 0)
                return;

            await _programmesTvProgCategoryMappingRepository.DeleteAsync(
                m => programmeIds.Contains(m.ProgrammesId));
        }

        /// <summary>
        /// Получение ТВ-дня по дате и времени.
        /// ТВ-день начинается в 02:45 и заканчивается в 02:45 следующего дня.
        /// Если время до 02:45, ТВ-день — это предыдущий календарный день.
        /// </summary>
        private static DateTime GetTvDay(DateTime dateTime)
        {
            var boundary = dateTime.Date.AddHours(2).AddMinutes(45);
            return dateTime >= boundary ? dateTime.Date : dateTime.Date.AddDays(-1);
        }

        /// <summary>
        /// Очистка программ: удаление дубликатов, малозначимых и старых записей
        /// </summary>
        private async Task CleanupProgrammesAsync()
        {
            var programmes = await GetAllProgrammesAsync();
            
            // Удаление дубликатов
            var duplicateGroups = programmes
                .GroupBy(p => new { p.TypeProgId, p.InternalChanId, p.ChannelId, p.TsStartMo, p.TsStopMo })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))
                .ToList();
            
            if (duplicateGroups.Count > 0)
            {
                await DeleteProgrammeMappingsAsync(duplicateGroups.Select(p => p.Id).ToList());
                await _programmesRepository.DeleteAsync(duplicateGroups);
            }
            
            programmes = await GetAllProgrammesAsync();
            
            // Удаление программ, которые превысили две недели от сегодняшнего дня
            var futureDate = DateTime.Now.AddDays(DaysToKeepProgrammes);
            var futureProgrammes = programmes
                .Where(x => x.TsStartMo >= futureDate)
                .ToList();
            
            if (futureProgrammes.Count > 0)
            {
                await DeleteProgrammeMappingsAsync(futureProgrammes.Select(p => p.Id).ToList());
                await _programmesRepository.DeleteAsync(futureProgrammes);
            }
            
            programmes = await GetAllProgrammesAsync();
            
            // Удаление программ за три недели ранее последней передачи
            if (programmes.Count > 0)
            {
                var maxTsStartMo = programmes.Max(x => x.TsStartMo);
                var cutoffDate = maxTsStartMo.AddDays(-DaysToDeleteOldProgrammes);
                
                // Находим ближайший понедельник
                var daysToMonday = ((int)DayOfWeek.Monday - (int)cutoffDate.DayOfWeek + 7) % 7;
                var mondayDate = cutoffDate.AddDays(daysToMonday);
                
                var oldProgrammes = programmes
                    .Where(x => x.TsStartMo < mondayDate)
                    .ToList();
                
                if (oldProgrammes.Count > 0)
                {
                    await DeleteProgrammeMappingsAsync(oldProgrammes.Select(p => p.Id).ToList());
                    await _programmesRepository.DeleteAsync(oldProgrammes);
                }
            }

            programmes = await GetAllProgrammesAsync();

            // Удаление дней с малым количеством записей в будущем
            // ТВ-день: от 02:45 до 02:45 следующего дня
            var now = DateTime.Now;
            var currentTvDay = GetTvDay(now);

            var futureSparseGroups = programmes
                .Where(x => GetTvDay(x.TsStop) > currentTvDay)
                .GroupBy(x => GetTvDay(x.TsStop))
                .Where(g => g.Count() < MinProgrammesPerFutureDay)
                .SelectMany(g => g)
                .ToList();

            if (futureSparseGroups.Count > 0)
            {
                _logger.Info("Удаление дней с малым количеством записей (<{0}): {1} записей в датах: {2}",
                    MinProgrammesPerFutureDay,
                    futureSparseGroups.Count,
                    string.Join(", ", futureSparseGroups
                        .GroupBy(x => GetTvDay(x.TsStop))
                        .Select(g => $"{g.Key:yyyy-MM-dd} ({g.Count()} шт.)")));
                await DeleteProgrammeMappingsAsync(futureSparseGroups.Select(p => p.Id).ToList());
                await _programmesRepository.DeleteAsync(futureSparseGroups);
            }
        }

        /// <summary>
        /// Сохранение лога обновления
        /// </summary>
        private async Task SaveUpdateLogAsync(long webResourceId, DateTime tsUpdateStart, 
            int channelQty, int newChannelQty, int programmeQty, DateTime minProgDate, DateTime maxProgDate)
        {
            var tsElapsed = DateTime.Now - tsUpdateStart;
            await _updateProgLogRepository.InsertAsync(new UpdateProgLog
            {
                WebResourceId = (int)webResourceId,
                TsUpdateStart = tsUpdateStart,
                TsUpdateEnd = DateTime.Now,
                UdtElapsedSec = (int)tsElapsed.TotalSeconds,
                MinProgDate = minProgDate,
                MaxProgDate = maxProgDate,
                QtyChans = channelQty,
                QtyProgrammes = programmeQty,
                QtyNewChans = newChannelQty,
                QtyNewProgrammes = programmeQty,
                IsSuccess = true,
                ErrorMessage = null
            });
        }

        /// <summary>
        /// Сохранение лога ошибки
        /// </summary>
        private async Task SaveErrorLogAsync(long webResourceId, Exception ex)
        {
            try
            {
                await _updateProgLogRepository.InsertAsync(new UpdateProgLog
                {
                    WebResourceId = (int) webResourceId,
                    TsUpdateStart = DateTime.Now,
                    TsUpdateEnd = DateTime.Now,
                    UdtElapsedSec = 0,
                    MinProgDate = DateTime.MinValue,
                    MaxProgDate = DateTime.MaxValue,
                    QtyChans = 0,
                    QtyProgrammes = 0,
                    QtyNewChans = 0,
                    QtyNewProgrammes = 0,
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
            catch (Exception logEx)
            {
                _logger.Error(logEx, "Ошибка при сохранении лога ошибки для ресурса ID='{0}'", webResourceId);
            }
        }
    }
    #endregion
}
