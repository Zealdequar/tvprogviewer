using System;
using System.Linq;
using TVProgViewer.Core;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Common;
using TVProgViewer.Services.Defaults;
using TVProgViewer.Services.Events;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.Orders;
using TVProgViewer.Services.Security;
using TVProgViewer.Services.Stores;

namespace TVProgViewer.Services.Users
{
    /// <summary>
    /// User registration service
    /// </summary>
    public partial class UserRegistrationService : IUserRegistrationService
    {
        #region Fields

        private readonly UserSettings _userSettings;
        private readonly IUserService _userService;
        private readonly IEncryptionService _encryptionService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly RewardPointsSettings _rewardPointsSettings;

        #endregion

        #region Ctor

        public UserRegistrationService(UserSettings UserSettings,
            IUserService UserService,
            IEncryptionService encryptionService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IRewardPointService rewardPointService,
            IStoreService storeService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            RewardPointsSettings rewardPointsSettings)
        {
            _userSettings = UserSettings;
            _userService = UserService;
            _encryptionService = encryptionService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _rewardPointService = rewardPointService;
            _storeService = storeService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _rewardPointsSettings = rewardPointsSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether the entered password matches with a saved one
        /// </summary>
        /// <param name="UserPassword">User password</param>
        /// <param name="enteredPassword">The entered password</param>
        /// <returns>True if passwords match; otherwise false</returns>
        protected bool PasswordsMatch(UserPassword userPassword, string enteredPassword)
        {
            if (userPassword == null || string.IsNullOrEmpty(enteredPassword))
                return false;

            var savedPassword = string.Empty;
            switch (userPassword.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    savedPassword = enteredPassword;
                    break;
                case PasswordFormat.Encrypted:
                    savedPassword = _encryptionService.EncryptText(enteredPassword);
                    break;
                case PasswordFormat.Hashed:
                    savedPassword = _encryptionService.CreatePasswordHash(enteredPassword, userPassword.PasswordSalt, _userSettings.HashedPasswordFormat);
                    break;
            }

            if (userPassword.Password == null)
                return false;

            return userPassword.Password.Equals(savedPassword);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Валидация пользователя
        /// </summary>
        /// <param name="usernameOrEmail">Имя пользователя или адрес электронной почты</param>
        /// <param name="password">Пароль</param>
        /// <returns>Результат</returns>
        public virtual UserLoginResults ValidateUser(string usernameOrEmail, string password)
        {
            var user = _userSettings.UsernamesEnabled ?
                _userService.GetUserByUsername(usernameOrEmail) :
                _userService.GetUserByEmail(usernameOrEmail);

            if (user == null)
                return UserLoginResults.UserNotExist;
            if (user.Deleted != null)
                return UserLoginResults.Deleted;
            if (!user.Active)
                return UserLoginResults.NotActive;
            // Только зарегистрированные могут залогиниться:
            if (!_userService.IsRegistered(user))
                return UserLoginResults.NotRegistered;
            // Проверить, не заблокирован ли пользователь:
            if (user.CannotLoginUntilDateUtc.HasValue && user.CannotLoginUntilDateUtc.Value > DateTime.UtcNow)
                return UserLoginResults.LockedOut;

            if (!PasswordsMatch(_userService.GetCurrentPassword(user.Id), password))
            {
                // Некорректный пароль:
                user.FailedLoginAttempts++;
                if (_userSettings.FailedPasswordAllowedAttempts > 0 &&
                    user.FailedLoginAttempts >= _userSettings.FailedPasswordAllowedAttempts)
                {
                    // Заблокировать:
                    user.CannotLoginUntilDateUtc = DateTime.UtcNow.AddMinutes(_userSettings.FailedPasswordLockoutMinutes);
                    // Очистить счётчик:
                    user.FailedLoginAttempts = 0;
                }

                _userService.UpdateUser(user);

                return UserLoginResults.WrongPassword;
            }

            // Обновление деталей входа:
            user.FailedLoginAttempts = 0;
            user.CannotLoginUntilDateUtc = null;
            user.RequireReLogin = false;
            user.LastLoginDateUtc = DateTime.UtcNow;
            _userService.UpdateUser(user);

            return UserLoginResults.Successful;
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual UserRegistrationResult RegisterUser(UserRegistrationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.User == null)
                throw new ArgumentException("Can't load current User");

            var result = new UserRegistrationResult();
            if (request.User.IsSearchEngineAccount())
            {
                result.AddError("Search engine can't be registered");
                return result;
            }

            if (request.User.IsBackgroundTaskAccount())
            {
                result.AddError("Background task account can't be registered");
                return result;
            }

            if (_userService.IsRegistered(request.User))
            {
                result.AddError("Current User is already registered");
                return result;
            }

            if (string.IsNullOrEmpty(request.Email))
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.EmailIsNotProvided"));
                return result;
            }

            if (!CommonHelper.IsValidEmail(request.Email))
            {
                result.AddError(_localizationService.GetResource("Common.WrongEmail"));
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.PasswordIsNotProvided"));
                return result;
            }

            if (_userSettings.UsernamesEnabled && string.IsNullOrEmpty(request.Username))
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.UsernameIsNotProvided"));
                return result;
            }

            //validate unique user
            if (_userService.GetUserByEmail(request.Email) != null)
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.EmailAlreadyExists"));
                return result;
            }

            if (_userSettings.UsernamesEnabled && _userService.GetUserByUsername(request.Username) != null)
            {
                result.AddError(_localizationService.GetResource("Account.Register.Errors.UsernameAlreadyExists"));
                return result;
            }

            //at this point request is valid
            request.User.UserName = request.Username;
            request.User.Email = request.Email;

            var userPassword = new UserPassword
            {
                UserId = request.User.Id,
                PasswordFormat = request.PasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };
            switch (request.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    userPassword.Password = request.Password;
                    break;
                case PasswordFormat.Encrypted:
                    userPassword.Password = _encryptionService.EncryptText(request.Password);
                    break;
                case PasswordFormat.Hashed:
                    var saltKey = _encryptionService.CreateSaltKey(TvProgUserServiceDefaults.PasswordSaltKeySize);
                    userPassword.PasswordSalt = saltKey;
                    userPassword.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey, _userSettings.HashedPasswordFormat);
                    break;
            }

            _userService.InsertUserPassword(userPassword);

            request.User.Active = request.IsApproved;

            //add to 'Registered' role
            var registeredRole = _userService.GetUserRoleBySystemName(TvProgUserDefaults.RegisteredRoleName);
            if (registeredRole == null)
                throw new TvProgException("'Registered' role could not be loaded");
            
            _userService.AddUserRoleMapping(new UserUserRoleMapping { UserId = request.User.Id, UserRoleId = registeredRole.Id });

            //remove from 'Guests' role            
            if (_userService.IsGuest(request.User))
            {                
                var guestRole = _userService.GetUserRoleBySystemName(TvProgUserDefaults.GuestsRoleName);
                _userService.RemoveUserRoleMapping(request.User, guestRole);
            }

            //add reward points for User registration (if enabled)
            if (_rewardPointsSettings.Enabled && _rewardPointsSettings.PointsForRegistration > 0)
            {
                var endDate = _rewardPointsSettings.RegistrationPointsValidity > 0
                    ? (DateTime?)DateTime.UtcNow.AddDays(_rewardPointsSettings.RegistrationPointsValidity.Value) : null;
                _rewardPointService.AddRewardPointsHistoryEntry(request.User, _rewardPointsSettings.PointsForRegistration,
                    request.StoreId, _localizationService.GetResource("RewardPoints.Message.EarnedForRegistration"), endDate: endDate);
            }

            _userService.UpdateUser(request.User);

            return result;
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual ChangePasswordResult ChangePassword(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var result = new ChangePasswordResult();
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.EmailIsNotProvided"));
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.PasswordIsNotProvided"));
                return result;
            }

            var User = _userService.GetUserByEmail(request.Email);
            if (User == null)
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.EmailNotFound"));
                return result;
            }
          
            //request isn't valid
            if (request.ValidateRequest && !PasswordsMatch(_userService.GetCurrentPassword(User.Id), request.OldPassword))
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.OldPasswordDoesntMatch"));
                return result;
            }

            //check for duplicates
            if (_userSettings.UnduplicatedPasswordsNumber > 0)
            {
                //get some of previous passwords
                var previousPasswords = _userService.GetUserPasswords(User.Id, passwordsToReturn: _userSettings.UnduplicatedPasswordsNumber);

                var newPasswordMatchesWithPrevious = previousPasswords.Any(password => PasswordsMatch(password, request.NewPassword));
                if (newPasswordMatchesWithPrevious)
                {
                    result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.PasswordMatchesWithPrevious"));
                    return result;
                }
            }

            //at this point request is valid
            var UserPassword = new UserPassword
            {
                UserId = User.Id,
                PasswordFormat = request.NewPasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };
            switch (request.NewPasswordFormat)
            {
                case PasswordFormat.Clear:
                    UserPassword.Password = request.NewPassword;
                    break;
                case PasswordFormat.Encrypted:
                    UserPassword.Password = _encryptionService.EncryptText(request.NewPassword);
                    break;
                case PasswordFormat.Hashed:
                    var saltKey = _encryptionService.CreateSaltKey(TvProgUserServiceDefaults.PasswordSaltKeySize);
                    UserPassword.PasswordSalt = saltKey;
                    UserPassword.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey,
                        request.HashedPasswordFormat ?? _userSettings.HashedPasswordFormat);
                    break;
            }

            _userService.InsertUserPassword(UserPassword);

            //publish event
            _eventPublisher.Publish(new UserPasswordChangedEvent(UserPassword));

            return result;
        }

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="newEmail">New email</param>
        /// <param name="requireValidation">Require validation of new email address</param>
        public virtual void SetEmail(User User, string newEmail, bool requireValidation)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            if (newEmail == null)
                throw new TvProgException("Email cannot be null");

            newEmail = newEmail.Trim();
            var oldEmail = User.Email;

            if (!CommonHelper.IsValidEmail(newEmail))
                throw new TvProgException(_localizationService.GetResource("Account.EmailUsernameErrors.NewEmailIsNotValid"));

            if (newEmail.Length > 100)
                throw new TvProgException(_localizationService.GetResource("Account.EmailUsernameErrors.EmailTooLong"));

            var User2 = _userService.GetUserByEmail(newEmail);
            if (User2 != null && User.Id != User2.Id)
                throw new TvProgException(_localizationService.GetResource("Account.EmailUsernameErrors.EmailAlreadyExists"));

            if (requireValidation)
            {
                //re-validate email
                User.EmailToRevalidate = newEmail;
                _userService.UpdateUser(User);

                //email re-validation message
                _genericAttributeService.SaveAttribute(User, TvProgUserDefaults.EmailRevalidationTokenAttribute, Guid.NewGuid().ToString());
                _workflowMessageService.SendUserEmailRevalidationMessage(User, _workContext.WorkingLanguage.Id);
            }
            else
            {
                User.Email = newEmail;
                _userService.UpdateUser(User);
                
                if (string.IsNullOrEmpty(oldEmail) || oldEmail.Equals(newEmail, StringComparison.InvariantCultureIgnoreCase)) 
                    return;

                //update newsletter subscription (if required)
                foreach (var store in _storeService.GetAllStores())
                {
                    var subscriptionOld = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(oldEmail, store.Id);
                    
                    if (subscriptionOld == null) 
                        continue;

                    subscriptionOld.Email = newEmail;
                    _newsLetterSubscriptionService.UpdateNewsLetterSubscription(subscriptionOld);
                }
            }
        }

        /// <summary>
        /// Установка пользователю пользовательского имени
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="newUsername">Новый пользователь</param>
        public virtual void SetUsername(User user, string newUsername)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!_userSettings.UsernamesEnabled)
                throw new TvProgException("Пользовательские имена отключены");

            newUsername = newUsername.Trim();

            if (newUsername.Length > TvProgUserServiceDefaults.UserUsernameLength)
                throw new TvProgException(_localizationService.GetResource("Account.EmailUsernameErrors.UsernameTooLong"));

            var user2 = _userService.GetUserByUsername(newUsername);
            if (user2 != null && user.Id != user2.Id)
                throw new TvProgException(_localizationService.GetResource("Account.EmailUsernameErrors.UsernameAlreadyExists"));

            user.UserName = newUsername;
            _userService.UpdateUser(user);
        }

        #endregion
    }
}