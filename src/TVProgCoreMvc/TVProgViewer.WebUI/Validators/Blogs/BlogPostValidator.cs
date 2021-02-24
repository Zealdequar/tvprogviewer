using FluentValidation;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.WebUI.Models.Blogs;

namespace TVProgViewer.WebUI.Validators.Blogs
{
    public partial class BlogPostValidator : BaseTvProgValidator<BlogPostModel>
    {
        public BlogPostValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddNewComment.CommentText).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Blog.Comments.CommentText.Required")).When(x => x.AddNewComment != null);
        }
    }
}