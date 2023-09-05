using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.Common;

namespace TvProgViewer.WebUI.Models.Profile
{
    public partial record ProfilePostsModel : BaseTvProgModel
    {
        public IList<PostsModel> Posts { get; set; }
        public PagerModel PagerModel { get; set; }
    }
}