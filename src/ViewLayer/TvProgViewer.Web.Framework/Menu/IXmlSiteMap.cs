﻿using System.Threading.Tasks;

namespace TvProgViewer.Web.Framework.Menu
{
    /// <summary>
    /// XML sitemap interface
    /// </summary>
    public partial interface IXmlSiteMap
    {
        /// <summary>
        /// Root node
        /// </summary>
        SiteMapNode RootNode { get; set; }

        /// <summary>
        /// Load sitemap
        /// </summary>
        /// <param name="physicalPath">Filepath to load a sitemap</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task LoadFromAsync(string physicalPath);
    }
}