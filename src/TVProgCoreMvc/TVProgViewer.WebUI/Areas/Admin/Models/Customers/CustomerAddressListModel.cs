using TVProgViewer.WebUI.Areas.Admin.Models.Common;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user address list model
    /// </summary>
    public partial record UserAddressListModel : BasePagedListModel<AddressModel>
    {
    }
}