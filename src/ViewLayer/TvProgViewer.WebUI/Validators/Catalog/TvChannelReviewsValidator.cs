using FluentValidation;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Validators;
using TvProgViewer.WebUI.Models.Catalog;

namespace TvProgViewer.WebUI.Validators.Catalog
{
    public partial class TvChannelReviewsValidator : BaseTvProgValidator<TvChannelReviewsModel>
    {
        public TvChannelReviewsValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddTvChannelReview.Title).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Reviews.Fields.Title.Required")).When(x => x.AddTvChannelReview != null);
            RuleFor(x => x.AddTvChannelReview.Title).Length(1, 200).WithMessageAwait(localizationService.GetResourceAsync("Reviews.Fields.Title.MaxLengthValidation"), 200).When(x => x.AddTvChannelReview != null && !string.IsNullOrEmpty(x.AddTvChannelReview.Title));
            RuleFor(x => x.AddTvChannelReview.ReviewText).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Reviews.Fields.ReviewText.Required")).When(x => x.AddTvChannelReview != null);
        }
    }
}