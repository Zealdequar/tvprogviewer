using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public partial record TvTypeProgModel: BaseTvProgEntityModel
    {
        public string Name { get; set; }
    }
}