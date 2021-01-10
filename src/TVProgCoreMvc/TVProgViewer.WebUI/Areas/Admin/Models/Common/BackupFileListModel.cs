using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents a backup file list model
    /// </summary>
    public partial record BackupFileListModel : BasePagedListModel<BackupFileModel>
    {
    }
}