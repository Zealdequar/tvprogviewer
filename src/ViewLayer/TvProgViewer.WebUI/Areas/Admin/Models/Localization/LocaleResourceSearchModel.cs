using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Localization
{
    /// <summary>
    /// Represents a locale resource search model
    /// </summary>
    public partial record LocaleResourceSearchModel : BaseSearchModel
    {
        #region Ctor

        public LocaleResourceSearchModel()
        {
            AddResourceString = new LocaleResourceModel();
        }

        #endregion

        #region Properties

        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Languages.Resources.SearchResourceName")]
        public string SearchResourceName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Languages.Resources.SearchResourceValue")]
        public string SearchResourceValue { get; set; }

        public LocaleResourceModel AddResourceString { get; set; }

        #endregion
    }
}