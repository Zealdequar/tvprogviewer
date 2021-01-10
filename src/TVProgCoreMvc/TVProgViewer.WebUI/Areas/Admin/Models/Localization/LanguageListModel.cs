using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Localization
{
    /// <summary>
    /// Represents a language list model
    /// </summary>
    public partial record LanguageListModel : BasePagedListModel<LanguageModel>
    {
    }
}