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
        private readonly ITvChannelService _tvChannelService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SearchCompleteController(
            IPermissionService permissionService,
            ITvChannelService tvChannelService,
            IWorkContext workContext)
        {
            _permissionService = permissionService;
            _tvChannelService = tvChannelService;
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

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            var vendorId = 0;
            if (currentVendor != null)
            {
                vendorId = currentVendor.Id;
            }

            //tvChannels
            const int tvChannelNumber = 15;
            var tvChannels = await _tvChannelService.SearchTvChannelsAsync(0,
                vendorId: vendorId,
                keywords: term,
                pageSize: tvChannelNumber,
                showHidden: true);

            var result = (from p in tvChannels
                            select new
                            {
                                label = p.Name,
                                tvChannelid = p.Id
                            }).ToList();

            return Json(result);
        }

        #endregion
    }
}