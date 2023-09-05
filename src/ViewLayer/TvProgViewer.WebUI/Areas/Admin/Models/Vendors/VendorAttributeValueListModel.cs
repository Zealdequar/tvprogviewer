using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor attribute value list model
    /// </summary>
    public partial record VendorAttributeValueListModel : BasePagedListModel<VendorAttributeValueModel>
    {
    }
}