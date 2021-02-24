using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Services.Seo;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    public partial class ProductValidator : BaseTvProgValidator<ProductModel>
    {
        public ProductValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.Name.Required"));

            RuleFor(x => x.SeName)
                .Length(0, TvProgSeoDefaults.SearchEngineNameLength)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), TvProgSeoDefaults.SearchEngineNameLength);

            RuleFor(x => x.RentalPriceLength)
                .GreaterThan(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.RentalPriceLength.ShouldBeGreaterThanZero"))
                .When(x => x.IsRental);

            SetDatabaseValidationRules<Product>(dataProvider);
        }
    }
}