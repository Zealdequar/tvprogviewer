using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Services.Helpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Logging;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.Security;
using TVProgViewer.Services.Stores;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Messages;
using TVProgViewer.Web.Framework.Controllers;
using TVProgViewer.Web.Framework.Mvc.Filters;

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class CampaignController : BaseAdminController
    {
        #region Fields

        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly ICampaignModelFactory _campaignModelFactory;
        private readonly ICampaignService _campaignService;
        private readonly IUserActivityService _userActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public CampaignController(EmailAccountSettings emailAccountSettings,
            ICampaignModelFactory campaignModelFactory,
            ICampaignService campaignService,
            IUserActivityService userActivityService,
            IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IPermissionService permissionService,
            IStoreContext storeContext,
            IStoreService storeService)
        {
            _emailAccountSettings = emailAccountSettings;
            _campaignModelFactory = campaignModelFactory;
            _campaignService = campaignService;
            _userActivityService = userActivityService;
            _dateTimeHelper = dateTimeHelper;
            _emailAccountService = emailAccountService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _permissionService = permissionService;
            _storeContext = storeContext;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual EmailAccount GetEmailAccount(int emailAccountId)
        {
            return _emailAccountService.GetEmailAccountById(emailAccountId)
                ?? _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId)
                ?? throw new TvProgException("Email account could not be loaded");
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //prepare model
            var model = _campaignModelFactory.PrepareCampaignSearchModel(new CampaignSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(CampaignSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _campaignModelFactory.PrepareCampaignListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //prepare model
            var model = _campaignModelFactory.PrepareCampaignModel(new CampaignModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CampaignModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var campaign = model.ToEntity<Campaign>();

                campaign.CreatedOnUtc = DateTime.UtcNow;
                campaign.DontSendBeforeDateUtc = model.DontSendBeforeDate.HasValue ?
                    (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value) : null;

                _campaignService.InsertCampaign(campaign);

                //activity log
                _userActivityService.InsertActivity("AddNewCampaign",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewCampaign"), campaign.Id), campaign);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Promotions.Campaigns.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = campaign.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = _campaignModelFactory.PrepareCampaignModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = _campaignService.GetCampaignById(id);
            if (campaign == null)
                return RedirectToAction("List");

            //prepare model
            var model = _campaignModelFactory.PrepareCampaignModel(null, campaign);

            return View(model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(CampaignModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = _campaignService.GetCampaignById(model.Id);
            if (campaign == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                campaign = model.ToEntity(campaign);

                campaign.DontSendBeforeDateUtc = model.DontSendBeforeDate.HasValue ?
                    (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value) : null;

                _campaignService.UpdateCampaign(campaign);

                //activity log
                _userActivityService.InsertActivity("EditCampaign",
                    string.Format(_localizationService.GetResource("ActivityLog.EditCampaign"), campaign.Id), campaign);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Promotions.Campaigns.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = campaign.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = _campaignModelFactory.PrepareCampaignModel(model, campaign, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-test-email")]
        public virtual IActionResult SendTestEmail(CampaignModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = _campaignService.GetCampaignById(model.Id);
            if (campaign == null)
                return RedirectToAction("List");

            //prepare model
            model = _campaignModelFactory.PrepareCampaignModel(model, campaign);

            //ensure that the entered email is valid
            if (!CommonHelper.IsValidEmail(model.TestEmail))
            {
                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Common.WrongEmail"));
                return View(model);
            }

            try
            {
                var emailAccount = GetEmailAccount(model.EmailAccountId);
                var subscription = _newsLetterSubscriptionService
                    .GetNewsLetterSubscriptionByEmailAndStoreId(model.TestEmail, _storeContext.CurrentStore.Id);
                if (subscription != null)
                {
                    //there's a subscription. let's use it
                    _campaignService.SendCampaign(campaign, emailAccount, new List<NewsLetterSubscription> { subscription });
                }
                else
                {
                    //no subscription found
                    _campaignService.SendCampaign(campaign, emailAccount, model.TestEmail);
                }

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Promotions.Campaigns.TestEmailSentToUsers"));

                return View(model);
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
            }

            //prepare model
            model = _campaignModelFactory.PrepareCampaignModel(model, campaign, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-mass-email")]
        public virtual IActionResult SendMassEmail(CampaignModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = _campaignService.GetCampaignById(model.Id);
            if (campaign == null)
                return RedirectToAction("List");

            //prepare model
            model = _campaignModelFactory.PrepareCampaignModel(model, campaign);

            try
            {
                var emailAccount = GetEmailAccount(model.EmailAccountId);

                //subscribers of certain store?
                var storeId = _storeService.GetStoreById(campaign.StoreId)?.Id ?? 0;
                var subscriptions = _newsLetterSubscriptionService.GetAllNewsLetterSubscriptions(storeId: storeId,
                    userRoleId: model.UserRoleId,
                    isActive: true);
                var totalEmailsSent = _campaignService.SendCampaign(campaign, emailAccount, subscriptions);

                _notificationService.SuccessNotification(string.Format(_localizationService.GetResource("Admin.Promotions.Campaigns.MassEmailSentToUsers"), totalEmailsSent));

                return View(model);
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
            }

            //prepare model
            model = _campaignModelFactory.PrepareCampaignModel(model, campaign, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = _campaignService.GetCampaignById(id);
            if (campaign == null)
                return RedirectToAction("List");

            _campaignService.DeleteCampaign(campaign);

            //activity log
            _userActivityService.InsertActivity("DeleteCampaign",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteCampaign"), campaign.Id), campaign);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Promotions.Campaigns.Deleted"));

            return RedirectToAction("List");
        }

        #endregion
    }
}