using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents an address attribute value list model
    /// </summary>
    public partial record AddressAttributeValueListModel : BasePagedListModel<AddressAttributeValueModel>
    {
    }
}