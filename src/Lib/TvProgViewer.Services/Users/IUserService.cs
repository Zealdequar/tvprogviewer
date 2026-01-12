using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Tax;

namespace TvProgViewer.Services.Users
{
    /// <summary>
    /// User service interface
    /// </summary>
    public partial interface IUserService
    {
        #region Users

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="lastActivityFromUtc">Last activity date from (UTC); null to load all records</param>
        /// <param name="lastActivityToUtc">Last activity date to (UTC); null to load all records</param>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
        /// <param name="email">Email; null to load all users</param>
        /// <param name="username">Username; null to load all users</param>
        /// <param name="firstName">First name; null to load all users</param>
        /// <param name="lastName">Last name; null to load all users</param>
        /// <param name="middleName">Middle name; null to load all users</param>
        /// <param name="dayOfBirth">Day of birth; 0 to load all users</param>
        /// <param name="monthOfBirth">Month of birth; 0 to load all users</param>
        /// <param name="company">Company; null to load all users</param>
        /// <param name="phone">SmartPhone; null to load all users</param>
        /// <param name="zipPostalCode">SmartPhone; null to load all users</param>
        /// <param name="ipAddress">IP address; null to load all users</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the users
        /// </returns>
        Task<IPagedList<User>> GetAllUsersAsync(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            DateTime? lastActivityFromUtc = null, DateTime? lastActivityToUtc = null,
            int affiliateId = 0, int vendorId = 0, int[] userRoleIds = null,
            string email = null, string username = null, string firstName = null, string lastName = null, string middleName = null,
            int dayOfBirth = 0, int monthOfBirth = 0,
            string company = null, string phone = null, string zipPostalCode = null, string ipAddress = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Gets online users
        /// </summary>
        /// <param name="lastActivityFromUtc">User last activity date (from)</param>
        /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the users
        /// </returns>
        Task<IPagedList<User>> GetOnlineUsersAsync(DateTime lastActivityFromUtc,
            int[] userRoleIds, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets users with shopping carts
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type; pass null to load all records</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="tvChannelId">TvChannel identifier; pass null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); pass null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); pass null to load all records</param>
        /// <param name="countryId">Billing country identifier; pass null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the users
        /// </returns>
        Task<IPagedList<User>> GetUsersWithShoppingCartsAsync(ShoppingCartType? shoppingCartType = null,
            int storeId = 0, int? tvChannelId = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null, int? countryId = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets user for shopping cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<User> GetShoppingCartUserAsync(IList<ShoppingCartItem> shoppingCart);

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteUserAsync(User user);

        /// <summary>
        /// Gets built-in system record used for background tasks
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains a user object
        /// </returns>
        Task<User> GetOrCreateBackgroundTaskUserAsync();

        /// <summary>
        /// Gets built-in system guest record used for requests from search engines
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains a user object
        /// </returns>
        Task<User> GetOrCreateSearchEngineUserAsync();

        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains a user
        /// </returns>
        Task<User> GetUserByIdAsync(int userId);

        /// <summary>
        /// Get users by identifiers
        /// </summary>
        /// <param name="userIds">User identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the users
        /// </returns>
        Task<IList<User>> GetUsersByIdsAsync(int[] userIds);

        /// <summary>
        /// Get users by guids
        /// </summary>
        /// <param name="userGuids">User guids</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the users
        /// </returns>
        Task<IList<User>> GetUsersByGuidsAsync(Guid[] userGuids);

        /// <summary>
        /// Gets a user by GUID
        /// </summary>
        /// <param name="userGuid">User GUID</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains a user
        /// </returns>
        Task<User> GetUserByGuidAsync(Guid userGuid);

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user
        /// </returns>
        Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Get user by system role
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user
        /// </returns>
        Task<User> GetUserBySystemNameAsync(string systemName);

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user
        /// </returns>
        Task<User> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Insert a guest user
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user
        /// </returns>
        Task<User> InsertGuestUserAsync();

        /// <summary>
        /// Вставка пользователя с ролью TvGuest
        /// </summary>
        /// <param name="uuid">Уникальный идентификатор пользователя</param>
        /// <param name="ipAddress">IP-адрес</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// Результат задачи содержит пользователя
        /// </returns>
        Task<User> InsertTvGuestUserAsync(string uuid, string ipAddress);

        /// <summary>
        /// Вставка пользователя с ролью TvGreenData
        /// </summary>
        /// <param name="uuid">Уникальный идентификатор пользователя</param>
        /// <param name="ipAddress">IP-адрес</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// Результат задачи содержит пользователя
        /// </returns>
        Task<User> InsertTvGreenDataUserAsync(string uuid, string ipAddress);

        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertUserAsync(User user);

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateUserAsync(User user);

        /// <summary>
        /// Reset data required for checkout
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCouponCodes">A value indicating whether to clear coupon code</param>
        /// <param name="clearCheckoutAttributes">A value indicating whether to clear selected checkout attributes</param>
        /// <param name="clearRewardPoints">A value indicating whether to clear "Use reward points" flag</param>
        /// <param name="clearShippingMethod">A value indicating whether to clear selected shipping method</param>
        /// <param name="clearPaymentMethod">A value indicating whether to clear selected payment method</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task ResetCheckoutDataAsync(User user, int storeId,
            bool clearCouponCodes = false, bool clearCheckoutAttributes = false,
            bool clearRewardPoints = true, bool clearShippingMethod = true,
            bool clearPaymentMethod = true);

        /// <summary>
        /// Delete guest user records
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="onlyWithoutShoppingCart">A value indicating whether to delete users only without shopping cart</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of deleted users
        /// </returns>
        Task<int> DeleteGuestUsersAsync(DateTime? createdFromUtc, DateTime? createdToUtc, bool onlyWithoutShoppingCart);

        /// <summary>
        /// Gets a default tax display type (if configured)
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<TaxDisplayType?> GetUserDefaultTaxDisplayTypeAsync(User user);

        /// <summary>
        /// Get full name
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user full name
        /// </returns>
        Task<string> GetUserFullNameAsync(User user);

        /// <summary>
        /// Formats the user name
        /// </summary>
        /// <param name="user">Source</param>
        /// <param name="stripTooLong">Strip too long user name</param>
        /// <param name="maxLength">Maximum user name length</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the formatted text
        /// </returns>
        Task<string> FormatUsernameAsync(User user, bool stripTooLong = false, int maxLength = 0);

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the coupon codes
        /// </returns>
        Task<string[]> ParseAppliedDiscountCouponCodesAsync(User user);

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the new coupon codes document
        /// </returns>
        Task ApplyDiscountCouponCodeAsync(User user, string couponCode);

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the new coupon codes document
        /// </returns>
        Task RemoveDiscountCouponCodeAsync(User user, string couponCode);

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the coupon codes
        /// </returns>
        Task<string[]> ParseAppliedGiftCardCouponCodesAsync(User user);

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the new coupon codes document
        /// </returns>
        Task ApplyGiftCardCouponCodeAsync(User user, string couponCode);

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the new coupon codes document
        /// </returns>
        Task RemoveGiftCardCouponCodeAsync(User user, string couponCode);

        /// <summary>
        /// Returns a list of guids of not existing users
        /// </summary>
        /// <param name="guids">The guids of the users to check</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of guids not existing users
        /// </returns>
        Task<Guid[]> GetNotExistingUsersAsync(Guid[] guids);

        #endregion

        #region User roles

        /// <summary>
        /// Add a user-user role mapping
        /// </summary>
        /// <param name="roleMapping">User-user role mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddUserRoleMappingAsync(UserUserRoleMapping roleMapping);

        /// <summary>
        /// Добавить пользователю маппинг телеканалов
        /// </summary>
        /// <param name="userChannelMapping">Пользовательский маппинг телеканалов</param>
        /// <returns>Задача предоставляет асинхронные операции</returns>
        Task AddUserChannelMappingAsync(UserChannelMapping userChannelMapping);

        /// <summary>
        /// Добпвить пользователю маппинг телеканалов
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="listChannelId">Список идентификаторов телеканалов</param>
        /// <returns>Задача, которая предоставляет асинхронные операции</returns>
        Task AddUserChannelMappingAsync(User user, IList<int> listChannelId);
        /// <summary>
        /// Remove a user-user role mapping
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="role">User role</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task RemoveUserRoleMappingAsync(User user, UserRole role);

        /// <summary>
        /// Удалить маппинг телеканалов для пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Задача, которая представляет асинхронные операции</returns>
        Task RemoveUserChannelMappingAsync(User user);
        /// <summary>
        /// Delete a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteUserRoleAsync(UserRole userRole);

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="userRoleId">User role identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user role
        /// </returns>
        Task<UserRole> GetUserRoleByIdAsync(int userRoleId);

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="systemName">User role system name</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user role
        /// </returns>
        Task<UserRole> GetUserRoleBySystemNameAsync(string systemName);

        /// <summary>
        /// Get user role identifiers
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user role identifiers
        /// </returns>
        Task<int[]> GetUserRoleIdsAsync(User user, bool showHidden = false);

        /// <summary>
        /// Gets list of user roles
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<IList<UserRole>> GetUserRolesAsync(User user, bool showHidden = false);

        /// <summary>
        /// Gets all user roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user roles
        /// </returns>
        Task<IList<UserRole>> GetAllUserRolesAsync(bool showHidden = false);

        /// <summary>
        /// Inserts a user role
        /// </summary>
        /// <param name="userRole">User role</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertUserRoleAsync(UserRole userRole);

        /// <summary>
        /// Gets a value indicating whether user is in a certain user role
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="userRoleSystemName">User role system name</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> IsInUserRoleAsync(User user, string userRoleSystemName, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether user is administrator
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> IsAdminAsync(User user, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether user is a forum moderator
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> IsForumModeratorAsync(User user, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether user is registered
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> IsRegisteredAsync(User user, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether user is guest
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> IsGuestAsync(User user, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether user is vendor
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> IsVendorAsync(User user, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Updates the user role
        /// </summary>
        /// <param name="userRole">User role</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateUserRoleAsync(UserRole userRole);

        #endregion

        #region User passwords

        /// <summary>
        /// Gets user passwords
        /// </summary>
        /// <param name="userId">User identifier; pass null to load all records</param>
        /// <param name="passwordFormat">Password format; pass null to load all records</param>
        /// <param name="passwordsToReturn">Number of returning passwords; pass null to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of user passwords
        /// </returns>
        Task<IList<UserPassword>> GetUserPasswordsAsync(int? userId = null,
            PasswordFormat? passwordFormat = null, int? passwordsToReturn = null);

        /// <summary>
        /// Get current user password
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user password
        /// </returns>
        Task<UserPassword> GetCurrentPasswordAsync(int userId);

        /// <summary>
        /// Insert a user password
        /// </summary>
        /// <param name="userPassword">User password</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertUserPasswordAsync(UserPassword userPassword);

        /// <summary>
        /// Update a user password
        /// </summary>
        /// <param name="userPassword">User password</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateUserPasswordAsync(UserPassword userPassword);

        /// <summary>
        /// Check whether password recovery token is valid
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="token">Token to validate</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPasswordRecoveryTokenValidAsync(User user, string token);

        /// <summary>
        /// Check whether password recovery link is expired
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPasswordRecoveryLinkExpiredAsync(User user);

        /// <summary>
        /// Check whether user password is expired 
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the rue if password is expired; otherwise false
        /// </returns>
        Task<bool> IsPasswordExpiredAsync(User user);

        #endregion

        #region User address mapping

        /// <summary>
        /// Gets a list of addresses mapped to user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the 
        /// </returns>
        Task<IList<Address>> GetAddressesByUserIdAsync(int userId);

        /// <summary>
        /// Gets a address mapped to user
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="addressId">Address identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<Address> GetUserAddressAsync(int userId, int addressId);

        /// <summary>
        /// Gets a user billing address
        /// </summary>
        /// <param name="user">User identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<Address> GetUserBillingAddressAsync(User user);

        /// <summary>
        /// Gets a user shipping address
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<Address> GetUserShippingAddressAsync(User user);

        /// <summary>
        /// Remove a user-address mapping record
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="address">Address</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task RemoveUserAddressAsync(User user, Address address);

        /// <summary>
        /// Inserts a user-address mapping record
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="address">Address</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertUserAddressAsync(User user, Address address);

        #endregion

        #region User GreenData info

        /// <summary>
        /// Добавить информацию о количестве срабатываний операций для пользователя GreenData
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="greenDataOperation">Операция для GreenData</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddUserGreenDataOperationAsync(User user, string greenDataOperation);

        /// <summary>
        /// Проверка операции на активность
        /// </summary>
        /// <param name="greenDataOperation">Операция для GreenData</param>
        /// <returns></returns>
        Task<bool> GetOperationActiveStatus(string greenDataOperation);
        #endregion
    }
}