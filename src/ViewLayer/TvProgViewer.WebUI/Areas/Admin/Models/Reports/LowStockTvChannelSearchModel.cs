using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a low stock tvchannel search model
    /// </summary>
    public partial record LowStockTvChannelSearchModel : BaseSearchModel
    {
        #region Ctor

        public LowStockTvChannelSearchModel()
        {
            AvailablePublishedOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Reports.LowStock.SearchPublished")]
        public int SearchPublishedId { get; set; }
        public IList<SelectListItem> AvailablePublishedOptions { get; set; }

        #endregion
    }
}