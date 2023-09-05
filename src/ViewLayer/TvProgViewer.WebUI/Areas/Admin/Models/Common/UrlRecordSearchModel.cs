using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents an URL record search model
    /// </summary>
    public partial record UrlRecordSearchModel : BaseSearchModel
    {
        #region Ctor

        public UrlRecordSearchModel()
        {
            AvailableLanguages = new List<SelectListItem>();
            AvailableActiveOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.System.SeNames.List.Name")]
        public string SeName { get; set; }

        [TvProgResourceDisplayName("Admin.System.SeNames.List.Language")]
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.System.SeNames.List.IsActive")]
        public int IsActiveId { get; set; }

        public IList<SelectListItem> AvailableLanguages { get; set; }

        public IList<SelectListItem> AvailableActiveOptions { get; set; }

        #endregion
    }
}