﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;

namespace TvProgViewer.Services.Plugins
{
    /// <summary>
    /// Represents the plugin service implementation
    /// </summary>
    public partial class PluginService : IPluginService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMigrationManager _migrationManager;
        private readonly ILogger _logger;
        private readonly ITvProgFileProvider _fileProvider;
        private readonly IPluginsInfo _pluginsInfo;
        private readonly IWebHelper _webHelper;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public PluginService(CatalogSettings catalogSettings,
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            IMigrationManager migrationManager,
            ILogger logger,
            ITvProgFileProvider fileProvider,
            IWebHelper webHelper,
            MediaSettings mediaSettings)
        {
            _catalogSettings = catalogSettings;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _migrationManager = migrationManager;
            _logger = logger;
            _fileProvider = fileProvider;
            _pluginsInfo = Singleton<IPluginsInfo>.Instance;
            _webHelper = webHelper;
            _mediaSettings = mediaSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether to load the plugin based on the load mode passed
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="loadMode">Load plugins mode</param>
        /// <returns>Result of check</returns>
        protected virtual bool FilterByLoadMode(PluginDescriptor pluginDescriptor, LoadPluginsMode loadMode)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            return loadMode switch
            {
                LoadPluginsMode.All => true,
                LoadPluginsMode.InstalledOnly => pluginDescriptor.Installed,
                LoadPluginsMode.NotInstalledOnly => !pluginDescriptor.Installed,
                _ => throw new NotSupportedException(nameof(loadMode)),
            };
        }

        /// <summary>
        /// Check whether to load the plugin based on the plugin group passed
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="group">Group name</param>
        /// <returns>Result of check</returns>
        protected virtual bool FilterByPluginGroup(PluginDescriptor pluginDescriptor, string group)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            if (string.IsNullOrEmpty(group))
                return true;

            return group.Equals(pluginDescriptor.Group, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Check whether to load the plugin based on the user passed
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result of check
        /// </returns>
        protected virtual async Task<bool> FilterByUserAsync(PluginDescriptor pluginDescriptor, User user)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            if (user == null || !pluginDescriptor.LimitedToUserRoles.Any())
                return true;

            if (_catalogSettings.IgnoreAcl)
                return true;

            return pluginDescriptor.LimitedToUserRoles.Intersect(await _userService.GetUserRoleIdsAsync(user)).Any();
        }

        /// <summary>
        /// Check whether to load the plugin based on the store identifier passed
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Result of check</returns>
        protected virtual bool FilterByStore(PluginDescriptor pluginDescriptor, int storeId)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            //no validation required
            if (storeId == 0)
                return true;

            if (!pluginDescriptor.LimitedToStores.Any())
                return true;

            return pluginDescriptor.LimitedToStores.Contains(storeId);
        }

        /// <summary>
        /// Check whether to load the plugin based on dependency from other plugin
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="dependsOnSystemName">Other plugin system name</param>
        /// <returns>Result of check</returns>
        protected virtual bool FilterByDependsOn(PluginDescriptor pluginDescriptor, string dependsOnSystemName)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            if (string.IsNullOrEmpty(dependsOnSystemName))
                return true;

            return pluginDescriptor.DependsOn?.Contains(dependsOnSystemName) ?? false;
        }

        /// <summary>
        /// Check whether to load the plugin based on the plugin friendly name passed
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="friendlyName">Plugin friendly name</param>
        /// <returns>Result of check</returns>
        protected virtual bool FilterByPluginFriendlyName(PluginDescriptor pluginDescriptor, string friendlyName)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            if (string.IsNullOrEmpty(friendlyName))
                return true;

            return pluginDescriptor.FriendlyName.Contains(friendlyName, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Check whether to load the plugin based on the plugin author passed
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="author">Plugin author</param>
        /// <returns>Result of check</returns>
        protected virtual bool FilterByPluginAuthor(PluginDescriptor pluginDescriptor, string author)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            if (string.IsNullOrEmpty(author))
                return true;

            return pluginDescriptor.Author.Contains(author, StringComparison.InvariantCultureIgnoreCase);
        }

        protected virtual void DeletePluginData(Type pluginType)
        {
            var assembly = Assembly.GetAssembly(pluginType);
            _migrationManager.ApplyDownMigrations(assembly);
        }

        protected virtual void InsertPluginData(Type pluginType, MigrationProcessType migrationProcessType = MigrationProcessType.NoMatter)
        {
            var assembly = Assembly.GetAssembly(pluginType);
            _migrationManager.ApplyUpMigrations(assembly, migrationProcessType);

            //mark update migrations as applied
            if (migrationProcessType == MigrationProcessType.Installation)
            {
                _migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update, true);
            }
        }

        protected virtual bool PluginsUploaded()
        {
            var pluginsDirectories =
                _fileProvider.GetDirectories(_fileProvider.MapPath(TvProgPluginDefaults.UploadedPath));

            if (!pluginsDirectories.Any())
                return false;

            return pluginsDirectories.Any(d =>
                _fileProvider.GetFiles(d, "*.dll").Any() || _fileProvider.GetFiles(d, "plugin.json").Any());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get plugin descriptors
        /// </summary>
        /// <typeparam name="TPlugin">The type of plugins to get</typeparam>
        /// <param name="loadMode">Filter by load plugins mode</param>
        /// <param name="user">Filter by  user; pass null to load all records</param>
        /// <param name="storeId">Filter by store; pass 0 to load all records</param>
        /// <param name="group">Filter by plugin group; pass null to load all records</param>
        /// <param name="friendlyName">Filter by plugin friendly name; pass null to load all records</param>
        /// <param name="author">Filter by plugin author; pass null to load all records</param>
        /// <param name="dependsOnSystemName">System name of the plugin to define dependencies</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the plugin descriptors
        /// </returns>
        public virtual async Task<IList<PluginDescriptor>> GetPluginDescriptorsAsync<TPlugin>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            User user = null, int storeId = 0, string group = null, string dependsOnSystemName = "", string friendlyName = null, string author = null) where TPlugin : class, IPlugin
        {
            var pluginDescriptors = _pluginsInfo.PluginDescriptors.Select(p => p.pluginDescriptor).ToList();

            //filter plugins
            pluginDescriptors = await pluginDescriptors.WhereAwait(async descriptor =>
                FilterByLoadMode(descriptor, loadMode) &&
                await FilterByUserAsync(descriptor, user) &&
                FilterByStore(descriptor, storeId) &&
                FilterByPluginGroup(descriptor, group) &&
                FilterByDependsOn(descriptor, dependsOnSystemName) &&
                FilterByPluginFriendlyName(descriptor, friendlyName) &&
                FilterByPluginAuthor(descriptor, author)).ToListAsync();

            //filter by the passed type
            if (typeof(TPlugin) != typeof(IPlugin))
                pluginDescriptors = pluginDescriptors.Where(descriptor => typeof(TPlugin).IsAssignableFrom(descriptor.PluginType)).ToList();

            //order by group name
            pluginDescriptors = pluginDescriptors.OrderBy(descriptor => descriptor.Group)
                .ThenBy(descriptor => descriptor.DisplayOrder).ToList();

            return pluginDescriptors;
        }

        /// <summary>
        /// Get a plugin descriptor by the system name
        /// </summary>
        /// <typeparam name="TPlugin">The type of plugin to get</typeparam>
        /// <param name="systemName">Plugin system name</param>
        /// <param name="loadMode">Load plugins mode</param>
        /// <param name="user">Filter by  user; pass null to load all records</param>
        /// <param name="storeId">Filter by store; pass 0 to load all records</param>
        /// <param name="group">Filter by plugin group; pass null to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the >Plugin descriptor
        /// </returns>
        public virtual async Task<PluginDescriptor> GetPluginDescriptorBySystemNameAsync<TPlugin>(string systemName,
            LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            User user = null, int storeId = 0, string @group = null) where TPlugin : class, IPlugin
        {
            return (await GetPluginDescriptorsAsync<TPlugin>(loadMode, user, storeId, group))
                .FirstOrDefault(descriptor => descriptor.SystemName.Equals(systemName));
        }

        /// <summary>
        /// Get plugins
        /// </summary>
        /// <typeparam name="TPlugin">The type of plugins to get</typeparam>
        /// <param name="loadMode">Filter by load plugins mode</param>
        /// <param name="user">Filter by user; pass null to load all records</param>
        /// <param name="storeId">Filter by store; pass 0 to load all records</param>
        /// <param name="group">Filter by plugin group; pass null to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the plugins
        /// </returns>
        public virtual async Task<IList<TPlugin>> GetPluginsAsync<TPlugin>(
            LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            User user = null, int storeId = 0, string @group = null) where TPlugin : class, IPlugin
        {
            return (await GetPluginDescriptorsAsync<TPlugin>(loadMode, user, storeId, group))
                .Select(descriptor => descriptor.Instance<TPlugin>()).ToList();
        }

        /// <summary>
        /// Find a plugin by the type which is located into the same assembly as a plugin
        /// </summary>
        /// <param name="typeInAssembly">Type</param>
        /// <returns>Plugin</returns>
        public virtual IPlugin FindPluginByTypeInAssembly(Type typeInAssembly)
        {
            if (typeInAssembly == null)
                throw new ArgumentNullException(nameof(typeInAssembly));

            //try to do magic
            var pluginDescriptor = _pluginsInfo.PluginDescriptors.FirstOrDefault(descriptor =>
                descriptor.pluginDescriptor?.ReferencedAssembly?.FullName?.Equals(typeInAssembly.Assembly.FullName,
                    StringComparison.InvariantCultureIgnoreCase) ?? false);

            return pluginDescriptor.pluginDescriptor?.Instance<IPlugin>();
        }

        /// <summary>
        /// Get plugin logo URL
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the logo URL
        /// </returns>
        public virtual Task<string> GetPluginLogoUrlAsync(PluginDescriptor pluginDescriptor)
        {
            var pluginDirectory = _fileProvider.GetDirectoryName(pluginDescriptor.OriginalAssemblyFile);
            if (string.IsNullOrEmpty(pluginDirectory))
                return Task.FromResult<string>(null);

            //check for supported extensions
            var logoExtension = TvProgPluginDefaults.SupportedLogoImageExtensions
                .FirstOrDefault(ext => _fileProvider.FileExists(_fileProvider.Combine(pluginDirectory, $"{TvProgPluginDefaults.LogoFileName}.{ext}")));
            if (string.IsNullOrWhiteSpace(logoExtension))
                return Task.FromResult<string>(null);

            var pathBase = _httpContextAccessor.HttpContext.Request.PathBase.Value ?? string.Empty;
            var logoPathUrl = _mediaSettings.UseAbsoluteImagePath ? _webHelper.GetStoreLocation() : $"{pathBase}/";

            var logoUrl = $"{logoPathUrl}{TvProgPluginDefaults.PathName}/" +
                $"{_fileProvider.GetDirectoryNameOnly(pluginDirectory)}/{TvProgPluginDefaults.LogoFileName}.{logoExtension}";

            return Task.FromResult(logoUrl);
        }

        /// <summary>
        /// Prepare plugin to the installation
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        /// <param name="user">User</param>
        /// <param name="checkDependencies">Specifies whether to check plugin dependencies</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PreparePluginToInstallAsync(string systemName, User user = null, bool checkDependencies = true)
        {
            //add plugin name to the appropriate list (if not yet contained) and save changes
            if (_pluginsInfo.PluginNamesToInstall.Any(item => item.SystemName == systemName))
                return;

            var pluginsAfterRestart = _pluginsInfo.InstalledPlugins.Select(pd => pd.SystemName).Where(installedSystemName => !_pluginsInfo.PluginNamesToUninstall.Contains(installedSystemName)).ToList();
            pluginsAfterRestart.AddRange(_pluginsInfo.PluginNamesToInstall.Select(item => item.SystemName));

            if (checkDependencies)
            {
                var descriptor = await GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.NotInstalledOnly);

                if (descriptor.DependsOn?.Any() ?? false)
                {
                    var dependsOn = descriptor.DependsOn
                        .Where(dependsOnSystemName => !pluginsAfterRestart.Contains(dependsOnSystemName)).ToList();

                    if (dependsOn.Any())
                    {
                        var dependsOnSystemNames = dependsOn.Aggregate((all, current) => $"{all}, {current}");

                        //do not inject services via constructor because it'll cause circular references
                        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

                        var errorMessage = string.Format(await localizationService.GetResourceAsync("Admin.Plugins.Errors.InstallDependsOn"), string.IsNullOrEmpty(descriptor.FriendlyName) ? descriptor.SystemName : descriptor.FriendlyName, dependsOnSystemNames);

                        throw new TvProgException(errorMessage);
                    }
                }
            }

            _pluginsInfo.PluginNamesToInstall.Add((systemName, user?.UserGuid));
            await _pluginsInfo.SaveAsync();
        }

        /// <summary>
        /// Prepare plugin to the uninstallation
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PreparePluginToUninstallAsync(string systemName)
        {
            //add plugin name to the appropriate list (if not yet contained) and save changes
            if (_pluginsInfo.PluginNamesToUninstall.Contains(systemName))
                return;

            var dependentPlugins = await GetPluginDescriptorsAsync<IPlugin>(dependsOnSystemName: systemName);
            var descriptor = await GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName);

            if (dependentPlugins.Any())
            {
                var dependsOn = new List<string>();

                foreach (var dependentPlugin in dependentPlugins)
                {
                    if (!_pluginsInfo.InstalledPlugins.Select(pd => pd.SystemName).Contains(dependentPlugin.SystemName))
                        continue;
                    if (_pluginsInfo.PluginNamesToUninstall.Contains(dependentPlugin.SystemName))
                        continue;

                    dependsOn.Add(string.IsNullOrEmpty(dependentPlugin.FriendlyName)
                        ? dependentPlugin.SystemName
                        : dependentPlugin.FriendlyName);
                }

                if (dependsOn.Any())
                {
                    var dependsOnSystemNames = dependsOn.Aggregate((all, current) => $"{all}, {current}");

                    //do not inject services via constructor because it'll cause circular references
                    var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

                    var errorMessage = string.Format(await localizationService.GetResourceAsync("Admin.Plugins.Errors.UninstallDependsOn"),
                        string.IsNullOrEmpty(descriptor.FriendlyName) ? descriptor.SystemName : descriptor.FriendlyName,
                        dependsOnSystemNames);

                    throw new TvProgException(errorMessage);
                }
            }

            var plugin = descriptor?.Instance<IPlugin>();

            if (plugin != null)
                await plugin.PreparePluginToUninstallAsync();

            _pluginsInfo.PluginNamesToUninstall.Add(systemName);
            await _pluginsInfo.SaveAsync();
        }

        /// <summary>
        /// Prepare plugin to the removing
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PreparePluginToDeleteAsync(string systemName)
        {
            //add plugin name to the appropriate list (if not yet contained) and save changes
            if (_pluginsInfo.PluginNamesToDelete.Contains(systemName))
                return;

            _pluginsInfo.PluginNamesToDelete.Add(systemName);
            await _pluginsInfo.SaveAsync();
        }

        /// <summary>
        /// Reset changes
        /// </summary>
        public virtual void ResetChanges()
        {
            //clear lists and save changes
            _pluginsInfo.PluginNamesToDelete.Clear();
            _pluginsInfo.PluginNamesToInstall.Clear();
            _pluginsInfo.PluginNamesToUninstall.Clear();
            _pluginsInfo.Save();

            //display all plugins on the plugin list page
            var pluginDescriptors = _pluginsInfo.PluginDescriptors.ToList();
            foreach (var pluginDescriptor in pluginDescriptors) 
                pluginDescriptor.pluginDescriptor.ShowInPluginsList = true;

            //clear the uploaded directory
            foreach (var directory in _fileProvider.GetDirectories(_fileProvider.MapPath(TvProgPluginDefaults.UploadedPath)))
                _fileProvider.DeleteDirectory(directory);
        }

        /// <summary>
        /// Clear installed plugins list
        /// </summary>
        public virtual void ClearInstalledPluginsList()
        {
            _pluginsInfo.InstalledPlugins.Clear();
        }

        /// <summary>
        /// Install plugins
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InstallPluginsAsync()
        {
            //get all uninstalled plugins
            var pluginDescriptors = _pluginsInfo.PluginDescriptors.Where(descriptor => !descriptor.pluginDescriptor.Installed).ToList();

            //filter plugins need to install
            pluginDescriptors = pluginDescriptors.Where(descriptor => _pluginsInfo.PluginNamesToInstall
                .Any(item => item.SystemName.Equals(descriptor.pluginDescriptor.SystemName))).ToList();
            if (!pluginDescriptors.Any())
                return;

            //do not inject services via constructor because it'll cause circular references
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var userActivityService = EngineContext.Current.Resolve<IUserActivityService>();

            //install plugins
            foreach (var descriptor in pluginDescriptors.OrderBy(pluginDescriptor => pluginDescriptor.pluginDescriptor.DisplayOrder))
            {
                try
                {
                    InsertPluginData(descriptor.pluginDescriptor.PluginType, MigrationProcessType.Installation);

                    //try to install an instance
                    await descriptor.pluginDescriptor.Instance<IPlugin>().InstallAsync();

                    //remove and add plugin system name to appropriate lists
                    var pluginToInstall = _pluginsInfo.PluginNamesToInstall
                        .FirstOrDefault(plugin => plugin.SystemName.Equals(descriptor.pluginDescriptor.SystemName));
                    _pluginsInfo.InstalledPlugins.Add(descriptor.pluginDescriptor.GetBaseInfoCopy);
                    _pluginsInfo.PluginNamesToInstall.Remove(pluginToInstall);

                    //activity log
                    var user = await _userService.GetUserByGuidAsync(pluginToInstall.UserGuid ?? Guid.Empty);
                    await userActivityService.InsertActivityAsync(user, "InstallNewPlugin",
                        string.Format(await localizationService.GetResourceAsync("ActivityLog.InstallNewPlugin"), descriptor.pluginDescriptor.SystemName));

                    //mark the plugin as installed
                    descriptor.pluginDescriptor.Installed = true;
                    descriptor.pluginDescriptor.ShowInPluginsList = true;
                }
                catch (Exception exception)
                {
                    //log error
                    var message = string.Format(await localizationService.GetResourceAsync("Admin.Plugins.Errors.NotInstalled"), descriptor.pluginDescriptor.SystemName);
                    await _logger.ErrorAsync(message, exception);
                }
            }

            //save changes
            await _pluginsInfo.SaveAsync();
        }

        /// <summary>
        /// Uninstall plugins
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UninstallPluginsAsync()
        {
            //get all installed plugins
            var pluginDescriptors = _pluginsInfo.PluginDescriptors.Where(descriptor => descriptor.pluginDescriptor.Installed).ToList();

            //filter plugins need to uninstall
            pluginDescriptors = pluginDescriptors
                .Where(descriptor => _pluginsInfo.PluginNamesToUninstall.Contains(descriptor.pluginDescriptor.SystemName)).ToList();
            if (!pluginDescriptors.Any())
                return;

            //do not inject services via constructor because it'll cause circular references
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var userActivityService = EngineContext.Current.Resolve<IUserActivityService>();

            //uninstall plugins
            foreach (var descriptor in pluginDescriptors.OrderByDescending(pluginDescriptor => pluginDescriptor.pluginDescriptor.DisplayOrder))
            {
                try
                {
                    var plugin = descriptor.pluginDescriptor.Instance<IPlugin>();
                    //try to uninstall an instance
                    await plugin.UninstallAsync();

                    //clear plugin data on the database
                    DeletePluginData(descriptor.pluginDescriptor.PluginType);

                    //remove plugin system name from appropriate lists
                    _pluginsInfo.InstalledPlugins.Remove(descriptor.pluginDescriptor);
                    _pluginsInfo.PluginNamesToUninstall.Remove(descriptor.pluginDescriptor.SystemName);

                    //activity log
                    await userActivityService.InsertActivityAsync("UninstallPlugin",
                        string.Format(await localizationService.GetResourceAsync("ActivityLog.UninstallPlugin"), descriptor.pluginDescriptor.SystemName));

                    //mark the plugin as uninstalled
                    descriptor.pluginDescriptor.Installed = false;
                    descriptor.pluginDescriptor.ShowInPluginsList = true;
                }
                catch (Exception exception)
                {
                    //log error
                    var message = string.Format(await localizationService.GetResourceAsync("Admin.Plugins.Errors.NotUninstalled"), descriptor.pluginDescriptor.SystemName);
                    await _logger.ErrorAsync(message, exception);
                }
            }

            //save changes
            await _pluginsInfo.SaveAsync();
        }

        /// <summary>
        /// Delete plugins
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeletePluginsAsync()
        {
            //get all uninstalled plugins (delete plugin only previously uninstalled)
            var pluginDescriptors = _pluginsInfo.PluginDescriptors.Where(descriptor => !descriptor.pluginDescriptor.Installed).ToList();

            //filter plugins need to delete
            pluginDescriptors = pluginDescriptors
                .Where(descriptor => _pluginsInfo.PluginNamesToDelete.Contains(descriptor.pluginDescriptor.SystemName)).ToList();
            if (!pluginDescriptors.Any())
                return;

            //do not inject services via constructor because it'll cause circular references
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var userActivityService = EngineContext.Current.Resolve<IUserActivityService>();

            //delete plugins
            foreach (var descriptor in pluginDescriptors)
            {
                try
                {
                    //try to delete a plugin directory from disk storage
                    var pluginDirectory = _fileProvider.GetDirectoryName(descriptor.pluginDescriptor.OriginalAssemblyFile);
                    if (_fileProvider.DirectoryExists(pluginDirectory))
                        _fileProvider.DeleteDirectory(pluginDirectory);

                    //remove plugin system name from the appropriate list
                    _pluginsInfo.PluginNamesToDelete.Remove(descriptor.pluginDescriptor.SystemName);

                    //activity log
                    await userActivityService.InsertActivityAsync("DeletePlugin",
                        string.Format(await localizationService.GetResourceAsync("ActivityLog.DeletePlugin"), descriptor.pluginDescriptor.SystemName));
                }
                catch (Exception exception)
                {
                    //log error
                    var message = string.Format(await localizationService.GetResourceAsync("Admin.Plugins.Errors.NotDeleted"), descriptor.pluginDescriptor.SystemName);
                    await _logger.ErrorAsync(message, exception);
                }
            }

            //save changes
            await _pluginsInfo.SaveAsync();
        }

        /// <summary>
        /// Check whether application restart is required to apply changes to plugins
        /// </summary>
        /// <returns>Result of check</returns>
        public virtual bool IsRestartRequired()
        {
            //return true if any of lists contains items or some plugins were uploaded
            return _pluginsInfo.PluginNamesToInstall.Any()
                   || _pluginsInfo.PluginNamesToUninstall.Any()
                   || _pluginsInfo.PluginNamesToDelete.Any()
                   || PluginsUploaded();
        }

        /// <summary>
        /// Update plugins
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdatePluginsAsync()
        {
            foreach (var installedPlugin in _pluginsInfo.InstalledPlugins)
            {
                var newVersion = _pluginsInfo.PluginDescriptors.FirstOrDefault(pd =>
                    pd.pluginDescriptor.SystemName.Equals(installedPlugin.SystemName, StringComparison.InvariantCultureIgnoreCase));

                if (newVersion.pluginDescriptor == null)
                    continue;

                if (installedPlugin.Version == newVersion.pluginDescriptor.Version)
                    continue;

                //run new migrations from the plugin if there are exists
                InsertPluginData(newVersion.pluginDescriptor.PluginType, MigrationProcessType.Update);

                //run the plugin update logic
                await newVersion.pluginDescriptor.Instance<IPlugin>().UpdateAsync(installedPlugin.Version, newVersion.pluginDescriptor.Version);

                //update installed plugin info
                installedPlugin.Version = newVersion.pluginDescriptor.Version;
            }

            await _pluginsInfo.SaveAsync();
        }

        /// <summary>
        /// Get names of incompatible plugins
        /// </summary>
        /// <returns>List of plugin names</returns>
        public virtual IList<string> GetIncompatiblePlugins()
        {
            return _pluginsInfo.IncompatiblePlugins.Keys.ToList();
        }

        /// <summary>
        /// Get all assembly loaded collisions
        /// </summary>
        /// <returns>List of plugin loaded assembly info</returns>
        public virtual IList<PluginLoadedAssemblyInfo> GetAssemblyCollisions()
        {
            return _pluginsInfo.AssemblyLoadedCollision;
        }

        #endregion
    }
}