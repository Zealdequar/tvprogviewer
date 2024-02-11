using FluentValidation;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Mapping;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Seo;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    public partial class TvChannelValidator : BaseTvProgValidator<TvChannelModel>
    {
        public TvChannelValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Fields.Name.Required"));
            
            RuleFor(x => x.SeName)
                .Length(0, TvProgSeoDefaults.SearchEngineNameLength)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), TvProgSeoDefaults.SearchEngineNameLength);
            
            RuleFor(x => x.RentalPriceLength)
                .GreaterThan(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Fields.RentalPriceLength.ShouldBeGreaterThanZero"))
                .When(x => x.IsRental);

            SetDatabaseValidationRules<TvChannel>(mappingEntityAccessor);
        }
    }
}