using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Rss;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Stores;
using TvProgViewer.Services.Vendors;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework;
using TvProgViewer.Web.Framework.Mvc;
using TvProgViewer.Web.Framework.Mvc.Filters;
using TvProgViewer.Web.Framework.Mvc.Routing;
using TvProgViewer.WebUI.Models.Catalog;
using TvProgViewer.Data.TvProgMain.ProgObjs;
using TvProgViewer.Services.TvProgMain;

namespace TvProgViewer.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class CatalogController : BasePublicController
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly IUserActivityService _userActivityService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ITvProgUrlHelper _nopUrlHelper;
        private readonly IPermissionService _permissionService;
        private readonly ITvChannelModelFactory _tvchannelModelFactory;
        private readonly ITvChannelService _tvchannelService;
        private readonly ITvChannelTagService _tvchannelTagService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IProgrammeService _programmeService;
        private readonly IGenreService _genreService; 

        #endregion

        #region Ctor

        public CatalogController(CatalogSettings catalogSettings,
            IAclService aclService,
            ICatalogModelFactory catalogModelFactory,
            ICategoryService categoryService,
            IUserActivityService userActivityService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            ITvProgUrlHelper nopUrlHelper,
            IPermissionService permissionService,
            ITvChannelModelFactory tvchannelModelFactory,
            ITvChannelService tvchannelService,
            ITvChannelTagService tvchannelTagService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings,
            IProgrammeService programmeService,
            IGenreService genreService)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _catalogModelFactory = catalogModelFactory;
            _categoryService = categoryService;
            _userActivityService = userActivityService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _nopUrlHelper = nopUrlHelper;
            _permissionService = permissionService;
            _tvchannelModelFactory = tvchannelModelFactory;
            _tvchannelService = tvchannelService;
            _tvchannelTagService = tvchannelTagService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _vendorSettings = vendorSettings;
            _programmeService = programmeService;
            _genreService = genreService;
        }

        #endregion

        #region Categories

        public virtual async Task<IActionResult> Category(int categoryId, CatalogTvChannelsCommand command)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (!await CheckCategoryAvailabilityAsync(category))
                return InvokeHttp404();

            var store = await _storeContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentUserAsync(),
                TvProgUserDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                store.Id);

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                DisplayEditLink(Url.Action("Edit", "Category", new { id = category.Id, area = AreaNames.Admin }));

            //activity log
            await _userActivityService.InsertActivityAsync("PublicStore.ViewCategory",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewCategory"), category.Name), category);

            //model
            var model = await _catalogModelFactory.PrepareCategoryModelAsync(category, command);

            //template
            var templateViewPath = await _catalogModelFactory.PrepareCategoryTemplateViewPathAsync(category.CategoryTemplateId);
            return View(templateViewPath, model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetCategoryTvChannels(int categoryId, CatalogTvChannelsCommand command)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (!await CheckCategoryAvailabilityAsync(category))
                return NotFound();

            var model = await _catalogModelFactory.PrepareCategoryTvChannelsModelAsync(category, command);

            return PartialView("_TvChannelsInGridOrLines", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetCatalogRoot()
        {
            var model = await _catalogModelFactory.PrepareRootCategoriesAsync();

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetCatalogSubCategories(int id)
        {
            var model = await _catalogModelFactory.PrepareSubCategoriesAsync(id);

            return Json(model);
        }

        #endregion

        #region Manufacturers

        public virtual async Task<IActionResult> Manufacturer(int manufacturerId, CatalogTvChannelsCommand command)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);

            if (!await CheckManufacturerAvailabilityAsync(manufacturer))
                return InvokeHttp404();

            var store = await _storeContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentUserAsync(),
                TvProgUserDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                store.Id);

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers))
                DisplayEditLink(Url.Action("Edit", "Manufacturer", new { id = manufacturer.Id, area = AreaNames.Admin }));

            //activity log
            await _userActivityService.InsertActivityAsync("PublicStore.ViewManufacturer",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewManufacturer"), manufacturer.Name), manufacturer);

            //model
            var model = await _catalogModelFactory.PrepareManufacturerModelAsync(manufacturer, command);

            //template
            var templateViewPath = await _catalogModelFactory.PrepareManufacturerTemplateViewPathAsync(manufacturer.ManufacturerTemplateId);

            return View(templateViewPath, model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetManufacturerTvChannels(int manufacturerId, CatalogTvChannelsCommand command)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);

            if (!await CheckManufacturerAvailabilityAsync(manufacturer))
                return NotFound();

            var model = await _catalogModelFactory.PrepareManufacturerTvChannelsModelAsync(manufacturer, command);

            return PartialView("_TvChannelsInGridOrLines", model);
        }

        public virtual async Task<IActionResult> ManufacturerAll()
        {
            var model = await _catalogModelFactory.PrepareManufacturerAllModelsAsync();

            return View(model);
        }

        #endregion

        #region Vendors

        public virtual async Task<IActionResult> Vendor(int vendorId, CatalogTvChannelsCommand command)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(vendorId);

            if (!await CheckVendorAvailabilityAsync(vendor))
                return InvokeHttp404();

            var store = await _storeContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentUserAsync(),
                TvProgUserDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(false),
                store.Id);

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageVendors))
                DisplayEditLink(Url.Action("Edit", "Vendor", new { id = vendor.Id, area = AreaNames.Admin }));

            //model
            var model = await _catalogModelFactory.PrepareVendorModelAsync(vendor, command);

            return View(model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetVendorTvChannels(int vendorId, CatalogTvChannelsCommand command)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(vendorId);

            if (!await CheckVendorAvailabilityAsync(vendor))
                return NotFound();

            var model = await _catalogModelFactory.PrepareVendorTvChannelsModelAsync(vendor, command);

            return PartialView("_TvChannelsInGridOrLines", model);
        }

        public virtual async Task<IActionResult> VendorAll()
        {
            //we don't allow viewing of vendors if "vendors" block is hidden
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return RedirectToRoute("Homepage");

            var model = await _catalogModelFactory.PrepareVendorAllModelsAsync();
            return View(model);
        }

        #endregion

        #region TvChannel tags

        public virtual async Task<IActionResult> TvChannelsByTag(int tvchannelTagId, CatalogTvChannelsCommand command)
        {
            var tvchannelTag = await _tvchannelTagService.GetTvChannelTagByIdAsync(tvchannelTagId);
            if (tvchannelTag == null)
                return InvokeHttp404();

            var model = await _catalogModelFactory.PrepareTvChannelsByTagModelAsync(tvchannelTag, command);

            return View(model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetTagTvChannels(int tagId, CatalogTvChannelsCommand command)
        {
            var tvchannelTag = await _tvchannelTagService.GetTvChannelTagByIdAsync(tagId);
            if (tvchannelTag == null)
                return NotFound();

            var model = await _catalogModelFactory.PrepareTagTvChannelsModelAsync(tvchannelTag, command);

            return PartialView("_TvChannelsInGridOrLines", model);
        }

        public virtual async Task<IActionResult> TvChannelTagsAll()
        {
            var model = await _catalogModelFactory.PreparePopularTvChannelTagsModelAsync();

            return View(model);
        }

        #endregion

        #region New (recently added) tvchannels page

        public virtual async Task<IActionResult> NewTvChannels(CatalogTvChannelsCommand command)
        {
            if (!_catalogSettings.NewTvChannelsEnabled)
                return InvokeHttp404();

            var model = new NewTvChannelsModel
            {
                CatalogTvChannelsModel = await _catalogModelFactory.PrepareNewTvChannelsModelAsync(command)
            };

            return View(model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetNewTvChannels(CatalogTvChannelsCommand command)
        {
            if (!_catalogSettings.NewTvChannelsEnabled)
                return NotFound();

            var model = await _catalogModelFactory.PrepareNewTvChannelsModelAsync(command);

            return PartialView("_TvChannelsInGridOrLines", model);
        }

        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> NewTvChannelsRss()
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var feed = new RssFeed(
                $"{await _localizationService.GetLocalizedAsync(store, x => x.Name)}: New tvchannels",
                "Information about tvchannels",
                new Uri(_webHelper.GetStoreLocation()),
                DateTime.UtcNow);

            if (!_catalogSettings.NewTvChannelsEnabled)
                return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));

            var items = new List<RssItem>();

            var storeId = store.Id;
            var tvchannels = await _tvchannelService.GetTvChannelsMarkedAsNewAsync(storeId: storeId);

            foreach (var tvchannel in tvchannels)
            {
                var seName = await _urlRecordService.GetSeNameAsync(tvchannel);
                var tvchannelUrl = await _nopUrlHelper.RouteGenericUrlAsync<TvChannel>(new { SeName = seName }, _webHelper.GetCurrentRequestProtocol());
                var tvchannelName = await _localizationService.GetLocalizedAsync(tvchannel, x => x.Name);
                var tvchannelDescription = await _localizationService.GetLocalizedAsync(tvchannel, x => x.ShortDescription);
                var item = new RssItem(tvchannelName, tvchannelDescription, new Uri(tvchannelUrl), $"urn:store:{store.Id}:newTvChannels:tvchannel:{tvchannel.Id}", tvchannel.CreatedOnUtc);
                items.Add(item);
                //uncomment below if you want to add RSS enclosure for pictures
                //var picture = _pictureService.GetPicturesByTvChannelId(tvchannel.Id, 1).FirstOrDefault();
                //if (picture != null)
                //{
                //    var imageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.TvChannelDetailsPictureSize);
                //    item.ElementExtensions.Add(new XElement("enclosure", new XAttribute("type", "image/jpeg"), new XAttribute("url", imageUrl), new XAttribute("length", picture.PictureBinary.Length)));
                //}

            }
            feed.Items = items;
            return new RssActionResult(feed, _webHelper.GetThisPageUrl(false));
        }

        #endregion

        #region Searching

        public virtual async Task<IActionResult> Search(SearchModel model, CatalogTvChannelsCommand command)
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            //'Continue shopping' URL
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentUserAsync(),
                TvProgUserDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(true),
                store.Id);

            if (model == null)
                model = new SearchModel();

            model = await _catalogModelFactory.PrepareSearchModelAsync(model, command);

            return View(model);
        }

        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> SearchTermAutoComplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Content("");

            term = term.Trim();

            if (string.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.TvChannelSearchTermMinimumLength)
                return Content("");

            //tvchannels
            var tvchannelNumber = _catalogSettings.TvChannelSearchAutoCompleteNumberOfTvChannels > 0 ?
                _catalogSettings.TvChannelSearchAutoCompleteNumberOfTvChannels : 10;
            var store = await _storeContext.GetCurrentStoreAsync();
            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(0,
                storeId: store.Id,
                keywords: term,
                languageId: (await _workContext.GetWorkingLanguageAsync()).Id,
                visibleIndividuallyOnly: true,
                pageSize: tvchannelNumber);

            var showLinkToResultSearch = _catalogSettings.ShowLinkToAllResultInSearchAutoComplete && (tvchannels.TotalCount > tvchannelNumber);

            var models = (await _tvchannelModelFactory.PrepareTvChannelOverviewModelsAsync(tvchannels, false, _catalogSettings.ShowTvChannelImagesInSearchAutoComplete, _mediaSettings.AutoCompleteSearchThumbPictureSize)).ToList();
            var result = (from p in models
                          select new
                          {
                              label = p.Name,
                              tvchannelurl = Url.RouteUrl<TvChannel>(new { SeName = p.SeName }),
                              tvchannelpictureurl = p.PictureModels.FirstOrDefault()?.ImageUrl,
                              showlinktoresultsearch = showLinkToResultSearch
                          })
                .ToList();
            return Json(result);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> SearchTvChannels(SearchModel searchModel, CatalogTvChannelsCommand command)
        {
            if (searchModel == null)
                searchModel = new SearchModel();

            var model = await _catalogModelFactory.PrepareSearchTvChannelsModelAsync(searchModel, command);

            return PartialView("_TvChannelsInGridOrLines", model);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GlobalSearchProgramme(int progType, string findTitle, string category,
                                                        string sidx, string sord, int page, int rows, string genres, string channels)
        {
            object jsonData;
            KeyValuePair<int, List<SystemProgramme>> result;
            if (User.Identity.IsAuthenticated)
            {
                /*result = await progRepository.SearchUserProgramme(UserId.Value, progType, findTitle
                    , (category != "null") ? category : null, sidx, sord, page, rows, genres, dates);

                jsonData = ControllerExtensions.GetJsonPagingInfo(page, rows, result);
                return Json(jsonData, JsonRequestBehavior.AllowGet);*/
            }
            result = await _programmeService.SearchGlobalProgrammeAsync(progType, findTitle
                , (category != "Все категории") ? category : null, sidx, sord, page, rows, genres, channels);
            jsonData = GetJsonPagingInfo(page, rows, result);
            return Json(jsonData);
        }
        
        /// <summary>
        /// Постраничное отображение
        /// </summary>
        /// <typeparam name="T">Тип список программы</typeparam>
        /// <param name="page">Страница</param>
        /// <param name="rows">Количество строк</param>
        /// <param name="result">Тип для постраничного отображения</param>
        /// <returns></returns>
        public static object GetJsonPagingInfo<T>(int page, int rows, KeyValuePair<int, T> result)
        {
            int pageSize = rows;
            int totalRecords = result.Key;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = result.Value
            };
            return jsonData;
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetGenres()
        {
            return Json(await _genreService.GetGenresAsync(null, true));
        }
        #endregion

        #region Utilities

        private async Task<bool> CheckCategoryAvailabilityAsync(Category category)
        {
            if (category is null)
                return false;

            var isAvailable = true;

            if (category.Deleted)
                isAvailable = false;

            var notAvailable =
                //published?
                !category.Published ||
                //ACL (access control list) 
                !await _aclService.AuthorizeAsync(category) ||
                //Store mapping
                !await _storeMappingService.AuthorizeAsync(category);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories);
            if (notAvailable && !hasAdminAccess)
                isAvailable = false;

            return isAvailable;
        }

        private async Task<bool> CheckManufacturerAvailabilityAsync(Manufacturer manufacturer)
        {
            var isAvailable = true;

            if (manufacturer == null || manufacturer.Deleted)
                isAvailable = false;

            var notAvailable =
                //published?
                !manufacturer.Published ||
                //ACL (access control list) 
                !await _aclService.AuthorizeAsync(manufacturer) ||
                //Store mapping
                !await _storeMappingService.AuthorizeAsync(manufacturer);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageManufacturers);
            if (notAvailable && !hasAdminAccess)
                isAvailable = false;

            return isAvailable;
        }

        private Task<bool> CheckVendorAvailabilityAsync(Vendor vendor)
        {
            var isAvailable = true;

            if (vendor == null || vendor.Deleted || !vendor.Active)
                isAvailable = false;

            return Task.FromResult(isAvailable);
        }

        #endregion
    }
}