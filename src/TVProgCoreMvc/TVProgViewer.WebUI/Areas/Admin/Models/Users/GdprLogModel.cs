using System;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a GDPR log (request) model
    /// </summary>
    public partial record GdprLogModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Users.GdprLog.Fields.UserInfo")]
        public string UserInfo { get; set; }

        [TvProgResourceDisplayName("Admin.Users.GdprLog.Fields.RequestType")]
        public string RequestType { get; set; }

        [TvProgResourceDisplayName("Admin.Users.GdprLog.Fields.RequestDetails")]
        public string RequestDetails { get; set; }

        [TvProgResourceDisplayName("Admin.Users.GdprLog.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}