using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.News;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.Services.Seo;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.News
{
    public partial class NewsItemValidator : BaseTvProgValidator<NewsItemModel>
    {
        public NewsItemValidator(ILocalizationService localizationService, ITvProgDataProvider dataProvider)
        {
            RuleFor(x => x.Title).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Fields.Title.Required"));

            RuleFor(x => x.Short).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Fields.Short.Required"));

            RuleFor(x => x.Full).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.ContentManagement.News.NewsItems.Fields.Full.Required"));

            RuleFor(x => x.SeName).Length(0, TvProgSeoDefaults.SearchEngineNameLength)
                .WithMessageAwait(localizationService.GetResourceAsync("Admin.SEO.SeName.MaxLengthValidation"), TvProgSeoDefaults.SearchEngineNameLength);

            SetDatabaseValidationRules<NewsItem>(dataProvider);
        }
    }
}