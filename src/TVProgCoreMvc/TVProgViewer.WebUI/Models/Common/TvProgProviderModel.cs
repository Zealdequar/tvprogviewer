using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Common
{
    public partial record TvProgProviderModel: BaseTvProgEntityModel
    {
        public string Name { get; set; }
    }
}
