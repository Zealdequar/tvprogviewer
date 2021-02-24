using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Services.Security;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.Web.Framework.Components;

namespace TVProgViewer.WebUI.Areas.Admin.Components
{
    /// <summary>
    /// Represents a view component that displays common statistics
    /// </summary>
    public class CommonStatisticsViewComponent : TvProgViewComponent
    {
        #region Fields

        private readonly ICommonModelFactory _commonModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CommonStatisticsViewComponent(ICommonModelFactory commonModelFactory,
            IPermissionService permissionService,
            IWorkContext workContext)
        {
            _commonModelFactory = commonModelFactory;
            _permissionService = permissionService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <returns>View component result</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUsers) ||
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders) ||
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReturnRequests) ||
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
            {
                return Content(string.Empty);
            }

            //a vendor doesn't have access to this report
            if (await _workContext.GetCurrentVendorAsync() != null)
                return Content(string.Empty);

            //prepare model
            var model = await _commonModelFactory.PrepareCommonStatisticsModelAsync();

            return View(model);
        }

        #endregion
    }
}