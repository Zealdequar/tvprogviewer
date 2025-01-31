﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Tax.Avalara.Models.Tax
{
    /// <summary>
    /// Represents a tax category search model
    /// </summary>
    public record TaxCategorySearchModel : BaseSearchModel
    {
        #region Ctor

        public TaxCategorySearchModel()
        {
            AddTaxCategory = new TaxCategoryModel();
            AvailableTypes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public TaxCategoryModel AddTaxCategory { get; set; }

        public IList<SelectListItem> AvailableTypes { get; set; }

        #endregion
    }
}