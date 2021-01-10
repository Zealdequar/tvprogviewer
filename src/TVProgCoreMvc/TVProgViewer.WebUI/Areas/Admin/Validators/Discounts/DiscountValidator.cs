using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Discounts;
using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Discounts
{
    public partial class DiscountValidator : BaseTvProgValidator<DiscountModel>
    {
        public DiscountValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.Discounts.Fields.Name.Required"));

            SetDatabaseValidationRules<Discount>(dataProvider);
        }
    }
}