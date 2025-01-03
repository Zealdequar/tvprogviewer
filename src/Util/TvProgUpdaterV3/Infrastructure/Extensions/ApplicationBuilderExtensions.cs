﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using TvProgViewer.Core;
using TvProgViewer.Core.Configuration;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Http;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Services.Authentication;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Installation;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Media.RoxyFileman;
using TvProgViewer.Services.Plugins;
using TvProgViewer.Services.ScheduleTasks;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.TvProgUpdaterV3.Globalization;
using TvProgViewer.TvProgUpdaterV3.Mvc.Routing;
using QuestPDF.Drawing;
using WebMarkupMin.AspNetCore7;
using WebOptimizer;
using Logging = TvProgViewer.Services.Logging;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using TvProgViewer.Services.TvProgMain;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;

namespace TvProgViewer.TvProgUpdaterV3.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void ConfigureRequestPipeline(this IApplicationBuilder application)
        {
            EngineContext.Current.ConfigureRequestPipeline(application);
        }

        public static void StartEngine(this IApplicationBuilder application)
        {
            var engine = EngineContext.Current;

            //further actions are performed only when the database is installed
            if (DataSettingsManager.IsDatabaseInstalled())
            {
                //log application start
                engine.Resolve<Logging.ILogger>().Information("Приложение запущено");

                //install and update plugins
                /* var pluginService = engine.Resolve<IPluginService>();
                 pluginService.InstallPluginsAsync().Wait();
                 pluginService.UpdatePluginsAsync().Wait();*/

                var factory = engine.Resolve<ILoggerFactory>();
                factory.AddNLog();
                factory.ConfigureNLog("NLog.config");

                var logger = engine.Resolve<ILogger<Program>>();
                logger.LogCritical("hello nlog");

                var updaterService = engine.Resolve<IUpdaterService>();
                Updater updater = new Updater(updaterService);
                var task = Task.Run(async () => await updater.UpdateTvProgrammes());
                task.WaitAndUnwrapException();

                //update tvProgViewer core and db
                var migrationManager = engine.Resolve<IMigrationManager>();
                var assembly = Assembly.GetAssembly(typeof(ApplicationBuilderExtensions));
                migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update);
                assembly = Assembly.GetAssembly(typeof(IMigrationManager));
                migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update);

                var taskScheduler = engine.Resolve<ITaskScheduler>();
                taskScheduler.InitializeAsync().Wait();
                taskScheduler.StartScheduler();
            }
        }

        /// <summary>
        /// Add exception handling
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseTvProgExceptionHandler(this IApplicationBuilder application)
        {
            var appSettings = EngineContext.Current.Resolve<AppSettings>();
            var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();
            var useDetailedExceptionPage = appSettings.Get<CommonConfig>().DisplayFullErrorStack || webHostEnvironment.IsDevelopment();
            if (useDetailedExceptionPage)
            {
                //get detailed exceptions for developing and testing purposes
                application.UseDeveloperExceptionPage();
            }
            else
            {
                //or use special exception handler
                application.UseExceptionHandler("/Error/Error");
            }

            //log errors
            application.UseExceptionHandler(handler =>
            {
                handler.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception == null)
                        return;

                    try
                    {
                        //check whether database is installed
                        if (DataSettingsManager.IsDatabaseInstalled())
                        {
                            //get current user
                            var currentUser = await EngineContext.Current.Resolve<IWorkContext>().GetCurrentUserAsync();

                            //log error
                            await EngineContext.Current.Resolve<Logging.ILogger>().ErrorAsync(exception.Message, exception, currentUser);
                        }
                    }
                    finally
                    {
                        //rethrow the exception to show the error page
                        ExceptionDispatchInfo.Throw(exception);
                    }
                });
            });
        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 404 status code that do not have a body
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UsePageNotFound(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(async context =>
            {
                //handle 404 Not Found
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    if (!webHelper.IsStaticResource())
                    {
                        //get original path and query
                        var originalPath = context.HttpContext.Request.Path;
                        var originalQueryString = context.HttpContext.Request.QueryString;

                        if (DataSettingsManager.IsDatabaseInstalled())
                        {
                            var commonSettings = EngineContext.Current.Resolve<CommonSettings>();

                            if (commonSettings.Log404Errors)
                            {
                                var logger = EngineContext.Current.Resolve<Logging.ILogger>();
                                var workContext = EngineContext.Current.Resolve<IWorkContext>();

                                await logger.ErrorAsync($"Error 404. The requested page ({originalPath}) was not found",
                                    user: await workContext.GetCurrentUserAsync());
                            }
                        }

                        try
                        {
                            //get new path
                            var pageNotFoundPath = "/page-not-found";
                            //re-execute request with new path
                            context.HttpContext.Response.Redirect(context.HttpContext.Request.PathBase + pageNotFoundPath);
                        }
                        finally
                        {
                            //return original path to request
                            context.HttpContext.Request.QueryString = originalQueryString;
                            context.HttpContext.Request.Path = originalPath;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 400 status code (bad request)
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseBadRequestResult(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(async context =>
            {
                //handle 404 (Bad request)
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    var logger = EngineContext.Current.Resolve<Logging.ILogger>();
                    var workContext = EngineContext.Current.Resolve<IWorkContext>();
                    await logger.ErrorAsync("Error 400. Bad request", null, user: await workContext.GetCurrentUserAsync());
                }
            });
        }

        /// <summary>
        /// Configure middleware for dynamically compressing HTTP responses
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseTvProgResponseCompression(this IApplicationBuilder application)
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //whether to use compression (gzip by default)
            if (EngineContext.Current.Resolve<CommonSettings>().UseResponseCompression)
                application.UseResponseCompression();
        }

        /// <summary>
        /// Adds WebOptimizer to the <see cref="IApplicationBuilder"/> request execution pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseTvProgWebOptimizer(this IApplicationBuilder application)
        {
            var fileProvider = EngineContext.Current.Resolve<ITvProgFileProvider>();
            var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();

            application.UseWebOptimizer(webHostEnvironment, new[]
            {
                new FileProviderOptions
                {
                    RequestPath =  new PathString("/Plugins"),
                    FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Plugins"))
                },
                new FileProviderOptions
                {
                    RequestPath =  new PathString("/Themes"),
                    FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Themes"))
                }
            });
        }

        /// <summary>
        /// Configure static file serving
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseTvProgStaticFiles(this IApplicationBuilder application)
        {
            var fileProvider = EngineContext.Current.Resolve<ITvProgFileProvider>();
            var appSettings = EngineContext.Current.Resolve<AppSettings>();

            void staticFileResponse(StaticFileResponseContext context)
            {
                if (!string.IsNullOrEmpty(appSettings.Get<CommonConfig>().StaticFilesCacheControl))
                    context.Context.Response.Headers.Append(HeaderNames.CacheControl, appSettings.Get<CommonConfig>().StaticFilesCacheControl);
            }

            //add handling if sitemaps 
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(TvProgSeoDefaults.SitemapXmlDirectory)),
                RequestPath = new PathString($"/{TvProgSeoDefaults.SitemapXmlDirectory}"),
                OnPrepareResponse = context =>
                {
                    if (!DataSettingsManager.IsDatabaseInstalled() ||
                        !EngineContext.Current.Resolve<SitemapXmlSettings>().SitemapXmlEnabled)
                    {
                        context.Context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Context.Response.ContentLength = 0;
                        context.Context.Response.Body = Stream.Null;
                    }
                }
            });

            //common static files
            application.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = staticFileResponse });

            //themes static files
            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Themes")),
                RequestPath = new PathString("/Themes"),
                OnPrepareResponse = staticFileResponse
            });

            //plugins static files
            var staticFileOptions = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.MapPath(@"Plugins")),
                RequestPath = new PathString("/Plugins"),
                OnPrepareResponse = staticFileResponse
            };

            //exclude files in blacklist
            if (!string.IsNullOrEmpty(appSettings.Get<CommonConfig>().PluginStaticFileExtensionsBlacklist))
            {
                var fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();

                foreach (var ext in appSettings.Get<CommonConfig>().PluginStaticFileExtensionsBlacklist
                    .Split(';', ',')
                    .Select(e => e.Trim().ToLowerInvariant())
                    .Select(e => $"{(e.StartsWith(".") ? string.Empty : ".")}{e}")
                    .Where(fileExtensionContentTypeProvider.Mappings.ContainsKey))
                {
                    fileExtensionContentTypeProvider.Mappings.Remove(ext);
                }

                staticFileOptions.ContentTypeProvider = fileExtensionContentTypeProvider;
            }

            application.UseStaticFiles(staticFileOptions);

            //add support for backups
            var provider = new FileExtensionContentTypeProvider
            {
                Mappings = { [".bak"] = MimeTypes.ApplicationOctetStream }
            };

            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(TvProgCommonDefaults.DbBackupsPath)),
                RequestPath = new PathString("/db_backups"),
                ContentTypeProvider = provider,
                OnPrepareResponse = context =>
                {
                    if (!DataSettingsManager.IsDatabaseInstalled() ||
                        !EngineContext.Current.Resolve<IPermissionService>().AuthorizeAsync(StandardPermissionProvider.ManageMaintenance).Result)
                    {
                        context.Context.Response.StatusCode = StatusCodes.Status404NotFound;
                        context.Context.Response.ContentLength = 0;
                        context.Context.Response.Body = Stream.Null;
                    }
                }
            });

            //add support for webmanifest files
            provider.Mappings[".webmanifest"] = MimeTypes.ApplicationManifestJson;

            application.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath("icons")),
                RequestPath = "/icons",
                ContentTypeProvider = provider
            });

            if (DataSettingsManager.IsDatabaseInstalled())
            {
                application.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = EngineContext.Current.Resolve<IRoxyFilemanFileProvider>(),
                    RequestPath = new PathString(TvProgRoxyFilemanDefaults.DefaultRootDirectory),
                    OnPrepareResponse = staticFileResponse
                });
            }

            if (appSettings.Get<CommonConfig>().ServeUnknownFileTypes)
            {
                application.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(fileProvider.GetAbsolutePath(".well-known")),
                    RequestPath = new PathString("/.well-known"),
                    ServeUnknownFileTypes = true,
                });
            }
        }

        /// <summary>
        /// Configure middleware checking whether requested page is keep alive page
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseKeepAlive(this IApplicationBuilder application)
        {
            application.UseMiddleware<KeepAliveMiddleware>();
        }

        /// <summary>
        /// Configure middleware checking whether database is installed
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseInstallUrl(this IApplicationBuilder application)
        {
            application.UseMiddleware<InstallUrlMiddleware>();
        }

        /// <summary>
        /// Adds the authentication middleware, which enables authentication capabilities.
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseTvProgAuthentication(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            application.UseMiddleware<AuthenticationMiddleware>();
        }

        /// <summary>
        /// Configure PDF
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseTvProgPdf(this IApplicationBuilder application)
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var fileProvider = EngineContext.Current.Resolve<ITvProgFileProvider>();
            var fontPaths = fileProvider.EnumerateFiles(fileProvider.MapPath("~/App_Data/Pdf/"), "*.ttf") ?? Enumerable.Empty<string>();

            //write placeholder characters instead of unavailable glyphs for both debug/release configurations
            QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;

            foreach (var fp in fontPaths)
            {
                FontManager.RegisterFont(File.OpenRead(fp));
            }
        }

        /// <summary>
        /// Configure the request localization feature
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseTvProgRequestLocalization(this IApplicationBuilder application)
        {
/*application.UseRequestLocalization(async options =>
            {
                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //prepare supported cultures
                var cultures = (await EngineContext.Current.Resolve<ILanguageService>().GetAllLanguagesAsync())
                    .OrderBy(language => language.DisplayOrder)
                    .Select(language => new CultureInfo(language.LanguageCulture)).ToList();
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
                options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault());
                options.ApplyCurrentCultureToResponseHeaders = true;

                //configure culture providers
                options.AddInitialRequestCultureProvider(new TvProgSeoUrlCultureProvider());
                var cookieRequestCultureProvider = options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().FirstOrDefault();
                if (cookieRequestCultureProvider is not null)
                    cookieRequestCultureProvider.CookieName = $"{TvProgCookieDefaults.Prefix}{TvProgCookieDefaults.CultureCookie}";
            });*/
        }

        /// <summary>
        /// Configure Endpoints routing
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseTvProgEndpoints(this IApplicationBuilder application)
        {
            //Execute the endpoint selected by the routing middleware
            application.UseEndpoints(endpoints =>
            {
                //register all routes
                EngineContext.Current.Resolve<IRoutePublisher>().RegisterRoutes(endpoints);
            });
        }

        /// <summary>
        /// Configure applying forwarded headers to their matching fields on the current request.
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseTvProgProxy(this IApplicationBuilder application)
        {
            var appSettings = EngineContext.Current.Resolve<AppSettings>();

            if (appSettings.Get<HostingConfig>().UseProxy)
            {
                var options = new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.All,
                    // IIS already serves as a reverse proxy and will add X-Forwarded headers to all requests,
                    // so we need to increase this limit, otherwise, passed forwarding headers will be ignored.
                    ForwardLimit = 2
                };

                if (!string.IsNullOrEmpty(appSettings.Get<HostingConfig>().ForwardedForHeaderName))
                    options.ForwardedForHeaderName = appSettings.Get<HostingConfig>().ForwardedForHeaderName;

                if (!string.IsNullOrEmpty(appSettings.Get<HostingConfig>().ForwardedProtoHeaderName))
                    options.ForwardedProtoHeaderName = appSettings.Get<HostingConfig>().ForwardedProtoHeaderName;

                if (!string.IsNullOrEmpty(appSettings.Get<HostingConfig>().KnownProxies))
                {
                    foreach (var strIp in appSettings.Get<HostingConfig>().KnownProxies.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                    {
                        if (IPAddress.TryParse(strIp, out var ip))
                            options.KnownProxies.Add(ip);
                    }

                    if (options.KnownProxies.Count > 1)
                        options.ForwardLimit = null; //disable the limit, because KnownProxies is configured
                }

                //configure forwarding
                application.UseForwardedHeaders(options);
            }
        }

        /// <summary>
        /// Configure WebMarkupMin
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseTvProgWebMarkupMin(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            application.UseWebMarkupMin();
        }
    }
}
