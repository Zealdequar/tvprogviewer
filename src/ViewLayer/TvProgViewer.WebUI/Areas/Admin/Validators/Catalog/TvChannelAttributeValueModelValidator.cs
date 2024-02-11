using FluentValidation;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Mapping;
using TvProgViewer.Services.Localization;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    public partial class TvChannelAttributeValueModelValidator : BaseTvProgValidator<TvChannelAttributeValueModel>
    {
        public TvChannelAttributeValueModelValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.Name.Required"));

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(1)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.Quantity.GreaterThanOrEqualTo1"))
                .When(x => x.AttributeValueTypeId == (int)AttributeValueType.AssociatedToTvChannel && !x.UserEntersQty);

            RuleFor(x => x.AssociatedTvChannelId)
                .GreaterThanOrEqualTo(1)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.AssociatedTvChannel.Choose"))
                .When(x => x.AttributeValueTypeId == (int)AttributeValueType.AssociatedToTvChannel);

            SetDatabaseValidationRules<TvChannelAttributeValue>(mappingEntityAccessor);
        }
    }
}