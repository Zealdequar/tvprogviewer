using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Media
{
    public partial record VideoModel : BaseTvProgModel
    {
        public string VideoUrl { get; set; }

        public string Allow { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
