using FluentValidation;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.WebUI.Models.User;

namespace TVProgViewer.WebUI.Validators.User
{
    public partial class LoginValidator : BaseTvProgValidator<LoginModel>
    {
        public LoginValidator(ILocalizationService localizationService, UserSettings userSettings)
        {
            if (!userSettings.UsernamesEnabled)
            {
                //login by email
                RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.Login.Fields.Email.Required"));
                RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            }
        }
    }
}