using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class ForumActiveDiscussionsSmallViewComponent : TvProgViewComponent
    {
        private readonly IForumModelFactory _forumModelFactory;

        public ForumActiveDiscussionsSmallViewComponent(IForumModelFactory forumModelFactory)
        {
            _forumModelFactory = forumModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _forumModelFactory.PrepareActiveDiscussionsModelAsync();
            if (!model.ForumTopics.Any())
                return Content("");

            return View(model);
        }
    }
}
