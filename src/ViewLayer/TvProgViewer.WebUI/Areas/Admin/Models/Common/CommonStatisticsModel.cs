using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Common
{
    public partial record CommonStatisticsModel : BaseTvProgModel
    {
        public int NumberOfOrders { get; set; }

        public int NumberOfUsers { get; set; }

        public int NumberOfPendingReturnRequests { get; set; }

        public int NumberOfLowStockTvChannels { get; set; }
    }
}