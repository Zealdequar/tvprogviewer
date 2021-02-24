using TVProgViewer.WebUI.Areas.Admin.Models.Common;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user address model
    /// </summary>
    public partial record UserAddressModel : BaseTvProgModel
    {
        #region Ctor

        public UserAddressModel()
        {
            Address = new AddressModel();
        }

        #endregion

        #region Properties

        public int UserId { get; set; }

        public AddressModel Address { get; set; }

        #endregion
    }
}