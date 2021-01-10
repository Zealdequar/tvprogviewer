using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Shipping;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Shipping
{
    public partial class ProductAvailabilityRangeValidator : BaseTvProgValidator<ProductAvailabilityRangeModel>
    {
        public ProductAvailabilityRangeValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Shipping.ProductAvailabilityRanges.Fields.Name.Required"));

            SetDatabaseValidationRules<ProductAvailabilityRange>(dataProvider);
        }
    }
}