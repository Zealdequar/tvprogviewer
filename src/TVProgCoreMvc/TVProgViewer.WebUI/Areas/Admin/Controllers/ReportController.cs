using Microsoft.AspNetCore.Mvc;
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

        public virtual IActionResult LowStock()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareLowStockProductSearchModel(new LowStockProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult LowStockList(LowStockProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _reportModelFactory.PrepareLowStockProductListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Bestsellers

        public virtual IActionResult Bestsellers()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareBestsellerSearchModel(new BestsellerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult BestsellersList(BestsellerSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _reportModelFactory.PrepareBestsellerListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Never Sold

        public virtual IActionResult NeverSold()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareNeverSoldSearchModel(new NeverSoldReportSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult NeverSoldList(NeverSoldReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _reportModelFactory.PrepareNeverSoldListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Country sales

        public virtual IActionResult CountrySales()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.OrderCountryReport))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareCountrySalesSearchModel(new CountryReportSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CountrySalesList(CountryReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.OrderCountryReport))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _reportModelFactory.PrepareCountrySalesListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region User reports

        public virtual IActionResult RegisteredUsers()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareUserReportsSearchModel(new UserReportsSearchModel());

            return View(model);
        }

        public virtual IActionResult BestUsersByOrderTotal()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareUserReportsSearchModel(new UserReportsSearchModel());

            return View(model);
        }

        public virtual IActionResult BestUsersByNumberOfOrders()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //prepare model
            var model = _reportModelFactory.PrepareUserReportsSearchModel(new UserReportsSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ReportBestUsersByOrderTotalList(BestUsersReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _reportModelFactory.PrepareBestUsersReportListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ReportBestUsersByNumberOfOrdersList(BestUsersReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _reportModelFactory.PrepareBestUsersReportListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ReportRegisteredUsersList(RegisteredUsersReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _reportModelFactory.PrepareRegisteredUsersReportListModel(searchModel);

            return Json(model);
        }        

        #endregion

        #endregion
    }
}
