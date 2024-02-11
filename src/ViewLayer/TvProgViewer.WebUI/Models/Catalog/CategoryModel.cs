using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.Media;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record CategoryModel : BaseTvProgEntityModel
    {
        public CategoryModel()
        {
            PictureModel = new PictureModel();
            FeaturedTvChannels = new List<TvChannelOverviewModel>();
            SubCategories = new List<SubCategoryModel>();
            CategoryBreadcrumb = new List<CategoryModel>();
            CatalogTvChannelsModel = new CatalogTvChannelsModel();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        
        public PictureModel PictureModel { get; set; }

        public bool DisplayCategoryBreadcrumb { get; set; }
        public IList<CategoryModel> CategoryBreadcrumb { get; set; }
        
        public IList<SubCategoryModel> SubCategories { get; set; }

        public IList<TvChannelOverviewModel> FeaturedTvChannels { get; set; }

        public CatalogTvChannelsModel CatalogTvChannelsModel { get; set; }

        #region Nested Classes

        public partial record SubCategoryModel : BaseTvProgEntityModel
        {
            public SubCategoryModel()
            {
                PictureModel = new PictureModel();
            }

            public string Name { get; set; }

            public string SeName { get; set; }

            public string Description { get; set; }

            public PictureModel PictureModel { get; set; }
        }

		#endregion
    }
}