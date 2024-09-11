using FluentValidation.TestHelper;
using TvProgViewer.WebUI.Models.User;
using TvProgViewer.WebUI.Validators.User;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Validators.User
{
    [TestFixture]
    public class PasswordRecoveryValidatorTests : BaseTvProgTest
    {
        private PasswordRecoveryValidator _validator;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<PasswordRecoveryValidator>();
        }
        
        [Test]
        public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
        {
            var model = new PasswordRecoveryModel
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
            var model = new PasswordRecoveryModel
            {
                Email = "adminexample.com"
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
        {
            var model = new PasswordRecoveryModel
            {
                Email = "admin@example.com"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }
}
