using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.UI.Paging;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a catalog tvchannels model
    /// </summary>
    public partial record CatalogTvChannelsModel : BasePageableModel
    {
        #region Properties

        /// <summary>
        /// Get or set a value indicating whether use standart or AJAX tvchannels loading (applicable to 'paging', 'filtering', 'view modes') in catalog
        /// </summary>
        public bool UseAjaxLoading { get; set; }

        /// <summary>
        /// Gets or sets the warning message
        /// </summary>
        public string WarningMessage { get; set; }

        /// <summary>
        /// Gets or sets the message if there are no tvchannels to return
        /// </summary>
        public string NoResultMessage { get; set; }

        /// <summary>
        /// Gets or sets the price range filter model
        /// </summary>
        public PriceRangeFilterModel PriceRangeFilter { get; set; }

        /// <summary>
        /// Gets or sets the specification filter model
        /// </summary>
        public SpecificationFilterModel SpecificationFilter { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer filter model
        /// </summary>
        public ManufacturerFilterModel ManufacturerFilter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tvchannel sorting is allowed
        /// </summary>
        public bool AllowTvChannelSorting { get; set; }

        /// <summary>
        /// Gets or sets available sort options
        /// </summary>
        public IList<SelectListItem> AvailableSortOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to change view mode
        /// </summary>
        public bool AllowTvChannelViewModeChanging { get; set; }

        /// <summary>
        /// Gets or sets available view mode options
        /// </summary>
        public IList<SelectListItem> AvailableViewModes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to select page size
        /// </summary>
        public bool AllowUsersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets available page size options
        /// </summary>
        public IList<SelectListItem> PageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets a order by
        /// </summary>
        public int? OrderBy { get; set; }

        /// <summary>
        /// Gets or sets a tvchannel sorting
        /// </summary>
        public string ViewMode { get; set; }

        /// <summary>
        /// Gets or sets the tvchannels
        /// </summary>
        public IList<TvChannelOverviewModel> TvChannels { get; set; }

        #endregion

        #region Ctor

        public CatalogTvChannelsModel()
        {
            PriceRangeFilter = new PriceRangeFilterModel();
            SpecificationFilter = new SpecificationFilterModel();
            ManufacturerFilter = new ManufacturerFilterModel();
            AvailableSortOptions = new List<SelectListItem>();
            AvailableViewModes = new List<SelectListItem>();
            PageSizeOptions = new List<SelectListItem>();
            TvChannels = new List<TvChannelOverviewModel>();
        }

        #endregion
    }
}
