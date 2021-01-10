using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.WebUI.Models.Common;

namespace TVProgViewer.WebUI.Models.Profile
{
    public partial record ProfilePostsModel : BaseTvProgModel
    {
        public IList<PostsModel> Posts { get; set; }
        public PagerModel PagerModel { get; set; }
    }
}