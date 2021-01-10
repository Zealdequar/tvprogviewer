using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a bestseller brief search model
    /// </summary>
    public partial record BestsellerBriefSearchModel : BaseSearchModel
    {
        #region Properties

        //keep it synchronized to OrderReportService record, BestSellersReport() method, orderBy parameter
        //TODO: move from int to enum
        public int OrderBy { get; set; }

        #endregion
    }
}