﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Topics;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Stores;
using TvProgViewer.Services.Topics;
using TvProgViewer.WebUI.Areas.Admin.Factories;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Topics;
using TvProgViewer.Web.Framework.Mvc.Filters;

namespace TvProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class TopicController : BaseAdminController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly ITopicModelFactory _topicModelFactory;
        private readonly ITopicService _topicService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;


        #endregion Fields

        #region Ctor

        public TopicController(IAclService aclService,
            IUserActivityService userActivityService,
            IUserService userService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ITopicModelFactory topicModelFactory,
            ITopicService topicService,
            IUrlRecordService urlRecordService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _userActivityService = userActivityService;
            _userService = userService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _topicModelFactory = topicModelFactory;
            _topicService = topicService;
            _urlRecordService = urlRecordService;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(Topic topic, TopicModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.Title,
                    localized.Title,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.Body,
                    localized.Body,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(topic, localized.SeName, localized.Title, false);
                await _urlRecordService.SaveSlugAsync(topic, seName, localized.LanguageId);
            }
        }

        protected virtual async Task SaveTopicAclAsync(Topic topic, TopicModel model)
        {
            topic.SubjectToAcl = model.SelectedUserRoleIds.Any();
            await _topicService.UpdateTopicAsync(topic);

            var existingAclRecords = await _aclService.GetAclRecordsAsync(topic);
            var allUserRoles = await _userService.GetAllUserRolesAsync(true);
            foreach (var userRole in allUserRoles)
            {
                if (model.SelectedUserRoleIds.Contains(userRole.Id))
                {
                    //new role
                    if (!existingAclRecords.Any(acl => acl.UserRoleId == userRole.Id))
                        await _aclService.InsertAclRecordAsync(topic, userRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.UserRoleId == userRole.Id);
                    if (aclRecordToDelete != null)
                        await _aclService.DeleteAclRecordAsync(aclRecordToDelete);
                }
            }
        }

        protected virtual async Task SaveStoreMappingsAsync(Topic topic, TopicModel model)
        {
            topic.LimitedToStores = model.SelectedStoreIds.Any();
            await _topicService.UpdateTopicAsync(topic);

            var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(topic);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await _storeMappingService.InsertStoreMappingAsync(topic, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List(bool showtour = false)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //prepare model
            var model = await _topicModelFactory.PrepareTopicSearchModelAsync(new TopicSearchModel());

            //show configuration tour
            if (showtour)
            {
                var user = await _workContext.GetCurrentUserAsync();
                var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, TvProgUserDefaults.HideConfigurationStepsAttribute);
                var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, TvProgUserDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(TopicSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _topicModelFactory.PrepareTopicListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //prepare model
            var model = await _topicModelFactory.PrepareTopicModelAsync(new TopicModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(TopicModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (!model.IsPasswordProtected)
                    model.Password = null;

                var topic = model.ToEntity<Topic>();
                await _topicService.InsertTopicAsync(topic);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(topic, model.SeName, topic.Title ?? topic.SystemName, true);
                await _urlRecordService.SaveSlugAsync(topic, model.SeName, 0);

                //ACL (user roles)
                await SaveTopicAclAsync(topic, model);

                //stores
                await SaveStoreMappingsAsync(topic, model);

                //locales
                await UpdateLocalesAsync(topic, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Topics.Added"));

                //activity log
                await _userActivityService.InsertActivityAsync("AddNewTopic",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewTopic"), topic.Title ?? topic.SystemName), topic);

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = topic.Id });
            }

            //prepare model
            model = await _topicModelFactory.PrepareTopicModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id, bool showtour = false)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //try to get a topic with the specified id
            var topic = await _topicService.GetTopicByIdAsync(id);
            if (topic == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _topicModelFactory.PrepareTopicModelAsync(null, topic);

            //show configuration tour
            if (showtour)
            {
                var user = await _workContext.GetCurrentUserAsync();
                var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, TvProgUserDefaults.HideConfigurationStepsAttribute);
                var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, TvProgUserDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(TopicModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //try to get a topic with the specified id
            var topic = await _topicService.GetTopicByIdAsync(model.Id);
            if (topic == null)
                return RedirectToAction("List");

            if (!model.IsPasswordProtected)
                model.Password = null;

            if (ModelState.IsValid)
            {
                topic = model.ToEntity(topic);
                await _topicService.UpdateTopicAsync(topic);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(topic, model.SeName, topic.Title ?? topic.SystemName, true);
                await _urlRecordService.SaveSlugAsync(topic, model.SeName, 0);

                //ACL (user roles)
                await SaveTopicAclAsync(topic, model);

                //stores
                await SaveStoreMappingsAsync(topic, model);

                //locales
                await UpdateLocalesAsync(topic, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Topics.Updated"));

                //activity log
                await _userActivityService.InsertActivityAsync("EditTopic",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditTopic"), topic.Title ?? topic.SystemName), topic);

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = topic.Id });
            }

            //prepare model
            model = await _topicModelFactory.PrepareTopicModelAsync(model, topic, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            //try to get a topic with the specified id
            var topic = await _topicService.GetTopicByIdAsync(id);
            if (topic == null)
                return RedirectToAction("List");

            await _topicService.DeleteTopicAsync(topic);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Topics.Deleted"));

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteTopic",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteTopic"), topic.Title ?? topic.SystemName), topic);

            return RedirectToAction("List");
        }

        #endregion
    }
}