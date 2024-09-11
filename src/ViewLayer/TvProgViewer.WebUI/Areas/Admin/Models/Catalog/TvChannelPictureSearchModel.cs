using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel picture search model
    /// </summary>
    public partial record TvChannelPictureSearchModel : BaseSearchModel
    {
        #region Properties

        public int TvChannelId { get; set; }
        
        #endregion
    }
}