using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor list model
    /// </summary>
    public partial record VendorListModel : BasePagedListModel<VendorModel>
    {
    }
}