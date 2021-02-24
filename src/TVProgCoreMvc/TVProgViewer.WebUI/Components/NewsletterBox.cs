using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class NewsletterBoxViewComponent : TvProgViewComponent
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
