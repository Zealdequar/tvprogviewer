using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Templates
{
    /// <summary>
    /// Represents a manufacturer template list model
    /// </summary>
    public partial record ManufacturerTemplateListModel : BasePagedListModel<ManufacturerTemplateModel>
    {
    }
}