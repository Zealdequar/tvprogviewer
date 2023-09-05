using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product video model
    /// </summary>
    public partial record ProductVideoModel : BaseTvProgEntityModel
    {
        #region Properties

        public int ProductId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Products.Multimedia.Videos.Fields.VideoUrl")]
        public string VideoUrl { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Products.Multimedia.Videos.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        #endregion
    }
}
