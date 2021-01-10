using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Common
{
    public partial record TvTypeProgModel: BaseTvProgEntityModel
    {
        public string Name { get; set; }
    }
}