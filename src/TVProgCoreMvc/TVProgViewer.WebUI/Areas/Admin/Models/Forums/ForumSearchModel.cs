using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Forums
{
    /// <summary>
    /// Represents a forum search model
    /// </summary>
    public partial record ForumSearchModel : BaseSearchModel
    {
        #region Properties

        public int ForumGroupId { get; set; }

        #endregion
    }
}