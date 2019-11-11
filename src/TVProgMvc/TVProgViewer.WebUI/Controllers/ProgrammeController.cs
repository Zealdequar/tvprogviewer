using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TVProgViewer.WebUI.Abstract;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using TVProgViewer.WebUI.Infrastructure;
using TVProgViewer.BusinessLogic.ProgObjs;
using TVProgViewer.Common;


namespace TVProgViewer.WebUI.Controllers
{
    /// <summary>
    /// Контроллер для работы с телепрограммой
    /// </summary>
    public class ProgrammeController : Controller
    {
        /// <summary>
        /// Репозиторий для телепрограммы
        /// </summary>
        private IProgrammesRepository progRepository;
        private IGenresRepository genresRepository;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private long? UserId { get { return LazyGlobalist.Instance.UserId(System.Web.HttpContext.Current); } }

        public ProgrammeController(IProgrammesRepository programmeRepository, IGenresRepository genRepository)
        {
            this.progRepository = programmeRepository;
            this.genresRepository = genRepository;
        }

        public ActionResult List()
        {
            ViewData["UID"] = UserId.HasValue ? 1 : 0;
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetSystemProgrammeAtNow(int progType, string category, string sidx, string sord, int page, int rows, string genres)
        {           
            if (User.Identity.IsAuthenticated && UserId != null)
            {
                return Json(await progRepository.GetUserProgrammesAtNowAsyncList(UserId.Value, progType, DateTimeOffset.Now, (category != "null") ? category : null
                    , sidx, sord, page, rows, genres)
                    , JsonRequestBehavior.AllowGet);
            }

            KeyValuePair<int, SystemProgramme[]> result = await progRepository.GetSystemProgrammesAtNowAsyncList(progType, DateTimeOffset.Now, (category != "null") ? category : null,
                 sidx, sord, page, rows, genres);

            var jsonData = ControllerExtensions.GetJsonPagingInfo(page, rows, result);
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetSystemProgrammeAtNext(int progType, string category, string sidx, string sord, int page, int rows, string genres)
        {
            if (User.Identity.IsAuthenticated && UserId != null)
            {
                return Json(await progRepository.GetUserProgrammesAtNextAsyncList(UserId.Value, progType, new DateTimeOffset(new DateTime(1800, 1, 1)), (category != "null") ? category : null,
                    sidx, sord, page, rows, genres)
                     , JsonRequestBehavior.AllowGet);
            }

            KeyValuePair<int, SystemProgramme[]> result = await progRepository.GetSystemProgrammesAtNextAsyncList(progType, new DateTimeOffset(new DateTime(1800, 1, 1)), (category != "null") ? category : null, sidx, sord, page, rows, genres);
            
            var jsonData = ControllerExtensions.GetJsonPagingInfo(page, rows, result);
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetTvProviderList()
        {
            return Json(await progRepository.GetProviderTypeAsyncList(), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetCategories()
        {
            return Json(await progRepository.GetCategories(), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SearchProgramme(int progType, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates)
        {
            if (User.Identity.IsAuthenticated && UserId != null)
            {
                return Json(await progRepository.SearchUserProgramme(UserId.Value, progType, findTitle), JsonRequestBehavior.AllowGet);
            }
            return Json(await progRepository.SearchProgramme(progType, findTitle, (category != "null") ? category : null, sidx, sord, page, rows, genres, dates), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetSystemProgrammePeriod(int progType)
        {
            return Json(await progRepository.GetSystemProgrammePeriodAsync(progType), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TreeView()
        {
            return PartialView();
        }

        public async Task<JsonResult> GetUserProgrammeOfDay(int progTypeID, int cid, string tsDate, string category)
        {
            if (UserId == null || !User.Identity.IsAuthenticated)
            {
                return await new Task<JsonResult>(null);
            }

            return Json(await progRepository.GetUserProgrammesOfDayList(UserId.Value, progTypeID, cid,
                                Convert.ToDateTime(tsDate).AddHours(5).AddMinutes(45),
                                Convert.ToDateTime(tsDate).AddDays(1).AddHours(5).AddMinutes(45), (category != "null") ? category : null), JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetGenres()
        {
            if (UserId == null || !User.Identity.IsAuthenticated)
               return Json(await genresRepository.GetGenres(null), JsonRequestBehavior.AllowGet);
            return Json(await genresRepository.GetGenres(UserId.Value), JsonRequestBehavior.AllowGet);
        }
    }
}