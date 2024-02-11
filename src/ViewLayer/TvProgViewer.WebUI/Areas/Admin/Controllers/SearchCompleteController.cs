using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Security;

namespace TvProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class SearchCompleteController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ITvChannelService _tvchannelService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SearchCompleteController(
            IPermissionService permissionService,
            ITvChannelService tvchannelService,
            IWorkContext workContext)
        {
            _permissionService = permissionService;
            _tvchannelService = tvchannelService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> SearchAutoComplete(string term)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return Content(string.Empty);

            const int searchTermMinimumLength = 3;
            if (string.IsNullOrWhiteSpace(term) || term.Length < searchTermMinimumLength)
                return Content(string.Empty);

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            var vendorId = 0;
            if (currentVendor != null)
            {
                vendorId = currentVendor.Id;
            }

            //tvchannels
            const int tvchannelNumber = 15;
            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(0,
                vendorId: vendorId,
                keywords: term,
                pageSize: tvchannelNumber,
                showHidden: true);

            var result = (from p in tvchannels
                            select new
                            {
                                label = p.Name,
                                tvchannelid = p.Id
                            }).ToList();

            return Json(result);
        }

        #endregion
    }
}