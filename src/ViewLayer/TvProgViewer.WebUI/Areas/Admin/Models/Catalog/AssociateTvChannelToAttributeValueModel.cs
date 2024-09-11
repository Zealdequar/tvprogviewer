using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel model to associate to the tvChannel attribute value
    /// </summary>
    public partial record AssociateTvChannelToAttributeValueModel : BaseTvProgModel
    {
        #region Properties
        
        public int AssociatedToTvChannelId { get; set; }

        #endregion
    }
}