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
        public StoreValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Name.Required"));
            RuleFor(x => x.Url).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Url.Required"));

            SetDatabaseValidationRules<Store>(dataProvider);
        }
    }
}