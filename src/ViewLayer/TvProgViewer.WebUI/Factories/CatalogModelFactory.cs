using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Events;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Topics;
using TvProgViewer.Services.Vendors;
using TvProgViewer.Web.Framework.Events;
using TvProgViewer.Web.Framework.Mvc.Routing;
using TvProgViewer.WebUI.Infrastructure.Cache;
using TvProgViewer.WebUI.Models.Catalog;
using TvProgViewer.WebUI.Models.Media;

namespace TvProgViewer.WebUI.Factories
{
    public partial class CatalogModelFactory : ICatalogModelFactory
    {
        #region Fields

        private readonly BlogSettings _blogSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly DisplayDefaultMenuItemSettings _displayDefaultMenuItemSettings;
        private readonly ForumSettings _forumSettings;
        private readonly ICategoryService _categoryService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserService _userService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly ITvProgUrlHelper _nopUrlHelper;
        private readonly IPictureService _pictureService;
        private readonly ITvChannelModelFactory _tvchannelModelFactory;
        private readonly ITvChannelService _tvchannelService;
        private readonly ITvChannelTagService _tvchannelTagService;
        private readonly ISearchTermService _searchTermService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ITopicService _topicService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public CatalogModelFactory(BlogSettings blogSettings,
            CatalogSettings catalogSettings,
            DisplayDefaultMenuItemSettings displayDefaultMenuItemSettings,
            ForumSettings forumSettings,
            ICategoryService categoryService,
            ICategoryTemplateService categoryTemplateService,
            ICurrencyService currencyService,
            IUserService userService,
            IEventPublisher eventPublisher,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IManufacturerTemplateService manufacturerTemplateService,
            ITvProgUrlHelper nopUrlHelper,
            IPictureService pictureService,
            ITvChannelModelFactory tvchannelModelFactory,
            ITvChannelService tvchannelService,
            ITvChannelTagService tvchannelTagService,
            ISearchTermService searchTermService,
            ISpecificationAttributeService specificationAttributeService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ITopicService topicService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings)
        {
            _blogSettings = blogSettings;
            _catalogSettings = catalogSettings;
            _displayDefaultMenuItemSettings = displayDefaultMenuItemSettings;
            _forumSettings = forumSettings;
            _categoryService = categoryService;
            _categoryTemplateService = categoryTemplateService;
            _currencyService = currencyService;
            _userService = userService;
            _eventPublisher = eventPublisher;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _manufacturerTemplateService = manufacturerTemplateService;
            _nopUrlHelper = nopUrlHelper;
            _pictureService = pictureService;
            _tvchannelModelFactory = tvchannelModelFactory;
            _tvchannelService = tvchannelService;
            _tvchannelTagService = tvchannelTagService;
            _searchTermService = searchTermService;
            _specificationAttributeService = specificationAttributeService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _topicService = topicService;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        protected virtual CategorySimpleModel GetCategorySimpleModel(XElement elem)
        {
            var model = new CategorySimpleModel
            {
                Id = int.Parse(elem.XPathSelectElement("Id").Value),
                Name = elem.XPathSelectElement("Name").Value,
                SeName = elem.XPathSelectElement("SeName").Value,

                NumberOfTvChannels = !string.IsNullOrEmpty(elem.XPathSelectElement("NumberOfTvChannels").Value)
                    ? int.Parse(elem.XPathSelectElement("NumberOfTvChannels").Value)
                    : (int?)null,

                IncludeInTopMenu = bool.Parse(elem.XPathSelectElement("IncludeInTopMenu").Value),
                HaveSubCategories = bool.Parse(elem.XPathSelectElement("HaveSubCategories").Value),
                Route = _nopUrlHelper.RouteGenericUrlAsync<Category>(new { SeName = elem.XPathSelectElement("SeName").Value }).Result
            };

            return model;
        }

        /// <summary>
        /// Gets the price range converted to primary store currency
        /// </summary>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the <see cref="Task"/> containing the price range converted to primary store currency
        /// </returns>
        protected virtual async Task<PriceRangeModel> GetConvertedPriceRangeAsync(CatalogTvChannelsCommand command)
        {
            var result = new PriceRangeModel();

            if (string.IsNullOrWhiteSpace(command.Price))
                return result;

            var fromTo = command.Price.Trim().Split(new[] { '-' });
            if (fromTo.Length == 2)
            {
                var rawFromPrice = fromTo[0]?.Trim();
                if (!string.IsNullOrEmpty(rawFromPrice) && decimal.TryParse(rawFromPrice, out var from))
                    result.From = from;

                var rawToPrice = fromTo[1]?.Trim();
                if (!string.IsNullOrEmpty(rawToPrice) && decimal.TryParse(rawToPrice, out var to))
                    result.To = to;

                if (result.From > result.To)
                    result.From = result.To;

                var workingCurrency = await _workContext.GetWorkingCurrencyAsync();

                if (result.From.HasValue)
                    result.From = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(result.From.Value, workingCurrency);

                if (result.To.HasValue)
                    result.To = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(result.To.Value, workingCurrency);
            }

            return result;
        }

        /// <summary>
        /// Prepares the specification filter model
        /// </summary>
        /// <param name="selectedOptions">The selected options to filter the tvchannels</param>
        /// <param name="availableOptions">The available options to filter the tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the specification filter model
        /// </returns>
        protected virtual async Task<SpecificationFilterModel> PrepareSpecificationFilterModel(IList<int> selectedOptions, IList<SpecificationAttributeOption> availableOptions)
        {
            var model = new SpecificationFilterModel();

            if (availableOptions?.Any() == true)
            {
                model.Enabled = true;

                var workingLanguage = await _workContext.GetWorkingLanguageAsync();

                foreach (var option in availableOptions)
                {
                    var attributeFilter = model.Attributes.FirstOrDefault(model => model.Id == option.SpecificationAttributeId);
                    if (attributeFilter == null)
                    {
                        var attribute = await _specificationAttributeService
                            .GetSpecificationAttributeByIdAsync(option.SpecificationAttributeId);
                        attributeFilter = new SpecificationAttributeFilterModel
                        {
                            Id = attribute.Id,
                            Name = await _localizationService
                                .GetLocalizedAsync(attribute, x => x.Name, workingLanguage.Id)
                        };
                        model.Attributes.Add(attributeFilter);
                    }

                    attributeFilter.Values.Add(new SpecificationAttributeValueFilterModel
                    {
                        Id = option.Id,
                        Name = await _localizationService
                            .GetLocalizedAsync(option, x => x.Name, workingLanguage.Id),
                        Selected = selectedOptions?.Any(optionId => optionId == option.Id) == true,
                        ColorSquaresRgb = option.ColorSquaresRgb
                    });
                }
            }

            return model;
        }

        /// <summary>
        /// Prepares the manufacturer filter model
        /// </summary>
        /// <param name="selectedManufacturers">The selected manufacturers to filter the tvchannels</param>
        /// <param name="availableManufacturers">The available manufacturers to filter the tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the specification filter model
        /// </returns>
        protected virtual async Task<ManufacturerFilterModel> PrepareManufacturerFilterModel(IList<int> selectedManufacturers, IList<Manufacturer> availableManufacturers)
        {
            var model = new ManufacturerFilterModel();

            if (availableManufacturers?.Any() == true)
            {
                model.Enabled = true;

                var workingLanguage = await _workContext.GetWorkingLanguageAsync();

                foreach (var manufacturer in availableManufacturers)
                {
                    model.Manufacturers.Add(new SelectListItem
                    {
                        Value = manufacturer.Id.ToString(),
                        Text = await _localizationService
                            .GetLocalizedAsync(manufacturer, x => x.Name, workingLanguage.Id),
                        Selected = selectedManufacturers?
                            .Any(manufacturerId => manufacturerId == manufacturer.Id) == true
                    });
                }
            }

            return model;
        }

        /// <summary>
        /// Prepares the price range filter
        /// </summary>
        /// <param name="selectedPriceRange">The selected price range to filter the tvchannels</param>
        /// <param name="availablePriceRange">The available price range to filter the tvchannels</param>
        /// <returns>The price range filter</returns>
        protected virtual async Task<PriceRangeFilterModel> PreparePriceRangeFilterAsync(PriceRangeModel selectedPriceRange, PriceRangeModel availablePriceRange)
        {
            var model = new PriceRangeFilterModel();

            if (!availablePriceRange.To.HasValue || availablePriceRange.To <= 0
                || availablePriceRange.To == availablePriceRange.From)
            {
                // filter by price isn't available
                selectedPriceRange.From = null;
                selectedPriceRange.To = null;

                return model;
            }

            if (selectedPriceRange.From < availablePriceRange.From)
                selectedPriceRange.From = availablePriceRange.From;

            if (selectedPriceRange.To > availablePriceRange.To)
                selectedPriceRange.To = availablePriceRange.To;

            var workingCurrency = await _workContext.GetWorkingCurrencyAsync();

            Task<decimal> toWorkingCurrencyAsync(decimal? price)
                => _currencyService.ConvertFromPrimaryStoreCurrencyAsync(price.Value, workingCurrency);

            model.Enabled = true;
            model.AvailablePriceRange.From = availablePriceRange.From > decimal.Zero
                ? Math.Floor(await toWorkingCurrencyAsync(availablePriceRange.From))
                : decimal.Zero;
            model.AvailablePriceRange.To = Math.Ceiling(await toWorkingCurrencyAsync(availablePriceRange.To));

            if (!selectedPriceRange.From.HasValue || availablePriceRange.From == selectedPriceRange.From)
            {
                //already converted
                model.SelectedPriceRange.From = model.AvailablePriceRange.From;
            }
            else if (selectedPriceRange.From > decimal.Zero)
                model.SelectedPriceRange.From = Math.Floor(await toWorkingCurrencyAsync(selectedPriceRange.From));

            if (!selectedPriceRange.To.HasValue || availablePriceRange.To == selectedPriceRange.To)
            {
                //already converted
                model.SelectedPriceRange.To = model.AvailablePriceRange.To;
            }
            else if (selectedPriceRange.To > decimal.Zero)
                model.SelectedPriceRange.To = Math.Ceiling(await toWorkingCurrencyAsync(selectedPriceRange.To));

            return model;
        }

        /// <summary>
        /// Prepares catalog tvchannels
        /// </summary>
        /// <param name="model">Catalog tvchannels model</param>
        /// <param name="tvchannels">The tvchannels</param>
        /// <param name="isFiltering">A value indicating that filtering has been applied</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareCatalogTvChannelsAsync(CatalogTvChannelsModel model, IPagedList<TvChannel> tvchannels, bool isFiltering = false)
        {
            if (!string.IsNullOrEmpty(model.WarningMessage))
                return;

            if (tvchannels.Count == 0 && isFiltering)
                model.NoResultMessage = await _localizationService.GetResourceAsync("Catalog.TvChannels.NoResult");
            else
            {
                model.TvChannels = (await _tvchannelModelFactory.PrepareTvChannelOverviewModelsAsync(tvchannels)).ToList();
                model.LoadPagedList(tvchannels);
            }
        }

        #endregion

        #region Categories

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category model
        /// </returns>
        public virtual async Task<CategoryModel> PrepareCategoryModelAsync(Category category, CatalogTvChannelsCommand command)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CategoryModel
            {
                Id = category.Id,
                Name = await _localizationService.GetLocalizedAsync(category, x => x.Name),
                Description = await _localizationService.GetLocalizedAsync(category, x => x.Description),
                MetaKeywords = await _localizationService.GetLocalizedAsync(category, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(category, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(category, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(category),
                CatalogTvChannelsModel = await PrepareCategoryTvChannelsModelAsync(category, command)
            };

            //category breadcrumb
            if (_catalogSettings.CategoryBreadcrumbEnabled)
            {
                model.DisplayCategoryBreadcrumb = true;

                model.CategoryBreadcrumb = await (await _categoryService.GetCategoryBreadCrumbAsync(category)).SelectAwait(async catBr =>
                    new CategoryModel
                    {
                        Id = catBr.Id,
                        Name = await _localizationService.GetLocalizedAsync(catBr, x => x.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(catBr)
                    }).ToListAsync();
            }

            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            //subcategories
            model.SubCategories = await (await _categoryService.GetAllCategoriesByParentCategoryIdAsync(category.Id))
                .SelectAwait(async curCategory =>
                {
                    var subCatModel = new CategoryModel.SubCategoryModel
                    {
                        Id = curCategory.Id,
                        Name = await _localizationService.GetLocalizedAsync(curCategory, y => y.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(curCategory),
                        Description = await _localizationService.GetLocalizedAsync(curCategory, y => y.Description)
                    };

                    //prepare picture model
                    var categoryPictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.CategoryPictureModelKey, curCategory,
                        pictureSize, true, await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(),
                        currentStore);

                    subCatModel.PictureModel = await _staticCacheManager.GetAsync(categoryPictureCacheKey, async () =>
                    {
                        var picture = await _pictureService.GetPictureByIdAsync(curCategory.PictureId);
                        string fullSizeImageUrl, imageUrl;

                        (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                        (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

                        var pictureModel = new PictureModel
                        {
                            FullSizeImageUrl = fullSizeImageUrl,
                            ImageUrl = imageUrl,
                            Title = string.Format(await _localizationService
                                .GetResourceAsync("Media.Category.ImageLinkTitleFormat"), subCatModel.Name),
                            AlternateText = string.Format(await _localizationService
                                .GetResourceAsync("Media.Category.ImageAlternateTextFormat"), subCatModel.Name)
                        };

                        return pictureModel;
                    });

                    return subCatModel;
                }).ToListAsync();

            //featured tvchannels
            if (!_catalogSettings.IgnoreFeaturedTvChannels)
            {
                var featuredTvChannels = await _tvchannelService.GetCategoryFeaturedTvChannelsAsync(category.Id, currentStore.Id);
                if (featuredTvChannels != null)
                    model.FeaturedTvChannels = (await _tvchannelModelFactory.PrepareTvChannelOverviewModelsAsync(featuredTvChannels)).ToList();
            }

            return model;
        }

        /// <summary>
        /// Prepare category template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category template view path
        /// </returns>
        public virtual async Task<string> PrepareCategoryTemplateViewPathAsync(int templateId)
        {
            var template = await _categoryTemplateService.GetCategoryTemplateByIdAsync(templateId) ??
                           (await _categoryTemplateService.GetAllCategoryTemplatesAsync()).FirstOrDefault();

            if (template == null)
                throw new Exception("No default template could be loaded");

            return template.ViewPath;
        }

        /// <summary>
        /// Prepare category navigation model
        /// </summary>
        /// <param name="currentCategoryId">Current category identifier</param>
        /// <param name="currentTvChannelId">Current tvchannel identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category navigation model
        /// </returns>
        public virtual async Task<CategoryNavigationModel> PrepareCategoryNavigationModelAsync(int currentCategoryId, int currentTvChannelId)
        {
            //get active category
            var activeCategoryId = 0;
            if (currentCategoryId > 0)
            {
                //category details page
                activeCategoryId = currentCategoryId;
            }
            else if (currentTvChannelId > 0)
            {
                //tvchannel details page
                var tvchannelCategories = await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(currentTvChannelId);
                if (tvchannelCategories.Any())
                    activeCategoryId = tvchannelCategories[0].CategoryId;
            }

            var cachedCategoriesModel = await PrepareCategorySimpleModelsAsync();
            var model = new CategoryNavigationModel
            {
                CurrentCategoryId = activeCategoryId,
                Categories = cachedCategoriesModel
            };

            return model;
        }

        /// <summary>
        /// Prepare top menu model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the op menu model
        /// </returns>
        public virtual async Task<TopMenuModel> PrepareTopMenuModelAsync()
        {
            var cachedCategoriesModel = new List<CategorySimpleModel>();
            //categories
            if (!_catalogSettings.UseAjaxLoadMenu)
                cachedCategoriesModel = await PrepareCategorySimpleModelsAsync();

            var store = await _storeContext.GetCurrentStoreAsync();

            //top menu topics
            var topicModel = await (await _topicService.GetAllTopicsAsync(store.Id, onlyIncludedInTopMenu: true))
                .SelectAwait(async t => new TopMenuModel.TopicModel
                {
                    Id = t.Id,
                    Name = await _localizationService.GetLocalizedAsync(t, x => x.Title),
                    SeName = await _urlRecordService.GetSeNameAsync(t)
                }).ToListAsync();

            var model = new TopMenuModel
            {
                Categories = cachedCategoriesModel,
                Topics = topicModel,
                NewTvChannelsEnabled = _catalogSettings.NewTvChannelsEnabled,
                BlogEnabled = _blogSettings.Enabled,
                ForumEnabled = _forumSettings.ForumsEnabled,
                DisplayHomepageMenuItem = _displayDefaultMenuItemSettings.DisplayHomepageMenuItem,
                DisplayNewTvChannelsMenuItem = _displayDefaultMenuItemSettings.DisplayNewTvChannelsMenuItem,
                DisplayTvChannelSearchMenuItem = _displayDefaultMenuItemSettings.DisplayTvChannelSearchMenuItem,
                DisplayUserInfoMenuItem = _displayDefaultMenuItemSettings.DisplayUserInfoMenuItem,
                DisplayBlogMenuItem = _displayDefaultMenuItemSettings.DisplayBlogMenuItem,
                DisplayForumsMenuItem = _displayDefaultMenuItemSettings.DisplayForumsMenuItem,
                DisplayContactUsMenuItem = _displayDefaultMenuItemSettings.DisplayContactUsMenuItem,
                UseAjaxMenu = _catalogSettings.UseAjaxLoadMenu
            };

            return model;
        }

        /// <summary>
        /// Prepare homepage category models
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of homepage category models
        /// </returns>
        public virtual async Task<List<CategoryModel>> PrepareHomepageCategoryModelsAsync()
        {
            var language = await _workContext.GetWorkingLanguageAsync();
            var user = await _workContext.GetCurrentUserAsync();
            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
            var store = await _storeContext.GetCurrentStoreAsync();
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;
            var categoriesCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.CategoryHomepageKey,
                store, userRoleIds, pictureSize, language, _webHelper.IsCurrentConnectionSecured());

            var model = await _staticCacheManager.GetAsync(categoriesCacheKey, async () =>
            {
                var homepageCategories = await _categoryService.GetAllCategoriesDisplayedOnHomepageAsync();
                return await homepageCategories.SelectAwait(async category =>
                {
                    var catModel = new CategoryModel
                    {
                        Id = category.Id,
                        Name = await _localizationService.GetLocalizedAsync(category, x => x.Name),
                        Description = await _localizationService.GetLocalizedAsync(category, x => x.Description),
                        MetaKeywords = await _localizationService.GetLocalizedAsync(category, x => x.MetaKeywords),
                        MetaDescription = await _localizationService.GetLocalizedAsync(category, x => x.MetaDescription),
                        MetaTitle = await _localizationService.GetLocalizedAsync(category, x => x.MetaTitle),
                        SeName = await _urlRecordService.GetSeNameAsync(category),
                    };

                    //prepare picture model
                    var secured = _webHelper.IsCurrentConnectionSecured();
                    var categoryPictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.CategoryPictureModelKey,
                        category, pictureSize, true, language, secured, store);
                    catModel.PictureModel = await _staticCacheManager.GetAsync(categoryPictureCacheKey, async () =>
                    {
                        var picture = await _pictureService.GetPictureByIdAsync(category.PictureId);
                        string fullSizeImageUrl, imageUrl;

                        (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                        (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

                        var titleLocale = await _localizationService.GetResourceAsync("Media.Category.ImageLinkTitleFormat");
                        var altLocale = await _localizationService.GetResourceAsync("Media.Category.ImageAlternateTextFormat");
                        return new PictureModel
                        {
                            FullSizeImageUrl = fullSizeImageUrl,
                            ImageUrl = imageUrl,
                            Title = string.Format(titleLocale, catModel.Name),
                            AlternateText = string.Format(altLocale, catModel.Name)
                        };
                    });

                    return catModel;
                }).ToListAsync();
            });

            return model;
        }

        /// <summary>
        /// Prepare root categories for menu
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of category (simple) models
        /// </returns>
        public virtual async Task<List<CategorySimpleModel>> PrepareRootCategoriesAsync()
        {
            var doc = await PrepareCategoryXmlDocumentAsync();

            var models = from xe in doc.Root.XPathSelectElements("CategorySimpleModel")
                         select GetCategorySimpleModel(xe);

            return models.ToList();
        }

        /// <summary>
        /// Prepare subcategories for menu
        /// </summary>
        /// <param name="id">Id of category to get subcategory</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the 
        /// </returns>
        public virtual async Task<List<CategorySimpleModel>> PrepareSubCategoriesAsync(int id)
        {
            var doc = await PrepareCategoryXmlDocumentAsync();

            var model = from xe in doc.Descendants("CategorySimpleModel")
                        where xe.XPathSelectElement("Id").Value == id.ToString()
                        select xe;

            var models = from xe in model.First().XPathSelectElements("SubCategories/CategorySimpleModel")
                         select GetCategorySimpleModel(xe);

            return models.ToList();
        }

        /// <summary>
        /// Prepares the category tvchannels model
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category tvchannels model
        /// </returns>
        public virtual async Task<CatalogTvChannelsModel> PrepareCategoryTvChannelsModelAsync(Category category, CatalogTvChannelsCommand command)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CatalogTvChannelsModel
            {
                UseAjaxLoading = _catalogSettings.UseAjaxCatalogTvChannelsLoading
            };

            var currentStore = await _storeContext.GetCurrentStoreAsync();

            //sorting
            await PrepareSortingOptionsAsync(model, command);
            //view mode
            await PrepareViewModesAsync(model, command);
            //page size
            await PreparePageSizeOptionsAsync(model, command, category.AllowUsersToSelectPageSize,
                category.PageSizeOptions, category.PageSize);

            var categoryIds = new List<int> { category.Id };

            //include subcategories
            if (_catalogSettings.ShowTvChannelsFromSubcategories)
                categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(category.Id, currentStore.Id));

            //price range
            PriceRangeModel selectedPriceRange = null;
            if (_catalogSettings.EnablePriceRangeFiltering && category.PriceRangeFiltering)
            {
                selectedPriceRange = await GetConvertedPriceRangeAsync(command);

                PriceRangeModel availablePriceRange = null;
                if (!category.ManuallyPriceRange)
                {
                    async Task<decimal?> getTvChannelPriceAsync(TvChannelSortingEnum orderBy)
                    {
                        var tvchannels = await _tvchannelService.SearchTvChannelsAsync(0, 1,
                            categoryIds: categoryIds,
                            storeId: currentStore.Id,
                            visibleIndividuallyOnly: true,
                            excludeFeaturedTvChannels: !_catalogSettings.IgnoreFeaturedTvChannels && !_catalogSettings.IncludeFeaturedTvChannelsInNormalLists,
                            orderBy: orderBy);

                        return tvchannels?.FirstOrDefault()?.Price ?? 0;
                    }

                    availablePriceRange = new PriceRangeModel
                    {
                        From = await getTvChannelPriceAsync(TvChannelSortingEnum.PriceAsc),
                        To = await getTvChannelPriceAsync(TvChannelSortingEnum.PriceDesc)
                    };
                }
                else
                {
                    availablePriceRange = new PriceRangeModel
                    {
                        From = category.PriceFrom,
                        To = category.PriceTo
                    };
                }

                model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
            }

            //filterable options
            var filterableOptions = await _specificationAttributeService
                .GetFiltrableSpecificationAttributeOptionsByCategoryIdAsync(category.Id);

            if (_catalogSettings.EnableSpecificationAttributeFiltering)
            {
                model.SpecificationFilter = await PrepareSpecificationFilterModel(command.SpecificationOptionIds, filterableOptions);
            }

            //filterable manufacturers
            if (_catalogSettings.EnableManufacturerFiltering)
            {
                var manufacturers = await _manufacturerService.GetManufacturersByCategoryIdAsync(category.Id);

                model.ManufacturerFilter = await PrepareManufacturerFilterModel(command.ManufacturerIds, manufacturers);
            }

            var filteredSpecs = command.SpecificationOptionIds is null ? null : filterableOptions.Where(fo => command.SpecificationOptionIds.Contains(fo.Id)).ToList();

            //tvchannels
            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(
                command.PageNumber - 1,
                command.PageSize,
                categoryIds: categoryIds,
                storeId: currentStore.Id,
                visibleIndividuallyOnly: true,
                excludeFeaturedTvChannels: !_catalogSettings.IgnoreFeaturedTvChannels && !_catalogSettings.IncludeFeaturedTvChannelsInNormalLists,
                priceMin: selectedPriceRange?.From,
                priceMax: selectedPriceRange?.To,
                manufacturerIds: command.ManufacturerIds,
                filteredSpecOptions: filteredSpecs,
                orderBy: (TvChannelSortingEnum)command.OrderBy);

            var isFiltering = filterableOptions.Any() || selectedPriceRange?.From is not null;
            await PrepareCatalogTvChannelsAsync(model, tvchannels, isFiltering);

            return model;
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of category (simple) models
        /// </returns>
        public virtual async Task<List<CategorySimpleModel>> PrepareCategorySimpleModelsAsync()
        {
            //load and cache them
            var language = await _workContext.GetWorkingLanguageAsync();
            var user = await _workContext.GetCurrentUserAsync();
            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
            var store = await _storeContext.GetCurrentStoreAsync();
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.CategoryAllModelKey,
                language, userRoleIds, store);

            return await _staticCacheManager.GetAsync(cacheKey, async () => await PrepareCategorySimpleModelsAsync(0));
        }

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of category (simple) models
        /// </returns>
        public virtual async Task<List<CategorySimpleModel>> PrepareCategorySimpleModelsAsync(int rootCategoryId, bool loadSubCategories = true)
        {
            var result = new List<CategorySimpleModel>();

            //little hack for performance optimization
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once (we know they are cached)
            var store = await _storeContext.GetCurrentStoreAsync();
            var allCategories = await _categoryService.GetAllCategoriesAsync(storeId: store.Id);
            var categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).OrderBy(c => c.DisplayOrder).ToList();
            foreach (var category in categories)
            {
                var categoryModel = new CategorySimpleModel
                {
                    Id = category.Id,
                    Name = await _localizationService.GetLocalizedAsync(category, x => x.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(category),
                    IncludeInTopMenu = category.IncludeInTopMenu
                };

                //number of tvchannels in each category
                if (_catalogSettings.ShowCategoryTvChannelNumber)
                {
                    var categoryIds = new List<int> { category.Id };
                    //include subcategories
                    if (_catalogSettings.ShowCategoryTvChannelNumberIncludingSubcategories)
                        categoryIds.AddRange(
                            await _categoryService.GetChildCategoryIdsAsync(category.Id, store.Id));

                    categoryModel.NumberOfTvChannels =
                        await _tvchannelService.GetNumberOfTvChannelsInCategoryAsync(categoryIds, store.Id);
                }

                if (loadSubCategories)
                {
                    var subCategories = await PrepareCategorySimpleModelsAsync(category.Id);
                    categoryModel.SubCategories.AddRange(subCategories);
                }

                categoryModel.HaveSubCategories = categoryModel.SubCategories.Count > 0 &
                    categoryModel.SubCategories.Any(x => x.IncludeInTopMenu);

                result.Add(categoryModel);
            }

            return result;
        }

        /// <summary>
        /// Prepare category (simple) xml document
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the xml document of category (simple) models
        /// </returns>
        public virtual async Task<XDocument> PrepareCategoryXmlDocumentAsync()
        {
            var language = await _workContext.GetWorkingLanguageAsync();
            var user = await _workContext.GetCurrentUserAsync();
            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
            var store = await _storeContext.GetCurrentStoreAsync();
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.CategoryXmlAllModelKey,
                language, userRoleIds, store);

            return await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var categories = await PrepareCategorySimpleModelsAsync();

                var xsSubmit = new XmlSerializer(typeof(List<CategorySimpleModel>));

                var settings = new XmlWriterSettings
                {
                    Async = true,
                    ConformanceLevel = ConformanceLevel.Auto
                };

                await using var strWriter = new StringWriter();
                await using var writer = XmlWriter.Create(strWriter, settings);
                xsSubmit.Serialize(writer, categories);
                var xml = strWriter.ToString();

                return XDocument.Parse(xml);
            });
        }

        #endregion

        #region Manufacturers

        /// <summary>
        /// Prepare manufacturer model
        /// </summary>
        /// <param name="manufacturer">Manufacturer identifier</param>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer model
        /// </returns>
        public virtual async Task<ManufacturerModel> PrepareManufacturerModelAsync(Manufacturer manufacturer, CatalogTvChannelsCommand command)
        {
            if (manufacturer == null)
                throw new ArgumentNullException(nameof(manufacturer));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new ManufacturerModel
            {
                Id = manufacturer.Id,
                Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                Description = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Description),
                MetaKeywords = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(manufacturer),
                CatalogTvChannelsModel = await PrepareManufacturerTvChannelsModelAsync(manufacturer, command)
            };

            //featured tvchannels
            if (!_catalogSettings.IgnoreFeaturedTvChannels)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var storeId = store.Id;
                var featuredTvChannels = await _tvchannelService.GetManufacturerFeaturedTvChannelsAsync(manufacturer.Id, storeId);
                if (featuredTvChannels != null)
                    model.FeaturedTvChannels = (await _tvchannelModelFactory.PrepareTvChannelOverviewModelsAsync(featuredTvChannels)).ToList();
            }

            return model;
        }

        /// <summary>
        /// Prepares the manufacturer tvchannels model
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer tvchannels model
        /// </returns>
        public virtual async Task<CatalogTvChannelsModel> PrepareManufacturerTvChannelsModelAsync(Manufacturer manufacturer, CatalogTvChannelsCommand command)
        {
            if (manufacturer == null)
                throw new ArgumentNullException(nameof(manufacturer));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CatalogTvChannelsModel
            {
                UseAjaxLoading = _catalogSettings.UseAjaxCatalogTvChannelsLoading
            };

            var manufacturerIds = new List<int> { manufacturer.Id };
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            //sorting
            await PrepareSortingOptionsAsync(model, command);
            //view mode
            await PrepareViewModesAsync(model, command);
            //page size
            await PreparePageSizeOptionsAsync(model, command, manufacturer.AllowUsersToSelectPageSize,
                manufacturer.PageSizeOptions, manufacturer.PageSize);

            //price range
            PriceRangeModel selectedPriceRange = null;
            if (_catalogSettings.EnablePriceRangeFiltering && manufacturer.PriceRangeFiltering)
            {
                selectedPriceRange = await GetConvertedPriceRangeAsync(command);

                PriceRangeModel availablePriceRange = null;
                if (!manufacturer.ManuallyPriceRange)
                {
                    async Task<decimal?> getTvChannelPriceAsync(TvChannelSortingEnum orderBy)
                    {
                        var tvchannels = await _tvchannelService.SearchTvChannelsAsync(0, 1,
                            manufacturerIds: manufacturerIds,
                            storeId: currentStore.Id,
                            visibleIndividuallyOnly: true,
                            excludeFeaturedTvChannels: !_catalogSettings.IgnoreFeaturedTvChannels && !_catalogSettings.IncludeFeaturedTvChannelsInNormalLists,
                            orderBy: orderBy);

                        return tvchannels?.FirstOrDefault()?.Price ?? 0;
                    }

                    availablePriceRange = new PriceRangeModel
                    {
                        From = await getTvChannelPriceAsync(TvChannelSortingEnum.PriceAsc),
                        To = await getTvChannelPriceAsync(TvChannelSortingEnum.PriceDesc)
                    };
                }
                else
                {
                    availablePriceRange = new PriceRangeModel
                    {
                        From = manufacturer.PriceFrom,
                        To = manufacturer.PriceTo
                    };
                }

                model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
            }

            // filterable options
            var filterableOptions = await _specificationAttributeService
                .GetFiltrableSpecificationAttributeOptionsByManufacturerIdAsync(manufacturer.Id);

            if (_catalogSettings.EnableSpecificationAttributeFiltering)
            {
                model.SpecificationFilter = await PrepareSpecificationFilterModel(command.SpecificationOptionIds, filterableOptions);
            }

            var filteredSpecs = command.SpecificationOptionIds is null ? null : filterableOptions.Where(fo => command.SpecificationOptionIds.Contains(fo.Id)).ToList();

            //tvchannels
            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(
                command.PageNumber - 1,
                command.PageSize,
                manufacturerIds: manufacturerIds,
                storeId: currentStore.Id,
                visibleIndividuallyOnly: true,
                excludeFeaturedTvChannels: !_catalogSettings.IgnoreFeaturedTvChannels && !_catalogSettings.IncludeFeaturedTvChannelsInNormalLists,
                priceMin: selectedPriceRange?.From,
                priceMax: selectedPriceRange?.To,
                filteredSpecOptions: filteredSpecs,
                orderBy: (TvChannelSortingEnum)command.OrderBy);

            var isFiltering = filterableOptions.Any() || selectedPriceRange?.From is not null;
            await PrepareCatalogTvChannelsAsync(model, tvchannels, isFiltering);

            return model;
        }

        /// <summary>
        /// Prepare manufacturer template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer template view path
        /// </returns>
        public virtual async Task<string> PrepareManufacturerTemplateViewPathAsync(int templateId)
        {
            var template = await _manufacturerTemplateService.GetManufacturerTemplateByIdAsync(templateId) ??
                           (await _manufacturerTemplateService.GetAllManufacturerTemplatesAsync()).FirstOrDefault();

            if (template == null)
                throw new Exception("No default template could be loaded");

            return template.ViewPath;
        }

        /// <summary>
        /// Prepare manufacturer all models
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of manufacturer models
        /// </returns>
        public virtual async Task<List<ManufacturerModel>> PrepareManufacturerAllModelsAsync()
        {
            var model = new List<ManufacturerModel>();

            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(storeId: currentStore.Id);
            foreach (var manufacturer in manufacturers)
            {
                var modelMan = new ManufacturerModel
                {
                    Id = manufacturer.Id,
                    Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                    Description = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Description),
                    MetaKeywords = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaKeywords),
                    MetaDescription = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaDescription),
                    MetaTitle = await _localizationService.GetLocalizedAsync(manufacturer, x => x.MetaTitle),
                    SeName = await _urlRecordService.GetSeNameAsync(manufacturer),
                };

                //prepare picture model
                var pictureSize = _mediaSettings.ManufacturerThumbPictureSize;
                var manufacturerPictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.ManufacturerPictureModelKey,
                    manufacturer, pictureSize, true, await _workContext.GetWorkingLanguageAsync(),
                    _webHelper.IsCurrentConnectionSecured(), currentStore);
                modelMan.PictureModel = await _staticCacheManager.GetAsync(manufacturerPictureCacheKey, async () =>
                {
                    var picture = await _pictureService.GetPictureByIdAsync(manufacturer.PictureId);
                    string fullSizeImageUrl, imageUrl;

                    (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                    (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = fullSizeImageUrl,
                        ImageUrl = imageUrl,
                        Title = string.Format(await _localizationService.GetResourceAsync("Media.Manufacturer.ImageLinkTitleFormat"), modelMan.Name),
                        AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Manufacturer.ImageAlternateTextFormat"), modelMan.Name)
                    };

                    return pictureModel;
                });

                model.Add(modelMan);
            }

            return model;
        }

        /// <summary>
        /// Prepare manufacturer navigation model
        /// </summary>
        /// <param name="currentManufacturerId">Current manufacturer identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer navigation model
        /// </returns>
        public virtual async Task<ManufacturerNavigationModel> PrepareManufacturerNavigationModelAsync(int currentManufacturerId)
        {
            var language = await _workContext.GetWorkingLanguageAsync();
            var user = await _workContext.GetCurrentUserAsync();
            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
            var store = await _storeContext.GetCurrentStoreAsync();
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.ManufacturerNavigationModelKey,
                currentManufacturerId, language, userRoleIds, store);
            var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var currentManufacturer = await _manufacturerService.GetManufacturerByIdAsync(currentManufacturerId);

                var manufacturers = await _manufacturerService.GetAllManufacturersAsync(storeId: store.Id,
                    pageSize: _catalogSettings.ManufacturersBlockItemsToDisplay);
                var model = new ManufacturerNavigationModel
                {
                    TotalManufacturers = manufacturers.TotalCount
                };

                foreach (var manufacturer in manufacturers)
                {
                    var modelMan = new ManufacturerBriefInfoModel
                    {
                        Id = manufacturer.Id,
                        Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(manufacturer),
                        IsActive = currentManufacturer != null && currentManufacturer.Id == manufacturer.Id,
                    };
                    model.Manufacturers.Add(modelMan);
                }

                return model;
            });

            return cachedModel;
        }

        #endregion

        #region Vendors

        /// <summary>
        /// Prepare vendor model
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor model
        /// </returns>
        public virtual async Task<VendorModel> PrepareVendorModelAsync(Vendor vendor, CatalogTvChannelsCommand command)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new VendorModel
            {
                Id = vendor.Id,
                Name = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                Description = await _localizationService.GetLocalizedAsync(vendor, x => x.Description),
                MetaKeywords = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(vendor),
                AllowUsersToContactVendors = _vendorSettings.AllowUsersToContactVendors,
                CatalogTvChannelsModel = await PrepareVendorTvChannelsModelAsync(vendor, command)
            };

            return model;
        }

        /// <summary>
        /// Prepares the vendor tvchannels model
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor tvchannels model
        /// </returns>
        public virtual async Task<CatalogTvChannelsModel> PrepareVendorTvChannelsModelAsync(Vendor vendor, CatalogTvChannelsCommand command)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CatalogTvChannelsModel
            {
                UseAjaxLoading = _catalogSettings.UseAjaxCatalogTvChannelsLoading
            };

            //sorting
            await PrepareSortingOptionsAsync(model, command);
            //view mode
            await PrepareViewModesAsync(model, command);
            //page size
            await PreparePageSizeOptionsAsync(model, command, vendor.AllowUsersToSelectPageSize,
                vendor.PageSizeOptions, vendor.PageSize);

            //price range
            PriceRangeModel selectedPriceRange = null;
            var store = await _storeContext.GetCurrentStoreAsync();
            if (_catalogSettings.EnablePriceRangeFiltering && vendor.PriceRangeFiltering)
            {
                selectedPriceRange = await GetConvertedPriceRangeAsync(command);

                PriceRangeModel availablePriceRange;
                if (!vendor.ManuallyPriceRange)
                {
                    async Task<decimal?> getTvChannelPriceAsync(TvChannelSortingEnum orderBy)
                    {
                        var tvchannels = await _tvchannelService.SearchTvChannelsAsync(0, 1,
                            vendorId: vendor.Id,
                            storeId: store.Id,
                            visibleIndividuallyOnly: true,
                            orderBy: orderBy);

                        return tvchannels?.FirstOrDefault()?.Price ?? 0;
                    }

                    availablePriceRange = new PriceRangeModel
                    {
                        From = await getTvChannelPriceAsync(TvChannelSortingEnum.PriceAsc),
                        To = await getTvChannelPriceAsync(TvChannelSortingEnum.PriceDesc)
                    };
                }
                else
                {
                    availablePriceRange = new PriceRangeModel
                    {
                        From = vendor.PriceFrom,
                        To = vendor.PriceTo
                    };
                }

                model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
            }

            //tvchannels
            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(
                command.PageNumber - 1,
                command.PageSize,
                vendorId: vendor.Id,
                priceMin: selectedPriceRange?.From,
                priceMax: selectedPriceRange?.To,
                storeId: store.Id,
                visibleIndividuallyOnly: true,
                orderBy: (TvChannelSortingEnum)command.OrderBy);

            var isFiltering = selectedPriceRange?.From is not null;
            await PrepareCatalogTvChannelsAsync(model, tvchannels, isFiltering);

            return model;
        }

        /// <summary>
        /// Prepare vendor all models
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of vendor models
        /// </returns>
        public virtual async Task<List<VendorModel>> PrepareVendorAllModelsAsync()
        {
            var model = new List<VendorModel>();
            var vendors = await _vendorService.GetAllVendorsAsync();
            foreach (var vendor in vendors)
            {
                var vendorModel = new VendorModel
                {
                    Id = vendor.Id,
                    Name = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                    Description = await _localizationService.GetLocalizedAsync(vendor, x => x.Description),
                    MetaKeywords = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaKeywords),
                    MetaDescription = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaDescription),
                    MetaTitle = await _localizationService.GetLocalizedAsync(vendor, x => x.MetaTitle),
                    SeName = await _urlRecordService.GetSeNameAsync(vendor),
                    AllowUsersToContactVendors = _vendorSettings.AllowUsersToContactVendors
                };

                //prepare picture model
                var pictureSize = _mediaSettings.VendorThumbPictureSize;
                var pictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.VendorPictureModelKey,
                    vendor, pictureSize, true, await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());
                vendorModel.PictureModel = await _staticCacheManager.GetAsync(pictureCacheKey, async () =>
                {
                    var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);
                    string fullSizeImageUrl, imageUrl;

                    (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                    (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = fullSizeImageUrl,
                        ImageUrl = imageUrl,
                        Title = string.Format(await _localizationService.GetResourceAsync("Media.Vendor.ImageLinkTitleFormat"), vendorModel.Name),
                        AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.Vendor.ImageAlternateTextFormat"), vendorModel.Name)
                    };

                    return pictureModel;
                });

                model.Add(vendorModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare vendor navigation model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vendor navigation model
        /// </returns>
        public virtual async Task<VendorNavigationModel> PrepareVendorNavigationModelAsync()
        {
            var cacheKey = TvProgModelCacheDefaults.VendorNavigationModelKey;
            var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var vendors = await _vendorService.GetAllVendorsAsync(pageSize: _vendorSettings.VendorsBlockItemsToDisplay);
                var model = new VendorNavigationModel
                {
                    TotalVendors = vendors.TotalCount
                };

                foreach (var vendor in vendors)
                {
                    model.Vendors.Add(new VendorBriefInfoModel
                    {
                        Id = vendor.Id,
                        Name = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(vendor),
                    });
                }

                return model;
            });

            return cachedModel;
        }

        #endregion

        #region TvChannel tags

        /// <summary>
        /// Prepare popular tvchannel tags model
        /// </summary>
        /// <param name="numberTagsToReturn">The number of tags to be returned; pass 0 to get all tags</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel tags model
        /// </returns>
        public virtual async Task<PopularTvChannelTagsModel> PreparePopularTvChannelTagsModelAsync(int numberTagsToReturn = 0)
        {
            var model = new PopularTvChannelTagsModel();

            var currentStore = await _storeContext.GetCurrentStoreAsync();

            var tagStats = await _tvchannelTagService.GetTvChannelCountAsync(currentStore.Id);

            model.TotalTags = tagStats.Count;

            model.Tags.AddRange(await tagStats
                //Take the most popular tags if specified
                .OrderByDescending(x => x.Value).Take(numberTagsToReturn > 0 ? numberTagsToReturn : tagStats.Count)
                .SelectAwait(async tagStat =>
                {
                    var tag = await _tvchannelTagService.GetTvChannelTagByIdAsync(tagStat.Key);

                    return new TvChannelTagModel
                    {
                        Id = tag.Id,
                        Name = await _localizationService.GetLocalizedAsync(tag, t => t.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(tag),
                        TvChannelCount = tagStat.Value
                    };
                })
                //sorting result
                .OrderBy(x => x.Name)
                .ToListAsync());

            return model;
        }

        /// <summary>
        /// Prepare tvchannels by tag model
        /// </summary>
        /// <param name="tvchannelTag">TvChannel tag</param>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannels by tag model
        /// </returns>
        public virtual async Task<TvChannelsByTagModel> PrepareTvChannelsByTagModelAsync(TvChannelTag tvchannelTag, CatalogTvChannelsCommand command)
        {
            if (tvchannelTag == null)
                throw new ArgumentNullException(nameof(tvchannelTag));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new TvChannelsByTagModel
            {
                Id = tvchannelTag.Id,
                TagName = await _localizationService.GetLocalizedAsync(tvchannelTag, y => y.Name),
                TagSeName = await _urlRecordService.GetSeNameAsync(tvchannelTag),
                CatalogTvChannelsModel = await PrepareTagTvChannelsModelAsync(tvchannelTag, command)
            };

            return model;
        }

        /// <summary>
        /// Prepares the tag tvchannels model
        /// </summary>
        /// <param name="tvchannelTag">TvChannel tag</param>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ag tvchannels model
        /// </returns>
        public virtual async Task<CatalogTvChannelsModel> PrepareTagTvChannelsModelAsync(TvChannelTag tvchannelTag, CatalogTvChannelsCommand command)
        {
            if (tvchannelTag == null)
                throw new ArgumentNullException(nameof(tvchannelTag));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CatalogTvChannelsModel
            {
                UseAjaxLoading = _catalogSettings.UseAjaxCatalogTvChannelsLoading
            };

            //sorting
            await PrepareSortingOptionsAsync(model, command);
            //view mode
            await PrepareViewModesAsync(model, command);
            //page size
            await PreparePageSizeOptionsAsync(model, command, _catalogSettings.TvChannelsByTagAllowUsersToSelectPageSize,
                _catalogSettings.TvChannelsByTagPageSizeOptions, _catalogSettings.TvChannelsByTagPageSize);

            //price range
            PriceRangeModel selectedPriceRange = null;
            var store = await _storeContext.GetCurrentStoreAsync();
            if (_catalogSettings.EnablePriceRangeFiltering && _catalogSettings.TvChannelsByTagPriceRangeFiltering)
            {
                selectedPriceRange = await GetConvertedPriceRangeAsync(command);

                PriceRangeModel availablePriceRange;
                if (!_catalogSettings.TvChannelsByTagManuallyPriceRange)
                {
                    async Task<decimal?> getTvChannelPriceAsync(TvChannelSortingEnum orderBy)
                    {
                        var tvchannels = await _tvchannelService.SearchTvChannelsAsync(0, 1,
                            storeId: store.Id,
                            tvchannelTagId: tvchannelTag.Id,
                            visibleIndividuallyOnly: true,
                            orderBy: orderBy);

                        return tvchannels?.FirstOrDefault()?.Price ?? 0;
                    }

                    availablePriceRange = new PriceRangeModel
                    {
                        From = await getTvChannelPriceAsync(TvChannelSortingEnum.PriceAsc),
                        To = await getTvChannelPriceAsync(TvChannelSortingEnum.PriceDesc)
                    };
                }
                else
                {
                    availablePriceRange = new PriceRangeModel
                    {
                        From = _catalogSettings.TvChannelsByTagPriceFrom,
                        To = _catalogSettings.TvChannelsByTagPriceTo
                    };
                }

                model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
            }

            //tvchannels
            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(
                command.PageNumber - 1,
                command.PageSize,
                priceMin: selectedPriceRange?.From,
                priceMax: selectedPriceRange?.To,
                storeId: store.Id,
                tvchannelTagId: tvchannelTag.Id,
                visibleIndividuallyOnly: true,
                orderBy: (TvChannelSortingEnum)command.OrderBy);

            var isFiltering = selectedPriceRange?.From is not null;
            await PrepareCatalogTvChannelsAsync(model, tvchannels, isFiltering);

            return model;
        }

        #endregion

        #region New tvchannels

        /// <summary>
        /// Prepare new tvchannels model
        /// </summary>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new tvchannels model
        /// </returns>
        public virtual async Task<CatalogTvChannelsModel> PrepareNewTvChannelsModelAsync(CatalogTvChannelsCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CatalogTvChannelsModel
            {
                UseAjaxLoading = _catalogSettings.UseAjaxCatalogTvChannelsLoading
            };

            var currentStore = await _storeContext.GetCurrentStoreAsync();

            //page size
            await PreparePageSizeOptionsAsync(model, command, _catalogSettings.NewTvChannelsAllowUsersToSelectPageSize,
                _catalogSettings.NewTvChannelsPageSizeOptions, _catalogSettings.NewTvChannelsPageSize);

            //tvchannels
            var tvchannels = await _tvchannelService.GetTvChannelsMarkedAsNewAsync(storeId: currentStore.Id,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);

            await PrepareCatalogTvChannelsAsync(model, tvchannels);

            return model;
        }

        #endregion

        #region Searching

        /// <summary>
        /// Prepare search model
        /// </summary>
        /// <param name="model">Search model</param>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the search model
        /// </returns>
        public virtual async Task<SearchModel> PrepareSearchModelAsync(SearchModel model, CatalogTvChannelsCommand command)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var categoriesModels = new List<SearchModel.CategoryModel>();
            //all categories
            var allCategories = await _categoryService.GetAllCategoriesAsync(storeId: currentStore.Id);
            foreach (var c in allCategories)
            {
                //generate full category name (breadcrumb)
                var categoryBreadcrumb = string.Empty;
                var breadcrumb = await _categoryService.GetCategoryBreadCrumbAsync(c, allCategories);
                for (var i = 0; i <= breadcrumb.Count - 1; i++)
                {
                    categoryBreadcrumb += await _localizationService.GetLocalizedAsync(breadcrumb[i], x => x.Name);
                    if (i != breadcrumb.Count - 1)
                        categoryBreadcrumb += " >> ";
                }

                categoriesModels.Add(new SearchModel.CategoryModel
                {
                    Id = c.Id,
                    Breadcrumb = categoryBreadcrumb
                });
            }

            if (categoriesModels.Any())
            {
                //first empty entry
                model.AvailableCategories.Add(new SelectListItem
                {
                    Value = "0",
                    Text = await _localizationService.GetResourceAsync("Common.All")
                });
                //all other categories
                foreach (var c in categoriesModels)
                {
                    model.AvailableCategories.Add(new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Breadcrumb,
                        Selected = model.cid == c.Id
                    });
                }
            }

            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(storeId: currentStore.Id);
            if (manufacturers.Any())
            {
                model.AvailableManufacturers.Add(new SelectListItem
                {
                    Value = "0",
                    Text = await _localizationService.GetResourceAsync("Common.All")
                });
                foreach (var m in manufacturers)
                    model.AvailableManufacturers.Add(new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = await _localizationService.GetLocalizedAsync(m, x => x.Name),
                        Selected = model.mid == m.Id
                    });
            }

            model.asv = _vendorSettings.AllowSearchByVendor;
            if (model.asv)
            {
                var vendors = await _vendorService.GetAllVendorsAsync();
                if (vendors.Any())
                {
                    model.AvailableVendors.Add(new SelectListItem
                    {
                        Value = "0",
                        Text = await _localizationService.GetResourceAsync("Common.All")
                    });
                    foreach (var vendor in vendors)
                        model.AvailableVendors.Add(new SelectListItem
                        {
                            Value = vendor.Id.ToString(),
                            Text = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                            Selected = model.vid == vendor.Id
                        });
                }
            }

            model.CatalogTvChannelsModel = await PrepareSearchTvChannelsModelAsync(model, command);

            return model;
        }

        /// <summary>
        /// Prepares the search tvchannels model
        /// </summary>
        /// <param name="model">Search model</param>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the search tvchannels model
        /// </returns>
        public virtual async Task<CatalogTvChannelsModel> PrepareSearchTvChannelsModelAsync(SearchModel searchModel, CatalogTvChannelsCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CatalogTvChannelsModel
            {
                UseAjaxLoading = _catalogSettings.UseAjaxCatalogTvChannelsLoading
            };

            //sorting
            await PrepareSortingOptionsAsync(model, command);
            //view mode
            await PrepareViewModesAsync(model, command);
            //page size
            await PreparePageSizeOptionsAsync(model, command, _catalogSettings.SearchPageAllowUsersToSelectPageSize,
                _catalogSettings.SearchPagePageSizeOptions, _catalogSettings.SearchPageTvChannelsPerPage);

            var searchTerms = searchModel.q == null
                ? string.Empty
                : searchModel.q.Trim();

            IPagedList<TvChannel> tvchannels = new PagedList<TvChannel>(new List<TvChannel>(), 0, 1);
            //only search if query string search keyword is set (used to avoid searching or displaying search term min length error message on /search page load)
            //we don't use "!string.IsNullOrEmpty(searchTerms)" in cases of "TvChannelSearchTermMinimumLength" set to 0 but searching by other parameters (e.g. category or price filter)
            var isSearchTermSpecified = _httpContextAccessor.HttpContext.Request.Query.ContainsKey("q");
            if (isSearchTermSpecified)
            {
                var currentStore = await _storeContext.GetCurrentStoreAsync();

                if (searchTerms.Length < _catalogSettings.TvChannelSearchTermMinimumLength)
                {
                    model.WarningMessage =
                        string.Format(await _localizationService.GetResourceAsync("Search.SearchTermMinimumLengthIsNCharacters"),
                            _catalogSettings.TvChannelSearchTermMinimumLength);
                }
                else
                {
                    var categoryIds = new List<int>();
                    var manufacturerId = 0;
                    var searchInDescriptions = false;
                    var vendorId = 0;
                    if (searchModel.advs)
                    {
                        //advanced search
                        var categoryId = searchModel.cid;
                        if (categoryId > 0)
                        {
                            categoryIds.Add(categoryId);
                            if (searchModel.isc)
                            {
                                //include subcategories
                                categoryIds.AddRange(
                                    await _categoryService.GetChildCategoryIdsAsync(categoryId, currentStore.Id));
                            }
                        }

                        manufacturerId = searchModel.mid;

                        if (searchModel.asv)
                            vendorId = searchModel.vid;

                        searchInDescriptions = searchModel.sid;
                    }

                    //var searchInTvChannelTags = false;
                    var searchInTvChannelTags = searchInDescriptions;
                    var workingLanguage = await _workContext.GetWorkingLanguageAsync();

                    //price range
                    PriceRangeModel selectedPriceRange = null;
                    if (_catalogSettings.EnablePriceRangeFiltering && _catalogSettings.SearchPagePriceRangeFiltering)
                    {
                        selectedPriceRange = await GetConvertedPriceRangeAsync(command);

                        PriceRangeModel availablePriceRange;
                        async Task<decimal?> getTvChannelPriceAsync(TvChannelSortingEnum orderBy)
                        {
                            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(0, 1,
                                categoryIds: categoryIds,
                                manufacturerIds: new List<int> { manufacturerId },
                                storeId: currentStore.Id,
                                visibleIndividuallyOnly: true,
                                keywords: searchTerms,
                                searchDescriptions: searchInDescriptions,
                                searchTvChannelTags: searchInTvChannelTags,
                                languageId: workingLanguage.Id,
                                vendorId: vendorId,
                                orderBy: orderBy);

                            return tvchannels?.FirstOrDefault()?.Price ?? 0;
                        }

                        if (_catalogSettings.SearchPageManuallyPriceRange)
                        {
                            var to = await getTvChannelPriceAsync(TvChannelSortingEnum.PriceDesc);

                            availablePriceRange = new PriceRangeModel
                            {
                                From = _catalogSettings.SearchPagePriceFrom,
                                To = to == 0 ? 0 : _catalogSettings.SearchPagePriceTo
                            };
                        }
                        else
                            availablePriceRange = new PriceRangeModel
                            {
                                From = await getTvChannelPriceAsync(TvChannelSortingEnum.PriceAsc),
                                To = await getTvChannelPriceAsync(TvChannelSortingEnum.PriceDesc)
                            };

                        model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
                    }

                    //tvchannels
                    tvchannels = await _tvchannelService.SearchTvChannelsAsync(
                        command.PageNumber - 1,
                        command.PageSize,
                        categoryIds: categoryIds,
                        manufacturerIds: new List<int> { manufacturerId },
                        storeId: currentStore.Id,
                        visibleIndividuallyOnly: true,
                        keywords: searchTerms,
                        priceMin: selectedPriceRange?.From,
                        priceMax: selectedPriceRange?.To,
                        searchDescriptions: searchInDescriptions,
                        searchTvChannelTags: searchInTvChannelTags,
                        languageId: workingLanguage.Id,
                        orderBy: (TvChannelSortingEnum)command.OrderBy,
                        vendorId: vendorId);

                    //search term statistics
                    if (!string.IsNullOrEmpty(searchTerms))
                    {
                        var searchTerm =
                            await _searchTermService.GetSearchTermByKeywordAsync(searchTerms, currentStore.Id);
                        if (searchTerm != null)
                        {
                            searchTerm.Count++;
                            await _searchTermService.UpdateSearchTermAsync(searchTerm);
                        }
                        else
                        {
                            searchTerm = new SearchTerm
                            {
                                Keyword = searchTerms,
                                StoreId = currentStore.Id,
                                Count = 1
                            };
                            await _searchTermService.InsertSearchTermAsync(searchTerm);
                        }
                    }

                    //event
                    await _eventPublisher.PublishAsync(new TvChannelSearchEvent
                    {
                        SearchTerm = searchTerms,
                        SearchInDescriptions = searchInDescriptions,
                        CategoryIds = categoryIds,
                        ManufacturerId = manufacturerId,
                        WorkingLanguageId = workingLanguage.Id,
                        VendorId = vendorId
                    });
                }
            }

            var isFiltering = !string.IsNullOrEmpty(searchTerms);
            await PrepareCatalogTvChannelsAsync(model, tvchannels, isFiltering);

            return model;
        }

        /// <summary>
        /// Prepare search box model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the search box model
        /// </returns>
        public virtual Task<SearchBoxModel> PrepareSearchBoxModelAsync()
        {
            var model = new SearchBoxModel
            {
                AutoCompleteEnabled = _catalogSettings.TvChannelSearchAutoCompleteEnabled,
                ShowTvChannelImagesInSearchAutoComplete = _catalogSettings.ShowTvChannelImagesInSearchAutoComplete,
                SearchTermMinimumLength = _catalogSettings.TvChannelSearchTermMinimumLength,
                ShowSearchBox = _catalogSettings.TvChannelSearchEnabled
            };

            return Task.FromResult(model);
        }

        #endregion

        #region Common

        /// <summary>
        /// Prepare sorting options
        /// </summary>
        /// <param name="model">Catalog tvchannels model</param>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareSortingOptionsAsync(CatalogTvChannelsModel model, CatalogTvChannelsCommand command)
        {
            //get active sorting options
            var activeSortingOptionsIds = Enum.GetValues(typeof(TvChannelSortingEnum)).Cast<int>()
                .Except(_catalogSettings.TvChannelSortingEnumDisabled).ToList();

            //order sorting options
            var orderedActiveSortingOptions = activeSortingOptionsIds
                .Select(id => new { Id = id, Order = _catalogSettings.TvChannelSortingEnumDisplayOrder.TryGetValue(id, out var order) ? order : id })
                .OrderBy(option => option.Order).ToList();

            //set the default option
            model.OrderBy = command.OrderBy;
            command.OrderBy = orderedActiveSortingOptions.FirstOrDefault()?.Id ?? (int)TvChannelSortingEnum.Position;

            //ensure that tvchannel sorting is enabled
            if (!_catalogSettings.AllowTvChannelSorting)
                return;

            model.AllowTvChannelSorting = true;
            command.OrderBy = model.OrderBy ?? command.OrderBy;

            //prepare available model sorting options
            foreach (var option in orderedActiveSortingOptions)
            {
                model.AvailableSortOptions.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedEnumAsync((TvChannelSortingEnum)option.Id),
                    Value = option.Id.ToString(),
                    Selected = option.Id == command.OrderBy
                });
            }
        }

        /// <summary>
        /// Prepare view modes
        /// </summary>
        /// <param name="model">Catalog tvchannels model</param>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareViewModesAsync(CatalogTvChannelsModel model, CatalogTvChannelsCommand command)
        {
            model.AllowTvChannelViewModeChanging = _catalogSettings.AllowTvChannelViewModeChanging;

            var viewMode = !string.IsNullOrEmpty(command.ViewMode)
                ? command.ViewMode
                : _catalogSettings.DefaultViewMode;
            model.ViewMode = viewMode;
            if (model.AllowTvChannelViewModeChanging)
            {
                //grid
                model.AvailableViewModes.Add(new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Catalog.ViewMode.Grid"),
                    Value = "grid",
                    Selected = viewMode == "grid"
                });
                //list
                model.AvailableViewModes.Add(new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Catalog.ViewMode.List"),
                    Value = "list",
                    Selected = viewMode == "list"
                });
            }
        }

        /// <summary>
        /// Prepare page size options
        /// </summary>
        /// <param name="model">Catalog tvchannels model</param>
        /// <param name="command">Model to get the catalog tvchannels</param>
        /// <param name="allowUsersToSelectPageSize">Are users allowed to select page size?</param>
        /// <param name="pageSizeOptions">Page size options</param>
        /// <param name="fixedPageSize">Fixed page size</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual Task PreparePageSizeOptionsAsync(CatalogTvChannelsModel model, CatalogTvChannelsCommand command,
            bool allowUsersToSelectPageSize, string pageSizeOptions, int fixedPageSize)
        {
            if (command.PageNumber <= 0)
                command.PageNumber = 1;

            model.AllowUsersToSelectPageSize = false;
            if (allowUsersToSelectPageSize && pageSizeOptions != null)
            {
                var pageSizes = pageSizeOptions.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    // get the first page size entry to use as the default (category page load) or if user enters invalid value via query string
                    if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                    {
                        if (int.TryParse(pageSizes.FirstOrDefault(), out var temp))
                        {
                            if (temp > 0)
                                command.PageSize = temp;
                        }
                    }

                    foreach (var pageSize in pageSizes)
                    {
                        if (!int.TryParse(pageSize, out var temp))
                            continue;

                        if (temp <= 0)
                            continue;

                        model.PageSizeOptions.Add(new SelectListItem
                        {
                            Text = pageSize,
                            Value = pageSize,
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        });
                    }

                    if (model.PageSizeOptions.Any())
                    {
                        model.PageSizeOptions = model.PageSizeOptions.OrderBy(x => int.Parse(x.Value)).ToList();
                        model.AllowUsersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                            command.PageSize = int.Parse(model.PageSizeOptions.First().Value);
                    }
                }
            }
            else
            {
                //user is not allowed to select a page size
                command.PageSize = fixedPageSize;
            }

            //ensure pge size is specified
            if (command.PageSize <= 0)
            {
                command.PageSize = fixedPageSize;
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
