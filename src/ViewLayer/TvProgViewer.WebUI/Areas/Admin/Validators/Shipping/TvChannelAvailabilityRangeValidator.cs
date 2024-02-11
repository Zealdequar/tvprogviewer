using FluentValidation;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Data.Mapping;
using TvProgViewer.Services.Localization;
using TvProgViewer.WebUI.Areas.Admin.Models.Shipping;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.WebUI.Areas.Admin.Validators.Shipping
{
    public partial class TvChannelAvailabilityRangeValidator : BaseTvProgValidator<TvChannelAvailabilityRangeModel>
    {
        public TvChannelAvailabilityRangeValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Shipping.TvChannelAvailabilityRanges.Fields.Name.Required"));

            SetDatabaseValidationRules<TvChannelAvailabilityRange>(mappingEntityAccessor);
        }
    }
}