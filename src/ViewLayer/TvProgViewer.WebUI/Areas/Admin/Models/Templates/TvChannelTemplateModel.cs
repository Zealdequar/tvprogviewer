using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Templates
{
    /// <summary>
    /// Represents a tvChannel template model
    /// </summary>
    public partial record TvChannelTemplateModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.System.Templates.TvChannel.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.System.Templates.TvChannel.ViewPath")]
        public string ViewPath { get; set; }

        [TvProgResourceDisplayName("Admin.System.Templates.TvChannel.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [TvProgResourceDisplayName("Admin.System.Templates.TvChannel.IgnoredTvChannelTypes")]
        public string IgnoredTvChannelTypes { get; set; }

        #endregion
    }
}