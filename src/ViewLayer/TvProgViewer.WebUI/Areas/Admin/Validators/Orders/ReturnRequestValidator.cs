using FluentValidation;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Data.Mapping;
using TvProgViewer.Services.Localization;
using TvProgViewer.WebUI.Areas.Admin.Models.Orders;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.WebUI.Areas.Admin.Validators.Orders
{
    public partial class ReturnRequestValidator : BaseTvProgValidator<ReturnRequestModel>
    {
        public ReturnRequestValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.ReasonForReturn).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.ReasonForReturn.Required"));
            RuleFor(x => x.RequestedAction).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.RequestedAction.Required"));
            RuleFor(x => x.Quantity).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.Quantity.Required"));
            RuleFor(x => x.Quantity).Must((model, value) => value >= model.ReturnedQuantity).WithMessage(model => string.Format(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.Quantity.MustBeEqualOrGreaterThanReturnedQuantityField").Result, model.ReturnedQuantity));
            RuleFor(x => x.ReturnedQuantity).GreaterThan(-1).WithMessageAwait(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.ReturnedQuantity.MustBeEqualOrGreaterThanZero"));
            RuleFor(x => x.ReturnedQuantity).Must((model, value) => value <= model.Quantity).WithMessage(model => string.Format(localizationService.GetResourceAsync("Admin.ReturnRequests.Fields.ReturnedQuantity.MustBeLessOrEqualQuantityField").Result, model.Quantity));

            SetDatabaseValidationRules<ReturnRequest>(mappingEntityAccessor);
        }
    }
}