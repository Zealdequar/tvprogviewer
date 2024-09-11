using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel review and review type mapping model
    /// </summary>
    public partial record TvChannelReviewReviewTypeMappingModel : BaseTvProgEntityModel
    {
        #region Properties

        public int TvChannelReviewId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviewsExt.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviewsExt.Fields.Description")]
        public string Description { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviewsExt.Fields.VisibleToAllUsers")]
        public bool VisibleToAllUsers { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannelReviewsExt.Fields.Rating")]
        public int Rating { get; set; }

        #endregion
    }
}
