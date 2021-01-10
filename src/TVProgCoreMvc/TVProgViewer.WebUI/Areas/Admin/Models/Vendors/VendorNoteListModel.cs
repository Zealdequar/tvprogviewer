using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor note list model
    /// </summary>
    public partial record VendorNoteListModel : BasePagedListModel<VendorNoteModel>
    {
    }
}