﻿using System;
using System.Threading.Tasks;
using TvProgViewer.WebUI.Models.Sitemap;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents a sitemap model factory
    /// </summary>
    public partial interface ISitemapModelFactory
    {
        /// <summary>
        /// Prepare the sitemap model
        /// </summary>
        /// <param name="pageModel">Sitemap page model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the sitemap model
        /// </returns>
        Task<SitemapModel> PrepareSitemapModelAsync(SitemapPageModel pageModel);

        /// <summary>
        /// Prepare sitemap model.
        /// This will build an XML sitemap for better index with search engines.
        /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
        /// </summary>
        /// <param name="id">Sitemap identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the sitemap model with sitemap.xml as string
        /// </returns>
        Task<SitemapXmlModel> PrepareSitemapXmlModelAsync(int id = 0);

        /// <summary>
        /// Prepare localized sitemap URL model
        /// </summary>
        /// <param name="routeName">Route name</param>
        /// <param name="getRouteParamsAwait">Lambda for route params object</param>
        /// <param name="dateTimeUpdatedOn">A time when URL was updated last time</param>
        /// <param name="updateFreq">How often to update url</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the sitemap URL model
        /// </returns>
        Task<SitemapUrlModel> PrepareLocalizedSitemapUrlAsync(string routeName,
            Func<int?, Task<object>> getRouteParamsAwait = null,
            DateTime? dateTimeUpdatedOn = null,
            UpdateFrequency updateFreq = UpdateFrequency.Weekly);
    }
}