using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel model to add to the manufacturer
    /// </summary>
    public partial record AddTvChannelToManufacturerModel : BaseTvProgModel
    {
        #region Ctor

        public AddTvChannelToManufacturerModel()
        {
            SelectedTvChannelIds = new List<int>();
        }
        #endregion

        #region Properties

        public int ManufacturerId { get; set; }

        public IList<int> SelectedTvChannelIds { get; set; }

        #endregion
    }
}