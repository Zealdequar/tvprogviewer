using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data;
using TVProgViewer.Services.Defaults;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    public partial class ProductValidator : BaseTvProgValidator<ProductModel>
    {
        public ProductValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Name.Required"));
            RuleFor(x => x.SeName).Length(0, TvProgSeoDefaults.SearchEngineNameLength)
                .WithMessage(string.Format(localizationService.GetResource("Admin.SEO.SeName.MaxLengthValidation"), TvProgSeoDefaults.SearchEngineNameLength));

            SetDatabaseValidationRules<Product>(dataProvider);
        }
    }
}