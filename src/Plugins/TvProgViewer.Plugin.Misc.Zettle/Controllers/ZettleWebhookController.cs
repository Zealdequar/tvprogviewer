using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Plugin.Misc.Zettle.Services;

namespace TvProgViewer.Plugin.Misc.Zettle.Controllers
{
    public class ZettleWebhookController : Controller
    {
        #region Fields

        private readonly ZettleService _zettleService;

        #endregion

        #region Ctor

        public ZettleWebhookController(ZettleService zettleService)
        {
            _zettleService = zettleService;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> Webhook()
        {
            await _zettleService.HandleWebhookAsync(Request);
            return Ok();
        }

        #endregion
    }
}