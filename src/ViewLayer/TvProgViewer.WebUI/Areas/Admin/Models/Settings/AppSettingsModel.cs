﻿using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents the app settings model
    /// </summary>
    public partial record AppSettingsModel : BaseTvProgModel
    {
        #region Ctor

        public AppSettingsModel()
        {
            DataConfigModel = new DataConfigModel();
            CacheConfigModel = new CacheConfigModel();
            DistributedCacheConfigModel = new DistributedCacheConfigModel();
            HostingConfigModel = new HostingConfigModel();
            AzureBlobConfigModel = new AzureBlobConfigModel();
            InstallationConfigModel = new InstallationConfigModel();
            PluginConfigModel = new PluginConfigModel();
            CommonConfigModel = new CommonConfigModel();
            WebOptimizerConfigModel = new WebOptimizerConfigModel();
            EnvironmentVariables = new List<string>();
        }

        #endregion

        #region Properties

        public DataConfigModel DataConfigModel { get; set; }

        public CacheConfigModel CacheConfigModel { get; set; }

        public HostingConfigModel HostingConfigModel { get; set; }

        public DistributedCacheConfigModel DistributedCacheConfigModel { get; set; }

        public AzureBlobConfigModel AzureBlobConfigModel { get; set; }

        public InstallationConfigModel InstallationConfigModel { get; set; }

        public PluginConfigModel PluginConfigModel { get; set; }

        public CommonConfigModel CommonConfigModel { get; set; }

        public WebOptimizerConfigModel WebOptimizerConfigModel { get; set; }

        public List<string> EnvironmentVariables { get; set; }

        #endregion
    }
}