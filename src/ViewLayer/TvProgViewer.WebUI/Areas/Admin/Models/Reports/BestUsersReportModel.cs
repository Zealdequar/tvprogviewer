using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a best users report model
    /// </summary>
    public partial record BestUsersReportModel : BaseTvProgModel
    {
        #region Properties

        public int UserId { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Users.BestBy.Fields.User")]
        public string UserName { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Users.BestBy.Fields.OrderTotal")]
        public string OrderTotal { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Users.BestBy.Fields.OrderCount")]
        public decimal OrderCount { get; set; }
        
        #endregion
    }
}