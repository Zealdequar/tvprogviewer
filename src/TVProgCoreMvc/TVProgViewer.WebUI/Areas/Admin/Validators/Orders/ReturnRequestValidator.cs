using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Orders;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Data;
using TVProgViewer.Core.Domain.Orders;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Orders
{
    public partial class ReturnRequestValidator : BaseTvProgValidator<ReturnRequestModel>
    {
        public ReturnRequestValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.ReasonForReturn).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.ReasonForReturn.Required"));
            RuleFor(x => x.RequestedAction).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.RequestedAction.Required"));

            SetDatabaseValidationRules<ReturnRequest>(dataProvider);
        }
    }
}