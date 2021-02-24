using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Tax;
using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Tax
{
    public partial class TaxCategoryValidator : BaseTvProgValidator<TaxCategoryModel>
    {
        public TaxCategoryValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Tax.Categories.Fields.Name.Required"));

            SetDatabaseValidationRules<TaxCategory>(dataProvider);
        }
    }
}