using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a Sitemap settings model
    /// </summary>
    public partial record SitemapSettingsModel : BaseTvProgModel, ISettingsModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapEnabled")]
        public bool SitemapEnabled { get; set; }
        public bool SitemapEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeBlogPosts")]
        public bool SitemapIncludeBlogPosts { get; set; }
        public bool SitemapIncludeBlogPosts_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeCategories")]
        public bool SitemapIncludeCategories { get; set; }
        public bool SitemapIncludeCategories_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeManufacturers")]
        public bool SitemapIncludeManufacturers { get; set; }
        public bool SitemapIncludeManufacturers_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeNews")]
        public bool SitemapIncludeNews { get; set; }
        public bool SitemapIncludeNews_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeTvChannels")]
        public bool SitemapIncludeTvChannels { get; set; }
        public bool SitemapIncludeTvChannels_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeTvChannelTags")]
        public bool SitemapIncludeTvChannelTags { get; set; }
        public bool SitemapIncludeTvChannelTags_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapIncludeTopics")]
        public bool SitemapIncludeTopics { get; set; }
        public bool SitemapIncludeTopics_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.SitemapPageSize")]
        public int SitemapPageSize { get; set; }
        public bool SitemapPageSize_OverrideForStore { get; set; }
    }
}
