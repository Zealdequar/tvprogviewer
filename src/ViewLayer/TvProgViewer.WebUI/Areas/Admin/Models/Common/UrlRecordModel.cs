using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents an URL record model
    /// </summary>
    public partial record UrlRecordModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.System.SeNames.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.System.SeNames.EntityId")]
        public int EntityId { get; set; }

        [TvProgResourceDisplayName("Admin.System.SeNames.EntityName")]
        public string EntityName { get; set; }

        [TvProgResourceDisplayName("Admin.System.SeNames.IsActive")]
        public bool IsActive { get; set; }

        [TvProgResourceDisplayName("Admin.System.SeNames.Language")]
        public string Language { get; set; }

        [TvProgResourceDisplayName("Admin.System.SeNames.Details")]
        public string DetailsUrl { get; set; }

        #endregion
    }
}