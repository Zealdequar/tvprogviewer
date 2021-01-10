using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Messages;
using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Messages
{
    public partial class MessageTemplateValidator : BaseTvProgValidator<MessageTemplateModel>
    {
        public MessageTemplateValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Subject).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Fields.Subject.Required"));
            RuleFor(x => x.Body).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Fields.Body.Required"));

            SetDatabaseValidationRules<MessageTemplate>(dataProvider);
        }
    }
}