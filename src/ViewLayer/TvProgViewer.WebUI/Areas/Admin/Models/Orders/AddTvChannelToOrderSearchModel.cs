using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a tvchannel search model to add to the order
    /// </summary>
    public partial record AddTvChannelToOrderSearchModel : BaseSearchModel
    {
        #region Ctor

        public AddTvChannelToOrderSearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailableTvChannelTypes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchTvChannelName")]
        public string SearchTvChannelName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchCategory")]
        public int SearchCategoryId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchManufacturer")]
        public int SearchManufacturerId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchTvChannelType")]
        public int SearchTvChannelTypeId { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        public IList<SelectListItem> AvailableManufacturers { get; set; }

        public IList<SelectListItem> AvailableTvChannelTypes { get; set; }

        public int OrderId { get; set; }

        #endregion
    }
}