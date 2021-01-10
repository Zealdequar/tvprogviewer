using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a GDPR log search model
    /// </summary>
    public partial record GdprLogSearchModel : BaseSearchModel
    {
        #region Ctor

        public GdprLogSearchModel()
        {
            AvailableRequestTypes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Users.GdprLog.List.SearchEmail")]
        public string SearchEmail { get; set; }

        [TvProgResourceDisplayName("Admin.Users.GdprLog.List.SearchRequestType")]
        public int SearchRequestTypeId { get; set; }

        public IList<SelectListItem> AvailableRequestTypes { get; set; }

        #endregion
    }
}