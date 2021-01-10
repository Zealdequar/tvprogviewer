using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Directory;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Directory
{
    public partial class MeasureWeightValidator : BaseTvProgValidator<MeasureWeightModel>
    {
        public MeasureWeightValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Shipping.Measures.Weights.Fields.Name.Required"));
            RuleFor(x => x.SystemKeyword).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Shipping.Measures.Weights.Fields.SystemKeyword.Required"));

            SetDatabaseValidationRules<MeasureWeight>(dataProvider);
        }
    }
}