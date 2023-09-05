using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Templates
{
    /// <summary>
    /// Represents a product template list model
    /// </summary>
    public partial record ProductTemplateListModel : BasePagedListModel<ProductTemplateModel>
    {
    }
}