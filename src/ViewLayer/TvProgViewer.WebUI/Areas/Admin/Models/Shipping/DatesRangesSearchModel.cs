using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a dates and ranges search model
    /// </summary>
    public partial record DatesRangesSearchModel : BaseSearchModel
    {
        #region Ctor

        public DatesRangesSearchModel()
        {
            DeliveryDateSearchModel = new DeliveryDateSearchModel();
            TvChannelAvailabilityRangeSearchModel = new TvChannelAvailabilityRangeSearchModel();
        }

        #endregion

        #region Properties

        public DeliveryDateSearchModel DeliveryDateSearchModel { get; set; }

        public TvChannelAvailabilityRangeSearchModel TvChannelAvailabilityRangeSearchModel { get; set; }

        #endregion
    }
}