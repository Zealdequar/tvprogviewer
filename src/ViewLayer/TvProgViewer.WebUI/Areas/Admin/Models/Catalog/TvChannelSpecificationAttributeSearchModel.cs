using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel specification attribute search model
    /// </summary>
    public partial record TvChannelSpecificationAttributeSearchModel : BaseSearchModel
    {
        #region Properties

        public int TvChannelId { get; set; }
        
        #endregion
    }
}