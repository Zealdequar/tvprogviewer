﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Directory;

namespace TvProgViewer.Services.Directory
{
    /// <summary>
    /// State province service interface
    /// </summary>
    public partial interface IStateProvinceService
    {
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="stateProvince">The state/province</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteStateProvinceAsync(StateProvince stateProvince);

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="stateProvinceId">The state/province identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the state/province
        /// </returns>
        Task<StateProvince> GetStateProvinceByIdAsync(int stateProvinceId);

        /// <summary>
        /// Gets a state/province by abbreviation
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <param name="countryId">Country identifier; pass null to load the state regardless of a country</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the state/province
        /// </returns>
        Task<StateProvince> GetStateProvinceByAbbreviationAsync(string abbreviation, int? countryId = null);

        /// <summary>
        /// Gets a state/province by address 
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the country
        /// </returns>
        Task<StateProvince> GetStateProvinceByAddressAsync(Address address);

        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the states
        /// </returns>
        Task<IList<StateProvince>> GetStateProvincesByCountryIdAsync(int countryId, int languageId = 0, bool showHidden = false);

        /// <summary>
        /// Gets all states/provinces
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the states
        /// </returns>
        Task<IList<StateProvince>> GetStateProvincesAsync(bool showHidden = false);

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertStateProvinceAsync(StateProvince stateProvince);

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateStateProvinceAsync(StateProvince stateProvince);
    }
}
