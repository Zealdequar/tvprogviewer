using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Common;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Common
{
    public partial class AddressAttributeValidator : BaseTvProgValidator<AddressAttributeModel>
    {
        public AddressAttributeValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Address.AddressAttributes.Fields.Name.Required"));

            SetDatabaseValidationRules<AddressAttribute>(dataProvider);
        }
    }
}