using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.WebUI.Components
{
    public partial class BlogTagsViewComponent : TvProgViewComponent
    {
        private readonly BlogSettings _blogSettings;
        private readonly IBlogModelFactory _blogModelFactory;

        public BlogTagsViewComponent(BlogSettings blogSettings, IBlogModelFactory blogModelFactory)
        {
            _blogSettings = blogSettings;
            _blogModelFactory = blogModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int currentCategoryId, int currentTvChannelId)
        {
            if (!_blogSettings.Enabled)
                return Content("");

            var model = await _blogModelFactory.PrepareBlogPostTagListModelAsync();
            return View(model);
        }
    }
}
