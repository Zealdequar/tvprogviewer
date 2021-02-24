using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core.Domain;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class StoreThemeSelectorViewComponent : TvProgViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly StoreInformationSettings _storeInformationSettings;

        public StoreThemeSelectorViewComponent(ICommonModelFactory commonModelFactory,
            StoreInformationSettings storeInformationSettings)
        {
            _commonModelFactory = commonModelFactory;
            _storeInformationSettings = storeInformationSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_storeInformationSettings.AllowUserToSelectTheme)
                return Content("");

            var model = await _commonModelFactory.PrepareStoreThemeSelectorModelAsync();
            return View(model);
        }
    }
}
