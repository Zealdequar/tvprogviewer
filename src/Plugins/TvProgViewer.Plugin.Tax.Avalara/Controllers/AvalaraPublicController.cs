using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core;
using TvProgViewer.Plugin.Tax.Avalara.Models.User;
using TvProgViewer.Plugin.Tax.Avalara.Services;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Tax;
using TvProgViewer.WebUI.Controllers;
using TvProgViewer.Web.Framework.Mvc.Filters;

namespace TvProgViewer.Plugin.Tax.Avalara.Controllers
{
    public class AvalaraPublicController : BasePublicController
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly AvalaraTaxSettings _avalaraTaxSettings;
        private readonly IUserService _userService;
        private readonly ITaxPluginManager _taxPluginManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AvalaraPublicController(AvalaraTaxManager avalaraTaxManager,
            AvalaraTaxSettings avalaraTaxSettings,
            IUserService userService,
            ITaxPluginManager taxPluginManager,
            IWorkContext workContext)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _avalaraTaxSettings = avalaraTaxSettings;
            _userService = userService;
            _taxPluginManager = taxPluginManager;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> ExemptionCertificates()
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, user))
                return RedirectToRoute("UserInfo");

            if (!_avalaraTaxSettings.EnableCertificates)
                return RedirectToRoute("UserInfo");

            //ACL
            if (_avalaraTaxSettings.UserRoleIds.Any())
            {
                var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
                if (!userRoleIds.Intersect(_avalaraTaxSettings.UserRoleIds).Any())
                    return RedirectToRoute("UserInfo");
            }

            var token = await _avalaraTaxManager.CreateTokenAsync(user);
            var link = await _avalaraTaxManager.GetInvitationAsync(user) ?? AvalaraTaxDefaults.CertExpressUrl;
            var certificates = await _avalaraTaxManager.GetUserCertificatesAsync(user);
            var model = new TaxExemptionModel
            {
                Token = token,
                Link = link,
                UserId = user.Id,
                Certificates = certificates?.Select(certificate => new ExemptionCertificateModel
                {
                    Id = certificate.id ?? 0,
                    Status = certificate.status,
                    SignedDate = certificate.signedDate.ToShortDateString(),
                    ExpirationDate = certificate.expirationDate.ToShortDateString(),
                    ExposureZone = certificate.exposureZone?.name
                }).ToList() ?? new List<ExemptionCertificateModel>()
            };

            model.AvailableExposureZones = (await _avalaraTaxManager.GetExposureZonesAsync())
                .Select(zone => new SelectListItem(zone.name, zone.name))
                .ToList();

            return View("~/Plugins/Tax.Avalara/Views/User/ExemptionCertificates.cshtml", model);
        }

        [CheckLanguageSeoCode(ignore: true)]
        public async Task<IActionResult> DownloadCertificate(int id)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, user))
                return RedirectToRoute("UserInfo");

            if (!_avalaraTaxSettings.EnableCertificates)
                return RedirectToRoute("UserInfo");

            //ACL
            if (_avalaraTaxSettings.UserRoleIds.Any())
            {
                var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
                if (!userRoleIds.Intersect(_avalaraTaxSettings.UserRoleIds).Any())
                    return RedirectToRoute("UserInfo");
            }

            //try to get a file by the identifier
            var file = await _avalaraTaxManager.DownloadCertificateAsync(id);
            if (file is null)
                return InvokeHttp404();

            return File(file.Data, file.ContentType, file.Filename?.Split(';')?.FirstOrDefault() ?? "certificate");
        }

        #endregion
    }
}