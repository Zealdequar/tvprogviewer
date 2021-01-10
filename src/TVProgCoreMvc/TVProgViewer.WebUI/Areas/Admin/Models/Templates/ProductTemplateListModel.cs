using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Templates
{
    /// <summary>
    /// Represents a product template list model
    /// </summary>
    public partial record ProductTemplateListModel : BasePagedListModel<ProductTemplateModel>
    {
    }
}