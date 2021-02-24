using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Security;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Users;
using TVProgViewer.WebUI.Areas.Admin.Models.Security;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the security model factory implementation
    /// </summary>
    public partial class SecurityModelFactory : ISecurityModelFactory
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public SecurityModelFactory(IUserService userService,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            _userService = userService;
            _localizationService = localizationService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare permission mapping model
        /// </summary>
        /// <param name="model">Permission mapping model</param>
        /// <returns>Permission mapping model</returns>
        public virtual async Task<PermissionMappingModel> PreparePermissionMappingModelAsync(PermissionMappingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var userRoles = await _userService.GetAllUserRolesAsync(true);
            model.AvailableUserRoles = userRoles.Select(role => role.ToModel<UserRoleModel>()).ToList();

            foreach (var permissionRecord in await _permissionService.GetAllPermissionRecordsAsync())
            {
                model.AvailablePermissions.Add(new PermissionRecordModel
                {
                    Name = await _localizationService.GetLocalizedPermissionNameAsync(permissionRecord),
                    SystemName = permissionRecord.SystemName
                });

                foreach (var role in userRoles)
                {
                    if (!model.Allowed.ContainsKey(permissionRecord.SystemName))
                        model.Allowed[permissionRecord.SystemName] = new Dictionary<int, bool>();
                    model.Allowed[permissionRecord.SystemName][role.Id] =
                        (await _permissionService.GetMappingByPermissionRecordIdAsync(permissionRecord.Id)).Any(mapping => mapping.UserRoleId == role.Id);
                }
            }

            return model;
        }

        #endregion
    }
}