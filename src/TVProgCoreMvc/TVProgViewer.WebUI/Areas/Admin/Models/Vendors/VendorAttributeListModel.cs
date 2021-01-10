using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor attribute list model
    /// </summary>
    public partial record VendorAttributeListModel : BasePagedListModel<VendorAttributeModel>
    {
    }
}