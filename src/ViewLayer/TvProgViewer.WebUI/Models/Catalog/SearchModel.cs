using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record SearchModel : BaseTvProgModel
    {
        public SearchModel()
        {
            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            CatalogTvChannelsModel = new CatalogTvChannelsModel();
        }

        /// <summary>
        /// Query string
        /// </summary>
        [TvProgResourceDisplayName("Search.SearchTerm")]
        public string q { get; set; }

        /// <summary>
        /// Category ID
        /// </summary>
        [TvProgResourceDisplayName("Search.Category")]
        public int cid { get; set; }

        [TvProgResourceDisplayName("Search.IncludeSubCategories")]
        public bool isc { get; set; }

        /// <summary>
        /// Manufacturer ID
        /// </summary>
        [TvProgResourceDisplayName("Search.Manufacturer")]
        public int mid { get; set; }

        /// <summary>
        /// Vendor ID
        /// </summary>
        [TvProgResourceDisplayName("Search.Vendor")]
        public int vid { get; set; }

        /// <summary>
        /// A value indicating whether to search in descriptions
        /// </summary>
        [TvProgResourceDisplayName("Search.SearchInDescriptions")]
        public bool sid { get; set; }

        /// <summary>
        /// A value indicating whether "advanced search" is enabled
        /// </summary>
        [TvProgResourceDisplayName("Search.AdvancedSearch")]
        public bool advs { get; set; }

        /// <summary>
        /// A value indicating whether "allow search by vendor" is enabled
        /// </summary>
        public bool asv { get; set; }

        public CatalogTvChannelsModel CatalogTvChannelsModel { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailableManufacturers { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }

        #region Nested classes

        public partial record CategoryModel : BaseTvProgEntityModel
        {
            public string Breadcrumb { get; set; }
        }

        #endregion
    }
}