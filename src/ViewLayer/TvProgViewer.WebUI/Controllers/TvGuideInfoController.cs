using Microsoft.AspNetCore.Mvc;
using Nito.AsyncEx.Synchronous;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TvProgViewer.Services.Common;


namespace TvProgViewer.WebUI.Controllers
{
    public partial class TvGuideInfoController : BasePublicController
    {
        #region Поля

        private readonly StoreHttpClient _storeHttpClient;

        #endregion

        #region Конструктор

        public TvGuideInfoController(StoreHttpClient storeHttpClient)
        {
           _storeHttpClient = storeHttpClient;
        }

        #endregion

        #region Методы

        public virtual IActionResult Update()
        {
            Task.Run(async() => await _storeHttpClient.UpdateTvProgrammes());
            return View();
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> RunUpdateAsync(string uuid)
        {
            await _storeHttpClient.UpdateTvProgrammes();
            return Json("{\"result\": \"OK\", \"guid\": \"" + uuid + "\"}");
        }

        #endregion
    }
}
