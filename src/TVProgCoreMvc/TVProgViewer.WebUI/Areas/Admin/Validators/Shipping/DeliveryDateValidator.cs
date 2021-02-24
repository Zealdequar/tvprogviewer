using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Shipping;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Shipping
{
    public partial class DeliveryDateValidator : BaseTvProgValidator<DeliveryDateModel>
    {
        public DeliveryDateValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.DeliveryDates.Fields.Name.Required"));

            SetDatabaseValidationRules<DeliveryDate>(dataProvider);
        }
    }
}