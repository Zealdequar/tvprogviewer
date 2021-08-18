using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using TVProgViewer.Core.Caching;
using TVProgViewer.Services.TvProgMain;
using TVProgViewer.Data.TvProgMain.ProgObjs;
using TVProgViewer.Web.Framework.Mvc.Filters;
using TVProgViewer.Web.Framework.Security;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Controllers
{
    public partial class HomeController : BasePublicController
    {
        #region Поля

        private readonly IProgrammeService _programmeService;
        private readonly IChannelService _channelService;
        private readonly IGenreService _genreService;

        #endregion

        #region Конструктор

        public HomeController(IProgrammeService programmeService,
            IChannelService channelService,
            IGenreService genreService)
        {
            _programmeService = programmeService;
            _channelService = channelService;
            _genreService = genreService;
        }

        #endregion

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
        public async Task<JsonResult> GetSystemChannels (int tvProgProvider, string sidx, string sord, int page, int rows)
        {
            object jsonData;
            KeyValuePair<int, List<SystemChannel>> result;
            result = await _channelService.GetSystemChannelsAsync(tvProgProvider, sidx, sord, page, rows);
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
            if (User.Identity.IsAuthenticated )
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
                return Json( await _programmeService.GetSystemProgrammePeriodAsync(progType.Value));
            return new JsonResult(null);
        }

    }
}