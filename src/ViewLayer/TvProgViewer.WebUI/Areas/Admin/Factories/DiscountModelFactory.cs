﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Services;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Discounts;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Seo;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.WebUI.Areas.Admin.Models.Discounts;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the discount model factory implementation
    /// </summary>
    public partial class DiscountModelFactory : IDiscountModelFactory
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDiscountPluginManager _discountPluginManager;
        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITvChannelService _tvChannelService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public DiscountModelFactory(CurrencySettings currencySettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            IDiscountPluginManager discountPluginManager,
            IDiscountService discountService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IOrderService orderService,
            IPriceFormatter priceFormatter,
            ITvChannelService tvChannelService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper)
        {
            _currencySettings = currencySettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _dateTimeHelper = dateTimeHelper;
            _discountPluginManager = discountPluginManager;
            _discountService = discountService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _orderService = orderService;
            _priceFormatter = priceFormatter;
            _tvChannelService = tvChannelService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare discount usage history search model
        /// </summary>
        /// <param name="searchModel">Discount usage history search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount usage history search model</returns>
        protected virtual DiscountUsageHistorySearchModel PrepareDiscountUsageHistorySearchModel(DiscountUsageHistorySearchModel searchModel,
            Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            searchModel.DiscountId = discount.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare discount tvChannel search model
        /// </summary>
        /// <param name="searchModel">Discount tvChannel search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount tvChannel search model</returns>
        protected virtual DiscountTvChannelSearchModel PrepareDiscountTvChannelSearchModel(DiscountTvChannelSearchModel searchModel, Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            searchModel.DiscountId = discount.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare discount category search model
        /// </summary>
        /// <param name="searchModel">Discount category search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount category search model</returns>
        protected virtual DiscountCategorySearchModel PrepareDiscountCategorySearchModel(DiscountCategorySearchModel searchModel, Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            searchModel.DiscountId = discount.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare discount manufacturer search model
        /// </summary>
        /// <param name="searchModel">Discount manufacturer search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>Discount manufacturer search model</returns>
        protected virtual DiscountManufacturerSearchModel PrepareDiscountManufacturerSearchModel(DiscountManufacturerSearchModel searchModel,
            Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            searchModel.DiscountId = discount.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare discount search model
        /// </summary>
        /// <param name="searchModel">Discount search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the discount search model
        /// </returns>
        public virtual async Task<DiscountSearchModel> PrepareDiscountSearchModelAsync(DiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available discount types
            await _baseAdminModelFactory.PrepareDiscountTypesAsync(searchModel.AvailableDiscountTypes);

            //prepare "is active" filter (0 - all; 1 - active only; 2 - inactive only)
            searchModel.AvailableActiveOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.List.IsActive.All")
            });
            searchModel.AvailableActiveOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.List.IsActive.ActiveOnly")
            });
            searchModel.AvailableActiveOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.List.IsActive.InactiveOnly")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged discount list model
        /// </summary>
        /// <param name="searchModel">Discount search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the discount list model
        /// </returns>
        public virtual async Task<DiscountListModel> PrepareDiscountListModelAsync(DiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter discounts
            var discountType = searchModel.SearchDiscountTypeId > 0 ? (DiscountType?)searchModel.SearchDiscountTypeId : null;
            var startDateUtc = searchModel.SearchStartDate.HasValue ?
                (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchStartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()) : null;
            var endDateUtc = searchModel.SearchEndDate.HasValue ?
                (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchEndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1) : null;
            var isActive = searchModel.IsActiveId == 0 ? null : (bool?)(searchModel.IsActiveId == 1);

            //get discounts
            var discounts = (await _discountService.GetAllDiscountsAsync(showHidden: true,
                discountType: discountType,
                couponCode: searchModel.SearchDiscountCouponCode,
                discountName: searchModel.SearchDiscountName,
                startDateUtc: startDateUtc,
                endDateUtc: endDateUtc,
                isActive: isActive)).ToPagedList(searchModel);

            //prepare list model
            var model = await new DiscountListModel().PrepareToGridAsync(searchModel, discounts, () =>
            {
                return discounts.SelectAwait(async discount =>
                {
                    //fill in model values from the entity
                    var discountModel = discount.ToModel<DiscountModel>();

                    //fill in additional values (not existing in the entity)
                    discountModel.DiscountTypeName = await _localizationService.GetLocalizedEnumAsync(discount.DiscountType);
                    discountModel.PrimaryStoreCurrencyCode = (await _currencyService
                        .GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
                    discountModel.TimesUsed = (await _discountService.GetAllDiscountUsageHistoryAsync(discount.Id, pageSize: 1)).TotalCount;

                    return discountModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare discount model
        /// </summary>
        /// <param name="model">Discount model</param>
        /// <param name="discount">Discount</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the discount model
        /// </returns>
        public virtual async Task<DiscountModel> PrepareDiscountModelAsync(DiscountModel model, Discount discount, bool excludeProperties = false)
        {
            if (discount != null)
            {
                //fill in model values from the entity
                model ??= discount.ToModel<DiscountModel>();

                //prepare available discount requirement rules
                var discountRules = await _discountPluginManager.LoadAllPluginsAsync();
                foreach (var discountRule in discountRules)
                {
                    model.AvailableDiscountRequirementRules.Add(new SelectListItem
                    {
                        Text = discountRule.PluginDescriptor.FriendlyName,
                        Value = discountRule.PluginDescriptor.SystemName
                    });
                }

                model.AvailableDiscountRequirementRules.Insert(0, new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.Requirements.DiscountRequirementType.AddGroup"),
                    Value = "AddGroup"
                });

                model.AvailableDiscountRequirementRules.Insert(0, new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Admin.Promotions.Discounts.Requirements.DiscountRequirementType.Select"),
                    Value = string.Empty
                });

                //prepare available requirement groups
                var requirementGroups = (await _discountService.GetAllDiscountRequirementsAsync(discount.Id)).Where(requirement => requirement.IsGroup);
                model.AvailableRequirementGroups = requirementGroups.Select(requirement =>
                    new SelectListItem { Value = requirement.Id.ToString(), Text = requirement.DiscountRequirementRuleSystemName }).ToList();

                //prepare nested search models
                PrepareDiscountUsageHistorySearchModel(model.DiscountUsageHistorySearchModel, discount);
                PrepareDiscountTvChannelSearchModel(model.DiscountTvChannelSearchModel, discount);
                PrepareDiscountCategorySearchModel(model.DiscountCategorySearchModel, discount);
                PrepareDiscountManufacturerSearchModel(model.DiscountManufacturerSearchModel, discount);
            }

            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

            //get URL of discount with coupon code
            if (model.RequiresCouponCode && !string.IsNullOrEmpty(model.CouponCode))
            {
                model.DiscountUrl = QueryHelpers.AddQueryString((_webHelper.GetStoreLocation()).TrimEnd('/'),
                    TvProgDiscountDefaults.DiscountCouponQueryParameter, model.CouponCode);
            }

            //set default values for the new model
            if (discount == null)
            {
                model.IsActive = true;
                model.LimitationTimes = 1;
            }

            return model;
        }

        /// <summary>
        /// Prepare discount requirement rule models
        /// </summary>
        /// <param name="requirements">Collection of discount requirements</param>
        /// <param name="discount">Discount</param>
        /// <param name="groupInteractionType">Interaction type within the group of requirements</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of discount requirement rule models
        /// </returns>
        public virtual async Task<IList<DiscountRequirementRuleModel>> PrepareDiscountRequirementRuleModelsAsync
            (ICollection<DiscountRequirement> requirements, Discount discount, RequirementGroupInteractionType groupInteractionType)
        {
            if (requirements == null)
                throw new ArgumentNullException(nameof(requirements));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            var lastRequirement = requirements.LastOrDefault();

            return await requirements.SelectAwait(async requirement =>
            {
                //set common properties
                var requirementModel = new DiscountRequirementRuleModel
                {
                    DiscountRequirementId = requirement.Id,
                    ParentId = requirement.ParentId,
                    IsGroup = requirement.IsGroup,
                    RuleName = requirement.DiscountRequirementRuleSystemName,
                    IsLastInGroup = lastRequirement == null || lastRequirement.Id == requirement.Id,
                    InteractionType = groupInteractionType.ToString().ToUpperInvariant()
                };

                var interactionType = requirement.InteractionType ?? RequirementGroupInteractionType.And;
                requirementModel.AvailableInteractionTypes = await interactionType.ToSelectListAsync();

                if (requirement.IsGroup)
                {
                    //get child requirements for the group
                    var childRequirements = await _discountService.GetDiscountRequirementsByParentAsync(requirement);

                    requirementModel.ChildRequirements = await PrepareDiscountRequirementRuleModelsAsync(childRequirements, discount, interactionType);

                    return requirementModel;
                }

                //or try to get name and configuration URL for the requirement
                var requirementRule = await _discountPluginManager.LoadPluginBySystemNameAsync(requirement.DiscountRequirementRuleSystemName);
                if (requirementRule == null)
                    return null;

                requirementModel.RuleName = requirementRule.PluginDescriptor.FriendlyName;
                requirementModel
                    .ConfigurationUrl = requirementRule.GetConfigurationUrl(discount.Id, requirement.Id);

                return requirementModel;
            }).ToListAsync();
        }

        /// <summary>
        /// Prepare paged discount usage history list model
        /// </summary>
        /// <param name="searchModel">Discount usage history search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the discount usage history list model
        /// </returns>
        public virtual async Task<DiscountUsageHistoryListModel> PrepareDiscountUsageHistoryListModelAsync(DiscountUsageHistorySearchModel searchModel,
            Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            //get discount usage history
            var history = await _discountService.GetAllDiscountUsageHistoryAsync(discountId: discount.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new DiscountUsageHistoryListModel().PrepareToGridAsync(searchModel, history, () =>
            {
                return history.SelectAwait(async historyEntry =>
                {
                    //fill in model values from the entity
                    var discountUsageHistoryModel = historyEntry.ToModel<DiscountUsageHistoryModel>();

                    //convert dates to the user time
                    discountUsageHistoryModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    var order = await _orderService.GetOrderByIdAsync(historyEntry.OrderId);
                    if (order != null)
                    {
                        discountUsageHistoryModel.OrderTotal = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false);
                        discountUsageHistoryModel.CustomOrderNumber = order.CustomOrderNumber;
                    }

                    return discountUsageHistoryModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged discount tvChannel list model
        /// </summary>
        /// <param name="searchModel">Discount tvChannel search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the discount tvChannel list model
        /// </returns>
        public virtual async Task<DiscountTvChannelListModel> PrepareDiscountTvChannelListModelAsync(DiscountTvChannelSearchModel searchModel, Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            //get tvChannels with applied discount
            var discountTvChannels = await _tvChannelService.GetTvChannelsWithAppliedDiscountAsync(discountId: discount.Id,
                showHidden: false,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new DiscountTvChannelListModel().PrepareToGrid(searchModel, discountTvChannels, () =>
            {
                //fill in model values from the entity
                return discountTvChannels.Select(tvChannel =>
                {
                    var discountTvChannelModel = tvChannel.ToModel<DiscountTvChannelModel>();
                    discountTvChannelModel.TvChannelId = tvChannel.Id;
                    discountTvChannelModel.TvChannelName = tvChannel.Name;

                    return discountTvChannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvChannel search model to add to the discount
        /// </summary>
        /// <param name="searchModel">TvChannel search model to add to the discount</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel search model to add to the discount
        /// </returns>
        public virtual async Task<AddTvChannelToDiscountSearchModel> PrepareAddTvChannelToDiscountSearchModelAsync(AddTvChannelToDiscountSearchModel searchModel)
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
        /// Prepare paged tvChannel list model to add to the discount
        /// </summary>
        /// <param name="searchModel">TvChannel search model to add to the discount</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel list model to add to the discount
        /// </returns>
        public virtual async Task<AddTvChannelToDiscountListModel> PrepareAddTvChannelToDiscountListModelAsync(AddTvChannelToDiscountSearchModel searchModel)
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
            var model = await new AddTvChannelToDiscountListModel().PrepareToGridAsync(searchModel, tvChannels, () =>
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

        /// <summary>
        /// Prepare paged discount category list model
        /// </summary>
        /// <param name="searchModel">Discount category search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the discount category list model
        /// </returns>
        public virtual async Task<DiscountCategoryListModel> PrepareDiscountCategoryListModelAsync(DiscountCategorySearchModel searchModel, Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            //get categories with applied discount
            var discountCategories = await _categoryService.GetCategoriesByAppliedDiscountAsync(discountId: discount.Id,
                showHidden: false,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new DiscountCategoryListModel().PrepareToGridAsync(searchModel, discountCategories, () =>
            {
                //fill in model values from the entity
                return discountCategories.SelectAwait(async category =>
                {
                    var discountCategoryModel = category.ToModel<DiscountCategoryModel>();

                    discountCategoryModel.CategoryName = await _categoryService.GetFormattedBreadCrumbAsync(category);
                    discountCategoryModel.CategoryId = category.Id;

                    return discountCategoryModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare category search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Category search model to add to the discount</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category search model to add to the discount
        /// </returns>
        public virtual Task<AddCategoryToDiscountSearchModel> PrepareAddCategoryToDiscountSearchModelAsync(AddCategoryToDiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged category list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Category search model to add to the discount</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category list model to add to the discount
        /// </returns>
        public virtual async Task<AddCategoryToDiscountListModel> PrepareAddCategoryToDiscountListModelAsync(AddCategoryToDiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get categories
            var categories = await _categoryService.GetAllCategoriesAsync(showHidden: true,
                categoryName: searchModel.SearchCategoryName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddCategoryToDiscountListModel().PrepareToGridAsync(searchModel, categories, () =>
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
        /// Prepare paged discount manufacturer list model
        /// </summary>
        /// <param name="searchModel">Discount manufacturer search model</param>
        /// <param name="discount">Discount</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the discount manufacturer list model
        /// </returns>
        public virtual async Task<DiscountManufacturerListModel> PrepareDiscountManufacturerListModelAsync(DiscountManufacturerSearchModel searchModel,
            Discount discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            //get manufacturers with applied discount
            var discountManufacturers = await _manufacturerService.GetManufacturersWithAppliedDiscountAsync(discountId: discount.Id,
                showHidden: false,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new DiscountManufacturerListModel().PrepareToGrid(searchModel, discountManufacturers, () =>
            {
                //fill in model values from the entity
                return discountManufacturers.Select(manufacturer =>
                {
                    var discountManufacturerModel = manufacturer.ToModel<DiscountManufacturerModel>();
                    discountManufacturerModel.ManufacturerId = manufacturer.Id;
                    discountManufacturerModel.ManufacturerName = manufacturer.Name;

                    return discountManufacturerModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare manufacturer search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Manufacturer search model to add to the discount</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer search model to add to the discount
        /// </returns>
        public virtual Task<AddManufacturerToDiscountSearchModel> PrepareAddManufacturerToDiscountSearchModelAsync(AddManufacturerToDiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged manufacturer list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Manufacturer search model to add to the discount</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer list model to add to the discount
        /// </returns>
        public virtual async Task<AddManufacturerToDiscountListModel> PrepareAddManufacturerToDiscountListModelAsync(AddManufacturerToDiscountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get manufacturers
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true,
                manufacturerName: searchModel.SearchManufacturerName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddManufacturerToDiscountListModel().PrepareToGridAsync(searchModel, manufacturers, () =>
            {
                return manufacturers.SelectAwait(async manufacturer =>
                {
                    var manufacturerModel = manufacturer.ToModel<ManufacturerModel>();
                    manufacturerModel.SeName = await _urlRecordService.GetSeNameAsync(manufacturer, 0, true, false);

                    return manufacturerModel;
                });
            });

            return model;
        }

        #endregion
    }
}