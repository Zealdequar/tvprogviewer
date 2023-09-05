﻿using System.Collections.Generic;

namespace TvProgViewer.WebUI.Models.Sitemap
{
    /// <summary>
    /// Represents an event that occurs when the sitemap is created
    /// </summary>
    public partial class SitemapCreatedEvent
    {
        #region Ctor

        public SitemapCreatedEvent(IList<SitemapUrlModel> sitemapUrls)
        {
            SitemapUrls = sitemapUrls ?? new List<SitemapUrlModel>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of sitemap URLs
        /// </summary>
        public IList<SitemapUrlModel> SitemapUrls { get; }

        #endregion
    }
}