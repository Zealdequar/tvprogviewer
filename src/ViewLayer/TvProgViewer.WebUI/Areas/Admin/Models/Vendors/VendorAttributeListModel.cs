using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor attribute list model
    /// </summary>
    public partial record VendorAttributeListModel : BasePagedListModel<VendorAttributeModel>
    {
    }
}