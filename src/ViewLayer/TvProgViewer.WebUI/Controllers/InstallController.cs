﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Configuration;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data;
using TvProgViewer.Data.Configuration;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Installation;
using TvProgViewer.Services.Plugins;
using TvProgViewer.Services.Security;
using TvProgViewer.Web.Framework.Security;
using TvProgViewer.WebUI.Infrastructure.Installation;
using TvProgViewer.WebUI.Models.Install;

namespace TvProgViewer.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class InstallController : Controller
    {
        #region Fields

        private readonly AppSettings _appSettings;
        private readonly Lazy<IInstallationLocalizationService> _locService;
        private readonly Lazy<IInstallationService> _installationService;
        private readonly ITvProgFileProvider _fileProvider;
        private readonly Lazy<IPermissionService> _permissionService;
        private readonly Lazy<IPluginService> _pluginService;
        private readonly Lazy<IStaticCacheManager> _staticCacheManager;
        private readonly Lazy<IUploadService> _uploadService;
        private readonly Lazy<IWebHelper> _webHelper;
        private readonly Lazy<TvProgHttpClient> _nopHttpClient;

        #endregion

        #region Ctor

        public InstallController(AppSettings appSettings,
            Lazy<IInstallationLocalizationService> locService,
            Lazy<IInstallationService> installationService,
            ITvProgFileProvider fileProvider,
            Lazy<IPermissionService> permissionService,
            Lazy<IPluginService> pluginService,
            Lazy<IStaticCacheManager> staticCacheManager,
            Lazy<IUploadService> uploadService,
            Lazy<IWebHelper> webHelper,
            Lazy<TvProgHttpClient> nopHttpClient)
        {
            _appSettings = appSettings;
            _locService = locService;
            _installationService = installationService;
            _fileProvider = fileProvider;
            _permissionService = permissionService;
            _pluginService = pluginService;
            _staticCacheManager = staticCacheManager;
            _uploadService = uploadService;
            _webHelper = webHelper;
            _nopHttpClient = nopHttpClient;
        }

        #endregion

        #region Utilites

        private InstallModel PrepareCountryList(InstallModel model)
        {
            if (!model.InstallRegionalResources)
                return model;

            var browserCulture = _locService.Value.GetBrowserCulture();
            var countries = new List<SelectListItem>
            {
                //This item was added in case it was not possible to automatically determine the country by culture
                new SelectListItem { Value = string.Empty, Text = _locService.Value.GetResource("CountrySelect") }
            };
            countries.AddRange(from country in ISO3166.GetCollection()
                               from localization in ISO3166.GetLocalizationInfo(country.Alpha2)
                               let lang = ISO3166.GetLocalizationInfo(country.Alpha2).Count() > 1 ? $" [{localization.Language} language]" : string.Empty
                               let item = new SelectListItem
                               {
                                   Value = $"{country.Alpha2}-{localization.Culture}",
                                   Text = $"{country.Name}{lang}",
                                   Selected = (localization.Culture == browserCulture) && browserCulture[^2..] == country.Alpha2
                               }
                               select item);
            model.AvailableCountries.AddRange(countries);

            return model;
        }

        private InstallModel PrepareLanguageList(InstallModel model)
        {
            foreach (var lang in _locService.Value.GetAvailableLanguages())
            {
                model.AvailableLanguages.Add(new SelectListItem
                {
                    Value = Url.Action("ChangeLanguage", "Install", new { language = lang.Code }),
                    Text = lang.Name,
                    Selected = _locService.Value.GetCurrentLanguage().Code == lang.Code
                });
            }

            return model;
        }

        private InstallModel PrepareAvailableDataProviders(InstallModel model)
        {
            model.AvailableDataProviders.AddRange(
                _locService.Value.GetAvailableProviderTypes()
                .OrderBy(v => v.Value)
                .Select(pt => new SelectListItem
                {
                    Value = pt.Key.ToString(),
                    Text = pt.Value
                }));

            return model;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            var model = new InstallModel
            {
                AdminEmail = "admin@yourStore.com",
                InstallSampleData = false,
                InstallRegionalResources = _appSettings.Get<InstallationConfig>().InstallRegionalResources,
                DisableSampleDataOption = _appSettings.Get<InstallationConfig>().DisableSampleData,
                CreateDatabaseIfNotExists = false,
                ConnectionStringRaw = false,
                DataProvider = DataProviderType.SqlServer
            };

            PrepareAvailableDataProviders(model);
            PrepareLanguageList(model);
            PrepareCountryList(model);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Index(InstallModel model)
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            model.DisableSampleDataOption = _appSettings.Get<InstallationConfig>().DisableSampleData;
            model.InstallRegionalResources = _appSettings.Get<InstallationConfig>().InstallRegionalResources;

            PrepareAvailableDataProviders(model);
            PrepareLanguageList(model);
            PrepareCountryList(model);

            //Consider granting access rights to the resource to the ASP.NET request identity. 
            //ASP.NET has a base process identity 
            //(typically {MACHINE}\ASPNET on IIS 5 or Network Service on IIS 6 and IIS 7, 
            //and the configured application pool identity on IIS 7.5) that is used if the application is not impersonating.
            //If the application is impersonating via <identity impersonate="true"/>, 
            //the identity will be the anonymous user (typically IUSR_MACHINENAME) or the authenticated request user.

            //validate permissions
            var dirsToCheck = _fileProvider.GetDirectoriesWrite();
            foreach (var dir in dirsToCheck)
                if (!_fileProvider.CheckPermissions(dir, false, true, true, false))
                    ModelState.AddModelError(string.Empty, string.Format(_locService.Value.GetResource("ConfigureDirectoryPermissions"), CurrentOSUser.FullName, dir));

            var filesToCheck = _fileProvider.GetFilesWrite();
            foreach (var file in filesToCheck)
            {
                if (!_fileProvider.FileExists(file))
                    continue;

                if (!_fileProvider.CheckPermissions(file, false, true, true, true))
                    ModelState.AddModelError(string.Empty, string.Format(_locService.Value.GetResource("ConfigureFilePermissions"), CurrentOSUser.FullName, file));
            }

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var dataProvider = DataProviderManager.GetDataProvider(model.DataProvider);

                var connectionString = model.ConnectionStringRaw ? model.ConnectionString : dataProvider.BuildConnectionString(model);

                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception(_locService.Value.GetResource("ConnectionStringWrongFormat"));

                DataSettingsManager.SaveSettings(new DataConfig
                {
                    DataProvider = model.DataProvider,
                    ConnectionString = connectionString
                }, _fileProvider);

                if (model.CreateDatabaseIfNotExists)
                {
                    try
                    {
                        dataProvider.CreateDatabase(model.Collation);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format(_locService.Value.GetResource("DatabaseCreationError"), ex.Message));
                    }
                }
                else
                {
                    //check whether database exists
                    if (!await dataProvider.DatabaseExistsAsync())
                        throw new Exception(_locService.Value.GetResource("DatabaseNotExists"));
                }

                dataProvider.InitializeDatabase();

                var cultureInfo = new CultureInfo(TvProgCommonDefaults.DefaultLanguageCulture);
                var regionInfo = new RegionInfo(TvProgCommonDefaults.DefaultLanguageCulture);

                var languagePackInfo = (DownloadUrl: string.Empty, Progress: 0);
                if (model.InstallRegionalResources)
                {
                    //try to get CultureInfo and RegionInfo
                    try
                    {
                        cultureInfo = new CultureInfo(model.Country[3..]);
                        regionInfo = new RegionInfo(model.Country[3..]);
                    }
                    catch { }

                    //get URL to download language pack
                    if (cultureInfo.Name != TvProgCommonDefaults.DefaultLanguageCulture)
                    {
                        try
                        {
                            var languageCode = _locService.Value.GetCurrentLanguage().Code[0..2];
                            var resultString = await _nopHttpClient.Value.InstallationCompletedAsync(model.AdminEmail, languageCode, cultureInfo.Name);
                            var result = JsonConvert.DeserializeAnonymousType(resultString,
                                new { Message = string.Empty, LanguagePack = new { Culture = string.Empty, Progress = 0, DownloadLink = string.Empty } });
                            if (result.LanguagePack.Progress > TvProgCommonDefaults.LanguagePackMinTranslationProgressToInstall)
                            {
                                languagePackInfo.DownloadUrl = result.LanguagePack.DownloadLink;
                                languagePackInfo.Progress = result.LanguagePack.Progress;
                            }

                        }
                        catch { }
                    }

                    //upload CLDR
                    await _uploadService.Value.UploadLocalePatternAsync(cultureInfo);
                }

                //now resolve installation service
                await _installationService.Value.InstallRequiredDataAsync(model.AdminEmail, model.AdminPassword, languagePackInfo, regionInfo, cultureInfo);

                if (model.InstallSampleData)
                    await _installationService.Value.InstallSampleDataAsync(model.AdminEmail);

                //prepare plugins to install
                _pluginService.Value.ClearInstalledPluginsList();

                var pluginsIgnoredDuringInstallation = new List<string>();
                if (!string.IsNullOrEmpty(_appSettings.Get<InstallationConfig>().DisabledPlugins))
                {
                    pluginsIgnoredDuringInstallation = _appSettings.Get<InstallationConfig>().DisabledPlugins
                        .Split(',', StringSplitOptions.RemoveEmptyEntries).Select(pluginName => pluginName.Trim()).ToList();
                }

                var plugins = (await _pluginService.Value.GetPluginDescriptorsAsync<IPlugin>(LoadPluginsMode.All))
                    .Where(pluginDescriptor => !pluginsIgnoredDuringInstallation.Contains(pluginDescriptor.SystemName))
                    .OrderBy(pluginDescriptor => pluginDescriptor.Group).ThenBy(pluginDescriptor => pluginDescriptor.DisplayOrder)
                    .ToList();

                foreach (var plugin in plugins)
                {
                    await _pluginService.Value.PreparePluginToInstallAsync(plugin.SystemName, checkDependencies: false);
                }

                //register default permissions
                var permissionProviders = new List<Type> { typeof(StandardPermissionProvider) };
                foreach (var providerType in permissionProviders)
                {
                    var provider = (IPermissionProvider)Activator.CreateInstance(providerType);
                    await _permissionService.Value.InstallPermissionsAsync(provider);
                }

                return View(new InstallModel { RestartUrl = Url.RouteUrl("Homepage") });

            }
            catch (Exception exception)
            {
                await _staticCacheManager.Value.ClearAsync();

                //clear provider settings if something got wrong
                DataSettingsManager.SaveSettings(new DataConfig(), _fileProvider);

                ModelState.AddModelError(string.Empty, string.Format(_locService.Value.GetResource("SetupFailed"), exception.Message));
            }

            return View(model);
        }

        public virtual IActionResult ChangeLanguage(string language)
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            _locService.Value.SaveCurrentLanguage(language);

            //Reload the page
            return RedirectToAction("Index", "Install");
        }

        [HttpPost]
        public virtual IActionResult RestartInstall()
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            return View("Index", new InstallModel { RestartUrl = Url.Action("Index", "Install") });
        }

        public virtual IActionResult RestartApplication()
        {
            if (DataSettingsManager.IsDatabaseInstalled())
                return RedirectToRoute("Homepage");

            //restart application
            _webHelper.Value.RestartAppDomain();

            return new EmptyResult();
        }

        #endregion
    }
}