using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Users;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Users
{
    public partial class UserAttributeValidator : BaseTvProgValidator<UserAttributeModel>
    {
        public UserAttributeValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Users.UserAttributes.Fields.Name.Required"));

            SetDatabaseValidationRules<UserAttribute>(dataProvider);
        }
    }
}