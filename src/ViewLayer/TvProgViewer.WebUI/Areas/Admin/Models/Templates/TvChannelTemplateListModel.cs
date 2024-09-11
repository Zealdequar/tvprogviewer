using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Templates
{
    /// <summary>
    /// Represents a tvChannel template list model
    /// </summary>
    public partial record TvChannelTemplateListModel : BasePagedListModel<TvChannelTemplateModel>
    {
    }
}