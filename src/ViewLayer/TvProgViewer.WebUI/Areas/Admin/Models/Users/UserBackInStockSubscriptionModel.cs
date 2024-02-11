using System;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user back in stock subscription model
    /// </summary>
    public partial record UserBackInStockSubscriptionModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Users.Users.BackInStockSubscriptions.Store")]
        public string StoreName { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.BackInStockSubscriptions.TvChannel")]
        public int TvChannelId { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.BackInStockSubscriptions.TvChannel")]
        public string TvChannelName { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.BackInStockSubscriptions.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}