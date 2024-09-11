using FluentValidation.TestHelper;
using TvProgViewer.WebUI.Models.Catalog;
using TvProgViewer.WebUI.Validators.Catalog;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Validators.Catalog
{
    [TestFixture]
    public class TvChannelEmailAFriendValidatorTests : BaseTvProgTest
    {
        private TvChannelEmailAFriendValidator _validator;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<TvChannelEmailAFriendValidator>();
        }
        
        [Test]
        public void ShouldHaveErrorWhenFriendEmailIsNullOrEmpty()
        {
            var model = new TvChannelEmailAFriendModel
            {
                FriendEmail = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
            model.FriendEmail = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
        }

        [Test]
        public void ShouldHaveErrorWhenFriendEmailIsWrongFormat()
        {
            var model = new TvChannelEmailAFriendModel
            {
                FriendEmail = "adminexample.com"
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FriendEmail);
        }

        [Test]
        public void PublicVoidShouldNotHaveErrorWhenFriendEmailIsCorrectFormat()
        {
            var model = new TvChannelEmailAFriendModel
            {
                FriendEmail = "admin@example.com"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FriendEmail);
        }

        [Test]
        public void ShouldHaveErrorWhenYourEmailAddressIsNullOrEmpty()
        {
            var model = new TvChannelEmailAFriendModel
            {
                YourEmailAddress = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.YourEmailAddress);
            model.YourEmailAddress = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.YourEmailAddress);
        }

        [Test]
        public void ShouldHaveErrorWhenYourEmailAddressIsWrongFormat()
        {
            var model = new TvChannelEmailAFriendModel
            {
                YourEmailAddress = "adminexample.com"
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.YourEmailAddress);
        }

        [Test]
        public void ShouldNotHaveErrorWhenYourEmailAddressIsCorrectFormat()
        {
            var model = new TvChannelEmailAFriendModel
            {
                YourEmailAddress = "admin@example.com"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.YourEmailAddress);
        }
    }
}
