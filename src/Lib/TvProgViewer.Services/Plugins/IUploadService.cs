using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TvProgViewer.Services.Plugins
{
    /// <summary>
    /// Represents a service for uploading application extensions (plugins or themes) and favicon and app icons
    /// </summary>
    public partial interface IUploadService
    {
        /// <summary>
        /// Upload plugins and/or themes
        /// </summary>
        /// <param name="archivefile">Archive file</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of uploaded items descriptor
        /// </returns>
        Task<IList<IDescriptor>> UploadPluginsAndThemesAsync(IFormFile archivefile);

        /// <summary>
        /// Upload favicon and app icons
        /// </summary>
        /// <param name="archivefile">Archive file which contains a set of special icons for different OS and devices</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UploadIconsArchiveAsync(IFormFile archivefile);

        /// <summary>
        /// Upload single favicon
        /// </summary>
        /// <param name="favicon">Favicon</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UploadFaviconAsync(IFormFile favicon);

        /// <summary>
        /// Upload locale pattern for current culture
        /// </summary>
        /// <param name="cultureInfo">CultureInfo</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UploadLocalePatternAsync(CultureInfo cultureInfo = null);
    }
}