﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain;
using TvProgViewer.Core.Domain.Affiliates;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Cms;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Logging;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Payments;
using TvProgViewer.Core.Domain.Polls;
using TvProgViewer.Core.Domain.ScheduleTasks;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Seo;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Domain.Topics;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Http;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Core.Security;
using TvProgViewer.Data;
using TvProgViewer.Services.Blogs;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.ExportImport;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.News;
using TvProgViewer.Services.Seo;

namespace TvProgViewer.Services.Installation
{
    /// <summary>
    /// Installation service
    /// </summary>
    public partial class InstallationService : IInstallationService
    {
        #region Fields

        private readonly ITvProgDataProvider _dataProvider;
        private readonly ITvProgFileProvider _fileProvider;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<CategoryTemplate> _categoryTemplateRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<DeliveryDate> _deliveryDateRepository;
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<ManufacturerTemplate> _manufacturerTemplateRepository;
        private readonly IRepository<MeasureDimension> _measureDimensionRepository;
        private readonly IRepository<MeasureWeight> _measureWeightRepository;
        private readonly IRepository<TvChannel> _tvChannelRepository;
        private readonly IRepository<TvChannelAttribute> _tvChannelAttributeRepository;
        private readonly IRepository<TvChannelAvailabilityRange> _tvChannelAvailabilityRangeRepository;
        private readonly IRepository<TvChannelTag> _tvChannelTagRepository;
        private readonly IRepository<TvChannelTemplate> _tvChannelTemplateRepository;
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;
        private readonly IRepository<SpecificationAttributeOption> _specificationAttributeOptionRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<TaxCategory> _taxCategoryRepository;
        private readonly IRepository<TopicTemplate> _topicTemplateRepository;
        private readonly IRepository<UrlRecord> _urlRecordRepository;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public InstallationService(ITvProgDataProvider dataProvider,
            ITvProgFileProvider fileProvider,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<Address> addressRepository,
            IRepository<Category> categoryRepository,
            IRepository<CategoryTemplate> categoryTemplateRepository,
            IRepository<Country> countryRepository,
            IRepository<Currency> currencyRepository,
            IRepository<User> userRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<DeliveryDate> deliveryDateRepository,
            IRepository<EmailAccount> emailAccountRepository,
            IRepository<Language> languageRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<ManufacturerTemplate> manufacturerTemplateRepository,
            IRepository<MeasureDimension> measureDimensionRepository,
            IRepository<MeasureWeight> measureWeightRepository,
            IRepository<TvChannel> tvChannelRepository,
            IRepository<TvChannelAttribute> tvChannelAttributeRepository,
            IRepository<TvChannelAvailabilityRange> tvChannelAvailabilityRangeRepository,
            IRepository<TvChannelTag> tvChannelTagRepository,
            IRepository<TvChannelTemplate> tvChannelTemplateRepository,
            IRepository<SpecificationAttribute> specificationAttributeRepository,
            IRepository<SpecificationAttributeOption> specificationAttributeOptionRepository,
            IRepository<StateProvince> stateProvinceRepository,
            IRepository<Store> storeRepository,
            IRepository<TaxCategory> taxCategoryRepository,
            IRepository<TopicTemplate> topicTemplateRepository,
            IRepository<UrlRecord> urlRecordRepository,
            IWebHelper webHelper)
        {
            _dataProvider = dataProvider;
            _fileProvider = fileProvider;
            _activityLogTypeRepository = activityLogTypeRepository;
            _addressRepository = addressRepository;
            _categoryRepository = categoryRepository;
            _categoryTemplateRepository = categoryTemplateRepository;
            _countryRepository = countryRepository;
            _currencyRepository = currencyRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _deliveryDateRepository = deliveryDateRepository;
            _emailAccountRepository = emailAccountRepository;
            _languageRepository = languageRepository;
            _manufacturerRepository = manufacturerRepository;
            _manufacturerTemplateRepository = manufacturerTemplateRepository;
            _measureDimensionRepository = measureDimensionRepository;
            _measureWeightRepository = measureWeightRepository;
            _tvChannelAttributeRepository = tvChannelAttributeRepository;
            _tvChannelAvailabilityRangeRepository = tvChannelAvailabilityRangeRepository;
            _tvChannelRepository = tvChannelRepository;
            _tvChannelTagRepository = tvChannelTagRepository;
            _tvChannelTemplateRepository = tvChannelTemplateRepository;
            _specificationAttributeRepository = specificationAttributeRepository;
            _specificationAttributeOptionRepository = specificationAttributeOptionRepository;
            _stateProvinceRepository = stateProvinceRepository;
            _storeRepository = storeRepository;
            _taxCategoryRepository = taxCategoryRepository;
            _topicTemplateRepository = topicTemplateRepository;
            _urlRecordRepository = urlRecordRepository;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task<T> InsertInstallationDataAsync<T>(T entity) where T : BaseEntity
        {
            return await _dataProvider.InsertEntityAsync(entity);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InsertInstallationDataAsync<T>(params T[] entities) where T : BaseEntity
        {
            await _dataProvider.BulkInsertEntitiesAsync(entities);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InsertInstallationDataAsync<T>(IList<T> entities) where T : BaseEntity
        {
            if (!entities.Any())
                return;

            await InsertInstallationDataAsync(entities.ToArray());
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task UpdateInstallationDataAsync<T>(T entity) where T : BaseEntity
        {
            await _dataProvider.UpdateEntityAsync(entity);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task UpdateInstallationDataAsync<T>(IList<T> entities) where T : BaseEntity
        {
            if (!entities.Any())
                return;

            foreach (var entity in entities)
                await _dataProvider.UpdateEntityAsync(entity);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task<int> GetSpecificationAttributeOptionIdAsync(string specAttributeName, string specAttributeOptionName)
        {
            var specificationAttribute = await _specificationAttributeRepository.Table
                .SingleAsync(sa => sa.Name == specAttributeName);

            var specificationAttributeOption = await _specificationAttributeOptionRepository.Table
                .SingleAsync(sao => sao.Name == specAttributeOptionName && sao.SpecificationAttributeId == specificationAttribute.Id);

            return specificationAttributeOption.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tvChannel"></param>
        /// <param name="fileName"></param>
        /// <param name="displayOrder"></param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the identifier of inserted picture
        /// </returns>
        protected virtual async Task<int> InsertTvChannelPictureAsync(TvChannel tvChannel, string fileName, int displayOrder = 1)
        {
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = GetSamplesPath();

            var pic = await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, fileName)), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync(tvChannel.Name));

            await InsertInstallationDataAsync(
                new TvChannelPicture
                {
                    TvChannelId = tvChannel.Id,
                    PictureId = pic.Id,
                    DisplayOrder = displayOrder
                });

            return pic.Id;
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task<string> ValidateSeNameAsync<T>(T entity, string seName) where T : BaseEntity
        {
            //duplicate of ValidateSeName method of \TvProg.Services\Seo\UrlRecordService.cs (we cannot inject it here)
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            //validation
            var okChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
            seName = seName.Trim().ToLowerInvariant();

            var sb = new StringBuilder();
            foreach (var c in seName.ToCharArray())
            {
                var c2 = c.ToString();
                if (okChars.Contains(c2))
                    sb.Append(c2);
            }

            seName = sb.ToString();
            seName = seName.Replace(" ", "-");
            while (seName.Contains("--"))
                seName = seName.Replace("--", "-");
            while (seName.Contains("__"))
                seName = seName.Replace("__", "_");

            //max length
            seName = CommonHelper.EnsureMaximumLength(seName, TvProgSeoDefaults.SearchEngineNameLength);

            //ensure this seName is not reserved yet
            var i = 2;
            var tempSeName = seName;
            while (true)
            {
                //check whether such slug already exists (and that is not the current entity)

                var query = from ur in _urlRecordRepository.Table
                            where tempSeName != null && ur.Slug == tempSeName
                            select ur;
                var urlRecord = await query.FirstOrDefaultAsync();

                var entityName = entity.GetType().Name;
                var reserved = urlRecord != null && !(urlRecord.EntityId == entity.Id && urlRecord.EntityName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
                if (!reserved)
                    break;

                tempSeName = $"{seName}-{i}";
                i++;
            }

            seName = tempSeName;

            return seName;
        }

        protected virtual string GetSamplesPath()
        {
            return _fileProvider.GetAbsolutePath(TvProgInstallationDefaults.SampleImagesPath);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallStoresAsync()
        {
            var storeUrl = _webHelper.GetStoreLocation();
            var stores = new List<Store>
            {
                new Store
                {
                    Name = "TvProgViewer",
                    DefaultTitle = "TvProgViewer",
                    DefaultMetaKeywords = string.Empty,
                    DefaultMetaDescription = string.Empty,
                    HomepageTitle = "Home page title",
                    HomepageDescription = "Home page description",
                    Url = storeUrl,
                    SslEnabled = _webHelper.IsCurrentConnectionSecured(),
                    Hosts = "yourstore.com,www.yourstore.com",
                    DisplayOrder = 1,
                    //should we set some default company info?
                    CompanyName = "Your company name",
                    CompanyAddress = "your company country, state, zip, street, etc",
                    CompanyPhoneNumber = "(123) 456-78901",
                    CompanyVat = null
                }
            };

            await InsertInstallationDataAsync(stores);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallMeasuresAsync(RegionInfo regionInfo)
        {
            var isMetric = regionInfo?.IsMetric ?? false;

            var measureDimensions = new List<MeasureDimension>
            {
                new MeasureDimension
                {
                    Name = "inch(es)",
                    SystemKeyword = "inches",
                    Ratio = isMetric ? 39.3701M : 1M,
                    DisplayOrder = isMetric ? 1 : 0
                },
                new MeasureDimension
                {
                    Name = "feet",
                    SystemKeyword = "feet",
                    Ratio = isMetric ? 3.28084M : 0.08333333M,
                    DisplayOrder = isMetric ? 1 : 0
                },
                new MeasureDimension
                {
                    Name = "meter(s)",
                    SystemKeyword = "meters",
                    Ratio = isMetric ? 1M : 0.0254M,
                    DisplayOrder = isMetric ? 0 : 1
                },
                new MeasureDimension
                {
                    Name = "millimetre(s)",
                    SystemKeyword = "millimetres",
                    Ratio = isMetric ? 1000M : 25.4M,
                    DisplayOrder = isMetric ? 0 : 1
                }
            };

            await InsertInstallationDataAsync(measureDimensions);

            var measureWeights = new List<MeasureWeight>
            {
                new MeasureWeight
                {
                    Name = "ounce(s)",
                    SystemKeyword = "ounce",
                    Ratio = isMetric ? 35.274M : 16M,
                    DisplayOrder = isMetric ? 1 : 0
                },
                new MeasureWeight
                {
                    Name = "lb(s)",
                    SystemKeyword = "lb",
                    Ratio = isMetric ? 2.20462M : 1M,
                    DisplayOrder = isMetric ? 1 : 0
                },
                new MeasureWeight
                {
                    Name = "kg(s)",
                    SystemKeyword = "kg",
                    Ratio = isMetric ? 1M : 0.45359237M,
                    DisplayOrder = isMetric ? 0 : 1
                },
                new MeasureWeight
                {
                    Name = "gram(s)",
                    SystemKeyword = "grams",
                    Ratio = isMetric ? 1000M : 453.59237M,
                    DisplayOrder = isMetric ? 0 : 1
                }
            };

            await InsertInstallationDataAsync(measureWeights);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallTaxCategoriesAsync()
        {
            var taxCategories = new List<TaxCategory>
            {
                new TaxCategory {Name = "Books", DisplayOrder = 1},
                new TaxCategory {Name = "Electronics & Software", DisplayOrder = 5},
                new TaxCategory {Name = "Downloadable TvChannels", DisplayOrder = 10},
                new TaxCategory {Name = "Jewelry", DisplayOrder = 15},
                new TaxCategory {Name = "Apparel", DisplayOrder = 20}
            };

            await InsertInstallationDataAsync(taxCategories);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallLanguagesAsync((string languagePackDownloadLink, int languagePackProgress) languagePackInfo, CultureInfo cultureInfo, RegionInfo regionInfo)
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var defaultCulture = new CultureInfo(TvProgCommonDefaults.DefaultLanguageCulture);
            var defaultLanguage = new Language
            {
                Name = defaultCulture.TwoLetterISOLanguageName.ToUpperInvariant(),
                LanguageCulture = defaultCulture.Name,
                UniqueSeoCode = defaultCulture.TwoLetterISOLanguageName,
                FlagImageFileName = $"{defaultCulture.Name.ToLowerInvariant()[^2..]}.png",
                Rtl = defaultCulture.TextInfo.IsRightToLeft,
                Published = true,
                DisplayOrder = 1
            };
            await InsertInstallationDataAsync(defaultLanguage);

            //Install locale resources for default culture
            var directoryPath = _fileProvider.MapPath(TvProgInstallationDefaults.LocalizationResourcesPath);
            var pattern = $"*.{TvProgInstallationDefaults.LocalizationResourcesFileExtension}";
            foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, pattern))
            {
                using var streamReader = new StreamReader(filePath);
                await localizationService.ImportResourcesFromXmlAsync(defaultLanguage, streamReader);
            }

            if (cultureInfo == null || regionInfo == null || cultureInfo.Name == TvProgCommonDefaults.DefaultLanguageCulture)
                return;

            var language = new Language
            {
                Name = cultureInfo.TwoLetterISOLanguageName.ToUpperInvariant(),
                LanguageCulture = cultureInfo.Name,
                UniqueSeoCode = cultureInfo.TwoLetterISOLanguageName,
                FlagImageFileName = $"{regionInfo.TwoLetterISORegionName.ToLowerInvariant()}.png",
                Rtl = cultureInfo.TextInfo.IsRightToLeft,
                Published = true,
                DisplayOrder = 2
            };
            await InsertInstallationDataAsync(language);

            if (string.IsNullOrEmpty(languagePackInfo.languagePackDownloadLink))
                return;

            //download and import language pack
            try
            {
                var httpClientFactory = EngineContext.Current.Resolve<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(TvProgHttpDefaults.DefaultHttpClient);
                await using var stream = await httpClient.GetStreamAsync(languagePackInfo.languagePackDownloadLink);
                using var streamReader = new StreamReader(stream);
                await localizationService.ImportResourcesFromXmlAsync(language, streamReader);

                //set this language as default
                language.DisplayOrder = 0;
                await UpdateInstallationDataAsync(language);

                //save progress for showing in admin panel (only for first start)
                await InsertInstallationDataAsync(new GenericAttribute
                {
                    EntityId = language.Id,
                    Key = TvProgCommonDefaults.LanguagePackProgressAttribute,
                    KeyGroup = nameof(Language),
                    Value = languagePackInfo.languagePackProgress.ToString(),
                    StoreId = 0,
                    CreatedOrUpdatedDateUTC = DateTime.UtcNow
                });
            }
            catch { }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallCurrenciesAsync(CultureInfo cultureInfo, RegionInfo regionInfo)
        {
            //set some currencies with a rate against the USD
            var defaultCurrencies = new List<string>() { "USD", "AUD", "GBP", "CAD", "CNY", "EUR", "HKD", "JPY", "RUB", "SEK", "INR" };
            var currencies = new List<Currency>
            {
                new Currency
                {
                    Name = "US Dollar",
                    CurrencyCode = "USD",
                    Rate = 1,
                    DisplayLocale = "en-US",
                    CustomFormatting = string.Empty,
                    Published = true,
                    DisplayOrder = 1,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Australian Dollar",
                    CurrencyCode = "AUD",
                    Rate = 1.34M,
                    DisplayLocale = "en-AU",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 2,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "British Pound",
                    CurrencyCode = "GBP",
                    Rate = 0.75M,
                    DisplayLocale = "en-GB",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 3,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Canadian Dollar",
                    CurrencyCode = "CAD",
                    Rate = 1.32M,
                    DisplayLocale = "en-CA",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 4,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Chinese Yuan Renminbi",
                    CurrencyCode = "CNY",
                    Rate = 6.43M,
                    DisplayLocale = "zh-CN",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 5,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Euro",
                    CurrencyCode = "EUR",
                    Rate = 0.86M,
                    DisplayLocale = string.Empty,
                    CustomFormatting = $"{"\u20ac"}0.00", //euro symbol
                    Published = false,
                    DisplayOrder = 6,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Hong Kong Dollar",
                    CurrencyCode = "HKD",
                    Rate = 7.84M,
                    DisplayLocale = "zh-HK",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 7,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Japanese Yen",
                    CurrencyCode = "JPY",
                    Rate = 110.45M,
                    DisplayLocale = "ja-JP",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 8,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Russian Rouble",
                    CurrencyCode = "RUB",
                    Rate = 63.25M,
                    DisplayLocale = "ru-RU",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 9,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                },
                new Currency
                {
                    Name = "Swedish Krona",
                    CurrencyCode = "SEK",
                    Rate = 8.80M,
                    DisplayLocale = "sv-SE",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 10,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding1
                },
                new Currency
                {
                    Name = "Indian Rupee",
                    CurrencyCode = "INR",
                    Rate = 68.03M,
                    DisplayLocale = "en-IN",
                    CustomFormatting = string.Empty,
                    Published = false,
                    DisplayOrder = 12,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    RoundingType = RoundingType.Rounding001
                }
            };

            //set additional currency
            if (cultureInfo != null && regionInfo != null)
            {
                if (!defaultCurrencies.Contains(regionInfo.ISOCurrencySymbol))
                {
                    currencies.Add(new Currency
                    {
                        Name = regionInfo.CurrencyEnglishName,
                        CurrencyCode = regionInfo.ISOCurrencySymbol,
                        Rate = 1,
                        DisplayLocale = cultureInfo.Name,
                        CustomFormatting = string.Empty,
                        Published = true,
                        DisplayOrder = 0,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                        RoundingType = RoundingType.Rounding001
                    });
                }

                foreach (var currency in currencies.Where(currency => currency.CurrencyCode == regionInfo.ISOCurrencySymbol))
                {
                    currency.Published = true;
                    currency.DisplayOrder = 0;
                }
            }


            await InsertInstallationDataAsync(currencies);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallCountriesAndStatesAsync()
        {
            var countries = ISO3166.GetCollection().Select(country => new Country
            {
                Name = country.Name,
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = country.Alpha2,
                ThreeLetterIsoCode = country.Alpha3,
                NumericIsoCode = country.NumericCode,
                SubjectToVat = country.SubjectToVat,
                DisplayOrder = country.NumericCode == 840 ? 1 : 100,
                Published = true
            }).ToList();

            await InsertInstallationDataAsync(countries.ToArray());

            //Import states for all countries
            var directoryPath = _fileProvider.MapPath(TvProgInstallationDefaults.LocalizationResourcesPath);
            var pattern = "*.txt";

            //we use different scope to prevent creating wrong settings in DI, because the settings data not exists yet
            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            using var scope = serviceScopeFactory.CreateScope();
            var importManager = EngineContext.Current.Resolve<IImportManager>(scope);
            foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, pattern))
            {
                await using var stream = new FileStream(filePath, FileMode.Open);
                await importManager.ImportStatesFromTxtAsync(stream, false);
            }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallShippingMethodsAsync()
        {
            var shippingMethods = new List<ShippingMethod>
            {
                new ShippingMethod
                {
                    Name = "Ground",
                    Description =
                        "Shipping by land transport",
                    DisplayOrder = 1
                },
                new ShippingMethod
                {
                    Name = "Next Day Air",
                    Description = "The one day air shipping",
                    DisplayOrder = 2
                },
                new ShippingMethod
                {
                    Name = "2nd Day Air",
                    Description = "The two day air shipping",
                    DisplayOrder = 3
                }
            };

            await InsertInstallationDataAsync(shippingMethods);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallDeliveryDatesAsync()
        {
            var deliveryDates = new List<DeliveryDate>
            {
                new DeliveryDate
                {
                    Name = "1-2 days",
                    DisplayOrder = 1
                },
                new DeliveryDate
                {
                    Name = "3-5 days",
                    DisplayOrder = 5
                },
                new DeliveryDate
                {
                    Name = "1 week",
                    DisplayOrder = 10
                }
            };

            await InsertInstallationDataAsync(deliveryDates);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallTvChannelAvailabilityRangesAsync()
        {
            var tvChannelAvailabilityRanges = new List<TvChannelAvailabilityRange>
            {
                new TvChannelAvailabilityRange
                {
                    Name = "2-4 days",
                    DisplayOrder = 1
                },
                new TvChannelAvailabilityRange
                {
                    Name = "7-10 days",
                    DisplayOrder = 2
                },
                new TvChannelAvailabilityRange
                {
                    Name = "2 weeks",
                    DisplayOrder = 3
                }
            };

            await InsertInstallationDataAsync(tvChannelAvailabilityRanges);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallSampleUsersAsync()
        {
            var crRegistered = await _userRoleRepository.Table
                .FirstOrDefaultAsync(userRole => userRole.SystemName == TvProgUserDefaults.RegisteredRoleName);

            if (crRegistered == null)
                throw new ArgumentNullException(nameof(crRegistered));

            //default store 
            var defaultStore = await _storeRepository.Table.FirstOrDefaultAsync();

            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            var storeId = defaultStore.Id;

            //second user
            var secondUserEmail = "steve_gates@tvprogviewer.ru";
            var secondUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = secondUserEmail,
                Username = secondUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultSecondUserAddress = await InsertInstallationDataAsync(
                new Address
                {
                    FirstName = "Steve",
                    LastName = "Gates",
                    MiddleName = "Stevich",
                    PhoneNumber = "87654321",
                    Email = secondUserEmail,
                    FaxNumber = string.Empty,
                    Company = "Steve Company",
                    Address1 = "750 Bel Air Rd.",
                    Address2 = string.Empty,
                    City = "Los Angeles",
                    StateProvinceId = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "California")?.Id,
                    CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA")?.Id,
                    ZipPostalCode = "90077",
                    CreatedOnUtc = DateTime.UtcNow
                });

            secondUser.BillingAddressId = defaultSecondUserAddress.Id;
            secondUser.ShippingAddressId = defaultSecondUserAddress.Id;
            secondUser.FirstName = defaultSecondUserAddress.FirstName;
            secondUser.LastName = defaultSecondUserAddress.LastName;
            secondUser.MiddleName = defaultSecondUserAddress.MiddleName;


            await InsertInstallationDataAsync(secondUser);

            await InsertInstallationDataAsync(new UserAddressMapping { UserId = secondUser.Id, AddressId = defaultSecondUserAddress.Id });
            await InsertInstallationDataAsync(new UserUserRoleMapping { UserId = secondUser.Id, UserRoleId = crRegistered.Id });

            //set user password
            await InsertInstallationDataAsync(new UserPassword
            {
                UserId = secondUser.Id,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //third user
            var thirdUserEmail = "arthur_holmes@tvprogviewer.ru";
            var thirdUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = thirdUserEmail,
                Username = thirdUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };

            var defaultThirdUserAddress = await InsertInstallationDataAsync(
                new Address
                {
                    FirstName = "Arthur",
                    LastName = "Holmes",
                    MiddleName = "Arthurovich",
                    PhoneNumber = "111222333",
                    Email = thirdUserEmail,
                    FaxNumber = string.Empty,
                    Company = "Holmes Company",
                    Address1 = "221B Baker Street",
                    Address2 = string.Empty,
                    City = "London",
                    CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "GBR")?.Id,
                    ZipPostalCode = "NW1 6XE",
                    CreatedOnUtc = DateTime.UtcNow
                });

            thirdUser.BillingAddressId = defaultThirdUserAddress.Id;
            thirdUser.ShippingAddressId = defaultThirdUserAddress.Id;
            thirdUser.FirstName = defaultThirdUserAddress.FirstName;
            thirdUser.LastName = defaultThirdUserAddress.LastName;
            thirdUser.MiddleName = defaultThirdUserAddress.MiddleName;

            await InsertInstallationDataAsync(thirdUser);

            await InsertInstallationDataAsync(new UserAddressMapping { UserId = thirdUser.Id, AddressId = defaultThirdUserAddress.Id });
            await InsertInstallationDataAsync(new UserUserRoleMapping { UserId = thirdUser.Id, UserRoleId = crRegistered.Id });

            //set user password
            await InsertInstallationDataAsync(new UserPassword
            {
                UserId = thirdUser.Id,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //fourth user
            var fourthUserEmail = "james_pan@tvprogviewer.ru";
            var fourthUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = fourthUserEmail,
                Username = fourthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultFourthUserAddress = await InsertInstallationDataAsync(
                new Address
                {
                    FirstName = "James",
                    LastName = "Pan",
                    MiddleName = "Jamesovich",
                    PhoneNumber = "369258147",
                    Email = fourthUserEmail,
                    FaxNumber = string.Empty,
                    Company = "Pan Company",
                    Address1 = "St Katharine’s West 16",
                    Address2 = string.Empty,
                    City = "St Andrews",
                    CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "GBR")?.Id,
                    ZipPostalCode = "KY16 9AX",
                    CreatedOnUtc = DateTime.UtcNow
                });

            fourthUser.BillingAddressId = defaultFourthUserAddress.Id;
            fourthUser.ShippingAddressId = defaultFourthUserAddress.Id;
            fourthUser.FirstName = defaultFourthUserAddress.FirstName;
            fourthUser.LastName = defaultFourthUserAddress.LastName;
            fourthUser.MiddleName = defaultFourthUserAddress.MiddleName;

            await InsertInstallationDataAsync(fourthUser);

            await InsertInstallationDataAsync(new UserAddressMapping { UserId = fourthUser.Id, AddressId = defaultFourthUserAddress.Id });
            await InsertInstallationDataAsync(new UserUserRoleMapping { UserId = fourthUser.Id, UserRoleId = crRegistered.Id });

            //set user password
            await InsertInstallationDataAsync(new UserPassword
            {
                UserId = fourthUser.Id,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //fifth user
            var fifthUserEmail = "brenda_lindgren@tvprogviewer.ru";
            var fifthUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = fifthUserEmail,
                Username = fifthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultFifthUserAddress = await InsertInstallationDataAsync(
                new Address
                {
                    FirstName = "Brenda",
                    LastName = "Lindgren",
                    MiddleName = "Brendovna",
                    PhoneNumber = "14785236",
                    Email = fifthUserEmail,
                    FaxNumber = string.Empty,
                    Company = "Brenda Company",
                    Address1 = "1249 Tongass Avenue, Suite B",
                    Address2 = string.Empty,
                    City = "Ketchikan",
                    StateProvinceId = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "Alaska")?.Id,
                    CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA")?.Id,
                    ZipPostalCode = "99901",
                    CreatedOnUtc = DateTime.UtcNow
                });

            fifthUser.BillingAddressId = defaultFifthUserAddress.Id;
            fifthUser.ShippingAddressId = defaultFifthUserAddress.Id;
            fifthUser.FirstName = defaultFifthUserAddress.FirstName;
            fifthUser.LastName = defaultFifthUserAddress.LastName;
            fifthUser.MiddleName = defaultFifthUserAddress.MiddleName;

            await InsertInstallationDataAsync(fifthUser);

            await InsertInstallationDataAsync(new UserAddressMapping { UserId = fifthUser.Id, AddressId = defaultFifthUserAddress.Id });
            await InsertInstallationDataAsync(new UserUserRoleMapping { UserId = fifthUser.Id, UserRoleId = crRegistered.Id });

            //set user password
            await InsertInstallationDataAsync(new UserPassword
            {
                UserId = fifthUser.Id,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //sixth user
            var sixthUserEmail = "victoria_victoria@tvprogviewer.ru";
            var sixthUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = sixthUserEmail,
                Username = sixthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultSixthUserAddress = await InsertInstallationDataAsync(
                new Address
                {
                    FirstName = "Victoria",
                    LastName = "Terces",
                    MiddleName = "Victorovna",
                    PhoneNumber = "45612378",
                    Email = sixthUserEmail,
                    FaxNumber = string.Empty,
                    Company = "Terces Company",
                    Address1 = "201 1st Avenue South",
                    Address2 = string.Empty,
                    City = "Saskatoon",
                    StateProvinceId = (await _stateProvinceRepository.Table.FirstOrDefaultAsync(sp => sp.Name == "Saskatchewan"))?.Id,
                    CountryId = (await _countryRepository.Table.FirstOrDefaultAsync(c => c.ThreeLetterIsoCode == "CAN"))?.Id,
                    ZipPostalCode = "S7K 1J9",
                    CreatedOnUtc = DateTime.UtcNow
                });

            sixthUser.BillingAddressId = defaultSixthUserAddress.Id;
            sixthUser.ShippingAddressId = defaultSixthUserAddress.Id;
            sixthUser.FirstName = defaultSixthUserAddress.FirstName;
            sixthUser.LastName = defaultSixthUserAddress.LastName;
            sixthUser.MiddleName = defaultSixthUserAddress.MiddleName;

            await InsertInstallationDataAsync(sixthUser);

            await InsertInstallationDataAsync(new UserAddressMapping { UserId = sixthUser.Id, AddressId = defaultSixthUserAddress.Id });
            await InsertInstallationDataAsync(new UserUserRoleMapping { UserId = sixthUser.Id, UserRoleId = crRegistered.Id });

            //set user password
            await InsertInstallationDataAsync(new UserPassword
            {
                UserId = sixthUser.Id,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallUsersAndUsersAsync(string defaultUserEmail, string defaultUserPassword)
        {
            var crAdministrators = new UserRole
            {
                Name = "Administrators",
                Active = true,
                IsSystemRole = true,
                SystemName = TvProgUserDefaults.AdministratorsRoleName
            };
            var crForumModerators = new UserRole
            {
                Name = "Forum Moderators",
                Active = true,
                IsSystemRole = true,
                SystemName = TvProgUserDefaults.ForumModeratorsRoleName
            };
            var crRegistered = new UserRole
            {
                Name = "Registered",
                Active = true,
                IsSystemRole = true,
                SystemName = TvProgUserDefaults.RegisteredRoleName
            };
            var crGuests = new UserRole
            {
                Name = "Guests",
                Active = true,
                IsSystemRole = true,
                SystemName = TvProgUserDefaults.GuestsRoleName
            };
            var crVendors = new UserRole
            {
                Name = "Vendors",
                Active = true,
                IsSystemRole = true,
                SystemName = TvProgUserDefaults.VendorsRoleName
            };
            var userRoles = new List<UserRole>
            {
                crAdministrators,
                crForumModerators,
                crRegistered,
                crGuests,
                crVendors
            };

            await InsertInstallationDataAsync(userRoles);

            //default store 
            var defaultStore = await _storeRepository.Table.FirstOrDefaultAsync();

            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            var storeId = defaultStore.Id;

            //admin user
            var adminUser = new User
            {
                UserGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                Username = defaultUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };

            var defaultAdminUserAddress = await InsertInstallationDataAsync(
                new Address
                {
                    FirstName = "John",
                    LastName = "Smith",
                    MiddleName = "Johnovich",
                    PhoneNumber = "12345678",
                    Email = defaultUserEmail,
                    FaxNumber = string.Empty,
                    Company = "TvProg Solutions Ltd",
                    Address1 = "21 West 52nd Street",
                    Address2 = string.Empty,
                    City = "New York",
                    StateProvinceId = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York")?.Id,
                    CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA")?.Id,
                    ZipPostalCode = "10021",
                    CreatedOnUtc = DateTime.UtcNow
                });

            adminUser.BillingAddressId = defaultAdminUserAddress.Id;
            adminUser.ShippingAddressId = defaultAdminUserAddress.Id;
            adminUser.FirstName = defaultAdminUserAddress.FirstName;
            adminUser.LastName = defaultAdminUserAddress.LastName;
            adminUser.MiddleName = defaultAdminUserAddress.MiddleName;

            await InsertInstallationDataAsync(adminUser);

            await InsertInstallationDataAsync(new UserAddressMapping { UserId = adminUser.Id, AddressId = defaultAdminUserAddress.Id });

            await InsertInstallationDataAsync(
                new UserUserRoleMapping { UserId = adminUser.Id, UserRoleId = crAdministrators.Id },
                new UserUserRoleMapping { UserId = adminUser.Id, UserRoleId = crForumModerators.Id },
                new UserUserRoleMapping { UserId = adminUser.Id, UserRoleId = crRegistered.Id });

            //set hashed admin password
            var userRegistrationService = EngineContext.Current.Resolve<IUserRegistrationService>();
            await userRegistrationService.ChangePasswordAsync(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword, null, TvProgUserServicesDefaults.DefaultHashedPasswordFormat));

            //search engine (crawler) built-in user
            var searchEngineUser = new User
            {
                Email = "builtin@search_engine_record.com",
                UserGuid = Guid.NewGuid(),
                AdminComment = "Built-in system guest record used for requests from search engines.",
                Active = true,
                IsSystemAccount = true,
                SystemName = TvProgUserDefaults.SearchEngineUserName,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };

            await InsertInstallationDataAsync(searchEngineUser);

            await InsertInstallationDataAsync(new UserUserRoleMapping { UserRoleId = crGuests.Id, UserId = searchEngineUser.Id });

            //built-in user for background tasks
            var backgroundTaskUser = new User
            {
                Email = "builtin@background-task-record.com",
                UserGuid = Guid.NewGuid(),
                AdminComment = "Built-in system record used for background tasks.",
                Active = true,
                IsSystemAccount = true,
                SystemName = TvProgUserDefaults.BackgroundTaskUserName,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };

            await InsertInstallationDataAsync(backgroundTaskUser);

            await InsertInstallationDataAsync(new UserUserRoleMapping { UserId = backgroundTaskUser.Id, UserRoleId = crGuests.Id });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallOrdersAsync()
        {
            static Address cloneAddress(Address address)
            {
                var addr = new Address
                {
                    FirstName = address.FirstName,
                    LastName = address.LastName,
                    Email = address.Email,
                    Company = address.Company,
                    CountryId = address.CountryId,
                    StateProvinceId = address.StateProvinceId,
                    County = address.County,
                    City = address.City,
                    Address1 = address.Address1,
                    Address2 = address.Address2,
                    ZipPostalCode = address.ZipPostalCode,
                    PhoneNumber = address.PhoneNumber,
                    FaxNumber = address.FaxNumber,
                    CustomAttributes = address.CustomAttributes,
                    CreatedOnUtc = address.CreatedOnUtc
                };

                return addr;
            }

            //default store
            var defaultStore = await _storeRepository.Table.FirstOrDefaultAsync();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            //first order
            var firstUser = await _userRepository.Table.FirstAsync(c => c.Email == "steve_gates@tvprogviewer.ru");

            var firstUserBillingAddress = await InsertInstallationDataAsync(cloneAddress(await _addressRepository.GetByIdAsync(firstUser.BillingAddressId)));
            var firstUserShippingAddress = await InsertInstallationDataAsync(cloneAddress(await _addressRepository.GetByIdAsync(firstUser.ShippingAddressId)));

            var firstOrder = new Order
            {
                StoreId = defaultStore.Id,
                OrderGuid = Guid.NewGuid(),
                UserId = firstUser.Id,
                UserLanguageId = _languageRepository.Table.First().Id,
                UserIp = "127.0.0.1",
                OrderSubtotalInclTax = 1855M,
                OrderSubtotalExclTax = 1855M,
                OrderSubTotalDiscountInclTax = decimal.Zero,
                OrderSubTotalDiscountExclTax = decimal.Zero,
                OrderShippingInclTax = decimal.Zero,
                OrderShippingExclTax = decimal.Zero,
                PaymentMethodAdditionalFeeInclTax = decimal.Zero,
                PaymentMethodAdditionalFeeExclTax = decimal.Zero,
                TaxRates = "0:0;",
                OrderTax = decimal.Zero,
                OrderTotal = 1855M,
                RefundedAmount = decimal.Zero,
                OrderDiscount = decimal.Zero,
                CheckoutAttributeDescription = string.Empty,
                CheckoutAttributesXml = string.Empty,
                UserCurrencyCode = "USD",
                CurrencyRate = 1M,
                AffiliateId = 0,
                OrderStatus = OrderStatus.Processing,
                AllowStoringCreditCardNumber = false,
                CardType = string.Empty,
                CardName = string.Empty,
                CardNumber = string.Empty,
                MaskedCreditCardNumber = string.Empty,
                CardCvv2 = string.Empty,
                CardExpirationMonth = string.Empty,
                CardExpirationYear = string.Empty,
                PaymentMethodSystemName = "Payments.CheckMoneyOrder",
                AuthorizationTransactionId = string.Empty,
                AuthorizationTransactionCode = string.Empty,
                AuthorizationTransactionResult = string.Empty,
                CaptureTransactionId = string.Empty,
                CaptureTransactionResult = string.Empty,
                SubscriptionTransactionId = string.Empty,
                PaymentStatus = PaymentStatus.Paid,
                PaidDateUtc = DateTime.UtcNow,
                BillingAddressId = firstUserBillingAddress.Id,
                ShippingAddressId = firstUserShippingAddress.Id,
                ShippingStatus = ShippingStatus.NotYetShipped,
                ShippingMethod = "Ground",
                PickupInStore = false,
                ShippingRateComputationMethodSystemName = "Shipping.FixedByWeightByTotal",
                CustomValuesXml = string.Empty,
                VatNumber = string.Empty,
                CreatedOnUtc = DateTime.UtcNow,
                CustomOrderNumber = string.Empty
            };

            await InsertInstallationDataAsync(firstOrder);
            firstOrder.CustomOrderNumber = firstOrder.Id.ToString();
            await UpdateInstallationDataAsync(firstOrder);

            //item Apple iCam
            var firstOrderItem1 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = firstOrder.Id,
                TvChannelId = _tvChannelRepository.Table.First(p => p.Name == "Apple iCam").Id,
                UnitPriceInclTax = 1300M,
                UnitPriceExclTax = 1300M,
                PriceInclTax = 1300M,
                PriceExclTax = 1300M,
                OriginalTvChannelCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };

            await InsertInstallationDataAsync(firstOrderItem1);

            //item Leica T Mirrorless Digital Camera
            var firstOrderItem2 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = firstOrder.Id,
                TvChannelId = _tvChannelRepository.Table.First(p => p.Name == "Leica T Mirrorless Digital Camera").Id,
                UnitPriceInclTax = 530M,
                UnitPriceExclTax = 530M,
                PriceInclTax = 530M,
                PriceExclTax = 530M,
                OriginalTvChannelCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };

            await InsertInstallationDataAsync(firstOrderItem2);

            //item $25 Virtual Gift Card
            var firstOrderItem3 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = firstOrder.Id,
                TvChannelId = _tvChannelRepository.Table.First(p => p.Name == "$25 Virtual Gift Card").Id,
                UnitPriceInclTax = 25M,
                UnitPriceExclTax = 25M,
                PriceInclTax = 25M,
                PriceExclTax = 25M,
                OriginalTvChannelCost = decimal.Zero,
                AttributeDescription = "From: Steve Gates &lt;steve_gates@tvprogviewer.ru&gt;<br />For: Brenda Lindgren &lt;brenda_lindgren@tvprogviewer.ru&gt;",
                AttributesXml = "<Attributes><GiftCardInfo><RecipientName>Brenda Lindgren</RecipientName><RecipientEmail>brenda_lindgren@tvprogviewer.ru</RecipientEmail><SenderName>Steve Gates</SenderName><SenderEmail>steve_gates@gmail.com</SenderEmail><Message></Message></GiftCardInfo></Attributes>",
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };

            await InsertInstallationDataAsync(firstOrderItem3);

            var firstOrderGiftcard = new GiftCard
            {
                GiftCardType = GiftCardType.Virtual,
                PurchasedWithOrderItemId = firstOrderItem3.Id,
                Amount = 25M,
                IsGiftCardActivated = false,
                GiftCardCouponCode = string.Empty,
                RecipientName = "Brenda Lindgren",
                RecipientEmail = "brenda_lindgren@tvprogviewer.ru",
                SenderName = "Steve Gates",
                SenderEmail = "steve_gates@tvprogviewer.ru",
                Message = string.Empty,
                IsRecipientNotified = false,
                CreatedOnUtc = DateTime.UtcNow
            };

            await InsertInstallationDataAsync(firstOrderGiftcard);

            //order notes
            await InsertInstallationDataAsync(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order placed",
                OrderId = firstOrder.Id
            });

            await InsertInstallationDataAsync(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order paid",
                OrderId = firstOrder.Id
            });

            //second order
            var secondUser = await _userRepository.Table.FirstAsync(c => c.Email == "arthur_holmes@tvprogviewer.ru");

            var secondUserBillingAddress = await InsertInstallationDataAsync(cloneAddress(await _addressRepository.GetByIdAsync(secondUser.BillingAddressId)));
            var secondUserShippingAddress = await InsertInstallationDataAsync(cloneAddress(await _addressRepository.GetByIdAsync(secondUser.ShippingAddressId)));

            var secondOrder = new Order
            {
                StoreId = defaultStore.Id,
                OrderGuid = Guid.NewGuid(),
                UserId = secondUser.Id,
                UserLanguageId = _languageRepository.Table.First().Id,
                UserIp = "127.0.0.1",
                OrderSubtotalInclTax = 2460M,
                OrderSubtotalExclTax = 2460M,
                OrderSubTotalDiscountInclTax = decimal.Zero,
                OrderSubTotalDiscountExclTax = decimal.Zero,
                OrderShippingInclTax = decimal.Zero,
                OrderShippingExclTax = decimal.Zero,
                PaymentMethodAdditionalFeeInclTax = decimal.Zero,
                PaymentMethodAdditionalFeeExclTax = decimal.Zero,
                TaxRates = "0:0;",
                OrderTax = decimal.Zero,
                OrderTotal = 2460M,
                RefundedAmount = decimal.Zero,
                OrderDiscount = decimal.Zero,
                CheckoutAttributeDescription = string.Empty,
                CheckoutAttributesXml = string.Empty,
                UserCurrencyCode = "USD",
                CurrencyRate = 1M,
                AffiliateId = 0,
                OrderStatus = OrderStatus.Pending,
                AllowStoringCreditCardNumber = false,
                CardType = string.Empty,
                CardName = string.Empty,
                CardNumber = string.Empty,
                MaskedCreditCardNumber = string.Empty,
                CardCvv2 = string.Empty,
                CardExpirationMonth = string.Empty,
                CardExpirationYear = string.Empty,
                PaymentMethodSystemName = "Payments.CheckMoneyOrder",
                AuthorizationTransactionId = string.Empty,
                AuthorizationTransactionCode = string.Empty,
                AuthorizationTransactionResult = string.Empty,
                CaptureTransactionId = string.Empty,
                CaptureTransactionResult = string.Empty,
                SubscriptionTransactionId = string.Empty,
                PaymentStatus = PaymentStatus.Pending,
                PaidDateUtc = null,
                BillingAddressId = secondUserBillingAddress.Id,
                ShippingAddressId = secondUserShippingAddress.Id,
                ShippingStatus = ShippingStatus.NotYetShipped,
                ShippingMethod = "Next Day Air",
                PickupInStore = false,
                ShippingRateComputationMethodSystemName = "Shipping.FixedByWeightByTotal",
                CustomValuesXml = string.Empty,
                VatNumber = string.Empty,
                CreatedOnUtc = DateTime.UtcNow,
                CustomOrderNumber = string.Empty
            };

            await InsertInstallationDataAsync(secondOrder);
            secondOrder.CustomOrderNumber = secondOrder.Id.ToString();
            await UpdateInstallationDataAsync(secondOrder);

            //order notes
            await InsertInstallationDataAsync(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order placed",
                OrderId = secondOrder.Id
            });

            //item Vintage Style Engagement Ring
            var secondOrderItem1 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = secondOrder.Id,
                TvChannelId = _tvChannelRepository.Table.First(p => p.Name == "Vintage Style Engagement Ring").Id,
                UnitPriceInclTax = 2100M,
                UnitPriceExclTax = 2100M,
                PriceInclTax = 2100M,
                PriceExclTax = 2100M,
                OriginalTvChannelCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };

            await InsertInstallationDataAsync(secondOrderItem1);

            //item Flower Girl Bracelet
            var secondOrderItem2 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = secondOrder.Id,
                TvChannelId = _tvChannelRepository.Table.First(p => p.Name == "Flower Girl Bracelet").Id,
                UnitPriceInclTax = 360M,
                UnitPriceExclTax = 360M,
                PriceInclTax = 360M,
                PriceExclTax = 360M,
                OriginalTvChannelCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };

            await InsertInstallationDataAsync(secondOrderItem2);

            //third order
            var thirdUser = await _userRepository.Table.FirstAsync(c => c.Email == "james_pan@tvprogviewer.ru");

            var thirdUserBillingAddress = await InsertInstallationDataAsync(cloneAddress(await _addressRepository.GetByIdAsync(thirdUser.BillingAddressId)));

            var thirdOrder = new Order
            {
                StoreId = defaultStore.Id,
                OrderGuid = Guid.NewGuid(),
                UserId = thirdUser.Id,
                UserLanguageId = (await _languageRepository.Table.FirstAsync()).Id,
                UserIp = "127.0.0.1",
                OrderSubtotalInclTax = 8.80M,
                OrderSubtotalExclTax = 8.80M,
                OrderSubTotalDiscountInclTax = decimal.Zero,
                OrderSubTotalDiscountExclTax = decimal.Zero,
                OrderShippingInclTax = decimal.Zero,
                OrderShippingExclTax = decimal.Zero,
                PaymentMethodAdditionalFeeInclTax = decimal.Zero,
                PaymentMethodAdditionalFeeExclTax = decimal.Zero,
                TaxRates = "0:0;",
                OrderTax = decimal.Zero,
                OrderTotal = 8.80M,
                RefundedAmount = decimal.Zero,
                OrderDiscount = decimal.Zero,
                CheckoutAttributeDescription = string.Empty,
                CheckoutAttributesXml = string.Empty,
                UserCurrencyCode = "USD",
                CurrencyRate = 1M,
                AffiliateId = 0,
                OrderStatus = OrderStatus.Pending,
                AllowStoringCreditCardNumber = false,
                CardType = string.Empty,
                CardName = string.Empty,
                CardNumber = string.Empty,
                MaskedCreditCardNumber = string.Empty,
                CardCvv2 = string.Empty,
                CardExpirationMonth = string.Empty,
                CardExpirationYear = string.Empty,
                PaymentMethodSystemName = "Payments.CheckMoneyOrder",
                AuthorizationTransactionId = string.Empty,
                AuthorizationTransactionCode = string.Empty,
                AuthorizationTransactionResult = string.Empty,
                CaptureTransactionId = string.Empty,
                CaptureTransactionResult = string.Empty,
                SubscriptionTransactionId = string.Empty,
                PaymentStatus = PaymentStatus.Pending,
                PaidDateUtc = null,
                BillingAddressId = thirdUserBillingAddress.Id,
                ShippingStatus = ShippingStatus.ShippingNotRequired,
                ShippingMethod = string.Empty,
                PickupInStore = false,
                ShippingRateComputationMethodSystemName = string.Empty,
                CustomValuesXml = string.Empty,
                VatNumber = string.Empty,
                CreatedOnUtc = DateTime.UtcNow,
                CustomOrderNumber = string.Empty
            };

            await InsertInstallationDataAsync(thirdOrder);
            thirdOrder.CustomOrderNumber = thirdOrder.Id.ToString();
            await UpdateInstallationDataAsync(thirdOrder);

            //order notes
            await InsertInstallationDataAsync(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order placed",
                OrderId = thirdOrder.Id
            });

            //item If You Wait
            var thirdOrderItem1 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = thirdOrder.Id,
                TvChannelId = _tvChannelRepository.Table.First(p => p.Name == "If You Wait (donation)").Id,
                UnitPriceInclTax = 3M,
                UnitPriceExclTax = 3M,
                PriceInclTax = 3M,
                PriceExclTax = 3M,
                OriginalTvChannelCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };

            await InsertInstallationDataAsync(thirdOrderItem1);

            //item Night Visions
            var thirdOrderItem2 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = thirdOrder.Id,
                TvChannelId = _tvChannelRepository.Table.First(p => p.Name == "Night Visions").Id,
                UnitPriceInclTax = 2.8M,
                UnitPriceExclTax = 2.8M,
                PriceInclTax = 2.8M,
                PriceExclTax = 2.8M,
                OriginalTvChannelCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };

            await InsertInstallationDataAsync(thirdOrderItem2);

            //item Science & Faith
            var thirdOrderItem3 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = thirdOrder.Id,
                TvChannelId = _tvChannelRepository.Table.First(p => p.Name == "Science & Faith").Id,
                UnitPriceInclTax = 3M,
                UnitPriceExclTax = 3M,
                PriceInclTax = 3M,
                PriceExclTax = 3M,
                OriginalTvChannelCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };

            await InsertInstallationDataAsync(thirdOrderItem3);

            //fourth order
            var fourthUser = await _userRepository.Table.FirstAsync(c => c.Email == "brenda_lindgren@tvprogviewer.ru");

            var fourthUserBillingAddress = await InsertInstallationDataAsync(cloneAddress(await _addressRepository.GetByIdAsync(fourthUser.BillingAddressId)));
            var fourthUserShippingAddress = await InsertInstallationDataAsync(cloneAddress(await _addressRepository.GetByIdAsync(fourthUser.ShippingAddressId)));
            var fourthUserPickupAddress = await InsertInstallationDataAsync(cloneAddress(await _addressRepository.GetByIdAsync(fourthUser.ShippingAddressId)));

            var fourthOrder = new Order
            {
                StoreId = defaultStore.Id,
                OrderGuid = Guid.NewGuid(),
                UserId = fourthUser.Id,
                UserLanguageId = _languageRepository.Table.First().Id,
                UserIp = "127.0.0.1",
                OrderSubtotalInclTax = 102M,
                OrderSubtotalExclTax = 102M,
                OrderSubTotalDiscountInclTax = decimal.Zero,
                OrderSubTotalDiscountExclTax = decimal.Zero,
                OrderShippingInclTax = decimal.Zero,
                OrderShippingExclTax = decimal.Zero,
                PaymentMethodAdditionalFeeInclTax = decimal.Zero,
                PaymentMethodAdditionalFeeExclTax = decimal.Zero,
                TaxRates = "0:0;",
                OrderTax = decimal.Zero,
                OrderTotal = 102M,
                RefundedAmount = decimal.Zero,
                OrderDiscount = decimal.Zero,
                CheckoutAttributeDescription = string.Empty,
                CheckoutAttributesXml = string.Empty,
                UserCurrencyCode = "USD",
                CurrencyRate = 1M,
                AffiliateId = 0,
                OrderStatus = OrderStatus.Processing,
                AllowStoringCreditCardNumber = false,
                CardType = string.Empty,
                CardName = string.Empty,
                CardNumber = string.Empty,
                MaskedCreditCardNumber = string.Empty,
                CardCvv2 = string.Empty,
                CardExpirationMonth = string.Empty,
                CardExpirationYear = string.Empty,
                PaymentMethodSystemName = "Payments.CheckMoneyOrder",
                AuthorizationTransactionId = string.Empty,
                AuthorizationTransactionCode = string.Empty,
                AuthorizationTransactionResult = string.Empty,
                CaptureTransactionId = string.Empty,
                CaptureTransactionResult = string.Empty,
                SubscriptionTransactionId = string.Empty,
                PaymentStatus = PaymentStatus.Paid,
                PaidDateUtc = DateTime.UtcNow,
                BillingAddressId = fourthUserBillingAddress.Id,
                ShippingAddressId = fourthUserShippingAddress.Id,
                ShippingStatus = ShippingStatus.Shipped,
                ShippingMethod = "Pickup in store",
                PickupInStore = true,
                PickupAddressId = fourthUserPickupAddress.Id,
                ShippingRateComputationMethodSystemName = "Pickup.PickupInStore",
                CustomValuesXml = string.Empty,
                VatNumber = string.Empty,
                CreatedOnUtc = DateTime.UtcNow,
                CustomOrderNumber = string.Empty
            };

            await InsertInstallationDataAsync(fourthOrder);
            fourthOrder.CustomOrderNumber = fourthOrder.Id.ToString();
            await UpdateInstallationDataAsync(fourthOrder);

            //order notes
            await InsertInstallationDataAsync(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order placed",
                OrderId = fourthOrder.Id
            });

            await InsertInstallationDataAsync(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order paid",
                OrderId = fourthOrder.Id
            });

            await InsertInstallationDataAsync(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order shipped",
                OrderId = fourthOrder.Id
            });

            //item Pride and Prejudice
            var fourthOrderItem1 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = fourthOrder.Id,
                TvChannelId = _tvChannelRepository.Table.First(p => p.Name == "Pride and Prejudice").Id,
                UnitPriceInclTax = 24M,
                UnitPriceExclTax = 24M,
                PriceInclTax = 24M,
                PriceExclTax = 24M,
                OriginalTvChannelCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };

            await InsertInstallationDataAsync(fourthOrderItem1);

            //item First Prize Pies
            var fourthOrderItem2 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = fourthOrder.Id,
                TvChannelId = _tvChannelRepository.Table.First(p => p.Name == "First Prize Pies").Id,
                UnitPriceInclTax = 51M,
                UnitPriceExclTax = 51M,
                PriceInclTax = 51M,
                PriceExclTax = 51M,
                OriginalTvChannelCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };

            await InsertInstallationDataAsync(fourthOrderItem2);

            //item Fahrenheit 451 by Ray Bradbury
            var fourthOrderItem3 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = fourthOrder.Id,
                TvChannelId = _tvChannelRepository.Table.First(p => p.Name == "Fahrenheit 451 by Ray Bradbury").Id,
                UnitPriceInclTax = 27M,
                UnitPriceExclTax = 27M,
                PriceInclTax = 27M,
                PriceExclTax = 27M,
                OriginalTvChannelCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };

            await InsertInstallationDataAsync(fourthOrderItem3);

            //shipments
            //shipment 1
            var fourthOrderShipment1 = new Shipment
            {
                OrderId = fourthOrder.Id,
                TrackingNumber = string.Empty,
                TotalWeight = 4M,
                ReadyForPickupDateUtc = DateTime.UtcNow,
                DeliveryDateUtc = DateTime.UtcNow,
                AdminComment = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            };

            await InsertInstallationDataAsync(fourthOrderShipment1);

            var fourthOrderShipment1Item1 = new ShipmentItem
            {
                OrderItemId = fourthOrderItem1.Id,
                Quantity = 1,
                WarehouseId = 0,
                ShipmentId = fourthOrderShipment1.Id
            };

            await InsertInstallationDataAsync(fourthOrderShipment1Item1);

            var fourthOrderShipment1Item2 = new ShipmentItem
            {
                OrderItemId = fourthOrderItem2.Id,
                Quantity = 1,
                WarehouseId = 0,
                ShipmentId = fourthOrderShipment1.Id
            };

            await InsertInstallationDataAsync(fourthOrderShipment1Item2);

            //shipment 2
            var fourthOrderShipment2 = new Shipment
            {
                OrderId = fourthOrder.Id,
                TrackingNumber = string.Empty,
                TotalWeight = 2M,
                ReadyForPickupDateUtc = DateTime.UtcNow,
                DeliveryDateUtc = DateTime.UtcNow,
                AdminComment = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            };

            await InsertInstallationDataAsync(fourthOrderShipment2);

            var fourthOrderShipment2Item1 = new ShipmentItem
            {
                OrderItemId = fourthOrderItem3.Id,
                Quantity = 1,
                WarehouseId = 0,
                ShipmentId = fourthOrderShipment2.Id
            };

            await InsertInstallationDataAsync(fourthOrderShipment2Item1);

            //fifth order
            var fifthUser = _userRepository.Table.First(c => c.Email == "victoria_victoria@tvprogviewer.ru");

            var fifthUserBillingAddress = await InsertInstallationDataAsync(cloneAddress(await _addressRepository.GetByIdAsync(fifthUser.BillingAddressId)));
            var fifthUserShippingAddress = await InsertInstallationDataAsync(cloneAddress(await _addressRepository.GetByIdAsync(fifthUser.ShippingAddressId)));

            var fifthOrder = new Order
            {
                StoreId = defaultStore.Id,
                OrderGuid = Guid.NewGuid(),
                UserId = fifthUser.Id,
                UserLanguageId = _languageRepository.Table.First().Id,
                UserIp = "127.0.0.1",
                OrderSubtotalInclTax = 43.50M,
                OrderSubtotalExclTax = 43.50M,
                OrderSubTotalDiscountInclTax = decimal.Zero,
                OrderSubTotalDiscountExclTax = decimal.Zero,
                OrderShippingInclTax = decimal.Zero,
                OrderShippingExclTax = decimal.Zero,
                PaymentMethodAdditionalFeeInclTax = decimal.Zero,
                PaymentMethodAdditionalFeeExclTax = decimal.Zero,
                TaxRates = "0:0;",
                OrderTax = decimal.Zero,
                OrderTotal = 43.50M,
                RefundedAmount = decimal.Zero,
                OrderDiscount = decimal.Zero,
                CheckoutAttributeDescription = string.Empty,
                CheckoutAttributesXml = string.Empty,
                UserCurrencyCode = "USD",
                CurrencyRate = 1M,
                AffiliateId = 0,
                OrderStatus = OrderStatus.Complete,
                AllowStoringCreditCardNumber = false,
                CardType = string.Empty,
                CardName = string.Empty,
                CardNumber = string.Empty,
                MaskedCreditCardNumber = string.Empty,
                CardCvv2 = string.Empty,
                CardExpirationMonth = string.Empty,
                CardExpirationYear = string.Empty,
                PaymentMethodSystemName = "Payments.CheckMoneyOrder",
                AuthorizationTransactionId = string.Empty,
                AuthorizationTransactionCode = string.Empty,
                AuthorizationTransactionResult = string.Empty,
                CaptureTransactionId = string.Empty,
                CaptureTransactionResult = string.Empty,
                SubscriptionTransactionId = string.Empty,
                PaymentStatus = PaymentStatus.Paid,
                PaidDateUtc = DateTime.UtcNow,
                BillingAddressId = fifthUserBillingAddress.Id,
                ShippingAddressId = fifthUserShippingAddress.Id,
                ShippingStatus = ShippingStatus.Delivered,
                ShippingMethod = "Ground",
                PickupInStore = false,
                ShippingRateComputationMethodSystemName = "Shipping.FixedByWeightByTotal",
                CustomValuesXml = string.Empty,
                VatNumber = string.Empty,
                CreatedOnUtc = DateTime.UtcNow,
                CustomOrderNumber = string.Empty
            };

            await InsertInstallationDataAsync(fifthOrder);
            fifthOrder.CustomOrderNumber = fifthOrder.Id.ToString();
            await UpdateInstallationDataAsync(fifthOrder);

            //order notes
            await InsertInstallationDataAsync(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order placed",
                OrderId = fifthOrder.Id
            });

            await InsertInstallationDataAsync(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order paid",
                OrderId = fifthOrder.Id
            });

            await InsertInstallationDataAsync(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order shipped",
                OrderId = fifthOrder.Id
            });

            await InsertInstallationDataAsync(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                Note = "Order delivered",
                OrderId = fifthOrder.Id
            });

            //item Levi's 511 Jeans
            var fifthOrderItem1 = new OrderItem
            {
                OrderItemGuid = Guid.NewGuid(),
                OrderId = fifthOrder.Id,
                TvChannelId = _tvChannelRepository.Table.First(p => p.Name == "Levi's 511 Jeans").Id,
                UnitPriceInclTax = 43.50M,
                UnitPriceExclTax = 43.50M,
                PriceInclTax = 43.50M,
                PriceExclTax = 43.50M,
                OriginalTvChannelCost = decimal.Zero,
                AttributeDescription = string.Empty,
                AttributesXml = string.Empty,
                Quantity = 1,
                DiscountAmountInclTax = decimal.Zero,
                DiscountAmountExclTax = decimal.Zero,
                DownloadCount = 0,
                IsDownloadActivated = false,
                LicenseDownloadId = 0,
                ItemWeight = null,
                RentalStartDateUtc = null,
                RentalEndDateUtc = null
            };

            await InsertInstallationDataAsync(fifthOrderItem1);

            //shipment 1
            var fifthOrderShipment1 = new Shipment
            {
                OrderId = fifthOrder.Id,
                TrackingNumber = string.Empty,
                TotalWeight = 2M,
                ShippedDateUtc = DateTime.UtcNow,
                DeliveryDateUtc = DateTime.UtcNow,
                AdminComment = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            };

            await InsertInstallationDataAsync(fifthOrderShipment1);

            var fifthOrderShipment1Item1 = new ShipmentItem
            {
                OrderItemId = fifthOrderItem1.Id,
                Quantity = 1,
                WarehouseId = 0,
                ShipmentId = fifthOrderShipment1.Id
            };

            await InsertInstallationDataAsync(fifthOrderShipment1Item1);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallActivityLogAsync(string defaultUserEmail)
        {
            //default user/user
            var defaultUser = _userRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultUser == null)
                throw new Exception("Cannot load default user");

            await InsertInstallationDataAsync(new ActivityLog
            {
                ActivityLogTypeId = _activityLogTypeRepository.Table.FirstOrDefault(alt => alt.SystemKeyword == "EditCategory")?.Id ?? throw new Exception("Cannot load LogType: EditCategory"),
                Comment = "Edited a category ('Computers')",
                CreatedOnUtc = DateTime.UtcNow,
                UserId = defaultUser.Id,
                IpAddress = "127.0.0.1"
            });

            await InsertInstallationDataAsync(new ActivityLog
            {
                ActivityLogTypeId = _activityLogTypeRepository.Table.FirstOrDefault(alt => alt.SystemKeyword == "EditDiscount")?.Id ?? throw new Exception("Cannot load LogType: EditDiscount"),
                Comment = "Edited a discount ('Sample discount with coupon code')",
                CreatedOnUtc = DateTime.UtcNow,
                UserId = defaultUser.Id,
                IpAddress = "127.0.0.1"
            });

            await InsertInstallationDataAsync(new ActivityLog
            {
                ActivityLogTypeId = _activityLogTypeRepository.Table.FirstOrDefault(alt => alt.SystemKeyword == "EditSpecAttribute")?.Id ?? throw new Exception("Cannot load LogType: EditSpecAttribute"),
                Comment = "Edited a specification attribute ('CPU Type')",
                CreatedOnUtc = DateTime.UtcNow,
                UserId = defaultUser.Id,
                IpAddress = "127.0.0.1"
            });

            await InsertInstallationDataAsync(new ActivityLog
            {
                ActivityLogTypeId = _activityLogTypeRepository.Table.FirstOrDefault(alt => alt.SystemKeyword == "AddNewTvChannelAttribute")?.Id ?? throw new Exception("Cannot load LogType: AddNewTvChannelAttribute"),
                Comment = "Added a new tvChannel attribute ('Some attribute')",
                CreatedOnUtc = DateTime.UtcNow,
                UserId = defaultUser.Id,
                IpAddress = "127.0.0.1"
            });

            await InsertInstallationDataAsync(new ActivityLog
            {
                ActivityLogTypeId = _activityLogTypeRepository.Table.FirstOrDefault(alt => alt.SystemKeyword == "DeleteGiftCard")?.Id ?? throw new Exception("Cannot load LogType: DeleteGiftCard"),
                Comment = "Deleted a gift card ('bdbbc0ef-be57')",
                CreatedOnUtc = DateTime.UtcNow,
                UserId = defaultUser.Id,
                IpAddress = "127.0.0.1"
            });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallSearchTermsAsync()
        {
            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            await InsertInstallationDataAsync(new SearchTerm
            {
                Count = 34,
                Keyword = "computer",
                StoreId = defaultStore.Id
            });

            await InsertInstallationDataAsync(new SearchTerm
            {
                Count = 30,
                Keyword = "camera",
                StoreId = defaultStore.Id
            });

            await InsertInstallationDataAsync(new SearchTerm
            {
                Count = 27,
                Keyword = "jewelry",
                StoreId = defaultStore.Id
            });

            await InsertInstallationDataAsync(new SearchTerm
            {
                Count = 26,
                Keyword = "shoes",
                StoreId = defaultStore.Id
            });

            await InsertInstallationDataAsync(new SearchTerm
            {
                Count = 19,
                Keyword = "jeans",
                StoreId = defaultStore.Id
            });

            await InsertInstallationDataAsync(new SearchTerm
            {
                Count = 10,
                Keyword = "gift",
                StoreId = defaultStore.Id
            });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallEmailAccountsAsync()
        {
            var emailAccounts = new List<EmailAccount>
            {
                new EmailAccount
                {
                    Email = "test@mail.com",
                    DisplayName = "Store name",
                    Host = "smtp.mail.com",
                    Port = 25,
                    Username = "123",
                    Password = "123",
                    EnableSsl = false,
                    UseDefaultCredentials = false
                }
            };

            await InsertInstallationDataAsync(emailAccounts);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallMessageTemplatesAsync()
        {
            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");

            var messageTemplates = new List<MessageTemplate>
            {
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.BlogCommentStoreOwnerNotification,
                    Subject = "%Store.Name%. New blog comment.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new blog comment has been created for blog post \"%BlogComment.BlogPostTitle%\".{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.BackInStockNotification,
                    Subject = "%Store.Name%. Back in stock notification",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %User.FullName%,{Environment.NewLine}<br />{Environment.NewLine}TvChannel <a target=\"_blank\" href=\"%BackInStockSubscription.TvChannelUrl%\">%BackInStockSubscription.TvChannelName%</a> is in stock.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserEmailValidationMessage,
                    Subject = "%Store.Name%. Email validation",
                    Body = $"<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}To activate your account <a href=\"%User.AccountActivationURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserEmailRevalidationMessage,
                    Subject = "%Store.Name%. Email validation",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %User.FullName%!{Environment.NewLine}<br />{Environment.NewLine}To validate your new email address <a href=\"%User.EmailRevalidationURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.PrivateMessageNotification,
                    Subject = "%Store.Name%. You have received a new private message",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}You have received a new private message.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserPasswordRecoveryMessage,
                    Subject = "%Store.Name%. Password recovery",
                    Body = $"<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}To change your password <a href=\"%User.PasswordRecoveryURL%\">click here</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserWelcomeMessage,
                    Subject = "Welcome to %Store.Name%",
                    Body = $"We welcome you to <a href=\"%Store.URL%\"> %Store.Name%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}You can now take part in the various services we have to offer you. Some of these services include:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Permanent Cart - Any tvChannels added to your online cart remain there until you remove them, or check them out.{Environment.NewLine}<br />{Environment.NewLine}Address Book - We can now deliver your tvChannels to another address other than yours! This is perfect to send birthday gifts direct to the birthday-person themselves.{Environment.NewLine}<br />{Environment.NewLine}Order History - View your history of purchases that you have made with us.{Environment.NewLine}<br />{Environment.NewLine}TvChannels Reviews - Share your opinions on tvChannels with our other users.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}For help with any of our online services, please email the store-owner: <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Note: This email address was provided on our registration page. If you own the email and did not register on our site, please send an email to <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewForumPostMessage,
                    Subject = "%Store.Name%. New Post Notification.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new post has been created in the topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Post author: %Forums.PostAuthor%{Environment.NewLine}<br />{Environment.NewLine}Post body: %Forums.PostBody%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewForumTopicMessage,
                    Subject = "%Store.Name%. New Topic Notification.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> has been created at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Click <a href=\"%Forums.TopicURL%\">here</a> for more info.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.GiftCardNotification,
                    Subject = "%GiftCard.SenderName% has sent you a gift card for %Store.Name%",
                    Body = $"<p>{Environment.NewLine}You have received a gift card for %Store.Name%{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}Dear %GiftCard.RecipientName%,{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%GiftCard.SenderName% (%GiftCard.SenderEmail%) has sent you a %GiftCard.Amount% gift card for <a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}Your gift card code is %GiftCard.CouponCode%{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}%GiftCard.Message%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.UserRegisteredStoreOwnerNotification,
                    Subject = "%Store.Name%. New user registration",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new user registered with your store. Below are the user's details:{Environment.NewLine}<br />{Environment.NewLine}Full name: %User.FullName%{Environment.NewLine}<br />{Environment.NewLine}Email: %User.Email%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewReturnRequestStoreOwnerNotification,
                    Subject = "%Store.Name%. New return request.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%User.FullName% has just submitted a new return request. Details are below:{Environment.NewLine}<br />{Environment.NewLine}Request ID: %ReturnRequest.CustomNumber%{Environment.NewLine}<br />{Environment.NewLine}TvChannel: %ReturnRequest.TvChannel.Quantity% x TvChannel: %ReturnRequest.TvChannel.Name%{Environment.NewLine}<br />{Environment.NewLine}Reason for return: %ReturnRequest.Reason%{Environment.NewLine}<br />{Environment.NewLine}Requested action: %ReturnRequest.RequestedAction%{Environment.NewLine}<br />{Environment.NewLine}User comments:{Environment.NewLine}<br />{Environment.NewLine}%ReturnRequest.UserComment%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewReturnRequestUserNotification,
                    Subject = "%Store.Name%. New return request.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %User.FullName%!{Environment.NewLine}<br />{Environment.NewLine}You have just submitted a new return request. Details are below:{Environment.NewLine}<br />{Environment.NewLine}Request ID: %ReturnRequest.CustomNumber%{Environment.NewLine}<br />{Environment.NewLine}TvChannel: %ReturnRequest.TvChannel.Quantity% x TvChannel: %ReturnRequest.TvChannel.Name%{Environment.NewLine}<br />{Environment.NewLine}Reason for return: %ReturnRequest.Reason%{Environment.NewLine}<br />{Environment.NewLine}Requested action: %ReturnRequest.RequestedAction%{Environment.NewLine}<br />{Environment.NewLine}User comments:{Environment.NewLine}<br />{Environment.NewLine}%ReturnRequest.UserComment%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsCommentStoreOwnerNotification,
                    Subject = "%Store.Name%. New news comment.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new news comment has been created for news \"%NewsComment.NewsTitle%\".{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage,
                    Subject = "%Store.Name%. Subscription activation message.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%NewsLetterSubscription.ActivationUrl%\">Click here to confirm your subscription to our list.</a>{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}If you received this email by mistake, simply delete it.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage,
                    Subject = "%Store.Name%. Subscription deactivation message.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%NewsLetterSubscription.DeactivationUrl%\">Click here to unsubscribe from our newsletter.</a>{Environment.NewLine}</p>{Environment.NewLine}<p>{Environment.NewLine}If you received this email by mistake, simply delete it.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewVatSubmittedStoreOwnerNotification,
                    Subject = "%Store.Name%. New VAT number is submitted.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%User.FullName% (%User.Email%) has just submitted a new VAT number. Details are below:{Environment.NewLine}<br />{Environment.NewLine}VAT number: %User.VatNumber%{Environment.NewLine}<br />{Environment.NewLine}VAT number status: %User.VatNumberStatus%{Environment.NewLine}<br />{Environment.NewLine}Received name: %VatValidationResult.Name%{Environment.NewLine}<br />{Environment.NewLine}Received address: %VatValidationResult.Address%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderCancelledUserNotification,
                    Subject = "%Store.Name%. Your order cancelled",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.UserFullName%,{Environment.NewLine}<br />{Environment.NewLine}Your order has been cancelled. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForUser%\">%Order.OrderURLForUser%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingLastName% %Order.BillingFirstName% %Order.BillingMiddleName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderProcessingUserNotification,
                    Subject = "%Store.Name%. Your order is processing",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.UserFullName%,{Environment.NewLine}<br />{Environment.NewLine}Your order is processing. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForUser%\">%Order.OrderURLForUser%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingLastName% %Order.BillingFirstName% %Order.BillingMiddleName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderCompletedUserNotification,
                    Subject = "%Store.Name%. Your order completed",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.UserFullName%,{Environment.NewLine}<br />{Environment.NewLine}Your order has been completed. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForUser%\">%Order.OrderURLForUser%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingLastName% %Order.BillingFirstName% %Order.BillingMiddleName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ShipmentDeliveredUserNotification,
                    Subject = "Your order from %Store.Name% has been %if (!%Order.IsCompletelyDelivered%) partially endif%delivered.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.UserFullName%,{Environment.NewLine}<br />{Environment.NewLine}Good news! You order has been %if (!%Order.IsCompletelyDelivered%) partially endif%delivered.{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForUser%\" target=\"_blank\">%Order.OrderURLForUser%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingLastName% %Order.BillingFirstName% %Order.BillingMiddleName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% Delivered TvChannels:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Shipment.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPlacedUserNotification,
                    Subject = "Order receipt from %Store.Name%.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.UserFullName%,{Environment.NewLine}<br />{Environment.NewLine}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForUser%\">%Order.OrderURLForUser%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingLastName% %Order.BillingFirstName% %Order.BillingMiddleName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPlacedStoreOwnerNotification,
                    Subject = "%Store.Name%. Purchase Receipt for Order #%Order.OrderNumber%",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.UserFullName% (%Order.UserEmail%) has just placed an order from your store. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingLastName% %Order.BillingFirstName% %Order.BillingMiddleName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ShipmentSentUserNotification,
                    Subject = "Your order from %Store.Name% has been %if (!%Order.IsCompletelyShipped%) partially endif%shipped.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.UserFullName%!,{Environment.NewLine}<br />{Environment.NewLine}Good news! You order has been %if (!%Order.IsCompletelyShipped%) partially endif%shipped.{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForUser%\" target=\"_blank\">%Order.OrderURLForUser%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingLastName% %Order.BillingFirstName% %Order.BillingMiddleName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% Shipped TvChannels:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Shipment.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ShipmentReadyForPickupUserNotification,
                    Subject = "Your order from %Store.Name% has been %if (!%Order.IsCompletelyReadyForPickup%) partially endif%ready for pickup.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.UserFullName%!,{Environment.NewLine}<br />{Environment.NewLine}Good news! You order has been %if (!%Order.IsCompletelyReadyForPickup%) partially endif%ready for pickup.{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForUser%\" target=\"_blank\">%Order.OrderURLForUser%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingLastName% %Order.BillingFirstName% %Order.BillingMiddleName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% TvChannels ready for pickup:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Shipment.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.TvChannelReviewStoreOwnerNotification,
                    Subject = "%Store.Name%. New tvChannel review.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}A new tvChannel review has been written for tvChannel \"%TvChannelReview.TvChannelName%\".{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.TvChannelReviewReplyUserNotification,
                    Subject = "%Store.Name%. TvChannel review reply.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %User.FullName%,{Environment.NewLine}<br />{Environment.NewLine}You received a reply from the store administration to your review for tvChannel \"%TvChannelReview.TvChannelName%\".{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.QuantityBelowStoreOwnerNotification,
                    Subject = "%Store.Name%. Quantity below notification. %TvChannel.Name%",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%TvChannel.Name% (ID: %TvChannel.ID%) low quantity.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Quantity: %TvChannel.StockQuantity%{Environment.NewLine}<br />{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.QuantityBelowAttributeCombinationStoreOwnerNotification,
                    Subject = "%Store.Name%. Quantity below notification. %TvChannel.Name%",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%TvChannel.Name% (ID: %TvChannel.ID%) low quantity.{Environment.NewLine}<br />{Environment.NewLine}%AttributeCombination.Formatted%{Environment.NewLine}<br />{Environment.NewLine}Quantity: %AttributeCombination.StockQuantity%{Environment.NewLine}<br />{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ReturnRequestStatusChangedUserNotification,
                    Subject = "%Store.Name%. Return request status was changed.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %User.FullName%,{Environment.NewLine}<br />{Environment.NewLine}Your return request #%ReturnRequest.CustomNumber% status has been changed.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.EmailAFriendMessage,
                    Subject = "%Store.Name%. Referred Item",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%EmailAFriend.Email% was shopping on %Store.Name% and wanted to share the following item with you.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<b><a target=\"_blank\" href=\"%TvChannel.TvChannelURLForUser%\">%TvChannel.Name%</a></b>{Environment.NewLine}<br />{Environment.NewLine}%TvChannel.ShortDescription%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}For more info click <a target=\"_blank\" href=\"%TvChannel.TvChannelURLForUser%\">here</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%EmailAFriend.PersonalMessage%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.WishlistToFriendMessage,
                    Subject = "%Store.Name%. Wishlist",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Wishlist.Email% was shopping on %Store.Name% and wanted to share a wishlist with you.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}For more info click <a target=\"_blank\" href=\"%Wishlist.URLForUser%\">here</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Wishlist.PersonalMessage%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Store.Name%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewOrderNoteAddedUserNotification,
                    Subject = "%Store.Name%. New order note has been added",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %User.FullName%,{Environment.NewLine}<br />{Environment.NewLine}New order note has been added to your account:{Environment.NewLine}<br />{Environment.NewLine}\"%Order.NewNoteText%\".{Environment.NewLine}<br />{Environment.NewLine}<a target=\"_blank\" href=\"%Order.OrderURLForUser%\">%Order.OrderURLForUser%</a>{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.RecurringPaymentCancelledStoreOwnerNotification,
                    Subject = "%Store.Name%. Recurring payment cancelled",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%RecurringPayment.CancelAfterFailedPayment%) The last payment for the recurring payment ID=%RecurringPayment.ID% failed, so it was cancelled. endif% %if (!%RecurringPayment.CancelAfterFailedPayment%) %User.FullName% (%User.Email%) has just cancelled a recurring payment ID=%RecurringPayment.ID%. endif%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.RecurringPaymentCancelledUserNotification,
                    Subject = "%Store.Name%. Recurring payment cancelled",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %User.FullName%,{Environment.NewLine}<br />{Environment.NewLine}%if (%RecurringPayment.CancelAfterFailedPayment%) It appears your credit card didn't go through for this recurring payment (<a href=\"%Order.OrderURLForUser%\" target=\"_blank\">%Order.OrderURLForUser%</a>){Environment.NewLine}<br />{Environment.NewLine}So your subscription has been cancelled. endif% %if (!%RecurringPayment.CancelAfterFailedPayment%) The recurring payment ID=%RecurringPayment.ID% was cancelled. endif%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.RecurringPaymentFailedUserNotification,
                    Subject = "%Store.Name%. Last recurring payment failed",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %User.FullName%,{Environment.NewLine}<br />{Environment.NewLine}It appears your credit card didn't go through for this recurring payment (<a href=\"%Order.OrderURLForUser%\" target=\"_blank\">%Order.OrderURLForUser%</a>){Environment.NewLine}<br /> %if (%RecurringPayment.RecurringPaymentType% == \"Manual\") {Environment.NewLine}You can recharge balance and manually retry payment or cancel it on the order history page. endif% %if (%RecurringPayment.RecurringPaymentType% == \"Automatic\") {Environment.NewLine}You can recharge balance and wait, we will try to make the payment again, or you can cancel it on the order history page. endif%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPlacedVendorNotification,
                    Subject = "%Store.Name%. Order placed",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%User.FullName% (%User.Email%) has just placed an order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPlacedAffiliateNotification,
                    Subject = "%Store.Name%. Order placed",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%User.FullName% (%User.Email%) has just placed an order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderRefundedUserNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% refunded",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.UserFullName%,{Environment.NewLine}<br />{Environment.NewLine}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Order #%Order.OrderNumber% has been has been refunded. Please allow 7-14 days for the refund to be reflected in your account.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Amount refunded: %Order.AmountRefunded%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForUser%\" target=\"_blank\">%Order.OrderURLForUser%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br /{Environment.NewLine}>Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderRefundedStoreOwnerNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% refunded",
                    Body = $"%Store.Name%. Order #%Order.OrderNumber% refunded', N'{Environment.NewLine}<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been just refunded{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Amount refunded: %Order.AmountRefunded%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPaidStoreOwnerNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been just paid{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPaidUserNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.UserFullName%,{Environment.NewLine}<br />{Environment.NewLine}Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Order #%Order.OrderNumber% has been just paid. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForUser%\" target=\"_blank\">%Order.OrderURLForUser%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingLastName% %Order.BillingFirstName% %Order.BillingMiddleName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPaidVendorNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been just paid.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.OrderPaidAffiliateNotification,
                    Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been just paid.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                    //this template is disabled by default
                    IsActive = false,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.NewVendorAccountApplyStoreOwnerNotification,
                    Subject = "%Store.Name%. New vendor account submitted.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%User.FullName% (%User.Email%) has just submitted for a vendor account. Details are below:{Environment.NewLine}<br />{Environment.NewLine}Vendor name: %Vendor.Name%{Environment.NewLine}<br />{Environment.NewLine}Vendor email: %Vendor.Email%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}You can activate it in admin area.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.VendorInformationChangeStoreOwnerNotification,
                    Subject = "%Store.Name%. Vendor information change.",
                    Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Vendor %Vendor.Name% (%Vendor.Email%) has just changed information about itself.{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ContactUsMessage,
                    Subject = "%Store.Name%. Contact us",
                    Body = $"<p>{Environment.NewLine}%ContactUs.Body%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                },
                new MessageTemplate
                {
                    Name = MessageTemplateSystemNames.ContactVendorMessage,
                    Subject = "%Store.Name%. Contact us",
                    Body = $"<p>{Environment.NewLine}%ContactUs.Body%{Environment.NewLine}</p>{Environment.NewLine}",
                    IsActive = true,
                    EmailAccountId = eaGeneral.Id
                }
            };

            await InsertInstallationDataAsync(messageTemplates);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallTopicsAsync()
        {
            var defaultTopicTemplate =
                _topicTemplateRepository.Table.FirstOrDefault(tt => tt.Name == "Default template");
            if (defaultTopicTemplate == null)
                throw new Exception("Topic template cannot be loaded");

            var topics = new List<Topic>
            {
                new Topic
                {
                    SystemName = "AboutUs",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    IncludeInFooterColumn1 = true,
                    DisplayOrder = 20,
                    Published = true,
                    Title = "About us",
                    Body =
                        "<p>Put your &quot;About Us&quot; information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "CheckoutAsGuestOrRegister",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = string.Empty,
                    Body =
                        "<p><strong>Register and save time!</strong><br />Register with us for future convenience:</p><ul><li>Fast and easy check out</li><li>Easy access to your order history and status</li></ul>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "ConditionsOfUse",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    IncludeInFooterColumn1 = true,
                    DisplayOrder = 15,
                    Published = true,
                    Title = "Conditions of Use",
                    Body = "<p>Put your conditions of use information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "ContactUs",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = string.Empty,
                    Body = "<p>Put your contact information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "ForumWelcomeMessage",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = "Forums",
                    Body = "<p>Put your welcome message here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "HomepageText",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = "Welcome to our store",
                    Body =
                        "<p>Online shopping is the process consumers go through to purchase tvChannels or services over the Internet. You can edit this in the admin site.</p><p>If you have questions, see the <a href=\"http://docs.tvprogviewer.ru/\">Documentation</a>, or post in the <a href=\"https://tvprogviewer.ru/boards/\">Forums</a> at <a href=\"https://tvprogviewer.ru\">tvprogviewer.ru</a></p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "LoginRegistrationInfo",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = "About login / registration",
                    Body =
                        "<p>Put your login / registration information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "PrivacyInfo",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    IncludeInFooterColumn1 = true,
                    DisplayOrder = 10,
                    Published = true,
                    Title = "Privacy notice",
                    Body = "<p>Put your privacy policy information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "PageNotFound",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = string.Empty,
                    Body =
                        "<p><strong>The page you requested was not found, and we have a fine guess why.</strong></p><ul><li>If you typed the URL directly, please make sure the spelling is correct.</li><li>The page no longer exists. In this case, we profusely apologize for the inconvenience and for any damage this may cause.</li></ul>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "ShippingInfo",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    IncludeInFooterColumn1 = true,
                    DisplayOrder = 5,
                    Published = true,
                    Title = "Shipping & returns",
                    Body =
                        "<p>Put your shipping &amp; returns information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "ApplyVendor",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = string.Empty,
                    Body = "<p>Put your apply vendor instructions here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                },
                new Topic
                {
                    SystemName = "VendorTermsOfService",
                    IncludeInSitemap = false,
                    IsPasswordProtected = false,
                    IncludeInFooterColumn1 = false,
                    DisplayOrder = 1,
                    Published = true,
                    Title = "Terms of services for vendors",
                    Body = "<p>Put your terms of service information here. You can edit this in the admin site.</p>",
                    TopicTemplateId = defaultTopicTemplate.Id
                }
            };

            await InsertInstallationDataAsync(topics);

            //search engine names
            foreach (var topic in topics)
            {
                await InsertInstallationDataAsync(new UrlRecord
                {
                    EntityId = topic.Id,
                    EntityName = nameof(Topic),
                    LanguageId = 0,
                    IsActive = true,
                    Slug = await ValidateSeNameAsync(topic, !string.IsNullOrEmpty(topic.Title) ? topic.Title : topic.SystemName)
                });
            }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallSettingsAsync(RegionInfo regionInfo)
        {
            var isMetric = regionInfo?.IsMetric ?? false;
            var country = regionInfo?.TwoLetterISORegionName ?? string.Empty;
            var isGermany = country == "DE";
            var isEurope = ISO3166.FromCountryCode(country)?.SubjectToVat ?? false;

            var settingService = EngineContext.Current.Resolve<ISettingService>();
            await settingService.SaveSettingAsync(new PdfSettings
            {
                LogoPictureId = 0,
                LetterPageSizeEnabled = false,
                RenderOrderNotes = true,
                FontFamily = "FreeSerif",
                InvoiceFooterTextColumn1 = null,
                InvoiceFooterTextColumn2 = null
            });

            await settingService.SaveSettingAsync(new SitemapSettings
            {
                SitemapEnabled = true,
                SitemapPageSize = 200,
                SitemapIncludeCategories = true,
                SitemapIncludeManufacturers = true,
                SitemapIncludeTvChannels = false,
                SitemapIncludeTvChannelTags = false,
                SitemapIncludeBlogPosts = true,
                SitemapIncludeNews = false,
                SitemapIncludeTopics = true
            });

            await settingService.SaveSettingAsync(new SitemapXmlSettings
            {
                SitemapXmlEnabled = true,
                SitemapXmlIncludeBlogPosts = true,
                SitemapXmlIncludeCategories = true,
                SitemapXmlIncludeManufacturers = true,
                SitemapXmlIncludeNews = true,
                SitemapXmlIncludeTvChannels = true,
                SitemapXmlIncludeTvChannelTags = true,
                SitemapXmlIncludeCustomUrls = true,
                SitemapXmlIncludeTopics = true,
                RebuildSitemapXmlAfterHours = 2 * 24,
                SitemapBuildOperationDelay = 60
            });

            await settingService.SaveSettingAsync(new CommonSettings
            {
                UseSystemEmailForContactUsForm = true,
                DisplayJavaScriptDisabledWarning = false,
                Log404Errors = true,
                BreadcrumbDelimiter = "/",
                BbcodeEditorOpenLinksInNewWindow = false,
                PopupForTermsOfServiceLinks = true,
                JqueryMigrateScriptLoggingActive = false,
                UseResponseCompression = true,
                FaviconAndAppIconsHeadCode =
                    "<link rel=\"apple-touch-icon\" sizes=\"180x180\" href=\"/icons/icons_0/apple-touch-icon.png\"><link rel=\"icon\" type=\"image/png\" sizes=\"32x32\" href=\"/icons/icons_0/favicon-32x32.png\"><link rel=\"icon\" type=\"image/png\" sizes=\"192x192\" href=\"/icons/icons_0/android-chrome-192x192.png\"><link rel=\"icon\" type=\"image/png\" sizes=\"16x16\" href=\"/icons/icons_0/favicon-16x16.png\"><link rel=\"manifest\" href=\"/icons/icons_0/site.webmanifest\"><link rel=\"mask-icon\" href=\"/icons/icons_0/safari-pinned-tab.svg\" color=\"#5bbad5\"><link rel=\"shortcut icon\" href=\"/icons/icons_0/favicon.ico\"><meta name=\"msapplication-TileColor\" content=\"#2d89ef\"><meta name=\"msapplication-TileImage\" content=\"/icons/icons_0/mstile-144x144.png\"><meta name=\"msapplication-config\" content=\"/icons/icons_0/browserconfig.xml\"><meta name=\"theme-color\" content=\"#ffffff\">",
                EnableHtmlMinification = true,
                RestartTimeout = TvProgCommonDefaults.RestartTimeout,
                HeaderCustomHtml = string.Empty,
                FooterCustomHtml = string.Empty
            });

            await settingService.SaveSettingAsync(new SeoSettings
            {
                PageTitleSeparator = ". ",
                PageTitleSeoAdjustment = PageTitleSeoAdjustment.PagenameAfterStorename,
                GenerateTvChannelMetaDescription = true,
                ConvertNonWesternChars = false,
                AllowUnicodeCharsInUrls = true,
                CanonicalUrlsEnabled = false,
                QueryStringInCanonicalUrlsEnabled = false,
                WwwRequirement = WwwRequirement.NoMatter,
                TwitterMetaTags = true,
                OpenGraphMetaTags = true,
                MicrodataEnabled = true,
                ReservedUrlRecordSlugs = TvProgSeoDefaults.ReservedUrlRecordSlugs,
                CustomHeadTags = string.Empty
            });

            await settingService.SaveSettingAsync(new AdminAreaSettings
            {
                DefaultGridPageSize = 15,
                PopupGridPageSize = 7,
                GridPageSizes = "7, 15, 20, 50, 100",
                RichEditorAdditionalSettings = null,
                RichEditorAllowJavaScript = false,
                RichEditorAllowStyleTag = false,
                UseRichEditorForUserEmails = false,
                UseRichEditorInMessageTemplates = false,
                CheckLicense = true,
                UseIsoDateFormatInJsonResult = true,
                ShowDocumentationReferenceLinks = true
            });

            await settingService.SaveSettingAsync(new TvChannelEditorSettings
            {
                Weight = true,
                Dimensions = true,
                TvChannelAttributes = true,
                SpecificationAttributes = true,
                PAngV = isGermany
            });

            await settingService.SaveSettingAsync(new GdprSettings
            {
                DeleteInactiveUsersAfterMonths = 36,
                GdprEnabled = false,
                LogPrivacyPolicyConsent = true,
                LogNewsletterConsent = true,
                LogUserProfileChanges = true
            });

            await settingService.SaveSettingAsync(new CatalogSettings
            {
                AllowViewUnpublishedTvChannelPage = true,
                DisplayDiscontinuedMessageForUnpublishedTvChannels = true,
                PublishBackTvChannelWhenCancellingOrders = false,
                ShowSkuOnTvChannelDetailsPage = true,
                ShowSkuOnCatalogPages = false,
                ShowManufacturerPartNumber = false,
                ShowGtin = false,
                ShowFreeShippingNotification = true,
                ShowShortDescriptionOnCatalogPages = false,
                AllowTvChannelSorting = true,
                AllowTvChannelViewModeChanging = true,
                DefaultViewMode = "grid",
                ShowTvChannelsFromSubcategories = false,
                ShowCategoryTvChannelNumber = false,
                ShowCategoryTvChannelNumberIncludingSubcategories = false,
                CategoryBreadcrumbEnabled = true,
                ShowShareButton = true,
                PageShareCode =
                    "<!-- AddThis Button BEGIN --><div class=\"addthis_toolbox addthis_default_style \"><a class=\"addthis_button_preferred_1\"></a><a class=\"addthis_button_preferred_2\"></a><a class=\"addthis_button_preferred_3\"></a><a class=\"addthis_button_preferred_4\"></a><a class=\"addthis_button_compact\"></a><a class=\"addthis_counter addthis_bubble_style\"></a></div><script src=\"http://s7.addthis.com/js/250/addthis_widget.js#pubid=nopsolutions\"></script><!-- AddThis Button END -->",
                TvChannelReviewsMustBeApproved = false,
                OneReviewPerTvChannelFromUser = false,
                DefaultTvChannelRatingValue = 5,
                AllowAnonymousUsersToReviewTvChannel = false,
                TvChannelReviewPossibleOnlyAfterPurchasing = false,
                NotifyStoreOwnerAboutNewTvChannelReviews = false,
                NotifyUserAboutTvChannelReviewReply = false,
                EmailAFriendEnabled = true,
                TvChannelLiveUrlEnabled = true,
                AllowAnonymousUsersToEmailAFriend = false,
                RecentlyViewedTvChannelsNumber = 3,
                RecentlyViewedTvChannelsEnabled = true,
                NewTvChannelsEnabled = true,
                NewTvChannelsPageSize = 6,
                NewTvChannelsAllowUsersToSelectPageSize = true,
                NewTvChannelsPageSizeOptions = "6, 3, 9",
                CompareTvChannelsEnabled = true,
                CompareTvChannelsNumber = 4,
                TvChannelSearchAutoCompleteEnabled = true,
                TvChannelSearchEnabled = true,
                TvChannelSearchAutoCompleteNumberOfTvChannels = 10,
                ShowLinkToAllResultInSearchAutoComplete = false,
                TvChannelSearchTermMinimumLength = 3,
                ShowTvChannelImagesInSearchAutoComplete = false,
                ShowBestsellersOnHomepage = false,
                NumberOfBestsellersOnHomepage = 4,
                SearchPageTvChannelsPerPage = 6,
                SearchPageAllowUsersToSelectPageSize = true,
                SearchPagePageSizeOptions = "6, 3, 9, 18",
                SearchPagePriceRangeFiltering = true,
                SearchPageManuallyPriceRange = true,
                SearchPagePriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom,
                SearchPagePriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo,
                TvChannelsAlsoPurchasedEnabled = true,
                TvChannelsAlsoPurchasedNumber = 4,
                AjaxProcessAttributeChange = true,
                NumberOfTvChannelTags = 15,
                TvChannelsByTagPageSize = 6,
                IncludeShortDescriptionInCompareTvChannels = false,
                IncludeFullDescriptionInCompareTvChannels = false,
                IncludeFeaturedTvChannelsInNormalLists = false,
                UseLinksInRequiredTvChannelWarnings = true,
                DisplayTierPricesWithDiscounts = true,
                IgnoreDiscounts = false,
                IgnoreFeaturedTvChannels = false,
                IgnoreAcl = true,
                IgnoreStoreLimitations = true,
                CacheTvChannelPrices = false,
                TvChannelsByTagAllowUsersToSelectPageSize = true,
                TvChannelsByTagPageSizeOptions = "6, 3, 9, 18",
                TvChannelsByTagPriceRangeFiltering = true,
                TvChannelsByTagManuallyPriceRange = true,
                TvChannelsByTagPriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom,
                TvChannelsByTagPriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo,
                MaximumBackInStockSubscriptions = 200,
                ManufacturersBlockItemsToDisplay = 2,
                DisplayTaxShippingInfoFooter = isGermany,
                DisplayTaxShippingInfoTvChannelDetailsPage = isGermany,
                DisplayTaxShippingInfoTvChannelBoxes = isGermany,
                DisplayTaxShippingInfoShoppingCart = isGermany,
                DisplayTaxShippingInfoWishlist = isGermany,
                DisplayTaxShippingInfoOrderDetailsPage = isGermany,
                DefaultCategoryPageSizeOptions = "6, 3, 9",
                DefaultCategoryPageSize = 6,
                DefaultManufacturerPageSizeOptions = "6, 3, 9",
                DefaultManufacturerPageSize = 6,
                ShowTvChannelReviewsTabOnAccountPage = true,
                TvChannelReviewsPageSizeOnAccountPage = 10,
                TvChannelReviewsSortByCreatedDateAscending = false,
                ExportImportTvChannelAttributes = true,
                ExportImportTvChannelSpecificationAttributes = true,
                ExportImportUseDropdownlistsForAssociatedEntities = true,
                ExportImportTvChannelsCountInOneFile = 500,
                ExportImportSplitTvChannelsFile = false,
                ExportImportRelatedEntitiesByName = true,
                CountDisplayedYearsDatePicker = 1,
                UseAjaxLoadMenu = false,
                UseAjaxCatalogTvChannelsLoading = true,
                EnableManufacturerFiltering = true,
                EnablePriceRangeFiltering = true,
                EnableSpecificationAttributeFiltering = true,
                DisplayFromPrices = false,
                AttributeValueOutOfStockDisplayType = AttributeValueOutOfStockDisplayType.AlwaysDisplay,
                AllowUsersToSearchWithCategoryName = true,
                AllowUsersToSearchWithManufacturerName = true,
                DisplayAllPicturesOnCatalogPages = false,
                TvChannelUrlStructureTypeId = (int)TvChannelUrlStructureType.TvChannel,
                ActiveSearchProviderSystemName = string.Empty
            });

            await settingService.SaveSettingAsync(new LocalizationSettings
            {
                DefaultAdminLanguageId =
                    _languageRepository.Table
                        .Single(l => l.LanguageCulture == TvProgCommonDefaults.DefaultLanguageCulture).Id,
                UseImagesForLanguageSelection = false,
                SeoFriendlyUrlsForLanguagesEnabled = false,
                AutomaticallyDetectLanguage = false,
                LoadAllLocaleRecordsOnStartup = true,
                LoadAllLocalizedPropertiesOnStartup = true,
                LoadAllUrlRecordsOnStartup = false,
                IgnoreRtlPropertyForAdminArea = false
            });

            await settingService.SaveSettingAsync(new UserSettings
            {
                UsernamesEnabled = false,
                CheckUsernameAvailabilityEnabled = false,
                AllowUsersToChangeUsernames = false,
                DefaultPasswordFormat = PasswordFormat.Hashed,
                HashedPasswordFormat = TvProgUserServicesDefaults.DefaultHashedPasswordFormat,
                PasswordMinLength = 6,
                PasswordRequireDigit = false,
                PasswordRequireLowercase = false,
                PasswordRequireNonAlphanumeric = false,
                PasswordRequireUppercase = false,
                UnduplicatedPasswordsNumber = 4,
                PasswordRecoveryLinkDaysValid = 7,
                PasswordLifetime = 90,
                FailedPasswordAllowedAttempts = 0,
                FailedPasswordLockoutMinutes = 30,
                UserRegistrationType = UserRegistrationType.Standard,
                AllowUsersToUploadAvatars = false,
                AvatarMaximumSizeBytes = 20000,
                DefaultAvatarEnabled = true,
                ShowUsersLocation = false,
                ShowUsersJoinDate = false,
                AllowViewingProfiles = false,
                NotifyNewUserRegistration = false,
                HideDownloadableTvChannelsTab = false,
                HideBackInStockSubscriptionsTab = false,
                DownloadableTvChannelsValidateUser = false,
                UserNameFormat = UserNameFormat.ShowFirstName,
                FirstNameEnabled = true,
                FirstNameRequired = true,
                LastNameEnabled = true,
                LastNameRequired = true,
                MiddleNameEnabled = true,
                GenderEnabled = true,
                BirthDateEnabled = true,
                BirthDateRequired = false,
                BirthDateMinimumAge = 18,
                CompanyEnabled = true,
                StreetAddressEnabled = false,
                StreetAddress2Enabled = false,
                ZipPostalCodeEnabled = false,
                CityEnabled = false,
                CountyEnabled = false,
                CountyRequired = false,
                CountryEnabled = false,
                CountryRequired = false,
                StateProvinceEnabled = false,
                StateProvinceRequired = false,
                SmartPhoneEnabled = true,
                FaxEnabled = false,
                AcceptPrivacyPolicyEnabled = false,
                NewsletterEnabled = true,
                NewsletterTickedByDefault = true,
                HideNewsletterBlock = false,
                NewsletterBlockAllowToUnsubscribe = false,
                OnlineUserMinutes = 20,
                StoreLastVisitedPage = false,
                StoreIpAddresses = true,
                LastActivityMinutes = 15,
                SuffixDeletedUsers = false,
                EnteringEmailTwice = false,
                RequireRegistrationForDownloadableTvChannels = false,
                AllowUsersToCheckGiftCardBalance = false,
                DeleteGuestTaskOlderThanMinutes = 1440,
                PhoneNumberValidationEnabled = false,
                PhoneNumberValidationUseRegex = false,
                PhoneNumberValidationRule = "^[0-9]{1,14}?$",
                AcceptPersonalDataAgreementEnabled = true,
                AcceptPersonalDataAgreementRequired = true
                
            });

            await settingService.SaveSettingAsync(new MultiFactorAuthenticationSettings
            {
                ForceMultifactorAuthentication = false
            });

            await settingService.SaveSettingAsync(new AddressSettings
            {
                CompanyEnabled = true,
                StreetAddressEnabled = true,
                StreetAddressRequired = true,
                StreetAddress2Enabled = true,
                ZipPostalCodeEnabled = true,
                ZipPostalCodeRequired = true,
                CityEnabled = true,
                CityRequired = true,
                CountyEnabled = false,
                CountyRequired = false,
                CountryEnabled = true,
                StateProvinceEnabled = true,
                SmartPhoneEnabled = true,
                SmartPhoneRequired = true,
                FaxEnabled = true
            });

            await settingService.SaveSettingAsync(new MediaSettings
            {
                AvatarPictureSize = 120,
                TvChannelThumbPictureSize = 415,
                TvChannelDetailsPictureSize = 550,
                TvChannelThumbPictureSizeOnTvChannelDetailsPage = 100,
                AssociatedTvChannelPictureSize = 220,
                CategoryThumbPictureSize = 450,
                ManufacturerThumbPictureSize = 420,
                VendorThumbPictureSize = 450,
                CartThumbPictureSize = 80,
                OrderThumbPictureSize = 80,
                MiniCartThumbPictureSize = 70,
                AutoCompleteSearchThumbPictureSize = 20,
                ImageSquarePictureSize = 32,
                MaximumImageSize = 1980,
                DefaultPictureZoomEnabled = false,
                AllowSVGUploads = false,
                DefaultImageQuality = 80,
                MultipleThumbDirectories = false,
                ImportTvChannelImagesUsingHash = true,
                AzureCacheControlHeader = string.Empty,
                UseAbsoluteImagePath = true,
                VideoIframeAllow = "fullscreen",
                VideoIframeWidth = 300,
                VideoIframeHeight = 150
            });

            await settingService.SaveSettingAsync(new StoreInformationSettings
            {
                StoreClosed = false,
                DefaultStoreTheme = "DefaultClean",
                AllowUserToSelectTheme = false,
                DisplayEuCookieLawWarning = isEurope,
                FacebookLink = "https://www.facebook.com/tvProgViewer",
                TwitterLink = "https://twitter.com/tvProgViewer",
                YoutubeLink = "https://www.youtube.com/user/tvProgViewer",
                InstagramLink = "https://www.instagram.com/tvprogviewer_official",
                HidePoweredByNopCommerce = false
            });

            await settingService.SaveSettingAsync(new ExternalAuthenticationSettings
            {
                RequireEmailValidation = false,
                LogErrors = false,
                AllowUsersToRemoveAssociations = true
            });

            await settingService.SaveSettingAsync(new RewardPointsSettings
            {
                Enabled = true,
                ExchangeRate = 1,
                PointsForRegistration = 0,
                RegistrationPointsValidity = 30,
                PointsForPurchases_Amount = 10,
                PointsForPurchases_Points = 1,
                MinOrderTotalToAwardPoints = 0,
                MaximumRewardPointsToUsePerOrder = 0,
                MaximumRedeemedRate = 0,
                PurchasesPointsValidity = 45,
                ActivationDelay = 0,
                ActivationDelayPeriodId = 0,
                DisplayHowMuchWillBeEarned = true,
                PointsAccumulatedForAllStores = true,
                PageSize = 10
            });

            var primaryCurrency = "USD";
            await settingService.SaveSettingAsync(new CurrencySettings
            {
                DisplayCurrencyLabel = false,
                PrimaryStoreCurrencyId =
                    _currencyRepository.Table.Single(c => c.CurrencyCode == primaryCurrency).Id,
                PrimaryExchangeRateCurrencyId =
                    _currencyRepository.Table.Single(c => c.CurrencyCode == primaryCurrency).Id,
                ActiveExchangeRateProviderSystemName = "CurrencyExchange.ECB",
                AutoUpdateEnabled = false
            });

            var baseDimension = isMetric ? "meters" : "inches";
            var baseWeight = isMetric ? "kg" : "lb";

            await settingService.SaveSettingAsync(new MeasureSettings
            {
                BaseDimensionId =
                    _measureDimensionRepository.Table.Single(m => m.SystemKeyword == baseDimension).Id,
                BaseWeightId = _measureWeightRepository.Table.Single(m => m.SystemKeyword == baseWeight).Id
            });

            await settingService.SaveSettingAsync(new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false,
                Color1 = "#b9babe",
                Color2 = "#ebecee",
                Color3 = "#dde2e6"
            });

            await settingService.SaveSettingAsync(new ShoppingCartSettings
            {
                DisplayCartAfterAddingTvChannel = false,
                DisplayWishlistAfterAddingTvChannel = false,
                MaximumShoppingCartItems = 1000,
                MaximumWishlistItems = 1000,
                AllowOutOfStockItemsToBeAddedToWishlist = false,
                MoveItemsFromWishlistToCart = true,
                CartsSharedBetweenStores = false,
                ShowTvChannelImagesOnShoppingCart = true,
                ShowTvChannelImagesOnWishList = true,
                ShowDiscountBox = true,
                ShowGiftCardBox = true,
                CrossSellsNumber = 4,
                EmailWishlistEnabled = true,
                AllowAnonymousUsersToEmailWishlist = false,
                MiniShoppingCartEnabled = true,
                ShowTvChannelImagesInMiniShoppingCart = true,
                MiniShoppingCartTvChannelNumber = 5,
                RoundPricesDuringCalculation = true,
                GroupTierPricesForDistinctShoppingCartItems = false,
                AllowCartItemEditing = true,
                RenderAssociatedAttributeValueQuantity = true
            });

            await settingService.SaveSettingAsync(new OrderSettings
            {
                ReturnRequestNumberMask = "{ID}",
                IsReOrderAllowed = true,
                MinOrderSubtotalAmount = 0,
                MinOrderSubtotalAmountIncludingTax = false,
                MinOrderTotalAmount = 0,
                AutoUpdateOrderTotalsOnEditingOrder = false,
                AnonymousCheckoutAllowed = true,
                TermsOfServiceOnShoppingCartPage = true,
                TermsOfServiceOnOrderConfirmPage = false,
                OnePageCheckoutEnabled = true,
                OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab = false,
                DisableBillingAddressCheckoutStep = false,
                DisableOrderCompletedPage = false,
                DisplayPickupInStoreOnShippingMethodPage = false,
                AttachPdfInvoiceToOrderPlacedEmail = false,
                AttachPdfInvoiceToOrderProcessingEmail = false,
                AttachPdfInvoiceToOrderCompletedEmail = false,
                GeneratePdfInvoiceInUserLanguage = true,
                AttachPdfInvoiceToOrderPaidEmail = false,
                ReturnRequestsEnabled = true,
                ReturnRequestsAllowFiles = false,
                ReturnRequestsFileMaximumSize = 2048,
                NumberOfDaysReturnRequestAvailable = 365,
                MinimumOrderPlacementInterval = 30,
                ActivateGiftCardsAfterCompletingOrder = false,
                DeactivateGiftCardsAfterCancellingOrder = false,
                DeactivateGiftCardsAfterDeletingOrder = false,
                CompleteOrderWhenDelivered = true,
                CustomOrderNumberMask = "{ID}",
                ExportWithTvChannels = true,
                AllowAdminsToBuyCallForPriceTvChannels = true,
                ShowTvChannelThumbnailInOrderDetailsPage = true,
                DisplayUserCurrencyOnOrders = false,
                DisplayOrderSummary = true
            });

            await settingService.SaveSettingAsync(new SecuritySettings
            {
                EncryptionKey = CommonHelper.GenerateRandomDigitCode(16),
                AdminAreaAllowedIpAddresses = null,
                HoneypotEnabled = false,
                HoneypotInputName = "hpinput",
                ProgrammesLoadKey = "231c3372-33eb-470e-b081-d9d4e011d80a",
                AllowNonAsciiCharactersInHeaders = true
            });

            await settingService.SaveSettingAsync(new ShippingSettings
            {
                ActiveShippingRateComputationMethodSystemNames = new List<string> { "Shipping.FixedByWeightByTotal" },
                ActivePickupPointProviderSystemNames = new List<string> { "Pickup.PickupInStore" },
                ShipToSameAddress = true,
                AllowPickupInStore = true,
                DisplayPickupPointsOnMap = false,
                IgnoreAdditionalShippingChargeForPickupInStore = true,
                UseWarehouseLocation = false,
                NotifyUserAboutShippingFromMultipleLocations = false,
                FreeShippingOverXEnabled = false,
                FreeShippingOverXValue = decimal.Zero,
                FreeShippingOverXIncludingTax = false,
                EstimateShippingTvChannelPageEnabled = true,
                EstimateShippingCartPageEnabled = true,
                EstimateShippingCityNameEnabled = false,
                DisplayShipmentEventsToUsers = false,
                DisplayShipmentEventsToStoreOwner = false,
                HideShippingTotal = false,
                ReturnValidOptionsIfThereAreAny = true,
                BypassShippingMethodSelectionIfOnlyOne = false,
                UseCubeRootMethod = true,
                ConsiderAssociatedTvChannelsDimensions = true,
                ShipSeparatelyOneItemEach = true,
                RequestDelay = 300,
                ShippingSorting = ShippingSortingEnum.Position,
            });

            await settingService.SaveSettingAsync(new PaymentSettings
            {
                ActivePaymentMethodSystemNames = new List<string> { "Payments.CheckMoneyOrder", "Payments.Manual" },
                AllowRePostingPayments = true,
                BypassPaymentMethodSelectionIfOnlyOne = true,
                ShowPaymentMethodDescriptions = true,
                SkipPaymentInfoStepForRedirectionPaymentMethods = false,
                CancelRecurringPaymentsAfterFailedPayment = false,
                RegenerateOrderGuidInterval = 180
            });

            await settingService.SaveSettingAsync(new TaxSettings
            {
                TaxBasedOn = TaxBasedOn.BillingAddress,
                TaxBasedOnPickupPointAddress = false,
                TaxDisplayType = TaxDisplayType.ExcludingTax,
                ActiveTaxProviderSystemName = "Tax.FixedOrByCountryStateZip",
                DefaultTaxAddressId = 0,
                DisplayTaxSuffix = false,
                DisplayTaxRates = false,
                PricesIncludeTax = false,
                AllowUsersToSelectTaxDisplayType = false,
                ForceTaxExclusionFromOrderSubtotal = false,
                DefaultTaxCategoryId = 0,
                HideZeroTax = false,
                HideTaxInOrderSummary = false,
                ShippingIsTaxable = false,
                ShippingPriceIncludesTax = false,
                ShippingTaxClassId = 0,
                PaymentMethodAdditionalFeeIsTaxable = false,
                PaymentMethodAdditionalFeeIncludesTax = false,
                PaymentMethodAdditionalFeeTaxClassId = 0,
                EuVatEnabled = isEurope,
                EuVatEnabledForGuests = false,
                EuVatShopCountryId =
                    isEurope
                        ? (_countryRepository.Table.FirstOrDefault(x => x.TwoLetterIsoCode == country)?.Id ?? 0)
                        : 0,
                EuVatAllowVatExemption = true,
                EuVatUseWebService = false,
                EuVatAssumeValid = false,
                EuVatEmailAdminWhenNewVatSubmitted = false,
                LogErrors = false
            });

            await settingService.SaveSettingAsync(new DateTimeSettings
            {
                DefaultStoreGmtZone = string.Empty,
                AllowUsersToSetTimeZone = false
            });

            await settingService.SaveSettingAsync(new BlogSettings
            {
                Enabled = true,
                PostsPageSize = 10,
                AllowNotRegisteredUsersToLeaveComments = true,
                NotifyAboutNewBlogComments = false,
                NumberOfTags = 15,
                ShowHeaderRssUrl = false,
                BlogCommentsMustBeApproved = false,
                ShowBlogCommentsPerStore = false
            });
            await settingService.SaveSettingAsync(new NewsSettings
            {
                Enabled = true,
                AllowNotRegisteredUsersToLeaveComments = true,
                NotifyAboutNewNewsComments = false,
                ShowNewsOnMainPage = true,
                MainPageNewsCount = 3,
                NewsArchivePageSize = 10,
                ShowHeaderRssUrl = false,
                NewsCommentsMustBeApproved = false,
                ShowNewsCommentsPerStore = false
            });

            await settingService.SaveSettingAsync(new ForumSettings
            {
                ForumsEnabled = false,
                RelativeDateTimeFormattingEnabled = true,
                AllowUsersToDeletePosts = false,
                AllowUsersToEditPosts = false,
                AllowUsersToManageSubscriptions = false,
                AllowGuestsToCreatePosts = false,
                AllowGuestsToCreateTopics = false,
                AllowPostVoting = true,
                MaxVotesPerDay = 30,
                TopicSubjectMaxLength = 450,
                PostMaxLength = 4000,
                StrippedTopicMaxLength = 45,
                TopicsPageSize = 10,
                PostsPageSize = 10,
                SearchResultsPageSize = 10,
                ActiveDiscussionsPageSize = 50,
                LatestUserPostsPageSize = 10,
                ShowUsersPostCount = true,
                ForumEditor = EditorType.BBCodeEditor,
                SignaturesEnabled = true,
                AllowPrivateMessages = false,
                ShowAlertForPM = false,
                PrivateMessagesPageSize = 10,
                ForumSubscriptionsPageSize = 10,
                NotifyAboutPrivateMessages = false,
                PMSubjectMaxLength = 450,
                PMTextMaxLength = 4000,
                HomepageActiveDiscussionsTopicCount = 5,
                ActiveDiscussionsFeedEnabled = false,
                ActiveDiscussionsFeedCount = 25,
                ForumFeedsEnabled = false,
                ForumFeedCount = 10,
                ForumSearchTermMinimumLength = 3
            });

            await settingService.SaveSettingAsync(new VendorSettings
            {
                DefaultVendorPageSizeOptions = "6, 3, 9",
                VendorsBlockItemsToDisplay = 0,
                ShowVendorOnTvChannelDetailsPage = true,
                AllowUsersToContactVendors = true,
                AllowUsersToApplyForVendorAccount = true,
                TermsOfServiceEnabled = false,
                AllowVendorsToEditInfo = false,
                NotifyStoreOwnerAboutVendorInformationChange = true,
                MaximumTvChannelNumber = 3000,
                AllowVendorsToImportTvChannels = true
            });

            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");
            await settingService.SaveSettingAsync(new EmailAccountSettings { DefaultEmailAccountId = eaGeneral.Id });

            await settingService.SaveSettingAsync(new WidgetSettings
            {
                ActiveWidgetSystemNames = new List<string> { "Widgets.NivoSlider" }
            });

            await settingService.SaveSettingAsync(new DisplayDefaultMenuItemSettings
            {
                DisplayHomepageMenuItem = true,
                DisplayNewTvChannelsMenuItem = true,
                DisplayTvChannelSearchMenuItem = true,
                DisplayUserInfoMenuItem = true,
                DisplayBlogMenuItem = true,
                DisplayForumsMenuItem = true,
                DisplayContactUsMenuItem = true
            });

            await settingService.SaveSettingAsync(new DisplayDefaultFooterItemSettings
            {
                DisplaySitemapFooterItem = true,
                DisplayContactUsFooterItem = true,
                DisplayTvChannelSearchFooterItem = true,
                DisplayNewsFooterItem = true,
                DisplayBlogFooterItem = true,
                DisplayForumsFooterItem = true,
                DisplayRecentlyViewedTvChannelsFooterItem = true,
                DisplayCompareTvChannelsFooterItem = true,
                DisplayNewTvChannelsFooterItem = true,
                DisplayUserInfoFooterItem = true,
                DisplayUserOrdersFooterItem = true,
                DisplayUserAddressesFooterItem = true,
                DisplayShoppingCartFooterItem = true,
                DisplayWishlistFooterItem = true,
                DisplayApplyVendorAccountFooterItem = true
            });

            await settingService.SaveSettingAsync(new CaptchaSettings
            {
                ReCaptchaApiUrl = "https://www.google.com/recaptcha/",
                ReCaptchaDefaultLanguage = string.Empty,
                ReCaptchaPrivateKey = string.Empty,
                ReCaptchaPublicKey = string.Empty,
                ReCaptchaRequestTimeout = 20,
                ReCaptchaTheme = string.Empty,
                AutomaticallyChooseLanguage = true,
                Enabled = false,
                CaptchaType = CaptchaType.CheckBoxReCaptchaV2,
                ReCaptchaV3ScoreThreshold = 0.5M,
                ShowOnApplyVendorPage = false,
                ShowOnBlogCommentPage = false,
                ShowOnContactUsPage = false,
                ShowOnEmailTvChannelToFriendPage = false,
                ShowOnEmailWishlistToFriendPage = false,
                ShowOnForgotPasswordPage = false,
                ShowOnForum = false,
                ShowOnLoginPage = false,
                ShowOnNewsCommentPage = false,
                ShowOnTvChannelReviewPage = false,
                ShowOnRegistrationPage = false,
                ShowOnCheckoutPageForGuests = false,
            });

            await settingService.SaveSettingAsync(new MessagesSettings { UsePopupNotifications = false });

            await settingService.SaveSettingAsync(new ProxySettings
            {
                Enabled = false,
                Address = string.Empty,
                Port = string.Empty,
                Username = string.Empty,
                Password = string.Empty,
                BypassOnLocal = true,
                PreAuthenticate = true
            });

            await settingService.SaveSettingAsync(new CookieSettings
            {
                CompareTvChannelsCookieExpires = 24 * 10,
                RecentlyViewedTvChannelsCookieExpires = 24 * 10,
                UserCookieExpires = 24 * 365
            });

            await settingService.SaveSettingAsync(new RobotsTxtSettings
            {
                DisallowPaths = new List<string>
                {
                    "/admin",
                    "/bin/",
                    "/files/",
                    "/files/exportimport/",
                    "/country/getstatesbycountryid",
                    "/install",
                    "/settvchannelreviewhelpfulness",
                    "/*?*returnUrl="
                },
                LocalizableDisallowPaths = new List<string>
                {
                    "/addtvсhanneltocart/catalog/",
                    "/addtvсhanneltocart/details/",
                    "/backinstocksubscriptions/manage",
                    "/boards/forumsubscriptions",
                    "/boards/forumwatch",
                    "/boards/postedit",
                    "/boards/postdelete",
                    "/boards/postcreate",
                    "/boards/topicedit",
                    "/boards/topicdelete",
                    "/boards/topiccreate",
                    "/boards/topicmove",
                    "/boards/topicwatch",
                    "/cart$",
                    "/changecurrency",
                    "/changelanguage",
                    "/changetaxtype",
                    "/checkout",
                    "/checkout/billingaddress",
                    "/checkout/completed",
                    "/checkout/confirm",
                    "/checkout/shippingaddress",
                    "/checkout/shippingmethod",
                    "/checkout/paymentinfo",
                    "/checkout/paymentmethod",
                    "/clearcomparelist",
                    "/comparetvсhannels",
                    "/comparetvсhannels/add/*",
                    "/user/avatar",
                    "/user/activation",
                    "/user/addresses",
                    "/user/changepassword",
                    "/user/checkusernameavailability",
                    "/user/downloadabletvсhannels",
                    "/user/info",
                    "/user/tvсhannelreviews",
                    "/deletepm",
                    "/emailwishlist",
                    "/eucookielawaccept",
                    "/inboxupdate",
                    "/newsletter/subscriptionactivation",
                    "/onepagecheckout",
                    "/order/history",
                    "/orderdetails",
                    "/passwordrecovery/confirm",
                    "/poll/vote",
                    "/privatemessages",
                    "/recentlyviewedtvсhannels",
                    "/returnrequest",
                    "/returnrequest/history",
                    "/rewardpoints/history",
                    "/search?",
                    "/sendpm",
                    "/sentupdate",
                    "/shoppingcart/*",
                    "/storeclosed",
                    "/subscribenewsletter",
                    "/topic/authenticate",
                    "/viewpm",
                    "/uploadfilecheckoutattribute",
                    "/uploadfiletvсhannelattribute",
                    "/uploadfilereturnrequest",
                    "/wishlist"
                }
            });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallCheckoutAttributesAsync()
        {
            var ca1 = await InsertInstallationDataAsync(new CheckoutAttribute
            {
                Name = "Gift wrapping",
                IsRequired = true,
                ShippableTvChannelRequired = true,
                AttributeControlType = AttributeControlType.DropdownList,
                DisplayOrder = 1
            });

            await InsertInstallationDataAsync(
                new CheckoutAttributeValue
                {
                    Name = "No",
                    PriceAdjustment = 0,
                    DisplayOrder = 1,
                    IsPreSelected = true,
                    CheckoutAttributeId = ca1.Id
                },
                new CheckoutAttributeValue
                {
                    Name = "Yes",
                    PriceAdjustment = 10,
                    DisplayOrder = 2,
                    CheckoutAttributeId = ca1.Id
                });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallSpecificationAttributesAsync()
        {
            var sag1 = await InsertInstallationDataAsync(
                new SpecificationAttributeGroup
                {
                    Name = "System unit"
                });

            var sa1 = await InsertInstallationDataAsync(
                new SpecificationAttribute
                {
                    Name = "Screensize",
                    DisplayOrder = 1
                });

            await InsertInstallationDataAsync(
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa1.Id,
                    Name = "13.0''",
                    DisplayOrder = 2
                },
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa1.Id,
                    Name = "13.3''",
                    DisplayOrder = 3
                },
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa1.Id,
                    Name = "14.0''",
                    DisplayOrder = 4
                },
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa1.Id,
                    Name = "15.0''",
                    DisplayOrder = 4
                },
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa1.Id,
                    Name = "15.6''",
                    DisplayOrder = 5
                });

            var sa2 = await InsertInstallationDataAsync(
                new SpecificationAttribute
                {
                    Name = "CPU Type",
                    DisplayOrder = 2,
                    SpecificationAttributeGroupId = sag1.Id
                });

            await InsertInstallationDataAsync(
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa2.Id,
                    Name = "Intel Core i5",
                    DisplayOrder = 1
                },
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa2.Id,
                    Name = "Intel Core i7",
                    DisplayOrder = 2
                });

            var sa3 = await InsertInstallationDataAsync(
                new SpecificationAttribute
                {
                    Name = "Memory",
                    DisplayOrder = 3,
                    SpecificationAttributeGroupId = sag1.Id
                });

            await InsertInstallationDataAsync(
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa3.Id,
                    Name = "4 GB",
                    DisplayOrder = 1
                },
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa3.Id,
                    Name = "8 GB",
                    DisplayOrder = 2
                },
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa3.Id,
                    Name = "16 GB",
                    DisplayOrder = 3
                });

            var sa4 = await InsertInstallationDataAsync(
                new SpecificationAttribute
                {
                    Name = "Hard drive",
                    DisplayOrder = 5,
                    SpecificationAttributeGroupId = sag1.Id
                });

            await InsertInstallationDataAsync(
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa4.Id,
                    Name = "128 GB",
                    DisplayOrder = 7
                },
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa4.Id,
                    Name = "500 GB",
                    DisplayOrder = 4
                },
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa4.Id,
                    Name = "1 TB",
                    DisplayOrder = 3
                });

            var sa5 = await InsertInstallationDataAsync(
                new SpecificationAttribute
                {
                    Name = "Color",
                    DisplayOrder = 1
                });

            await InsertInstallationDataAsync(
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa5.Id,
                    Name = "Grey",
                    DisplayOrder = 2,
                    ColorSquaresRgb = "#8a97a8"
                },
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa5.Id,
                    Name = "Red",
                    DisplayOrder = 3,
                    ColorSquaresRgb = "#8a374a"
                },
                new SpecificationAttributeOption
                {
                    SpecificationAttributeId = sa5.Id,
                    Name = "Blue",
                    DisplayOrder = 4,
                    ColorSquaresRgb = "#47476f"
                });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallTvChannelAttributesAsync()
        {
            var tvChannelAttributes = new List<TvChannelAttribute>
            {
                new TvChannelAttribute
                {
                    Name = "Color"
                },
                new TvChannelAttribute
                {
                    Name = "Print"
                },
                new TvChannelAttribute
                {
                    Name = "Custom Text"
                },
                new TvChannelAttribute
                {
                    Name = "HDD"
                },
                new TvChannelAttribute
                {
                    Name = "OS"
                },
                new TvChannelAttribute
                {
                    Name = "Processor"
                },
                new TvChannelAttribute
                {
                    Name = "RAM"
                },
                new TvChannelAttribute
                {
                    Name = "Size"
                },
                new TvChannelAttribute
                {
                    Name = "Software"
                }
            };

            await InsertInstallationDataAsync(tvChannelAttributes);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallCategoriesAsync()
        {
            //pictures
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = GetSamplesPath();

            var categoryTemplateInGridAndLines = _categoryTemplateRepository
                .Table.FirstOrDefault(pt => pt.Name == "TvChannels in Grid or Lines");
            if (categoryTemplateInGridAndLines == null)
                throw new Exception("Category template cannot be loaded");

            //categories
            var allCategories = new List<Category>();
            var categoryComputers = new Category
            {
                Name = "Computers",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_computers.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Computers"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryComputers);

            await InsertInstallationDataAsync(categoryComputers);

            var categoryDesktops = new Category
            {
                Name = "Desktops",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryComputers.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_desktops.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Desktops"))).Id,
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryDesktops);

            await InsertInstallationDataAsync(categoryDesktops);

            var categoryNotebooks = new Category
            {
                Name = "Notebooks",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryComputers.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_notebooks.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Notebooks"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryNotebooks);

            await InsertInstallationDataAsync(categoryNotebooks);

            var categorySoftware = new Category
            {
                Name = "Software",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryComputers.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_software.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Software"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categorySoftware);

            await InsertInstallationDataAsync(categorySoftware);

            var categoryElectronics = new Category
            {
                Name = "Electronics",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_electronics.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Electronics"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomepage = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryElectronics);

            await InsertInstallationDataAsync(categoryElectronics);

            var categoryCameraPhoto = new Category
            {
                Name = "Camera & photo",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryElectronics.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_camera_photo.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Camera, photo"))).Id,
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryCameraPhoto);

            await InsertInstallationDataAsync(categoryCameraPhoto);

            var categoryCellSmartPhones = new Category
            {
                Name = "Cell phones",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryElectronics.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_cell_phones.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Cell phones"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryCellSmartPhones);

            await InsertInstallationDataAsync(categoryCellSmartPhones);

            var categoryOthers = new Category
            {
                Name = "Others",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryElectronics.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_accessories.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Accessories"))).Id,
                IncludeInTopMenu = true,
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryOthers);

            await InsertInstallationDataAsync(categoryOthers);

            var categoryApparel = new Category
            {
                Name = "Apparel",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_apparel.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Apparel"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomepage = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryApparel);

            await InsertInstallationDataAsync(categoryApparel);

            var categoryShoes = new Category
            {
                Name = "Shoes",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryApparel.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_shoes.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Shoes"))).Id,
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryShoes);

            await InsertInstallationDataAsync(categoryShoes);

            var categoryClothing = new Category
            {
                Name = "Clothing",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryApparel.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_clothing.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Clothing"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 2,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryClothing);

            await InsertInstallationDataAsync(categoryClothing);

            var categoryAccessories = new Category
            {
                Name = "Accessories",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                ParentCategoryId = categoryApparel.Id,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_apparel_accessories.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Apparel Accessories"))).Id,
                IncludeInTopMenu = true,
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo,
                Published = true,
                DisplayOrder = 3,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryAccessories);

            await InsertInstallationDataAsync(categoryAccessories);

            var categoryDigitalDownloads = new Category
            {
                Name = "Digital downloads",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_digital_downloads.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Digital downloads"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                ShowOnHomepage = true,
                DisplayOrder = 4,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryDigitalDownloads);

            await InsertInstallationDataAsync(categoryDigitalDownloads);

            var categoryBooks = new Category
            {
                Name = "Books",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                MetaKeywords = "Books, Dictionary, Textbooks",
                MetaDescription = "Books category description",
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_book.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Book"))).Id,
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryBooks);

            await InsertInstallationDataAsync(categoryBooks);

            var categoryJewelry = new Category
            {
                Name = "Jewelry",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_jewelry.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Jewelry"))).Id,
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 6,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryJewelry);

            await InsertInstallationDataAsync(categoryJewelry);

            var categoryGiftCards = new Category
            {
                Name = "Gift Cards",
                CategoryTemplateId = categoryTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "category_gift_cards.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync("Gift Cards"))).Id,
                IncludeInTopMenu = true,
                Published = true,
                DisplayOrder = 7,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allCategories.Add(categoryGiftCards);

            await InsertInstallationDataAsync(categoryGiftCards);

            //search engine names
            foreach (var category in allCategories)
                await InsertInstallationDataAsync(new UrlRecord
                {
                    EntityId = category.Id,
                    EntityName = nameof(Category),
                    LanguageId = 0,
                    IsActive = true,
                    Slug = await ValidateSeNameAsync(category, category.Name)
                });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallManufacturersAsync()
        {
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = GetSamplesPath();

            var manufacturerTemplateInGridAndLines =
                _manufacturerTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "TvChannels in Grid or Lines");
            if (manufacturerTemplateInGridAndLines == null)
                throw new Exception("Manufacturer template cannot be loaded");

            var allManufacturers = new List<Manufacturer>();
            var manufacturerAsus = new Manufacturer
            {
                Name = "Apple",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo,
                Published = true,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "manufacturer_apple.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Apple"))).Id,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            await InsertInstallationDataAsync(manufacturerAsus);

            allManufacturers.Add(manufacturerAsus);

            var manufacturerHp = new Manufacturer
            {
                Name = "HP",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                PriceRangeFiltering = true,
                ManuallyPriceRange = true,
                PriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom,
                PriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo,
                Published = true,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "manufacturer_hp.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Hp"))).Id,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            await InsertInstallationDataAsync(manufacturerHp);

            allManufacturers.Add(manufacturerHp);

            var manufacturerNike = new Manufacturer
            {
                Name = "Nike",
                ManufacturerTemplateId = manufacturerTemplateInGridAndLines.Id,
                PageSize = 6,
                AllowUsersToSelectPageSize = true,
                PageSizeOptions = "6, 3, 9",
                Published = true,
                PictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "manufacturer_nike.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Nike"))).Id,
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            await InsertInstallationDataAsync(manufacturerNike);

            allManufacturers.Add(manufacturerNike);

            //search engine names
            foreach (var manufacturer in allManufacturers)
                await InsertInstallationDataAsync(new UrlRecord
                {
                    EntityId = manufacturer.Id,
                    EntityName = nameof(Manufacturer),
                    LanguageId = 0,
                    IsActive = true,
                    Slug = await ValidateSeNameAsync(manufacturer, manufacturer.Name)
                });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallComputersAsync(TvChannelTemplate tvChannelTemplateSimple, List<TvChannel> allTvChannels, string sampleImagesPath, IPictureService pictureService, List<RelatedTvChannel> relatedTvChannels)
        {
            var tvChannelBuildComputer = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Build your own computer",
                Sku = "COMP_CUST",
                ShortDescription = "Build it",
                FullDescription = "<p>Fight back against cluttered workspaces with the stylish IBM zBC12 All-in-One desktop PC, featuring powerful computing resources and a stunning 20.1-inch widescreen display with stunning XBRITE-HiColor LCD technology. The black IBM zBC12 has a built-in microphone and MOTION EYE camera with face-tracking technology that allows for easy communication with friends and family. And it has a built-in DVD burner and Sony's Movie Store software so you can create a digital entertainment library for personal viewing at your convenience. Easy to setup and even easier to use, this JS-series All-in-One includes an elegantly designed keyboard and a USB mouse.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "build-your-own-computer",
                AllowUserReviews = true,
                Price = 1200M,
                IsShipEnabled = true,
                IsFreeShipping = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                ShowOnHomepage = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            allTvChannels.Add(tvChannelBuildComputer);

            await InsertInstallationDataAsync(tvChannelBuildComputer);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelBuildComputer.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Desktops").Id,
                DisplayOrder = 1
            });

            var picTvChannelDesktops1 = await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "tvChannel_Desktops_1.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync(tvChannelBuildComputer.Name));
            var picTvChannelDesktops2 = await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "tvChannel_Desktops_2.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync(tvChannelBuildComputer.Name));

            await InsertInstallationDataAsync(
                new TvChannelPicture
                {
                    TvChannelId = tvChannelBuildComputer.Id,
                    PictureId = picTvChannelDesktops1.Id,
                    DisplayOrder = 1
                },
                new TvChannelPicture
                {
                    TvChannelId = tvChannelBuildComputer.Id,
                    PictureId = picTvChannelDesktops2.Id,
                    DisplayOrder = 2
                });

            var pamProcessor = await InsertInstallationDataAsync(new TvChannelAttributeMapping
            {
                TvChannelId = tvChannelBuildComputer.Id,
                TvChannelAttributeId = _tvChannelAttributeRepository.Table.Single(x => x.Name == "Processor").Id,
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true
            });

            await InsertInstallationDataAsync(
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamProcessor.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "2.2 GHz Intel Pentium Dual-Core E2200",
                    DisplayOrder = 1
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamProcessor.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "2.5 GHz Intel Pentium Dual-Core E2200",
                    IsPreSelected = true,
                    PriceAdjustment = 15,
                    DisplayOrder = 2
                });

            var pamRam = await InsertInstallationDataAsync(new TvChannelAttributeMapping
            {
                TvChannelId = tvChannelBuildComputer.Id,
                TvChannelAttributeId = _tvChannelAttributeRepository.Table.Single(x => x.Name == "RAM").Id,
                AttributeControlType = AttributeControlType.DropdownList,
                IsRequired = true
            });

            await InsertInstallationDataAsync(
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamRam.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "2 GB",
                    DisplayOrder = 1
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamRam.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "4GB",
                    PriceAdjustment = 20,
                    DisplayOrder = 2
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamRam.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "8GB",
                    PriceAdjustment = 60,
                    DisplayOrder = 3
                });

            var pamHdd = await InsertInstallationDataAsync(
                new TvChannelAttributeMapping
                {
                    TvChannelId = tvChannelBuildComputer.Id,
                    TvChannelAttributeId = _tvChannelAttributeRepository.Table.Single(x => x.Name == "HDD").Id,
                    AttributeControlType = AttributeControlType.RadioList,
                    IsRequired = true
                });

            await InsertInstallationDataAsync(
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamHdd.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "320 GB",
                    DisplayOrder = 1
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamHdd.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "400 GB",
                    PriceAdjustment = 100,
                    DisplayOrder = 2
                });

            var pamOs = await InsertInstallationDataAsync(
                new TvChannelAttributeMapping
                {
                    TvChannelId = tvChannelBuildComputer.Id,
                    TvChannelAttributeId = _tvChannelAttributeRepository.Table.Single(x => x.Name == "OS").Id,
                    AttributeControlType = AttributeControlType.RadioList,
                    IsRequired = true
                });

            await InsertInstallationDataAsync(
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamOs.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "Vista Home",
                    PriceAdjustment = 50,
                    IsPreSelected = true,
                    DisplayOrder = 1
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamOs.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "Vista Premium",
                    PriceAdjustment = 60,
                    DisplayOrder = 2
                });

            var pamSoftware = await InsertInstallationDataAsync(new TvChannelAttributeMapping
            {
                TvChannelId = tvChannelBuildComputer.Id,
                TvChannelAttributeId = _tvChannelAttributeRepository.Table.Single(x => x.Name == "Software").Id,
                AttributeControlType = AttributeControlType.Checkboxes
            });

            await InsertInstallationDataAsync(
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamSoftware.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "Microsoft Office",
                    PriceAdjustment = 50,
                    IsPreSelected = true,
                    DisplayOrder = 1
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamSoftware.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "Acrobat Reader",
                    PriceAdjustment = 10,
                    DisplayOrder = 2
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamSoftware.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "Total Commander",
                    PriceAdjustment = 5,
                    DisplayOrder = 2
                });

            await AddTvChannelTagAsync(tvChannelBuildComputer, "awesome");
            await AddTvChannelTagAsync(tvChannelBuildComputer, "computer");

            var tvChannelDigitalStorm = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Digital Storm VANQUISH 3 Custom Performance PC",
                Sku = "DS_VA3_PC",
                ShortDescription = "Digital Storm Vanquish 3 Desktop PC",
                FullDescription = "<p>Blow the doors off today’s most demanding games with maximum detail, speed, and power for an immersive gaming experience without breaking the bank.</p><p>Stay ahead of the competition, VANQUISH 3 is fully equipped to easily handle future upgrades, keeping your system on the cutting edge for years to come.</p><p>Each system is put through an extensive stress test, ensuring you experience zero bottlenecks and get the maximum performance from your hardware.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "compaq-presario-sr1519x-pentium-4-desktop-pc-with-cdrw",
                AllowUserReviews = true,
                Price = 1259M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelDigitalStorm);

            await InsertInstallationDataAsync(tvChannelDigitalStorm);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelDigitalStorm.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Desktops").Id,
                DisplayOrder = 1
            });

            var picTvChannelDigitalStorm = await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "tvChannel_DigitalStorm.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync(tvChannelDigitalStorm.Name));

            await InsertInstallationDataAsync(new TvChannelPicture
            {
                TvChannelId = tvChannelDigitalStorm.Id,
                PictureId = picTvChannelDigitalStorm.Id,
                DisplayOrder = 1
            });

            await AddTvChannelTagAsync(tvChannelDigitalStorm, "cool");
            await AddTvChannelTagAsync(tvChannelDigitalStorm, "computer");

            var tvChannelLenovoIdeaCentre = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Lenovo IdeaCentre 600 All-in-One PC",
                Sku = "LE_IC_600",
                ShortDescription = string.Empty,
                FullDescription = "<p>The A600 features a 21.5in screen, DVD or optional Blu-Ray drive, support for the full beans 1920 x 1080 HD, Dolby Home Cinema certification and an optional hybrid analogue/digital TV tuner.</p><p>Connectivity is handled by 802.11a/b/g - 802.11n is optional - and an ethernet port. You also get four USB ports, a Firewire slot, a six-in-one card reader and a 1.3- or two-megapixel webcam.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "hp-iq506-touchsmart-desktop-pc",
                AllowUserReviews = true,
                Price = 500M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelLenovoIdeaCentre);

            await InsertInstallationDataAsync(tvChannelLenovoIdeaCentre);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelLenovoIdeaCentre.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Desktops").Id,
                DisplayOrder = 1
            });

            var picTvChannelLenovoIdeaCentre = await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "tvChannel_LenovoIdeaCentre.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync(tvChannelLenovoIdeaCentre.Name));

            await InsertInstallationDataAsync(new TvChannelPicture
            {
                TvChannelId = tvChannelLenovoIdeaCentre.Id,
                PictureId = picTvChannelLenovoIdeaCentre.Id,
                DisplayOrder = 1
            });

            await AddTvChannelTagAsync(tvChannelLenovoIdeaCentre, "awesome");
            await AddTvChannelTagAsync(tvChannelLenovoIdeaCentre, "computer");

            var tvChannelAppleMacBookPro = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Apple MacBook Pro 13-inch",
                Sku = "AP_MBP_13",
                ShortDescription = "A groundbreaking Retina display. A new force-sensing trackpad. All-flash architecture. Powerful dual-core and quad-core Intel processors. Together, these features take the notebook to a new level of performance. And they will do the same for you in everything you create.",
                FullDescription = "<p>With fifth-generation Intel Core processors, the latest graphics, and faster flash storage, the incredibly advanced MacBook Pro with Retina display moves even further ahead in performance and battery life.* *Compared with the previous generation.</p><p>Retina display with 2560-by-1600 resolution</p><p>Fifth-generation dual-core Intel Core i5 processor</p><p>Intel Iris Graphics</p><p>Up to 9 hours of battery life1</p><p>Faster flash storage2</p><p>802.11ac Wi-Fi</p><p>Two Thunderbolt 2 ports for connecting high-performance devices and transferring data at lightning speed</p><p>Two USB 3 ports (compatible with USB 2 devices) and HDMI</p><p>FaceTime HD camera</p><p>Pages, Numbers, Keynote, iPhoto, iMovie, GarageBand included</p><p>OS X, the world's most advanced desktop operating system</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "asus-eee-pc-1000ha-10-inch-netbook",
                AllowUserReviews = true,
                Price = 1800M,
                IsShipEnabled = true,
                IsFreeShipping = true,
                Weight = 3,
                Length = 3,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 2,
                OrderMaximumQuantity = 10000,
                Published = true,
                ShowOnHomepage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelAppleMacBookPro);

            await InsertInstallationDataAsync(tvChannelAppleMacBookPro);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelAppleMacBookPro.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Notebooks").Id,
                DisplayOrder = 1
            });

            await InsertInstallationDataAsync(new TvChannelManufacturer
            {
                TvChannelId = tvChannelAppleMacBookPro.Id,
                ManufacturerId = _manufacturerRepository.Table.Single(c => c.Name == "Apple").Id,
                DisplayOrder = 2
            });

            var picTvChannelMacBook1 = await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "tvChannel_macbook_1.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync(tvChannelAppleMacBookPro.Name));
            var picTvChannelMacBook2 = await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "tvChannel_macbook_2.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync(tvChannelAppleMacBookPro.Name));

            await InsertInstallationDataAsync(new TvChannelPicture
            {
                TvChannelId = tvChannelAppleMacBookPro.Id,
                PictureId = picTvChannelMacBook1.Id,
                DisplayOrder = 1
            }, new TvChannelPicture
            {
                TvChannelId = tvChannelAppleMacBookPro.Id,
                PictureId = picTvChannelMacBook2.Id,
                DisplayOrder = 2
            });

            await InsertInstallationDataAsync(
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelAppleMacBookPro.Id,
                    AllowFiltering = false,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 1,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Screensize", "13.0''")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelAppleMacBookPro.Id,
                    AllowFiltering = true,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 2,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("CPU Type", "Intel Core i5")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelAppleMacBookPro.Id,
                    AllowFiltering = true,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 3,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Memory", "4 GB")
                });

            await AddTvChannelTagAsync(tvChannelAppleMacBookPro, "compact");
            await AddTvChannelTagAsync(tvChannelAppleMacBookPro, "awesome");
            await AddTvChannelTagAsync(tvChannelAppleMacBookPro, "computer");

            var tvChannelAsusN551JK = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Asus N551JK-XO076H Laptop",
                Sku = "AS_551_LP",
                ShortDescription = "Laptop Asus N551JK Intel Core i7-4710HQ 2.5 GHz, RAM 16GB, HDD 1TB, Video NVidia GTX 850M 4GB, BluRay, 15.6, Full HD, Win 8.1",
                FullDescription = "<p>The ASUS N550JX combines cutting-edge audio and visual technology to deliver an unsurpassed multimedia experience. A full HD wide-view IPS panel is tailor-made for watching movies and the intuitive touchscreen makes for easy, seamless navigation. ASUS has paired the N550JX’s impressive display with SonicMaster Premium, co-developed with Bang & Olufsen ICEpower® audio experts, for true surround sound. A quad-speaker array and external subwoofer combine for distinct vocals and a low bass that you can feel.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "asus-eee-pc-900ha-89-inch-netbook-black",
                AllowUserReviews = true,
                Price = 1500M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            allTvChannels.Add(tvChannelAsusN551JK);

            await InsertInstallationDataAsync(tvChannelAsusN551JK);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelAsusN551JK.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Notebooks").Id,
                DisplayOrder = 1
            });

            var picTvChannelAsuspcN551Jk = await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "tvChannel_asuspc_N551JK.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync(tvChannelAsusN551JK.Name));

            await InsertInstallationDataAsync(new TvChannelPicture
            {
                TvChannelId = tvChannelAsusN551JK.Id,
                PictureId = picTvChannelAsuspcN551Jk.Id,
                DisplayOrder = 1
            });

            await InsertInstallationDataAsync(
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelAsusN551JK.Id,
                    AllowFiltering = false,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 1,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Screensize", "15.6''")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelAsusN551JK.Id,
                    AllowFiltering = true,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 2,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("CPU Type", "Intel Core i7")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelAsusN551JK.Id,
                    AllowFiltering = true,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 3,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Memory", "16 GB")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelAsusN551JK.Id,
                    AllowFiltering = false,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 4,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Hard drive", "1 TB")
                });

            await AddTvChannelTagAsync(tvChannelAsusN551JK, "compact");
            await AddTvChannelTagAsync(tvChannelAsusN551JK, "awesome");
            await AddTvChannelTagAsync(tvChannelAsusN551JK, "computer");

            var tvChannelSamsungSeries = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Samsung Series 9 NP900X4C Premium Ultrabook",
                Sku = "SM_900_PU",
                ShortDescription = "Samsung Series 9 NP900X4C-A06US 15-Inch Ultrabook (1.70 GHz Intel Core i5-3317U Processor, 8GB DDR3, 128GB SSD, Windows 8) Ash Black",
                FullDescription = "<p>Designed with mobility in mind, Samsung's durable, ultra premium, lightweight Series 9 laptop (model NP900X4C-A01US) offers mobile professionals and power users a sophisticated laptop equally suited for work and entertainment. Featuring a minimalist look that is both simple and sophisticated, its polished aluminum uni-body design offers an iconic look and feel that pushes the envelope with an edge just 0.58 inches thin. This Series 9 laptop also includes a brilliant 15-inch SuperBright Plus display with HD+ technology, 128 GB Solid State Drive (SSD), 8 GB of system memory, and up to 10 hours of battery life.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "hp-pavilion-artist-edition-dv2890nr-141-inch-laptop",
                AllowUserReviews = true,
                Price = 1590M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                //ShowOnHomepage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelSamsungSeries);

            await InsertInstallationDataAsync(tvChannelSamsungSeries);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelSamsungSeries.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Notebooks").Id,
                DisplayOrder = 1
            });

            var picTvChannelSamsungNp900X4C = await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "tvChannel_SamsungNP900X4C.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync(tvChannelSamsungSeries.Name));

            await InsertInstallationDataAsync(new TvChannelPicture
            {
                TvChannelId = tvChannelSamsungSeries.Id,
                PictureId = picTvChannelSamsungNp900X4C.Id,
                DisplayOrder = 1
            });

            await InsertInstallationDataAsync(
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelSamsungSeries.Id,
                    AllowFiltering = false,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 1,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Screensize", "15.0''")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelSamsungSeries.Id,
                    AllowFiltering = true,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 2,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("CPU Type", "Intel Core i5")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelSamsungSeries.Id,
                    AllowFiltering = true,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 3,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Memory", "8 GB")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelSamsungSeries.Id,
                    AllowFiltering = false,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 4,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Hard drive", "128 GB")
                });

            await AddTvChannelTagAsync(tvChannelSamsungSeries, "nice");
            await AddTvChannelTagAsync(tvChannelSamsungSeries, "computer");
            await AddTvChannelTagAsync(tvChannelSamsungSeries, "compact");

            var tvChannelHpSpectre = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "HP Spectre XT Pro UltraBook",
                Sku = "HP_SPX_UB",
                ShortDescription = "HP Spectre XT Pro UltraBook / Intel Core i5-2467M / 13.3 / 4GB / 128GB / Windows 7 Professional / Laptop",
                FullDescription = "<p>Introducing HP ENVY Spectre XT, the Ultrabook designed for those who want style without sacrificing substance. It's sleek. It's thin. And with Intel. Corer i5 processor and premium materials, it's designed to go anywhere from the bistro to the boardroom, it's unlike anything you've ever seen from HP.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "hp-pavilion-elite-m9150f-desktop-pc",
                AllowUserReviews = true,
                Price = 1350M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelHpSpectre);

            await InsertInstallationDataAsync(tvChannelHpSpectre);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelHpSpectre.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Notebooks").Id,
                DisplayOrder = 1
            });

            await InsertInstallationDataAsync(new TvChannelManufacturer
            {
                TvChannelId = tvChannelHpSpectre.Id,
                ManufacturerId = _manufacturerRepository.Table.Single(c => c.Name == "HP").Id,
                DisplayOrder = 3
            });

            var picTvChannelHpSpectreXt1 = await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "tvChannel_HPSpectreXT_1.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync(tvChannelHpSpectre.Name));
            var picTvChannelHpSpectreXt2 = await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "tvChannel_HPSpectreXT_2.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync(tvChannelHpSpectre.Name));

            await InsertInstallationDataAsync(new TvChannelPicture
            {
                TvChannelId = tvChannelHpSpectre.Id,
                PictureId = picTvChannelHpSpectreXt1.Id,
                DisplayOrder = 1
            },
            new TvChannelPicture
            {
                TvChannelId = tvChannelHpSpectre.Id,
                PictureId = picTvChannelHpSpectreXt2.Id,
                DisplayOrder = 2
            });

            await InsertInstallationDataAsync(
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelHpSpectre.Id,
                    AllowFiltering = false,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 1,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Screensize", "13.3''")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelHpSpectre.Id,
                    AllowFiltering = true,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 2,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("CPU Type", "Intel Core i5")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelHpSpectre.Id,
                    AllowFiltering = true,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 3,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Memory", "4 GB")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelHpSpectre.Id,
                    AllowFiltering = false,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 4,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Hard drive", "128 GB")
                });

            await AddTvChannelTagAsync(tvChannelHpSpectre, "nice");
            await AddTvChannelTagAsync(tvChannelHpSpectre, "computer");

            var tvChannelHpEnvy = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "HP Envy 6-1180ca 15.6-Inch Sleekbook",
                Sku = "HP_ESB_15",
                ShortDescription = "HP ENVY 6-1202ea Ultrabook Beats Audio, 3rd generation Intel® CoreTM i7-3517U processor, 8GB RAM, 500GB HDD, Microsoft Windows 8, AMD Radeon HD 8750M (2 GB DDR3 dedicated)",
                FullDescription = "The UltrabookTM that's up for anything. Thin and light, the HP ENVY is the large screen UltrabookTM with Beats AudioTM. With a soft-touch base that makes it easy to grab and go, it's a laptop that's up for anything.<br /><br /><b>Features</b><br /><br />- Windows 8 or other operating systems available<br /><br /><b>Top performance. Stylish design. Take notice.</b><br /><br />- At just 19.8 mm (0.78 in) thin, the HP ENVY UltrabookTM is slim and light enough to take anywhere. It's the laptop that gets you noticed with the power to get it done.<br />- With an eye-catching metal design, it's a laptop that you want to carry with you. The soft-touch, slip-resistant base gives you the confidence to carry it with ease.<br /><br /><b>More entertaining. More gaming. More fun.</b><br /><br />- Own the UltrabookTM with Beats AudioTM, dual speakers, a subwoofer, and an awesome display. Your music, movies and photo slideshows will always look and sound their best.<br />- Tons of video memory let you experience incredible gaming and multimedia without slowing down. Create and edit videos in a flash. And enjoy more of what you love to the fullest.<br />- The HP ENVY UltrabookTM is loaded with the ports you'd expect on a world-class laptop, but on a Sleekbook instead. Like HDMI, USB, RJ-45, and a headphone jack. You get all the right connections without compromising size.<br /><br /><b>Only from HP.</b><br /><br />- Life heats up. That's why there's HP CoolSense technology, which automatically adjusts your notebook's temperature based on usage and conditions. It stays cool. You stay comfortable.<br />- With HP ProtectSmart, your notebook's data stays safe from accidental bumps and bruises. It senses motion and plans ahead, stopping your hard drive and protecting your entire digital life.<br />- Keep playing even in dimly lit rooms or on red eye flights. The optional backlit keyboard[1] is full-size so you don't compromise comfort. Backlit keyboard. Another bright idea.<br /><br />",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "hp-pavilion-g60-230us-160-inch-laptop",
                AllowUserReviews = true,
                Price = 1460M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelHpEnvy);

            await InsertInstallationDataAsync(tvChannelHpEnvy);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelHpEnvy.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Notebooks").Id,
                DisplayOrder = 1
            });

            await InsertInstallationDataAsync(new TvChannelManufacturer
            {
                TvChannelId = tvChannelHpEnvy.Id,
                ManufacturerId = _manufacturerRepository.Table.Single(c => c.Name == "HP").Id,
                DisplayOrder = 4
            });

            var picTvChannelHpEnvy6 = await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "tvChannel_HpEnvy6.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync(tvChannelHpEnvy.Name));

            await InsertInstallationDataAsync(new TvChannelPicture
            {
                TvChannelId = tvChannelHpEnvy.Id,
                PictureId = picTvChannelHpEnvy6.Id,
                DisplayOrder = 1
            });

            await InsertInstallationDataAsync(
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelHpEnvy.Id,
                    AllowFiltering = false,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 1,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Screensize", "15.6''")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelHpEnvy.Id,
                    AllowFiltering = true,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 2,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("CPU Type", "Intel Core i7")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelHpEnvy.Id,
                    AllowFiltering = true,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 3,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Memory", "8 GB")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelHpEnvy.Id,
                    AllowFiltering = false,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 4,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Hard drive", "500 GB")
                });

            await AddTvChannelTagAsync(tvChannelHpEnvy, "computer");
            await AddTvChannelTagAsync(tvChannelHpEnvy, "cool");
            await AddTvChannelTagAsync(tvChannelHpEnvy, "compact");

            var tvChannelLenovoThinkpad = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Lenovo Thinkpad X1 Carbon Laptop",
                Sku = "LE_TX1_CL",
                ShortDescription = "Lenovo Thinkpad X1 Carbon Touch Intel Core i7 14 Ultrabook",
                FullDescription = "<p>The X1 Carbon brings a new level of quality to the ThinkPad legacy of high standards and innovation. It starts with the durable, carbon fiber-reinforced roll cage, making for the best Ultrabook construction available, and adds a host of other new features on top of the old favorites. Because for 20 years, we haven't stopped innovating. And you shouldn't stop benefiting from that.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "toshiba-satellite-a305-s6908-154-inch-laptop",
                AllowUserReviews = true,
                Price = 1360M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelLenovoThinkpad);

            await InsertInstallationDataAsync(tvChannelLenovoThinkpad);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelLenovoThinkpad.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Notebooks").Id,
                DisplayOrder = 1
            });

            var picTvChannelLenovoThinkpad = await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "tvChannel_LenovoThinkpad.jpeg")), MimeTypes.ImageJpeg, await pictureService.GetPictureSeNameAsync(tvChannelLenovoThinkpad.Name));

            await InsertInstallationDataAsync(new TvChannelPicture
            {
                TvChannelId = tvChannelLenovoThinkpad.Id,
                PictureId = picTvChannelLenovoThinkpad.Id,
                DisplayOrder = 1
            });

            await InsertInstallationDataAsync(
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelLenovoThinkpad.Id,
                    AllowFiltering = false,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 1,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Screensize", "14.0''")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelLenovoThinkpad.Id,
                    AllowFiltering = true,
                    ShowOnTvChannelPage = true,
                    DisplayOrder = 2,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("CPU Type", "Intel Core i7")
                });

            await AddTvChannelTagAsync(tvChannelLenovoThinkpad, "awesome");
            await AddTvChannelTagAsync(tvChannelLenovoThinkpad, "computer");
            await AddTvChannelTagAsync(tvChannelLenovoThinkpad, "compact");

            var tvChannelAdobePhotoshop = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Adobe Photoshop CS4",
                Sku = "AD_CS4_PH",
                ShortDescription = "Easily find and view all your photos",
                FullDescription = "<p>Adobe Photoshop CS4 software combines power and simplicity so you can make ordinary photos extraordinary; tell engaging stories in beautiful, personalized creations for print and web; and easily find and view all your photos. New Photoshop.com membership* works with Photoshop CS4 so you can protect your photos with automatic online backup and 2 GB of storage; view your photos anywhere you are; and share your photos in fun, interactive ways with invitation-only Online Albums.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "adobe-photoshop-elements-7",
                AllowUserReviews = true,
                Price = 75M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelAdobePhotoshop);

            await InsertInstallationDataAsync(tvChannelAdobePhotoshop);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelAdobePhotoshop.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Software").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelAdobePhotoshop, "tvChannel_AdobePhotoshop.jpeg");

            await AddTvChannelTagAsync(tvChannelAdobePhotoshop, "computer");
            await AddTvChannelTagAsync(tvChannelAdobePhotoshop, "awesome");

            var tvChannelWindows8Pro = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Windows 8 Pro",
                Sku = "MS_WIN_8P",
                ShortDescription = "Windows 8 is a Microsoft operating system that was released in 2012 as part of the company's Windows NT OS family. ",
                FullDescription = "<p>Windows 8 Pro is comparable to Windows 7 Professional and Ultimate and is targeted towards enthusiasts and business users; it includes all the features of Windows 8. Additional features include the ability to receive Remote Desktop connections, the ability to participate in a Windows Server domain, Encrypting File System, Hyper-V, and Virtual Hard Disk Booting, Group Policy as well as BitLocker and BitLocker To Go. Windows Media Center functionality is available only for Windows 8 Pro as a separate software package.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "corel-paint-shop-pro-photo-x2",
                AllowUserReviews = true,
                Price = 65M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelWindows8Pro);

            await InsertInstallationDataAsync(tvChannelWindows8Pro);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelWindows8Pro.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Software").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelWindows8Pro, "tvChannel_Windows8.jpeg");

            await AddTvChannelTagAsync(tvChannelWindows8Pro, "awesome");
            await AddTvChannelTagAsync(tvChannelWindows8Pro, "computer");

            var tvChannelSoundForge = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Sound Forge Pro 11 (recurring)",
                Sku = "SF_PRO_11",
                ShortDescription = "Advanced audio waveform editor.",
                FullDescription = "<p>Sound Forge™ Pro is the application of choice for a generation of creative and prolific artists, producers, and editors. Record audio quickly on a rock-solid platform, address sophisticated audio processing tasks with surgical precision, and render top-notch master files with ease. New features include one-touch recording, metering for the new critical standards, more repair and restoration tools, and exclusive round-trip interoperability with SpectraLayers Pro. Taken together, these enhancements make this edition of Sound Forge Pro the deepest and most advanced audio editing platform available.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "major-league-baseball-2k9",
                IsRecurring = true,
                RecurringCycleLength = 30,
                RecurringCyclePeriod = RecurringTvChannelCyclePeriod.Months,
                RecurringTotalCycles = 12,
                AllowUserReviews = true,
                Price = 54.99M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelSoundForge);

            await InsertInstallationDataAsync(tvChannelSoundForge);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelSoundForge.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Software").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelSoundForge, "tvChannel_SoundForge.jpeg");

            await AddTvChannelTagAsync(tvChannelSoundForge, "game");
            await AddTvChannelTagAsync(tvChannelSoundForge, "computer");
            await AddTvChannelTagAsync(tvChannelSoundForge, "cool");

            relatedTvChannels.AddRange(new[]
            {
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelLenovoIdeaCentre.Id,
                    TvChannelId2 = tvChannelDigitalStorm.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelDigitalStorm.Id,
                    TvChannelId2 = tvChannelBuildComputer.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelDigitalStorm.Id,
                    TvChannelId2 = tvChannelLenovoIdeaCentre.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelDigitalStorm.Id,
                    TvChannelId2 = tvChannelLenovoThinkpad.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelDigitalStorm.Id,
                    TvChannelId2 = tvChannelAppleMacBookPro.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelLenovoIdeaCentre.Id,
                    TvChannelId2 = tvChannelBuildComputer.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelAsusN551JK.Id,
                    TvChannelId2 = tvChannelLenovoThinkpad.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelAsusN551JK.Id,
                    TvChannelId2 = tvChannelAppleMacBookPro.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelAsusN551JK.Id,
                    TvChannelId2 = tvChannelSamsungSeries.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelAsusN551JK.Id,
                    TvChannelId2 = tvChannelHpSpectre.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelLenovoThinkpad.Id,
                    TvChannelId2 = tvChannelAsusN551JK.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelLenovoThinkpad.Id,
                    TvChannelId2 = tvChannelAppleMacBookPro.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelLenovoThinkpad.Id,
                    TvChannelId2 = tvChannelSamsungSeries.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelLenovoThinkpad.Id,
                    TvChannelId2 = tvChannelHpEnvy.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelAppleMacBookPro.Id,
                    TvChannelId2 = tvChannelLenovoThinkpad.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelAppleMacBookPro.Id,
                    TvChannelId2 = tvChannelSamsungSeries.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelAppleMacBookPro.Id,
                    TvChannelId2 = tvChannelAsusN551JK.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelAppleMacBookPro.Id,
                    TvChannelId2 = tvChannelHpSpectre.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelHpSpectre.Id,
                    TvChannelId2 = tvChannelLenovoThinkpad.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelHpSpectre.Id,
                    TvChannelId2 = tvChannelSamsungSeries.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelHpSpectre.Id,
                    TvChannelId2 = tvChannelAsusN551JK.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelHpSpectre.Id,
                    TvChannelId2 = tvChannelHpEnvy.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelHpEnvy.Id,
                    TvChannelId2 = tvChannelAsusN551JK.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelHpEnvy.Id,
                    TvChannelId2 = tvChannelAppleMacBookPro.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelHpEnvy.Id,
                    TvChannelId2 = tvChannelHpSpectre.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelHpEnvy.Id,
                    TvChannelId2 = tvChannelSamsungSeries.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelSamsungSeries.Id,
                    TvChannelId2 = tvChannelAsusN551JK.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelSamsungSeries.Id,
                    TvChannelId2 = tvChannelAppleMacBookPro.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelSamsungSeries.Id,
                    TvChannelId2 = tvChannelHpEnvy.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelSamsungSeries.Id,
                    TvChannelId2 = tvChannelHpSpectre.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelLenovoIdeaCentre.Id,
                    TvChannelId2 = tvChannelLenovoThinkpad.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelLenovoIdeaCentre.Id,
                    TvChannelId2 = tvChannelAppleMacBookPro.Id
                }
            });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallElectronicsAsync(TvChannelTemplate tvChannelTemplateSimple, TvChannelTemplate tvChannelTemplateGrouped, List<TvChannel> allTvChannels, string sampleImagesPath, IPictureService pictureService, List<RelatedTvChannel> relatedTvChannels)
        {
            //this one is a grouped tvChannel with two associated ones
            var tvChannelNikonD5500DSLR = new TvChannel
            {
                TvChannelType = TvChannelType.GroupedTvChannel,
                VisibleIndividually = true,
                Name = "Nikon D5500 DSLR",
                Sku = "N5500DS_0",
                ShortDescription = "Slim, lightweight Nikon D5500 packs a vari-angle touchscreen",
                FullDescription = "Nikon has announced its latest DSLR, the D5500. A lightweight, compact DX-format camera with a 24.2MP sensor, it’s the first of its type to offer a vari-angle touchscreen. The D5500 replaces the D5300 in Nikon’s range, and while it offers much the same features the company says it’s a much slimmer and lighter prospect. There’s a deep grip for easier handling and built-in Wi-Fi that lets you transfer and share shots via your phone or tablet.",
                TvChannelTemplateId = tvChannelTemplateGrouped.Id,
                //SeName = "canon-digital-slr-camera",
                AllowUserReviews = true,
                Published = true,
                Price = 670M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelNikonD5500DSLR);

            await InsertInstallationDataAsync(tvChannelNikonD5500DSLR);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelNikonD5500DSLR.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Camera & photo").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelNikonD5500DSLR, "tvChannel_NikonCamera_1.jpeg");
            await InsertTvChannelPictureAsync(tvChannelNikonD5500DSLR, "tvChannel_NikonCamera_2.jpeg", 2);

            await AddTvChannelTagAsync(tvChannelNikonD5500DSLR, "cool");
            await AddTvChannelTagAsync(tvChannelNikonD5500DSLR, "camera");

            var tvChannelNikonD5500DslrAssociated1 = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = false, //hide this tvChannels
                ParentGroupedTvChannelId = tvChannelNikonD5500DSLR.Id,
                Name = "Nikon D5500 DSLR - Black",
                Sku = "N5500DS_B",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "canon-digital-slr-camera-black",
                AllowUserReviews = true,
                Published = true,
                Price = 670M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelNikonD5500DslrAssociated1);

            await InsertInstallationDataAsync(tvChannelNikonD5500DslrAssociated1);

            await InsertTvChannelPictureAsync(tvChannelNikonD5500DslrAssociated1, "tvChannel_NikonCamera_black.jpeg");

            var tvChannelNikonD5500DslrAssociated2 = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = false, //hide this tvChannels
                ParentGroupedTvChannelId = tvChannelNikonD5500DSLR.Id,
                Name = "Nikon D5500 DSLR - Red",
                Sku = "N5500DS_R",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "canon-digital-slr-camera-silver",
                AllowUserReviews = true,
                Published = true,
                Price = 630M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelNikonD5500DslrAssociated2);

            await InsertInstallationDataAsync(tvChannelNikonD5500DslrAssociated2);

            await InsertTvChannelPictureAsync(tvChannelNikonD5500DslrAssociated2, "tvChannel_NikonCamera_red.jpeg");

            var tvChannelLeica = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Leica T Mirrorless Digital Camera",
                Sku = "LT_MIR_DC",
                ShortDescription = "Leica T (Typ 701) Silver",
                FullDescription = "<p>The new Leica T offers a minimalist design that's crafted from a single block of aluminum.  Made in Germany and assembled by hand, this 16.3 effective mega pixel camera is easy to use.  With a massive 3.7 TFT LCD intuitive touch screen control, the user is able to configure and save their own menu system.  The Leica T has outstanding image quality and also has 16GB of built in memory.  This is Leica's first system camera to use Wi-Fi.  Add the T-App to your portable iOS device and be able to transfer and share your images (free download from the Apple App Store)</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "canon-vixia-hf100-camcorder",
                AllowUserReviews = true,
                Price = 530M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelLeica);

            await InsertInstallationDataAsync(tvChannelLeica);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelLeica.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Camera & photo").Id,
                DisplayOrder = 3
            });

            await InsertTvChannelPictureAsync(tvChannelLeica, "tvChannel_LeicaT.jpeg");

            await AddTvChannelTagAsync(tvChannelLeica, "camera");
            await AddTvChannelTagAsync(tvChannelLeica, "cool");

            var tvChannelAppleICam = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Apple iCam",
                Sku = "APPLE_CAM",
                ShortDescription = "Photography becomes smart",
                FullDescription = "<p>A few months ago we featured the amazing WVIL camera, by many considered the future of digital photography. This is another very good looking concept, iCam is the vision of Italian designer Antonio DeRosa, the idea is to have a device that attaches to the iSmartPhone 5, which then allows the user to have a camera with interchangeable lenses. The device would also feature a front-touch screen and a projector. Would be great if apple picked up on this and made it reality.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "panasonic-hdc-sdt750k-high-definition-3d-camcorder",
                AllowUserReviews = true,
                Price = 1300M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelAppleICam);

            await InsertInstallationDataAsync(tvChannelAppleICam);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelAppleICam.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Camera & photo").Id,
                DisplayOrder = 2
            });

            await InsertInstallationDataAsync(new TvChannelManufacturer
            {
                TvChannelId = tvChannelAppleICam.Id,
                ManufacturerId = _manufacturerRepository.Table.Single(c => c.Name == "Apple").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelAppleICam, "tvChannel_iCam.jpeg");

            var tvChannelHtcOne = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "HTC One M8 Android L 5.0 Lollipop",
                Sku = "M8_HTC_5L",
                ShortDescription = "HTC - One (M8) 4G LTE Cell SmartPhone with 32GB Memory - Gunmetal (Sprint)",
                FullDescription = "<p><b>HTC One (M8) Cell SmartPhone for Sprint:</b> With its brushed-metal design and wrap-around unibody frame, the HTC One (M8) is designed to fit beautifully in your hand. It's fun to use with amped up sound and a large Full HD touch screen, and intuitive gesture controls make it seem like your phone almost knows what you need before you do. <br /><br />Sprint Easy Pay option available in store.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "blackberry-bold-9000-phone-black-att",
                AllowUserReviews = true,
                Price = 245M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                ShowOnHomepage = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelHtcOne);

            await InsertInstallationDataAsync(tvChannelHtcOne);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelHtcOne.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Cell phones").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelHtcOne, "tvChannel_HTC_One_M8.jpeg");

            await AddTvChannelTagAsync(tvChannelHtcOne, "cell");
            await AddTvChannelTagAsync(tvChannelHtcOne, "compact");
            await AddTvChannelTagAsync(tvChannelHtcOne, "awesome");

            var tvChannelHtcOneMini = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "HTC One Mini Blue",
                Sku = "OM_HTC_BL",
                ShortDescription = "HTC One and HTC One Mini now available in bright blue hue",
                FullDescription = "<p>HTC One mini smartphone with 4.30-inch 720x1280 display powered by 1.4GHz processor alongside 1GB RAM and 4-Ultrapixel rear camera.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "samsung-rugby-a837-phone-black-att",
                AllowUserReviews = true,
                Price = 100M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelHtcOneMini);

            await InsertInstallationDataAsync(tvChannelHtcOneMini);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelHtcOneMini.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Cell phones").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelHtcOneMini, "tvChannel_HTC_One_Mini_1.jpeg");
            await InsertTvChannelPictureAsync(tvChannelHtcOneMini, "tvChannel_HTC_One_Mini_2.jpeg", 2);

            await AddTvChannelTagAsync(tvChannelHtcOneMini, "awesome");
            await AddTvChannelTagAsync(tvChannelHtcOneMini, "compact");
            await AddTvChannelTagAsync(tvChannelHtcOneMini, "cell");

            var tvChannelNokiaLumia = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Nokia Lumia 1020",
                Sku = "N_1020_LU",
                ShortDescription = "Nokia Lumia 1020 4G Cell SmartPhone (Unlocked)",
                FullDescription = "<p>Capture special moments for friends and family with this Nokia Lumia 1020 32GB WHITE cell phone that features an easy-to-use 41.0MP rear-facing camera and a 1.2MP front-facing camera. The AMOLED touch screen offers 768 x 1280 resolution for crisp visuals.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "sony-dcr-sr85-1mp-60gb-hard-drive-handycam-camcorder",
                AllowUserReviews = true,
                Price = 349M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelNokiaLumia);

            await InsertInstallationDataAsync(tvChannelNokiaLumia);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelNokiaLumia.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Cell phones").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelNokiaLumia, "tvChannel_Lumia1020.jpeg");

            await AddTvChannelTagAsync(tvChannelNokiaLumia, "awesome");
            await AddTvChannelTagAsync(tvChannelNokiaLumia, "cool");
            await AddTvChannelTagAsync(tvChannelNokiaLumia, "camera");

            var tvChannelBeatsPill = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Beats Pill 2.0 Wireless Speaker",
                Sku = "BP_20_WSP",
                ShortDescription = "<b>Pill 2.0 Portable Bluetooth Speaker (1-Piece):</b> Watch your favorite movies and listen to music with striking sound quality. This lightweight, portable speaker is easy to take with you as you travel to any destination, keeping you entertained wherever you are. ",
                FullDescription = "<ul><li>Pair and play with your Bluetooth® device with 30 foot range</li><li>Built-in speakerphone</li><li>7 hour rechargeable battery</li><li>Power your other devices with USB charge out</li><li>Tap two Beats Pills™ together for twice the sound with Beats Bond™</li></ul>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "acer-aspire-one-89-mini-notebook-case-black",
                AllowUserReviews = true,
                Price = 79.99M,
                IsShipEnabled = true,
                IsFreeShipping = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                HasTierPrices = true
            };
            allTvChannels.Add(tvChannelBeatsPill);

            await InsertInstallationDataAsync(tvChannelBeatsPill);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelBeatsPill.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Others").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelBeatsPill, "tvChannel_PillBeats_1.jpeg");
            await InsertTvChannelPictureAsync(tvChannelBeatsPill, "tvChannel_PillBeats_2.jpeg", 2);

            await InsertInstallationDataAsync(new List<TierPrice>
            {
                new TierPrice
                {
                    Quantity = 2,
                    Price = 19,
                    TvChannelId = tvChannelBeatsPill.Id
                },
                new TierPrice
                {
                    Quantity = 5,
                    Price = 17,
                    TvChannelId = tvChannelBeatsPill.Id
                },
                new TierPrice
                {
                    Quantity = 10,
                    Price = 15,
                    StartDateTimeUtc = DateTime.UtcNow.AddDays(-7),
                    EndDateTimeUtc = DateTime.UtcNow.AddDays(7),
                    TvChannelId = tvChannelBeatsPill.Id
                }
            });

            await AddTvChannelTagAsync(tvChannelBeatsPill, "computer");
            await AddTvChannelTagAsync(tvChannelBeatsPill, "cool");

            var tvChannelUniversalTabletCover = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Universal 7-8 Inch Tablet Cover",
                Sku = "TC_78I_UN",
                ShortDescription = "Universal protection for 7-inch & 8-inch tablets",
                FullDescription = "<p>Made of durable polyurethane, our Universal Cover is slim, lightweight, and strong, with protective corners that stretch to hold most 7 and 8-inch tablets securely. This tough case helps protects your tablet from bumps, scuffs, and dings.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "apc-back-ups-rs-800va-ups-800-va-ups-battery-lead-acid-br800blk",
                AllowUserReviews = true,
                Price = 39M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelUniversalTabletCover);

            await InsertInstallationDataAsync(tvChannelUniversalTabletCover);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelUniversalTabletCover.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Others").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelUniversalTabletCover, "tvChannel_TabletCover.jpeg");

            await AddTvChannelTagAsync(tvChannelUniversalTabletCover, "computer");
            await AddTvChannelTagAsync(tvChannelUniversalTabletCover, "cool");

            var tvChannelPortableSoundSpeakers = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Portable Sound Speakers",
                Sku = "PT_SPK_SN",
                ShortDescription = "Universall portable sound speakers",
                FullDescription = "<p>Your phone cut the cord, now it's time for you to set your music free and buy a Bluetooth speaker. Thankfully, there's one suited for everyone out there.</p><p>Some Bluetooth speakers excel at packing in as much functionality as the unit can handle while keeping the price down. Other speakers shuck excess functionality in favor of premium build materials instead. Whatever path you choose to go down, you'll be greeted with many options to suit your personal tastes.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "microsoft-bluetooth-notebook-mouse-5000-macwindows",
                AllowUserReviews = true,
                Price = 37M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Electronics & Software").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelPortableSoundSpeakers);

            await InsertInstallationDataAsync(tvChannelPortableSoundSpeakers);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelPortableSoundSpeakers.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Others").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelPortableSoundSpeakers, "tvChannel_Speakers.jpeg");

            relatedTvChannels.AddRange(new[]
            {
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelLeica.Id,
                     TvChannelId2 = tvChannelHtcOneMini.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelLeica.Id,
                     TvChannelId2 = tvChannelNikonD5500DSLR.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelLeica.Id,
                     TvChannelId2 = tvChannelAppleICam.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelLeica.Id,
                     TvChannelId2 = tvChannelNokiaLumia.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelHtcOne.Id,
                     TvChannelId2 = tvChannelHtcOneMini.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelHtcOne.Id,
                     TvChannelId2 = tvChannelNokiaLumia.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelHtcOne.Id,
                     TvChannelId2 = tvChannelBeatsPill.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelHtcOne.Id,
                     TvChannelId2 = tvChannelPortableSoundSpeakers.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelHtcOneMini.Id,
                     TvChannelId2 = tvChannelHtcOne.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelHtcOneMini.Id,
                     TvChannelId2 = tvChannelNokiaLumia.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelHtcOneMini.Id,
                     TvChannelId2 = tvChannelBeatsPill.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelHtcOneMini.Id,
                     TvChannelId2 = tvChannelPortableSoundSpeakers.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelNokiaLumia.Id,
                     TvChannelId2 = tvChannelHtcOne.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelNokiaLumia.Id,
                     TvChannelId2 = tvChannelHtcOneMini.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelNokiaLumia.Id,
                     TvChannelId2 = tvChannelBeatsPill.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelNokiaLumia.Id,
                     TvChannelId2 = tvChannelPortableSoundSpeakers.Id
                }
            });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallApparelAsync(TvChannelTemplate tvChannelTemplateSimple, List<TvChannel> allTvChannels, string sampleImagesPath, IPictureService pictureService, List<RelatedTvChannel> relatedTvChannels, TvChannelAvailabilityRange tvChannelAvailabilityRange)
        {
            var tvChannelNikeFloral = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Nike Floral Roshe Customized Running Shoes",
                Sku = "NK_FRC_RS",
                ShortDescription = "When you ran across these shoes, you will immediately fell in love and needed a pair of these customized beauties.",
                FullDescription = "<p>Each Rosh Run is personalized and exclusive, handmade in our workshop Custom. Run Your Rosh creations born from the hand of an artist specialized in sneakers, more than 10 years of experience.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "adidas-womens-supernova-csh-7-running-shoe",
                AllowUserReviews = true,
                Price = 40M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelNikeFloral);

            await InsertInstallationDataAsync(tvChannelNikeFloral);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelNikeFloral.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Shoes").Id,
                DisplayOrder = 1
            });

            await InsertInstallationDataAsync(new TvChannelManufacturer
            {
                TvChannelId = tvChannelNikeFloral.Id,
                ManufacturerId = _manufacturerRepository.Table.Single(c => c.Name == "Nike").Id,
                DisplayOrder = 2
            });

            var picTvChannelNikeFloralShoe1Id = await InsertTvChannelPictureAsync(tvChannelNikeFloral, "tvChannel_NikeFloralShoe_1.jpg");
            var picTvChannelNikeFloralShoe2Id = await InsertTvChannelPictureAsync(tvChannelNikeFloral, "tvChannel_NikeFloralShoe_2.jpg", 2);

            await InsertInstallationDataAsync(new TvChannelSpecificationAttribute
            {
                TvChannelId = tvChannelNikeFloral.Id,
                AllowFiltering = true,
                ShowOnTvChannelPage = false,
                DisplayOrder = 1,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Color", "Grey")
            });

            var pamSize = await InsertInstallationDataAsync(
                new TvChannelAttributeMapping
                {
                    TvChannelId = tvChannelNikeFloral.Id,
                    TvChannelAttributeId = _tvChannelAttributeRepository.Table.Single(x => x.Name == "Size").Id,
                    AttributeControlType = AttributeControlType.DropdownList,
                    IsRequired = true
                });

            await InsertInstallationDataAsync(
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "8",
                    DisplayOrder = 1
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "9",
                    DisplayOrder = 2
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "10",
                    DisplayOrder = 3
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "11",
                    DisplayOrder = 4
                });

            var pamColor = await InsertInstallationDataAsync(
                new TvChannelAttributeMapping
                {
                    TvChannelId = tvChannelNikeFloral.Id,
                    TvChannelAttributeId = _tvChannelAttributeRepository.Table.Single(x => x.Name == "Color").Id,
                    AttributeControlType = AttributeControlType.DropdownList,
                    IsRequired = true
                });

            await InsertInstallationDataAsync(
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamColor.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "White/Blue",
                    DisplayOrder = 1
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamColor.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "White/Black",
                    DisplayOrder = 2
                });

            var pamPrint = await InsertInstallationDataAsync(
                new TvChannelAttributeMapping
                {
                    TvChannelId = tvChannelNikeFloral.Id,
                    TvChannelAttributeId = _tvChannelAttributeRepository.Table.Single(x => x.Name == "Print").Id,
                    AttributeControlType = AttributeControlType.ImageSquares,
                    IsRequired = true
                });

            await InsertInstallationDataAsync(
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamPrint.Id,
                    PictureId = picTvChannelNikeFloralShoe1Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "Natural",
                    DisplayOrder = 1,
                    ImageSquaresPictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "p_attribute_print_2.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Natural Print"))).Id
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamPrint.Id,
                    PictureId = picTvChannelNikeFloralShoe2Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "Fresh",
                    DisplayOrder = 2,
                    ImageSquaresPictureId = (await pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(_fileProvider.Combine(sampleImagesPath, "p_attribute_print_1.jpg")), MimeTypes.ImagePJpeg, await pictureService.GetPictureSeNameAsync("Fresh Print"))).Id
                });

            await AddTvChannelTagAsync(tvChannelNikeFloral, "cool");
            await AddTvChannelTagAsync(tvChannelNikeFloral, "shoes");
            await AddTvChannelTagAsync(tvChannelNikeFloral, "apparel");

            await UpdateInstallationDataAsync(tvChannelNikeFloral);

            var tvChannelAdidas = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "adidas Consortium Campus 80s Running Shoes",
                Sku = "AD_C80_RS",
                ShortDescription = "adidas Consortium Campus 80s Primeknit Light Maroon/Running Shoes",
                FullDescription = "<p>One of three colorways of the adidas Consortium Campus 80s Primeknit set to drop alongside each other. This pair comes in light maroon and running white. Featuring a maroon-based primeknit upper with white accents. A limited release, look out for these at select adidas Consortium accounts worldwide.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "etnies-mens-digit-sneaker",
                AllowUserReviews = true,
                Price = 27.56M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                //ShowOnHomepage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelAdidas);

            await InsertInstallationDataAsync(tvChannelAdidas);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelAdidas.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Shoes").Id,
                DisplayOrder = 1
            });

            var picTvChannelAdidasId = await InsertTvChannelPictureAsync(tvChannelAdidas, "tvChannel_adidas.jpg");
            var picTvChannelAdidas2Id = await InsertTvChannelPictureAsync(tvChannelAdidas, "tvChannel_adidas_2.jpg", 2);
            var picTvChannelAdidas3Id = await InsertTvChannelPictureAsync(tvChannelAdidas, "tvChannel_adidas_3.jpg", 3);

            await InsertInstallationDataAsync(
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelAdidas.Id,
                    AllowFiltering = true,
                    ShowOnTvChannelPage = false,
                    DisplayOrder = 1,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Color", "Grey")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelAdidas.Id,
                    AllowFiltering = true,
                    ShowOnTvChannelPage = false,
                    DisplayOrder = 2,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Color", "Red")
                },
                new TvChannelSpecificationAttribute
                {
                    TvChannelId = tvChannelAdidas.Id,
                    AllowFiltering = true,
                    ShowOnTvChannelPage = false,
                    DisplayOrder = 3,
                    SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Color", "Blue")
                });

            var pamAdidasSize = await InsertInstallationDataAsync(
                new TvChannelAttributeMapping
                {
                    TvChannelId = tvChannelAdidas.Id,
                    TvChannelAttributeId = _tvChannelAttributeRepository.Table.Single(x => x.Name == "Size").Id,
                    AttributeControlType = AttributeControlType.DropdownList,
                    IsRequired = true
                });

            await InsertInstallationDataAsync(
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamAdidasSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "8",
                    DisplayOrder = 1
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamAdidasSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "9",
                    DisplayOrder = 2
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamAdidasSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "10",
                    DisplayOrder = 3
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamAdidasSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "11",
                    DisplayOrder = 4
                });

            var pamAdidasColor = await InsertInstallationDataAsync(
                new TvChannelAttributeMapping
                {
                    TvChannelId = tvChannelAdidas.Id,
                    TvChannelAttributeId = _tvChannelAttributeRepository.Table.Single(x => x.Name == "Color").Id,
                    AttributeControlType = AttributeControlType.ColorSquares,
                    IsRequired = true
                });

            await InsertInstallationDataAsync(
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamAdidasColor.Id,
                    PictureId = picTvChannelAdidasId,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "Red",
                    IsPreSelected = true,
                    ColorSquaresRgb = "#663030",
                    DisplayOrder = 1
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamAdidasColor.Id,
                    PictureId = picTvChannelAdidas2Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "Blue",
                    ColorSquaresRgb = "#363656",
                    DisplayOrder = 2
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamAdidasColor.Id,
                    PictureId = picTvChannelAdidas3Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "Silver",
                    ColorSquaresRgb = "#c5c5d5",
                    DisplayOrder = 3
                });

            await AddTvChannelTagAsync(tvChannelAdidas, "cool");
            await AddTvChannelTagAsync(tvChannelAdidas, "shoes");
            await AddTvChannelTagAsync(tvChannelAdidas, "apparel");

            await UpdateInstallationDataAsync(tvChannelAdidas);

            var tvChannelNikeZoom = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Nike SB Zoom Stefan Janoski \"Medium Mint\"",
                Sku = "NK_ZSJ_MM",
                ShortDescription = "Nike SB Zoom Stefan Janoski Dark Grey Medium Mint Teal ...",
                FullDescription = "The newly Nike SB Zoom Stefan Janoski gets hit with a \"Medium Mint\" accents that sits atop a Dark Grey suede. Expected to drop in October.",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "v-blue-juniors-cuffed-denim-short-with-rhinestones",
                AllowUserReviews = true,
                Price = 30M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            allTvChannels.Add(tvChannelNikeZoom);

            await InsertInstallationDataAsync(tvChannelNikeZoom);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelNikeZoom.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Shoes").Id,
                DisplayOrder = 1
            });

            await InsertInstallationDataAsync(new TvChannelManufacturer
            {
                TvChannelId = tvChannelNikeZoom.Id,
                ManufacturerId = _manufacturerRepository.Table.Single(c => c.Name == "Nike").Id,
                DisplayOrder = 2
            });

            await InsertTvChannelPictureAsync(tvChannelNikeZoom, "tvChannel_NikeZoom.jpg");

            await InsertInstallationDataAsync(new TvChannelSpecificationAttribute
            {
                TvChannelId = tvChannelNikeZoom.Id,
                AllowFiltering = true,
                ShowOnTvChannelPage = false,
                DisplayOrder = 1,
                SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync("Color", "Grey")
            });

            await AddTvChannelTagAsync(tvChannelNikeZoom, "jeans");
            await AddTvChannelTagAsync(tvChannelNikeZoom, "cool");
            await AddTvChannelTagAsync(tvChannelNikeZoom, "apparel");

            var tvChannelNikeTailwind = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Nike Tailwind Loose Short-Sleeve Running Shirt",
                Sku = "NK_TLS_RS",
                ShortDescription = string.Empty,
                FullDescription = "<p>Boost your adrenaline with the Nike® Women's Tailwind Running Shirt. The lightweight, slouchy fit is great for layering, and moisture-wicking fabrics keep you feeling at your best. This tee has a notched hem for an enhanced range of motion, while flat seams with reinforcement tape lessen discomfort and irritation over longer distances. Put your keys and card in the side zip pocket and take off in your Nike® running t-shirt.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "50s-rockabilly-polka-dot-top-jr-plus-size",
                AllowUserReviews = true,
                Published = true,
                Price = 15M,
                IsShipEnabled = true,
                Weight = 1,
                Length = 2,
                Width = 3,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelNikeTailwind);

            await InsertInstallationDataAsync(tvChannelNikeTailwind);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelNikeTailwind.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Clothing").Id,
                DisplayOrder = 1
            });

            await InsertInstallationDataAsync(new TvChannelManufacturer
            {
                TvChannelId = tvChannelNikeTailwind.Id,
                ManufacturerId = _manufacturerRepository.Table.Single(c => c.Name == "Nike").Id,
                DisplayOrder = 2
            });

            await InsertTvChannelPictureAsync(tvChannelNikeTailwind, "tvChannel_NikeShirt.jpg");

            var pamNikeSize = await InsertInstallationDataAsync(
                new TvChannelAttributeMapping
                {
                    TvChannelId = tvChannelNikeTailwind.Id,
                    TvChannelAttributeId = _tvChannelAttributeRepository.Table.Single(x => x.Name == "Size").Id,
                    AttributeControlType = AttributeControlType.DropdownList,
                    IsRequired = true
                });

            await InsertInstallationDataAsync(
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamNikeSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "Small",
                    DisplayOrder = 1
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamNikeSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "1X",
                    DisplayOrder = 2
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamNikeSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "2X",
                    DisplayOrder = 3
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamNikeSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "3X",
                    DisplayOrder = 4
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamNikeSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "4X",
                    DisplayOrder = 5
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamNikeSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "5X",
                    DisplayOrder = 6
                });

            await AddTvChannelTagAsync(tvChannelNikeTailwind, "cool");
            await AddTvChannelTagAsync(tvChannelNikeTailwind, "apparel");
            await AddTvChannelTagAsync(tvChannelNikeTailwind, "shirt");

            var tvChannelOversizedWomenTShirt = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Oversized Women T-Shirt",
                Sku = "WM_OVR_TS",
                ShortDescription = string.Empty,
                FullDescription = "<p>This oversized women t-Shirt needs minimum ironing. It is a great tvChannel at a great value!</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "arrow-mens-wrinkle-free-pinpoint-solid-long-sleeve",
                AllowUserReviews = true,
                Price = 24M,
                IsShipEnabled = true,
                Weight = 4,
                Length = 3,
                Width = 3,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                HasTierPrices = true
            };

            allTvChannels.Add(tvChannelOversizedWomenTShirt);

            await InsertInstallationDataAsync(tvChannelOversizedWomenTShirt);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelOversizedWomenTShirt.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Clothing").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelOversizedWomenTShirt, "tvChannel_WomenTShirt.jpg");

            await InsertInstallationDataAsync(new List<TierPrice>
            {
                new TierPrice
                {
                    Quantity = 3,
                    Price = 21,
                    TvChannelId = tvChannelOversizedWomenTShirt.Id
                },
                new TierPrice
                {
                    Quantity = 7,
                    Price = 19,
                    TvChannelId = tvChannelOversizedWomenTShirt.Id
                },
                new TierPrice
                {
                    Quantity = 10,
                    Price = 16,
                    TvChannelId = tvChannelOversizedWomenTShirt.Id
                }
            });

            await AddTvChannelTagAsync(tvChannelOversizedWomenTShirt, "cool");
            await AddTvChannelTagAsync(tvChannelOversizedWomenTShirt, "apparel");
            await AddTvChannelTagAsync(tvChannelOversizedWomenTShirt, "shirt");

            var tvChannelCustomTShirt = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Custom T-Shirt",
                Sku = "CS_TSHIRT",
                ShortDescription = "T-Shirt - Add Your Content",
                FullDescription = "<p>Comfort comes in all shapes and forms, yet this tee out does it all. Rising above the rest, our classic cotton crew provides the simple practicality you need to make it through the day. Tag-free, relaxed fit wears well under dress shirts or stands alone in laid-back style. Reinforced collar and lightweight feel give way to long-lasting shape and breathability. One less thing to worry about, rely on this tee to provide comfort and ease with every wear.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "custom-t-shirt",
                AllowUserReviews = true,
                Price = 15M,
                IsShipEnabled = true,
                Weight = 4,
                Length = 3,
                Width = 3,
                Height = 3,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelCustomTShirt);

            await InsertInstallationDataAsync(tvChannelCustomTShirt);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelCustomTShirt.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Clothing").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelCustomTShirt, "tvChannel_CustomTShirt.jpeg");

            await InsertInstallationDataAsync(
                new TvChannelAttributeMapping
                {
                    TvChannelId = tvChannelCustomTShirt.Id,
                    TvChannelAttributeId = _tvChannelAttributeRepository.Table.Single(x => x.Name == "Custom Text").Id,
                    TextPrompt = "Enter your text:",
                    AttributeControlType = AttributeControlType.TextBox,
                    IsRequired = true
                });

            await AddTvChannelTagAsync(tvChannelCustomTShirt, "cool");
            await AddTvChannelTagAsync(tvChannelCustomTShirt, "shirt");
            await AddTvChannelTagAsync(tvChannelCustomTShirt, "apparel");

            var tvChannelLeviJeans = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Levi's 511 Jeans",
                Sku = "LV_511_JN",
                ShortDescription = "Levi's Faded Black 511 Jeans ",
                FullDescription = "<p>Between a skinny and straight fit, our 511&trade; slim fit jeans are cut close without being too restricting. Slim throughout the thigh and leg opening for a long and lean look.</p><ul><li>Slouch1y at top; sits below the waist</li><li>Slim through the leg, close at the thigh and straight to the ankle</li><li>Stretch for added comfort</li><li>Classic five-pocket styling</li><li>99% Cotton, 1% Spandex, 11.2 oz. - Imported</li></ul>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "levis-skinny-511-jeans",
                AllowUserReviews = true,
                Price = 43.5M,
                OldPrice = 55M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                HasTierPrices = true
            };
            allTvChannels.Add(tvChannelLeviJeans);

            await InsertInstallationDataAsync(tvChannelLeviJeans);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelLeviJeans.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Clothing").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelLeviJeans, "tvChannel_LeviJeans_1.jpg");
            await InsertTvChannelPictureAsync(tvChannelLeviJeans, "tvChannel_LeviJeans_2.jpg", 2);

            await InsertInstallationDataAsync(new List<TierPrice>
            {
                new TierPrice
                {
                    Quantity = 3,
                    Price = 40,
                    TvChannelId = tvChannelLeviJeans.Id
                },
                new TierPrice
                {
                    Quantity = 6,
                    Price = 38,
                    TvChannelId = tvChannelLeviJeans.Id
                },
                new TierPrice
                {
                    Quantity = 10,
                    Price = 35,
                    TvChannelId = tvChannelLeviJeans.Id
                }
            });

            await AddTvChannelTagAsync(tvChannelLeviJeans, "cool");
            await AddTvChannelTagAsync(tvChannelLeviJeans, "jeans");
            await AddTvChannelTagAsync(tvChannelLeviJeans, "apparel");

            var tvChannelObeyHat = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Obey Propaganda Hat",
                Sku = "OB_HAT_PR",
                ShortDescription = string.Empty,
                FullDescription = "<p>Printed poplin 5 panel camp hat with debossed leather patch and web closure</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "indiana-jones-shapeable-wool-hat",
                AllowUserReviews = true,
                Price = 30M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelObeyHat);

            await InsertInstallationDataAsync(tvChannelObeyHat);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelObeyHat.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Accessories").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelObeyHat, "tvChannel_hat.jpg");

            var pamObeyHatSize = await InsertInstallationDataAsync(
                new TvChannelAttributeMapping
                {
                    TvChannelId = tvChannelObeyHat.Id,
                    TvChannelAttributeId = _tvChannelAttributeRepository.Table.Single(x => x.Name == "Size").Id,
                    AttributeControlType = AttributeControlType.DropdownList,
                    IsRequired = true
                });

            await InsertInstallationDataAsync(
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamObeyHatSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "Small",
                    DisplayOrder = 1
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamObeyHatSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "Medium",
                    DisplayOrder = 2
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamObeyHatSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "Large",
                    DisplayOrder = 3
                },
                new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = pamObeyHatSize.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = "X-Large",
                    DisplayOrder = 4
                });

            await AddTvChannelTagAsync(tvChannelObeyHat, "apparel");
            await AddTvChannelTagAsync(tvChannelObeyHat, "cool");

            var tvChannelBelt = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Reversible Horseferry Check Belt",
                Sku = "RH_CHK_BL",
                ShortDescription = "Reversible belt in Horseferry check with smooth leather trim",
                FullDescription = "<p>Reversible belt in Horseferry check with smooth leather trim</p><p>Leather lining, polished metal buckle</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "nike-golf-casual-belt",
                AllowUserReviews = true,
                Price = 45M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                TvChannelAvailabilityRangeId = tvChannelAvailabilityRange.Id,
                StockQuantity = 0,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelBelt);

            await InsertInstallationDataAsync(tvChannelBelt);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelBelt.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Accessories").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelBelt, "tvChannel_Belt.jpeg");

            var tvChannelSunglasses = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Ray Ban Aviator Sunglasses",
                Sku = "RB_AVR_SG",
                ShortDescription = "Aviator sunglasses are one of the first widely popularized styles of modern day sunwear.",
                FullDescription = "<p>Since 1937, Ray-Ban can genuinely claim the title as the world's leading sunglasses and optical eyewear brand. Combining the best of fashion and sports performance, the Ray-Ban line of Sunglasses delivers a truly classic style that will have you looking great today and for years to come.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "ray-ban-aviator-sunglasses-rb-3025",
                AllowUserReviews = true,
                Price = 25M,
                IsShipEnabled = true,
                Weight = 7,
                Length = 7,
                Width = 7,
                Height = 7,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Apparel").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelSunglasses);

            await InsertInstallationDataAsync(tvChannelSunglasses);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelSunglasses.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Accessories").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelSunglasses, "tvChannel_Sunglasses.jpg");

            await AddTvChannelTagAsync(tvChannelSunglasses, "apparel");
            await AddTvChannelTagAsync(tvChannelSunglasses, "cool");

            relatedTvChannels.AddRange(new[]
            {
                 new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelAdidas.Id,
                     TvChannelId2 = tvChannelLeviJeans.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelAdidas.Id,
                     TvChannelId2 = tvChannelNikeFloral.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelAdidas.Id,
                     TvChannelId2 = tvChannelNikeZoom.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelAdidas.Id,
                     TvChannelId2 = tvChannelNikeTailwind.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelLeviJeans.Id,
                     TvChannelId2 = tvChannelAdidas.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelLeviJeans.Id,
                     TvChannelId2 = tvChannelNikeFloral.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelLeviJeans.Id,
                     TvChannelId2 = tvChannelNikeZoom.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelLeviJeans.Id,
                     TvChannelId2 = tvChannelNikeTailwind.Id
                },

                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelCustomTShirt.Id,
                     TvChannelId2 = tvChannelLeviJeans.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelCustomTShirt.Id,
                     TvChannelId2 = tvChannelNikeTailwind.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelCustomTShirt.Id,
                     TvChannelId2 = tvChannelOversizedWomenTShirt.Id
                },
                new RelatedTvChannel
                {
                     TvChannelId1 = tvChannelCustomTShirt.Id,
                     TvChannelId2 = tvChannelObeyHat.Id
                }
            });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallDigitalDownloadsAsync(TvChannelTemplate tvChannelTemplateSimple, List<TvChannel> allTvChannels, string sampleImagesPath, IPictureService pictureService, List<RelatedTvChannel> relatedTvChannels, string sampleDownloadsPath, IDownloadService downloadService)
        {
            var downloadNightVision1 = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = MimeTypes.ApplicationXZipCo,
                DownloadBinary = await _fileProvider.ReadAllBytesAsync(sampleDownloadsPath + "tvChannel_NightVision_1.zip"),
                Extension = ".zip",
                Filename = "Night_Vision_1",
                IsNew = true
            };
            await downloadService.InsertDownloadAsync(downloadNightVision1);
            var downloadNightVision2 = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = MimeTypes.TextPlain,
                DownloadBinary = await _fileProvider.ReadAllBytesAsync(sampleDownloadsPath + "tvChannel_NightVision_2.txt"),
                Extension = ".txt",
                Filename = "Night_Vision_1",
                IsNew = true
            };
            await downloadService.InsertDownloadAsync(downloadNightVision2);
            var tvChannelNightVision = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Night Visions",
                Sku = "NIGHT_VSN",
                ShortDescription = "Night Visions is the debut studio album by American rock band Imagine Dragons.",
                FullDescription = "<p>Original Release Date: September 4, 2012</p><p>Release Date: September 4, 2012</p><p>Genre - Alternative rock, indie rock, electronic rock</p><p>Label - Interscope/KIDinaKORNER</p><p>Copyright: (C) 2011 Interscope Records</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "poker-face",
                AllowUserReviews = true,
                Price = 2.8M,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Downloadable TvChannels").Id,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                IsDownload = true,
                DownloadId = downloadNightVision1.Id,
                DownloadActivationType = DownloadActivationType.WhenOrderIsPaid,
                UnlimitedDownloads = true,
                HasUserAgreement = false,
                HasSampleDownload = true,
                SampleDownloadId = downloadNightVision2.Id,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelNightVision);

            await InsertInstallationDataAsync(tvChannelNightVision);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelNightVision.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Digital downloads").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelNightVision, "tvChannel_NightVisions.jpeg");

            await AddTvChannelTagAsync(tvChannelNightVision, "awesome");
            await AddTvChannelTagAsync(tvChannelNightVision, "digital");

            var downloadIfYouWait1 = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = MimeTypes.ApplicationXZipCo,
                DownloadBinary = await _fileProvider.ReadAllBytesAsync(sampleDownloadsPath + "tvChannel_IfYouWait_1.zip"),
                Extension = ".zip",
                Filename = "If_You_Wait_1",
                IsNew = true
            };
            await downloadService.InsertDownloadAsync(downloadIfYouWait1);
            var downloadIfYouWait2 = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = MimeTypes.TextPlain,
                DownloadBinary = await _fileProvider.ReadAllBytesAsync(sampleDownloadsPath + "tvChannel_IfYouWait_2.txt"),
                Extension = ".txt",
                Filename = "If_You_Wait_1",
                IsNew = true
            };
            await downloadService.InsertDownloadAsync(downloadIfYouWait2);
            var tvChannelIfYouWait = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "If You Wait (donation)",
                Sku = "IF_YOU_WT",
                ShortDescription = "If You Wait is the debut studio album by English indie pop band London Grammar",
                FullDescription = "<p>Original Release Date: September 6, 2013</p><p>Genre - Electronica, dream pop downtempo, pop</p><p>Label - Metal & Dust/Ministry of Sound</p><p>Producer - Tim Bran, Roy Kerr London, Grammar</p><p>Length - 43:22</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "single-ladies-put-a-ring-on-it",
                UserEntersPrice = true,
                MinimumUserEnteredPrice = 0.5M,
                MaximumUserEnteredPrice = 100M,
                AllowUserReviews = true,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Downloadable TvChannels").Id,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                IsDownload = true,
                DownloadId = downloadIfYouWait1.Id,
                DownloadActivationType = DownloadActivationType.WhenOrderIsPaid,
                UnlimitedDownloads = true,
                HasUserAgreement = false,
                HasSampleDownload = true,
                SampleDownloadId = downloadIfYouWait2.Id,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelIfYouWait);

            await InsertInstallationDataAsync(tvChannelIfYouWait);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelIfYouWait.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Digital downloads").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelIfYouWait, "tvChannel_IfYouWait.jpeg");

            await AddTvChannelTagAsync(tvChannelIfYouWait, "digital");
            await AddTvChannelTagAsync(tvChannelIfYouWait, "awesome");

            var downloadScienceAndFaith = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                ContentType = MimeTypes.ApplicationXZipCo,
                DownloadBinary = await _fileProvider.ReadAllBytesAsync(sampleDownloadsPath + "tvChannel_ScienceAndFaith_1.zip"),
                Extension = ".zip",
                Filename = "Science_And_Faith",
                IsNew = true
            };
            await downloadService.InsertDownloadAsync(downloadScienceAndFaith);
            var tvChannelScienceAndFaith = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Science & Faith",
                Sku = "SCI_FAITH",
                ShortDescription = "Science & Faith is the second studio album by Irish pop rock band The Script.",
                FullDescription = "<p># Original Release Date: September 10, 2010<br /># Label: RCA, Epic/Phonogenic(America)<br /># Copyright: 2010 RCA Records.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "the-battle-of-los-angeles",
                AllowUserReviews = true,
                UserEntersPrice = true,
                MinimumUserEnteredPrice = 0.5M,
                MaximumUserEnteredPrice = 1000M,
                Price = decimal.Zero,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Downloadable TvChannels").Id,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                IsDownload = true,
                DownloadId = downloadScienceAndFaith.Id,
                DownloadActivationType = DownloadActivationType.WhenOrderIsPaid,
                UnlimitedDownloads = true,
                HasUserAgreement = false,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelScienceAndFaith);

            await InsertInstallationDataAsync(tvChannelScienceAndFaith);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelScienceAndFaith.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Digital downloads").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelScienceAndFaith, "tvChannel_ScienceAndFaith.jpeg");

            await AddTvChannelTagAsync(tvChannelScienceAndFaith, "digital");
            await AddTvChannelTagAsync(tvChannelScienceAndFaith, "awesome");

            relatedTvChannels.AddRange(new[]
            {
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelIfYouWait.Id,
                    TvChannelId2 = tvChannelNightVision.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelIfYouWait.Id,
                    TvChannelId2 = tvChannelScienceAndFaith.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelNightVision.Id,
                    TvChannelId2 = tvChannelIfYouWait.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelNightVision.Id,
                    TvChannelId2 = tvChannelScienceAndFaith.Id
                }
            });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallBooksAsync(TvChannelTemplate tvChannelTemplateSimple, List<TvChannel> allTvChannels, string sampleImagesPath, IPictureService pictureService, List<RelatedTvChannel> relatedTvChannels)
        {
            var tvChannelFahrenheit = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Fahrenheit 451 by Ray Bradbury",
                Sku = "FR_451_RB",
                ShortDescription = "Fahrenheit 451 is a dystopian novel by Ray Bradbury published in 1953. It is regarded as one of his best works.",
                FullDescription = "<p>The novel presents a future American society where books are outlawed and firemen burn any that are found. The title refers to the temperature that Bradbury understood to be the autoignition point of paper.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "best-grilling-recipes",
                AllowUserReviews = true,
                Price = 27M,
                OldPrice = 30M,
                IsShipEnabled = true,
                IsFreeShipping = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Books").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelFahrenheit);

            await InsertInstallationDataAsync(tvChannelFahrenheit);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelFahrenheit.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Books").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelFahrenheit, "tvChannel_Fahrenheit451.jpeg");

            await AddTvChannelTagAsync(tvChannelFahrenheit, "awesome");
            await AddTvChannelTagAsync(tvChannelFahrenheit, "book");
            await AddTvChannelTagAsync(tvChannelFahrenheit, "nice");

            var tvChannelFirstPrizePies = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "First Prize Pies",
                Sku = "FIRST_PRP",
                ShortDescription = "Allison Kave made pies as a hobby, until one day her boyfriend convinced her to enter a Brooklyn pie-making contest. She won. In fact, her pies were such a hit that she turned pro.",
                FullDescription = "<p>First Prize Pies, a boutique, made-to-order pie business that originated on New York's Lower East Side, has become synonymous with tempting and unusual confections. For the home baker who is passionate about seasonal ingredients and loves a creative approach to recipes, First Prize Pies serves up 52 weeks of seasonal and eclectic pastries in an interesting pie-a-week format. Clear instructions, technical tips and creative encouragement guide novice bakers as well as pie mavens. With its nostalgia-evoking photos of homemade pies fresh out of the oven, First Prize Pies will be as giftable as it is practical.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "eatingwell-in-season",
                AllowUserReviews = true,
                Price = 51M,
                OldPrice = 67M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Books").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelFirstPrizePies);

            await InsertInstallationDataAsync(tvChannelFirstPrizePies);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelFirstPrizePies.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Books").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelFirstPrizePies, "tvChannel_FirstPrizePies.jpeg");

            await AddTvChannelTagAsync(tvChannelFirstPrizePies, "book");

            var tvChannelPrideAndPrejudice = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Pride and Prejudice",
                Sku = "PRIDE_PRJ",
                ShortDescription = "Pride and Prejudice is a novel of manners by Jane Austen, first published in 1813.",
                FullDescription = "<p>Set in England in the early 19th century, Pride and Prejudice tells the story of Mr and Mrs Bennet's five unmarried daughters after the rich and eligible Mr Bingley and his status-conscious friend, Mr Darcy, have moved into their neighbourhood. While Bingley takes an immediate liking to the eldest Bennet daughter, Jane, Darcy has difficulty adapting to local society and repeatedly clashes with the second-eldest Bennet daughter, Elizabeth.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "the-best-skillet-recipes",
                AllowUserReviews = true,
                Price = 24M,
                OldPrice = 35M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Books").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelPrideAndPrejudice);

            await InsertInstallationDataAsync(tvChannelPrideAndPrejudice);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelPrideAndPrejudice.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Books").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelPrideAndPrejudice, "tvChannel_PrideAndPrejudice.jpeg");

            await AddTvChannelTagAsync(tvChannelPrideAndPrejudice, "book");

            relatedTvChannels.AddRange(new[]
            {
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelPrideAndPrejudice.Id,
                    TvChannelId2 = tvChannelFirstPrizePies.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelPrideAndPrejudice.Id,
                    TvChannelId2 = tvChannelFahrenheit.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelFirstPrizePies.Id,
                    TvChannelId2 = tvChannelPrideAndPrejudice.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelFirstPrizePies.Id,
                    TvChannelId2 = tvChannelFahrenheit.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelFahrenheit.Id,
                    TvChannelId2 = tvChannelFirstPrizePies.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelFahrenheit.Id,
                    TvChannelId2 = tvChannelPrideAndPrejudice.Id
                }
            });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallJewelryAsync(TvChannelTemplate tvChannelTemplateSimple, List<TvChannel> allTvChannels, string sampleImagesPath, IPictureService pictureService, List<RelatedTvChannel> relatedTvChannels)
        {
            var tvChannelElegantGemstoneNecklace = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Elegant Gemstone Necklace (rental)",
                Sku = "EG_GEM_NL",
                ShortDescription = "Classic and elegant gemstone necklace now available in our store",
                FullDescription = "<p>For those who like jewelry, creating their ownelegant jewelry from gemstone beads provides an economical way to incorporate genuine gemstones into your jewelry wardrobe. Manufacturers create beads from all kinds of precious gemstones and semi-precious gemstones, which are available in bead shops, craft stores, and online marketplaces.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "diamond-pave-earrings",
                AllowUserReviews = true,
                IsRental = true,
                RentalPriceLength = 1,
                RentalPricePeriod = RentalPricePeriod.Days,
                Price = 30M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Jewelry").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelElegantGemstoneNecklace);

            await InsertInstallationDataAsync(tvChannelElegantGemstoneNecklace);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelElegantGemstoneNecklace.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Jewelry").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelElegantGemstoneNecklace, "tvChannel_GemstoneNecklaces.jpg");

            await AddTvChannelTagAsync(tvChannelElegantGemstoneNecklace, "jewelry");
            await AddTvChannelTagAsync(tvChannelElegantGemstoneNecklace, "awesome");

            var tvChannelFlowerGirlBracelet = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Flower Girl Bracelet",
                Sku = "FL_GIRL_B",
                ShortDescription = "Personalised Flower Braceled",
                FullDescription = "<p>This is a great gift for your flower girl to wear on your wedding day. A delicate bracelet that is made with silver plated soldered cable chain, gives this bracelet a dainty look for young wrist. A Swarovski heart, shown in Rose, hangs off a silver plated flower. Hanging alongside the heart is a silver plated heart charm with Flower Girl engraved on both sides. This is a great style for the younger flower girl.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "diamond-tennis-bracelet",
                AllowUserReviews = true,
                Price = 360M,
                IsShipEnabled = true,
                IsFreeShipping = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Jewelry").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelFlowerGirlBracelet);

            await InsertInstallationDataAsync(tvChannelFlowerGirlBracelet);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelFlowerGirlBracelet.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Jewelry").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelFlowerGirlBracelet, "tvChannel_FlowerBracelet.jpg");

            await AddTvChannelTagAsync(tvChannelFlowerGirlBracelet, "awesome");
            await AddTvChannelTagAsync(tvChannelFlowerGirlBracelet, "jewelry");

            var tvChannelEngagementRing = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "Vintage Style Engagement Ring",
                Sku = "VS_ENG_RN",
                ShortDescription = "1.24 Carat (ctw) in 14K White Gold (Certified)",
                FullDescription = "<p>Dazzle her with this gleaming 14 karat white gold vintage proposal. A ravishing collection of 11 decadent diamonds come together to invigorate a superbly ornate gold shank. Total diamond weight on this antique style engagement ring equals 1 1/4 carat (ctw). Item includes diamond certificate.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "vintage-style-three-stone-diamond-engagement-ring",
                AllowUserReviews = true,
                Price = 2100M,
                IsShipEnabled = true,
                Weight = 2,
                Length = 2,
                Width = 2,
                Height = 2,
                TaxCategoryId = _taxCategoryRepository.Table.Single(tc => tc.Name == "Jewelry").Id,
                ManageInventoryMethod = ManageInventoryMethod.ManageStock,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                DisplayStockAvailability = true,
                LowStockActivity = LowStockActivity.DisableBuyButton,
                BackorderMode = BackorderMode.NoBackorders,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannelEngagementRing);

            await InsertInstallationDataAsync(tvChannelEngagementRing);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannelEngagementRing.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Jewelry").Id,
                DisplayOrder = 1
            });

            await InsertTvChannelPictureAsync(tvChannelEngagementRing, "tvChannel_EngagementRing_1.jpg");

            await AddTvChannelTagAsync(tvChannelEngagementRing, "jewelry");
            await AddTvChannelTagAsync(tvChannelEngagementRing, "awesome");

            relatedTvChannels.AddRange(new[]
            {
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelFlowerGirlBracelet.Id,
                    TvChannelId2 = tvChannelEngagementRing.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelFlowerGirlBracelet.Id,
                    TvChannelId2 = tvChannelElegantGemstoneNecklace.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelEngagementRing.Id,
                    TvChannelId2 = tvChannelFlowerGirlBracelet.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelEngagementRing.Id,
                    TvChannelId2 = tvChannelElegantGemstoneNecklace.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelElegantGemstoneNecklace.Id,
                    TvChannelId2 = tvChannelFlowerGirlBracelet.Id
                },
                new RelatedTvChannel
                {
                    TvChannelId1 = tvChannelElegantGemstoneNecklace.Id,
                    TvChannelId2 = tvChannelEngagementRing.Id
                }
            });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallGiftCardsAsync(TvChannelTemplate tvChannelTemplateSimple, List<TvChannel> allTvChannels, string sampleImagesPath, IPictureService pictureService, List<RelatedTvChannel> relatedTvChannels, DeliveryDate deliveryDate)
        {
            var tvChannel25GiftCard = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "$25 Virtual Gift Card",
                Sku = "VG_CR_025",
                ShortDescription = "$25 Gift Card. Gift Cards must be redeemed through our site Web site toward the purchase of eligible tvChannels.",
                FullDescription = "<p>Gift Cards must be redeemed through our site Web site toward the purchase of eligible tvChannels. Purchases are deducted from the GiftCard balance. Any unused balance will be placed in the recipient's GiftCard account when redeemed. If an order exceeds the amount of the GiftCard, the balance must be paid with a credit card or other available payment method.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "25-virtual-gift-card",
                AllowUserReviews = true,
                Price = 25M,
                IsGiftCard = true,
                GiftCardType = GiftCardType.Virtual,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                Published = true,
                ShowOnHomepage = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannel25GiftCard);

            await InsertInstallationDataAsync(tvChannel25GiftCard);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannel25GiftCard.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Gift Cards").Id,
                DisplayOrder = 2
            });

            await InsertTvChannelPictureAsync(tvChannel25GiftCard, "tvChannel_25giftcart.jpeg");

            await AddTvChannelTagAsync(tvChannel25GiftCard, "nice");
            await AddTvChannelTagAsync(tvChannel25GiftCard, "gift");

            var tvChannel50GiftCard = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "$50 Physical Gift Card",
                Sku = "PG_CR_050",
                ShortDescription = "$50 Gift Card. Gift Cards must be redeemed through our site Web site toward the purchase of eligible tvChannels.",
                FullDescription = "<p>Gift Cards must be redeemed through our site Web site toward the purchase of eligible tvChannels. Purchases are deducted from the GiftCard balance. Any unused balance will be placed in the recipient's GiftCard account when redeemed. If an order exceeds the amount of the GiftCard, the balance must be paid with a credit card or other available payment method.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "50-physical-gift-card",
                AllowUserReviews = true,
                Price = 50M,
                IsGiftCard = true,
                GiftCardType = GiftCardType.Physical,
                IsShipEnabled = true,
                IsFreeShipping = true,
                DeliveryDateId = deliveryDate.Id,
                Weight = 1,
                Length = 1,
                Width = 1,
                Height = 1,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                Published = true,
                MarkAsNew = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannel50GiftCard);

            await InsertInstallationDataAsync(tvChannel50GiftCard);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannel50GiftCard.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Gift Cards").Id,
                DisplayOrder = 3
            });

            await InsertTvChannelPictureAsync(tvChannel50GiftCard, "tvChannel_50giftcart.jpeg");

            var tvChannel100GiftCard = new TvChannel
            {
                TvChannelType = TvChannelType.SimpleTvChannel,
                VisibleIndividually = true,
                Name = "$100 Physical Gift Card",
                Sku = "PG_CR_100",
                ShortDescription = "$100 Gift Card. Gift Cards must be redeemed through our site Web site toward the purchase of eligible tvChannels.",
                FullDescription = "<p>Gift Cards must be redeemed through our site Web site toward the purchase of eligible tvChannels. Purchases are deducted from the GiftCard balance. Any unused balance will be placed in the recipient's GiftCard account when redeemed. If an order exceeds the amount of the GiftCard, the balance must be paid with a credit card or other available payment method.</p>",
                TvChannelTemplateId = tvChannelTemplateSimple.Id,
                //SeName = "100-physical-gift-card",
                AllowUserReviews = true,
                Price = 100M,
                IsGiftCard = true,
                GiftCardType = GiftCardType.Physical,
                IsShipEnabled = true,
                DeliveryDateId = deliveryDate.Id,
                Weight = 1,
                Length = 1,
                Width = 1,
                Height = 1,
                ManageInventoryMethod = ManageInventoryMethod.DontManageStock,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
                StockQuantity = 10000,
                NotifyAdminForQuantityBelow = 1,
                AllowBackInStockSubscriptions = false,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            allTvChannels.Add(tvChannel100GiftCard);

            await InsertInstallationDataAsync(tvChannel100GiftCard);

            await InsertInstallationDataAsync(new TvChannelCategory
            {
                TvChannelId = tvChannel100GiftCard.Id,
                CategoryId = _categoryRepository.Table.Single(c => c.Name == "Gift Cards").Id,
                DisplayOrder = 4
            });

            await InsertTvChannelPictureAsync(tvChannel100GiftCard, "tvChannel_100giftcart.jpeg");
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallTvChannelsAsync(string defaultUserEmail)
        {
            var tvChannelTemplateSimple = _tvChannelTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Simple tvChannel");
            if (tvChannelTemplateSimple == null)
                throw new Exception("Simple tvChannel template could not be loaded");
            var tvChannelTemplateGrouped = _tvChannelTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Grouped tvChannel (with variants)");
            if (tvChannelTemplateGrouped == null)
                throw new Exception("Grouped tvChannel template could not be loaded");

            //delivery date
            var deliveryDate = _deliveryDateRepository.Table.FirstOrDefault();
            if (deliveryDate == null)
                throw new Exception("No default deliveryDate could be loaded");

            //tvChannel availability range
            var tvChannelAvailabilityRange = _tvChannelAvailabilityRangeRepository.Table.FirstOrDefault();
            if (tvChannelAvailabilityRange == null)
                throw new Exception("No default tvChannel availability range could be loaded");

            //default user/user
            var defaultUser = _userRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultUser == null)
                throw new Exception("Cannot load default user");

            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            //pictures
            var pictureService = EngineContext.Current.Resolve<IPictureService>();
            var sampleImagesPath = GetSamplesPath();

            //downloads
            var downloadService = EngineContext.Current.Resolve<IDownloadService>();
            var sampleDownloadsPath = GetSamplesPath();

            //tvChannels
            var allTvChannels = new List<TvChannel>();

            //related tvChannels
            var relatedTvChannels = new List<RelatedTvChannel>();

            //desktops, notebooks, software
            await InstallComputersAsync(tvChannelTemplateSimple, allTvChannels, sampleImagesPath, pictureService, relatedTvChannels);
            //camera & photo, cell phones, others
            await InstallElectronicsAsync(tvChannelTemplateSimple, tvChannelTemplateGrouped, allTvChannels, sampleImagesPath, pictureService, relatedTvChannels);
            //shoes, clothing, accessories
            await InstallApparelAsync(tvChannelTemplateSimple, allTvChannels, sampleImagesPath, pictureService, relatedTvChannels, tvChannelAvailabilityRange);
            //digital downloads
            await InstallDigitalDownloadsAsync(tvChannelTemplateSimple, allTvChannels, sampleImagesPath, pictureService, relatedTvChannels, sampleDownloadsPath, downloadService);
            //books
            await InstallBooksAsync(tvChannelTemplateSimple, allTvChannels, sampleImagesPath, pictureService, relatedTvChannels);
            //jewelry
            await InstallJewelryAsync(tvChannelTemplateSimple, allTvChannels, sampleImagesPath, pictureService, relatedTvChannels);
            //gift cards
            await InstallGiftCardsAsync(tvChannelTemplateSimple, allTvChannels, sampleImagesPath, pictureService, relatedTvChannels, deliveryDate);

            //search engine names
            foreach (var tvChannel in allTvChannels)
                await InsertInstallationDataAsync(new UrlRecord
                {
                    EntityId = tvChannel.Id,
                    EntityName = nameof(TvChannel),
                    LanguageId = 0,
                    IsActive = true,
                    Slug = await ValidateSeNameAsync(tvChannel, tvChannel.Name)
                });

            //related tvChannels
            await InsertInstallationDataAsync(relatedTvChannels);

            //reviews
            using (var random = new SecureRandomNumberGenerator())
            {
                foreach (var tvChannel in allTvChannels)
                {
                    if (tvChannel.TvChannelType != TvChannelType.SimpleTvChannel)
                        continue;

                    //only 3 of 4 tvChannels will have reviews
                    if (random.Next(4) == 3)
                        continue;

                    //rating from 4 to 5
                    var rating = random.Next(4, 6);

                    await InsertInstallationDataAsync(new TvChannelReview
                    {
                        UserId = defaultUser.Id,
                        TvChannelId = tvChannel.Id,
                        StoreId = defaultStore.Id,
                        IsApproved = true,
                        Title = "Some sample review",
                        ReviewText = $"This sample review is for the {tvChannel.Name}. I've been waiting for this tvChannel to be available. It is priced just right.",
                        //random (4 or 5)
                        Rating = rating,
                        HelpfulYesTotal = 0,
                        HelpfulNoTotal = 0,
                        CreatedOnUtc = DateTime.UtcNow
                    });

                    tvChannel.ApprovedRatingSum = rating;
                    tvChannel.ApprovedTotalReviews = 1;
                }
            }

            await UpdateInstallationDataAsync(allTvChannels);

            //stock quantity history
            foreach (var tvChannel in allTvChannels)
                if (tvChannel.StockQuantity > 0)
                    await InsertInstallationDataAsync(new StockQuantityHistory
                    {
                        TvChannelId = tvChannel.Id,
                        WarehouseId = tvChannel.WarehouseId > 0 ? (int?)tvChannel.WarehouseId : null,
                        QuantityAdjustment = tvChannel.StockQuantity,
                        StockQuantity = tvChannel.StockQuantity,
                        Message = "The stock quantity has been edited",
                        CreatedOnUtc = DateTime.UtcNow
                    });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallForumsAsync()
        {
            var forumGroup = new ForumGroup
            {
                Name = "General",
                DisplayOrder = 5,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            await InsertInstallationDataAsync(forumGroup);

            var newTvChannelsForum = new Forum
            {
                ForumGroupId = forumGroup.Id,
                Name = "New TvChannels",
                Description = "Discuss new tvChannels and industry trends",
                NumTopics = 0,
                NumPosts = 0,
                LastPostUserId = 0,
                LastPostTime = null,
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            await InsertInstallationDataAsync(newTvChannelsForum);

            var mobileDevicesForum = new Forum
            {
                ForumGroupId = forumGroup.Id,
                Name = "Mobile Devices Forum",
                Description = "Discuss the mobile phone market",
                NumTopics = 0,
                NumPosts = 0,
                LastPostUserId = 0,
                LastPostTime = null,
                DisplayOrder = 10,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            await InsertInstallationDataAsync(mobileDevicesForum);

            var packagingShippingForum = new Forum
            {
                ForumGroupId = forumGroup.Id,
                Name = "Packaging & Shipping",
                Description = "Discuss packaging & shipping",
                NumTopics = 0,
                NumPosts = 0,
                LastPostTime = null,
                DisplayOrder = 20,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            await InsertInstallationDataAsync(packagingShippingForum);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallDiscountsAsync()
        {
            var discounts = new List<Discount>
            {
                new Discount
                {
                    IsActive = true,
                    Name = "Sample discount with coupon code",
                    DiscountType = DiscountType.AssignedToSkus,
                    DiscountLimitation = DiscountLimitationType.Unlimited,
                    UsePercentage = false,
                    DiscountAmount = 10,
                    RequiresCouponCode = true,
                    CouponCode = "123"
                },
                new Discount
                {
                    IsActive = true,
                    Name = "'20% order total' discount",
                    DiscountType = DiscountType.AssignedToOrderTotal,
                    DiscountLimitation = DiscountLimitationType.Unlimited,
                    UsePercentage = true,
                    DiscountPercentage = 20,
                    StartDateUtc = new DateTime(2010, 1, 1),
                    EndDateUtc = new DateTime(2020, 1, 1),
                    RequiresCouponCode = true,
                    CouponCode = "456"
                }
            };

            await InsertInstallationDataAsync(discounts);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallBlogPostsAsync(string defaultUserEmail)
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();

            if (defaultLanguage == null)
                throw new Exception("Default language could not be loaded");

            var blogService = EngineContext.Current.Resolve<IBlogService>();

            var blogPosts = new List<BlogPost>
            {
                new BlogPost
                {
                    AllowComments = true,
                    LanguageId = defaultLanguage.Id,
                    Title = "How a blog can help your growing e-Viewer business",
                    BodyOverview = "<p>When you start an online business, your main aim is to sell the tvChannels, right? As a business owner, you want to showcase your store to more audience. So, you decide to go on social media, why? Because everyone is doing it, then why shouldn&rsquo;t you? It is tempting as everyone is aware of the hype that it is the best way to market your brand.</p><p>Do you know having a blog for your online store can be very helpful? Many businesses do not understand the importance of having a blog because they don&rsquo;t have time to post quality content.</p><p>Today, we will talk about how a blog can play an important role for the growth of your e-Viewer business. Later, we will also discuss some tips that will be helpful to you for writing business related blog posts.</p>",
                    Body = "<p>When you start an online business, your main aim is to sell the tvChannels, right? As a business owner, you want to showcase your store to more audience. So, you decide to go on social media, why? Because everyone is doing it, then why shouldn&rsquo;t you? It is tempting as everyone is aware of the hype that it is the best way to market your brand.</p><p>Do you know having a blog for your online store can be very helpful? Many businesses do not understand the importance of having a blog because they don&rsquo;t have time to post quality content.</p><p>Today, we will talk about how a blog can play an important role for the growth of your e-Viewer business. Later, we will also discuss some tips that will be helpful to you for writing business related blog posts.</p><h3>1) Blog is useful in educating your users</h3><p>Blogging is one of the best way by which you can educate your users about your tvChannels/services that you offer. This helps you as a business owner to bring more value to your brand. When you provide useful information to the users about your tvChannels, they are more likely to buy tvChannels from you. You can use your blog for providing tutorials in regard to the use of your tvChannels.</p><p><strong>For example:</strong> If you have an online store that offers computer parts. You can write tutorials about how to build a computer or how to make your computer&rsquo;s performance better. While talking about these things, you can mention tvChannels in the tutorials and provide link to your tvChannels within the blog post from your website. Your potential users might get different ideas of using your tvChannel and will likely to buy tvChannels from your online store.</p><h3>2) Blog helps your business in Search Engine Optimization (SEO)</h3><p>Blog posts create more internal links to your website which helps a lot in SEO. Blog is a great way to have quality content on your website related to your tvChannels/services which is indexed by all major search engines like Google, Bing and Yahoo. The more original content you write in your blog post, the better ranking you will get in search engines. SEO is an on-going process and posting blog posts regularly keeps your site active all the time which is beneficial when it comes to search engine optimization.</p><p><strong>For example:</strong> Let&rsquo;s say you sell &ldquo;Sony Television Model XYZ&rdquo; and you regularly publish blog posts about your tvChannel. Now, whenever someone searches for &ldquo;Sony Television Model XYZ&rdquo;, Google will crawl on your website knowing that you have something to do with this particular tvChannel. Hence, your website will show up on the search result page whenever this item is being searched.</p><h3>3) Blog helps in boosting your sales by convincing the potential users to buy</h3><p>If you own an online business, there are so many ways you can share different stories with your audience in regard your tvChannels/services that you offer. Talk about how you started your business, share stories that educate your audience about what&rsquo;s new in your industry, share stories about how your tvChannel/service was beneficial to someone or share anything that you think your audience might find interesting (it does not have to be related to your tvChannel). This kind of blogging shows that you are an expert in your industry and interested in educating your audience. It sets you apart in the competitive market. This gives you an opportunity to showcase your expertise by educating the visitors and it can turn your audience into buyers.</p><p><strong>Fun Fact:</strong> Did you know that 92% of companies who decided to blog acquired users through their blog?</p><p><a href=\"https://tvprogviewer.ru/\">tvProgViewer</a> is great e-Viewer solution that also offers a variety of CMS features including blog. A store owner has full access for managing the blog posts and related comments.</p>",
                    Tags = "e-commerce, blog, moey",
                    CreatedOnUtc = DateTime.UtcNow
                },
                new BlogPost
                {
                    AllowComments = true,
                    LanguageId = defaultLanguage.Id,
                    Title = "Why your online store needs a wish list",
                    BodyOverview = "<p>What comes to your mind, when you hear the term&rdquo; wish list&rdquo;? The application of this feature is exactly how it sounds like: a list of things that you wish to get. As an online store owner, would you like your users to be able to save tvChannels in a wish list so that they review or buy them later? Would you like your users to be able to share their wish list with friends and family for gift giving?</p><p>Offering your users a feature of wish list as part of shopping cart is a great way to build loyalty to your store site. Having the feature of wish list on a store site allows online businesses to engage with their users in a smart way as it allows the shoppers to create a list of what they desire and their preferences for future purchase.</p>",
                    Body = "<p>What comes to your mind, when you hear the term&rdquo; wish list&rdquo;? The application of this feature is exactly how it sounds like: a list of things that you wish to get. As an online store owner, would you like your users to be able to save tvChannels in a wish list so that they review or buy them later? Would you like your users to be able to share their wish list with friends and family for gift giving?</p><p>Offering your users a feature of wish list as part of shopping cart is a great way to build loyalty to your store site. Having the feature of wish list on a store site allows online businesses to engage with their users in a smart way as it allows the shoppers to create a list of what they desire and their preferences for future purchase.</p><p>Does every e-Viewer store needs a wish list? The answer to this question in most cases is yes, because of the following reasons:</p><p><strong>Understanding the needs of your users</strong> - A wish list is a great way to know what is in your user&rsquo;s mind. Try to think the purchase history as a small portion of the user&rsquo;s preferences. But, the wish list is like a wide open door that can give any online business a lot of valuable information about their user and what they like or desire.</p><p><strong>Shoppers like to share their wish list with friends and family</strong> - Providing your users a way to email their wish list to their friends and family is a pleasant way to make online shopping enjoyable for the shoppers. It is always a good idea to make the wish list sharable by a unique link so that it can be easily shared though different channels like email or on social media sites.</p><p><strong>Wish list can be a great marketing tool</strong> &ndash; Another way to look at wish list is a great marketing tool because it is extremely targeted and the recipients are always motivated to use it. For example: when your younger brother tells you that his wish list is on a certain e-Viewer store. What is the first thing you are going to do? You are most likely to visit the e-Viewer store, check out the wish list and end up buying something for your younger brother.</p><p>So, how a wish list is a marketing tool? The reason is quite simple, it introduce your online store to new users just how it is explained in the above example.</p><p><strong>Encourage users to return to the store site</strong> &ndash; Having a feature of wish list on the store site can increase the return traffic because it encourages users to come back and buy later. Allowing the users to save the wish list to their online accounts gives them a reason return to the store site and login to the account at any time to view or edit the wish list items.</p><p><strong>Wish list can be used for gifts for different occasions like weddings or birthdays. So, what kind of benefits a gift-giver gets from a wish list?</strong></p><ul><li>It gives them a surety that they didn&rsquo;t buy a wrong gift</li><li>It guarantees that the recipient will like the gift</li><li>It avoids any awkward moments when the recipient unwraps the gift and as a gift-giver you got something that the recipient do not want</li></ul><p><strong>Wish list is a great feature to have on a store site &ndash; So, what kind of benefits a business owner gets from a wish list</strong></p><ul><li>It is a great way to advertise an online store as many people do prefer to shop where their friend or family shop online</li><li>It allows the current users to return to the store site and open doors for the new users</li><li>It allows store admins to track what&rsquo;s in users wish list and run promotions accordingly to target specific user segments</li></ul><p><a href=\"https://tvprogviewer.ru/\">tvProgViewer</a> offers the feature of wish list that allows users to create a list of tvChannels that they desire or planning to buy in future.</p>",
                    Tags = "e-commerce, tvProgViewer, sample tag, money",
                    CreatedOnUtc = DateTime.UtcNow.AddSeconds(1)
                }
            };

            await InsertInstallationDataAsync(blogPosts);

            //search engine names
            foreach (var blogPost in blogPosts)
                await InsertInstallationDataAsync(new UrlRecord
                {
                    EntityId = blogPost.Id,
                    EntityName = nameof(BlogPost),
                    LanguageId = blogPost.LanguageId,
                    IsActive = true,
                    Slug = await ValidateSeNameAsync(blogPost, blogPost.Title)
                });

            //comments
            var defaultUser = _userRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultUser == null)
                throw new Exception("Cannot load default user");

            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            foreach (var blogPost in blogPosts)
                await blogService.InsertBlogCommentAsync(new BlogComment
                {
                    BlogPostId = blogPost.Id,
                    UserId = defaultUser.Id,
                    CommentText = "This is a sample comment for this blog post",
                    IsApproved = true,
                    StoreId = defaultStore.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });

            await UpdateInstallationDataAsync(blogPosts);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallNewsAsync(string defaultUserEmail)
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();

            if (defaultLanguage == null)
                throw new Exception("Default language could not be loaded");

            var newsService = EngineContext.Current.Resolve<INewsService>();

            var news = new List<NewsItem>
            {
                new NewsItem
                {
                    AllowComments = true,
                    LanguageId = defaultLanguage.Id,
                    Title = "About tvProgViewer",
                    Short = "It's stable and highly usable. From downloads to documentation, www.tvprogviewer.ru offers a comprehensive base of information, resources, and support to the tvProgViewer community.",
                    Full = "<p>For full feature list go to <a href=\"https://www.tvprogviewer.ru\">tvprogviewer.ru</a></p><p>Providing outstanding custom search engine optimization, web development services and e-commerce development solutions to our clients at a fair price in a professional manner.</p>",
                    Published = true,
                    CreatedOnUtc = DateTime.UtcNow
                },
                new NewsItem
                {
                    AllowComments = true,
                    LanguageId = defaultLanguage.Id,
                    Title = "tvProgViewer new release!",
                    Short = "tvProgViewer includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included! tvProgViewer is a fully customizable shopping cart",
                    Full = "<p>tvProgViewer includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included!</p>",
                    Published = true,
                    CreatedOnUtc = DateTime.UtcNow.AddSeconds(1)
                },
                new NewsItem
                {
                    AllowComments = true,
                    LanguageId = defaultLanguage.Id,
                    Title = "New online store is open!",
                    Short = "The new tvProgViewer store is open now! We are very excited to offer our new range of tvChannels. We will be constantly adding to our range so please register on our site.",
                    Full = "<p>Our online store is officially up and running. Stock up for the holiday season! We have a great selection of items. We will be constantly adding to our range so please register on our site, this will enable you to keep up to date with any new tvChannels.</p><p>All shipping is worldwide and will leave the same day an order is placed! Happy Shopping and spread the word!!</p>",
                    Published = true,
                    CreatedOnUtc = DateTime.UtcNow.AddSeconds(2)
                }
            };

            await InsertInstallationDataAsync(news);

            //search engine names
            foreach (var newsItem in news)
                await InsertInstallationDataAsync(new UrlRecord
                {
                    EntityId = newsItem.Id,
                    EntityName = nameof(NewsItem),
                    LanguageId = newsItem.LanguageId,
                    IsActive = true,
                    Slug = await ValidateSeNameAsync(newsItem, newsItem.Title)
                });

            //comments
            var defaultUser = _userRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultUser == null)
                throw new Exception("Cannot load default user");

            //default store
            var defaultStore = _storeRepository.Table.FirstOrDefault();
            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            foreach (var newsItem in news)
                await newsService.InsertNewsCommentAsync(new NewsComment
                {
                    NewsItemId = newsItem.Id,
                    UserId = defaultUser.Id,
                    CommentTitle = "Sample comment title",
                    CommentText = "This is a sample comment...",
                    IsApproved = true,
                    StoreId = defaultStore.Id,
                    CreatedOnUtc = DateTime.UtcNow
                });

            await UpdateInstallationDataAsync(news);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallPollsAsync()
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();

            if (defaultLanguage == null)
                throw new Exception("Default language could not be loaded");

            var poll1 = new Poll
            {
                LanguageId = defaultLanguage.Id,
                Name = "Do you like tvProgViewer?",
                SystemKeyword = string.Empty,
                Published = true,
                ShowOnHomepage = true,
                DisplayOrder = 1
            };

            await InsertInstallationDataAsync(poll1);

            var answers = new List<PollAnswer>
            {
                new PollAnswer
            {
                Name = "Excellent",
                DisplayOrder = 1,
                PollId = poll1.Id
            },
                new PollAnswer
            {
                Name = "Good",
                DisplayOrder = 2,
                PollId = poll1.Id
            },
                new PollAnswer
            {
                Name = "Poor",
                DisplayOrder = 3,
                PollId = poll1.Id
            },
                new PollAnswer
            {
                Name = "Very bad",
                DisplayOrder = 4,
                PollId = poll1.Id
            }
            };

            await InsertInstallationDataAsync(answers);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallActivityLogTypesAsync()
        {
            var activityLogTypes = new List<ActivityLogType>
            {
                //admin area activities
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAddressAttribute",
                    Enabled = true,
                    Name = "Add a new address attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAddressAttributeValue",
                    Enabled = true,
                    Name = "Add a new address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAffiliate",
                    Enabled = true,
                    Name = "Add a new affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewBlogPost",
                    Enabled = true,
                    Name = "Add a new blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCampaign",
                    Enabled = true,
                    Name = "Add a new campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCategory",
                    Enabled = true,
                    Name = "Add a new category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCheckoutAttribute",
                    Enabled = true,
                    Name = "Add a new checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCountry",
                    Enabled = true,
                    Name = "Add a new country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCurrency",
                    Enabled = true,
                    Name = "Add a new currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewUser",
                    Enabled = true,
                    Name = "Add a new user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewUserAttribute",
                    Enabled = true,
                    Name = "Add a new user attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewUserAttributeValue",
                    Enabled = true,
                    Name = "Add a new user attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewUserRole",
                    Enabled = true,
                    Name = "Add a new user role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewDiscount",
                    Enabled = true,
                    Name = "Add a new discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewEmailAccount",
                    Enabled = true,
                    Name = "Add a new email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewGiftCard",
                    Enabled = true,
                    Name = "Add a new gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewLanguage",
                    Enabled = true,
                    Name = "Add a new language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewManufacturer",
                    Enabled = true,
                    Name = "Add a new manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewMeasureDimension",
                    Enabled = true,
                    Name = "Add a new measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewMeasureWeight",
                    Enabled = true,
                    Name = "Add a new measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewNews",
                    Enabled = true,
                    Name = "Add a new news"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewTvChannel",
                    Enabled = true,
                    Name = "Add a new tvChannel"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewTvChannelAttribute",
                    Enabled = true,
                    Name = "Add a new tvChannel attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSetting",
                    Enabled = true,
                    Name = "Add a new setting"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSpecAttribute",
                    Enabled = true,
                    Name = "Add a new specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSpecAttributeGroup",
                    Enabled = true,
                    Name = "Add a new specification attribute group"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewStateProvince",
                    Enabled = true,
                    Name = "Add a new state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewStore",
                    Enabled = true,
                    Name = "Add a new store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewTopic",
                    Enabled = true,
                    Name = "Add a new topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewReviewType",
                    Enabled = true,
                    Name = "Add a new review type"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewVendor",
                    Enabled = true,
                    Name = "Add a new vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewVendorAttribute",
                    Enabled = true,
                    Name = "Add a new vendor attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewVendorAttributeValue",
                    Enabled = true,
                    Name = "Add a new vendor attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewWarehouse",
                    Enabled = true,
                    Name = "Add a new warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewWidget",
                    Enabled = true,
                    Name = "Add a new widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteActivityLog",
                    Enabled = true,
                    Name = "Delete activity log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAddressAttribute",
                    Enabled = true,
                    Name = "Delete an address attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAddressAttributeValue",
                    Enabled = true,
                    Name = "Delete an address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAffiliate",
                    Enabled = true,
                    Name = "Delete an affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteBlogPost",
                    Enabled = true,
                    Name = "Delete a blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteBlogPostComment",
                    Enabled = true,
                    Name = "Delete a blog post comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCampaign",
                    Enabled = true,
                    Name = "Delete a campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCategory",
                    Enabled = true,
                    Name = "Delete category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCheckoutAttribute",
                    Enabled = true,
                    Name = "Delete a checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCountry",
                    Enabled = true,
                    Name = "Delete a country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCurrency",
                    Enabled = true,
                    Name = "Delete a currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteUser",
                    Enabled = true,
                    Name = "Delete a user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteUserAttribute",
                    Enabled = true,
                    Name = "Delete a user attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteUserAttributeValue",
                    Enabled = true,
                    Name = "Delete a user attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteUserRole",
                    Enabled = true,
                    Name = "Delete a user role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteDiscount",
                    Enabled = true,
                    Name = "Delete a discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteEmailAccount",
                    Enabled = true,
                    Name = "Delete an email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteGiftCard",
                    Enabled = true,
                    Name = "Delete a gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteLanguage",
                    Enabled = true,
                    Name = "Delete a language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteManufacturer",
                    Enabled = true,
                    Name = "Delete a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMeasureDimension",
                    Enabled = true,
                    Name = "Delete a measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMeasureWeight",
                    Enabled = true,
                    Name = "Delete a measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMessageTemplate",
                    Enabled = true,
                    Name = "Delete a message template"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteNews",
                    Enabled = true,
                    Name = "Delete a news"
                },
                 new ActivityLogType
                {
                    SystemKeyword = "DeleteNewsComment",
                    Enabled = true,
                    Name = "Delete a news comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteOrder",
                    Enabled = true,
                    Name = "Delete an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeletePlugin",
                    Enabled = true,
                    Name = "Delete a plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteTvChannel",
                    Enabled = true,
                    Name = "Delete a tvChannel"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteTvChannelAttribute",
                    Enabled = true,
                    Name = "Delete a tvChannel attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteTvChannelReview",
                    Enabled = true,
                    Name = "Delete a tvChannel review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteReturnRequest",
                    Enabled = true,
                    Name = "Delete a return request"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteReviewType",
                    Enabled = true,
                    Name = "Delete a review type"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSetting",
                    Enabled = true,
                    Name = "Delete a setting"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSpecAttribute",
                    Enabled = true,
                    Name = "Delete a specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSpecAttributeGroup",
                    Enabled = true,
                    Name = "Delete a specification attribute group"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStateProvince",
                    Enabled = true,
                    Name = "Delete a state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStore",
                    Enabled = true,
                    Name = "Delete a store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSystemLog",
                    Enabled = true,
                    Name = "Delete system log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteTopic",
                    Enabled = true,
                    Name = "Delete a topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteVendor",
                    Enabled = true,
                    Name = "Delete a vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteVendorAttribute",
                    Enabled = true,
                    Name = "Delete a vendor attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteVendorAttributeValue",
                    Enabled = true,
                    Name = "Delete a vendor attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteWarehouse",
                    Enabled = true,
                    Name = "Delete a warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteWidget",
                    Enabled = true,
                    Name = "Delete a widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditActivityLogTypes",
                    Enabled = true,
                    Name = "Edit activity log types"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditAddressAttribute",
                    Enabled = true,
                    Name = "Edit an address attribute"
                },
                 new ActivityLogType
                {
                    SystemKeyword = "EditAddressAttributeValue",
                    Enabled = true,
                    Name = "Edit an address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditAffiliate",
                    Enabled = true,
                    Name = "Edit an affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditBlogPost",
                    Enabled = true,
                    Name = "Edit a blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCampaign",
                    Enabled = true,
                    Name = "Edit a campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCategory",
                    Enabled = true,
                    Name = "Edit category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCheckoutAttribute",
                    Enabled = true,
                    Name = "Edit a checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCountry",
                    Enabled = true,
                    Name = "Edit a country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCurrency",
                    Enabled = true,
                    Name = "Edit a currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditUser",
                    Enabled = true,
                    Name = "Edit a user"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditUserAttribute",
                    Enabled = true,
                    Name = "Edit a user attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditUserAttributeValue",
                    Enabled = true,
                    Name = "Edit a user attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditUserRole",
                    Enabled = true,
                    Name = "Edit a user role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditDiscount",
                    Enabled = true,
                    Name = "Edit a discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditEmailAccount",
                    Enabled = true,
                    Name = "Edit an email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditGiftCard",
                    Enabled = true,
                    Name = "Edit a gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditLanguage",
                    Enabled = true,
                    Name = "Edit a language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditManufacturer",
                    Enabled = true,
                    Name = "Edit a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMeasureDimension",
                    Enabled = true,
                    Name = "Edit a measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMeasureWeight",
                    Enabled = true,
                    Name = "Edit a measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMessageTemplate",
                    Enabled = true,
                    Name = "Edit a message template"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditNews",
                    Enabled = true,
                    Name = "Edit a news"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditOrder",
                    Enabled = true,
                    Name = "Edit an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPlugin",
                    Enabled = true,
                    Name = "Edit a plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTvChannel",
                    Enabled = true,
                    Name = "Edit a tvChannel"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTvChannelAttribute",
                    Enabled = true,
                    Name = "Edit a tvChannel attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTvChannelReview",
                    Enabled = true,
                    Name = "Edit a tvChannel review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPromotionProviders",
                    Enabled = true,
                    Name = "Edit promotion providers"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditReturnRequest",
                    Enabled = true,
                    Name = "Edit a return request"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditReviewType",
                    Enabled = true,
                    Name = "Edit a review type"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSettings",
                    Enabled = true,
                    Name = "Edit setting(s)"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditStateProvince",
                    Enabled = true,
                    Name = "Edit a state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditStore",
                    Enabled = true,
                    Name = "Edit a store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTask",
                    Enabled = true,
                    Name = "Edit a task"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSpecAttribute",
                    Enabled = true,
                    Name = "Edit a specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSpecAttributeGroup",
                    Enabled = true,
                    Name = "Edit a specification attribute group"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditVendor",
                    Enabled = true,
                    Name = "Edit a vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditVendorAttribute",
                    Enabled = true,
                    Name = "Edit a vendor attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditVendorAttributeValue",
                    Enabled = true,
                    Name = "Edit a vendor attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditWarehouse",
                    Enabled = true,
                    Name = "Edit a warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTopic",
                    Enabled = true,
                    Name = "Edit a topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditWidget",
                    Enabled = true,
                    Name = "Edit a widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Impersonation.Started",
                    Enabled = true,
                    Name = "User impersonation session. Started"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Impersonation.Finished",
                    Enabled = true,
                    Name = "User impersonation session. Finished"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportCategories",
                    Enabled = true,
                    Name = "Categories were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportManufacturers",
                    Enabled = true,
                    Name = "Manufacturers were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportTvChannels",
                    Enabled = true,
                    Name = "TvChannels were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportNewsLetterSubscriptions",
                    Enabled = true,
                    Name = "Newsletter subscriptions were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportStates",
                    Enabled = true,
                    Name = "States were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ExportUsers",
                    Enabled = true,
                    Name = "Users were exported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ExportCategories",
                    Enabled = true,
                    Name = "Categories were exported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ExportManufacturers",
                    Enabled = true,
                    Name = "Manufacturers were exported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ExportTvChannels",
                    Enabled = true,
                    Name = "TvChannels were exported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ExportOrders",
                    Enabled = true,
                    Name = "Orders were exported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ExportStates",
                    Enabled = true,
                    Name = "States were exported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ExportNewsLetterSubscriptions",
                    Enabled = true,
                    Name = "Newsletter subscriptions were exported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "InstallNewPlugin",
                    Enabled = true,
                    Name = "Install a new plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UninstallPlugin",
                    Enabled = true,
                    Name = "Uninstall a plugin"
                },
                //public store activities
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewCategory",
                    Enabled = false,
                    Name = "Public store. View a category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewManufacturer",
                    Enabled = false,
                    Name = "Public store. View a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewTvChannel",
                    Enabled = false,
                    Name = "Public store. View a tvChannel"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.PlaceOrder",
                    Enabled = false,
                    Name = "Public store. Place an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.SendPM",
                    Enabled = false,
                    Name = "Public store. Send PM"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ContactUs",
                    Enabled = false,
                    Name = "Public store. Use contact us form"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToCompareList",
                    Enabled = false,
                    Name = "Public store. Add to compare list"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToShoppingCart",
                    Enabled = false,
                    Name = "Public store. Add to shopping cart"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToWishlist",
                    Enabled = false,
                    Name = "Public store. Add to wishlist"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.Login",
                    Enabled = false,
                    Name = "Public store. Login"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.Logout",
                    Enabled = false,
                    Name = "Public store. Logout"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddTvChannelReview",
                    Enabled = false,
                    Name = "Public store. Add tvChannel review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddNewsComment",
                    Enabled = false,
                    Name = "Public store. Add news comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddBlogComment",
                    Enabled = false,
                    Name = "Public store. Add blog comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddForumTopic",
                    Enabled = false,
                    Name = "Public store. Add forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.EditForumTopic",
                    Enabled = false,
                    Name = "Public store. Edit forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.DeleteForumTopic",
                    Enabled = false,
                    Name = "Public store. Delete forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddForumPost",
                    Enabled = false,
                    Name = "Public store. Add forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.EditForumPost",
                    Enabled = false,
                    Name = "Public store. Edit forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.DeleteForumPost",
                    Enabled = false,
                    Name = "Public store. Delete forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UploadNewPlugin",
                    Enabled = true,
                    Name = "Upload a plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UploadNewTheme",
                    Enabled = true,
                    Name = "Upload a theme"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UploadIcons",
                    Enabled = true,
                    Name = "Upload a favicon and app icons"
                }
            };

            await InsertInstallationDataAsync(activityLogTypes);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallTvChannelTemplatesAsync()
        {
            var tvChannelTemplates = new List<TvChannelTemplate>
            {
                new TvChannelTemplate
                {
                    Name = "Simple tvChannel",
                    ViewPath = "TvChannelTemplate.Simple",
                    DisplayOrder = 10,
                    IgnoredTvChannelTypes = ((int)TvChannelType.GroupedTvChannel).ToString()
                },
                new TvChannelTemplate
                {
                    Name = "Grouped tvChannel (with variants)",
                    ViewPath = "TvChannelTemplate.Grouped",
                    DisplayOrder = 100,
                    IgnoredTvChannelTypes = ((int)TvChannelType.SimpleTvChannel).ToString()
                }
            };

            await InsertInstallationDataAsync(tvChannelTemplates);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallCategoryTemplatesAsync()
        {
            var categoryTemplates = new List<CategoryTemplate>
            {
                new CategoryTemplate
                {
                    Name = "TvChannels in Grid or Lines",
                    ViewPath = "CategoryTemplate.TvChannelsInGridOrLines",
                    DisplayOrder = 1
                }
            };

            await InsertInstallationDataAsync(categoryTemplates);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallManufacturerTemplatesAsync()
        {
            var manufacturerTemplates = new List<ManufacturerTemplate>
            {
                new ManufacturerTemplate
                {
                    Name = "TvChannels in Grid or Lines",
                    ViewPath = "ManufacturerTemplate.TvChannelsInGridOrLines",
                    DisplayOrder = 1
                }
            };

            await InsertInstallationDataAsync(manufacturerTemplates);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallTopicTemplatesAsync()
        {
            var topicTemplates = new List<TopicTemplate>
            {
                new TopicTemplate
                {
                    Name = "Default template",
                    ViewPath = "TopicDetails",
                    DisplayOrder = 1
                }
            };

            await InsertInstallationDataAsync(topicTemplates);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallScheduleTasksAsync()
        {
            var lastEnabledUtc = DateTime.UtcNow;
            var tasks = new List<ScheduleTask>
            {
                new ScheduleTask
                {
                    Name = "Send emails",
                    Seconds = 60,
                    Type = "TvProgViewer.Services.Messages.QueuedMessagesSendTask, TvProgViewer.Services",
                    Enabled = true,
                    LastEnabledUtc = lastEnabledUtc,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Keep alive",
                    Seconds = 300,
                    Type = "TvProgViewer.Services.Common.KeepAliveTask, TvProgViewer.Services",
                    Enabled = true,
                    LastEnabledUtc = lastEnabledUtc,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = nameof(ResetLicenseCheckTask),
                    Seconds = 2073600,
                    Type = "TvProg.Services.Common.ResetLicenseCheckTask, TvProg.Services",
                    Enabled = true,
                    LastEnabledUtc = lastEnabledUtc,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Delete guests",
                    Seconds = 600,
                    Type = "TvProgViewer.Services.Users.DeleteGuestsTask, TvProgViewer.Services",
                    Enabled = true,
                    LastEnabledUtc = lastEnabledUtc,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Clear cache",
                    Seconds = 600,
                    Type = "TvProgViewer.Services.Caching.ClearCacheTask, TvProgViewer.Services",
                    Enabled = false,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Clear log",
                    //60 minutes
                    Seconds = 3600,
                    Type = "TvProgViewer.Services.Logging.ClearLogTask, TvProgViewer.Services",
                    Enabled = false,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Update currency exchange rates",
                    //60 minutes
                    Seconds = 3600,
                    Type = "TvProgViewer.Services.Directory.UpdateExchangeRateTask, TvProgViewer.Services",
                    Enabled = true,
                    LastEnabledUtc = lastEnabledUtc,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Delete inactive users (GDPR)",
                    //24 hours
                    Seconds = 86400,
                    Type = "TvProgViewer.Services.Gdpr.DeleteInactiveUsersTask, TvProgViewer.Services",
                    Enabled = false,
                    StopOnError = false
                },
                new ScheduleTask
                {
                    Name = "Обновление телепрограммы по расписанию",
                    Seconds = 300,
                    Type = "TvProgViewer.Services.Common.UpdateTvProgTask, TvProgViewer.Services",
                    Enabled = true,
                    StopOnError = false
                }
            };

            await InsertInstallationDataAsync(tasks);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallReturnRequestReasonsAsync()
        {
            var returnRequestReasons = new List<ReturnRequestReason>
            {
                new ReturnRequestReason
                {
                    Name = "Received Wrong TvChannel",
                    DisplayOrder = 1
                },
                new ReturnRequestReason
                {
                    Name = "Wrong TvChannel Ordered",
                    DisplayOrder = 2
                },
                new ReturnRequestReason
                {
                    Name = "There Was A Problem With The TvChannel",
                    DisplayOrder = 3
                }
            };

            await InsertInstallationDataAsync(returnRequestReasons);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallReturnRequestActionsAsync()
        {
            var returnRequestActions = new List<ReturnRequestAction>
            {
                new ReturnRequestAction
                {
                    Name = "Repair",
                    DisplayOrder = 1
                },
                new ReturnRequestAction
                {
                    Name = "Replacement",
                    DisplayOrder = 2
                },
                new ReturnRequestAction
                {
                    Name = "Store Credit",
                    DisplayOrder = 3
                }
            };

            await InsertInstallationDataAsync(returnRequestActions);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallWarehousesAsync()
        {
            var warehouse1address = new Address
            {
                Address1 = "21 West 52nd Street",
                City = "New York",
                StateProvinceId = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York")?.Id,
                CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA")?.Id,
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow
            };

            await InsertInstallationDataAsync(warehouse1address);

            var warehouse2address = new Address
            {
                Address1 = "300 South Spring Stree",
                City = "Los Angeles",
                StateProvinceId = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "California")?.Id,
                CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA")?.Id,
                ZipPostalCode = "90013",
                CreatedOnUtc = DateTime.UtcNow
            };

            await InsertInstallationDataAsync(warehouse2address);

            var warehouses = new List<Warehouse>
            {
                new Warehouse
                {
                    Name = "Warehouse 1 (New York)",
                    AddressId = warehouse1address.Id
                },
                new Warehouse
                {
                    Name = "Warehouse 2 (Los Angeles)",
                    AddressId = warehouse2address.Id
                }
            };

            await InsertInstallationDataAsync(warehouses);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallVendorsAsync()
        {
            var vendors = new List<Vendor>
            {
                new Vendor
                {
                    Name = "Vendor 1",
                    Email = "vendor1email@gmail.com",
                    Description = "Some description...",
                    AdminComment = string.Empty,
                    PictureId = 0,
                    Active = true,
                    DisplayOrder = 1,
                    PageSize = 6,
                    AllowUsersToSelectPageSize = true,
                    PageSizeOptions = "6, 3, 9, 18",
                    PriceRangeFiltering = true,
                    ManuallyPriceRange = true,
                    PriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom,
                    PriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo,
                },
                new Vendor
                {
                    Name = "Vendor 2",
                    Email = "vendor2email@gmail.com",
                    Description = "Some description...",
                    AdminComment = string.Empty,
                    PictureId = 0,
                    Active = true,
                    DisplayOrder = 2,
                    PageSize = 6,
                    AllowUsersToSelectPageSize = true,
                    PageSizeOptions = "6, 3, 9, 18"
                }
            };

            await InsertInstallationDataAsync(vendors);

            //search engine names
            foreach (var vendor in vendors)
                await InsertInstallationDataAsync(new UrlRecord
                {
                    EntityId = vendor.Id,
                    EntityName = nameof(Vendor),
                    LanguageId = 0,
                    IsActive = true,
                    Slug = await ValidateSeNameAsync(vendor, vendor.Name)
                });
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InstallAffiliatesAsync()
        {
            var affiliateAddress = new Address
            {
                FirstName = "John",
                LastName = "Smith",
                MiddleName = "Johnovich",
                Email = "affiliate_email@gmail.com",
                Company = "Company name here...",
                City = "New York",
                Address1 = "21 West 52nd Street",
                ZipPostalCode = "10021",
                PhoneNumber = "123456789",
                StateProvinceId = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York")?.Id,
                CountryId = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA")?.Id,
                CreatedOnUtc = DateTime.UtcNow
            };

            await InsertInstallationDataAsync(affiliateAddress);

            var affilate = new Affiliate
            {
                Active = true,
                AddressId = affiliateAddress.Id
            };

            await InsertInstallationDataAsync(affilate);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task AddTvChannelTagAsync(TvChannel tvChannel, string tag)
        {
            var tvChannelTag = _tvChannelTagRepository.Table.FirstOrDefault(pt => pt.Name == tag);

            if (tvChannelTag is null)
            {
                tvChannelTag = new TvChannelTag
                {
                    Name = tag
                };

                await InsertInstallationDataAsync(tvChannelTag);

                //search engine name
                await InsertInstallationDataAsync(new UrlRecord
                {
                    EntityId = tvChannelTag.Id,
                    EntityName = nameof(TvChannelTag),
                    LanguageId = 0,
                    IsActive = true,
                    Slug = await ValidateSeNameAsync(tvChannelTag, tvChannelTag.Name)
                });
            }

            await InsertInstallationDataAsync(new TvChannelTvChannelTagMapping { TvChannelTagId = tvChannelTag.Id, TvChannelId = tvChannel.Id });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Install required data
        /// </summary>
        /// <param name="defaultUserEmail">Default user email</param>
        /// <param name="defaultUserPassword">Default user password</param>
        /// <param name="languagePackInfo">Language pack info</param>
        /// <param name="regionInfo">RegionInfo</param>
        /// <param name="cultureInfo">CultureInfo</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InstallRequiredDataAsync(string defaultUserEmail, string defaultUserPassword,
            (string languagePackDownloadLink, int languagePackProgress) languagePackInfo, RegionInfo regionInfo, CultureInfo cultureInfo)
        {
            await InstallStoresAsync();
            await InstallMeasuresAsync(regionInfo);
            await InstallTaxCategoriesAsync();
            await InstallLanguagesAsync(languagePackInfo, cultureInfo, regionInfo);
            await InstallCurrenciesAsync(cultureInfo, regionInfo);
            await InstallCountriesAndStatesAsync();
            await InstallShippingMethodsAsync();
            await InstallDeliveryDatesAsync();
            await InstallTvChannelAvailabilityRangesAsync();
            await InstallEmailAccountsAsync();
            await InstallMessageTemplatesAsync();
            await InstallTopicTemplatesAsync();
            await InstallSettingsAsync(regionInfo);
            await InstallUsersAndUsersAsync(defaultUserEmail, defaultUserPassword);
            await InstallTopicsAsync();
            await InstallActivityLogTypesAsync();
            await InstallTvChannelTemplatesAsync();
            await InstallCategoryTemplatesAsync();
            await InstallManufacturerTemplatesAsync();
            await InstallScheduleTasksAsync();
            await InstallReturnRequestReasonsAsync();
            await InstallReturnRequestActionsAsync();
        }

        /// <summary>
        /// Install sample data
        /// </summary>
        /// <param name="defaultUserEmail">Default user email</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InstallSampleDataAsync(string defaultUserEmail)
        {
            await InstallSampleUsersAsync();
            await InstallCheckoutAttributesAsync();
            await InstallSpecificationAttributesAsync();
            await InstallTvChannelAttributesAsync();
            await InstallCategoriesAsync();
            await InstallManufacturersAsync();
            await InstallTvChannelsAsync(defaultUserEmail);
            await InstallForumsAsync();
            await InstallDiscountsAsync();
            await InstallBlogPostsAsync(defaultUserEmail);
            await InstallNewsAsync(defaultUserEmail);
            await InstallPollsAsync();
            await InstallWarehousesAsync();
            await InstallVendorsAsync();
            await InstallAffiliatesAsync();
            await InstallOrdersAsync();
            await InstallActivityLogAsync(defaultUserEmail);
            await InstallSearchTermsAsync();

            var settingService = EngineContext.Current.Resolve<ISettingService>();

            await settingService.SaveSettingAsync(new DisplayDefaultMenuItemSettings
            {
                DisplayHomepageMenuItem = false,
                DisplayNewTvChannelsMenuItem = false,
                DisplayTvChannelSearchMenuItem = false,
                DisplayUserInfoMenuItem = false,
                DisplayBlogMenuItem = false,
                DisplayForumsMenuItem = false,
                DisplayContactUsMenuItem = false
            });
        }

        #endregion
    }
}
