using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Messages;
using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Messages
{
    public partial class EmailAccountValidator : BaseTvProgValidator<EmailAccountModel>
    {
        public EmailAccountValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Email).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Admin.Common.WrongEmail"));

            RuleFor(x => x.DisplayName).NotEmpty();

            SetDatabaseValidationRules<EmailAccount>(dataProvider);
        }
    }
}