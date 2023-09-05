using FluentValidation;
using TvProgViewer.Plugin.DiscountRules.UserRoles.Models;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.Plugin.DiscountRules.UserRoles.Validators
{
    /// <summary>
    /// Represents an <see cref="RequirementModel"/> validator.
    /// </summary>
    public class RequirementModelValidator : BaseTvProgValidator<RequirementModel>
    {
        public RequirementModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.DiscountId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.UserRoles.Fields.DiscountId.Required"));
            RuleFor(model => model.UserRoleId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.UserRoles.Fields.UserRoleId.Required"));
        }
    }
}
