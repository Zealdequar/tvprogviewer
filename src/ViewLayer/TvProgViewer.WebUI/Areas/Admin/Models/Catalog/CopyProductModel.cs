using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a copy product model
    /// </summary>
    public partial record CopyProductModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.Products.Copy.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Products.Copy.CopyMultimedia")]
        public bool CopyMultimedia { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.Products.Copy.Published")]
        public bool Published { get; set; }

        #endregion
    }
}