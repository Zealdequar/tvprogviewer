using FluentValidation;
using TVProgViewer.WebUI.Areas.Admin.Models.Plugins;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;

namespace TVProgViewer.WebUI.Areas.Admin.Validators.Plugins
{
    public partial class PluginValidator : BaseTvProgValidator<PluginModel>
    {
        public PluginValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FriendlyName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Admin.Configuration.Plugins.Fields.FriendlyName.Required"));
        }
    }
}