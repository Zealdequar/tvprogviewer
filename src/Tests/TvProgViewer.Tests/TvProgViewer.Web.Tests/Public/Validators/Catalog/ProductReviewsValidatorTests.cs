using FluentValidation.TestHelper;
using TvProgViewer.WebUI.Models.Catalog;
using TvProgViewer.WebUI.Validators.Catalog;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Validators.Catalog
{
    [TestFixture]
    public class TvChannelReviewsValidatorTests : BaseTvProgTest
    {
        private TvChannelReviewsValidator _validator;

        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<TvChannelReviewsValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenTitleIsNullOrEmpty()
        {
            var model = new TvChannelReviewsModel { AddTvChannelReview = { Title = null } };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddTvChannelReview.Title);
            model.AddTvChannelReview.Title = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddTvChannelReview.Title);
        }

        [Test]
        public void ShouldNotHaveErrorWhenTitleIsSpecified()
        {
            var model = new TvChannelReviewsModel { AddTvChannelReview = { Title = "some comment" } };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AddTvChannelReview.Title);
        }

        [Test]
        public void ShouldHaveErrorWhenReviewTextIsNullOrEmpty()
        {
            var model = new TvChannelReviewsModel { AddTvChannelReview = { ReviewText = null } };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddTvChannelReview.ReviewText);
            model.AddTvChannelReview.ReviewText = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddTvChannelReview.ReviewText);
        }

        [Test]
        public void ShouldNotHaveErrorWhenReviewTextIsSpecified()
        {
            var model = new TvChannelReviewsModel { AddTvChannelReview = { ReviewText = "some comment" } };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AddTvChannelReview.ReviewText);
        }
    }
}
