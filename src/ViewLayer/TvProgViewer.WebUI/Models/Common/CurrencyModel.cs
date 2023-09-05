using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public partial record CurrencyModel : BaseTvProgEntityModel
    {
        public string Name { get; set; }

        public string CurrencySymbol { get; set; }
    }
}