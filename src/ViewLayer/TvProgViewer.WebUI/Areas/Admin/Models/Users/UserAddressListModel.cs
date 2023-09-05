using TvProgViewer.WebUI.Areas.Admin.Models.Common;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user address list model
    /// </summary>
    public partial record UserAddressListModel : BasePagedListModel<AddressModel>
    {
    }
}