using System.Threading.Tasks;
using TVProgViewer.WebUI.Areas.Admin.Models.MultiFactorAuthentication;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the multi-factor authentication method model factory
    /// </summary>
    public partial interface IMultiFactorAuthenticationMethodModelFactory
    {
        /// <summary>
        /// Prepare multi-factor authentication method search model
        /// </summary>
        /// <param name="searchModel">Multi-factor authentication method search model</param>
        /// <returns>Multi-factor authentication method search model</returns>
        MultiFactorAuthenticationMethodSearchModel PrepareMultiFactorAuthenticationMethodSearchModel(
            MultiFactorAuthenticationMethodSearchModel searchModel);

        /// <summary>
        /// Prepare paged multi-factor authentication method list model
        /// </summary>
        /// <param name="searchModel">Multi-factor authentication method search model</param>
        /// <returns>Multi-factor authentication method list model</returns>
        Task<MultiFactorAuthenticationMethodListModel> PrepareMultiFactorAuthenticationMethodListModelAsync(
            MultiFactorAuthenticationMethodSearchModel searchModel);
    }
}
