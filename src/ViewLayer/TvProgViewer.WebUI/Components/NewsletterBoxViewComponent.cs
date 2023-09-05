using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Components;

namespace TvProgViewer.WebUI.Components
{
    public partial class NewsletterBoxViewComponent : TvProgViewComponent
    {
        private readonly UserSettings _userSettings;
        private readonly INewsletterModelFactory _newsletterModelFactory;

        public NewsletterBoxViewComponent(UserSettings userSettings, INewsletterModelFactory newsletterModelFactory)
        {
            _userSettings = userSettings;
            _newsletterModelFactory = newsletterModelFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_userSettings.HideNewsletterBlock)
                return Content("");

            var model = await _newsletterModelFactory.PrepareNewsletterBoxModelAsync();
            return View(model);
        }
    }
}
