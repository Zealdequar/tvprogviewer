using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core.Domain.Affiliates;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Services.Affiliates;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Common;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Helpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Orders;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Affiliates;
using TVProgViewer.WebUI.Areas.Admin.Models.Common;
using TVProgViewer.Web.Framework.Models.Extensions;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the affiliate model factory implementation
    /// </summary>
    public partial class AffiliateModelFactory : IAffiliateModelFactory
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly IAffiliateService _affiliateService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IStateProvinceService _stateProvinceService;

        #endregion

        #region Ctor

        public AffiliateModelFactory(IAddressService addressService,
            IAffiliateService affiliateService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IOrderService orderService,
            IPriceFormatter priceFormatter,
            IStateProvinceService stateProvinceService)
        {
            _addressService = addressService;
            _affiliateService = affiliateService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _countryService = countryService;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _orderService = orderService;
            _priceFormatter = priceFormatter;
            _stateProvinceService = stateProvinceService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        protected virtual async Task PrepareAddressModelAsync(AddressModel model, Address address = null)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //set all address fields as enabled and required
            model.FirstNameEnabled = true;
            model.FirstNameRequired = true;
            model.LastNameEnabled = true;
            model.LastNameRequired = true;
            model.EmailEnabled = true;
            model.EmailRequired = true;
            model.CompanyEnabled = true;
            model.CountryEnabled = true;
            model.CountryRequired = true;
            model.StateProvinceEnabled = true;
            model.CountyEnabled = true;
            model.CountyRequired = true;
            model.CityEnabled = true;
            model.CityRequired = true;
            model.StreetAddressEnabled = true;
            model.StreetAddressRequired = true;
            model.StreetAddress2Enabled = true;
            model.ZipPostalCodeEnabled = true;
            model.ZipPostalCodeRequired = true;
            model.PhoneEnabled = true;
            model.PhoneRequired = true;
            model.FaxEnabled = true;

            //prepare available countries
            await _baseAdminModelFactory.PrepareCountriesAsync(model.AvailableCountries);

            //prepare available states
            await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(model.AvailableStates, model.CountryId);
        }

        /// <summary>
        /// Prepare affiliated order search model
        /// </summary>
        /// <param name="searchModel">Affiliated order search model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Affiliated order search model</returns>
        protected virtual async Task<AffiliatedOrderSearchModel> PrepareAffiliatedOrderSearchModelAsync(AffiliatedOrderSearchModel searchModel, Affiliate affiliate)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            searchModel.AffliateId = affiliate.Id;

            //prepare available order, payment and shipping statuses
            await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
            await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);
            await _baseAdminModelFactory.PrepareShippingStatusesAsync(searchModel.AvailableShippingStatuses);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare affiliated user search model
        /// </summary>
        /// <param name="searchModel">Affiliated user search model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Affiliated user search model</returns>
        protected virtual AffiliatedUserSearchModel PrepareAffiliatedUserSearchModel(AffiliatedUserSearchModel searchModel, Affiliate affiliate)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            searchModel.AffliateId = affiliate.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare affiliate search model
        /// </summary>
        /// <param name="searchModel">Affiliate search model</param>
        /// <returns>Affiliate search model</returns>
        public virtual Task<AffiliateSearchModel> PrepareAffiliateSearchModelAsync(AffiliateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged affiliate list model
        /// </summary>
        /// <param name="searchModel">Affiliate search model</param>
        /// <returns>Affiliate list model</returns>
        public virtual async Task<AffiliateListModel> PrepareAffiliateListModelAsync(AffiliateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get affiliates
            var affiliates = await _affiliateService.GetAllAffiliatesAsync(searchModel.SearchFriendlyUrlName,
                searchModel.SearchFirstName,
                searchModel.SearchLastName,
                searchModel.LoadOnlyWithOrders,
                searchModel.OrdersCreatedFromUtc,
                searchModel.OrdersCreatedToUtc,
                searchModel.Page - 1, searchModel.PageSize, true);

            //prepare list model
            var model = await new AffiliateListModel().PrepareToGridAsync(searchModel, affiliates, () =>
            {
                //fill in model values from the entity
                return affiliates.SelectAwait(async affiliate =>
                {
                    var address = await _addressService.GetAddressByIdAsync(affiliate.AddressId);

                    var affiliateModel = affiliate.ToModel<AffiliateModel>();
                    affiliateModel.Address = address.ToModel<AddressModel>();
                    affiliateModel.Address.CountryName = (await _countryService.GetCountryByAddressAsync(address))?.Name;
                    affiliateModel.Address.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(address))?.Name;

                    return affiliateModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare affiliate model
        /// </summary>
        /// <param name="model">Affiliate model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Affiliate model</returns>
        public virtual async Task<AffiliateModel> PrepareAffiliateModelAsync(AffiliateModel model, Affiliate affiliate, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (affiliate != null)
            {
                model ??= affiliate.ToModel<AffiliateModel>();
                model.Url = await _affiliateService.GenerateUrlAsync(affiliate);

                //prepare nested search models
                await PrepareAffiliatedOrderSearchModelAsync(model.AffiliatedOrderSearchModel, affiliate);
                PrepareAffiliatedUserSearchModel(model.AffiliatedUserSearchModel, affiliate);

                //prepare address model
                var address = await _addressService.GetAddressByIdAsync(affiliate.AddressId);
                model.Address = address.ToModel(model.Address);
                await PrepareAddressModelAsync(model.Address, address);

                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    model.AdminComment = affiliate.AdminComment;
                    model.FriendlyUrlName = affiliate.FriendlyUrlName;
                    model.Active = affiliate.Active;
                }
            }
            else
            {
                await PrepareAddressModelAsync(model.Address);
            }

            return model;
        }

        /// <summary>
        /// Prepare paged affiliated order list model
        /// </summary>
        /// <param name="searchModel">Affiliated order search model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Affiliated order list model</returns>
        public virtual async Task<AffiliatedOrderListModel> PrepareAffiliatedOrderListModelAsync(AffiliatedOrderSearchModel searchModel, Affiliate affiliate)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            //get parameters to filter orders
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
            var orderStatusIds = searchModel.OrderStatusId > 0 ? new List<int> { searchModel.OrderStatusId } : null;
            var paymentStatusIds = searchModel.PaymentStatusId > 0 ? new List<int> { searchModel.PaymentStatusId } : null;
            var shippingStatusIds = searchModel.ShippingStatusId > 0 ? new List<int> { searchModel.ShippingStatusId } : null;

            //get orders
            var orders = await _orderService.SearchOrdersAsync(createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                affiliateId: affiliate.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new AffiliatedOrderListModel().PrepareToGridAsync(searchModel, orders, () =>
            {
                //fill in model values from the entity
                return orders.SelectAwait(async order =>
                {
                    var affiliatedOrderModel = order.ToModel<AffiliatedOrderModel>();

                    //fill in additional values (not existing in the entity)
                    affiliatedOrderModel.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                    affiliatedOrderModel.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
                    affiliatedOrderModel.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);
                    affiliatedOrderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false);

                    affiliatedOrderModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

                    return affiliatedOrderModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged affiliated user list model
        /// </summary>
        /// <param name="searchModel">Affiliated user search model</param>
        /// <param name="affiliate">Affiliate</param>
        /// <returns>Affiliated user list model</returns>
        public virtual async Task<AffiliatedUserListModel> PrepareAffiliatedUserListModelAsync(AffiliatedUserSearchModel searchModel,
            Affiliate affiliate)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            //get users
            var users = await _userService.GetAllUsersAsync(affiliateId: affiliate.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new AffiliatedUserListModel().PrepareToGrid(searchModel, users, () =>
            {
                //fill in model values from the entity
                return users.Select(user =>
                {
                    var affiliatedUserModel = user.ToModel<AffiliatedUserModel>();
                    affiliatedUserModel.Name = user.Email;

                    return affiliatedUserModel;
                });
            });

            return model;
        }

        #endregion
    }
}