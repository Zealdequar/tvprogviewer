using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.Media;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record VendorModel : BaseTvProgEntityModel
    {
        public VendorModel()
        {
            PictureModel = new PictureModel();
            CatalogProductsModel = new CatalogProductsModel();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public bool AllowUsersToContactVendors { get; set; }

        public PictureModel PictureModel { get; set; }

        public CatalogProductsModel CatalogProductsModel { get; set; }
    }
}