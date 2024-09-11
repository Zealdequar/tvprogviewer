using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// TvChannel attribute service interface
    /// </summary>
    public partial interface ITvChannelAttributeService
    {
        #region TvChannel attributes

        /// <summary>
        /// Deletes a tvChannel attribute
        /// </summary>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelAttributeAsync(TvChannelAttribute tvChannelAttribute);

        /// <summary>
        /// Deletes tvChannel attributes
        /// </summary>
        /// <param name="tvChannelAttributes">TvChannel attributes</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelAttributesAsync(IList<TvChannelAttribute> tvChannelAttributes);

        /// <summary>
        /// Gets all tvChannel attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attributes
        /// </returns>
        Task<IPagedList<TvChannelAttribute>> GetAllTvChannelAttributesAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a tvChannel attribute 
        /// </summary>
        /// <param name="tvChannelAttributeId">TvChannel attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute 
        /// </returns>
        Task<TvChannelAttribute> GetTvChannelAttributeByIdAsync(int tvChannelAttributeId);

        /// <summary>
        /// Gets tvChannel attributes 
        /// </summary>
        /// <param name="tvChannelAttributeIds">TvChannel attribute identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attributes
        /// </returns>
        Task<IList<TvChannelAttribute>> GetTvChannelAttributeByIdsAsync(int[] tvChannelAttributeIds);

        /// <summary>
        /// Inserts a tvChannel attribute
        /// </summary>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelAttributeAsync(TvChannelAttribute tvChannelAttribute);

        /// <summary>
        /// Updates the tvChannel attribute
        /// </summary>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelAttributeAsync(TvChannelAttribute tvChannelAttribute);

        /// <summary>
        /// Returns a list of IDs of not existing attributes
        /// </summary>
        /// <param name="attributeId">The IDs of the attributes to check</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of IDs not existing attributes
        /// </returns>
        Task<int[]> GetNotExistingAttributesAsync(int[] attributeId);

        #endregion

        #region TvChannel attributes mappings

        /// <summary>
        /// Deletes a tvChannel attribute mapping
        /// </summary>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelAttributeMappingAsync(TvChannelAttributeMapping tvChannelAttributeMapping);

        /// <summary>
        /// Gets tvChannel attribute mappings by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">The tvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute mapping collection
        /// </returns>
        Task<IList<TvChannelAttributeMapping>> GetTvChannelAttributeMappingsByTvChannelIdAsync(int tvChannelId);

        /// <summary>
        /// Gets a tvChannel attribute mapping
        /// </summary>
        /// <param name="tvChannelAttributeMappingId">TvChannel attribute mapping identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute mapping
        /// </returns>
        Task<TvChannelAttributeMapping> GetTvChannelAttributeMappingByIdAsync(int tvChannelAttributeMappingId);

        /// <summary>
        /// Inserts a tvChannel attribute mapping
        /// </summary>
        /// <param name="tvChannelAttributeMapping">The tvChannel attribute mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelAttributeMappingAsync(TvChannelAttributeMapping tvChannelAttributeMapping);

        /// <summary>
        /// Updates the tvChannel attribute mapping
        /// </summary>
        /// <param name="tvChannelAttributeMapping">The tvChannel attribute mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelAttributeMappingAsync(TvChannelAttributeMapping tvChannelAttributeMapping);

        #endregion

        #region TvChannel attribute values

        /// <summary>
        /// Deletes a tvChannel attribute value
        /// </summary>
        /// <param name="tvChannelAttributeValue">TvChannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelAttributeValueAsync(TvChannelAttributeValue tvChannelAttributeValue);

        /// <summary>
        /// Gets tvChannel attribute values by tvChannel attribute mapping identifier
        /// </summary>
        /// <param name="tvChannelAttributeMappingId">The tvChannel attribute mapping identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute values
        /// </returns>
        Task<IList<TvChannelAttributeValue>> GetTvChannelAttributeValuesAsync(int tvChannelAttributeMappingId);

        /// <summary>
        /// Gets a tvChannel attribute value
        /// </summary>
        /// <param name="tvChannelAttributeValueId">TvChannel attribute value identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute value
        /// </returns>
        Task<TvChannelAttributeValue> GetTvChannelAttributeValueByIdAsync(int tvChannelAttributeValueId);

        /// <summary>
        /// Inserts a tvChannel attribute value
        /// </summary>
        /// <param name="tvChannelAttributeValue">The tvChannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelAttributeValueAsync(TvChannelAttributeValue tvChannelAttributeValue);

        /// <summary>
        /// Updates the tvChannel attribute value
        /// </summary>
        /// <param name="tvChannelAttributeValue">The tvChannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelAttributeValueAsync(TvChannelAttributeValue tvChannelAttributeValue);

        #endregion

        #region Predefined tvChannel attribute values

        /// <summary>
        /// Deletes a predefined tvChannel attribute value
        /// </summary>
        /// <param name="ppav">Predefined tvChannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeletePredefinedTvChannelAttributeValueAsync(PredefinedTvChannelAttributeValue ppav);

        /// <summary>
        /// Gets predefined tvChannel attribute values by tvChannel attribute identifier
        /// </summary>
        /// <param name="tvChannelAttributeId">The tvChannel attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute mapping collection
        /// </returns>
        Task<IList<PredefinedTvChannelAttributeValue>> GetPredefinedTvChannelAttributeValuesAsync(int tvChannelAttributeId);

        /// <summary>
        /// Gets a predefined tvChannel attribute value
        /// </summary>
        /// <param name="id">Predefined tvChannel attribute value identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the predefined tvChannel attribute value
        /// </returns>
        Task<PredefinedTvChannelAttributeValue> GetPredefinedTvChannelAttributeValueByIdAsync(int id);

        /// <summary>
        /// Inserts a predefined tvChannel attribute value
        /// </summary>
        /// <param name="ppav">The predefined tvChannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertPredefinedTvChannelAttributeValueAsync(PredefinedTvChannelAttributeValue ppav);

        /// <summary>
        /// Updates the predefined tvChannel attribute value
        /// </summary>
        /// <param name="ppav">The predefined tvChannel attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdatePredefinedTvChannelAttributeValueAsync(PredefinedTvChannelAttributeValue ppav);

        #endregion

        #region TvChannel attribute combinations

        /// <summary>
        /// Deletes a tvChannel attribute combination
        /// </summary>
        /// <param name="combination">TvChannel attribute combination</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelAttributeCombinationAsync(TvChannelAttributeCombination combination);

        /// <summary>
        /// Gets all tvChannel attribute combinations
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute combinations
        /// </returns>
        Task<IList<TvChannelAttributeCombination>> GetAllTvChannelAttributeCombinationsAsync(int tvChannelId);

        /// <summary>
        /// Gets a tvChannel attribute combination
        /// </summary>
        /// <param name="tvChannelAttributeCombinationId">TvChannel attribute combination identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute combination
        /// </returns>
        Task<TvChannelAttributeCombination> GetTvChannelAttributeCombinationByIdAsync(int tvChannelAttributeCombinationId);

        /// <summary>
        /// Gets a tvChannel attribute combination by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute combination
        /// </returns>
        Task<TvChannelAttributeCombination> GetTvChannelAttributeCombinationBySkuAsync(string sku);

        /// <summary>
        /// Inserts a tvChannel attribute combination
        /// </summary>
        /// <param name="combination">TvChannel attribute combination</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelAttributeCombinationAsync(TvChannelAttributeCombination combination);

        /// <summary>
        /// Updates a tvChannel attribute combination
        /// </summary>
        /// <param name="combination">TvChannel attribute combination</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelAttributeCombinationAsync(TvChannelAttributeCombination combination);

        #endregion
    }
}
