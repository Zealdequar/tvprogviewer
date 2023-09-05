using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Widgets.What3words.Models
{
    /// <summary>
    /// Represents what3words address model
    /// </summary>
    public record What3wordsAddressModel : BaseTvProgModel
    {
        public string ApiKey { get; set; }

        public string Prefix { get; set; }
    }
}
