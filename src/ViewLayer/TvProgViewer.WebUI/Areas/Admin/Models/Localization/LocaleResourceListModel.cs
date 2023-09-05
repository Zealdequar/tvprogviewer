using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Localization
{
    /// <summary>
    /// Represents a locale resource list model
    /// </summary>
    public partial record LocaleResourceListModel : BasePagedListModel<LocaleResourceModel>
    {
    }
}