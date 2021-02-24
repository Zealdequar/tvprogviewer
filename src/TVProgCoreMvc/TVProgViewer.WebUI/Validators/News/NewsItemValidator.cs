using FluentValidation;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.WebUI.Models.News;

namespace TVProgViewer.WebUI.Validators.News
{
    public partial class NewsItemValidator : BaseTvProgValidator<NewsItemModel>
    {
        public NewsItemValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.AddNewComment.CommentTitle).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("News.Comments.CommentTitle.Required")).When(x => x.AddNewComment != null);
            RuleFor(x => x.AddNewComment.CommentTitle).Length(1, 200).WithMessageAwait(localizationService.GetResourceAsync("News.Comments.CommentTitle.MaxLengthValidation"), 200).When(x => x.AddNewComment != null && !string.IsNullOrEmpty(x.AddNewComment.CommentTitle));
            RuleFor(x => x.AddNewComment.CommentText).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("News.Comments.CommentText.Required")).When(x => x.AddNewComment != null);
        }
    }
}