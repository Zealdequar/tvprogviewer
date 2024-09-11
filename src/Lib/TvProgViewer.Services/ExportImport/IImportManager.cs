using System.IO;
using System.Threading.Tasks;

namespace TvProgViewer.Services.ExportImport
{
    /// <summary>
    /// Import manager interface
    /// </summary>
    public partial interface IImportManager
    {
        /// <summary>
        /// Import tvChannels from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task ImportTvChannelsFromXlsxAsync(Stream stream);

        /// <summary>
        /// Import newsletter subscribers from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of imported subscribers
        /// </returns>
        Task<int> ImportNewsletterSubscribersFromTxtAsync(Stream stream);

        /// <summary>
        /// Import states from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="writeLog">Indicates whether to add logging</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of imported states
        /// </returns>
        Task<int> ImportStatesFromTxtAsync(Stream stream, bool writeLog = true);

        /// <summary>
        /// Import manufacturers from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task ImportManufacturersFromXlsxAsync(Stream stream);

        /// <summary>
        /// Import categories from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task ImportCategoriesFromXlsxAsync(Stream stream);

        /// <summary>
        /// Import orders from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task ImportOrdersFromXlsxAsync(Stream stream);
    }
}
