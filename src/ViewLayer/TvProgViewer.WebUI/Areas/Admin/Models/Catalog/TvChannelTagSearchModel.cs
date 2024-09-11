using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel tag search model
    /// </summary>
    public partial record TvChannelTagSearchModel : BaseSearchModel
    {
        [TvProgResourceDisplayName("Admin.Catalog.TvChannelTags.Fields.SearchTagName")]
        public string SearchTagName { get; set; }
    }
}