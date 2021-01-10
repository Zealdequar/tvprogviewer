using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Directory
{
    public partial record StateProvinceModel : BaseTvProgModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}