using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents a backup file list model
    /// </summary>
    public partial record BackupFileListModel : BasePagedListModel<BackupFileModel>
    {
    }
}