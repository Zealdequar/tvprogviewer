using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product tag search model
    /// </summary>
    public partial record ProductTagSearchModel : BaseSearchModel
    {
        [TvProgResourceDisplayName("Admin.Catalog.ProductTags.Fields.SearchTagName")]
        public string SearchTagName { get; set; }
    }
}