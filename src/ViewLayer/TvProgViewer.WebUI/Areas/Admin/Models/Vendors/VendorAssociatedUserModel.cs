using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor associated user model
    /// </summary>
    public partial record VendorAssociatedUserModel : BaseTvProgEntityModel
    {
        #region Properties

        public string Email { get; set; }

        #endregion
    }
}