using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel order search model
    /// </summary>
    public partial record TvChannelOrderSearchModel : BaseSearchModel
    {
        #region Properties

        public int TvChannelId { get; set; }
        
        #endregion
    }
}