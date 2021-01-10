using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Logging;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.Security;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.WebUI.Areas.Admin.Models.Security;

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class SecurityController : BaseAdminController
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISecurityModelFactory _securityModelFactory;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public SecurityController(IUserService userService,
            ILocalizationService localizationService,
            ILogger logger,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISecurityModelFactory securityModelFactory,
            IWorkContext workContext)
        {
            _userService = userService;
            _localizationService = localizationService;
            _logger = logger;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _securityModelFactory = securityModelFactory;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public virtual IActionResult AccessDenied(string pageUrl)
        {
            var currentUser = _workContext.CurrentUser;
            if (currentUser == null || _userService.IsGuest(currentUser))
            {
                _logger.Information($"Access denied to anonymous request on {pageUrl}");
                return View();
            }

            _logger.Information($"Access denied to user #{currentUser.Email} '{currentUser.Email}' on {pageUrl}");

            return View();
        }

        public virtual IActionResult Permissions()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            //prepare model
            var model = _securityModelFactory.PreparePermissionMappingModel(new PermissionMappingModel());

            return View(model);
        }

        [HttpPost, ActionName("Permissions")]
        public virtual IActionResult PermissionsSave(PermissionMappingModel model, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            var permissionRecords = _permissionService.GetAllPermissionRecords();
            var userRoles = _userService.GetAllUserRoles(true);

            foreach (var cr in userRoles)
            {
                var formKey = "allow_" + cr.Id;
                var permissionRecordSystemNamesToRestrict = !StringValues.IsNullOrEmpty(form[formKey])
                    ? form[formKey].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
                    : new List<string>();

                foreach (var pr in permissionRecords)
                {
                    var allow = permissionRecordSystemNamesToRestrict.Contains(pr.SystemName);

                    if (allow == _permissionService.Authorize(pr.SystemName, cr.Id))
                        continue;

                    if (allow)
                    {
                        _permissionService.InsertPermissionRecordUserRoleMapping(new PermissionRecordUserRoleMapping { PermissionRecordId = pr.Id, UserRoleId = cr.Id });
                    }
                    else
                    {
                        _permissionService.DeletePermissionRecordUserRoleMapping(pr.Id, cr.Id);                        
                    }

                    _permissionService.UpdatePermissionRecord(pr);
                }
            }

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.ACL.Updated"));

            return RedirectToAction("Permissions");
        }

        #endregion
    }
}