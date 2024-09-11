using FluentValidation.TestHelper;
using TvProgViewer.WebUI.Models.Blogs;
using TvProgViewer.WebUI.Validators.Blogs;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Validators.Blogs
{
    [TestFixture]
    public class BlogPostValidatorTests : BaseTvProgTest
    {
        private BlogPostValidator _validator;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<BlogPostValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenCommentIsNullOrEmpty()
        {
            var model = new BlogPostModel { AddNewComment = { CommentText = null } };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddNewComment.CommentText);
            model.AddNewComment.CommentText = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.AddNewComment.CommentText);
        }

        [Test]
        public void ShouldNotHaveErrorWhenCommentIsSpecified()
        {
            var model = new BlogPostModel { AddNewComment = { CommentText = "some comment" } };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.AddNewComment.CommentText);
        }
    }
}
