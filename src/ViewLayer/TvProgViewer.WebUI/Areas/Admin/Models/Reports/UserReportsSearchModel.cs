using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a user reports search model
    /// </summary>
    public partial record UserReportsSearchModel : BaseSearchModel
    {
        #region Ctor

        public UserReportsSearchModel()
        {
            BestUsersByOrderTotal = new BestUsersReportSearchModel();
            BestUsersByNumberOfOrders = new BestUsersReportSearchModel();
            RegisteredUsers = new RegisteredUsersReportSearchModel();
        }

        #endregion

        #region Properties

        public BestUsersReportSearchModel BestUsersByOrderTotal { get; set; }

        public BestUsersReportSearchModel BestUsersByNumberOfOrders { get; set; }

        public RegisteredUsersReportSearchModel RegisteredUsers { get; set; }

        #endregion
    }
}