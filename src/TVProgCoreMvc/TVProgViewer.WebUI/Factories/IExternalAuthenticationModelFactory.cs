using System.Collections.Generic;
using System.Threading.Tasks;
using TVProgViewer.WebUI.Models.User;

namespace TVProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the interface of the external authentication model factory
    /// </summary>
    public partial interface IExternalAuthenticationModelFactory
    {
        /// <summary>
        /// Prepare the external authentication method model
        /// </summary>
        /// <returns>List of the external authentication method model</returns>
        Task<List<ExternalAuthenticationMethodModel>> PrepareExternalMethodsModelAsync();
    }
}
