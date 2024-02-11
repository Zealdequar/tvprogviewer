using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a manufacturer model
    /// </summary>
    public partial record ManufacturerModel : BaseTvProgEntityModel, IAclSupportedModel, IDiscountSupportedModel,
        ILocalizedModel<ManufacturerLocalizedModel>, IStoreMappingSupportedModel
    {
        #region Ctor

        public ManufacturerModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
            Locales = new List<ManufacturerLocalizedModel>();
            AvailableManufacturerTemplates = new List<SelectListItem>();

            AvailableDiscounts = new List<SelectListItem>();
            SelectedDiscountIds = new List<int>();

            SelectedUserRoleIds = new List<int>();
            AvailableUserRoles = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();

            ManufacturerTvChannelSearchModel = new ManufacturerTvChannelSearchModel();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Description")]
        public string Description { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.ManufacturerTemplate")]
        public int ManufacturerTemplateId { get; set; }

        public IList<SelectListItem> AvailableManufacturerTemplates { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaDescription")]
        public string MetaDescription { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaTitle")]
        public string MetaTitle { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.SeName")]
        public string SeName { get; set; }

        [UIHint("Picture")]
        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Picture")]
        public int PictureId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.PageSize")]
        public int PageSize { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.AllowUsersToSelectPageSize")]
        public bool AllowUsersToSelectPageSize { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.PageSizeOptions")]
        public string PageSizeOptions { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.PriceRangeFiltering")]
        public bool PriceRangeFiltering { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.PriceFrom")]
        public decimal PriceFrom { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.PriceTo")]
        public decimal PriceTo { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.ManuallyPriceRange")]
        public bool ManuallyPriceRange { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Published")]
        public bool Published { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Deleted")]
        public bool Deleted { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
        
        public IList<ManufacturerLocalizedModel> Locales { get; set; }

        //ACL (user roles)
        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.AclUserRoles")]
        public IList<int> SelectedUserRoleIds { get; set; }
        public IList<SelectListItem> AvailableUserRoles { get; set; }
        
        //store mapping
        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        //discounts
        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Discounts")]
        public IList<int> SelectedDiscountIds { get; set; }
        public IList<SelectListItem> AvailableDiscounts { get; set; }

        public ManufacturerTvChannelSearchModel ManufacturerTvChannelSearchModel { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        #endregion
    }

    public partial record ManufacturerLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.Description")]
        public string Description {get;set;}

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaDescription")]
        public string MetaDescription { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.MetaTitle")]
        public string MetaTitle { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Fields.SeName")]
        public string SeName { get; set; }
    }
}