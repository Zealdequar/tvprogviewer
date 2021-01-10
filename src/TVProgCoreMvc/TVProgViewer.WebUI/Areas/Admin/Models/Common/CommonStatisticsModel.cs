using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Common
{
    public partial record CommonStatisticsModel : BaseTvProgModel
    {
        public int NumberOfOrders { get; set; }

        public int NumberOfUsers { get; set; }

        public int NumberOfPendingReturnRequests { get; set; }

        public int NumberOfLowStockProducts { get; set; }
    }
}