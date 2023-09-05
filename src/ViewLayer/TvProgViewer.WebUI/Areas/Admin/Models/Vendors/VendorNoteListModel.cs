using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor note list model
    /// </summary>
    public partial record VendorNoteListModel : BasePagedListModel<VendorNoteModel>
    {
    }
}