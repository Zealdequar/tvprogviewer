using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Discounts;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Manufacturer service
    /// </summary>
    public partial interface IManufacturerService
    {
        /// <summary>
        /// Clean up manufacturer references for a specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task ClearDiscountManufacturerMappingAsync(Discount discount);

        /// <summary>
        /// Deletes a discount-manufacturer mapping record
        /// </summary>
        /// <param name="discountManufacturerMapping">Discount-manufacturer mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteDiscountManufacturerMappingAsync(DiscountManufacturerMapping discountManufacturerMapping);

        /// <summary>
        /// Deletes a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteManufacturerAsync(Manufacturer manufacturer);

        /// <summary>
        /// Delete manufacturers
        /// </summary>
        /// <param name="manufacturers">Manufacturers</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteManufacturersAsync(IList<Manufacturer> manufacturers);

        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <param name="manufacturerName">Manufacturer name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" tvChannels
        /// false - load only "Unpublished" tvChannels
        /// </param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturers
        /// </returns>
        Task<IPagedList<Manufacturer>> GetAllManufacturersAsync(string manufacturerName = "",
            int storeId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false,
            bool? overridePublished = null);

        /// <summary>
        /// Get manufacturer identifiers to which a discount is applied
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer identifiers
        /// </returns>
        Task<IList<int>> GetAppliedManufacturerIdsAsync(Discount discount, User user);

        /// <summary>
        /// Gets a manufacturer
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer
        /// </returns>
        Task<Manufacturer> GetManufacturerByIdAsync(int manufacturerId);

        /// <summary>
        /// Gets the manufacturers by category identifier
        /// </summary>
        /// <param name="categoryId">Cateogry identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturers
        /// </returns>
        Task<IList<Manufacturer>> GetManufacturersByCategoryIdAsync(int categoryId);

        /// <summary>
        /// Gets manufacturers by identifier
        /// </summary>
        /// <param name="manufacturerIds">manufacturer identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturers
        /// </returns>
        Task<IList<Manufacturer>> GetManufacturersByIdsAsync(int[] manufacturerIds);

        /// <summary>
        /// Get manufacturers for which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted manufacturers</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of manufacturers
        /// </returns>
        Task<IPagedList<Manufacturer>> GetManufacturersWithAppliedDiscountAsync(int? discountId = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts a manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertManufacturerAsync(Manufacturer manufacturer);

        /// <summary>
        /// Updates the manufacturer
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateManufacturerAsync(Manufacturer manufacturer);

        /// <summary>
        /// Deletes a tvChannel manufacturer mapping
        /// </summary>
        /// <param name="tvChannelManufacturer">TvChannel manufacturer mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelManufacturerAsync(TvChannelManufacturer tvChannelManufacturer);

        /// <summary>
        /// Gets tvChannel manufacturer collection
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel manufacturer collection
        /// </returns>
        Task<IPagedList<TvChannelManufacturer>> GetTvChannelManufacturersByManufacturerIdAsync(int manufacturerId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a tvChannel manufacturer mapping collection
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel manufacturer mapping collection
        /// </returns>
        Task<IList<TvChannelManufacturer>> GetTvChannelManufacturersByTvChannelIdAsync(int tvChannelId, bool showHidden = false);

        /// <summary>
        /// Gets a tvChannel manufacturer mapping 
        /// </summary>
        /// <param name="tvChannelManufacturerId">TvChannel manufacturer mapping identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel manufacturer mapping
        /// </returns>
        Task<TvChannelManufacturer> GetTvChannelManufacturerByIdAsync(int tvChannelManufacturerId);

        /// <summary>
        /// Inserts a tvChannel manufacturer mapping
        /// </summary>
        /// <param name="tvChannelManufacturer">TvChannel manufacturer mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelManufacturerAsync(TvChannelManufacturer tvChannelManufacturer);

        /// <summary>
        /// Updates the tvChannel manufacturer mapping
        /// </summary>
        /// <param name="tvChannelManufacturer">TvChannel manufacturer mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelManufacturerAsync(TvChannelManufacturer tvChannelManufacturer);

        /// <summary>
        /// Get manufacturer IDs for tvChannels
        /// </summary>
        /// <param name="tvChannelIds">TvChannels IDs</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer IDs for tvChannels
        /// </returns>
        Task<IDictionary<int, int[]>> GetTvChannelManufacturerIdsAsync(int[] tvChannelIds);

        /// <summary>
        /// Returns a list of names of not existing manufacturers
        /// </summary>
        /// <param name="manufacturerIdsNames">The names and/or IDs of the manufacturers to check</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of names and/or IDs not existing manufacturers
        /// </returns>
        Task<string[]> GetNotExistingManufacturersAsync(string[] manufacturerIdsNames);

        /// <summary>
        /// Returns a TvChannelManufacturer that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>A TvChannelManufacturer that has the specified values; otherwise null</returns>
        TvChannelManufacturer FindTvChannelManufacturer(IList<TvChannelManufacturer> source, int tvChannelId, int manufacturerId);

        /// <summary>
        /// Get a discount-manufacturer mapping record
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<DiscountManufacturerMapping> GetDiscountAppliedToManufacturerAsync(int manufacturerId, int discountId);

        /// <summary>
        /// Inserts a discount-manufacturer mapping record
        /// </summary>
        /// <param name="discountManufacturerMapping">Discount-manufacturer mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertDiscountManufacturerMappingAsync(DiscountManufacturerMapping discountManufacturerMapping);
    }
}