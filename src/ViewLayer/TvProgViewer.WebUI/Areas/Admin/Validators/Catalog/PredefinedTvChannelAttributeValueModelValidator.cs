using FluentValidation;
using TvProgViewer.Services.Localization;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    public partial class PredefinedTvChannelAttributeValueModelValidator : BaseTvProgValidator<PredefinedTvChannelAttributeValueModel>
    {
        public PredefinedTvChannelAttributeValueModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.Attributes.TvChannelAttributes.PredefinedValues.Fields.Name.Required"));
        }
    }
}