using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Shipping
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
            ProductAvailabilityRangeSearchModel = new ProductAvailabilityRangeSearchModel();
        }

        #endregion

        #region Properties

        public DeliveryDateSearchModel DeliveryDateSearchModel { get; set; }

        public ProductAvailabilityRangeSearchModel ProductAvailabilityRangeSearchModel { get; set; }

        #endregion
    }
}