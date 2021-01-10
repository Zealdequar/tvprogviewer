using System;
using System.Collections.Generic;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Tax;

namespace TVProgViewer.Services.Users
{
    /// <summary>
    /// User service interface
    /// </summary>
    public partial interface IUserService
    {
        #region Users

        /// <summary>
        /// Gets all Users
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="UserRoleIds">A list of User role identifiers to filter by (at least one match); pass null or empty list in order to load all Users; </param>
        /// <param name="email">Email; null to load all Users</param>
        /// <param name="username">Username; null to load all Users</param>
        /// <param name="firstName">First name; null to load all Users</param>
        /// <param name="lastName">Last name; null to load all Users</param>
        /// <param name="dayOfBirth">Day of birth; 0 to load all Users</param>
        /// <param name="monthOfBirth">Month of birth; 0 to load all Users</param>
        /// <param name="company">Company; null to load all Users</param>
        /// <param name="phone">Phone; null to load all Users</param>
        /// <param name="zipPostalCode">Phone; null to load all Users</param>
        /// <param name="ipAddress">IP address; null to load all Users</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>Users</returns>
        IPagedList<User> GetAllUsers(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int affiliateId = 0, int vendorId = 0, int[] userRoleIds = null,
            string email = null, string username = null, string firstName = null, string lastName = null,
            int dayOfBirth = 0, int monthOfBirth = 0,
            string company = null, string phone = null, string zipPostalCode = null, string ipAddress = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Gets online Users
        /// </summary>
        /// <param name="lastActivityFromUtc">User last activity date (from)</param>
        /// <param name="UserRoleIds">A list of User role identifiers to filter by (at least one match); pass null or empty list in order to load all Users; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Users</returns>
        IPagedList<User> GetOnlineUsers(DateTime lastActivityFromUtc,
            int[] UserRoleIds, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets Users with shopping carts
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type; pass null to load all records</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="productId">Product identifier; pass null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); pass null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); pass null to load all records</param>
        /// <param name="countryId">Billing country identifier; pass null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Users</returns>
        IPagedList<User> GetUsersWithShoppingCarts(ShoppingCartType? shoppingCartType = null,
            int storeId = 0, int? productId = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null, int? countryId = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets User for shopping cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>Result</returns>
        User GetShoppingCartUser(IList<ShoppingCartItem> shoppingCart);

        /// <summary>
        /// Delete a User
        /// </summary>
        /// <param name="User">User</param>
        void DeleteUser(User User);

        /// <summary>
        /// Gets a User
        /// </summary>
        /// <param name="UserId">User identifier</param>
        /// <returns>A User</returns>
        User GetUserById(int UserId);

        /// <summary>
        /// Get Users by identifiers
        /// </summary>
        /// <param name="UserIds">User identifiers</param>
        /// <returns>Users</returns>
        IList<User> GetUsersByIds(int[] UserIds);

        /// <summary>
        /// Gets a User by GUID
        /// </summary>
        /// <param name="UserGuid">User GUID</param>
        /// <returns>A User</returns>
        User GetUserByGuid(Guid UserGuid);

        Address GetUserAddress(int userId, int addressId);
        IEnumerable<Address> GetAddressesByUserId(int userID);
        Address GetUserShippingAddress(User user);
        Address GetUserBillingAddress(User user);
        void RemoveUserAddress(User user, Address address);
        /// <summary>
        /// Get User by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>User</returns>
        User GetUserByEmail(string email);

        /// <summary>
        /// Get User by system role
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>User</returns>
        User GetUserBySystemName(string systemName);

        /// <summary>
        /// Get User by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User</returns>
        User GetUserByUsername(string username);

        /// <summary>
        /// Insert a guest User
        /// </summary>
        /// <returns>User</returns>
        User InsertGuestUser();

        /// <summary>
        /// Insert a User
        /// </summary>
        /// <param name="User">User</param>
        void InsertUser(User user);


        void InsertUserAddress(User user, Address address);
        /// <summary>
        /// Updates the User
        /// </summary>
        /// <param name="User">User</param>
        void UpdateUser(User User);

        /// <summary>
        /// Reset data required for checkout
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCouponCodes">A value indicating whether to clear coupon code</param>
        /// <param name="clearCheckoutAttributes">A value indicating whether to clear selected checkout attributes</param>
        /// <param name="clearRewardPoints">A value indicating whether to clear "Use reward points" flag</param>
        /// <param name="clearShippingMethod">A value indicating whether to clear selected shipping method</param>
        /// <param name="clearPaymentMethod">A value indicating whether to clear selected payment method</param>
        void ResetCheckoutData(User User, int storeId,
            bool clearCouponCodes = false, bool clearCheckoutAttributes = false,
            bool clearRewardPoints = true, bool clearShippingMethod = true,
            bool clearPaymentMethod = true);

        /// <summary>
        /// Delete guest User records
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="onlyWithoutShoppingCart">A value indicating whether to delete Users only without shopping cart</param>
        /// <returns>Number of deleted Users</returns>
        int DeleteGuestUsers(DateTime? createdFromUtc, DateTime? createdToUtc, bool onlyWithoutShoppingCart);

        /// <summary>
        /// Gets a default tax display type (if configured)
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>Result</returns>
        TaxDisplayType? GetUserDefaultTaxDisplayType(User User);

        /// <summary>
        /// Get full name
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>User full name</returns>
        string GetUserFullName(User User);

        /// <summary>
        /// Formats the User name
        /// </summary>
        /// <param name="User">Source</param>
        /// <param name="stripTooLong">Strip too long User name</param>
        /// <param name="maxLength">Maximum User name length</param>
        /// <returns>Formatted text</returns>
        string FormatUsername(User User, bool stripTooLong = false, int maxLength = 0);

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>Coupon codes</returns>
        string[] ParseAppliedDiscountCouponCodes(User User);

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>New coupon codes document</returns>
        void ApplyDiscountCouponCode(User User, string couponCode);

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>New coupon codes document</returns>
        void RemoveDiscountCouponCode(User User, string couponCode);

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>Coupon codes</returns>
        string[] ParseAppliedGiftCardCouponCodes(User User);

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>New coupon codes document</returns>
        void ApplyGiftCardCouponCode(User User, string couponCode);

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>New coupon codes document</returns>
        void RemoveGiftCardCouponCode(User User, string couponCode);

        #endregion

        #region User roles

        /// <summary>
        /// Add a User-User role mapping
        /// </summary>
        /// <param name="roleMapping">User-User role mapping</param>
        void AddUserRoleMapping(UserUserRoleMapping roleMapping);

        /// <summary>
        /// Remove a User-User role mapping
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="role">User role</param>
        void RemoveUserRoleMapping(User User, UserRole role);

        /// <summary>
        /// Delete a User role
        /// </summary>
        /// <param name="UserRole">User role</param>
        void DeleteUserRole(UserRole UserRole);

        /// <summary>
        /// Gets a User role
        /// </summary>
        /// <param name="UserRoleId">User role identifier</param>
        /// <returns>User role</returns>
        UserRole GetUserRoleById(int UserRoleId);

        /// <summary>
        /// Gets a User role
        /// </summary>
        /// <param name="systemName">User role system name</param>
        /// <returns>User role</returns>
        UserRole GetUserRoleBySystemName(string systemName);

        /// <summary>
        /// Get User role identifiers
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>User role identifiers</returns>
        int[] GetUserRoleIds(User User, bool showHidden = false);

        /// <summary>
        /// Gets list of User roles
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>Result</returns>
        IList<UserRole> GetUserRoles(User User, bool showHidden = false);

        /// <summary>
        /// Gets all User roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>User roles</returns>
        IList<UserRole> GetAllUserRoles(bool showHidden = false);

        /// <summary>
        /// Inserts a User role
        /// </summary>
        /// <param name="UserRole">User role</param>
        void InsertUserRole(UserRole UserRole);

        /// <summary>
        /// Gets a value indicating whether User is in a certain User role
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="UserRoleSystemName">User role system name</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        bool IsInUserRole(User User, string UserRoleSystemName, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether User is administrator
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        bool IsAdmin(User User, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether User is a forum moderator
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        bool IsForumModerator(User User, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether User is registered
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        bool IsRegistered(User User, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether User is guest
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        bool IsGuest(User User, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Gets a value indicating whether User is vendor
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        bool IsVendor(User User, bool onlyActiveUserRoles = true);

        /// <summary>
        /// Updates the User role
        /// </summary>
        /// <param name="UserRole">User role</param>
        void UpdateUserRole(UserRole UserRole);

        #endregion

        #region User passwords

        /// <summary>
        /// Gets User passwords
        /// </summary>
        /// <param name="UserId">User identifier; pass null to load all records</param>
        /// <param name="passwordFormat">Password format; pass null to load all records</param>
        /// <param name="passwordsToReturn">Number of returning passwords; pass null to load all records</param>
        /// <returns>List of User passwords</returns>
        IList<UserPassword> GetUserPasswords(int? UserId = null,
            PasswordFormat? passwordFormat = null, int? passwordsToReturn = null);

        /// <summary>
        /// Get current User password
        /// </summary>
        /// <param name="UserId">User identifier</param>
        /// <returns>User password</returns>
        UserPassword GetCurrentPassword(int UserId);

        /// <summary>
        /// Insert a User password
        /// </summary>
        /// <param name="UserPassword">User password</param>
        void InsertUserPassword(UserPassword UserPassword);

        /// <summary>
        /// Update a User password
        /// </summary>
        /// <param name="UserPassword">User password</param>
        void UpdateUserPassword(UserPassword UserPassword);

        /// <summary>
        /// Check whether password recovery token is valid
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="token">Token to validate</param>
        /// <returns>Result</returns>
        bool IsPasswordRecoveryTokenValid(User User, string token);

        /// <summary>
        /// Check whether password recovery link is expired
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>Result</returns>
        bool IsPasswordRecoveryLinkExpired(User User);

        /// <summary>
        /// Check whether User password is expired 
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>True if password is expired; otherwise false</returns>
        bool PasswordIsExpired(User User);
      

        #endregion
    }
}