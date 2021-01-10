using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents an order note list model
    /// </summary>
    public partial record OrderNoteListModel : BasePagedListModel<OrderNoteModel>
    {
    }
}