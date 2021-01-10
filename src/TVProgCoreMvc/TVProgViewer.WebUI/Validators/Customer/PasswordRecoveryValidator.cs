using FluentValidation;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.WebUI.Models.User;

namespace TVProgViewer.WebUI.Validators.User
{
    public partial class PasswordRecoveryValidator : BaseTvProgValidator<PasswordRecoveryModel>
    {
        public PasswordRecoveryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.PasswordRecovery.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }
    }
}