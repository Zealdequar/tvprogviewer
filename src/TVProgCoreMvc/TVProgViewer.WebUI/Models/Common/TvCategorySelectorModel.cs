using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Common
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
