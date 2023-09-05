using Microsoft.AspNetCore.Mvc;

namespace TvProgViewer.Plugin.Payments.CyberSource.Controllers
{
    public class CyberSourceWebhookController : Controller
    {
        #region Methods

        public IActionResult PayerRedirect()
        {
            return Ok();
        }

        #endregion
    }
}