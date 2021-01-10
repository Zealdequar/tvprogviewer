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
        public TaxCategoryValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Tax.Categories.Fields.Name.Required"));

            SetDatabaseValidationRules<TaxCategory>(dataProvider);
        }
    }
}