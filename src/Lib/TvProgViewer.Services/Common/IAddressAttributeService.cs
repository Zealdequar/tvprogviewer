﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Common;

namespace TvProgViewer.Services.Common
{
    /// <summary>
    /// Address attribute service
    /// </summary>
    public partial interface IAddressAttributeService
    {
        /// <summary>
        /// Deletes an address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteAddressAttributeAsync(AddressAttribute addressAttribute);

        /// <summary>
        /// Gets all address attributes
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the address attributes
        /// </returns>
        Task<IList<AddressAttribute>> GetAllAddressAttributesAsync();

        /// <summary>
        /// Gets an address attribute 
        /// </summary>
        /// <param name="addressAttributeId">Address attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the address attribute
        /// </returns>
        Task<AddressAttribute> GetAddressAttributeByIdAsync(int addressAttributeId);

        /// <summary>
        /// Inserts an address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertAddressAttributeAsync(AddressAttribute addressAttribute);

        /// <summary>
        /// Updates the address attribute
        /// </summary>
        /// <param name="addressAttribute">Address attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateAddressAttributeAsync(AddressAttribute addressAttribute);

        /// <summary>
        /// Deletes an address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteAddressAttributeValueAsync(AddressAttributeValue addressAttributeValue);

        /// <summary>
        /// Gets address attribute values by address attribute identifier
        /// </summary>
        /// <param name="addressAttributeId">The address attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the address attribute values
        /// </returns>
        Task<IList<AddressAttributeValue>> GetAddressAttributeValuesAsync(int addressAttributeId);

        /// <summary>
        /// Gets an address attribute value
        /// </summary>
        /// <param name="addressAttributeValueId">Address attribute value identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the address attribute value
        /// </returns>
        Task<AddressAttributeValue> GetAddressAttributeValueByIdAsync(int addressAttributeValueId);

        /// <summary>
        /// Inserts a address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertAddressAttributeValueAsync(AddressAttributeValue addressAttributeValue);

        /// <summary>
        /// Updates the address attribute value
        /// </summary>
        /// <param name="addressAttributeValue">Address attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateAddressAttributeValueAsync(AddressAttributeValue addressAttributeValue);
    }
}
