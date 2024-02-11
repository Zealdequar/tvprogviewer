using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record CategorySimpleModel : BaseTvProgEntityModel
    {
        public CategorySimpleModel()
        {
            SubCategories = new List<CategorySimpleModel>();
        }

        public string Name { get; set; }

        public string SeName { get; set; }

        public int? NumberOfTvChannels { get; set; }

        public bool IncludeInTopMenu { get; set; }

        public List<CategorySimpleModel> SubCategories { get; set; }

        public bool HaveSubCategories { get; set; }

        public string Route { get; set; }
    }
}