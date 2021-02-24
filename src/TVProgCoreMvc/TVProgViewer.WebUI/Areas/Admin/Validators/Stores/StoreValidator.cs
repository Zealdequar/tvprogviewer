using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Stores;
using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Stores
{
    public partial class StoreValidator : BaseTvProgValidator<StoreModel>
    {
        public StoreValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Stores.Fields.Name.Required"));
            RuleFor(x => x.Url).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Stores.Fields.Url.Required"));

            SetDatabaseValidationRules<Store>(dataProvider);
        }
    }
}