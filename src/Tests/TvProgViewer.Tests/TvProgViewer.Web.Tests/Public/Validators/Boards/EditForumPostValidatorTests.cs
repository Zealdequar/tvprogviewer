using FluentValidation.TestHelper;
using TvProgViewer.WebUI.Models.Boards;
using TvProgViewer.WebUI.Validators.Boards;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Validators.Boards
{
    [TestFixture]
    public class EditForumPostValidatorTests : BaseTvProgTest
    {
        private EditForumPostValidator _validator;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<EditForumPostValidator>();
        }
        
        [Test]
        public void ShouldHaveErrorWhenTextIsNullOrEmpty()
        {
            var model = new EditForumPostModel
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
            var model = new EditForumPostModel
            {
                Text = "some comment"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Text);
        }
    }
}
