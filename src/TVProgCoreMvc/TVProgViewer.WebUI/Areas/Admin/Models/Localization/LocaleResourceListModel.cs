using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Localization
{
    /// <summary>
    /// Represents a locale resource list model
    /// </summary>
    public record LocaleResourceListModel : BasePagedListModel<LocaleResourceModel>
    {
    }
}