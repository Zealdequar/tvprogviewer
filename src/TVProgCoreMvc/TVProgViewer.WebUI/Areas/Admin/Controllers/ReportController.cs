using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TVProgViewer.Services.Security;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.WebUI.Areas.Admin.Models.Reports;

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class ReportController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IReportModelFactory _reportModelFactory;

        #endregion

        #region Ctor

        public ReportController(
            IPermissionService permissionService,
            IReportModelFactory reportModelFactory)
        {
            _permissionService = permissionService;
            _reportModelFactory = reportModelFactory;
        }

        #endregion

        #region Methods

        #region Low stock

        public virtual async Task<IActionResult> LowStock()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = await _reportModelFactory.PrepareLowStockProductSearchModelAsync(new LowStockProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> LowStockList(LowStockProductSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _reportModelFactory.PrepareLowStockProductListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Bestsellers

        public virtual async Task<IActionResult> Bestsellers()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = await _reportModelFactory.PrepareBestsellerSearchModelAsync(new BestsellerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> BestsellersList(BestsellerSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _reportModelFactory.PrepareBestsellerListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> BestsellersReportAggregates(BestsellerSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var totalAmount = await _reportModelFactory.GetBestsellerTotalAmountAsync(searchModel);

            return Json(new { aggregatortotal = totalAmount });
        }

        #endregion

        #region Never Sold

        public virtual async Task<IActionResult> NeverSold()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = await _reportModelFactory.PrepareNeverSoldSearchModelAsync(new NeverSoldReportSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> NeverSoldList(NeverSoldReportSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _reportModelFactory.PrepareNeverSoldListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Country sales

        public virtual async Task<IActionResult> CountrySales()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.OrderCountryReport))
                return AccessDeniedView();

            //prepare model
            var model = await _reportModelFactory.PrepareCountrySalesSearchModelAsync(new CountryReportSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CountrySalesList(CountryReportSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.OrderCountryReport))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _reportModelFactory.PrepareCountrySalesListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region User reports

        public virtual async Task<IActionResult> RegisteredUsers()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //prepare model
            var model = await _reportModelFactory.PrepareUserReportsSearchModelAsync(new UserReportsSearchModel());

            return View(model);
        }

        public virtual async Task<IActionResult> BestUsersByOrderTotal()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //prepare model
            var model = await _reportModelFactory.PrepareUserReportsSearchModelAsync(new UserReportsSearchModel());

            return View(model);
        }

        public virtual async Task<IActionResult> BestUsersByNumberOfOrders()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //prepare model
            var model = await _reportModelFactory.PrepareUserReportsSearchModelAsync(new UserReportsSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReportBestUsersByOrderTotalList(BestUsersReportSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _reportModelFactory.PrepareBestUsersReportListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReportBestUsersByNumberOfOrdersList(BestUsersReportSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _reportModelFactory.PrepareBestUsersReportListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReportRegisteredUsersList(RegisteredUsersReportSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _reportModelFactory.PrepareRegisteredUsersReportListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #endregion
    }
}
