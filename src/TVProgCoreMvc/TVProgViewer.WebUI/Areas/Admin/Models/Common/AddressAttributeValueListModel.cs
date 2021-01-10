using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents an address attribute value list model
    /// </summary>
    public record AddressAttributeValueListModel : BasePagedListModel<AddressAttributeValueModel>
    {
    }
}