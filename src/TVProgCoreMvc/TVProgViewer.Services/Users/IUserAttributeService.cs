using System.Collections.Generic;
using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Services.Users
{
    /// <summary>
    /// User attribute service
    /// </summary>
    public partial interface IUserAttributeService
    {
        /// <summary>
        /// Deletes a user attribute
        /// </summary>
        /// <param name="userAttribute">User attribute</param>
        Task DeleteUserAttributeAsync(UserAttribute userAttribute);

        /// <summary>
        /// Gets all user attributes
        /// </summary>
        /// <returns>User attributes</returns>
        Task<IList<UserAttribute>> GetAllUserAttributesAsync();

        /// <summary>
        /// Gets a user attribute 
        /// </summary>
        /// <param name="userAttributeId">User attribute identifier</param>
        /// <returns>User attribute</returns>
        Task<UserAttribute> GetUserAttributeByIdAsync(int userAttributeId);

        /// <summary>
        /// Inserts a user attribute
        /// </summary>
        /// <param name="userAttribute">User attribute</param>
        Task InsertUserAttributeAsync(UserAttribute userAttribute);

        /// <summary>
        /// Updates the user attribute
        /// </summary>
        /// <param name="userAttribute">User attribute</param>
        Task UpdateUserAttributeAsync(UserAttribute userAttribute);

        /// <summary>
        /// Deletes a user attribute value
        /// </summary>
        /// <param name="userAttributeValue">User attribute value</param>
        Task DeleteUserAttributeValueAsync(UserAttributeValue userAttributeValue);

        /// <summary>
        /// Gets user attribute values by user attribute identifier
        /// </summary>
        /// <param name="userAttributeId">The user attribute identifier</param>
        /// <returns>User attribute values</returns>
        Task<IList<UserAttributeValue>> GetUserAttributeValuesAsync(int userAttributeId);

        /// <summary>
        /// Gets a user attribute value
        /// </summary>
        /// <param name="userAttributeValueId">User attribute value identifier</param>
        /// <returns>User attribute value</returns>
        Task<UserAttributeValue> GetUserAttributeValueByIdAsync(int userAttributeValueId);

        /// <summary>
        /// Inserts a user attribute value
        /// </summary>
        /// <param name="userAttributeValue">User attribute value</param>
        Task InsertUserAttributeValueAsync(UserAttributeValue userAttributeValue);

        /// <summary>
        /// Updates the user attribute value
        /// </summary>
        /// <param name="userAttributeValue">User attribute value</param>
        Task UpdateUserAttributeValueAsync(UserAttributeValue userAttributeValue);
    }
}
