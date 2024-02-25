using System.Threading.Tasks;
using TvProgViewer.WebUI.Areas.Admin.Models.Security;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
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
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the permission mapping model
        /// </returns>
        Task<PermissionMappingModel> PreparePermissionMappingModelAsync(PermissionMappingModel model);
    }
}