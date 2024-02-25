using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.WebUI.Models.Directory;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the interface of the country model factory
    /// </summary>
    public partial interface ICountryModelFactory
    {
        /// <summary>
        /// Get states and provinces by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="addSelectStateItem">Whether to add "Select state" item to list of states</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of identifiers and names of states and provinces
        /// </returns>
        Task<IList<StateProvinceModel>> GetStatesByCountryIdAsync(string countryId, bool addSelectStateItem);
    }
}
