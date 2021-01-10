using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Orders;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Orders
{
    public partial class CheckoutAttributeValueValidator : BaseTvProgValidator<CheckoutAttributeValueModel>
    {
        public CheckoutAttributeValueValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Attributes.CheckoutAttributes.Values.Fields.Name.Required"));

            SetDatabaseValidationRules<CheckoutAttributeValue>(dataProvider);;
        }
    }
}