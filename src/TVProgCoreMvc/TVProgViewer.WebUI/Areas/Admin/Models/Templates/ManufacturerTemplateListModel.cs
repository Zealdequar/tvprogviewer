using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Templates
{
    /// <summary>
    /// Represents a manufacturer template list model
    /// </summary>
    public partial record ManufacturerTemplateListModel : BasePagedListModel<ManufacturerTemplateModel>
    {
    }
}