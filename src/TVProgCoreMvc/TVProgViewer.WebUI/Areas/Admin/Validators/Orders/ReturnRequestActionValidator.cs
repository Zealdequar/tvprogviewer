using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Orders;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Data;
using TVProgViewer.Core.Domain.Orders;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Orders
{
    public partial class ReturnRequestActionValidator : BaseTvProgValidator<ReturnRequestActionModel>
    {
        public ReturnRequestActionValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Order.ReturnRequestActions.Name.Required"));

            SetDatabaseValidationRules<ReturnRequestAction>(dataProvider);
        }
    }
}