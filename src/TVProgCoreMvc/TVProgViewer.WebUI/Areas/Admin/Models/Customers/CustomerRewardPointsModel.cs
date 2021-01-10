using System;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a reward point model
    /// </summary>
    public partial record UserRewardPointsModel : BaseTvProgEntityModel
    {
        #region Properties

        public string StoreName { get; set; }

        public int Points { get; set; }

        public string PointsBalance { get; set; }

        public string Message { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? EndDate { get; set; }

        #endregion
    }
}