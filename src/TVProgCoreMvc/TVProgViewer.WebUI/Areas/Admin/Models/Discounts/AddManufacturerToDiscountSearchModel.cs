using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a manufacturer search model to add to the discount
    /// </summary>
    public partial record AddManufacturerToDiscountSearchModel : BaseSearchModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.Manufacturers.List.SearchManufacturerName")]
        public string SearchManufacturerName { get; set; }

        #endregion
    }
}