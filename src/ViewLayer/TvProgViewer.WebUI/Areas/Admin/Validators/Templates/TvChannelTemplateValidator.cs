using FluentValidation;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Data.Mapping;
using TvProgViewer.Services.Localization;
using TvProgViewer.WebUI.Areas.Admin.Models.Templates;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.WebUI.Areas.Admin.Validators.Templates
{
    public partial class TvChannelTemplateValidator : BaseTvProgValidator<TvChannelTemplateModel>
    {
        public TvChannelTemplateValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.TvChannel.Name.Required"));
            RuleFor(x => x.ViewPath).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.System.Templates.TvChannel.ViewPath.Required"));

            SetDatabaseValidationRules<TvChannelTemplate>(mappingEntityAccessor);
        }
    }
}