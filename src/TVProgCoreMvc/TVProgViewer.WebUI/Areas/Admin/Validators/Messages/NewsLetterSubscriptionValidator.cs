using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Messages;
using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Messages
{
    public partial class NewsLetterSubscriptionValidator : BaseTvProgValidator<NewsletterSubscriptionModel>
    {
        public NewsLetterSubscriptionValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Admin.Promotions.NewsLetterSubscriptions.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));

            SetDatabaseValidationRules<NewsLetterSubscription>(dataProvider);
        }
    }
}