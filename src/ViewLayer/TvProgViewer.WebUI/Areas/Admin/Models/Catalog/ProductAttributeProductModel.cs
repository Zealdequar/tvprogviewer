﻿using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a model of products that use the product attribute
    /// </summary>
    public partial record ProductAttributeProductModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.UsedByProducts.Product")]
        public string ProductName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Attributes.ProductAttributes.UsedByProducts.Published")]
        public bool Published { get; set; }

        #endregion
    }
}