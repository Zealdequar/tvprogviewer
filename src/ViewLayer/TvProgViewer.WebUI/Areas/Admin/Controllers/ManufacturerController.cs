﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Discounts;
using TvProgViewer.Services.ExportImport;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Stores;
using TvProgViewer.WebUI.Areas.Admin.Factories;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Controllers;
using TvProgViewer.Web.Framework.Mvc;
using TvProgViewer.Web.Framework.Mvc.Filters;

namespace TvProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class ManufacturerController : BaseAdminController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly IDiscountService _discountService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IManufacturerModelFactory _manufacturerModelFactory;
        private readonly IManufacturerService _manufacturerService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly ITvChannelService _tvChannelService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ManufacturerController(IAclService aclService,
            IUserActivityService userActivityService,
            IUserService userService,
            IDiscountService discountService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IManufacturerModelFactory manufacturerModelFactory,
            IManufacturerService manufacturerService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            ITvChannelService tvChannelService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _userActivityService = userActivityService;
            _userService = userService;
            _discountService = discountService;
            _exportManager = exportManager;
            _importManager = importManager;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _manufacturerModelFactory = manufacturerModelFactory;
            _manufacturerService = manufacturerService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _tvChannelService = tvChannelService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(Manufacturer manufacturer, ManufacturerModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(manufacturer,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(manufacturer,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(manufacturer,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(manufacturer,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(manufacturer,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(manufacturer, localized.SeName, localized.Name, false);
                await _urlRecordService.SaveSlugAsync(manufacturer, seName, localized.LanguageId);
            }
        }

        protected virtual async Task UpdatePictureSeoNamesAsync(Manufacturer manufacturer)
        {
            var picture = await _pictureService.GetPictureByIdAsync(manufacturer.PictureId);
            if (picture != null)
                await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(manufacturer.Name));
        }

        protected virtual async Task SaveManufacturerAclAsync(Manufacturer manufacturer, ManufacturerModel model)
        {
            manufacturer.SubjectToAcl = model.SelectedUserRoleIds.Any();
            await _manufacturerService.UpdateManufacturerAsync(manufacturer);

            var existingAclRecords = await _aclService.GetAclRecordsAsync(manufacturer);
            var allUserRoles = await _userService.GetAllUserRolesAsync(true);
            foreach (var userRole in allUserRoles)
            {
                if (model.SelectedUserRoleIds.Contains(userRole.Id))
                {
                    //new role
                    if (!existingAclRecords.Any(acl => acl.UserRoleId == userRole.Id))
                        await _aclService.InsertAclRecordAsync(manufacturer, userRole.Id);
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

        protected virtual async Task SaveStoreMappingsAsync(Manufacturer manufacturer, ManufacturerModel model)
        {
            manufacturer.LimitedToStores = model.SelectedStoreIds.Any();
            await _manufacturerService.UpdateManufacturerAsync(manufacturer);

            var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(manufacturer);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await _storeMappingService.InsertStoreMappingAsync(manufacturer, store.Id);
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

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //prepare model
            var model = await _manufacturerModelFactory.PrepareManufacturerSearchModelAsync(new ManufacturerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ManufacturerSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _manufacturerModelFactory.PrepareManufacturerListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //prepare model
            var model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(new ManufacturerModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(ManufacturerModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var manufacturer = model.ToEntity<Manufacturer>();
                manufacturer.CreatedOnUtc = DateTime.UtcNow;
                manufacturer.UpdatedOnUtc = DateTime.UtcNow;
                await _manufacturerService.InsertManufacturerAsync(manufacturer);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(manufacturer, model.SeName, manufacturer.Name, true);
                await _urlRecordService.SaveSlugAsync(manufacturer, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(manufacturer, model);

                //discounts
                var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToManufacturers, showHidden: true, isActive: null);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        //manufacturer.AppliedDiscounts.Add(discount);
                        await _manufacturerService.InsertDiscountManufacturerMappingAsync(new DiscountManufacturerMapping { EntityId = manufacturer.Id, DiscountId = discount.Id });

                }

                await _manufacturerService.UpdateManufacturerAsync(manufacturer);

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(manufacturer);

                //ACL (user roles)
                await SaveManufacturerAclAsync(manufacturer, model);

                //stores
                await SaveStoreMappingsAsync(manufacturer, model);

                //activity log
                await _userActivityService.InsertActivityAsync("AddNewManufacturer",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewManufacturer"), manufacturer.Name), manufacturer);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = manufacturer.Id });
            }

            //prepare model
            model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
            if (manufacturer == null || manufacturer.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(null, manufacturer);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ManufacturerModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(model.Id);
            if (manufacturer == null || manufacturer.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = manufacturer.PictureId;
                manufacturer = model.ToEntity(manufacturer);
                manufacturer.UpdatedOnUtc = DateTime.UtcNow;
                await _manufacturerService.UpdateManufacturerAsync(manufacturer);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(manufacturer, model.SeName, manufacturer.Name, true);
                await _urlRecordService.SaveSlugAsync(manufacturer, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(manufacturer, model);

                //discounts
                var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToManufacturers, showHidden: true, isActive: null);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new discount
                        if (await _manufacturerService.GetDiscountAppliedToManufacturerAsync(manufacturer.Id, discount.Id) is null)
                            await _manufacturerService.InsertDiscountManufacturerMappingAsync(new DiscountManufacturerMapping { EntityId = manufacturer.Id, DiscountId = discount.Id });
                    }
                    else
                    {
                        //remove discount
                        if (await _manufacturerService.GetDiscountAppliedToManufacturerAsync(manufacturer.Id, discount.Id) is DiscountManufacturerMapping discountManufacturerMapping)
                            await _manufacturerService.DeleteDiscountManufacturerMappingAsync(discountManufacturerMapping);
                    }
                }

                await _manufacturerService.UpdateManufacturerAsync(manufacturer);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != manufacturer.PictureId)
                {
                    var prevPicture = await _pictureService.GetPictureByIdAsync(prevPictureId);
                    if (prevPicture != null)
                        await _pictureService.DeletePictureAsync(prevPicture);
                }

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(manufacturer);

                //ACL
                await SaveManufacturerAclAsync(manufacturer, model);

                //stores
                await SaveStoreMappingsAsync(manufacturer, model);

                //activity log
                await _userActivityService.InsertActivityAsync("EditManufacturer",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditManufacturer"), manufacturer.Name), manufacturer);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = manufacturer.Id });
            }

            //prepare model
            model = await _manufacturerModelFactory.PrepareManufacturerModelAsync(model, manufacturer, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
            if (manufacturer == null)
                return RedirectToAction("List");

            await _manufacturerService.DeleteManufacturerAsync(manufacturer);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteManufacturer",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteManufacturer"), manufacturer.Name), manufacturer);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var manufacturers = await _manufacturerService.GetManufacturersByIdsAsync(selectedIds.ToArray());
            await _manufacturerService.DeleteManufacturersAsync(manufacturers);

            var locale = await _localizationService.GetResourceAsync("ActivityLog.DeleteManufacturer");
            foreach (var manufacturer in manufacturers)
            {
                //activity log
                await _userActivityService.InsertActivityAsync("DeleteManufacturer", string.Format(locale, manufacturer.Name), manufacturer);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Export / Import

        public virtual async Task<IActionResult> ExportXml()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            try
            {
                var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true);
                var xml = await _exportManager.ExportManufacturersToXmlAsync(manufacturers);
                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "manufacturers.xml");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        public virtual async Task<IActionResult> ExportXlsx()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            try
            {
                var bytes = await _exportManager.ExportManufacturersToXlsxAsync((await _manufacturerService.GetAllManufacturersAsync(showHidden: true)).Where(p => !p.Deleted));

                return File(bytes, MimeTypes.TextXlsx, "manufacturers.xlsx");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportFromXlsx(IFormFile importexcelfile)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //a vendor cannot import manufacturers
            if (await _workContext.GetCurrentVendorAsync() != null)
                return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                {
                    await _importManager.ImportManufacturersFromXlsxAsync(importexcelfile.OpenReadStream());
                }
                else
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Manufacturers.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region TvChannels

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelList(ManufacturerTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return await AccessDeniedDataTablesJson();

            //try to get a manufacturer with the specified id
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(searchModel.ManufacturerId)
                ?? throw new ArgumentException("No manufacturer found with the specified id");

            //prepare model
            var model = await _manufacturerModelFactory.PrepareManufacturerTvChannelListModelAsync(searchModel, manufacturer);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelUpdate(ManufacturerTvChannelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a tvChannel manufacturer with the specified id
            var tvChannelManufacturer = await _manufacturerService.GetTvChannelManufacturerByIdAsync(model.Id)
                ?? throw new ArgumentException("No tvChannel manufacturer mapping found with the specified id");

            //fill entity from model
            tvChannelManufacturer = model.ToEntity(tvChannelManufacturer);
            await _manufacturerService.UpdateTvChannelManufacturerAsync(tvChannelManufacturer);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //try to get a tvChannel manufacturer with the specified id
            var tvChannelManufacturer = await _manufacturerService.GetTvChannelManufacturerByIdAsync(id)
                ?? throw new ArgumentException("No tvChannel manufacturer mapping found with the specified id");

            await _manufacturerService.DeleteTvChannelManufacturerAsync(tvChannelManufacturer);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> TvChannelAddPopup(int manufacturerId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //prepare model
            var model = await _manufacturerModelFactory.PrepareAddTvChannelToManufacturerSearchModelAsync(new AddTvChannelToManufacturerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAddPopupList(AddTvChannelToManufacturerSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _manufacturerModelFactory.PrepareAddTvChannelToManufacturerListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> TvChannelAddPopup(AddTvChannelToManufacturerModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //get selected tvChannels
            var selectedTvChannels = await _tvChannelService.GetTvChannelsByIdsAsync(model.SelectedTvChannelIds.ToArray());
            if (selectedTvChannels.Any())
            {
                var existingTvChannelmanufacturers = await _manufacturerService
                    .GetTvChannelManufacturersByManufacturerIdAsync(model.ManufacturerId, showHidden: true);
                foreach (var tvChannel in selectedTvChannels)
                {
                    //whether tvChannel manufacturer with such parameters already exists
                    if (_manufacturerService.FindTvChannelManufacturer(existingTvChannelmanufacturers, tvChannel.Id, model.ManufacturerId) != null)
                        continue;

                    //insert the new tvChannel manufacturer mapping
                    await _manufacturerService.InsertTvChannelManufacturerAsync(new TvChannelManufacturer
                    {
                        ManufacturerId = model.ManufacturerId,
                        TvChannelId = tvChannel.Id,
                        IsFeaturedTvChannel = false,
                        DisplayOrder = 1
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddTvChannelToManufacturerSearchModel());
        }

        #endregion
    }
}