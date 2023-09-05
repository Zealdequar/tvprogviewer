using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Misc.Sendinblue.Models
{
    /// <summary>
    /// Represents message template model
    /// </summary>
    public record SendinblueMessageTemplateModel : BaseTvProgEntityModel
    {
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string ListOfStores { get; set; }

        public bool UseSendinblueTemplate { get; set; }

        public string EditLink { get; set; }
    }
}