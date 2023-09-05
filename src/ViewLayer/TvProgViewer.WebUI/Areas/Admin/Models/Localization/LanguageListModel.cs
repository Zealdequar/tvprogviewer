using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Localization
{
    /// <summary>
    /// Represents a language list model
    /// </summary>
    public partial record LanguageListModel : BasePagedListModel<LanguageModel>
    {
    }
}