using FluentValidation;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    /// <summary>
    /// Represent a review type validator
    /// </summary>
    public partial class ReviewTypeValidator : BaseTvProgValidator<ReviewTypeModel>
    {
        public ReviewTypeValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Settings.ReviewType.Fields.Name.Required"));
            RuleFor(x => x.Description).NotEmpty().WithMessage(localizationService.GetResource("Admin.Settings.ReviewType.Fields.Description.Required"));

            SetDatabaseValidationRules<ReviewType>(dataProvider);
        }
    }
}
