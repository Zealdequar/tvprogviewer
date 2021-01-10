using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Templates
{
    /// <summary>
    /// Represents a category template list model
    /// </summary>
    public partial record CategoryTemplateListModel : BasePagedListModel<CategoryTemplateModel>
    {
    }
}