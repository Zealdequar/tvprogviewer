using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Misc.Sendinblue.Models
{
    /// <summary>
    /// Represents SMS list model
    /// </summary>
    public record SmsListModel : BasePagedListModel<SmsModel>
    {
    }
}