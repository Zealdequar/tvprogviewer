using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Users;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Users
{
    public partial class UserAttributeValueValidator : BaseTvProgValidator<UserAttributeValueModel>
    {
        public UserAttributeValueValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Users.UserAttributes.Values.Fields.Name.Required"));

            SetDatabaseValidationRules<UserAttributeValue>(dataProvider);
        }
    }
}