using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUglify.Helpers;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Data.TvProgMain.ProgObjs;
using TvProgViewer.Services.TvProgMain;
using TvProgViewer.Services.Users;
using TvProgViewer.WebUI.Infrastructure;
using TvProgViewer.WebUI.Models.Tree;

namespace TvProgViewer.WebUI.Controllers
{
    public partial class HomeController : BasePublicController
    {
        #region Поля

        private readonly IProgrammeService _programmeService;
        private readonly IChannelService _channelService;
        private readonly IGenreService _genreService;
        private readonly IUserService _userService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly SecuritySettings _securitySettings;

        #endregion

        #region Классы

        [DataContract]
        internal class Rules
        {
            [DataMember]
            public string field { get; set; }
            [DataMember]
            public string op { get; set; }
            [DataMember]
            public string data { get; set; }
        }

        [DataContract]
        internal class Filt
        {
            [DataMember]
            public string groupOp { get; set; }
            [DataMember]
            public List<Rules> rules { get; set; }

        }

        #endregion

        #region Конструктор

        public HomeController(IProgrammeService programmeService,
            IChannelService channelService,
            IGenreService genreService,
            IUserService userService,
            IWebHelper webHelper,
            IWorkContext workContext,
            SecuritySettings securitySettings)
        {
            _programmeService = programmeService;
            _channelService = channelService;
            _genreService = genreService;
            _userService = userService;
            _webHelper = webHelper;
            _workContext = workContext;
            _securitySettings = securitySettings;
        }

        #endregion

        #region Методы

        public virtual IActionResult Index()
        {
            return View();
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetGenres()
        {
            return Json(await _genreService.GetGenresAsync(null, true));
        }

        /// <summary>
        /// Получение списка категорий
        /// </summary>
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetCategories()
        {
            return Json(await _programmeService.GetCategoriesAsync());
        }

        /// <summary>
        /// Определяет, является ли текущий пользователь совершеннолетним
        /// (для неавторизованных всегда возвращается false)
        /// </summary>
        private async Task<bool> IsAdultUserAsync()
        {
            return User.Identity.IsAuthenticated &&
                await _workContext.GetCurrentUserFullYearsOldAsync() >= 18;
        }

        /// <summary>
        /// Преобразует имя категории в параметр выборки:
        /// обобщённая категория "Все категории" заменяется на null (без фильтра по категории)
        /// </summary>
        private static string ResolveCategory(string category)
        {
            return category != TvProgCategoryDefaults.AllCategoriesName ? category : null;
        }

        /// <summary>
        /// Выбирает вариант выборки телепрограммы в зависимости от возраста пользователя
        /// (совершеннолетним доступны передачи для взрослых)
        /// </summary>
        /// <param name="progType">Тип телепрограммы</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="mode">Режимы выборки: 1 - сейчас; 2 - следом</param>
        /// <param name="category">Категория</param>
        /// <param name="sidx">Поле, по которому включена сортировка</param>
        /// <param name="sord">Порядок, по которому включена сортировка</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="rows">Количество строк</param>
        /// <param name="genres">Жанры</param>
        /// <param name="channels">Каналы</param>
        private async Task<KeyValuePair<int, List<SystemProgramme>>> GetProgrammesForCurrentUserAsync(int progType,
            DateTimeOffset dateTimeOffset, int mode, string category, string sidx, string sord, int page, int rows, string genres, string channels)
        {
            var isAdult = await IsAdultUserAsync();
            return isAdult
                ? await _programmeService.GetUserAdultProgrammesAsync(progType, dateTimeOffset, mode, category,
                    sidx, sord, page, rows, genres, channels, User.Identity.IsAuthenticated)
                : await _programmeService.GetSystemProgrammesAsync(progType, dateTimeOffset, mode, category,
                    sidx, sord, page, rows, genres, channels, User.Identity.IsAuthenticated);
        }

        /// <summary>
        /// Выбирает вариант поиска телепередач в зависимости от возраста пользователя
        /// </summary>
        /// <param name="progType">Тип телепрограммы</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        /// <param name="category">Категория</param>
        /// <param name="sidx">Поле сортировки</param>
        /// <param name="sord">Порядок сортировки</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="rows">Количество строк</param>
        /// <param name="genres">Жанры</param>
        /// <param name="dates">Даты</param>
        /// <param name="channels">Каналы</param>
        private async Task<KeyValuePair<int, List<SystemProgramme>>> SearchProgrammesForCurrentUserAsync(int progType, string findTitle, string category,
            string sidx, string sord, int page, int rows, string genres, string dates, string channels)
        {
            var isAdult = await IsAdultUserAsync();
            return isAdult
                ? await _programmeService.SearchAdultProgrammeAsync(progType, findTitle, category,
                    sidx, sord, page, rows, genres, dates, channels, User.Identity.IsAuthenticated)
                : await _programmeService.SearchProgrammeAsync(progType, findTitle, category,
                    sidx, sord, page, rows, genres, dates, channels, User.Identity.IsAuthenticated);
        }

        /// <summary>
        /// Получение телепрограммы на данный момент
        /// </summary>
        /// <param name="progType">Тип телепрограммы</param>
        /// <param name="category">Категория</param>
        /// <param name="sidx">Поле, по которому включена сортировка</param>
        /// <param name="sord">Порядок, по которому включена сортировка</param>
        /// <param name="page">Номер страницы</param>
        /// <param name="rows">Количество строк</param>
        /// <param name="genres">Жанры</param>
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetSystemProgrammeAtNow(int progType, string category, string sidx, string sord, int page, int rows, string genres, string channels)
        {
            var result = await GetProgrammesForCurrentUserAsync(progType, DateTimeOffset.Now, 1, ResolveCategory(category),
                sidx, sord, page, rows, genres, channels);

            return Json(GetJsonPagingInfo(page, rows, result));
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetSystemProgrammeAtNext(int progType, string category, string sidx, string sord, int page, int rows, string genres, string channels)
        {
            var nextStartTime = new DateTimeOffset(new DateTime(1800, 1, 1));
            var result = await GetProgrammesForCurrentUserAsync(progType, nextStartTime, 2, ResolveCategory(category),
                sidx, sord, page, rows, genres, channels);

            return Json(GetJsonPagingInfo(page, rows, result));
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task SendStatForChannelRating(string uuid, int progType, string channels)
        {
            var user = await _userService.InsertTvGuestUserAsync(uuid, _webHelper.GetCurrentIpAddress());
            if (user != null)
            {
                await _userService.RemoveUserChannelMappingAsync(user);
                if (!channels.IsNullOrWhiteSpace())
                {
                    var listChannelId = channels.Split([';'], StringSplitOptions.RemoveEmptyEntries)
                        .Where(ch => int.TryParse(ch, out _))
                        .Select(int.Parse)
                        .ToList();
                    await _userService.AddUserChannelMappingAsync(user, listChannelId);
                }
            }
        }


        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetSystemChannels(int tvProgProvider, string filters, string sidx, string sord, int page, int rows)
        {
            string filtData = null;
            if (filters != null)
            {
                var item = JsonConvert.DeserializeObject<Filt>(filters);
                if (item?.rules.Count > 0)
                {
                    filtData = item?.rules[0]?.data;
                }
            }
            var result = await _channelService.GetSystemChannelsAsync(tvProgProvider, filtData, sidx, sord, page, rows);
            return Json(GetJsonPagingInfo(page, rows, result));
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> SearchProgramme(int progType, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates, string channels)
        {
            var result = await SearchProgrammesForCurrentUserAsync(progType, findTitle, ResolveCategory(category),
                sidx, sord, page, rows, genres, dates, channels);

            return Json(GetJsonPagingInfo(page, rows, result));
        }

        /// <summary>
        /// Постраничное отображение
        /// </summary>
        /// <typeparam name="T">Тип список программы</typeparam>
        /// <param name="page">Страница</param>
        /// <param name="rows">Количество строк</param>
        /// <param name="result">Тип для постраничного отображения</param>
        /// <returns></returns>
        public static object GetJsonPagingInfo<T>(int page, int rows, KeyValuePair<int, T> result)
        {
            int pageSize = rows;
            int totalRecords = result.Key;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = result.Value
            };
            return jsonData;
        }

        /// <summary>
        /// Получение периода действия программы телепередач
        /// </summary>
        /// <param name="progType">Тип телепрограммы</param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetSystemProgrammePeriod(int? progType)
        {
            if (progType.HasValue)
                return Json(await _programmeService.GetSystemProgrammePeriodAsync(progType.Value));
            return new JsonResult(null);
        }

        /// <summary>
        /// Получение дерева в формате JSON
        /// </summary>
        /// <param name="providerId">Идентификатор поставщика программы телепередач</param>
        /// <param name="typeProg">Тип программы телепередач</param>
        /// <param name="mode">Режим отображения: 1 - группировка под датам; 2 - группировка по телеканалам</param>
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetTreeData(int providerId, int typeProg, string jsonChannels, int mode)
        {
            TreeNode root = new()
            {
                id = Guid.NewGuid().ToString(),
                text = "",
                state = new State(true, false, false),
                icon = "/images/i/satellite_dish2.png",
                children = []
            };

            // Границы трёх частей периода вычисляются общим с сервисом телепрограммы способом
            var bounds = (await _programmeService.GetSystemProgrammePeriodAsync(typeProg)).GetPeriodBounds();

            // Каналы сортируются один раз перед построением всех узлов дерева
            List<UserChannel> chanList = (await _channelService.GetUserChannelsByLocalStorageAsync(providerId, jsonChannels))
                .OrderBy(o => o.SysOrderCol)
                .ThenBy(o => o.InternalId)
                .ToList();

            TreeNode oneHalfRoot = CreateHalfRootNode(bounds.TsMin, bounds.Middle1);
            TreeNode twoHalfRoot = CreateHalfRootNode(bounds.Middle1.AddDays(1), bounds.Middle2);
            TreeNode threeHalfRoot = CreateHalfRootNode(bounds.Middle2.AddDays(1), bounds.TsMax);

            switch (mode)
            {
                case 1: // Группировка по датам:
                    oneHalfRoot = CreateTreeByDays(chanList, bounds.TsMin, bounds.Middle1, oneHalfRoot);
                    twoHalfRoot = CreateTreeByDays(chanList, bounds.Middle1.AddDays(1), bounds.Middle2, twoHalfRoot);
                    threeHalfRoot = CreateTreeByDays(chanList, bounds.Middle2.AddDays(1), bounds.TsMax, threeHalfRoot);
                    break;
                case 2: // Группировка по телеканалам:
                    oneHalfRoot = CreateTreeByChannels(chanList, bounds.TsMin, bounds.Middle1, oneHalfRoot, 1);
                    twoHalfRoot = CreateTreeByChannels(chanList, bounds.Middle1.AddDays(1), bounds.Middle2, twoHalfRoot, 2);
                    threeHalfRoot = CreateTreeByChannels(chanList, bounds.Middle2.AddDays(1), bounds.TsMax, threeHalfRoot, 3);
                    break;
            }

            root.children.Add(oneHalfRoot);
            root.children.Add(twoHalfRoot);
            root.children.Add(threeHalfRoot);
            return Json(root);
        }

        /// <summary>
        /// Создание родительского узла для половины периода телепрограммы
        /// </summary>
        /// <param name="left">Левая дата периода</param>
        /// <param name="right">Правая дата периода</param>
        /// <returns>Созданный узел</returns>
        private static TreeNode CreateHalfRootNode(DateTime left, DateTime right)
        {
            return new TreeNode
            {
                id = $"({left.ToShortDateString()}—{right.ToShortDateString()})",
                text = $"({left.ToShortDateString()}—{right.ToShortDateString()})",
                state = new State(false, false, false),
                icon = "/images/i/All.png",
                children = []
            };
        }

        /// <summary>
        /// Создание узла телеканала
        /// </summary>
        /// <param name="channel">Телеканал</param>
        /// <param name="id">Идентификатор узла</param>
        /// <param name="withChildren">Создавать ли коллекцию дочерних узлов</param>
        /// <returns>Созданный узел</returns>
        private static TreeNode CreateChannelNode(UserChannel channel, string id, bool withChildren)
        {
            return new TreeNode
            {
                id = id,
                text = channel.Title,
                state = new State(false, false, false),
                icon = !string.IsNullOrWhiteSpace(channel.FileName25) ? channel.FileName25 : "images/i/satellite_25.png",
                children = withChildren ? [] : null
            };
        }

        /// <summary>
        /// Создание узла дня недели
        /// </summary>
        /// <param name="date">Дата</param>
        /// <param name="id">Идентификатор узла</param>
        /// <param name="withChildren">Создавать ли коллекцию дочерних узлов</param>
        /// <returns>Созданный узел</returns>
        private static TreeNode CreateDayNode(DateTime date, string id, bool withChildren)
        {
            return new TreeNode
            {
                id = id,
                text = $"({date.ToShortDateString()})",
                state = new State(false, false, false),
                icon = $"/images/i/{TvProgViewer.Services.TvProgMain.Extensions.GetDayIconName(date.DayOfWeek)}.png",
                children = withChildren ? [] : null
            };
        }

        /// <summary>
        /// Создание дерева по дням недели
        /// </summary>
        /// <param name="chanList">Отсортированный список телеканалов</param>
        /// <param name="left">Левая дата</param>
        /// <param name="right">Правая дата</param>
        /// <param name="node">Родительский узел</param>
        /// <returns>Заполненный родительский узел</returns>
        private static TreeNode CreateTreeByDays(List<UserChannel> chanList, DateTime left, DateTime right, TreeNode node)
        {
            for (DateTime tsCurDate = left; tsCurDate <= right; tsCurDate = tsCurDate.AddDays(1))
            {
                TreeNode dayNode = CreateDayNode(tsCurDate, tsCurDate.ToShortDateString(), withChildren: true);
                node.children.Add(dayNode);
                foreach (UserChannel channel in chanList)
                {
                    dayNode.children.Add(CreateChannelNode(channel, $"{tsCurDate.ToShortDateString()}_{channel.ChannelId}", withChildren: false));
                }
            }
            return node;
        }

        /// <summary>
        /// Cоздание дерева по каналам
        /// </summary>
        /// <param name="chanList">Отсортированный список телеканалов</param>
        /// <param name="left">Левая дата</param>
        /// <param name="right">Правая дата</param>
        /// <param name="node">Родительский узел</param>
        /// <param name="nodeIndex">Порядковый номер родительского узла</param>
        /// <returns>Заполненный родительский узел</returns>
        private static TreeNode CreateTreeByChannels(List<UserChannel> chanList, DateTime left, DateTime right, TreeNode node, int nodeIndex)
        {
            foreach (UserChannel channel in chanList)
            {
                TreeNode channelNode = CreateChannelNode(channel, $"{nodeIndex}_{channel.ChannelId}", withChildren: true);
                node.children.Add(channelNode);
                for (DateTime tsCurDate = left; tsCurDate <= right; tsCurDate = tsCurDate.AddDays(1))
                {
                    channelNode.children.Add(CreateDayNode(tsCurDate, $"{tsCurDate.ToShortDateString()}_{channel.ChannelId}", withChildren: false));
                }
            }
            return node;
        }

        /// <summary>
        /// Выбор метода получения программы передач за день в зависимости от статуса пользователя
        /// </summary>
        /// <param name="progTypeID">Тип телепрограммы</param>
        /// <param name="cid">Телеканал</param>
        /// <param name="dtStart">Время начала выборки</param>
        /// <param name="dtEnd">Время завершения выборки</param>
        /// <param name="category">Категория</param>
        private async Task<List<SystemProgramme>> GetUserProgrammesOfDayForCurrentUserAsync(int progTypeID, int cid, DateTime dtStart, DateTime dtEnd, string category)
        {
            var isAdult = await IsAdultUserAsync();
            return isAdult
                ? await _programmeService.GetUserAdultProgrammesOfDayListAsync(progTypeID, cid, dtStart, dtEnd, category, User.Identity.IsAuthenticated)
                : await _programmeService.GetUserProgrammesOfDayListAsync(progTypeID, cid, dtStart, dtEnd, category, User.Identity.IsAuthenticated);
        }

        /// <summary>
        /// Получение программы передач за день
        /// </summary>
        /// <param name="progTypeID">Тип телепрограммы</param>
        /// <param name="cid">Телеканал</param>
        /// <param name="tsDate">На дату</param>
        /// <param name="category">Категория</param>
        public async Task<JsonResult> GetUserProgrammeOfDay(int progTypeID, int cid, string tsDate, string category)
        {
            IFormatProvider provider = CultureInfo.GetCultureInfo("ru-RU");
            DateTime dtStart = Convert.ToDateTime(tsDate, provider).AddHours(5).AddMinutes(45);
            DateTime dtEnd = Convert.ToDateTime(tsDate, provider).AddDays(1).AddHours(5).AddMinutes(45);

            return Json(await GetUserProgrammesOfDayForCurrentUserAsync(progTypeID, cid, dtStart, dtEnd, ResolveCategory(category)));
        }

        /// <summary>
        /// Получение списка каналов
        /// </summary>
        /// <param name="providerId">Идентификатор провайдера</param>
        /// <param name="jsonChannels">JSON каналов</param>
        public async Task<JsonResult> GetChannels (int providerId, string jsonChannels)
        {
            List<UserChannel> chanList = await _channelService.GetUserChannelsByLocalStorageAsync(providerId, jsonChannels);
            return Json(chanList.ToArray());
        }

        /// <summary>
        /// Получение uuid авторизованного пользователя
        /// </summary>
        public async Task<JsonResult> GetUnidentificator()
        {
            string uuid = null;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _workContext.GetCurrentUserAsync();
                uuid = user.UserGuid.ToString().ToLowerInvariant();
            }
            uuid = !string.IsNullOrWhiteSpace(uuid) ? $"\"{uuid}\"" : "null";
            return Json($"{{\"uuid\":{uuid}}}");
        }

        /// <summary>
        /// Выполняет операцию выгрузки данных для внешних потребителей:
        /// проверяет ключ доступа и статус операции, подготавливает данные
        /// и регистрирует обращение пользователя
        /// </summary>
        /// <param name="key">Ключ доступа к выгрузке данных</param>
        /// <param name="operationName">Имя операции</param>
        /// <param name="uuid">Идентификатор пользователя выгрузки</param>
        /// <param name="prepareDataFunc">Функция подготовки выгружаемых данных</param>
        private async Task<JsonResult> ExecuteGreenDataOperationAsync(string key, string operationName, string uuid, Func<Task<object>> prepareDataFunc)
        {
            if (!string.Equals(key, _securitySettings.ProgrammesLoadKey, StringComparison.OrdinalIgnoreCase))
                return Json("{}");

            var user = await _userService.InsertTvGreenDataUserAsync(uuid, _webHelper.GetCurrentIpAddress());
            bool isActiveOperation = await _userService.GetOperationActiveStatus(operationName);
            if (!((user == null || (user.Active && !user.Deleted)) && isActiveOperation))
                return Json("{}");

            var data = await prepareDataFunc();
            if (user != null)
            {
                await _userService.AddUserGreenDataOperationAsync(user, operationName);
            }

            return Json(data);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public Task<JsonResult> GetAllChannels(string key, string uuid)
        {
            return ExecuteGreenDataOperationAsync(key, TvProgUserDefaults.GetAllChannelsOperationName, uuid, async () =>
                new { tvChannels = await _channelService.GetAllChannels() });
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public Task<JsonResult> GetAllProgrammes(string key, string uuid, int page, int rows)
        {
            return ExecuteGreenDataOperationAsync(key, TvProgUserDefaults.GetAllProgrammesOperationName, uuid, async () =>
                new { tvschedule = await _programmeService.GetAllProgrammes(page, rows) });
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public Task<JsonResult> GetProgrammesStatus(string key, string uuid)
        {
            return ExecuteGreenDataOperationAsync(key, TvProgUserDefaults.GetProgrammeStatusOperationName, uuid, async () =>
                new { tvshedulestatus = await _programmeService.GetProgrammeStatus() });
        }
        #endregion
    }
}
