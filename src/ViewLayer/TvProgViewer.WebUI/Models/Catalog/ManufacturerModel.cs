using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.Media;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record ManufacturerModel : BaseTvProgEntityModel
    {
        public ManufacturerModel()
        {
            PictureModel = new PictureModel();
            FeaturedTvChannels = new List<TvChannelOverviewModel>();
            CatalogTvChannelsModel = new CatalogTvChannelsModel();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }

        public PictureModel PictureModel { get; set; }

        public IList<TvChannelOverviewModel> FeaturedTvChannels { get; set; }

        public CatalogTvChannelsModel CatalogTvChannelsModel { get; set; }
    }
}