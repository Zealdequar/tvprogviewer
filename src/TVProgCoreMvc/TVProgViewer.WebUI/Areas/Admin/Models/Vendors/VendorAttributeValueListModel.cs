using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor attribute value list model
    /// </summary>
    public partial record VendorAttributeValueListModel : BasePagedListModel<VendorAttributeValueModel>
    {
    }
}