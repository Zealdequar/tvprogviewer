﻿using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record BackInStockSubscribeModel : BaseTvProgModel
    {
        public int TvChannelId { get; set; }
        public string TvChannelName { get; set; }
        public string TvChannelSeName { get; set; }

        public bool IsCurrentUserRegistered { get; set; }
        public bool SubscriptionAllowed { get; set; }
        public bool AlreadySubscribed { get; set; }

        public int MaximumBackInStockSubscriptions { get; set; }
        public int CurrentNumberOfBackInStockSubscriptions { get; set; }
    }
}