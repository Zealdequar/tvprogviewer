using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents an order note list model
    /// </summary>
    public partial record OrderNoteListModel : BasePagedListModel<OrderNoteModel>
    {
    }
}