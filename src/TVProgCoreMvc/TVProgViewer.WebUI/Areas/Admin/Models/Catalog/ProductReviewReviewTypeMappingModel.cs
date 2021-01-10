using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product review and review type mapping model
    /// </summary>
    public record ProductReviewReviewTypeMappingModel : BaseTvProgEntityModel
    {
        #region Properties

        public int ProductReviewId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.ProductReviewsExt.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.ProductReviewsExt.Fields.Description")]
        public string Description { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.ProductReviewsExt.Fields.VisibleToAllUsers")]
        public bool VisibleToAllUsers { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.ProductReviewsExt.Fields.Rating")]
        public int Rating { get; set; }

        #endregion
    }
}
