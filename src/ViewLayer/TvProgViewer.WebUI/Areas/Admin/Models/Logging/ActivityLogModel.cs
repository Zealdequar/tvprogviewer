using System;
using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Logging
{
    /// <summary>
    /// Represents an activity log model
    /// </summary>
    public partial record ActivityLogModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Users.ActivityLog.Fields.ActivityLogType")]
        public string ActivityLogTypeName { get; set; }

        [TvProgResourceDisplayName("Admin.Users.ActivityLog.Fields.User")]
        public int UserId { get; set; }

        [TvProgResourceDisplayName("Admin.Users.ActivityLog.Fields.UserEmail")]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [TvProgResourceDisplayName("Admin.Users.ActivityLog.Fields.Comment")]
        public string Comment { get; set; }

        [TvProgResourceDisplayName("Admin.Users.ActivityLog.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [TvProgResourceDisplayName("Admin.Users.ActivityLog.Fields.IpAddress")]
        public string IpAddress { get; set; }

        #endregion
    }
}