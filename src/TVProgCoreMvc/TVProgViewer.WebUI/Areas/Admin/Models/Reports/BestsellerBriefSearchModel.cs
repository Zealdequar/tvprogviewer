using TVProgViewer.Services.Orders;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a bestseller brief search model
    /// </summary>
    public partial record BestsellerBriefSearchModel : BaseSearchModel
    {
        #region Properties

        public OrderByEnum OrderBy { get; set; }

        #endregion
    }
}