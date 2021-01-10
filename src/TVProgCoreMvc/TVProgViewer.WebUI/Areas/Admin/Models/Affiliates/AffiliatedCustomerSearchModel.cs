using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Affiliates
{
    /// <summary>
    /// Represents an affiliated user search model
    /// </summary>
    public partial record AffiliatedUserSearchModel : BaseSearchModel
    {
        #region Properties

        public int AffliateId { get; set; }

        #endregion
    }
}