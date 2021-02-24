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
using System.Threading.Tasks;

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

        protected virtual async Task<EmailAccount> GetEmailAccountAsync(int emailAccountId)
        {
            return await _emailAccountService.GetEmailAccountByIdAsync(emailAccountId)
                ?? await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId)
                ?? throw new TvProgException("Email account could not be loaded");
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //prepare model
            var model = await _campaignModelFactory.PrepareCampaignSearchModelAsync(new CampaignSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(CampaignSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCampaigns))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _campaignModelFactory.PrepareCampaignListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //prepare model
            var model = await _campaignModelFactory.PrepareCampaignModelAsync(new CampaignModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CampaignModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var campaign = model.ToEntity<Campaign>();

                campaign.CreatedOnUtc = DateTime.UtcNow;
                campaign.DontSendBeforeDateUtc = model.DontSendBeforeDate.HasValue ?
                    (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value) : null;

                await _campaignService.InsertCampaignAsync(campaign);

                //activity log
                await _userActivityService.InsertActivityAsync("AddNewCampaign",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCampaign"), campaign.Id), campaign);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = campaign.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _campaignModelFactory.PrepareCampaignModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = await _campaignService.GetCampaignByIdAsync(id);
            if (campaign == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _campaignModelFactory.PrepareCampaignModelAsync(null, campaign);

            return View(model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(CampaignModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = await _campaignService.GetCampaignByIdAsync(model.Id);
            if (campaign == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                campaign = model.ToEntity(campaign);

                campaign.DontSendBeforeDateUtc = model.DontSendBeforeDate.HasValue ?
                    (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.DontSendBeforeDate.Value) : null;

                await _campaignService.UpdateCampaignAsync(campaign);

                //activity log
                await _userActivityService.InsertActivityAsync("EditCampaign",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCampaign"), campaign.Id), campaign);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = campaign.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _campaignModelFactory.PrepareCampaignModelAsync(model, campaign, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-test-email")]
        public virtual async Task<IActionResult> SendTestEmail(CampaignModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = await _campaignService.GetCampaignByIdAsync(model.Id);
            if (campaign == null)
                return RedirectToAction("List");

            //prepare model
            model = await _campaignModelFactory.PrepareCampaignModelAsync(model, campaign);

            //ensure that the entered email is valid
            if (!CommonHelper.IsValidEmail(model.TestEmail))
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.WrongEmail"));
                return View(model);
            }

            try
            {
                var emailAccount = await GetEmailAccountAsync(model.EmailAccountId);
                var subscription = await _newsLetterSubscriptionService
                    .GetNewsLetterSubscriptionByEmailAndStoreIdAsync(model.TestEmail, (await _storeContext.GetCurrentStoreAsync()).Id);
                if (subscription != null)
                {
                    //there's a subscription. let's use it
                    await _campaignService.SendCampaignAsync(campaign, emailAccount, new List<NewsLetterSubscription> { subscription });
                }
                else
                {
                    //no subscription found
                    await _campaignService.SendCampaignAsync(campaign, emailAccount, model.TestEmail);
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.TestEmailSentToUsers"));

                return View(model);
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            //prepare model
            model = await _campaignModelFactory.PrepareCampaignModelAsync(model, campaign, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-mass-email")]
        public virtual async Task<IActionResult> SendMassEmail(CampaignModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = await _campaignService.GetCampaignByIdAsync(model.Id);
            if (campaign == null)
                return RedirectToAction("List");

            //prepare model
            model = await _campaignModelFactory.PrepareCampaignModelAsync(model, campaign);

            try
            {
                var emailAccount = await GetEmailAccountAsync(model.EmailAccountId);

                //subscribers of certain store?
                var storeId = (await _storeService.GetStoreByIdAsync(campaign.StoreId))?.Id ?? 0;
                var subscriptions = await _newsLetterSubscriptionService.GetAllNewsLetterSubscriptionsAsync(storeId: storeId,
                    userRoleId: model.UserRoleId,
                    isActive: true);
                var totalEmailsSent = await _campaignService.SendCampaignAsync(campaign, emailAccount, subscriptions);

                _notificationService.SuccessNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.MassEmailSentToUsers"), totalEmailsSent));

                return View(model);
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            //prepare model
            model = await _campaignModelFactory.PrepareCampaignModelAsync(model, campaign, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCampaigns))
                return AccessDeniedView();

            //try to get a campaign with the specified id
            var campaign = await _campaignService.GetCampaignByIdAsync(id);
            if (campaign == null)
                return RedirectToAction("List");

            await _campaignService.DeleteCampaignAsync(campaign);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteCampaign",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCampaign"), campaign.Id), campaign);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Campaigns.Deleted"));

            return RedirectToAction("List");
        }

        #endregion
    }
}