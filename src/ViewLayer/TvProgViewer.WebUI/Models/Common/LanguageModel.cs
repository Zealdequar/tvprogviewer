using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public partial record LanguageModel : BaseTvProgEntityModel
    {
        public string Name { get; set; }

        public string FlagImageFileName { get; set; }
    }
}