using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Common
{
    public partial record LanguageModel : BaseTvProgEntityModel
    {
        public string Name { get; set; }

        public string FlagImageFileName { get; set; }
    }
}