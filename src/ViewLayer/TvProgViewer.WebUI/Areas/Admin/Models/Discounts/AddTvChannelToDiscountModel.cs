using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a tvChannel model to add to the discount
    /// </summary>
    public partial record AddTvChannelToDiscountModel : BaseTvProgModel
    {
        #region Ctor

        public AddTvChannelToDiscountModel()
        {
            SelectedTvChannelIds = new List<int>();
        }
        #endregion

        #region Properties

        public int DiscountId { get; set; }

        public IList<int> SelectedTvChannelIds { get; set; }

        #endregion
    }
}