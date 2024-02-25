using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Vendors;

namespace TvProgViewer.Services.Vendors
{
    /// <summary>
    /// Represents a vendor attribute service
    /// </summary>
    public partial interface IVendorAttributeService
    {
        #region Vendor attributes

        /// <summary>
        /// Gets all vendor attributes
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor attributes
        /// </returns>
        Task<IList<VendorAttribute>> GetAllVendorAttributesAsync();

        /// <summary>
        /// Gets a vendor attribute 
        /// </summary>
        /// <param name="vendorAttributeId">Vendor attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor attribute
        /// </returns>
        Task<VendorAttribute> GetVendorAttributeByIdAsync(int vendorAttributeId);

        /// <summary>
        /// Inserts a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertVendorAttributeAsync(VendorAttribute vendorAttribute);

        /// <summary>
        /// Updates a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateVendorAttributeAsync(VendorAttribute vendorAttribute);

        /// <summary>
        /// Deletes a vendor attribute
        /// </summary>
        /// <param name="vendorAttribute">Vendor attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteVendorAttributeAsync(VendorAttribute vendorAttribute);

        #endregion

        #region Vendor attribute values

        /// <summary>
        /// Gets vendor attribute values by vendor attribute identifier
        /// </summary>
        /// <param name="vendorAttributeId">The vendor attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor attribute values
        /// </returns>
        Task<IList<VendorAttributeValue>> GetVendorAttributeValuesAsync(int vendorAttributeId);

        /// <summary>
        /// Gets a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValueId">Vendor attribute value identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor attribute value
        /// </returns>
        Task<VendorAttributeValue> GetVendorAttributeValueByIdAsync(int vendorAttributeValueId);

        /// <summary>
        /// Inserts a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertVendorAttributeValueAsync(VendorAttributeValue vendorAttributeValue);

        /// <summary>
        /// Updates a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateVendorAttributeValueAsync(VendorAttributeValue vendorAttributeValue);

        /// <summary>
        /// Deletes a vendor attribute value
        /// </summary>
        /// <param name="vendorAttributeValue">Vendor attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteVendorAttributeValueAsync(VendorAttributeValue vendorAttributeValue);

        #endregion
    }
}