using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class ForumBreadcrumbViewComponent : TvProgViewComponent
    {
        private readonly IForumModelFactory _forumModelFactory;

        public ForumBreadcrumbViewComponent(IForumModelFactory forumModelFactory)
        {
            _forumModelFactory = forumModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? forumGroupId, int? forumId, int? forumTopicId)
        {
            var model = await _forumModelFactory.PrepareForumBreadcrumbModelAsync(forumGroupId, forumId, forumTopicId);
            return View(model);
        }
    }
}
