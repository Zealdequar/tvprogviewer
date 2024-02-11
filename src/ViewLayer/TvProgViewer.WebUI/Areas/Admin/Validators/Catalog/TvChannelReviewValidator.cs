using FluentValidation;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Mapping;
using TvProgViewer.Services.Localization;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    public partial class TvChannelReviewValidator : BaseTvProgValidator<TvChannelReviewModel>
    {
        public TvChannelReviewValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor, IWorkContext workContext)
        {
            var isLoggedInAsVendor = workContext.GetCurrentVendorAsync().Result != null;
            //vendor can edit "Reply text" only
            if (!isLoggedInAsVendor)
            {
                RuleFor(x => x.Title).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.TvChannelReviews.Fields.Title.Required"));
                RuleFor(x => x.ReviewText).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Catalog.TvChannelReviews.Fields.ReviewText.Required"));
            }

            SetDatabaseValidationRules<TvChannelReview>(mappingEntityAccessor);
        }
    }
}