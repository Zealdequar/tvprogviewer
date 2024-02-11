using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user role tvchannel search model
    /// </summary>
    public partial record UserRoleTvChannelSearchModel : BaseSearchModel
    {
        #region Ctor

        public UserRoleTvChannelSearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            AvailableTvChannelTypes = new List<SelectListItem>();
            AddTvChannelToUserRoleModel = new AddTvChannelToUserRoleModel();
        }

        #endregion

        #region Properties

        public AddTvChannelToUserRoleModel AddTvChannelToUserRoleModel { get; set; }

        public bool IsLoggedInAsVendor { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchTvChannelName")]
        public string SearchTvChannelName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchCategory")]
        public int SearchCategoryId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchManufacturer")]
        public int SearchManufacturerId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchStore")]
        public int SearchStoreId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchVendor")]
        public int SearchVendorId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.List.SearchTvChannelType")]
        public int SearchTvChannelTypeId { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        public IList<SelectListItem> AvailableManufacturers { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public IList<SelectListItem> AvailableVendors { get; set; }

        public IList<SelectListItem> AvailableTvChannelTypes { get; set; }

        #endregion
    }
}