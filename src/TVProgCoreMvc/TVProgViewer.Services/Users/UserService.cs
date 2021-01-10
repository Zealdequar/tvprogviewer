using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using TVProgViewer.Core;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.Data;
using TVProgViewer.Services.Caching.CachingDefaults;
using TVProgViewer.Services.Caching.Extensions;
using TVProgViewer.Services.Common;
using TVProgViewer.Services.Events;
using TVProgViewer.Services.Localization;

namespace TVProgViewer.Services.Users
{
    /// <summary>
    /// User service
    /// </summary>
    public partial class UserService : IUserService
    {
        #region Fields

        private readonly UserSettings _userSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IDataProvider _dataProvider;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRepository<Address> _userAddressRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserAddressMapping> _UserAddressMappingRepository;
        private readonly IRepository<UserUserRoleMapping> _userUserRoleMappingRepository;
        private readonly IRepository<UserPassword> _UserPasswordRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IRepository<ShoppingCartItem> _shoppingCartRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public UserService(UserSettings UserSettings,
            ICacheManager cacheManager,
            IDataProvider dataProvider,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IRepository<Address> userAddressRepository,
            IRepository<User> userRepository,
            IRepository<UserAddressMapping> UserAddressMappingRepository,
            IRepository<UserUserRoleMapping> UserUserRoleMappingRepository,
            IRepository<UserPassword> UserPasswordRepository,
            IRepository<UserRole> UserRoleRepository,
            IRepository<GenericAttribute> gaRepository,
            IRepository<ShoppingCartItem> shoppingCartRepository,
            IStaticCacheManager staticCacheManager,
            ShoppingCartSettings shoppingCartSettings)
        {
            _userSettings = UserSettings;
            _cacheManager = cacheManager;
            _dataProvider = dataProvider;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _userAddressRepository = userAddressRepository;
            _userRepository = userRepository;
            _UserAddressMappingRepository = UserAddressMappingRepository;
            _userUserRoleMappingRepository = UserUserRoleMappingRepository;
            _UserPasswordRepository = UserPasswordRepository;
            _userRoleRepository = UserRoleRepository;
            _gaRepository = gaRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _staticCacheManager = staticCacheManager;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Methods

        #region Users

        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <param name="vendorId">Vendor identifier</param>
        /// <param name="userRoleIds">A list of User role identifiers to filter by (at least one match); pass null or empty list in order to load all Users; </param>
        /// <param name="email">Email; null to load all Users</param>
        /// <param name="username">Username; null to load all Users</param>
        /// <param name="firstName">First name; null to load all Users</param>
        /// <param name="lastName">Last name; null to load all Users</param>
        /// <param name="dayOfBirth">Day of birth; 0 to load all Users</param>
        /// <param name="monthOfBirth">Month of birth; 0 to load all Users</param>
        /// <param name="company">Company; null to load all Users</param>
        /// <param name="phone">Phone; null to load all Users</param>
        /// <param name="zipPostalCode">Phone; null to load all Users</param>
        /// <param name="ipAddress">IP address; null to load all Users</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>Пользователи</returns>
        public virtual IPagedList<User> GetAllUsers(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int affiliateId = 0, int vendorId = 0, int[] userRoleIds = null,
            string email = null, string username = null, string firstName = null, string lastName = null,
            int dayOfBirth = 0, int monthOfBirth = 0,
            string company = null, string phone = null, string zipPostalCode = null, string ipAddress = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _userRepository.Table;
            if (createdFromUtc.HasValue)
                query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);

            query = query.Where(c => c.Deleted == null);

            if (userRoleIds != null && userRoleIds.Length > 0)
            {
                query = query.Join(_userUserRoleMappingRepository.Table, x => x.Id, y => y.UserId,
                        (x, y) => new { User = x, Mapping = y })
                    .Where(z => userRoleIds.Contains(z.Mapping.UserRoleId))
                    .Select(z => z.User)
                    .Distinct();
            }

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(c => c.Email.Contains(email));
            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(c => c.UserName.Contains(username));
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                                z.Attribute.Key == TvProgUserDefaults.FirstNameAttribute &&
                                z.Attribute.Value.Contains(firstName))
                    .Select(z => z.User);
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                                z.Attribute.Key == TvProgUserDefaults.LastNameAttribute &&
                                z.Attribute.Value.Contains(lastName))
                    .Select(z => z.User);
            }

            //date of birth is stored as a string into database.
            //we also know that date of birth is stored in the following format YYYY-MM-DD (for example, 1983-02-18).
            //so let's search it as a string
            if (dayOfBirth > 0 && monthOfBirth > 0)
            {
                //both are specified
                var dateOfBirthStr = monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-" + dayOfBirth.ToString("00", CultureInfo.InvariantCulture);

                //z.Attribute.Value.Length - dateOfBirthStr.Length = 5
                //dateOfBirthStr.Length = 5
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                                z.Attribute.Key == TvProgUserDefaults.DateOfBirthAttribute &&
                                z.Attribute.Value.Substring(5, 5) == dateOfBirthStr)
                    .Select(z => z.User);
            }
            else if (dayOfBirth > 0)
            {
                //only day is specified
                var dateOfBirthStr = dayOfBirth.ToString("00", CultureInfo.InvariantCulture);

                //z.Attribute.Value.Length - dateOfBirthStr.Length = 8
                //dateOfBirthStr.Length = 2
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                                z.Attribute.Key == TvProgUserDefaults.DateOfBirthAttribute &&
                                z.Attribute.Value.Substring(8, 2) == dateOfBirthStr)
                    .Select(z => z.User);
            }
            else if (monthOfBirth > 0)
            {
                //only month is specified
                var dateOfBirthStr = "-" + monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-";
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                                z.Attribute.Key == TvProgUserDefaults.DateOfBirthAttribute &&
                                z.Attribute.Value.Contains(dateOfBirthStr))
                    .Select(z => z.User);
            }
            //search by company
            if (!string.IsNullOrWhiteSpace(company))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                                z.Attribute.Key == TvProgUserDefaults.CompanyAttribute &&
                                z.Attribute.Value.Contains(company))
                    .Select(z => z.User);
            }
            //search by phone
            if (!string.IsNullOrWhiteSpace(phone))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                                z.Attribute.Key == TvProgUserDefaults.PhoneAttribute &&
                                z.Attribute.Value.Contains(phone))
                    .Select(z => z.User);
            }
            //search by zip
            if (!string.IsNullOrWhiteSpace(zipPostalCode))
            {
                query = query
                    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { User = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(User) &&
                                z.Attribute.Key == TvProgUserDefaults.ZipPostalCodeAttribute &&
                                z.Attribute.Value.Contains(zipPostalCode))
                    .Select(z => z.User);
            }

            //search by IpAddress
            if (!string.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
            {
                query = query.Where(w => w.LastIpAddress == ipAddress);
            }

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            var Users = new PagedList<User>(query, pageIndex, pageSize, getOnlyTotalCount);
            return Users;
        }

        /// <summary>
        /// Получение пользователей онлайн
        /// </summary>
        /// <param name="lastActivityFromUtc">Дата последней пользовательской активности (с)</param>
        /// <param name="userRoleIds">Список пользовательских ролей идентифицирующих для фильтрации по (не меньше одного совпадения); Передать нулевой или пустой список для загрузки всех пользователей; </param>
        /// <param name="pageIndex">Индекс страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <returns>Users</returns>
        public virtual IPagedList<User> GetOnlineUsers(DateTime lastActivityFromUtc,
            int[] userRoleIds, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _userRepository.Table;
            query = query.Where(c => lastActivityFromUtc <= c.LastActivityDateUtc);
            query = query.Where(c => c.Deleted == null);

            if (userRoleIds != null && userRoleIds.Length > 0)
                query = query.Where(c => _userUserRoleMappingRepository.Table.Any(ccrm => ccrm.UserId == c.Id && userRoleIds.Contains(ccrm.UserRoleId)));

            query = query.OrderByDescending(c => c.LastActivityDateUtc);
            var users = new PagedList<User>(query, pageIndex, pageSize);

            return users;
        }

        /// <summary>
        /// Gets Users with shopping carts
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type; pass null to load all records</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="productId">Product identifier; pass null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); pass null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); pass null to load all records</param>
        /// <param name="countryId">Billing country identifier; pass null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Users</returns>
        public virtual IPagedList<User> GetUsersWithShoppingCarts(ShoppingCartType? shoppingCartType = null,
            int storeId = 0, int? productId = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null, int? countryId = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            //get all shopping cart items
            var items = _shoppingCartRepository.Table;

            //filter by type
            if (shoppingCartType.HasValue)
                items = items.Where(item => item.ShoppingCartTypeId == (int)shoppingCartType.Value);

            //filter shopping cart items by store
            if (storeId > 0 && !_shoppingCartSettings.CartsSharedBetweenStores)
                items = items.Where(item => item.StoreId == storeId);

            //filter shopping cart items by product
            if (productId > 0)
                items = items.Where(item => item.ProductId == productId);

            //filter shopping cart items by date
            if (createdFromUtc.HasValue)
                items = items.Where(item => createdFromUtc.Value <= item.CreatedOnUtc);
            if (createdToUtc.HasValue)
                items = items.Where(item => createdToUtc.Value >= item.CreatedOnUtc);

            //get all active Users
            var users = _userRepository.Table.Where(user => user.Active && user.Deleted == null);

            var usersWithCarts = from c in users
                join item in items on c.Id equals item.UserId
                orderby c.Id
                select c;

            return new PagedList<User>(usersWithCarts.Distinct(), pageIndex, pageSize);
        }

        /// <summary>
        /// Получение пользователей для корзины покупок
        /// </summary>
        /// <param name="shoppingCart">Корзина покупок</param>
        /// <returns>Результат</returns>
        public virtual User GetShoppingCartUser(IList<ShoppingCartItem> shoppingCart)
        {
            var userId = shoppingCart.FirstOrDefault()?.UserId;

            return userId.HasValue && userId != 0 ? GetUserById(userId.Value) : null;
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="User">Пользователь</param>
        public virtual void DeleteUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (user.IsSystemAccount)
                throw new TvProgException($"Системный аккаунт пользователя ({user.SystemName}) не может быть удалён");

            user.Deleted = DateTime.Now;

            if (_userSettings.SuffixDeletedUsers)
            {
                if (!string.IsNullOrEmpty(user.Email))
                    user.Email += "-DELETED";
                if (!string.IsNullOrEmpty(user.UserName))
                    user.UserName += "-DELETED";
            }

            UpdateUser(user);

            // Событие-уведомление:
            _eventPublisher.EntityDeleted(user);
        }

        /// <summary>
        /// Gets a User
        /// </summary>
        /// <param name="UserId">User identifier</param>
        /// <returns>A User</returns>
        public virtual User GetUserById(int UserId)
        {
            if (UserId == 0)
                return null;

            return _userRepository.ToCachedGetById(UserId);
        }

        /// <summary>
        /// Получение пользователей по идентификаторам
        /// </summary>
        /// <param name="userIds">Пользовательские идентификаторы</param>
        /// <returns>Пользователи</returns>
        public virtual IList<User> GetUsersByIds(int[] userIds)
        {
            if (userIds == null || userIds.Length == 0)
                return new List<User>();

            var query = from c in _userRepository.Table
                        where userIds.Contains(c.Id) && c.Deleted == null
                        select c;
            var users = query.ToList();

            // Сортировка подходящих идентификаторов:
            var sortedUsers = new List<User>();
            foreach (var id in userIds)
            {
                var user = users.Find(x => x.Id == id);
                if (user != null)
                    sortedUsers.Add(user);
            }

            return sortedUsers;
        }

        /// <summary>
        /// Gets a User by GUID
        /// </summary>
        /// <param name="UserGuid">User GUID</param>
        /// <returns>A User</returns>
        public virtual User GetUserByGuid(Guid UserGuid)
        {
            if (UserGuid == Guid.Empty)
                return null;

            var query = from c in _userRepository.Table
                        where c.UserGuid == UserGuid
                        orderby c.Id
                        select c;
            var User = query.FirstOrDefault();
            return User;
        }

        /// <summary>
        /// Get User by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>User</returns>
        public virtual User GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.Email == email
                        select c;
            var User = query.FirstOrDefault();
            return User;
        }

        /// <summary>
        /// Get User by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>User</returns>
        public virtual User GetUserBySystemName(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.SystemName == systemName
                        select c;
            var User = query.FirstOrDefault();
            return User;
        }

        /// <summary>
        /// Получение пользователя по пользовательскому имени
        /// </summary>
        /// <param name="username">Пользовательское имя</param>
        /// <returns>Пользователь</returns>
        public virtual User GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.UserName == username
                        select c;
            var user = query.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Вставка гостевого пользователя
        /// </summary>
        /// <returns>Пользователь</returns>
        public virtual User InsertGuestUser()
        {
            var user = new User
            {
                BirthDate = DateTime.Now,
                UserGuid = Guid.NewGuid(),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow
            };

            // Добавление к гостевой роли:
            var guestRole = GetUserRoleBySystemName(TvProgUserDefaults.GuestsRoleName);
            if (guestRole == null)
                throw new TvProgException("'Guests' role could not be loaded");

            _userRepository.Insert(user);

            AddUserRoleMapping(new UserUserRoleMapping { UserId = user.Id, UserRoleId = guestRole.Id });

            return user;
        }

        /// <summary>
        /// Insert a User
        /// </summary>
        /// <param name="User">User</param>
        public virtual void InsertUser(User User)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            _userRepository.Insert(User);

            //event notification
            _eventPublisher.EntityInserted(User);
        }

        /// <summary>
        /// Updates the User
        /// </summary>
        /// <param name="User">User</param>
        public virtual void UpdateUser(User User)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            _userRepository.Update(User);

            //event notification
            _eventPublisher.EntityUpdated(User);
        }

        /// <summary>
        /// Reset data required for checkout
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCouponCodes">A value indicating whether to clear coupon code</param>
        /// <param name="clearCheckoutAttributes">A value indicating whether to clear selected checkout attributes</param>
        /// <param name="clearRewardPoints">A value indicating whether to clear "Use reward points" flag</param>
        /// <param name="clearShippingMethod">A value indicating whether to clear selected shipping method</param>
        /// <param name="clearPaymentMethod">A value indicating whether to clear selected payment method</param>
        public virtual void ResetCheckoutData(User User, int storeId,
            bool clearCouponCodes = false, bool clearCheckoutAttributes = false,
            bool clearRewardPoints = true, bool clearShippingMethod = true,
            bool clearPaymentMethod = true)
        {
            if (User == null)
                throw new ArgumentNullException();

            //clear entered coupon codes
            if (clearCouponCodes)
            {
                _genericAttributeService.SaveAttribute<string>(User, TvProgUserDefaults.DiscountCouponCodeAttribute, null);
                _genericAttributeService.SaveAttribute<string>(User, TvProgUserDefaults.GiftCardCouponCodesAttribute, null);
            }

            //clear checkout attributes
            if (clearCheckoutAttributes)
            {
                _genericAttributeService.SaveAttribute<string>(User, TvProgUserDefaults.CheckoutAttributes, null, storeId);
            }

            //clear reward points flag
            if (clearRewardPoints)
            {
                _genericAttributeService.SaveAttribute(User, TvProgUserDefaults.UseRewardPointsDuringCheckoutAttribute, false, storeId);
            }

            //clear selected shipping method
            if (clearShippingMethod)
            {
                _genericAttributeService.SaveAttribute<ShippingOption>(User, TvProgUserDefaults.SelectedShippingOptionAttribute, null, storeId);
                _genericAttributeService.SaveAttribute<ShippingOption>(User, TvProgUserDefaults.OfferedShippingOptionsAttribute, null, storeId);
                _genericAttributeService.SaveAttribute<PickupPoint>(User, TvProgUserDefaults.SelectedPickupPointAttribute, null, storeId);
            }

            //clear selected payment method
            if (clearPaymentMethod)
            {
                _genericAttributeService.SaveAttribute<string>(User, TvProgUserDefaults.SelectedPaymentMethodAttribute, null, storeId);
            }

            UpdateUser(User);
        }

        /// <summary>
        /// Delete guest User records
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="onlyWithoutShoppingCart">A value indicating whether to delete Users only without shopping cart</param>
        /// <returns>Number of deleted Users</returns>
        public virtual int DeleteGuestUsers(DateTime? createdFromUtc, DateTime? createdToUtc, bool onlyWithoutShoppingCart)
        {
            //prepare parameters
            var pOnlyWithoutShoppingCart = SqlParameterHelper.GetBooleanParameter("OnlyWithoutShoppingCart", onlyWithoutShoppingCart);
            var pCreatedFromUtc = SqlParameterHelper.GetDateTimeParameter("CreatedFromUtc", createdFromUtc);
            var pCreatedToUtc = SqlParameterHelper.GetDateTimeParameter("CreatedToUtc", createdToUtc);
            var pTotalRecordsDeleted = SqlParameterHelper.GetOutputInt32Parameter("TotalRecordsDeleted");

            //invoke stored procedure
            _dataProvider.Query<object>("EXEC [DeleteGuests] @OnlyWithoutShoppingCart, @CreatedFromUtc, @CreatedToUtc, @TotalRecordsDeleted OUTPUT",
                pOnlyWithoutShoppingCart,
                pCreatedFromUtc,
                pCreatedToUtc,
                pTotalRecordsDeleted);

            var totalRecordsDeleted = pTotalRecordsDeleted.Value != DBNull.Value ? Convert.ToInt32(pTotalRecordsDeleted.Value) : 0;
            return totalRecordsDeleted;
        }

        /// <summary>
        /// Gets a default tax display type (if configured)
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>Result</returns>
        public virtual TaxDisplayType? GetUserDefaultTaxDisplayType(User User)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var roleWithOverriddenTaxType = GetUserRoles(User).FirstOrDefault(cr => cr.Active && cr.OverrideTaxDisplayType);
            if (roleWithOverriddenTaxType == null)
                return null;

            return (TaxDisplayType)roleWithOverriddenTaxType.DefaultTaxDisplayTypeId;
        }

        /// <summary>
        /// Get full name
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>User full name</returns>
        public virtual string GetUserFullName(User User)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var firstName = _genericAttributeService.GetAttribute<string>(User, TvProgUserDefaults.FirstNameAttribute);
            var lastName = _genericAttributeService.GetAttribute<string>(User, TvProgUserDefaults.LastNameAttribute);

            var fullName = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
                fullName = $"{firstName} {lastName}";
            else
            {
                if (!string.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!string.IsNullOrWhiteSpace(lastName))
                    fullName = lastName;
            }

            return fullName;
        }

        /// <summary>
        /// Форматирование пользовательского имени
        /// </summary>
        /// <param name="user">Источник</param>
        /// <param name="stripTooLong">Слишком длинное имя пользователя</param>
        /// <param name="maxLength">Максимальная длина имени пользователя</param>
        /// <returns>Форматированный текст</returns>
        public virtual string FormatUsername(User user, bool stripTooLong = false, int maxLength = 0)
        {
            if (user == null)
                return string.Empty;

            if (IsGuest(user))
                return EngineContext.Current.Resolve<ILocalizationService>().GetResource("User.Guest");

            var result = string.Empty;
            switch (_userSettings.UserNameFormat)
            {
                case UserNameFormat.ShowEmails:
                    result = user.Email;
                    break;
                case UserNameFormat.ShowUsernames:
                    result = user.UserName;
                    break;
                case UserNameFormat.ShowFullNames:
                    result = GetUserFullName(user);
                    break;
                case UserNameFormat.ShowFirstName:
                    result = _genericAttributeService.GetAttribute<string>(user, TvProgUserDefaults.FirstNameAttribute);
                    break;
                default:
                    break;
            }

            if (stripTooLong && maxLength > 0)
                result = CommonHelper.EnsureMaximumLength(result, maxLength);

            return result;
        }

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>Coupon codes</returns>
        public virtual string[] ParseAppliedDiscountCouponCodes(User User)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var existingCouponCodes = _genericAttributeService.GetAttribute<string>(User, TvProgUserDefaults.DiscountCouponCodeAttribute);

            var couponCodes = new List<string>();
            if (string.IsNullOrEmpty(existingCouponCodes))
                return couponCodes.ToArray();

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(existingCouponCodes);

                var nodeList1 = xmlDoc.SelectNodes(@"//DiscountCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["Code"] == null)
                        continue;
                    var code = node1.Attributes["Code"].InnerText.Trim();
                    couponCodes.Add(code);
                }
            }
            catch
            {
                // ignored
            }

            return couponCodes.ToArray();
        }

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>New coupon codes document</returns>
        public virtual void ApplyDiscountCouponCode(User User, string couponCode)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var result = string.Empty;
            try
            {
                var existingCouponCodes = _genericAttributeService.GetAttribute<string>(User, TvProgUserDefaults.DiscountCouponCodeAttribute);

                couponCode = couponCode.Trim().ToLower();

                var xmlDoc = new XmlDocument();
                if (string.IsNullOrEmpty(existingCouponCodes))
                {
                    var element1 = xmlDoc.CreateElement("DiscountCouponCodes");
                    xmlDoc.AppendChild(element1);
                }
                else
                {
                    xmlDoc.LoadXml(existingCouponCodes);
                }

                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//DiscountCouponCodes");

                XmlElement gcElement = null;
                //find existing
                var nodeList1 = xmlDoc.SelectNodes(@"//DiscountCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["Code"] == null)
                        continue;

                    var couponCodeAttribute = node1.Attributes["Code"].InnerText.Trim();

                    if (couponCodeAttribute.ToLower() != couponCode.ToLower())
                        continue;

                    gcElement = (XmlElement)node1;
                    break;
                }

                //create new one if not found
                if (gcElement == null)
                {
                    gcElement = xmlDoc.CreateElement("CouponCode");
                    gcElement.SetAttribute("Code", couponCode);
                    rootElement.AppendChild(gcElement);
                }

                result = xmlDoc.OuterXml;
            }
            catch
            {
                // ignored
            }

            //apply new value
            _genericAttributeService.SaveAttribute(User, TvProgUserDefaults.DiscountCouponCodeAttribute, result);
        }

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>New coupon codes document</returns>
        public virtual void RemoveDiscountCouponCode(User User, string couponCode)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            //get applied coupon codes
            var existingCouponCodes = ParseAppliedDiscountCouponCodes(User);

            //clear them
            _genericAttributeService.SaveAttribute<string>(User, TvProgUserDefaults.DiscountCouponCodeAttribute, null);

            //save again except removed one
            foreach (var existingCouponCode in existingCouponCodes)
                if (!existingCouponCode.Equals(couponCode, StringComparison.InvariantCultureIgnoreCase))
                    ApplyDiscountCouponCode(User, existingCouponCode);
        }

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>Coupon codes</returns>
        public virtual string[] ParseAppliedGiftCardCouponCodes(User User)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var existingCouponCodes = _genericAttributeService.GetAttribute<string>(User, TvProgUserDefaults.GiftCardCouponCodesAttribute);

            var couponCodes = new List<string>();
            if (string.IsNullOrEmpty(existingCouponCodes))
                return couponCodes.ToArray();

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(existingCouponCodes);

                var nodeList1 = xmlDoc.SelectNodes(@"//GiftCardCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["Code"] == null)
                        continue;

                    var code = node1.Attributes["Code"].InnerText.Trim();
                    couponCodes.Add(code);
                }
            }
            catch
            {
                // ignored
            }

            return couponCodes.ToArray();
        }

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>New coupon codes document</returns>
        public virtual void ApplyGiftCardCouponCode(User User, string couponCode)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var result = string.Empty;
            try
            {
                var existingCouponCodes = _genericAttributeService.GetAttribute<string>(User, TvProgUserDefaults.GiftCardCouponCodesAttribute);

                couponCode = couponCode.Trim().ToLower();

                var xmlDoc = new XmlDocument();
                if (string.IsNullOrEmpty(existingCouponCodes))
                {
                    var element1 = xmlDoc.CreateElement("GiftCardCouponCodes");
                    xmlDoc.AppendChild(element1);
                }
                else
                {
                    xmlDoc.LoadXml(existingCouponCodes);
                }

                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//GiftCardCouponCodes");

                XmlElement gcElement = null;
                //find existing
                var nodeList1 = xmlDoc.SelectNodes(@"//GiftCardCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["Code"] == null)
                        continue;

                    var couponCodeAttribute = node1.Attributes["Code"].InnerText.Trim();
                    if (couponCodeAttribute.ToLower() != couponCode.ToLower())
                        continue;

                    gcElement = (XmlElement)node1;
                    break;
                }

                //create new one if not found
                if (gcElement == null)
                {
                    gcElement = xmlDoc.CreateElement("CouponCode");
                    gcElement.SetAttribute("Code", couponCode);
                    rootElement.AppendChild(gcElement);
                }

                result = xmlDoc.OuterXml;
            }
            catch
            {
                // ignored
            }

            //apply new value
            _genericAttributeService.SaveAttribute(User, TvProgUserDefaults.GiftCardCouponCodesAttribute, result);
        }

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>New coupon codes document</returns>
        public virtual void RemoveGiftCardCouponCode(User User, string couponCode)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            //get applied coupon codes
            var existingCouponCodes = ParseAppliedGiftCardCouponCodes(User);

            //clear them
            _genericAttributeService.SaveAttribute<string>(User, TvProgUserDefaults.GiftCardCouponCodesAttribute, null);

            //save again except removed one
            foreach (var existingCouponCode in existingCouponCodes)
                if (!existingCouponCode.Equals(couponCode, StringComparison.InvariantCultureIgnoreCase))
                    ApplyGiftCardCouponCode(User, existingCouponCode);
        }

        #endregion

        #region User roles

        /// <summary>
        /// Add a User-User role mapping
        /// </summary>
        /// <param name="roleMapping">User-User role mapping</param>
        public void AddUserRoleMapping(UserUserRoleMapping roleMapping)
        {
            if (roleMapping is null)
                throw new ArgumentNullException(nameof(roleMapping));

            _userUserRoleMappingRepository.Insert(roleMapping);

            _eventPublisher.EntityInserted(roleMapping);
        }

        /// <summary>
        /// Remove a User-User role mapping
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="role">User role</param>
        public void RemoveUserRoleMapping(User User, UserRole role)
        {
            if (User is null)
                throw new ArgumentNullException(nameof(User));

            if (role is null)
                throw new ArgumentNullException(nameof(role));

            var mapping = _userUserRoleMappingRepository.Table.SingleOrDefault(ccrm => ccrm.UserId == User.Id && ccrm.UserRoleId == role.Id);

            if (mapping != null)
            {
                _userUserRoleMappingRepository.Delete(mapping);

                _eventPublisher.EntityDeleted(mapping);
            }
        }

        /// <summary>
        /// Delete a User role
        /// </summary>
        /// <param name="UserRole">User role</param>
        public virtual void DeleteUserRole(UserRole UserRole)
        {
            if (UserRole == null)
                throw new ArgumentNullException(nameof(UserRole));

            if (UserRole.IsSystemRole)
                throw new TvProgException("System role could not be deleted");

            _userRoleRepository.Delete(UserRole);

            //event notification
            _eventPublisher.EntityDeleted(UserRole);
        }

        /// <summary>
        /// Gets a User role
        /// </summary>
        /// <param name="UserRoleId">User role identifier</param>
        /// <returns>User role</returns>
        public virtual UserRole GetUserRoleById(int UserRoleId)
        {
            if (UserRoleId == 0)
                return null;

            return _userRoleRepository.ToCachedGetById(UserRoleId);
        }

        /// <summary>
        /// Gets a User role
        /// </summary>
        /// <param name="systemName">User role system name</param>
        /// <returns>User role</returns>
        public virtual UserRole GetUserRoleBySystemName(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var key = TvProgUserServiceCachingDefaults.UserRolesBySystemNameCacheKey.FillCacheKey(systemName);

            var query = from cr in _userRoleRepository.Table
                orderby cr.Id
                where cr.SystemName == systemName
                select cr;
            var UserRole = query.ToCachedFirstOrDefault(key);

            return UserRole;
        }

        /// <summary>
        /// Get User role identifiers
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>User role identifiers</returns>
        public virtual int[] GetUserRoleIds(User user, bool showHidden = false)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(User));

            var query = from cr in _userRoleRepository.Table
                        join crm in _userUserRoleMappingRepository.Table on cr.Id equals crm.UserRoleId
                        where crm.UserId == user.Id && 
                        (showHidden || cr.Active)
                        select cr.Id;

            var key = TvProgUserServiceCachingDefaults.UserRoleIdsCacheKey.FillCacheKey(user.Id, showHidden);

            return query.ToCachedArray(key);
        }

        /// <summary>
        /// Gets list of User roles
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>Result</returns>
        public virtual IList<UserRole> GetUserRoles(User user, bool showHidden = false)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(User));

            var query = from cr in _userRoleRepository.Table
                        join crm in _userUserRoleMappingRepository.Table on cr.Id equals crm.UserRoleId
                        where crm.UserId == user.Id && 
                        (showHidden || cr.Active)
                        select cr;

            var key = TvProgUserServiceCachingDefaults.UserRolesCacheKey.FillCacheKey(user.Id, showHidden);

            return query.ToCachedArray(key);
        }

        /// <summary>
        /// Gets all User roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>User roles</returns>
        public virtual IList<UserRole> GetAllUserRoles(bool showHidden = false)
        {
            var key = TvProgUserServiceCachingDefaults.UserRolesAllCacheKey.FillCacheKey(showHidden);

            var query = from cr in _userRoleRepository.Table
                orderby cr.Name
                where showHidden || cr.Active
                select cr;

            var UserRoles = query.ToCachedList(key);

            return UserRoles;
        }

        /// <summary>
        /// Inserts a User role
        /// </summary>
        /// <param name="UserRole">User role</param>
        public virtual void InsertUserRole(UserRole userRole)
        {
            if (userRole == null)
                throw new ArgumentNullException(nameof(userRole));

            _userRoleRepository.Insert(userRole);

            //event notification
            _eventPublisher.EntityInserted(userRole);
        }

        /// <summary>
        /// Gets a value indicating whether User is in a certain User role
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="UserRoleSystemName">User role system name</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        public virtual bool IsInUserRole(User user,
            string userRoleSystemName, bool onlyActiveUserRoles = true)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(User));

            if (string.IsNullOrEmpty(userRoleSystemName))
                throw new ArgumentNullException(nameof(userRoleSystemName));

            var UserRoles = GetUserRoles(user, !onlyActiveUserRoles);

            return UserRoles?.Any(cr => cr.SystemName == userRoleSystemName) ?? false;
        }

        /// <summary>
        /// Gets a value indicating whether User is administrator
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        public virtual bool IsAdmin(User User, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(User, TvProgUserDefaults.AdministratorsRoleName, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether User is a forum moderator
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        public virtual bool IsForumModerator(User User, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(User, TvProgUserDefaults.ForumModeratorsRoleName, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether User is registered
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        public virtual bool IsRegistered(User User, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(User, TvProgUserDefaults.RegisteredRoleName, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether User is guest
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        public virtual bool IsGuest(User User, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(User, TvProgUserDefaults.GuestsRoleName, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether User is vendor
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        public virtual bool IsVendor(User User, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(User, TvProgUserDefaults.VendorsRoleName, onlyActiveUserRoles);
        }

        /// <summary>
        /// Updates the User role
        /// </summary>
        /// <param name="UserRole">User role</param>
        public virtual void UpdateUserRole(UserRole UserRole)
        {
            if (UserRole == null)
                throw new ArgumentNullException(nameof(UserRole));

            _userRoleRepository.Update(UserRole);

            //event notification
            _eventPublisher.EntityUpdated(UserRole);
        }

        #endregion

        #region User passwords

        /// <summary>
        /// Получение пароля пользователя
        /// </summary>
        /// <param name="UserId">Пользовательский идентификатор; передать null для загрузки всех записей</param>
        /// <param name="passwordFormat">Формат пароля; передать null для загрузки всех записей</param>
        /// <param name="passwordsToReturn">Число возвращаемых паролей; передать null для загрузки всех записей</param>
        /// <returns>Список паролей пользователя</returns>
        public virtual IList<UserPassword> GetUserPasswords(int? UserId = null,
            PasswordFormat? passwordFormat = null, int? passwordsToReturn = null)
        {
            var query = _UserPasswordRepository.Table;

            //filter by User
            if (UserId.HasValue)
                query = query.Where(password => password.UserId == UserId.Value);

            //filter by password format
            if (passwordFormat.HasValue)
                query = query.Where(password => password.PasswordFormatId == (int)passwordFormat.Value);

            //get the latest passwords
            if (passwordsToReturn.HasValue)
                query = query.OrderByDescending(password => password.CreatedOnUtc).Take(passwordsToReturn.Value);

            return query.ToList();
        }

        /// <summary>
        /// Get current User password
        /// </summary>
        /// <param name="UserId">User identifier</param>
        /// <returns>User password</returns>
        public virtual UserPassword GetCurrentPassword(int UserId)
        {
            if (UserId == 0)
                return null;

            //return the latest password
            return GetUserPasswords(UserId, passwordsToReturn: 1).FirstOrDefault();
        }

        /// <summary>
        /// Insert a User password
        /// </summary>
        /// <param name="UserPassword">User password</param>
        public virtual void InsertUserPassword(UserPassword UserPassword)
        {
            if (UserPassword == null)
                throw new ArgumentNullException(nameof(UserPassword));

            _UserPasswordRepository.Insert(UserPassword);

            //event notification
            _eventPublisher.EntityInserted(UserPassword);
        }

        /// <summary>
        /// Update a User password
        /// </summary>
        /// <param name="UserPassword">User password</param>
        public virtual void UpdateUserPassword(UserPassword UserPassword)
        {
            if (UserPassword == null)
                throw new ArgumentNullException(nameof(UserPassword));

            _UserPasswordRepository.Update(UserPassword);

            //event notification
            _eventPublisher.EntityUpdated(UserPassword);
        }

        /// <summary>
        /// Check whether password recovery token is valid
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="token">Token to validate</param>
        /// <returns>Result</returns>
        public virtual bool IsPasswordRecoveryTokenValid(User User, string token)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var cPrt = _genericAttributeService.GetAttribute<string>(User, TvProgUserDefaults.PasswordRecoveryTokenAttribute);
            if (string.IsNullOrEmpty(cPrt))
                return false;

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }

        /// <summary>
        /// Check whether password recovery link is expired
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>Result</returns>
        public virtual bool IsPasswordRecoveryLinkExpired(User User)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            if (_userSettings.PasswordRecoveryLinkDaysValid == 0)
                return false;

            var geneatedDate = _genericAttributeService.GetAttribute<DateTime?>(User, TvProgUserDefaults.PasswordRecoveryTokenDateGeneratedAttribute);
            if (!geneatedDate.HasValue)
                return false;

            var daysPassed = (DateTime.UtcNow - geneatedDate.Value).TotalDays;
            if (daysPassed > _userSettings.PasswordRecoveryLinkDaysValid)
                return true;

            return false;
        }

        /// <summary>
        /// Check whether User password is expired 
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>True if password is expired; otherwise false</returns>
        public virtual bool PasswordIsExpired(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //the guests don't have a password
            if (IsGuest(user))
                return false;

            //password lifetime is disabled for user
            if (!GetUserRoles(user).Any(role => role.Active && role.EnablePasswordLifetime))
                return false;

            //setting disabled for all
            if (_userSettings.PasswordLifetime == 0)
                return false;

            //cache result between HTTP requests
            var cacheKey = TvProgUserServiceCachingDefaults.UserPasswordLifetimeCacheKey.FillCacheKey(user);

            //get current password usage time
            var currentLifetime = _staticCacheManager.Get(cacheKey, () =>
            {
                var userPassword = GetCurrentPassword(user.Id);
                //password is not found, so return max value to force User to change password
                if (userPassword == null)
                    return int.MaxValue;

                return (DateTime.UtcNow - userPassword.CreatedOnUtc).Days;
            });

            return currentLifetime >= _userSettings.PasswordLifetime;
        }

        #endregion

        #region User address mapping

        /// <summary>
        /// Remove a User-address mapping record
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="address">Address</param>
        public virtual void RemoveUserAddress(User User, Address address)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            if (_UserAddressMappingRepository.Table.FirstOrDefault(m => m.AddressId == address.Id && m.UserId == User.Id) is UserAddressMapping mapping)
            {
                _UserAddressMappingRepository.Delete(mapping);

                _eventPublisher.EntityDeleted(mapping);
            }
        }

        /// <summary>
        /// Inserts a User-address mapping record
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="address">Address</param>
        public virtual void InsertUserAddress(User User, Address address)
        {
            if (User is null)
                throw new ArgumentNullException(nameof(User));

            if (address is null)
                throw new ArgumentNullException(nameof(address));

            if (_UserAddressMappingRepository.Table.FirstOrDefault(m => m.AddressId == address.Id && m.UserId == User.Id) is null)
            {
                var mapping = new UserAddressMapping
                {
                    AddressId = address.Id,
                    UserId = User.Id
                };

                _UserAddressMappingRepository.Insert(mapping);

                _eventPublisher.EntityInserted(mapping);
            }
        }

        /// <summary>
        /// Gets a list of addresses mapped to User
        /// </summary>
        /// <param name="UserId">User identifier</param>
        /// <returns>Result</returns>
        public virtual IList<Address> GetAddressesByUserId(int userId)
        {
            var query = from address in _userAddressRepository.Table
                join cam in _UserAddressMappingRepository.Table on address.Id equals cam.AddressId
                where cam.UserId == userId
                select address;

            var key = TvProgUserServiceCachingDefaults.UserAddressesByUserIdCacheKey.FillCacheKey(userId);

            return _cacheManager.Get(key, () => query.ToList());
        }

        public Address GetUserAddress(int userId, int addressId)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Address> IUserService.GetAddressesByUserId(int userID)
        {
            throw new NotImplementedException();
        }

        public Address GetUserShippingAddress(User user)
        {
            throw new NotImplementedException();
        }

        public Address GetUserBillingAddress(User user)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}