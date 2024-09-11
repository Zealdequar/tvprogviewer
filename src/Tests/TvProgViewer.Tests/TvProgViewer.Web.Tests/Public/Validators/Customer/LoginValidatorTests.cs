using FluentValidation.TestHelper;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Localization;
using TvProgViewer.WebUI.Models.User;
using TvProgViewer.WebUI.Validators.User;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Validators.User
{
    [TestFixture]
    public class LoginValidatorTests : BaseTvProgTest
    {
        private LoginValidator _validator;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<LoginValidator>();
        }
        
        [Test]
        public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
        {
            var model = new LoginModel
            {
                Email = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
            model.Email = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldHaveErrorWhenEmailIsWrongFormat()
        {
            var model = new LoginModel
            {
                Email = "adminexample.com"
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
        {
            var model = new LoginModel
            {
                Email = "admin@example.com"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsNullButUsernamesAreEnabled()
        {
            var userSettings = new UserSettings
            {
                UsernamesEnabled = true
            };
            _validator = new LoginValidator(GetService<ILocalizationService>(), userSettings);

            var model = new LoginModel
            {
                Email = null
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }
}
