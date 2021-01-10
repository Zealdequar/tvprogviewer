using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a review type list model
    /// </summary>
    public partial record ReviewTypeListModel : BasePagedListModel<ReviewTypeModel>
    {
    }
}
