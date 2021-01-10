using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor list model
    /// </summary>
    public partial record VendorListModel : BasePagedListModel<VendorModel>
    {
    }
}