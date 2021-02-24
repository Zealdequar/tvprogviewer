﻿using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a related product model
    /// </summary>
    public partial record RelatedProductModel : BaseTvProgEntityModel
    {
        #region Properties

        public int ProductId2 { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Products.RelatedProducts.Fields.Product")]
        public string Product2Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Products.RelatedProducts.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        #endregion
    }
}