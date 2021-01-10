using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TVProgViewer.Core;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.Data;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Localization;

namespace TVProgViewer.Services.Installation
{
    /// <summary>
    /// Installation service using SQL files (fast installation)
    /// </summary>
    public partial class SqlFileInstallationService : IInstallationService
    {
        #region Fields

        private readonly ITvProgFileProvider _fileProvider;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public SqlFileInstallationService(ITvProgFileProvider fileProvider,
            IRepository<User> UserRepository,
            IRepository<Language> languageRepository,
            IRepository<Store> storeRepository,
            IWebHelper webHelper)
        {
           _fileProvider = fileProvider;
            _userRepository = UserRepository;
            _languageRepository = languageRepository;
            _storeRepository = storeRepository;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Install locales
        /// </summary>
        protected virtual void InstallLocaleResources()
        {
            //'English' language
            var language = _languageRepository.Table.Single(l => l.Name == "English");

            //save resources
            var directoryPath = _fileProvider.MapPath(TvProgInstallationDefaults.LocalizationResourcesPath);
            var pattern = $"*.{TvProgInstallationDefaults.LocalizationResourcesFileExtension}";
            foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, pattern))
            {
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                using (var streamReader = new StreamReader(filePath))
                {
                    localizationService.ImportResourcesFromXml(language, streamReader);
                }
            }
        }

        /// <summary>
        /// Обновление пользователя по умолчанию
        /// </summary>
        /// <param name="defaultUserEmail">Адрес эл. почты</param>
        /// <param name="defaultUserPassword">Пароль</param>
        protected virtual void UpdateDefaultUser(string defaultUserEmail, string defaultUserPassword)
        {
            var adminUser = _userRepository.Table.Single(x => x.Email == "admin@yourStore.com");
            if (adminUser == null)
                throw new Exception("Admin user cannot be loaded");

            adminUser.UserGuid = Guid.NewGuid();
            adminUser.Email = defaultUserEmail;
            adminUser.UserName = defaultUserEmail;
            _userRepository.Update(adminUser);

            var UserRegistrationService = EngineContext.Current.Resolve<IUserRegistrationService>();
            UserRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword, null, TvProgUserServiceDefaults.DefaultHashedPasswordFormat));
        }

        /// <summary>
        /// Update default store URL
        /// </summary>
        protected virtual void UpdateDefaultStoreUrl()
        {
            var store = _storeRepository.Table.FirstOrDefault();
            if (store == null)
                throw new Exception("Default store cannot be loaded");

            store.Url = _webHelper.GetStoreLocation(false);
            _storeRepository.Update(store);
        }

        /// <summary>
        /// Execute SQL file
        /// </summary>
        /// <param name="path">File path</param>
        protected virtual void ExecuteSqlFile(string path)
        {
            var statements = new List<string>();

            using (var reader = new StreamReader(path))
            {
                string statement;
                while ((statement = ReadNextStatementFromStream(reader)) != null)
                    statements.Add(statement);
            }

            var dataProvider = EngineContext.Current.Resolve<IDataProvider>();

            foreach (var stmt in statements)
                dataProvider.Query<object>(stmt);
        }

        /// <summary>
        /// Read next statement from stream
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <returns>Result</returns>
        protected virtual string ReadNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();
            while (true)
            {
                var lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();

                    return null;
                }

                if (lineOfText.TrimEnd().ToUpper() == "GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Install required data
        /// </summary>
        /// <param name="defaultUserEmail">Default user email</param>
        /// <param name="defaultUserPassword">Default user password</param>
        public virtual void InstallRequiredData(string defaultUserEmail, string defaultUserPassword)
        {
            ExecuteSqlFile(_fileProvider.MapPath(TvProgInstallationDefaults.RequiredDataPath));
            InstallLocaleResources();
            UpdateDefaultUser(defaultUserEmail, defaultUserPassword);
            UpdateDefaultStoreUrl();
        }

        /// <summary>
        /// Install sample data
        /// </summary>
        /// <param name="defaultUserEmail">Default user email</param>
        public virtual void InstallSampleData(string defaultUserEmail)
        {
            ExecuteSqlFile(_fileProvider.MapPath(TvProgInstallationDefaults.SampleDataPath));
        }

        #endregion
    }
}