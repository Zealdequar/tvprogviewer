﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
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
    public partial class CategoryController : BaseAdminController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICategoryModelFactory _categoryModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly IDiscountService _discountService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly ITvChannelService _tvChannelService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CategoryController(IAclService aclService,
            ICategoryModelFactory categoryModelFactory,
            ICategoryService categoryService,
            IUserActivityService userActivityService,
            IUserService userService,
            IDiscountService discountService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            ITvChannelService tvChannelService,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _categoryModelFactory = categoryModelFactory;
            _categoryService = categoryService;
            _userActivityService = userActivityService;
            _userService = userService;
            _discountService = discountService;
            _exportManager = exportManager;
            _importManager = importManager;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _tvChannelService = tvChannelService;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(Category category, CategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(category,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(category, localized.SeName, localized.Name, false);
                await _urlRecordService.SaveSlugAsync(category, seName, localized.LanguageId);
            }
        }

        protected virtual async Task UpdatePictureSeoNamesAsync(Category category)
        {
            var picture = await _pictureService.GetPictureByIdAsync(category.PictureId);
            if (picture != null)
                await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(category.Name));
        }

        protected virtual async Task SaveCategoryAclAsync(Category category, CategoryModel model)
        {
            category.SubjectToAcl = model.SelectedUserRoleIds.Any();
            await _categoryService.UpdateCategoryAsync(category);

            var existingAclRecords = await _aclService.GetAclRecordsAsync(category);
            var allUserRoles = await _userService.GetAllUserRolesAsync(true);
            foreach (var userRole in allUserRoles)
            {
                if (model.SelectedUserRoleIds.Contains(userRole.Id))
                {
                    //new role
                    if (!existingAclRecords.Any(acl => acl.UserRoleId == userRole.Id))
                        await _aclService.InsertAclRecordAsync(category, userRole.Id);
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

        protected virtual async Task SaveStoreMappingsAsync(Category category, CategoryModel model)
        {
            category.LimitedToStores = model.SelectedStoreIds.Any();
            await _categoryService.UpdateCategoryAsync(category);

            var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(category);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await _storeMappingService.InsertStoreMappingAsync(category, store.Id);
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
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //prepare model
            var model = await _categoryModelFactory.PrepareCategorySearchModelAsync(new CategorySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(CategorySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _categoryModelFactory.PrepareCategoryListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //prepare model
            var model = await _categoryModelFactory.PrepareCategoryModelAsync(new CategoryModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CategoryModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var category = model.ToEntity<Category>();
                category.CreatedOnUtc = DateTime.UtcNow;
                category.UpdatedOnUtc = DateTime.UtcNow;
                await _categoryService.InsertCategoryAsync(category);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(category, model.SeName, category.Name, true);
                await _urlRecordService.SaveSlugAsync(category, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(category, model);

                //discounts
                var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true, isActive: null);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        await _categoryService.InsertDiscountCategoryMappingAsync(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });
                }

                await _categoryService.UpdateCategoryAsync(category);

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(category);

                //ACL (user roles)
                await SaveCategoryAclAsync(category, model);

                //stores
                await SaveStoreMappingsAsync(category, model);

                //activity log
                await _userActivityService.InsertActivityAsync("AddNewCategory",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCategory"), category.Name), category);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = category.Id });
            }

            //prepare model
            model = await _categoryModelFactory.PrepareCategoryModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null || category.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = await _categoryModelFactory.PrepareCategoryModelAsync(null, category);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(CategoryModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = await _categoryService.GetCategoryByIdAsync(model.Id);
            if (category == null || category.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = category.PictureId;

                //if parent category changes, we need to clear cache for previous parent category
                if (category.ParentCategoryId != model.ParentCategoryId)
                {
                    await _staticCacheManager.RemoveByPrefixAsync(TvProgCatalogDefaults.CategoriesByParentCategoryPrefix, category.ParentCategoryId);
                    await _staticCacheManager.RemoveByPrefixAsync(TvProgCatalogDefaults.CategoriesChildIdsPrefix, category.ParentCategoryId);
                }

                category = model.ToEntity(category);
                category.UpdatedOnUtc = DateTime.UtcNow;
                await _categoryService.UpdateCategoryAsync(category);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(category, model.SeName, category.Name, true);
                await _urlRecordService.SaveSlugAsync(category, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(category, model);

                //discounts
                var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true, isActive: null);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new discount
                        if (await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is null)
                            await _categoryService.InsertDiscountCategoryMappingAsync(new DiscountCategoryMapping { DiscountId = discount.Id, EntityId = category.Id });
                    }
                    else
                    {
                        //remove discount
                        if (await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is DiscountCategoryMapping mapping)
                            await _categoryService.DeleteDiscountCategoryMappingAsync(mapping);
                    }
                }

                await _categoryService.UpdateCategoryAsync(category);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != category.PictureId)
                {
                    var prevPicture = await _pictureService.GetPictureByIdAsync(prevPictureId);
                    if (prevPicture != null)
                        await _pictureService.DeletePictureAsync(prevPicture);
                }

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(category);

                //ACL
                await SaveCategoryAclAsync(category, model);

                //stores
                await SaveStoreMappingsAsync(category, model);

                //activity log
                await _userActivityService.InsertActivityAsync("EditCategory",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCategory"), category.Name), category);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = category.Id });
            }

            //prepare model
            model = await _categoryModelFactory.PrepareCategoryModelAsync(model, category, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return RedirectToAction("List");

            await _categoryService.DeleteCategoryAsync(category);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteCategory",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCategory"), category.Name), category);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            await _categoryService.DeleteCategoriesAsync(await (await _categoryService.GetCategoriesByIdsAsync(selectedIds.ToArray())).WhereAwait(async p => await _workContext.GetCurrentVendorAsync() == null).ToListAsync());

            return Json(new { Result = true });
        }

        #endregion

        #region Export / Import

        public virtual async Task<IActionResult> ExportXml()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            try
            {
                var xml = await _exportManager.ExportCategoriesToXmlAsync();

                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "categories.xml");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        public virtual async Task<IActionResult> ExportXlsx()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            try
            {
                var bytes = await _exportManager
                    .ExportCategoriesToXlsxAsync((await _categoryService.GetAllCategoriesAsync(showHidden: true)).ToList());

                return File(bytes, MimeTypes.TextXlsx, "categories.xlsx");
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
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //a vendor cannot import categories
            if (await _workContext.GetCurrentVendorAsync() != null)
                return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                {
                    await _importManager.ImportCategoriesFromXlsxAsync(importexcelfile.OpenReadStream());
                }
                else
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Imported"));

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
        public virtual async Task<IActionResult> TvChannelList(CategoryTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            //try to get a category with the specified id
            var category = await _categoryService.GetCategoryByIdAsync(searchModel.CategoryId)
                ?? throw new ArgumentException("No category found with the specified id");

            //prepare model
            var model = await _categoryModelFactory.PrepareCategoryTvChannelListModelAsync(searchModel, category);

            return Json(model);
        }

        public virtual async Task<IActionResult> TvChannelUpdate(CategoryTvChannelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a tvChannel category with the specified id
            var tvChannelCategory = await _categoryService.GetTvChannelCategoryByIdAsync(model.Id)
                ?? throw new ArgumentException("No tvChannel category mapping found with the specified id");

            //fill entity from tvChannel
            tvChannelCategory = model.ToEntity(tvChannelCategory);
            await _categoryService.UpdateTvChannelCategoryAsync(tvChannelCategory);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> TvChannelDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a tvChannel category with the specified id
            var tvChannelCategory = await _categoryService.GetTvChannelCategoryByIdAsync(id)
                ?? throw new ArgumentException("No tvChannel category mapping found with the specified id", nameof(id));

            await _categoryService.DeleteTvChannelCategoryAsync(tvChannelCategory);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> TvChannelAddPopup(int categoryId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //prepare model
            var model = await _categoryModelFactory.PrepareAddTvChannelToCategorySearchModelAsync(new AddTvChannelToCategorySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAddPopupList(AddTvChannelToCategorySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _categoryModelFactory.PrepareAddTvChannelToCategoryListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> TvChannelAddPopup(AddTvChannelToCategoryModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //get selected tvChannels
            var selectedTvChannels = await _tvChannelService.GetTvChannelsByIdsAsync(model.SelectedTvChannelIds.ToArray());
            if (selectedTvChannels.Any())
            {
                var existingTvChannelCategories = await _categoryService.GetTvChannelCategoriesByCategoryIdAsync(model.CategoryId, showHidden: true);
                foreach (var tvChannel in selectedTvChannels)
                {
                    //whether tvChannel category with such parameters already exists
                    if (_categoryService.FindTvChannelCategory(existingTvChannelCategories, tvChannel.Id, model.CategoryId) != null)
                        continue;

                    //insert the new tvChannel category mapping
                    await _categoryService.InsertTvChannelCategoryAsync(new TvChannelCategory
                    {
                        CategoryId = model.CategoryId,
                        TvChannelId = tvChannel.Id,
                        IsFeaturedTvChannel = false,
                        DisplayOrder = 1
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddTvChannelToCategorySearchModel());
        }

        #endregion
    }
}