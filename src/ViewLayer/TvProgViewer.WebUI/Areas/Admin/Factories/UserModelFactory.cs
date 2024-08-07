using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Services.Affiliates;
using TvProgViewer.Services.Authentication.External;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Gdpr;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Stores;
using TvProgViewer.Services.Tax;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Common;
using TvProgViewer.WebUI.Areas.Admin.Models.Users;
using TvProgViewer.WebUI.Areas.Admin.Models.ShoppingCart;
using TvProgViewer.Web.Framework.Factories;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the user model factory implementation
    /// </summary>
    public partial class UserModelFactory : IUserModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly UserSettings _userSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly GdprSettings _gdprSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAffiliateService _affiliateService;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserAttributeParser _userAttributeParser;
        private readonly IUserAttributeService _userAttributeService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGeoLookupService _geoLookupService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITvChannelAttributeFormatter _tvchannelAttributeFormatter;
        private readonly ITvChannelService _tvchannelService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;
        private readonly MediaSettings _mediaSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public UserModelFactory(AddressSettings addressSettings,
            UserSettings userSettings,
            DateTimeSettings dateTimeSettings,
            GdprSettings gdprSettings,
            ForumSettings forumSettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressModelFactory addressModelFactory,
            IAffiliateService affiliateService,
            IAuthenticationPluginManager authenticationPluginManager,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            IUserActivityService userActivityService,
            IUserAttributeParser userAttributeParser,
            IUserAttributeService userAttributeService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            IGeoLookupService geoLookupService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            ITvChannelAttributeFormatter tvchannelAttributeFormatter,
            ITvChannelService tvchannelService,
            IRewardPointService rewardPointService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreService storeService,
            ITaxService taxService,
            MediaSettings mediaSettings,
            RewardPointsSettings rewardPointsSettings,
            TaxSettings taxSettings)
        {
            _addressSettings = addressSettings;
            _userSettings = userSettings;
            _dateTimeSettings = dateTimeSettings;
            _gdprSettings = gdprSettings;
            _forumSettings = forumSettings;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _addressAttributeFormatter = addressAttributeFormatter;
            _addressModelFactory = addressModelFactory;
            _affiliateService = affiliateService;
            _authenticationPluginManager = authenticationPluginManager;
            _backInStockSubscriptionService = backInStockSubscriptionService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _countryService = countryService;
            _userActivityService = userActivityService;
            _userAttributeParser = userAttributeParser;
            _userAttributeService = userAttributeService;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _externalAuthenticationService = externalAuthenticationService;
            _gdprService = gdprService;
            _genericAttributeService = genericAttributeService;
            _geoLookupService = geoLookupService;
            _localizationService = localizationService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _orderService = orderService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _tvchannelAttributeFormatter = tvchannelAttributeFormatter;
            _tvchannelService = tvchannelService;
            _rewardPointService = rewardPointService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _storeService = storeService;
            _taxService = taxService;
            _mediaSettings = mediaSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the reward points model to add to the user
        /// </summary>
        /// <param name="model">Reward points model to add to the user</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task PrepareAddRewardPointsToUserModelAsync(AddRewardPointsToUserModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var store = await _storeContext.GetCurrentStoreAsync();

            model.Message = string.Empty;
            model.ActivatePointsImmediately = true;
            model.StoreId = store.Id;

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores, false);
        }

        /// <summary>
        /// Prepare user associated external authorization models
        /// </summary>
        /// <param name="models">List of user associated external authorization models</param>
        /// <param name="user">User</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task PrepareAssociatedExternalAuthModelsAsync(IList<UserAssociatedExternalAuthModel> models, User user)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            foreach (var record in await _externalAuthenticationService.GetUserExternalAuthenticationRecordsAsync(user))
            {
                var method = await _authenticationPluginManager.LoadPluginBySystemNameAsync(record.ProviderSystemName);
                if (method == null)
                    continue;

                models.Add(new UserAssociatedExternalAuthModel
                {
                    Id = record.Id,
                    Email = record.Email,
                    ExternalIdentifier = !string.IsNullOrEmpty(record.ExternalDisplayIdentifier)
                        ? record.ExternalDisplayIdentifier : record.ExternalIdentifier,
                    AuthMethodName = method.PluginDescriptor.FriendlyName
                });
            }
        }

        /// <summary>
        /// Prepare user attribute models
        /// </summary>
        /// <param name="models">List of user attribute models</param>
        /// <param name="user">User</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task PrepareUserAttributeModelsAsync(IList<UserModel.UserAttributeModel> models, User user)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //get available user attributes
            var userAttributes = await _userAttributeService.GetAllUserAttributesAsync();
            foreach (var attribute in userAttributes)
            {
                var attributeModel = new UserModel.UserAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _userAttributeService.GetUserAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new UserModel.UserAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                if (user != null)
                {
                    var selectedUserAttributes = user.CustomUserAttributesXML;
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                            {
                                if (!string.IsNullOrEmpty(selectedUserAttributes))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = await _userAttributeParser.ParseUserAttributeValuesAsync(selectedUserAttributes);
                                    foreach (var attributeValue in selectedValues)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                                item.IsPreSelected = true;
                                }
                            }
                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                            {
                                //do nothing
                                //values are already pre-set
                            }
                            break;
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                            {
                                if (!string.IsNullOrEmpty(selectedUserAttributes))
                                {
                                    var enteredText = _userAttributeParser.ParseValues(selectedUserAttributes, attribute.Id);
                                    if (enteredText.Any())
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }
                            break;
                        case AttributeControlType.Datepicker:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                        case AttributeControlType.FileUpload:
                        default:
                            //not supported attribute control types
                            break;
                    }
                }

                models.Add(attributeModel);
            }
        }

        /// <summary>
        /// Prepare HTML string address
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task PrepareModelAddressHtmlAsync(AddressModel model, Address address)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var addressHtmlSb = new StringBuilder("<div>");

            if (_addressSettings.CompanyEnabled && !string.IsNullOrEmpty(model.Company))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Company));

            if (_addressSettings.StreetAddressEnabled && !string.IsNullOrEmpty(model.Address1))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Address1));

            if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(model.Address2))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Address2));

            if (_addressSettings.CityEnabled && !string.IsNullOrEmpty(model.City))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.City));

            if (_addressSettings.CountyEnabled && !string.IsNullOrEmpty(model.County))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.County));

            if (_addressSettings.StateProvinceEnabled && !string.IsNullOrEmpty(model.StateProvinceName))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.StateProvinceName));

            if (_addressSettings.ZipPostalCodeEnabled && !string.IsNullOrEmpty(model.ZipPostalCode))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.ZipPostalCode));

            if (_addressSettings.CountryEnabled && !string.IsNullOrEmpty(model.CountryName))
                addressHtmlSb.AppendFormat("{0}", WebUtility.HtmlEncode(model.CountryName));

            var customAttributesFormatted = await _addressAttributeFormatter.FormatAttributesAsync(address?.CustomAttributes);
            if (!string.IsNullOrEmpty(customAttributesFormatted))
            {
                //already encoded
                addressHtmlSb.AppendFormat("<br />{0}", customAttributesFormatted);
            }

            addressHtmlSb.Append("</div>");

            model.AddressHtml = addressHtmlSb.ToString();
        }

        /// <summary>
        /// Prepare reward points search model
        /// </summary>
        /// <param name="searchModel">Reward points search model</param>
        /// <param name="user">User</param>
        /// <returns>Reward points search model</returns>
        protected virtual UserRewardPointsSearchModel PrepareRewardPointsSearchModel(UserRewardPointsSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            searchModel.UserId = user.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare user address search model
        /// </summary>
        /// <param name="searchModel">User address search model</param>
        /// <param name="user">User</param>
        /// <returns>User address search model</returns>
        protected virtual UserAddressSearchModel PrepareUserAddressSearchModel(UserAddressSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            searchModel.UserId = user.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare user order search model
        /// </summary>
        /// <param name="searchModel">User order search model</param>
        /// <param name="user">User</param>
        /// <returns>User order search model</returns>
        protected virtual UserOrderSearchModel PrepareUserOrderSearchModel(UserOrderSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            searchModel.UserId = user.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare user shopping cart search model
        /// </summary>
        /// <param name="searchModel">User shopping cart search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user shopping cart search model
        /// </returns>
        protected virtual async Task<UserShoppingCartSearchModel> PrepareUserShoppingCartSearchModelAsync(UserShoppingCartSearchModel searchModel,
            User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            searchModel.UserId = user.Id;

            //prepare available shopping cart types (search shopping cart by default)
            searchModel.ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart;
            await _baseAdminModelFactory.PrepareShoppingCartTypesAsync(searchModel.AvailableShoppingCartTypes, false);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare user activity log search model
        /// </summary>
        /// <param name="searchModel">User activity log search model</param>
        /// <param name="user">User</param>
        /// <returns>User activity log search model</returns>
        protected virtual UserActivityLogSearchModel PrepareUserActivityLogSearchModel(UserActivityLogSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            searchModel.UserId = user.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare user back in stock subscriptions search model
        /// </summary>
        /// <param name="searchModel">User back in stock subscriptions search model</param>
        /// <param name="user">User</param>
        /// <returns>User back in stock subscriptions search model</returns>
        protected virtual UserBackInStockSubscriptionSearchModel PrepareUserBackInStockSubscriptionSearchModel(
            UserBackInStockSubscriptionSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            searchModel.UserId = user.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare user back in stock subscriptions search model
        /// </summary>
        /// <param name="searchModel">User back in stock subscriptions search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user back in stock subscriptions search model
        /// </returns>
        protected virtual async Task<UserAssociatedExternalAuthRecordsSearchModel> PrepareUserAssociatedExternalAuthRecordsSearchModelAsync(
            UserAssociatedExternalAuthRecordsSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            searchModel.UserId = user.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();
            //prepare external authentication records
            await PrepareAssociatedExternalAuthModelsAsync(searchModel.AssociatedExternalAuthRecords, user);

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare user search model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user search model
        /// </returns>
        public virtual async Task<UserSearchModel> PrepareUserSearchModelAsync(UserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.UsernamesEnabled = _userSettings.UsernamesEnabled;
            searchModel.AvatarEnabled = _userSettings.AllowUsersToUploadAvatars;
            searchModel.FirstNameEnabled = _userSettings.FirstNameEnabled;
            searchModel.LastNameEnabled = _userSettings.LastNameEnabled;
            searchModel.MiddleNameEnabled = _userSettings.MiddleNameEnabled;
            searchModel.BirthDateEnabled = _userSettings.BirthDateEnabled;
            searchModel.CompanyEnabled = _userSettings.CompanyEnabled;
            searchModel.SmartPhoneEnabled = _userSettings.SmartPhoneEnabled;
            searchModel.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;

            //search registered users by default
            var registeredRole = await _userService.GetUserRoleBySystemNameAsync(TvProgUserDefaults.RegisteredRoleName);
            if (registeredRole != null)
                searchModel.SelectedUserRoleIds.Add(registeredRole.Id);

            //prepare available user roles
            await _aclSupportedModelFactory.PrepareModelUserRolesAsync(searchModel);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged user list model
        /// </summary>
        /// <param name="searchModel">User search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user list model
        /// </returns>
        public virtual async Task<UserListModel> PrepareUserListModelAsync(UserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter users
            _ = int.TryParse(searchModel.SearchDayOfBirth, out var dayOfBirth);
            _ = int.TryParse(searchModel.SearchMonthOfBirth, out var monthOfBirth);
            var createdFromUtc = !searchModel.SearchRegistrationDateFrom.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchRegistrationDateFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var createdToUtc = !searchModel.SearchRegistrationDateTo.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchRegistrationDateTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
            var lastActivityFromUtc = !searchModel.SearchLastActivityFrom.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchLastActivityFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var lastActivityToUtc = !searchModel.SearchLastActivityTo.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchLastActivityTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            //exclude guests from the result when filter "by registration date" is used
            if (createdFromUtc.HasValue || createdToUtc.HasValue)
            {
                if (!searchModel.SelectedUserRoleIds.Any())
                {
                    var userRoles = await _userService.GetAllUserRolesAsync(showHidden: true);
                    searchModel.SelectedUserRoleIds = userRoles
                        .Where(cr => cr.SystemName != TvProgUserDefaults.GuestsRoleName).Select(cr => cr.Id).ToList();
                }
                else
                {
                    var guestRole = await _userService.GetUserRoleBySystemNameAsync(TvProgUserDefaults.GuestsRoleName);
                    if (guestRole != null)
                        searchModel.SelectedUserRoleIds.Remove(guestRole.Id);
                }
            }

            //get users
            var users = await _userService.GetAllUsersAsync(userRoleIds: searchModel.SelectedUserRoleIds.ToArray(),
                email: searchModel.SearchEmail,
                username: searchModel.SearchUsername,
                firstName: searchModel.SearchFirstName,
                lastName: searchModel.SearchLastName,
                middleName: searchModel.SearchMiddleName,
                dayOfBirth: dayOfBirth,
                monthOfBirth: monthOfBirth,
                company: searchModel.SearchCompany,
                createdFromUtc: createdFromUtc,
                createdToUtc: createdToUtc,
                lastActivityFromUtc: lastActivityFromUtc,
                lastActivityToUtc: lastActivityToUtc,
                phone: searchModel.SearchSmartPhone,
                zipPostalCode: searchModel.SearchZipPostalCode,
                ipAddress: searchModel.SearchIpAddress,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new UserListModel().PrepareToGridAsync(searchModel, users, () =>
            {
                return users.SelectAwait(async user =>
                {
                    //fill in model values from the entity
                    var userModel = user.ToModel<UserModel>();

                    //convert dates to the user time
                    userModel.Email = (await _userService.IsRegisteredAsync(user))
                        ? user.Email
                        : await _localizationService.GetResourceAsync("Admin.Users.Guest");
                    userModel.FullName = await _userService.GetUserFullNameAsync(user);
                    userModel.Company = user.Company;
                    userModel.SmartPhone = user.SmartPhone;
                    userModel.ZipPostalCode = user.ZipPostalCode;

                    userModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(user.CreatedOnUtc, DateTimeKind.Utc);
                    userModel.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(user.LastActivityDateUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    userModel.UserRoleNames = string.Join(", ",
                        (await _userService.GetUserRolesAsync(user)).Select(role => role.Name));
                    if (_userSettings.AllowUsersToUploadAvatars)
                    {
                        var avatarPictureId = await _genericAttributeService.GetAttributeAsync<int>(user, TvProgUserDefaults.AvatarPictureIdAttribute);
                        userModel.AvatarUrl = await _pictureService
                            .GetPictureUrlAsync(avatarPictureId, _mediaSettings.AvatarPictureSize, _userSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
                    }

                    return userModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare user model
        /// </summary>
        /// <param name="model">User model</param>
        /// <param name="user">User</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user model
        /// </returns>
        public virtual async Task<UserModel> PrepareUserModelAsync(UserModel model, User user, bool excludeProperties = false)
        {
            if (user != null)
            {
                //fill in model values from the entity
                model ??= new UserModel();

                model.Id = user.Id;
                model.DisplayVatNumber = _taxSettings.EuVatEnabled;
                model.AllowSendingOfPrivateMessage = await _userService.IsRegisteredAsync(user) &&
                    _forumSettings.AllowPrivateMessages;
                model.AllowSendingOfWelcomeMessage = await _userService.IsRegisteredAsync(user) &&
                    _userSettings.UserRegistrationType == UserRegistrationType.AdminApproval;
                model.AllowReSendingOfActivationMessage = await _userService.IsRegisteredAsync(user) && !user.Active &&
                    _userSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
                model.GdprEnabled = _gdprSettings.GdprEnabled;

                model.MultiFactorAuthenticationProvider = await _genericAttributeService
                    .GetAttributeAsync<string>(user, TvProgUserDefaults.SelectedMultiFactorAuthenticationProviderAttribute);

                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    model.Email = user.Email;
                    model.Username = user.Username;
                    model.VendorId = user.VendorId;
                    model.AdminComment = user.AdminComment;
                    model.IsTaxExempt = user.IsTaxExempt;
                    model.Active = user.Active;
                    model.FirstName = user.FirstName;
                    model.LastName = user.LastName;
                    model.MiddleName = user.MiddleName;
                    model.Gender = user.Gender;
                    model.BirthDate = user.BirthDate;
                    model.Company = user.Company;
                    model.StreetAddress = user.StreetAddress;
                    model.StreetAddress2 = user.StreetAddress2;
                    model.ZipPostalCode = user.ZipPostalCode;
                    model.City = user.City;
                    model.County = user.County;
                    model.CountryId = user.CountryId;
                    model.StateProvinceId = user.StateProvinceId;
                    model.SmartPhone = user.SmartPhone;
                    model.Fax = user.Fax;
                    model.GmtZone = user.GmtZone;
                    model.VatNumber = user.VatNumber;
                    model.VatNumberStatusNote = await _localizationService.GetLocalizedEnumAsync(user.VatNumberStatus);
                    model.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(user.LastActivityDateUtc, DateTimeKind.Utc);
                    model.LastIpAddress = user.LastIpAddress;
                    model.LastVisitedPage = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.LastVisitedPageAttribute);
                    model.SelectedUserRoleIds = (await _userService.GetUserRoleIdsAsync(user)).ToList();
                    model.RegisteredInStore = (await _storeService.GetAllStoresAsync())
                        .FirstOrDefault(store => store.Id == user.RegisteredInStoreId)?.Name ?? string.Empty;
                    model.DisplayRegisteredInStore = model.Id > 0 && !string.IsNullOrEmpty(model.RegisteredInStore) &&
                        (await _storeService.GetAllStoresAsync()).Select(x => x.Id).Count() > 1;
                    model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(user.CreatedOnUtc, DateTimeKind.Utc);

                    //prepare model affiliate
                    var affiliate = await _affiliateService.GetAffiliateByIdAsync(user.AffiliateId);
                    if (affiliate != null)
                    {
                        model.AffiliateId = affiliate.Id;
                        model.AffiliateName = await _affiliateService.GetAffiliateFullNameAsync(affiliate);
                    }

                    //prepare model newsletter subscriptions
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        model.SelectedNewsletterSubscriptionStoreIds = await (await _storeService.GetAllStoresAsync())
                            .WhereAwait(async store => await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(user.Email, store.Id) != null)
                            .Select(store => store.Id).ToListAsync();
                    }
                }
                //prepare reward points model
                model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
                if (model.DisplayRewardPointsHistory)
                    await PrepareAddRewardPointsToUserModelAsync(model.AddRewardPoints);

                //prepare nested search models
                PrepareRewardPointsSearchModel(model.UserRewardPointsSearchModel, user);
                PrepareUserAddressSearchModel(model.UserAddressSearchModel, user);
                PrepareUserOrderSearchModel(model.UserOrderSearchModel, user);
                await PrepareUserShoppingCartSearchModelAsync(model.UserShoppingCartSearchModel, user);
                PrepareUserActivityLogSearchModel(model.UserActivityLogSearchModel, user);
                PrepareUserBackInStockSubscriptionSearchModel(model.UserBackInStockSubscriptionSearchModel, user);
                await PrepareUserAssociatedExternalAuthRecordsSearchModelAsync(model.UserAssociatedExternalAuthRecordsSearchModel, user);
            }
            else
            {
                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    //precheck Registered Role as a default role while creating a new user through admin
                    var registeredRole = await _userService.GetUserRoleBySystemNameAsync(TvProgUserDefaults.RegisteredRoleName);
                    if (registeredRole != null)
                        model.SelectedUserRoleIds.Add(registeredRole.Id);
                }
            }

            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            model.FirstNameEnabled = _userSettings.FirstNameEnabled;
            model.LastNameEnabled = _userSettings.LastNameEnabled;
            model.MiddleNameEnabled = _userSettings.MiddleNameEnabled;
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.BirthDateEnabled = _userSettings.BirthDateEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CountyEnabled = _userSettings.CountyEnabled;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.SmartPhoneEnabled = _userSettings.SmartPhoneEnabled;
            model.FaxEnabled = _userSettings.FaxEnabled;

            //set default values for the new model
            if (user == null)
            {
                model.Active = true;
                model.DisplayVatNumber = false;
            }

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(model.AvailableVendors,
                defaultItemText: await _localizationService.GetResourceAsync("Admin.Users.Users.Fields.Vendor.None"));

            //prepare model user attributes
            await PrepareUserAttributeModelsAsync(model.UserAttributes, user);

            //prepare model stores for newsletter subscriptions
            model.AvailableNewsletterSubscriptionStores = (await _storeService.GetAllStoresAsync()).Select(store => new SelectListItem
            {
                Value = store.Id.ToString(),
                Text = store.Name,
                Selected = model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id)
            }).ToList();

            //prepare model user roles
            await _aclSupportedModelFactory.PrepareModelUserRolesAsync(model);

            //prepare available time zones
            await _baseAdminModelFactory.PrepareTimeZonesAsync(model.AvailableTimeZones, false);

            //prepare available countries and states
            if (_userSettings.CountryEnabled)
            {
                await _baseAdminModelFactory.PrepareCountriesAsync(model.AvailableCountries);
                if (_userSettings.StateProvinceEnabled)
                    await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(model.AvailableStates, model.CountryId == 0 ? null : (int?)model.CountryId);
            }

            return model;
        }

        /// <summary>
        /// Prepare paged reward points list model
        /// </summary>
        /// <param name="searchModel">Reward points search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the reward points list model
        /// </returns>
        public virtual async Task<UserRewardPointsListModel> PrepareRewardPointsListModelAsync(UserRewardPointsSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //get reward points history
            var rewardPoints = await _rewardPointService.GetRewardPointsHistoryAsync(user.Id,
                showNotActivated: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new UserRewardPointsListModel().PrepareToGridAsync(searchModel, rewardPoints, () =>
            {
                return rewardPoints.SelectAwait(async historyEntry =>
                {
                    //fill in model values from the entity        
                    var rewardPointsHistoryModel = historyEntry.ToModel<UserRewardPointsModel>();

                    //convert dates to the user time
                    var activatingDate = await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc);
                    rewardPointsHistoryModel.CreatedOn = activatingDate;

                    rewardPointsHistoryModel.PointsBalance = historyEntry.PointsBalance.HasValue
                        ? historyEntry.PointsBalance.ToString()
                        : string.Format((await _localizationService.GetResourceAsync("Admin.Users.Users.RewardPoints.ActivatedLater")), activatingDate);
                    rewardPointsHistoryModel.EndDate = !historyEntry.EndDateUtc.HasValue
                        ? null
                        : (DateTime?)(await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.EndDateUtc.Value, DateTimeKind.Utc));

                    //fill in additional values (not existing in the entity)
                    rewardPointsHistoryModel.StoreName = (await _storeService.GetStoreByIdAsync(historyEntry.StoreId))?.Name ?? "Unknown";

                    return rewardPointsHistoryModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged user address list model
        /// </summary>
        /// <param name="searchModel">User address search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user address list model
        /// </returns>
        public virtual async Task<UserAddressListModel> PrepareUserAddressListModelAsync(UserAddressSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //get user addresses
            var addresses = (await _userService.GetAddressesByUserIdAsync(user.Id))
                .OrderByDescending(address => address.CreatedOnUtc).ThenByDescending(address => address.Id).ToList()
                .ToPagedList(searchModel);

            //prepare list model
            var model = await new UserAddressListModel().PrepareToGridAsync(searchModel, addresses, () =>
            {
                return addresses.SelectAwait(async address =>
                {
                    //fill in model values from the entity        
                    var addressModel = address.ToModel<AddressModel>();

                    addressModel.CountryName = (await _countryService.GetCountryByAddressAsync(address))?.Name;
                    addressModel.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(address))?.Name;

                    //fill in additional values (not existing in the entity)
                    await PrepareModelAddressHtmlAsync(addressModel, address);

                    return addressModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare user address model
        /// </summary>
        /// <param name="model">User address model</param>
        /// <param name="user">User</param>
        /// <param name="address">Address</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user address model
        /// </returns>
        public virtual async Task<UserAddressModel> PrepareUserAddressModelAsync(UserAddressModel model,
            User user, Address address, bool excludeProperties = false)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (address != null)
            {
                //fill in model values from the entity
                model ??= new UserAddressModel();

                //whether to fill in some of properties
                if (!excludeProperties)
                    model.Address = address.ToModel(model.Address);
            }

            model.UserId = user.Id;

            //prepare address model
            await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);
            model.Address.FirstNameRequired = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailRequired = true;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.CountyRequired = _addressSettings.CountyRequired;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.SmartPhoneRequired = _addressSettings.SmartPhoneRequired;
            model.Address.FaxRequired = _addressSettings.FaxRequired;

            return model;
        }

        /// <summary>
        /// Prepare paged user order list model
        /// </summary>
        /// <param name="searchModel">User order search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user order list model
        /// </returns>
        public virtual async Task<UserOrderListModel> PrepareUserOrderListModelAsync(UserOrderSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //get user orders
            var orders = await _orderService.SearchOrdersAsync(userId: user.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new UserOrderListModel().PrepareToGridAsync(searchModel, orders, () =>
            {
                return orders.SelectAwait(async order =>
                {
                    //fill in model values from the entity
                    var orderModel = order.ToModel<UserOrderModel>();

                    //convert dates to the user time
                    orderModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    orderModel.StoreName = (await _storeService.GetStoreByIdAsync(order.StoreId))?.Name ?? "Unknown";
                    orderModel.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                    orderModel.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
                    orderModel.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);
                    orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false);

                    return orderModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged user shopping cart list model
        /// </summary>
        /// <param name="searchModel">User shopping cart search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user shopping cart list model
        /// </returns>
        public virtual async Task<UserShoppingCartListModel> PrepareUserShoppingCartListModelAsync(UserShoppingCartSearchModel searchModel,
            User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //get user shopping cart
            var shoppingCart = (await _shoppingCartService.GetShoppingCartAsync(user, (ShoppingCartType)searchModel.ShoppingCartTypeId))
                .ToPagedList(searchModel);

            //prepare list model
            var model = await new UserShoppingCartListModel().PrepareToGridAsync(searchModel, shoppingCart, () =>
            {
                return shoppingCart.SelectAwait(async item =>
                {
                    //fill in model values from the entity
                    var shoppingCartItemModel = item.ToModel<ShoppingCartItemModel>();

                    var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(item.TvChannelId);

                    //fill in additional values (not existing in the entity)
                    shoppingCartItemModel.TvChannelName = tvchannel.Name;
                    shoppingCartItemModel.Store = (await _storeService.GetStoreByIdAsync(item.StoreId))?.Name ?? "Unknown";
                    shoppingCartItemModel.AttributeInfo = await _tvchannelAttributeFormatter.FormatAttributesAsync(tvchannel, item.AttributesXml);
                    var (unitPrice, _, _) = await _shoppingCartService.GetUnitPriceAsync(item, true);
                    shoppingCartItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync((await _taxService.GetTvChannelPriceAsync(tvchannel, unitPrice)).price);
                    shoppingCartItemModel.UnitPriceValue = (await _taxService.GetTvChannelPriceAsync(tvchannel, unitPrice)).price;
                    var (subTotal, _, _, _) = await _shoppingCartService.GetSubTotalAsync(item, true);
                    shoppingCartItemModel.Total = await _priceFormatter.FormatPriceAsync((await _taxService.GetTvChannelPriceAsync(tvchannel, subTotal)).price);
                    shoppingCartItemModel.TotalValue = (await _taxService.GetTvChannelPriceAsync(tvchannel, subTotal)).price;

                    //convert dates to the user time
                    shoppingCartItemModel.UpdatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(item.UpdatedOnUtc, DateTimeKind.Utc);

                    return shoppingCartItemModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged user activity log list model
        /// </summary>
        /// <param name="searchModel">User activity log search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user activity log list model
        /// </returns>
        public virtual async Task<UserActivityLogListModel> PrepareUserActivityLogListModelAsync(UserActivityLogSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //get user activity log
            var activityLog = await _userActivityService.GetAllActivitiesAsync(userId: user.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new UserActivityLogListModel().PrepareToGridAsync(searchModel, activityLog, () =>
            {
                return activityLog.SelectAwait(async logItem =>
                {
                    //fill in model values from the entity
                    var userActivityLogModel = logItem.ToModel<UserActivityLogModel>();

                    //fill in additional values (not existing in the entity)
                    userActivityLogModel.ActivityLogTypeName = (await _userActivityService.GetActivityTypeByIdAsync(logItem.ActivityLogTypeId))?.Name;

                    //convert dates to the user time
                    userActivityLogModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    return userActivityLogModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged user back in stock subscriptions list model
        /// </summary>
        /// <param name="searchModel">User back in stock subscriptions search model</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user back in stock subscriptions list model
        /// </returns>
        public virtual async Task<UserBackInStockSubscriptionListModel> PrepareUserBackInStockSubscriptionListModelAsync(
            UserBackInStockSubscriptionSearchModel searchModel, User user)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //get user back in stock subscriptions
            var subscriptions = await _backInStockSubscriptionService.GetAllSubscriptionsByUserIdAsync(user.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new UserBackInStockSubscriptionListModel().PrepareToGridAsync(searchModel, subscriptions, () =>
            {
                return subscriptions.SelectAwait(async subscription =>
                {
                    //fill in model values from the entity
                    var subscriptionModel = subscription.ToModel<UserBackInStockSubscriptionModel>();

                    //convert dates to the user time
                    subscriptionModel.CreatedOn =
                        await _dateTimeHelper.ConvertToUserTimeAsync(subscription.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    subscriptionModel.StoreName = (await _storeService.GetStoreByIdAsync(subscription.StoreId))?.Name ?? "Unknown";
                    subscriptionModel.TvChannelName = (await _tvchannelService.GetTvChannelByIdAsync(subscription.TvChannelId))?.Name ?? "Unknown";

                    return subscriptionModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare online user search model
        /// </summary>
        /// <param name="searchModel">Online user search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the online user search model
        /// </returns>
        public virtual Task<OnlineUserSearchModel> PrepareOnlineUserSearchModelAsync(OnlineUserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged online user list model
        /// </summary>
        /// <param name="searchModel">Online user search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the online user list model
        /// </returns>
        public virtual async Task<OnlineUserListModel> PrepareOnlineUserListModelAsync(OnlineUserSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter users
            var lastActivityFrom = DateTime.UtcNow.AddMinutes(-_userSettings.OnlineUserMinutes);

            //get online users
            var users = await _userService.GetOnlineUsersAsync(userRoleIds: null,
                 lastActivityFromUtc: lastActivityFrom,
                 pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new OnlineUserListModel().PrepareToGridAsync(searchModel, users, () =>
            {
                return users.SelectAwait(async user =>
                {
                    //fill in model values from the entity
                    var userModel = user.ToModel<OnlineUserModel>();

                    //convert dates to the user time
                    userModel.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(user.LastActivityDateUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    userModel.UserInfo = (await _userService.IsRegisteredAsync(user))
                        ? user.Email
                        : await _localizationService.GetResourceAsync("Admin.Users.Guest");
                    userModel.LastIpAddress = _userSettings.StoreIpAddresses
                        ? user.LastIpAddress
                        : await _localizationService.GetResourceAsync("Admin.Users.OnlineUsers.Fields.IPAddress.Disabled");
                    userModel.Location = _geoLookupService.LookupCountryName(user.LastIpAddress);
                    userModel.LastVisitedPage = _userSettings.StoreLastVisitedPage
                        ? await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.LastVisitedPageAttribute)
                        : await _localizationService.GetResourceAsync("Admin.Users.OnlineUsers.Fields.LastVisitedPage.Disabled");

                    return userModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare GDPR request (log) search model
        /// </summary>
        /// <param name="searchModel">GDPR request search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gDPR request search model
        /// </returns>
        public virtual async Task<GdprLogSearchModel> PrepareGdprLogSearchModelAsync(GdprLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare request types
            await _baseAdminModelFactory.PrepareGdprRequestTypesAsync(searchModel.AvailableRequestTypes);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged GDPR request list model
        /// </summary>
        /// <param name="searchModel">GDPR request search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gDPR request list model
        /// </returns>
        public virtual async Task<GdprLogListModel> PrepareGdprLogListModelAsync(GdprLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var userId = 0;
            var userInfo = "";
            if (!string.IsNullOrEmpty(searchModel.SearchEmail))
            {
                var user = await _userService.GetUserByEmailAsync(searchModel.SearchEmail);
                if (user != null)
                    userId = user.Id;
                else
                {
                    userInfo = searchModel.SearchEmail;
                }
            }
            //get requests
            var gdprLog = await _gdprService.GetAllLogAsync(
                userId: userId,
                userInfo: userInfo,
                requestType: searchModel.SearchRequestTypeId > 0 ? (GdprRequestType?)searchModel.SearchRequestTypeId : null,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new GdprLogListModel().PrepareToGridAsync(searchModel, gdprLog, () =>
            {
                return gdprLog.SelectAwait(async log =>
                {
                    //fill in model values from the entity
                    var user = await _userService.GetUserByIdAsync(log.UserId);

                    var requestModel = log.ToModel<GdprLogModel>();

                    //fill in additional values (not existing in the entity)
                    requestModel.UserInfo = user != null && !user.Deleted && !string.IsNullOrEmpty(user.Email)
                        ? user.Email
                        : log.UserInfo;
                    requestModel.RequestType = await _localizationService.GetLocalizedEnumAsync(log.RequestType);

                    requestModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(log.CreatedOnUtc, DateTimeKind.Utc);

                    return requestModel;
                });
            });

            return model;
        }

        #endregion
    }
}
