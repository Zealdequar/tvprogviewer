using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    public partial class ProductTagValidator : BaseTvProgValidator<ProductTagModel>
    {
        public ProductTagValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.ProductTags.Fields.Name.Required"));

            SetDatabaseValidationRules<ProductTag>(dataProvider);
        }
    }
}