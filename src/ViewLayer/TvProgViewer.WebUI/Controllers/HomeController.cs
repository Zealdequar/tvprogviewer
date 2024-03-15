using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TvProgViewer.Services.TvProgMain;
using TvProgViewer.Data.TvProgMain.ProgObjs;
using System.Threading.Tasks;
using TvProgViewer.WebUI.Models.Tree;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Linq;
using TvProgViewer.Services.Users;
using NUglify.Helpers;
using TvProgViewer.Core;
using System.Globalization;

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

        /// <summary>
        /// Словарь с днями недели для обтображения пиктограмм
        /// </summary>
        private readonly Dictionary<string, string> dictWeek = new Dictionary<string, string>()
                                                       {
                                                           {"monday", "Mon"},
                                                           {"tuesday", "Tue"},
                                                           {"wednesday", "Wen"},
                                                           {"thursday", "Ths" },
                                                           {"friday", "Fri" },
                                                           {"saturday", "Sat"},
                                                           {"sunday", "Sun"}
                                                       };

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
            IWorkContext workContext)
        {
            _programmeService = programmeService;
            _channelService = channelService;
            _genreService = genreService;
            _userService = userService;
            _webHelper = webHelper;
            _workContext = workContext;
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
        /// <returns></returns>
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetCategories()
        {
            return Json(await _programmeService.GetCategoriesAsync());
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
        /// <returns></returns>
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetSystemProgrammeAtNow(int progType, string category, string sidx, string sord, int page, int rows, string genres, string channels)
        {
            object jsonData;
            KeyValuePair<int, List<SystemProgramme>> result = new();
            if (User.Identity.IsAuthenticated)
            {
                if (await _workContext.GetCurrentUserFullYearsOldAsync() >= 18)
                {
                    result = await _programmeService.GetUserAdultProgrammesAsync(progType, DateTimeOffset.Now, 1, (category != "Все категории") ? category : null
                        , sidx, sord, page, rows, genres, channels);
                } else
                {
                    result = await _programmeService.GetSystemProgrammesAsync(progType, DateTimeOffset.Now, 1, (category != "Все категории") ? category : null
                        , sidx, sord, page, rows, genres, channels);
                }
            } else
            {
                result = await _programmeService.GetSystemProgrammesAsync(progType, DateTimeOffset.Now, 1, (category != "Все категории") ? category : null
                        , sidx, sord, page, rows, genres, channels);
            }

            jsonData = GetJsonPagingInfo(page, rows, result);
            return Json(jsonData);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetSystemProgrammeAtNext(int progType, string category, string sidx, string sord, int page, int rows, string genres, string channels)
        {
            object jsonData;
            KeyValuePair<int, List<SystemProgramme>> result = new();
            if (User.Identity.IsAuthenticated)
            {
                if (await _workContext.GetCurrentUserFullYearsOldAsync() >= 18)
                {
                    result = await _programmeService.GetUserAdultProgrammesAsync(progType, new DateTimeOffset(new DateTime(1800,1,1)), 2
                        , (category != "Все категории") ? category : null, sidx, sord, page, rows, genres, channels);
                }
                else
                {
                    result = await _programmeService.GetSystemProgrammesAsync(progType, new DateTimeOffset(new DateTime(1800, 1, 1)), 2
                        , (category != "Все категории") ? category : null, sidx, sord, page, rows, genres, channels);
                }
            } else
            {
                result = await _programmeService.GetSystemProgrammesAsync(progType, new DateTimeOffset(new DateTime(1800, 1, 1)), 2
                        , (category != "Все категории") ? category : null, sidx, sord, page, rows, genres, channels);
            }
            jsonData = GetJsonPagingInfo(page, rows, result);
            return Json(jsonData);
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
                    var listChannelId = await channels.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(ch => int.TryParse(ch, out int id))
                        .Select(ch => { _ = int.TryParse(ch, out int id); return id; }).ToListAsync();
                    await _userService.AddUserChannelMappingAsync(user, listChannelId);
                }
            }
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetSystemChannels(int tvProgProvider, string filters, string sidx, string sord, int page, int rows)
        {
            object jsonData;
            KeyValuePair<int, List<SystemChannel>> result;
            string filtData = null;
            if (filters != null)
            {
                var item = JsonConvert.DeserializeObject<Filt>(filters);
                if (item?.rules.Count > 0)
                {
                    filtData = item?.rules[0]?.data;
                }
            }
            result = await _channelService.GetSystemChannelsAsync(tvProgProvider, filtData, sidx, sord, page, rows);
            jsonData = GetJsonPagingInfo(page, rows, result);
            return Json(jsonData);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> SearchProgramme(int progType, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates, string channels)
        {
            object jsonData;
            KeyValuePair<int, List<SystemProgramme>> result = new();
            if (User.Identity.IsAuthenticated)
            {
                if (await _workContext.GetCurrentUserFullYearsOldAsync() >= 18)
                {
                    result = await _programmeService.SearchAdultProgrammeAsync(progType, findTitle, (category != "Все категории") ? category : null, sidx, sord, page, rows, genres, dates, channels);
                }
                else
                {
                    result = await _programmeService.SearchProgrammeAsync(progType, findTitle, (category != "Все категории") ? category : null, sidx, sord, page, rows, genres, dates, channels);
                }
            }
            else 
            {
                result = await _programmeService.SearchProgrammeAsync(progType, findTitle, (category != "Все категории") ? category : null, sidx, sord, page, rows, genres, dates, channels);
            }
            
            jsonData = GetJsonPagingInfo(page, rows, result);
            return Json(jsonData);
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

            ProgPeriod periodMinMax = await _programmeService.GetSystemProgrammePeriodAsync(typeProg);
            DateTime tsMin = periodMinMax.dtStart.HasValue ? periodMinMax.dtStart.Value.DateTime : DateTime.Now;
            tsMin = new DateTime(tsMin.Year, tsMin.Month, tsMin.Day, 5, 45, 0);
            DateTime tsMiddle1 = tsMin.AddDays(6);
            DateTime tsMiddle2 = tsMiddle1.AddDays(7); 
            DateTime tsMax = periodMinMax.dtEnd.HasValue ? periodMinMax.dtEnd.Value.DateTime : DateTime.Now;
            tsMax = tsMax.AddDays(-1);
            tsMax = new DateTime(tsMax.Year, tsMax.Month, tsMax.Day, 5, 45, 0);

            List<UserChannel> chanList = await _channelService.GetUserChannelsByLocalStorageAsync(providerId, jsonChannels);
            TreeNode oneHalfRoot = new()
            {
                id = $"({tsMin.ToShortDateString()}—{tsMiddle1.ToShortDateString()})",
                text = $"({tsMin.ToShortDateString()}—{tsMiddle1.ToShortDateString()})",
                state = new State(false, false, false),
                icon = "/images/i/All.png",
                children = []
            };
            TreeNode twoHalfRoot = new()
            {
                id = $"({tsMiddle1.AddDays(1).ToShortDateString()}—{tsMiddle2.ToShortDateString()})",
                text = $"({tsMiddle1.AddDays(1).ToShortDateString()}—{tsMiddle2.ToShortDateString()})",
                state = new State(false, false, false),
                icon = "/images/i/All.png",
                children = []
            };
            TreeNode threeHalfRoot = new()
            {
                id = $"({tsMiddle2.AddDays(1).ToShortDateString()}—{tsMax.ToShortDateString()})",
                text = $"({tsMiddle2.AddDays(1).ToShortDateString()}—{tsMax.ToShortDateString()})",
                state = new State(false, false, false),
                icon = "/images/i/All.png",
                children = []
            };

            if (mode == 1) // Группировка по датам:
            {
                oneHalfRoot = CreateTreeByDays(chanList, tsMin, tsMiddle1, oneHalfRoot);
                twoHalfRoot = CreateTreeByDays(chanList, tsMiddle1.AddDays(1), tsMiddle2, twoHalfRoot);
                threeHalfRoot = CreateTreeByDays(chanList, tsMiddle2.AddDays(1), tsMax, threeHalfRoot);
            }
            else if (mode == 2) // Группировка по телеканалам:
            {
                oneHalfRoot = CreateTreeByChannels(chanList, tsMin, tsMiddle1, oneHalfRoot, 1);
                twoHalfRoot = CreateTreeByChannels(chanList, tsMiddle1.AddDays(1), tsMiddle2, twoHalfRoot, 2);
                threeHalfRoot = CreateTreeByChannels(chanList, tsMiddle2.AddDays(1), tsMax, threeHalfRoot, 3);
            }
            
            root.children.Add(oneHalfRoot);
            root.children.Add(twoHalfRoot);
            root.children.Add(threeHalfRoot);
            return Json(root);
        }

        /// <summary>
        /// Создание дерева по дням недели
        /// </summary>
        /// <param name="chanList">Список телеканалов</param>
        /// <param name="left">Левая дата</param>
        /// <param name="right">Правая дата</param>
        /// <param name="node">Родительский узел</param>
        /// <returns>Заполненный родительский узел</returns>
        private TreeNode CreateTreeByDays(List<UserChannel> chanList, DateTime left, DateTime right, TreeNode node)
        {
            for (DateTime tsCurDate = left; tsCurDate <= right; tsCurDate = tsCurDate.AddDays(1))
            {
                string strKey = dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                TreeNode dayNode = new()
                {
                    id = tsCurDate.ToShortDateString(),
                    text = $"({tsCurDate.ToShortDateString()})",
                    state = new State(false, false, false),
                    icon = $"/images/i/{strKey}.png",
                    children = []
                };
                node.children.Add(dayNode);
                foreach (UserChannel channel in chanList.OrderBy(o => o.SysOrderCol).ThenBy(o => o.InternalId))
                {
                    TreeNode channelNode = new()
                    {
                        id = $"{tsCurDate.ToShortDateString()}_{channel.ChannelId}",
                        text = channel.Title,
                        state = new State(false, false, false),
                        icon = $"{(!string.IsNullOrWhiteSpace(channel.FileName25) ? channel.FileName25 : "images/i/satellite_25.png")}"
                    };
                    dayNode.children.Add(channelNode);
                }
            }
            return node;
        }

        /// <summary>
        /// Cоздание дерева по каналам
        /// </summary>
        /// <param name="chanList">Список телеканалов</param>
        /// <param name="left">Левая дата</param>
        /// <param name="right">Правая дата</param>
        /// <param name="node">Родительский узел</param>
        /// <param name="nodeIndex">Порядковый номер родительского узла</param>
        /// <returns>Заполненный родительский узел</returns>
        private TreeNode CreateTreeByChannels(List<UserChannel> chanList, DateTime left, DateTime right, TreeNode node, int nodeIndex)
        {
            foreach (UserChannel channel in chanList.OrderBy(o => o.SysOrderCol).ThenBy(o => o.InternalId))
            {
                TreeNode channelNode = new()
                {
                    id = $"{nodeIndex}_{channel.ChannelId}",
                    text = channel.Title,
                    state = new State(false, false, false),
                    icon = $"{(!string.IsNullOrWhiteSpace(channel.FileName25) ? channel.FileName25 : "images/i/satellite_25.png")}",
                    children = []
                };
                node.children.Add(channelNode);
                for (DateTime tsCurDate = left; tsCurDate <= right; tsCurDate = tsCurDate.AddDays(1))
                {
                    string strKey = dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                    TreeNode dayNode = new()
                    {
                        id = $"{tsCurDate.ToShortDateString()}_{channel.ChannelId}",
                        text = $"({tsCurDate.ToShortDateString()})",
                        state = new State(false, false, false),
                        icon = $"/images/i/{strKey}.png"
                    };
                    channelNode.children.Add(dayNode);
                }
            }
            return node;
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
            if (User.Identity.IsAuthenticated)
            {
                if (await _workContext.GetCurrentUserFullYearsOldAsync() >= 18)
                {
                    return Json(await _programmeService.GetUserAdultProgrammesOfDayListAsync(progTypeID, cid, dtStart, dtEnd,
                                   (category != "null") ? category : null));                }
                else
                {
                    return Json(await _programmeService.GetUserProgrammesOfDayListAsync(progTypeID, cid, dtStart, dtEnd,
                                   (category != "null") ? category : null));
                }
            }
            else
            {
                return Json(await _programmeService.GetUserProgrammesOfDayListAsync(progTypeID, cid, dtStart, dtEnd,
                (category != "null") ? category : null));
            }
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
        #endregion
    }
}