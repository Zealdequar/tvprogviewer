using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Catalog
{
    public partial class ProductReviewValidator : BaseTvProgValidator<ProductReviewModel>
    {
        public ProductReviewValidator(IDataProvider dataProvider, ILocalizationService localizationService, IWorkContext workContext)
        {
            var isLoggedInAsVendor = workContext.CurrentVendor != null;
            //vendor can edit "Reply text" only
            if (!isLoggedInAsVendor)
            {
                RuleFor(x => x.Title).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.ProductReviews.Fields.Title.Required"));
                RuleFor(x => x.ReviewText).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.ProductReviews.Fields.ReviewText.Required"));
            }

            SetDatabaseValidationRules<ProductReview>(dataProvider);
        }
    }
}