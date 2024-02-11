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
        /// Deletes a tvchannel attribute
        /// </summary>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTvChannelAttributeAsync(TvChannelAttribute tvchannelAttribute);

        /// <summary>
        /// Deletes tvchannel attributes
        /// </summary>
        /// <param name="tvchannelAttributes">TvChannel attributes</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTvChannelAttributesAsync(IList<TvChannelAttribute> tvchannelAttributes);

        /// <summary>
        /// Gets all tvchannel attributes
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attributes
        /// </returns>
        Task<IPagedList<TvChannelAttribute>> GetAllTvChannelAttributesAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a tvchannel attribute 
        /// </summary>
        /// <param name="tvchannelAttributeId">TvChannel attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute 
        /// </returns>
        Task<TvChannelAttribute> GetTvChannelAttributeByIdAsync(int tvchannelAttributeId);

        /// <summary>
        /// Gets tvchannel attributes 
        /// </summary>
        /// <param name="tvchannelAttributeIds">TvChannel attribute identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attributes
        /// </returns>
        Task<IList<TvChannelAttribute>> GetTvChannelAttributeByIdsAsync(int[] tvchannelAttributeIds);

        /// <summary>
        /// Inserts a tvchannel attribute
        /// </summary>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertTvChannelAttributeAsync(TvChannelAttribute tvchannelAttribute);

        /// <summary>
        /// Updates the tvchannel attribute
        /// </summary>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTvChannelAttributeAsync(TvChannelAttribute tvchannelAttribute);

        /// <summary>
        /// Returns a list of IDs of not existing attributes
        /// </summary>
        /// <param name="attributeId">The IDs of the attributes to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of IDs not existing attributes
        /// </returns>
        Task<int[]> GetNotExistingAttributesAsync(int[] attributeId);

        #endregion

        #region TvChannel attributes mappings

        /// <summary>
        /// Deletes a tvchannel attribute mapping
        /// </summary>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTvChannelAttributeMappingAsync(TvChannelAttributeMapping tvchannelAttributeMapping);

        /// <summary>
        /// Gets tvchannel attribute mappings by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">The tvchannel identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute mapping collection
        /// </returns>
        Task<IList<TvChannelAttributeMapping>> GetTvChannelAttributeMappingsByTvChannelIdAsync(int tvchannelId);

        /// <summary>
        /// Gets a tvchannel attribute mapping
        /// </summary>
        /// <param name="tvchannelAttributeMappingId">TvChannel attribute mapping identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute mapping
        /// </returns>
        Task<TvChannelAttributeMapping> GetTvChannelAttributeMappingByIdAsync(int tvchannelAttributeMappingId);

        /// <summary>
        /// Inserts a tvchannel attribute mapping
        /// </summary>
        /// <param name="tvchannelAttributeMapping">The tvchannel attribute mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertTvChannelAttributeMappingAsync(TvChannelAttributeMapping tvchannelAttributeMapping);

        /// <summary>
        /// Updates the tvchannel attribute mapping
        /// </summary>
        /// <param name="tvchannelAttributeMapping">The tvchannel attribute mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTvChannelAttributeMappingAsync(TvChannelAttributeMapping tvchannelAttributeMapping);

        #endregion

        #region TvChannel attribute values

        /// <summary>
        /// Deletes a tvchannel attribute value
        /// </summary>
        /// <param name="tvchannelAttributeValue">TvChannel attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTvChannelAttributeValueAsync(TvChannelAttributeValue tvchannelAttributeValue);

        /// <summary>
        /// Gets tvchannel attribute values by tvchannel attribute mapping identifier
        /// </summary>
        /// <param name="tvchannelAttributeMappingId">The tvchannel attribute mapping identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute values
        /// </returns>
        Task<IList<TvChannelAttributeValue>> GetTvChannelAttributeValuesAsync(int tvchannelAttributeMappingId);

        /// <summary>
        /// Gets a tvchannel attribute value
        /// </summary>
        /// <param name="tvchannelAttributeValueId">TvChannel attribute value identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute value
        /// </returns>
        Task<TvChannelAttributeValue> GetTvChannelAttributeValueByIdAsync(int tvchannelAttributeValueId);

        /// <summary>
        /// Inserts a tvchannel attribute value
        /// </summary>
        /// <param name="tvchannelAttributeValue">The tvchannel attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertTvChannelAttributeValueAsync(TvChannelAttributeValue tvchannelAttributeValue);

        /// <summary>
        /// Updates the tvchannel attribute value
        /// </summary>
        /// <param name="tvchannelAttributeValue">The tvchannel attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTvChannelAttributeValueAsync(TvChannelAttributeValue tvchannelAttributeValue);

        #endregion

        #region Predefined tvchannel attribute values

        /// <summary>
        /// Deletes a predefined tvchannel attribute value
        /// </summary>
        /// <param name="ppav">Predefined tvchannel attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeletePredefinedTvChannelAttributeValueAsync(PredefinedTvChannelAttributeValue ppav);

        /// <summary>
        /// Gets predefined tvchannel attribute values by tvchannel attribute identifier
        /// </summary>
        /// <param name="tvchannelAttributeId">The tvchannel attribute identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute mapping collection
        /// </returns>
        Task<IList<PredefinedTvChannelAttributeValue>> GetPredefinedTvChannelAttributeValuesAsync(int tvchannelAttributeId);

        /// <summary>
        /// Gets a predefined tvchannel attribute value
        /// </summary>
        /// <param name="id">Predefined tvchannel attribute value identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the predefined tvchannel attribute value
        /// </returns>
        Task<PredefinedTvChannelAttributeValue> GetPredefinedTvChannelAttributeValueByIdAsync(int id);

        /// <summary>
        /// Inserts a predefined tvchannel attribute value
        /// </summary>
        /// <param name="ppav">The predefined tvchannel attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertPredefinedTvChannelAttributeValueAsync(PredefinedTvChannelAttributeValue ppav);

        /// <summary>
        /// Updates the predefined tvchannel attribute value
        /// </summary>
        /// <param name="ppav">The predefined tvchannel attribute value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdatePredefinedTvChannelAttributeValueAsync(PredefinedTvChannelAttributeValue ppav);

        #endregion

        #region TvChannel attribute combinations

        /// <summary>
        /// Deletes a tvchannel attribute combination
        /// </summary>
        /// <param name="combination">TvChannel attribute combination</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTvChannelAttributeCombinationAsync(TvChannelAttributeCombination combination);

        /// <summary>
        /// Gets all tvchannel attribute combinations
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute combinations
        /// </returns>
        Task<IList<TvChannelAttributeCombination>> GetAllTvChannelAttributeCombinationsAsync(int tvchannelId);

        /// <summary>
        /// Gets a tvchannel attribute combination
        /// </summary>
        /// <param name="tvchannelAttributeCombinationId">TvChannel attribute combination identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute combination
        /// </returns>
        Task<TvChannelAttributeCombination> GetTvChannelAttributeCombinationByIdAsync(int tvchannelAttributeCombinationId);

        /// <summary>
        /// Gets a tvchannel attribute combination by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute combination
        /// </returns>
        Task<TvChannelAttributeCombination> GetTvChannelAttributeCombinationBySkuAsync(string sku);

        /// <summary>
        /// Inserts a tvchannel attribute combination
        /// </summary>
        /// <param name="combination">TvChannel attribute combination</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertTvChannelAttributeCombinationAsync(TvChannelAttributeCombination combination);

        /// <summary>
        /// Updates a tvchannel attribute combination
        /// </summary>
        /// <param name="combination">TvChannel attribute combination</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTvChannelAttributeCombinationAsync(TvChannelAttributeCombination combination);

        #endregion
    }
}
