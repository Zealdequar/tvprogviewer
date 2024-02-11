using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.WebUI.Components
{
    public partial class NewsRssHeaderLinkViewComponent : TvProgViewComponent
    {
        private readonly NewsSettings _newsSettings;

        public NewsRssHeaderLinkViewComponent(NewsSettings newsSettings)
        {
            _newsSettings = newsSettings;
        }

        public IViewComponentResult Invoke(int currentCategoryId, int currentTvChannelId)
        {
            if (!_newsSettings.Enabled || !_newsSettings.ShowHeaderRssUrl)
                return Content("");

            return View();
        }
    }
}
