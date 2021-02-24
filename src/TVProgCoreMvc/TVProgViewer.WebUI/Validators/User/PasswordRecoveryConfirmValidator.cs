using FluentValidation;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.WebUI.Models.User;

namespace TVProgViewer.WebUI.Validators.User
{
    public partial class PasswordRecoveryConfirmValidator : BaseTvProgValidator<PasswordRecoveryConfirmModel>
    {
        public PasswordRecoveryConfirmValidator(ILocalizationService localizationService, UserSettings userSettings)
        {
            RuleFor(x => x.NewPassword).IsPassword(localizationService, userSettings);
            RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.PasswordRecovery.ConfirmNewPassword.Required"));
            RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessageAwait(localizationService.GetResourceAsync("Account.PasswordRecovery.NewPassword.EnteredPasswordsDoNotMatch"));
        }
    }
}