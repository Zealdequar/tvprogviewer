﻿using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a specification attribute option search model
    /// </summary>
    public partial record SpecificationAttributeOptionSearchModel : BaseSearchModel
    {
        #region Properties

        public int SpecificationAttributeId { get; set; }

        #endregion
    }
}