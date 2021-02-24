using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Messages;
using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Messages
{
    public partial class QueuedEmailValidator : BaseTvProgValidator<QueuedEmailModel>
    {
        public QueuedEmailValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.From).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.From.Required"));
            RuleFor(x => x.To).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.To.Required"));

            RuleFor(x => x.SentTries).NotNull().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.SentTries.Required"))
                                    .InclusiveBetween(0, 99999).WithMessageAwait(localizationService.GetResourceAsync("Admin.System.QueuedEmails.Fields.SentTries.Range"));

            SetDatabaseValidationRules<QueuedEmail>(dataProvider);

        }
    }
}