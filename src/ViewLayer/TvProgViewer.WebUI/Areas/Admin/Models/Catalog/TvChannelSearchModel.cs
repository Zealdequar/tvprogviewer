using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.WebUI.Areas.Admin.Models.Common;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel search model
    /// </summary>
    public partial record TvChannelSearchModel : BaseSearchModel
    {
        #region Ctor

        public TvChannelSearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            AvailableTvChannelTypes = new List<SelectListItem>();
            AvailablePublishedOptions = new List<SelectListItem>();
            LicenseCheckModel = new();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchTvChannelName")]
        public string SearchTvChannelName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchCategory")]
        public int SearchCategoryId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchIncludeSubCategories")]
        public bool SearchIncludeSubCategories { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchManufacturer")]
        public int SearchManufacturerId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchStore")]
        public int SearchStoreId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchVendor")]
        public int SearchVendorId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchWarehouse")]
        public int SearchWarehouseId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchTvChannelType")]
        public int SearchTvChannelTypeId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchPublished")]
        public int SearchPublishedId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.GoDirectlyToSku")]
        public string GoDirectlyToSku { get; set; }

        public bool IsLoggedInAsVendor { get; set; }

        public bool AllowVendorsToImportTvChannels { get; set; }

        public LicenseCheckModel LicenseCheckModel { get; set; }

        public bool HideStoresList { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        public IList<SelectListItem> AvailableManufacturers { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public IList<SelectListItem> AvailableWarehouses { get; set; }

        public IList<SelectListItem> AvailableVendors { get; set; }

        public IList<SelectListItem> AvailableTvChannelTypes { get; set; }

        public IList<SelectListItem> AvailablePublishedOptions { get; set; }

        #endregion
    }
}