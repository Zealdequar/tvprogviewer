using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Users;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Users
{
    public partial class UserRoleValidator : BaseTvProgValidator<UserRoleModel>
    {
        public UserRoleValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Users.UserRoles.Fields.Name.Required"));

            SetDatabaseValidationRules<UserRole>(dataProvider);
        }
    }
}