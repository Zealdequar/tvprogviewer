using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents an address attribute list model
    /// </summary>
    public partial record AddressAttributeListModel : BasePagedListModel<AddressAttributeModel>
    {
    }
}