using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Media
{
    public partial record PictureModel : BaseTvProgModel
    {
        public string ImageUrl { get; set; }

        public string ThumbImageUrl { get; set; }

        public string FullSizeImageUrl { get; set; }

        public string Title { get; set; }

        public string AlternateText { get; set; }
    }
}