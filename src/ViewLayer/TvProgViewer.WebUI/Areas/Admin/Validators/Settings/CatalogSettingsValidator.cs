using FluentValidation;
using TvProgViewer.Services.Localization;
using TvProgViewer.WebUI.Areas.Admin.Models.Settings;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.WebUI.Areas.Admin.Validators.Settings
{
    public partial class CatalogSettingsValidator : BaseTvProgValidator<CatalogSettingsModel>
    {
        public CatalogSettingsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SearchPagePriceFrom)
                .GreaterThanOrEqualTo(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Catalog.SearchPagePriceFrom.GreaterThanOrEqualZero"))
                .When(x => x.SearchPagePriceRangeFiltering && x.SearchPageManuallyPriceRange);

            RuleFor(x => x.SearchPagePriceTo)
                .GreaterThan(x => x.SearchPagePriceFrom > decimal.Zero ? x.SearchPagePriceFrom : decimal.Zero)
                .WithMessage(x => string.Format(localizationService.GetResourceAsync("Admin.Configuration.Settings.Catalog.SearchPagePriceTo.GreaterThanZeroOrPriceFrom").Result, x.SearchPagePriceFrom > decimal.Zero ? x.SearchPagePriceFrom : decimal.Zero))
                .When(x => x.SearchPagePriceRangeFiltering && x.SearchPageManuallyPriceRange);

            RuleFor(x => x.TvChannelsByTagPriceFrom)
                .GreaterThanOrEqualTo(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Settings.Catalog.TvChannelsByTagPriceFrom.GreaterThanOrEqualZero"))
                .When(x => x.TvChannelsByTagPriceRangeFiltering && x.TvChannelsByTagManuallyPriceRange);

            RuleFor(x => x.TvChannelsByTagPriceTo)
                .GreaterThan(x => x.TvChannelsByTagPriceFrom > decimal.Zero ? x.TvChannelsByTagPriceFrom : decimal.Zero)
                .WithMessage(x => string.Format(localizationService.GetResourceAsync("Admin.Configuration.Settings.Catalog.TvChannelsByTagPriceTo.GreaterThanZeroOrPriceFrom").Result, x.TvChannelsByTagPriceFrom > decimal.Zero ? x.TvChannelsByTagPriceFrom : decimal.Zero))
                .When(x => x.TvChannelsByTagPriceRangeFiltering && x.TvChannelsByTagManuallyPriceRange);
        }
    }
}