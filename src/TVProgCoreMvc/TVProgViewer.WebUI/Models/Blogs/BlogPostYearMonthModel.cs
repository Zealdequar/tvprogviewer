using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Blogs
{
    public partial record BlogPostYearModel : BaseTvProgModel
    {
        public BlogPostYearModel()
        {
            Months = new List<BlogPostMonthModel>();
        }
        public int Year { get; set; }
        public IList<BlogPostMonthModel> Months { get; set; }
    }

    public partial record BlogPostMonthModel : BaseTvProgModel
    {
        public int Month { get; set; }

        public int BlogPostCount { get; set; }
    }
}