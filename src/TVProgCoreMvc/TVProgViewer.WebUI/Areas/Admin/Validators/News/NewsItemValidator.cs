using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.News;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Data;
using TVProgViewer.Services.Defaults;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.News
{
    public partial class NewsItemValidator : BaseTvProgValidator<NewsItemModel>
    {
        public NewsItemValidator(IDataProvider dataProvider, ILocalizationService localizationService)
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Fields.Title.Required"));

            RuleFor(x => x.Short).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Fields.Short.Required"));

            RuleFor(x => x.Full).NotEmpty().WithMessage(localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Fields.Full.Required"));

            RuleFor(x => x.SeName).Length(0, TvProgSeoDefaults.SearchEngineNameLength)
                .WithMessage(string.Format(localizationService.GetResource("Admin.SEO.SeName.MaxLengthValidation"), TvProgSeoDefaults.SearchEngineNameLength));

            SetDatabaseValidationRules<NewsItem>(dataProvider);
        }
    }
}