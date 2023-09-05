﻿using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product tag model
    /// </summary>
    public partial record ProductTagModel : BaseTvProgEntityModel, ILocalizedModel<ProductTagLocalizedModel>
    {
        #region Ctor

        public ProductTagModel()
        {
            Locales = new List<ProductTagLocalizedModel>();
        }
        
        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.ProductTags.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.ProductTags.Fields.ProductCount")]
        public int ProductCount { get; set; }

        public IList<ProductTagLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial record ProductTagLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.ProductTags.Fields.Name")]
        public string Name { get; set; }
    }
}