using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Components;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Components
{
    public class TaxTypeSelectorViewComponent : TvProgViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly TaxSettings _taxSettings;

        public TaxTypeSelectorViewComponent(ICommonModelFactory commonModelFactory,
            TaxSettings taxSettings)
        {
            _commonModelFactory = commonModelFactory;
            _taxSettings = taxSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_taxSettings.AllowUsersToSelectTaxDisplayType)
                return Content("");

            var model = await _commonModelFactory.PrepareTaxTypeSelectorModelAsync();
            return View(model);
        }
    }
}
