using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public partial record TvCategorySelectorModel : BaseTvProgModel
    {
        public TvCategorySelectorModel()
        {
            AvaliableCategories = new List<TvCategoryModel>();
        }

        public IList<TvCategoryModel> AvaliableCategories { get; set; }

        public string CurrentCategoryName { get; set; }
    }
}
