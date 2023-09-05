using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Misc.Sendinblue.Models
{
    /// <summary>
    /// Represents message template list model
    /// </summary>
    public record SendinblueMessageTemplateListModel : BasePagedListModel<SendinblueMessageTemplateModel>
    {
    }
}