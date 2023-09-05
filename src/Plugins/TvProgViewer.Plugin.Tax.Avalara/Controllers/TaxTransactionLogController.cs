using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Plugin.Tax.Avalara.Models.Log;
using TvProgViewer.Plugin.Tax.Avalara.Services;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Html;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Security;
using TvProgViewer.WebUI.Areas.Admin.Controllers;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.Plugin.Tax.Avalara.Controllers
{
    public class TaxTransactionLogController : BaseAdminController
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly TaxTransactionLogService _taxTransactionLogService;

        #endregion

        #region Ctor

        public TaxTransactionLogController(IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IHtmlFormatter htmlFormatter,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            TaxTransactionLogService taxTransactionLogService)
        {
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _htmlFormatter = htmlFormatter;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _taxTransactionLogService = taxTransactionLogService;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> LogList(TaxTransactionLogSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return await AccessDeniedDataTablesJson();

            //prepare filter parameters
            var createdFromValue = searchModel.CreatedFrom.HasValue
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync())
                : null;
            var createdToValue = searchModel.CreatedTo.HasValue
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1)
                : null;

            //get tax transaction log
            var taxtransactionLog = await _taxTransactionLogService.GetTaxTransactionLogAsync(createdFromUtc: createdFromValue, createdToUtc: createdToValue,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new TaxTransactionLogListModel().PrepareToGridAsync(searchModel, taxtransactionLog, () =>
            {
                return taxtransactionLog.SelectAwait(async logItem => new TaxTransactionLogModel
                {
                    Id = logItem.Id,
                    StatusCode = logItem.StatusCode,
                    Url = logItem.Url,
                    UserId = logItem.UserId,
                    CreatedDate = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedDateUtc, DateTimeKind.Utc)
                });
            });

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            await _taxTransactionLogService.DeleteTaxTransactionLogAsync(selectedIds.ToArray());

            return Json(new { Result = true });
        }

        public async Task<IActionResult> View(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //try to get log item with the passed identifier
            var logItem = await _taxTransactionLogService.GetTaxTransactionLogByIdAsync(id);
            if (logItem == null)
                return RedirectToAction("Configure", "Avalara");

            var model = new TaxTransactionLogModel
            {
                Id = logItem.Id,
                StatusCode = logItem.StatusCode,
                Url = logItem.Url,
                RequestMessage = _htmlFormatter.FormatText(logItem.RequestMessage, false, true, false, false, false, false),
                ResponseMessage = _htmlFormatter.FormatText(logItem.ResponseMessage, false, true, false, false, false, false),
                UserId = logItem.UserId,
                UserEmail = (await _userService.GetUserByIdAsync(logItem.UserId))?.Email,
                CreatedDate = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedDateUtc, DateTimeKind.Utc)
            };

            return View("~/Plugins/Tax.Avalara/Views/Log/View.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //try to get log item with the passed identifier
            var logItem = await _taxTransactionLogService.GetTaxTransactionLogByIdAsync(id);
            if (logItem != null)
            {
                await _taxTransactionLogService.DeleteTaxTransactionLogAsync(logItem);
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Log.Deleted"));
            }

            return RedirectToAction("Configure", "Avalara");
        }

        public async Task<IActionResult> ClearAll()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            await _taxTransactionLogService.ClearLogAsync();

            return Json(new { Result = true });
        }

        #endregion
    }
}