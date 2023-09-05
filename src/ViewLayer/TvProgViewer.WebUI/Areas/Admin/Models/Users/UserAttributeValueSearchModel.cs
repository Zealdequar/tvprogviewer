using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user attribute value search model
    /// </summary>
    public partial record UserAttributeValueSearchModel : BaseSearchModel
    {
        #region Properties

        public int UserAttributeId { get; set; }

        #endregion
    }
}