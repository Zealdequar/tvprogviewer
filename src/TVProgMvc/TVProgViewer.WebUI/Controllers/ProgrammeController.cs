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
        private IProgrammesRepository repository;
        private long? UserId { get { return LazyGlobalist.Instance.UserId(System.Web.HttpContext.Current); } }

        public ProgrammeController(IProgrammesRepository programmeRepository)
        {
            this.repository = programmeRepository;
        }

        public ActionResult List()
        {
            ViewData["UID"] = UserId.HasValue ? 1 : 0;
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetSystemProgrammeAtNow(int progType, string category, string sidx, string sord, int page, int rows)
        {           
            if (User.Identity.IsAuthenticated && UserId != null)
            {
                return Json(await repository.GetUserProgrammesAtNowAsyncList(UserId.Value, progType, DateTimeOffset.Now, (category != "null") ? category : null)
                    , JsonRequestBehavior.AllowGet);
            }

            KeyValuePair<int, SystemProgramme[]> result = await repository.GetSystemProgrammesAtNowAsyncList(progType, DateTimeOffset.Now, (category != "null") ? category : null,
                 sidx, sord, page, rows);

            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = result.Key;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = result.Value.ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetSystemProgrammeAtNext(int progType, string category, string sidx, string sord, int page, int rows)
        {
            if (User.Identity.IsAuthenticated && UserId != null)
            {
                return Json(await repository.GetUserProgrammesAtNextAsyncList(UserId.Value, progType, new DateTimeOffset(new DateTime(1800, 1, 1)), (category != "null") ? category : null)
                     , JsonRequestBehavior.AllowGet);
            }
            return Json(await repository.GetSystemProgrammesAtNextAsyncList(progType, new DateTimeOffset(new DateTime(1800,1,1)), (category != "null") ? category : null, sidx, sord, page, rows), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetTvProviderList()
        {
            return Json(await repository.GetProviderTypeAsyncList(), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetCategories()
        {
            return Json(await repository.GetCategories(), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SearchProgramme(int progType, string findTitle)
        {
            if (User.Identity.IsAuthenticated && UserId != null)
            {
                return Json(await repository.SearchUserProgramme(UserId.Value, progType, findTitle), JsonRequestBehavior.AllowGet);
            }
            return Json(await repository.SearchProgramme(progType, findTitle), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetSystemProgrammePeriod(int progType)
        {
            return Json(await repository.GetSystemProgrammePeriodAsync(progType), JsonRequestBehavior.AllowGet);
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

            return Json(await repository.GetUserProgrammesOfDayList(UserId.Value, progTypeID, cid,
                                Convert.ToDateTime(tsDate).AddHours(5).AddMinutes(45),
                                Convert.ToDateTime(tsDate).AddDays(1).AddHours(5).AddMinutes(45), (category != "null") ? category : null), JsonRequestBehavior.AllowGet);
        }
    }
}