using System.Threading.Tasks;
using TVProgViewer.WebUI.Areas.Admin.Models.Security;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the security model factory
    /// </summary>
    public partial interface ISecurityModelFactory
    {
        /// <summary>
        /// Prepare permission mapping model
        /// </summary>
        /// <param name="model">Permission mapping model</param>
        /// <returns>Permission mapping model</returns>
        Task<PermissionMappingModel> PreparePermissionMappingModelAsync(PermissionMappingModel model);
    }
}