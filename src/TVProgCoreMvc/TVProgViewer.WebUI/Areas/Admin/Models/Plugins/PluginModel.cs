using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Plugins
{
    /// <summary>
    /// Represents a plugin model
    /// </summary>
    public partial record PluginModel : BaseTvProgModel, IAclSupportedModel, ILocalizedModel<PluginLocalizedModel>, IPluginModel, IStoreMappingSupportedModel
    {
        #region Ctor

        public PluginModel()
        {
            Locales = new List<PluginLocalizedModel>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            SelectedUserRoleIds = new List<int>();
            AvailableUserRoles = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.Plugins.Fields.Group")]
        public string Group { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Plugins.Fields.FriendlyName")]
        public string FriendlyName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Plugins.Fields.SystemName")]
        public string SystemName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Plugins.Fields.Version")]
        public string Version { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Plugins.Fields.Author")]
        public string Author { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Plugins.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Plugins.Fields.Configure")]
        public string ConfigurationUrl { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Plugins.Fields.Installed")]
        public bool Installed { get; set; }
        
        public string Description { get; set; }

        public bool CanChangeEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Plugins.Fields.IsEnabled")]
        public bool IsEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Plugins.Fields.Logo")]
        public string LogoUrl { get; set; }

        public IList<PluginLocalizedModel> Locales { get; set; }

        //ACL (user roles)
        [TvProgResourceDisplayName("Admin.Configuration.Plugins.Fields.AclUserRoles")]
        public IList<int> SelectedUserRoleIds { get; set; }
        public IList<SelectListItem> AvailableUserRoles { get; set; }

        //store mapping
        [TvProgResourceDisplayName("Admin.Configuration.Plugins.Fields.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public bool IsActive { get; set; }

        #endregion
    }

    public partial record PluginLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Plugins.Fields.FriendlyName")]
        public string FriendlyName { get; set; }
    }
}