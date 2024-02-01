using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.TvProgMain;

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

        public async Task<IActionResult> Update()
        {
            await _storeHttpClient.UpdateTvProgrammes();
            return View();
        }

        #endregion
    }
}
