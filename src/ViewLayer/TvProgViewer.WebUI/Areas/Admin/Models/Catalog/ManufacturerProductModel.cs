﻿using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a manufacturer product model
    /// </summary>
    public partial record ManufacturerProductModel : BaseTvProgEntityModel
    {
        #region Properties

        public int ManufacturerId { get; set; }

        public int ProductId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Products.Fields.Product")]
        public string ProductName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Products.Fields.IsFeaturedProduct")]
        public bool IsFeaturedProduct { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.Products.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        #endregion
    }
}