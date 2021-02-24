using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Blogs;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Services.Seo;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Blogs
{
    public partial class BlogPostValidator : BaseTvProgValidator<BlogPostModel>
    {
        public BlogPostValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Fields.Title.Required"));

            RuleFor(x => x.Body)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Fields.Body.Required"));

            //blog tags should not contain dots
            //current implementation does not support it because it can be handled as file extension
            RuleFor(x => x.Tags)
                .Must(x => x == null || !x.Contains("."))
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Fields.Tags.NoDots"));

            RuleFor(x => x.SeName).Length(0, TvProgSeoDefaults.SearchEngineNameLength)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), TvProgSeoDefaults.SearchEngineNameLength);

            SetDatabaseValidationRules<BlogPost>(dataProvider);
        }
    }
}