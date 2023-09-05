using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a registered users report model
    /// </summary>
    public partial record RegisteredUsersReportModel : BaseTvProgModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Reports.Users.RegisteredUsers.Fields.Period")]
        public string Period { get; set; }

        [TvProgResourceDisplayName("Admin.Reports.Users.RegisteredUsers.Fields.Users")]
        public int Users { get; set; }

        #endregion
    }
}