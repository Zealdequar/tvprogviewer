using System;
using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.User
{
    public partial record UserDownloadableTvChannelsModel : BaseTvProgModel
    {
        public UserDownloadableTvChannelsModel()
        {
            Items = new List<DownloadableTvChannelsModel>();
        }

        public IList<DownloadableTvChannelsModel> Items { get; set; }

        #region Nested classes

        public partial record DownloadableTvChannelsModel : BaseTvProgModel
        {
            public Guid OrderItemGuid { get; set; }

            public int OrderId { get; set; }
            public string CustomOrderNumber { get; set; }

            public int TvChannelId { get; set; }
            public string TvChannelName { get; set; }
            public string TvChannelSeName { get; set; }
            public string TvChannelAttributes { get; set; }

            public int DownloadId { get; set; }
            public int LicenseId { get; set; }

            public DateTime CreatedOn { get; set; }
        }

        #endregion
    }

    public partial record UserAgreementModel : BaseTvProgModel
    {
        public Guid OrderItemGuid { get; set; }
        public string UserAgreementText { get; set; }
    }
}