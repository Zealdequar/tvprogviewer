using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Plugin.Payments.PayPalViewer.Services;

namespace TvProgViewer.Plugin.Payments.PayPalViewer.Controllers
{
    public class PayPalViewerWebhookController : Controller
    {
        #region Fields

        private readonly PayPalViewerSettings _settings;
        private readonly ServiceManager _serviceManager;

        #endregion

        #region Ctor

        public PayPalViewerWebhookController(PayPalViewerSettings settings,
            ServiceManager serviceManager)
        {
            _settings = settings;
            _serviceManager = serviceManager;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> WebhookHandler()
        {
            await _serviceManager.HandleWebhookAsync(_settings, Request);
            return Ok();
        }

        #endregion
    }
}