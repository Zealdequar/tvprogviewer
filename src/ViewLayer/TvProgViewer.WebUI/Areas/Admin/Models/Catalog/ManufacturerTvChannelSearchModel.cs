using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a manufacturer tvChannel search model
    /// </summary>
    public partial record ManufacturerTvChannelSearchModel : BaseSearchModel
    {
        #region Properties

        public int ManufacturerId { get; set; }

        #endregion
    }
}