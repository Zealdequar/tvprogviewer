using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Blogs;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Data;
using TVProgViewer.Services.Defaults;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Blogs
{
    public partial class BlogPostValidator : BaseTvProgValidator<BlogPostModel>
    {
        public BlogPostValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Fields.Title.Required"));

            RuleFor(x => x.Body)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Fields.Body.Required"));

            //blog tags should not contain dots
            //current implementation does not support it because it can be handled as file extension
            RuleFor(x => x.Tags)
                .Must(x => x == null || !x.Contains("."))
                .WithMessage(localizationService.GetResource("Admin.ContentManagement.Blog.BlogPosts.Fields.Tags.NoDots"));

            RuleFor(x => x.SeName).Length(0, TvProgSeoDefaults.SearchEngineNameLength)
                .WithMessage(string.Format(localizationService.GetResource("Admin.SEO.SeName.MaxLengthValidation"), TvProgSeoDefaults.SearchEngineNameLength));

            SetDatabaseValidationRules<BlogPost>(dataProvider);
        }
    }
}