using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using TvProgViewer.Core.Caching;
using TvProgViewer.Services.TvProgMain;
using TvProgViewer.Data.TvProgMain.ProgObjs;
using TvProgViewer.Web.Framework.Mvc.Filters;
using TvProgViewer.Web.Framework.Security;
using System.Threading.Tasks;
using TvProgViewer.WebUI.Models.Tree;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Linq;
using TvProgViewer.Services.Users;
using FluentMigrator.Model;
using NUglify.Helpers;
using TvProgViewer.Core;

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
            IWebHelper webHelper)
        {
            _programmeService = programmeService;
            _channelService = channelService;
            _genreService = genreService;
            _userService = userService;
            _webHelper = webHelper;
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
            KeyValuePair<int, List<SystemProgramme>> result;
            if (User.Identity.IsAuthenticated)
            {
                /* result = _programmeService.GetUserProgrammesAtNowAsyncList(UserId.Value, progType, DateTimeOffset.Now, (category != "null") ? category : null
                     , sidx, sord, page, rows, genres);
                 jsonData = ControllerExtensions.GetJsonPagingInfo(page, rows, result);
                 return Json(jsonData, JsonRequestBehavior.AllowGet);*/
            }
            result = await _programmeService.GetSystemProgrammesAsync(progType, DateTimeOffset.Now, 1, (category != "Все категории") ? category : null,
                sidx, sord, page, rows, genres, channels);

            jsonData = GetJsonPagingInfo(page, rows, result);
            return Json(jsonData);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetSystemProgrammeAtNext(int progType, string category, string sidx, string sord, int page, int rows, string genres, string channels)
        {
            object jsonData;
            KeyValuePair<int, List<SystemProgramme>> result;
            if (User.Identity.IsAuthenticated)
            {
                /*result = await progRepository.GetUserProgrammesAtNextAsyncList(UserId.Value, progType, new DateTimeOffset(new DateTime(1800, 1, 1)), (category != "null") ? category : null,
                    sidx, sord, page, rows, genres);
                jsonData = ControllerExtensions.GetJsonPagingInfo(page, rows, result);
                return Json(jsonData, JsonRequestBehavior.AllowGet);*/
            }

            result = await _programmeService.GetSystemProgrammesAsync(progType, new DateTimeOffset(new DateTime(1800, 1, 1)), 2, (category != "Все категории") ? category : null,
                sidx, sord, page, rows, genres, channels);
            jsonData = GetJsonPagingInfo(page, rows, result);
            return Json(jsonData);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task SendStatForChannelRating(string uuid, int progType, string channels)
        {
            var user = await _userService.InsertTvGuestUserAsync(uuid, _webHelper.GetCurrentIpAddress());
            await _userService.RemoveUserChannelMappingAsync(user);
            if (!channels.IsNullOrWhiteSpace())
            {
                var listChannelId = await channels.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(ch => int.TryParse(ch, out int id))
                    .Select(ch => { _ = int.TryParse(ch, out int id); return id; }).ToListAsync();
                await _userService.AddUserChannelMappingAsync(user, listChannelId);
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
            KeyValuePair<int, List<SystemProgramme>> result;
            if (User.Identity.IsAuthenticated)
            {
                /*result = await progRepository.SearchUserProgramme(UserId.Value, progType, findTitle
                    , (category != "null") ? category : null, sidx, sord, page, rows, genres, dates);

                jsonData = ControllerExtensions.GetJsonPagingInfo(page, rows, result);
                return Json(jsonData, JsonRequestBehavior.AllowGet);*/
            }
            result = await _programmeService.SearchProgrammeAsync(progType, findTitle, (category != "Все категории") ? category : null, sidx, sord, page, rows, genres, dates, channels);
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
            TreeNode root = new TreeNode()
            {
                id = Guid.NewGuid().ToString(),
                text = "",
                state = new State(true, false, false),
                icon = "/images/i/satellite_dish2.png",
                children = new List<TreeNode>()
            };

            ProgPeriod periodMinMax = await _programmeService.GetSystemProgrammePeriodAsync(typeProg);
            DateTime tsMin = periodMinMax.dtStart.HasValue ? periodMinMax.dtStart.Value.DateTime : new DateTime();
            tsMin = new DateTime(tsMin.Year, tsMin.Month, tsMin.Day, 5, 45, 0);
            DateTime tsMiddle = tsMin.AddDays(6);
            DateTime tsMax = periodMinMax.dtEnd.HasValue ? periodMinMax.dtEnd.Value.DateTime : new DateTime();
            tsMax = new DateTime(tsMax.AddDays(-1).Year, tsMax.AddDays(-1).Month, tsMax.AddDays(-1).Day, 5, 45, 0);

            List<UserChannel> chanList = await _channelService.GetUserChannelsByLocalStorageAsync(providerId, jsonChannels);
            TreeNode oneHalfRoot = new TreeNode()
            {
                id = $"({tsMin.ToShortDateString()}—{tsMiddle.ToShortDateString()})",
                text = $"({tsMin.ToShortDateString()}—{tsMiddle.ToShortDateString()})",
                state = new State(false, false, false),
                icon = "/images/i/All.png",
                children = new List<TreeNode>()
            };
            TreeNode twoHalfRoot = new TreeNode()
            {
                id = $"({tsMiddle.AddDays(1).ToShortDateString()}—{tsMax.ToShortDateString()})",
                text = $"({tsMiddle.AddDays(1).ToShortDateString()}—{tsMax.ToShortDateString()})",
                state = new State(false, false, false),
                icon = "/images/i/All.png",
                children = new List<TreeNode>()
            };
            if (mode == 1) // Группировка по датам
            {
                for (DateTime tsCurDate = tsMin; tsCurDate <= tsMiddle; tsCurDate = tsCurDate.AddDays(1))
                {
                    string strKey = dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                    TreeNode dayNode = new TreeNode()
                    {
                        id = tsCurDate.ToShortDateString(),
                        text = $"({tsCurDate.ToShortDateString()})",
                        state = new State(false, false, false),
                        icon = $"/images/i/{strKey}.png",
                        children = new List<TreeNode>()
                    };
                    oneHalfRoot.children.Add(dayNode);
                    foreach (UserChannel channel in chanList.OrderBy(o => o.SysOrderCol).ThenBy(o => o.InternalId))
                    {
                        TreeNode channelNode = new TreeNode()
                        {
                            id = $"{tsCurDate.ToShortDateString()}_{channel.ChannelId}",
                            text = channel.Title,
                            state = new State(false, false, false),
                            icon = $"{(!string.IsNullOrWhiteSpace(channel.FileName25) ? channel.FileName25 : "images/i/satellite_25.png")}"
                        };
                        dayNode.children.Add(channelNode);
                    }
                }

                for (DateTime tsCurDate = tsMiddle.AddDays(1); tsCurDate <= tsMax; tsCurDate = tsCurDate.AddDays(1))
                {
                    string strKey = dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                    TreeNode dayNode = new TreeNode()
                    {
                        id = tsCurDate.ToShortDateString(),
                        text = $"({tsCurDate.ToShortDateString()})",
                        state = new State(false, false, false),
                        icon = $"/images/i/{strKey}.png",
                        children = new List<TreeNode>()
                    };
                    twoHalfRoot.children.Add(dayNode);
                    foreach (UserChannel channel in chanList.OrderBy(o => o.SysOrderCol).ThenBy(o => o.InternalId))
                    {
                        TreeNode channelNode = new TreeNode()
                        {
                            id = $"{tsCurDate.ToShortDateString()}_{channel.ChannelId}",
                            text = channel.Title,
                            state = new State(false, false, false),
                            icon = $"{(!string.IsNullOrWhiteSpace(channel.FileName25) ? channel.FileName25 : "images/i/satellite_25.png")}"
                        };
                        dayNode.children.Add(channelNode);
                    }
                }
            }
            else
            {
                if (mode == 2)
                {
                    foreach (UserChannel channel in chanList.OrderBy(o => o.SysOrderCol).ThenBy(o => o.InternalId))
                    {
                        TreeNode channelNode = new TreeNode()
                        {
                            id = $"1_{channel.ChannelId}",
                            text = channel.Title,
                            state = new State(false, false, false),
                            icon = $"{(!string.IsNullOrWhiteSpace(channel.FileName25) ? channel.FileName25 : "images/i/satellite_25.png")}",
                            children = new List<TreeNode>()
                        };
                        oneHalfRoot.children.Add(channelNode);
                        for (DateTime tsCurDate = tsMin; tsCurDate <= tsMiddle; tsCurDate = tsCurDate.AddDays(1))
                        {
                            string strKey = dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                            TreeNode dayNode = new TreeNode()
                            {
                                id = $"{tsCurDate.ToShortDateString()}_{channel.ChannelId}",
                                text = $"({tsCurDate.ToShortDateString()})",
                                state = new State(false, false, false),
                                icon = $"/images/i/{strKey}.png"
                            };
                            channelNode.children.Add(dayNode);
                        }
                    }
                    foreach (UserChannel channel in chanList.OrderBy(o => o.SysOrderCol).ThenBy(o => o.InternalId))
                    {
                        TreeNode channelNode = new TreeNode()
                        {
                            id = $"2_{channel.ChannelId}",
                            text = channel.Title,
                            state = new State(false, false, false),
                            icon = $"{(!string.IsNullOrWhiteSpace(channel.FileName25) ? channel.FileName25 : "images/i/satellite_25.png")}",
                            children = new List<TreeNode>()
                        };
                        twoHalfRoot.children.Add(channelNode);
                        for (DateTime tsCurDate = tsMiddle.AddDays(1); tsCurDate <= tsMax; tsCurDate = tsCurDate.AddDays(1))
                        {
                            string strKey = dictWeek[tsCurDate.DayOfWeek.ToString().ToLower()];
                            TreeNode dayNode = new TreeNode()
                            {
                                id = $"{tsCurDate.ToShortDateString()}_{channel.ChannelId}",
                                text = $"({tsCurDate.ToShortDateString()})",
                                state = new State(false, false, false),
                                icon = $"/images/i/{strKey}.png"
                            };
                            channelNode.children.Add(dayNode);
                        }
                    }
                }
            }

            root.children.Add(oneHalfRoot);
            root.children.Add(twoHalfRoot);
            return Json(root);
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
            return Json(await _programmeService.GetUserProgrammesOfDayListAsync(null, progTypeID, cid,
                                Convert.ToDateTime(tsDate).AddHours(5).AddMinutes(45),
                                Convert.ToDateTime(tsDate).AddDays(1).AddHours(5).AddMinutes(45), (category != "null") ? category : null));
        }

        #endregion
    }
}