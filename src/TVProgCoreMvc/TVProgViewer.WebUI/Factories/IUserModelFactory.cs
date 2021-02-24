using System.Collections.Generic;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.WebUI.Models.User;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the interface of the user model factory
    /// </summary>
    public partial interface IUserModelFactory
    {
        /// <summary>
        /// Prepare the user info model
        /// </summary>
        /// <param name="model">User info model</param>
        /// <param name="user">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomUserAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <returns>User info model</returns>
        Task<UserInfoModel> PrepareUserInfoModelAsync(UserInfoModel model, User user,
            bool excludeProperties, string overrideCustomUserAttributesXml = "");

        /// <summary>
        /// Prepare the user register model
        /// </summary>
        /// <param name="model">User register model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomUserAttributesXml">Overridden user attributes in XML format; pass null to use CustomUserAttributes of user</param>
        /// <param name="setDefaultValues">Whether to populate model properties by default values</param>
        /// <returns>User register model</returns>
        Task<RegisterModel> PrepareRegisterModelAsync(RegisterModel model, bool excludeProperties,
            string overrideCustomUserAttributesXml = "", bool setDefaultValues = false);

        /// <summary>
        /// Prepare the login model
        /// </summary>
        /// <param name="checkoutAsGuest">Whether to checkout as guest is enabled</param>
        /// <returns>Login model</returns>
        Task<LoginModel> PrepareLoginModelAsync(bool? checkoutAsGuest);

        /// <summary>
        /// Prepare the password recovery model
        /// </summary>
        /// <param name="model">Password recovery model</param>
        /// <returns>Password recovery model</returns>
        Task<PasswordRecoveryModel> PreparePasswordRecoveryModelAsync(PasswordRecoveryModel model);

        /// <summary>
        /// Prepare the register result model
        /// </summary>
        /// <param name="resultId">Value of UserRegistrationType enum</param>
        /// <param name="returnUrl">URL to redirect</param>
        /// <returns>Register result model</returns>
        Task<RegisterResultModel> PrepareRegisterResultModelAsync(int resultId, string returnUrl);

        /// <summary>
        /// Prepare the user navigation model
        /// </summary>
        /// <param name="selectedTabId">Identifier of the selected tab</param>
        /// <returns>User navigation model</returns>
        Task<UserNavigationModel> PrepareUserNavigationModelAsync(int selectedTabId = 0);

        /// <summary>
        /// Prepare the user address list model
        /// </summary>
        /// <returns>User address list model</returns>  
        Task<UserAddressListModel> PrepareUserAddressListModelAsync();

        /// <summary>
        /// Prepare the user downloadable products model
        /// </summary>
        /// <returns>User downloadable products model</returns>
        Task<UserDownloadableProductsModel> PrepareUserDownloadableProductsModelAsync();

        /// <summary>
        /// Prepare the user agreement model
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <param name="product">Product</param>
        /// <returns>User agreement model</returns>
        Task<UserAgreementModel> PrepareUserAgreementModelAsync(OrderItem orderItem, Product product);

        /// <summary>
        /// Prepare the change password model
        /// </summary>
        /// <returns>Change password model</returns>
        Task<ChangePasswordModel> PrepareChangePasswordModelAsync();

        /// <summary>
        /// Prepare the user avatar model
        /// </summary>
        /// <param name="model">User avatar model</param>
        /// <returns>User avatar model</returns>
        Task<UserAvatarModel> PrepareUserAvatarModelAsync(UserAvatarModel model);

        /// <summary>
        /// Prepare the GDPR tools model
        /// </summary>
        /// <returns>GDPR tools model</returns>
        Task<GdprToolsModel> PrepareGdprToolsModelAsync();

        /// <summary>
        /// Prepare the check gift card balance model
        /// </summary>
        /// <returns>check gift card balance model</returns>
        Task<CheckGiftCardBalanceModel> PrepareCheckGiftCardBalanceModelAsync();

        /// <summary>
        /// Prepare the multi-factor authentication model
        /// </summary>
        /// <param name="model">Multi-factor authentication model</param>
        /// <returns>Multi-factor authentication model</returns>
        Task<MultiFactorAuthenticationModel> PrepareMultiFactorAuthenticationModelAsync(MultiFactorAuthenticationModel model);

        /// <summary>
        /// Prepare the multi-factor provider model
        /// </summary>
        /// <param name="providerModel">Multi-factor provider model</param>
        /// <param name="sysName">Multi-factor provider system name</param>
        /// <param name="isLogin">Is login page</param>
        /// <returns>Multi-factor authentication model</returns>
        Task<MultiFactorAuthenticationProviderModel> PrepareMultiFactorAuthenticationProviderModelAsync(MultiFactorAuthenticationProviderModel providerModel, string sysName, bool isLogin = false);
    }
}
