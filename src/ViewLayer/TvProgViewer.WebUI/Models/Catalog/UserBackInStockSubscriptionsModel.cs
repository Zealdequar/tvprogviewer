using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.Common;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record UserBackInStockSubscriptionsModel : BaseTvProgModel
    {
        public UserBackInStockSubscriptionsModel()
        {
            Subscriptions = new List<BackInStockSubscriptionModel>();
        }

        public IList<BackInStockSubscriptionModel> Subscriptions { get; set; }
        public PagerModel PagerModel { get; set; }

        #region Nested classes

        public partial record BackInStockSubscriptionModel : BaseTvProgEntityModel
        {
            public int TvChannelId { get; set; }
            public string TvChannelName { get; set; }
            public string SeName { get; set; }
        }

        #endregion
    }
}