using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor search model
    /// </summary>
    public partial record VendorSearchModel : BaseSearchModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Vendors.List.SearchName")]
        public string SearchName { get; set; }

        [TvProgResourceDisplayName("Admin.Vendors.List.SearchEmail")]
        public string SearchEmail { get; set; }

        #endregion
    }
}