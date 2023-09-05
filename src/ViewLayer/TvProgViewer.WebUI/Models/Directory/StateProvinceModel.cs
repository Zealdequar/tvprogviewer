using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Directory
{
    public partial record StateProvinceModel : BaseTvProgModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}