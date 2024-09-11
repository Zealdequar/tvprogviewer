using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Discounts;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Seo;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Extensions;
using TvProgViewer.Web.Framework.Factories;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the category model factory implementation
    /// </summary>
    public partial class CategoryModelFactory : ICategoryModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly IDiscountService _discountService;
        private readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly ITvChannelService _tvChannelService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public CategoryModelFactory(CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            ICurrencyService currencyService,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICategoryService categoryService,
            IDiscountService discountService,
            IDiscountSupportedModelFactory discountSupportedModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            ITvChannelService tvChannelService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IUrlRecordService urlRecordService)
        {
            _catalogSettings = catalogSettings;
            _currencySettings = currencySettings;
            _currencyService = currencyService;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _baseAdminModelFactory = baseAdminModelFactory;
            _categoryService = categoryService;
            _discountService = discountService;
            _discountSupportedModelFactory = discountSupportedModelFactory;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _tvChannelService = tvChannelService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare category tvChannel search model
        /// </summary>
        /// <param name="searchModel">Category tvChannel search model</param>
        /// <param name="category">Category</param>
        /// <returns>Category tvChannel search model</returns>
        protected virtual CategoryTvChannelSearchModel PrepareCategoryTvChannelSearchModel(CategoryTvChannelSearchModel searchModel, Category category)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (category == null)
                throw new ArgumentNullException(nameof(category));

            searchModel.CategoryId = category.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare category search model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category search model
        /// </returns>
        public virtual async Task<CategorySearchModel> PrepareCategorySearchModelAsync(CategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.Categories.List.SearchPublished.All")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.Categories.List.SearchPublished.PublishedOnly")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.Categories.List.SearchPublished.UnpublishedOnly")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged category list model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category list model
        /// </returns>
        public virtual async Task<CategoryListModel> PrepareCategoryListModelAsync(CategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            //get categories
            var categories = await _categoryService.GetAllCategoriesAsync(categoryName: searchModel.SearchCategoryName,
                showHidden: true,
                storeId: searchModel.SearchStoreId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
                overridePublished: searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1));

            //prepare grid model
            var model = await new CategoryListModel().PrepareToGridAsync(searchModel, categories, () =>
            {
                return categories.SelectAwait(async category =>
                {
                    //fill in model values from the entity
                    var categoryModel = category.ToModel<CategoryModel>();

                    //fill in additional values (not existing in the entity)
                    categoryModel.Breadcrumb = await _categoryService.GetFormattedBreadCrumbAsync(category);
                    categoryModel.SeName = await _urlRecordService.GetSeNameAsync(category, 0, true, false);

                    return categoryModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="model">Category model</param>
        /// <param name="category">Category</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category model
        /// </returns>
        public virtual async Task<CategoryModel> PrepareCategoryModelAsync(CategoryModel model, Category category, bool excludeProperties = false)
        {
            Func<CategoryLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (category != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = category.ToModel<CategoryModel>();
                    model.SeName = await _urlRecordService.GetSeNameAsync(category, 0, true, false);
                }

                //prepare nested search model
                PrepareCategoryTvChannelSearchModel(model.CategoryTvChannelSearchModel, category);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(category, entity => entity.Name, languageId, false, false);
                    locale.Description = await _localizationService.GetLocalizedAsync(category, entity => entity.Description, languageId, false, false);
                    locale.MetaKeywords = await _localizationService.GetLocalizedAsync(category, entity => entity.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = await _localizationService.GetLocalizedAsync(category, entity => entity.MetaDescription, languageId, false, false);
                    locale.MetaTitle = await _localizationService.GetLocalizedAsync(category, entity => entity.MetaTitle, languageId, false, false);
                    locale.SeName = await _urlRecordService.GetSeNameAsync(category, languageId, false, false);
                };
            }

            //set default values for the new model
            if (category == null)
            {
                model.PageSize = _catalogSettings.DefaultCategoryPageSize;
                model.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
                model.Published = true;
                model.IncludeInTopMenu = true;
                model.AllowUsersToSelectPageSize = true;
                model.PriceRangeFiltering = true;
                model.ManuallyPriceRange = true;
                model.PriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom;
                model.PriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo;
            }

            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare available category templates
            await _baseAdminModelFactory.PrepareCategoryTemplatesAsync(model.AvailableCategoryTemplates, false);

            //prepare available parent categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(model.AvailableCategories,
                defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Fields.Parent.None"));

            //prepare model discounts
            var availableDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true, isActive: null);
            await _discountSupportedModelFactory.PrepareModelDiscountsAsync(model, category, availableDiscounts, excludeProperties);

            //prepare model user roles
            await _aclSupportedModelFactory.PrepareModelUserRolesAsync(model, category, excludeProperties);

            //prepare model stores
            await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, category, excludeProperties);

            return model;
        }

        /// <summary>
        /// Prepare paged category tvChannel list model
        /// </summary>
        /// <param name="searchModel">Category tvChannel search model</param>
        /// <param name="category">Category</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category tvChannel list model
        /// </returns>
        public virtual async Task<CategoryTvChannelListModel> PrepareCategoryTvChannelListModelAsync(CategoryTvChannelSearchModel searchModel, Category category)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (category == null)
                throw new ArgumentNullException(nameof(category));

            //get tvChannel categories
            var tvChannelCategories = await _categoryService.GetTvChannelCategoriesByCategoryIdAsync(category.Id,
                showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new CategoryTvChannelListModel().PrepareToGridAsync(searchModel, tvChannelCategories, () =>
            {
                return tvChannelCategories.SelectAwait(async tvChannelCategory =>
                {
                    //fill in model values from the entity
                    var categoryTvChannelModel = tvChannelCategory.ToModel<CategoryTvChannelModel>();

                    //fill in additional values (not existing in the entity)
                    categoryTvChannelModel.TvChannelName = (await _tvChannelService.GetTvChannelByIdAsync(tvChannelCategory.TvChannelId))?.Name;

                    return categoryTvChannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvChannel search model to add to the category
        /// </summary>
        /// <param name="searchModel">TvChannel search model to add to the category</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel search model to add to the category
        /// </returns>
        public virtual async Task<AddTvChannelToCategorySearchModel> PrepareAddTvChannelToCategorySearchModelAsync(AddTvChannelToCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available tvChannel types
            await _baseAdminModelFactory.PrepareTvChannelTypesAsync(searchModel.AvailableTvChannelTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged tvChannel list model to add to the category
        /// </summary>
        /// <param name="searchModel">TvChannel search model to add to the category</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel list model to add to the category
        /// </returns>
        public virtual async Task<AddTvChannelToCategoryListModel> PrepareAddTvChannelToCategoryListModelAsync(AddTvChannelToCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tvChannels
            var tvChannels = await _tvChannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                tvChannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddTvChannelToCategoryListModel().PrepareToGridAsync(searchModel, tvChannels, () =>
            {
                return tvChannels.SelectAwait(async tvChannel =>
                {
                    var tvChannelModel = tvChannel.ToModel<TvChannelModel>();

                    tvChannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvChannel, 0, true, false);

                    return tvChannelModel;
                });
            });

            return model;
        }

        #endregion
    }
}