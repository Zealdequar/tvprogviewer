using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Services.Forums;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class ForumLastPostViewComponent : TvProgViewComponent
    {
        private readonly IForumModelFactory _forumModelFactory;
        private readonly IForumService _forumService;

        public ForumLastPostViewComponent(IForumModelFactory forumModelFactory, IForumService forumService)
        {
            _forumModelFactory = forumModelFactory;
            _forumService = forumService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int forumPostId, bool showTopic)
        {
            var forumPost = await _forumService.GetPostByIdAsync(forumPostId);
            var model = await _forumModelFactory.PrepareLastPostModelAsync(forumPost, showTopic);

            return View(model);

        }
    }
}
