using System;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user activity log model
    /// </summary>
    public partial record UserActivityLogModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Users.Users.ActivityLog.ActivityLogType")]
        public string ActivityLogTypeName { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.ActivityLog.Comment")]
        public string Comment { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.ActivityLog.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.ActivityLog.IpAddress")]
        public string IpAddress { get; set; }

        #endregion
    }
}