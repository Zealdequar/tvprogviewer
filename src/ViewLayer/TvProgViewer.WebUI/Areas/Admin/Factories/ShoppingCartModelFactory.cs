using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Stores;
using TvProgViewer.Services.Tax;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.ShoppingCart;
using TvProgViewer.Web.Framework.Extensions;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the shopping cart model factory implementation
    /// </summary>
    public partial class ShoppingCartModelFactory : IShoppingCartModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITvChannelAttributeFormatter _tvChannelAttributeFormatter;
        private readonly ITvChannelService _tvChannelService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;

        #endregion

        #region Ctor

        public ShoppingCartModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IPriceFormatter priceFormatter,
            ITvChannelAttributeFormatter tvChannelAttributeFormatter,
            ITvChannelService tvChannelService,
            IShoppingCartService shoppingCartService,
            IStoreService storeService,
            ITaxService taxService)
        {
            _catalogSettings = catalogSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _countryService = countryService;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _priceFormatter = priceFormatter;
            _tvChannelAttributeFormatter = tvChannelAttributeFormatter;
            _tvChannelService = tvChannelService;
            _shoppingCartService = shoppingCartService;
            _storeService = storeService;
            _taxService = taxService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare shopping cart item search model
        /// </summary>
        /// <param name="searchModel">Shopping cart item search model</param>
        /// <returns>Shopping cart item search model</returns>
        protected virtual ShoppingCartItemSearchModel PrepareShoppingCartItemSearchModel(ShoppingCartItemSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare shopping cart search model
        /// </summary>
        /// <param name="searchModel">Shopping cart search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shopping cart search model
        /// </returns>
        public virtual async Task<ShoppingCartSearchModel> PrepareShoppingCartSearchModelAsync(ShoppingCartSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available shopping cart types
            await _baseAdminModelFactory.PrepareShoppingCartTypesAsync(searchModel.AvailableShoppingCartTypes, false);

            //set default search values
            searchModel.ShoppingCartType = ShoppingCartType.ShoppingCart;

            //prepare available billing countries
            searchModel.AvailableCountries = (await _countryService.GetAllCountriesForBillingAsync(showHidden: true))
                .Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() }).ToList();
            searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare nested search model
            PrepareShoppingCartItemSearchModel(searchModel.ShoppingCartItemSearchModel);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged shopping cart list model
        /// </summary>
        /// <param name="searchModel">Shopping cart search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shopping cart list model
        /// </returns>
        public virtual async Task<ShoppingCartListModel> PrepareShoppingCartListModelAsync(ShoppingCartSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get users with shopping carts
            var users = await _userService.GetUsersWithShoppingCartsAsync(searchModel.ShoppingCartType,
                storeId: searchModel.StoreId,
                tvChannelId: searchModel.TvChannelId,
                createdFromUtc: searchModel.StartDate,
                createdToUtc: searchModel.EndDate,
                countryId: searchModel.BillingCountryId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new ShoppingCartListModel().PrepareToGridAsync(searchModel, users, () =>
            {
                return users.SelectAwait(async user =>
                {
                    //fill in model values from the entity
                    var shoppingCartModel = new ShoppingCartModel
                    {
                        UserId = user.Id
                    };

                    //fill in additional values (not existing in the entity)
                    shoppingCartModel.UserEmail = (await _userService.IsRegisteredAsync(user))
                        ? user.Email
                        : await _localizationService.GetResourceAsync("Admin.Users.Guest");
                    shoppingCartModel.TotalItems = (await _shoppingCartService
                        .GetShoppingCartAsync(user, searchModel.ShoppingCartType,
                            searchModel.StoreId, searchModel.TvChannelId, searchModel.StartDate, searchModel.EndDate))
                        .Sum(item => item.Quantity);

                    return shoppingCartModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged shopping cart item list model
        /// </summary>
        /// <param name="searchModel">Shopping cart item search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shopping cart item list model
        /// </returns>
        public virtual async Task<ShoppingCartItemListModel> PrepareShoppingCartItemListModelAsync(ShoppingCartItemSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //get shopping cart items
            var items = (await _shoppingCartService.GetShoppingCartAsync(user, searchModel.ShoppingCartType,
                searchModel.StoreId, searchModel.TvChannelId, searchModel.StartDate, searchModel.EndDate)).ToPagedList(searchModel);

            var isSearchTvChannel = searchModel.TvChannelId > 0;

            TvChannel tvChannel = null;

            if (isSearchTvChannel)
            {
                tvChannel = await _tvChannelService.GetTvChannelByIdAsync(searchModel.TvChannelId) ?? throw new Exception("TvChannel is not found");
            }

            var store = await _storeService.GetStoreByIdAsync(searchModel.StoreId);
            //prepare list model
            var model = await new ShoppingCartItemListModel().PrepareToGridAsync(searchModel, items, () =>
            {
                return items
                .OrderByDescending(item => item.CreatedOnUtc)
                .SelectAwait(async item =>
                {
                    //fill in model values from the entity
                    var itemModel = item.ToModel<ShoppingCartItemModel>();

                    if (!isSearchTvChannel)
                        tvChannel = await _tvChannelService.GetTvChannelByIdAsync(item.TvChannelId);

                    //convert dates to the user time
                    itemModel.UpdatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(item.UpdatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    itemModel.Store = (await _storeService.GetStoreByIdAsync(item.StoreId))?.Name ?? "Deleted";
                    itemModel.AttributeInfo = await _tvChannelAttributeFormatter.FormatAttributesAsync(tvChannel, item.AttributesXml, user, store);
                    var (unitPrice, _, _) = await _shoppingCartService.GetUnitPriceAsync(item, true);
                    itemModel.UnitPrice = await _priceFormatter.FormatPriceAsync((await _taxService.GetTvChannelPriceAsync(tvChannel, unitPrice)).price);
                    itemModel.UnitPriceValue = (await _taxService.GetTvChannelPriceAsync(tvChannel, unitPrice)).price;
                    var (subTotal, _, _, _) = await _shoppingCartService.GetSubTotalAsync(item, true);
                    itemModel.Total = await _priceFormatter.FormatPriceAsync((await _taxService.GetTvChannelPriceAsync(tvChannel, subTotal)).price);
                    itemModel.TotalValue = (await _taxService.GetTvChannelPriceAsync(tvChannel, subTotal)).price;

                    //set tvChannel name since it does not survive mapping
                    itemModel.TvChannelName = tvChannel.Name;

                    return itemModel;
                });
            });

            return model;
        }

        #endregion
    }
}