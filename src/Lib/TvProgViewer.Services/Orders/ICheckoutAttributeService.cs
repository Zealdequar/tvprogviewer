using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Orders;

namespace TvProgViewer.Services.Orders
{
    /// <summary>
    /// Checkout attribute service
    /// </summary>
    public partial interface ICheckoutAttributeService
    {
        #region Checkout attributes

        /// <summary>
        /// Deletes a checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteCheckoutAttributeAsync(CheckoutAttribute checkoutAttribute);

        /// <summary>
        /// Deletes checkout attributes
        /// </summary>
        /// <param name="checkoutAttributes">Checkout attributes</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteCheckoutAttributesAsync(IList<CheckoutAttribute> checkoutAttributes);

        /// <summary>
        /// Gets all checkout attributes
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="excludeShippableAttributes">A value indicating whether we should exclude shippable attributes</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the checkout attributes
        /// </returns>
        Task<IList<CheckoutAttribute>> GetAllCheckoutAttributesAsync(int storeId = 0, bool excludeShippableAttributes = false);

        /// <summary>
        /// Gets a checkout attribute 
        /// </summary>
        /// <param name="checkoutAttributeId">Checkout attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the checkout attribute
        /// </returns>
        Task<CheckoutAttribute> GetCheckoutAttributeByIdAsync(int checkoutAttributeId);

        /// <summary>
        /// Gets checkout attributes 
        /// </summary>
        /// <param name="checkoutAttributeIds">Checkout attribute identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the checkout attributes
        /// </returns>
        Task<IList<CheckoutAttribute>> GetCheckoutAttributeByIdsAsync(int[] checkoutAttributeIds);

        /// <summary>
        /// Inserts a checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertCheckoutAttributeAsync(CheckoutAttribute checkoutAttribute);

        /// <summary>
        /// Updates the checkout attribute
        /// </summary>
        /// <param name="checkoutAttribute">Checkout attribute</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateCheckoutAttributeAsync(CheckoutAttribute checkoutAttribute);

        #endregion

        #region Checkout attribute values

        /// <summary>
        /// Deletes a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteCheckoutAttributeValueAsync(CheckoutAttributeValue checkoutAttributeValue);

        /// <summary>
        /// Gets checkout attribute values by checkout attribute identifier
        /// </summary>
        /// <param name="checkoutAttributeId">The checkout attribute identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the checkout attribute values
        /// </returns>
        Task<IList<CheckoutAttributeValue>> GetCheckoutAttributeValuesAsync(int checkoutAttributeId);

        /// <summary>
        /// Gets a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValueId">Checkout attribute value identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the checkout attribute value
        /// </returns>
        Task<CheckoutAttributeValue> GetCheckoutAttributeValueByIdAsync(int checkoutAttributeValueId);

        /// <summary>
        /// Inserts a checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertCheckoutAttributeValueAsync(CheckoutAttributeValue checkoutAttributeValue);

        /// <summary>
        /// Updates the checkout attribute value
        /// </summary>
        /// <param name="checkoutAttributeValue">Checkout attribute value</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateCheckoutAttributeValueAsync(CheckoutAttributeValue checkoutAttributeValue);
        
        #endregion
    }
}
