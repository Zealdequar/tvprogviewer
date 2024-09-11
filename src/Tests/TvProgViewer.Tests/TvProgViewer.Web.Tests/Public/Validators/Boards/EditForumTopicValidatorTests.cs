using FluentValidation.TestHelper;
using TvProgViewer.WebUI.Models.Boards;
using TvProgViewer.WebUI.Validators.Boards;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Validators.Boards
{
    [TestFixture]
    public class EditForumTopicValidatorTests : BaseTvProgTest
    {
        private EditForumTopicValidator _validator;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<EditForumTopicValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenSubjectIsNullOrEmpty()
        {
            var model = new EditForumTopicModel
            {
                Subject = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
            model.Subject = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
        }

        [Test]
        public void ShouldNotHaveErrorWhenSubjectIsSpecified()
        {
            var model = new EditForumTopicModel
            {
                Subject = "some comment"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Subject);
        }

        [Test]
        public void ShouldHaveErrorWhenTextIsNullOrEmpty()
        {
            var model = new EditForumTopicModel
            {
                Text = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Text);
            model.Text = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Text);
        }

        [Test]
        public void ShouldNotHaveErrorWhenTextIsSpecified()
        {
            var model = new EditForumTopicModel
            {
                Text = "some comment"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Text);
        }
    }
}
