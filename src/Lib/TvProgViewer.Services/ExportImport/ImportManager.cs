using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Payments;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Http;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.ExportImport.Help;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Shipping;
using TvProgViewer.Services.Shipping.Date;
using TvProgViewer.Services.Stores;
using TvProgViewer.Services.Tax;
using TvProgViewer.Services.Vendors;

namespace TvProgViewer.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager : IImportManager
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAddressService _addressService;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly ICategoryService _categoryService;
        private readonly ICountryService _countryService;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly ICustomNumberFormatter _customNumberFormatter;
        private readonly ITvProgDataProvider _dataProvider;
        private readonly IDateRangeService _dateRangeService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILogger _logger;
        private readonly IManufacturerService _manufacturerService;
        private readonly IMeasureService _measureService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly ITvProgFileProvider _fileProvider;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly ITvChannelAttributeService _tvChannelAttributeService;
        private readonly ITvChannelService _tvChannelService;
        private readonly ITvChannelTagService _tvChannelTagService;
        private readonly ITvChannelTemplateService _tvChannelTemplateService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IShippingService _shippingService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public ImportManager(CatalogSettings catalogSettings,
            IAddressService addressService,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            ICategoryService categoryService,
            ICountryService countryService,
            IUserActivityService userActivityService,
            IUserService userService,
            ICustomNumberFormatter customNumberFormatter,
            ITvProgDataProvider dataProvider,
            IDateRangeService dateRangeService,
            IHttpClientFactory httpClientFactory,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            ILogger logger,
            IManufacturerService manufacturerService,
            IMeasureService measureService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ITvProgFileProvider fileProvider,
            IOrderService orderService,
            IPictureService pictureService,
            ITvChannelAttributeService tvChannelAttributeService,
            ITvChannelService tvChannelService,
            ITvChannelTagService tvChannelTagService,
            ITvChannelTemplateService tvChannelTemplateService,
            IServiceScopeFactory serviceScopeFactory,
            IShippingService shippingService,
            ISpecificationAttributeService specificationAttributeService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ITaxCategoryService taxCategoryService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings)
        {
            _addressService = addressService;
            _backInStockSubscriptionService = backInStockSubscriptionService;
            _catalogSettings = catalogSettings;
            _categoryService = categoryService;
            _countryService = countryService;
            _userActivityService = userActivityService;
            _userService = userService;
            _customNumberFormatter = customNumberFormatter;
            _dataProvider = dataProvider;
            _dateRangeService = dateRangeService;
            _httpClientFactory = httpClientFactory;
            _fileProvider = fileProvider;
            _languageService = languageService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _logger = logger;
            _manufacturerService = manufacturerService;
            _measureService = measureService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _orderService = orderService;
            _pictureService = pictureService;
            _tvChannelAttributeService = tvChannelAttributeService;
            _tvChannelService = tvChannelService;
            _tvChannelTagService = tvChannelTagService;
            _tvChannelTemplateService = tvChannelTemplateService;
            _serviceScopeFactory = serviceScopeFactory;
            _shippingService = shippingService;
            _specificationAttributeService = specificationAttributeService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _taxCategoryService = taxCategoryService;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        private static ExportedAttributeType GetTypeOfExportedAttribute(IXLWorksheet defaultWorksheet, List<IXLWorksheet> localizedWorksheets, PropertyManager<ExportTvChannelAttribute, Language> tvChannelAttributeManager, PropertyManager<ExportSpecificationAttribute, Language> specificationAttributeManager, int iRow)
        {
            tvChannelAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, iRow, ExportTvChannelAttribute.TvChannelAttributeCellOffset);

            if (tvChannelAttributeManager.IsCaption)
            {
                foreach (var worksheet in localizedWorksheets)
                    tvChannelAttributeManager.ReadLocalizedFromXlsx(worksheet, iRow, ExportTvChannelAttribute.TvChannelAttributeCellOffset);

                return ExportedAttributeType.TvChannelAttribute;
            }

            specificationAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, iRow, ExportTvChannelAttribute.TvChannelAttributeCellOffset);

            if (specificationAttributeManager.IsCaption)
            {
                foreach (var worksheet in localizedWorksheets)
                    specificationAttributeManager.ReadLocalizedFromXlsx(worksheet, iRow, ExportTvChannelAttribute.TvChannelAttributeCellOffset);

                return ExportedAttributeType.SpecificationAttribute;
            }

            return ExportedAttributeType.NotSpecified;
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private static async Task SetOutLineForSpecificationAttributeRowAsync(object cellValue, IXLWorksheet worksheet, int endRow)
        {
            var attributeType = (cellValue ?? string.Empty).ToString();

            if (attributeType.Equals("AttributeType", StringComparison.InvariantCultureIgnoreCase))
            {
                worksheet.Row(endRow).OutlineLevel = 1;
            }
            else
            {
                if ((await SpecificationAttributeType.Option.ToSelectListAsync(useLocalization: false))
                    .Any(p => p.Text.Equals(attributeType, StringComparison.InvariantCultureIgnoreCase)))
                    worksheet.Row(endRow).OutlineLevel = 1;
                else if (int.TryParse(attributeType, out var attributeTypeId) && Enum.IsDefined(typeof(SpecificationAttributeType), attributeTypeId))
                    worksheet.Row(endRow).OutlineLevel = 1;
            }
        }

        private static void CopyDataToNewFile(ImportTvChannelMetadata metadata, IXLWorksheet worksheet, string filePath, int startRow, int endRow, int endCell)
        {
            using var stream = new FileStream(filePath, FileMode.OpenOrCreate);
            // ok, we can run the real code of the sample now
            using var workbook = new XLWorkbook(stream);
            // uncomment this line if you want the XML written out to the outputDir
            //xlPackage.DebugMode = true; 

            // get handles to the worksheets
            var outWorksheet = workbook.Worksheets.Add(typeof(TvChannel).Name);
            metadata.Manager.WriteDefaultCaption(outWorksheet);
            var outRow = 2;
            for (var row = startRow; row <= endRow; row++)
            {
                outWorksheet.Row(outRow).OutlineLevel = worksheet.Row(row).OutlineLevel;
                for (var cell = 1; cell <= endCell; cell++)
                {
                    outWorksheet.Row(outRow).Cell(cell).Value = worksheet.Row(row).Cell(cell).Value;
                }

                outRow += 1;
            }

            workbook.Save();
        }

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            if (columnName == null)
                throw new ArgumentNullException(nameof(columnName));

            for (var i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        protected virtual string GetMimeTypeFromFilePath(string filePath)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out var mimeType);

            //set to jpeg in case mime type cannot be found
            return mimeType ?? _pictureService.GetPictureContentTypeByFileExtension(_fileProvider.GetFileExtension(filePath));
        }

        /// <summary>
        /// Creates or loads the image
        /// </summary>
        /// <param name="picturePath">The path to the image file</param>
        /// <param name="name">The name of the object</param>
        /// <param name="picId">Image identifier, may be null</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the image or null if the image has not changed
        /// </returns>
        protected virtual async Task<Picture> LoadPictureAsync(string picturePath, string name, int? picId = null)
        {
            if (string.IsNullOrEmpty(picturePath) || !_fileProvider.FileExists(picturePath))
                return null;

            var mimeType = GetMimeTypeFromFilePath(picturePath);
            if (string.IsNullOrEmpty(mimeType))
                return null;

            var newPictureBinary = await _fileProvider.ReadAllBytesAsync(picturePath);
            var pictureAlreadyExists = false;
            if (picId != null)
            {
                //compare with existing tvChannel pictures
                var existingPicture = await _pictureService.GetPictureByIdAsync(picId.Value);
                if (existingPicture != null)
                {
                    var existingBinary = await _pictureService.LoadPictureBinaryAsync(existingPicture);
                    //picture binary after validation (like in database)
                    var validatedPictureBinary = await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType, name);
                    if (existingBinary.SequenceEqual(validatedPictureBinary) ||
                        existingBinary.SequenceEqual(newPictureBinary))
                    {
                        pictureAlreadyExists = true;
                    }
                }
            }

            if (pictureAlreadyExists)
                return null;

            var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, await _pictureService.GetPictureSeNameAsync(name));
            return newPicture;
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task LogPictureInsertErrorAsync(string picturePath, Exception ex)
        {
            var extension = _fileProvider.GetFileExtension(picturePath);
            var name = _fileProvider.GetFileNameWithoutExtension(picturePath);

            var point = string.IsNullOrEmpty(extension) ? string.Empty : ".";
            var fileName = _fileProvider.FileExists(picturePath) ? $"{name}{point}{extension}" : string.Empty;

            await _logger.ErrorAsync($"Insert picture failed (file name: {fileName})", ex);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task ImportTvChannelImagesUsingServicesAsync(IList<TvChannelPictureMetadata> tvChannelPictureMetadata)
        {
            foreach (var tvChannel in tvChannelPictureMetadata)
            {
                foreach (var picturePath in new[] { tvChannel.Picture1Path, tvChannel.Picture2Path, tvChannel.Picture3Path })
                {
                    if (string.IsNullOrEmpty(picturePath))
                        continue;

                    var mimeType = GetMimeTypeFromFilePath(picturePath);
                    if (string.IsNullOrEmpty(mimeType))
                        continue;

                    var newPictureBinary = await _fileProvider.ReadAllBytesAsync(picturePath);
                    var pictureAlreadyExists = false;
                    if (!tvChannel.IsNew)
                    {
                        //compare with existing tvChannel pictures
                        var existingPictures = await _pictureService.GetPicturesByTvChannelIdAsync(tvChannel.TvChannelItem.Id);
                        foreach (var existingPicture in existingPictures)
                        {
                            var existingBinary = await _pictureService.LoadPictureBinaryAsync(existingPicture);
                            //picture binary after validation (like in database)
                            var validatedPictureBinary = await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType, picturePath);
                            if (!existingBinary.SequenceEqual(validatedPictureBinary) &&
                                !existingBinary.SequenceEqual(newPictureBinary))
                                continue;
                            //the same picture content
                            pictureAlreadyExists = true;
                            break;
                        }
                    }

                    if (pictureAlreadyExists)
                        continue;

                    try
                    {
                        var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, await _pictureService.GetPictureSeNameAsync(tvChannel.TvChannelItem.Name));
                        await _tvChannelService.InsertTvChannelPictureAsync(new TvChannelPicture
                        {
                            //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                            //pictures are duplicated
                            //maybe because entity size is too large
                            PictureId = newPicture.Id,
                            DisplayOrder = 1,
                            TvChannelId = tvChannel.TvChannelItem.Id
                        });
                        await _tvChannelService.UpdateTvChannelAsync(tvChannel.TvChannelItem);
                    }
                    catch (Exception ex)
                    {
                        await LogPictureInsertErrorAsync(picturePath, ex);
                    }
                }
            }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task ImportTvChannelImagesUsingHashAsync(IList<TvChannelPictureMetadata> tvChannelPictureMetadata, IList<TvChannel> allTvChannelsBySku)
        {
            //performance optimization, load all pictures hashes
            //it will only be used if the images are stored in the SQL Server database (not compact)
            var trimByteCount = _dataProvider.SupportedLengthOfBinaryHash - 1;
            var tvChannelsImagesIds = await _tvChannelService.GetTvChannelsImagesIdsAsync(allTvChannelsBySku.Select(p => p.Id).ToArray());

            var allTvChannelPictureIds = tvChannelsImagesIds.SelectMany(p => p.Value);

            var allPicturesHashes = allTvChannelPictureIds.Any() ? await _dataProvider.GetFieldHashesAsync<PictureBinary>(p => allTvChannelPictureIds.Contains(p.PictureId),
                p => p.PictureId, p => p.BinaryData) : new Dictionary<int, string>();

            foreach (var tvChannel in tvChannelPictureMetadata)
            {
                foreach (var picturePath in new[] { tvChannel.Picture1Path, tvChannel.Picture2Path, tvChannel.Picture3Path })
                {
                    if (string.IsNullOrEmpty(picturePath))
                        continue;
                    try
                    {
                        var mimeType = GetMimeTypeFromFilePath(picturePath);
                        if (string.IsNullOrEmpty(mimeType))
                            continue;

                        var newPictureBinary = await _fileProvider.ReadAllBytesAsync(picturePath);
                        var pictureAlreadyExists = false;
                        var seoFileName = await _pictureService.GetPictureSeNameAsync(tvChannel.TvChannelItem.Name);

                        if (!tvChannel.IsNew)
                        {
                            var newImageHash = HashHelper.CreateHash(
                                newPictureBinary,
                                ExportImportDefaults.ImageHashAlgorithm,
                                trimByteCount);

                            var newValidatedImageHash = HashHelper.CreateHash(
                                await _pictureService.ValidatePictureAsync(newPictureBinary, mimeType, seoFileName),
                                ExportImportDefaults.ImageHashAlgorithm,
                                trimByteCount);

                            var imagesIds = tvChannelsImagesIds.ContainsKey(tvChannel.TvChannelItem.Id)
                                ? tvChannelsImagesIds[tvChannel.TvChannelItem.Id]
                                : Array.Empty<int>();

                            pictureAlreadyExists = allPicturesHashes.Where(p => imagesIds.Contains(p.Key))
                                .Select(p => p.Value)
                                .Any(p =>
                                    p.Equals(newImageHash, StringComparison.OrdinalIgnoreCase) ||
                                    p.Equals(newValidatedImageHash, StringComparison.OrdinalIgnoreCase));
                        }

                        if (pictureAlreadyExists)
                            continue;

                        var newPicture = await _pictureService.InsertPictureAsync(newPictureBinary, mimeType, seoFileName);

                        await _tvChannelService.InsertTvChannelPictureAsync(new TvChannelPicture
                        {
                            //EF has some weird issue if we set "Picture = newPicture" instead of "PictureId = newPicture.Id"
                            //pictures are duplicated
                            //maybe because entity size is too large
                            PictureId = newPicture.Id,
                            DisplayOrder = 1,
                            TvChannelId = tvChannel.TvChannelItem.Id
                        });

                        await _tvChannelService.UpdateTvChannelAsync(tvChannel.TvChannelItem);
                    }
                    catch (Exception ex)
                    {
                        await LogPictureInsertErrorAsync(picturePath, ex);
                    }
                }
            }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task<(string seName, bool isParentCategoryExists)> UpdateCategoryByXlsxAsync(Category category, PropertyManager<Category, Language> manager, Dictionary<string, ValueTask<Category>> allCategories, bool isNew)
        {
            var seName = string.Empty;
            var isParentCategoryExists = true;
            var isParentCategorySet = false;

            foreach (var property in manager.GetDefaultProperties)
            {
                switch (property.PropertyName)
                {
                    case "Name":
                        category.Name = property.StringValue.Split(new[] { ">>" }, StringSplitOptions.RemoveEmptyEntries).Last().Trim();
                        break;
                    case "Description":
                        category.Description = property.StringValue;
                        break;
                    case "CategoryTemplateId":
                        category.CategoryTemplateId = property.IntValue;
                        break;
                    case "MetaKeywords":
                        category.MetaKeywords = property.StringValue;
                        break;
                    case "MetaDescription":
                        category.MetaDescription = property.StringValue;
                        break;
                    case "MetaTitle":
                        category.MetaTitle = property.StringValue;
                        break;
                    case "ParentCategoryId":
                        if (!isParentCategorySet)
                        {
                            var parentCategory = await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Id == property.IntValue);
                            isParentCategorySet = parentCategory != null;

                            isParentCategoryExists = isParentCategorySet || property.IntValue == 0;

                            category.ParentCategoryId = parentCategory?.Id ?? property.IntValue;
                        }

                        break;
                    case "ParentCategoryName":
                        if (_catalogSettings.ExportImportCategoriesUsingCategoryName && !isParentCategorySet)
                        {
                            var categoryName = manager.GetDefaultProperty("ParentCategoryName").StringValue;
                            if (!string.IsNullOrEmpty(categoryName))
                            {
                                var parentCategory = allCategories.ContainsKey(categoryName)
                                    //try find category by full name with all parent category names
                                    ? await allCategories[categoryName]
                                    //try find category by name
                                    : await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Name.Equals(categoryName, StringComparison.InvariantCulture));

                                if (parentCategory != null)
                                {
                                    category.ParentCategoryId = parentCategory.Id;
                                    isParentCategorySet = true;
                                }
                                else
                                {
                                    isParentCategoryExists = false;
                                }
                            }
                        }

                        break;
                    case "Picture":
                        var picture = await LoadPictureAsync(manager.GetDefaultProperty("Picture").StringValue, category.Name, isNew ? null : (int?)category.PictureId);
                        if (picture != null)
                            category.PictureId = picture.Id;
                        break;
                    case "PageSize":
                        category.PageSize = property.IntValue;
                        break;
                    case "AllowUsersToSelectPageSize":
                        category.AllowUsersToSelectPageSize = property.BooleanValue;
                        break;
                    case "PageSizeOptions":
                        category.PageSizeOptions = property.StringValue;
                        break;
                    case "ShowOnHomepage":
                        category.ShowOnHomepage = property.BooleanValue;
                        break;
                    case "PriceRangeFiltering":
                        category.PriceRangeFiltering = property.BooleanValue;
                        break;
                    case "PriceFrom":
                        category.PriceFrom = property.DecimalValue;
                        break;
                    case "PriceTo":
                        category.PriceTo = property.DecimalValue;
                        break;
                    case "AutomaticallyCalculatePriceRange":
                        category.ManuallyPriceRange = property.BooleanValue;
                        break;
                    case "IncludeInTopMenu":
                        category.IncludeInTopMenu = property.BooleanValue;
                        break;
                    case "Published":
                        category.Published = property.BooleanValue;
                        break;
                    case "DisplayOrder":
                        category.DisplayOrder = property.IntValue;
                        break;
                    case "SeName":
                        seName = property.StringValue;
                        break;
                }
            }

            category.UpdatedOnUtc = DateTime.UtcNow;
            return (seName, isParentCategoryExists);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task<(Category category, bool isNew, string curentCategoryBreadCrumb)> GetCategoryFromXlsxAsync(PropertyManager<Category, Language> manager, IXLWorksheet worksheet, int iRow, Dictionary<string, ValueTask<Category>> allCategories)
        {
            manager.ReadDefaultFromXlsx(worksheet, iRow);

            //try get category from database by ID
            var category = await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Id == manager.GetDefaultProperty("Id")?.IntValue);

            if (_catalogSettings.ExportImportCategoriesUsingCategoryName && category == null)
            {
                var categoryName = manager.GetDefaultProperty("Name").StringValue;
                if (!string.IsNullOrEmpty(categoryName))
                {
                    category = allCategories.ContainsKey(categoryName)
                        //try find category by full name with all parent category names
                        ? await allCategories[categoryName]
                        //try find category by name
                        : await await allCategories.Values.FirstOrDefaultAwaitAsync(async c => (await c).Name.Equals(categoryName, StringComparison.InvariantCulture));
                }
            }

            var isNew = category == null;

            category ??= new Category();

            var curentCategoryBreadCrumb = string.Empty;

            if (isNew)
            {
                category.CreatedOnUtc = DateTime.UtcNow;
                //default values
                category.PageSize = _catalogSettings.DefaultCategoryPageSize;
                category.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
                category.Published = true;
                category.IncludeInTopMenu = true;
                category.AllowUsersToSelectPageSize = true;
            }
            else
                curentCategoryBreadCrumb = await _categoryService.GetFormattedBreadCrumbAsync(category);

            return (category, isNew, curentCategoryBreadCrumb);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task SaveCategoryAsync(bool isNew, Category category, Dictionary<string, ValueTask<Category>> allCategories, string curentCategoryBreadCrumb, bool setSeName, string seName)
        {
            if (isNew)
                await _categoryService.InsertCategoryAsync(category);
            else
                await _categoryService.UpdateCategoryAsync(category);

            var categoryBreadCrumb = await _categoryService.GetFormattedBreadCrumbAsync(category);
            if (!allCategories.ContainsKey(categoryBreadCrumb))
                allCategories.Add(categoryBreadCrumb, new ValueTask<Category>(category));
            if (!string.IsNullOrEmpty(curentCategoryBreadCrumb) && allCategories.ContainsKey(curentCategoryBreadCrumb) &&
                categoryBreadCrumb != curentCategoryBreadCrumb)
                allCategories.Remove(curentCategoryBreadCrumb);

            //search engine name
            if (setSeName)
                await _urlRecordService.SaveSlugAsync(category, await _urlRecordService.ValidateSeNameAsync(category, seName, category.Name, true), 0);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected async Task ImportCategoryLocalizedAsync(Category category, WorkbookMetadata<Category> metadata, PropertyManager<Category, Language> manager, int iRow, IList<Language> languages)
        {
            if (!metadata.LocalizedWorksheets.Any())
                return;

            var setSeName = metadata.LocalizedProperties.Any(p => p.PropertyName == "SeName");
            foreach (var language in languages)
            {
                var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                if (lWorksheet == null)
                    continue;

                manager.CurrentLanguage = language;
                manager.ReadLocalizedFromXlsx(lWorksheet, iRow);

                foreach (var property in manager.GetLocalizedProperties)
                {
                    string localizedName = null;

                    switch (property.PropertyName)
                    {
                        case "Name":
                            localizedName = property.StringValue;
                            await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.Name, localizedName, language.Id);
                            break;
                        case "Description":
                            await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.Description, property.StringValue, language.Id);
                            break;
                        case "MetaKeywords":
                            await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.MetaKeywords, property.StringValue, language.Id);
                            break;
                        case "MetaDescription":
                            await _localizedEntityService.SaveLocalizedValueAsync(category, c => c.MetaDescription, property.StringValue, language.Id);
                            break;
                        case "MetaTitle":
                            await _localizedEntityService.SaveLocalizedValueAsync(category, m => m.MetaTitle, property.StringValue, language.Id);
                            break;
                        case "SeName":
                            //search engine name
                            if (setSeName)
                            {
                                var lSeName = await _urlRecordService.ValidateSeNameAsync(category, property.StringValue, localizedName, false);
                                await _urlRecordService.SaveSlugAsync(category, lSeName, language.Id);
                            }
                            break;
                    }
                }
            }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected async Task ImportManufaturerLocalizedAsync(Manufacturer manufacturer, WorkbookMetadata<Manufacturer> metadata, PropertyManager<Manufacturer, Language> manager, int iRow, IList<Language> languages)
        {
            if (!metadata.LocalizedWorksheets.Any())
                return;

            var setSeName = metadata.LocalizedProperties.Any(p => p.PropertyName == "SeName");
            foreach (var language in languages)
            {
                var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                if (lWorksheet == null)
                    continue;

                manager.CurrentLanguage = language;
                manager.ReadLocalizedFromXlsx(lWorksheet, iRow);

                foreach (var property in manager.GetLocalizedProperties)
                {
                    string localizedName = null;

                    switch (property.PropertyName)
                    {
                        case "Name":
                            localizedName = property.StringValue;
                            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.Name, localizedName, language.Id);
                            break;
                        case "Description":
                            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.Description, property.StringValue, language.Id);
                            break;
                        case "MetaKeywords":
                            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.MetaKeywords, property.StringValue, language.Id);
                            break;
                        case "MetaDescription":
                            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.MetaDescription, property.StringValue, language.Id);
                            break;
                        case "MetaTitle":
                            await _localizedEntityService.SaveLocalizedValueAsync(manufacturer, m => m.MetaTitle, property.StringValue, language.Id);
                            break;
                        case "SeName":
                            //search engine name
                            if (setSeName)
                            {
                                var localizedSeName = await _urlRecordService.ValidateSeNameAsync(manufacturer, property.StringValue, localizedName, false);
                                await _urlRecordService.SaveSlugAsync(manufacturer, localizedSeName, language.Id);
                            }
                            break;
                    }
                }
            }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task SetOutLineForTvChannelAttributeRowAsync(object cellValue, IXLWorksheet worksheet, int endRow)
        {
            try
            {
                var aid = Convert.ToInt32(cellValue ?? -1);

                var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(aid);

                if (tvChannelAttribute != null)
                    worksheet.Row(endRow).OutlineLevel = 1;
            }
            catch (FormatException)
            {
                if ((cellValue ?? string.Empty).ToString() == "AttributeId")
                    worksheet.Row(endRow).OutlineLevel = 1;
            }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task ImportTvChannelAttributeAsync(ImportTvChannelMetadata metadata, TvChannel lastLoadedTvChannel, IList<Language> languages, int iRow)
        {
            var tvChannelAttributeManager = metadata.TvChannelAttributeManager;
            if (!_catalogSettings.ExportImportTvChannelAttributes || lastLoadedTvChannel == null || tvChannelAttributeManager.IsCaption)
                return;

            var tvChannelAttributeId = tvChannelAttributeManager.GetDefaultProperty("AttributeId").IntValue;
            var attributeControlTypeId = tvChannelAttributeManager.GetDefaultProperty("AttributeControlType").IntValue;

            var tvChannelAttributeValueId = tvChannelAttributeManager.GetDefaultProperty("TvChannelAttributeValueId").IntValue;
            var associatedTvChannelId = tvChannelAttributeManager.GetDefaultProperty("AssociatedTvChannelId").IntValue;
            var valueName = tvChannelAttributeManager.GetDefaultProperty("ValueName").StringValue;
            var attributeValueTypeId = tvChannelAttributeManager.GetDefaultProperty("AttributeValueType").IntValue;
            var colorSquaresRgb = tvChannelAttributeManager.GetDefaultProperty("ColorSquaresRgb").StringValue;
            var imageSquaresPictureId = tvChannelAttributeManager.GetDefaultProperty("ImageSquaresPictureId").IntValue;
            var priceAdjustment = tvChannelAttributeManager.GetDefaultProperty("PriceAdjustment").DecimalValue;
            var priceAdjustmentUsePercentage = tvChannelAttributeManager.GetDefaultProperty("PriceAdjustmentUsePercentage").BooleanValue;
            var weightAdjustment = tvChannelAttributeManager.GetDefaultProperty("WeightAdjustment").DecimalValue;
            var cost = tvChannelAttributeManager.GetDefaultProperty("Cost").DecimalValue;
            var userEntersQty = tvChannelAttributeManager.GetDefaultProperty("UserEntersQty").BooleanValue;
            var quantity = tvChannelAttributeManager.GetDefaultProperty("Quantity").IntValue;
            var isPreSelected = tvChannelAttributeManager.GetDefaultProperty("IsPreSelected").BooleanValue;
            var displayOrder = tvChannelAttributeManager.GetDefaultProperty("DisplayOrder").IntValue;
            var pictureId = tvChannelAttributeManager.GetDefaultProperty("PictureId").IntValue;
            var textPrompt = tvChannelAttributeManager.GetDefaultProperty("AttributeTextPrompt").StringValue;
            var isRequired = tvChannelAttributeManager.GetDefaultProperty("AttributeIsRequired").BooleanValue;
            var attributeDisplayOrder = tvChannelAttributeManager.GetDefaultProperty("AttributeDisplayOrder").IntValue;

            var tvChannelAttributeMapping = (await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(lastLoadedTvChannel.Id))
                .FirstOrDefault(pam => pam.TvChannelAttributeId == tvChannelAttributeId);

            if (tvChannelAttributeMapping == null)
            {
                //insert mapping
                tvChannelAttributeMapping = new TvChannelAttributeMapping
                {
                    TvChannelId = lastLoadedTvChannel.Id,
                    TvChannelAttributeId = tvChannelAttributeId,
                    TextPrompt = textPrompt,
                    IsRequired = isRequired,
                    AttributeControlTypeId = attributeControlTypeId,
                    DisplayOrder = attributeDisplayOrder
                };
                await _tvChannelAttributeService.InsertTvChannelAttributeMappingAsync(tvChannelAttributeMapping);
            }
            else
            {
                tvChannelAttributeMapping.AttributeControlTypeId = attributeControlTypeId;
                tvChannelAttributeMapping.TextPrompt = textPrompt;
                tvChannelAttributeMapping.IsRequired = isRequired;
                tvChannelAttributeMapping.DisplayOrder = attributeDisplayOrder;
                await _tvChannelAttributeService.UpdateTvChannelAttributeMappingAsync(tvChannelAttributeMapping);
            }

            var pav = (await _tvChannelAttributeService.GetTvChannelAttributeValuesAsync(tvChannelAttributeMapping.Id))
                .FirstOrDefault(p => p.Id == tvChannelAttributeValueId);

            //var pav = await _tvChannelAttributeService.GetTvChannelAttributeValueByIdAsync(tvChannelAttributeValueId);

            var attributeControlType = (AttributeControlType)attributeControlTypeId;

            if (pav == null)
            {
                switch (attributeControlType)
                {
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    case AttributeControlType.MultilineTextbox:
                    case AttributeControlType.TextBox:
                        if (tvChannelAttributeMapping.ValidationRulesAllowed())
                        {
                            tvChannelAttributeMapping.ValidationMinLength = tvChannelAttributeManager.GetDefaultProperty("ValidationMinLength")?.IntValueNullable;
                            tvChannelAttributeMapping.ValidationMaxLength = tvChannelAttributeManager.GetDefaultProperty("ValidationMaxLength")?.IntValueNullable;
                            tvChannelAttributeMapping.ValidationFileMaximumSize = tvChannelAttributeManager.GetDefaultProperty("ValidationFileMaximumSize")?.IntValueNullable;
                            tvChannelAttributeMapping.ValidationFileAllowedExtensions = tvChannelAttributeManager.GetDefaultProperty("ValidationFileAllowedExtensions")?.StringValue;
                            tvChannelAttributeMapping.DefaultValue = tvChannelAttributeManager.GetDefaultProperty("DefaultValue")?.StringValue;

                            await _tvChannelAttributeService.UpdateTvChannelAttributeMappingAsync(tvChannelAttributeMapping);
                        }

                        return;
                }

                pav = new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = tvChannelAttributeMapping.Id,
                    AttributeValueType = (AttributeValueType)attributeValueTypeId,
                    AssociatedTvChannelId = associatedTvChannelId,
                    Name = valueName,
                    PriceAdjustment = priceAdjustment,
                    PriceAdjustmentUsePercentage = priceAdjustmentUsePercentage,
                    WeightAdjustment = weightAdjustment,
                    Cost = cost,
                    IsPreSelected = isPreSelected,
                    DisplayOrder = displayOrder,
                    ColorSquaresRgb = colorSquaresRgb,
                    ImageSquaresPictureId = imageSquaresPictureId,
                    UserEntersQty = userEntersQty,
                    Quantity = quantity,
                    PictureId = pictureId
                };

                await _tvChannelAttributeService.InsertTvChannelAttributeValueAsync(pav);
            }
            else
            {
                pav.AttributeValueTypeId = attributeValueTypeId;
                pav.AssociatedTvChannelId = associatedTvChannelId;
                pav.Name = valueName;
                pav.ColorSquaresRgb = colorSquaresRgb;
                pav.ImageSquaresPictureId = imageSquaresPictureId;
                pav.PriceAdjustment = priceAdjustment;
                pav.PriceAdjustmentUsePercentage = priceAdjustmentUsePercentage;
                pav.WeightAdjustment = weightAdjustment;
                pav.Cost = cost;
                pav.UserEntersQty = userEntersQty;
                pav.Quantity = quantity;
                pav.IsPreSelected = isPreSelected;
                pav.DisplayOrder = displayOrder;
                pav.PictureId = pictureId;

                await _tvChannelAttributeService.UpdateTvChannelAttributeValueAsync(pav);
            }

            if (!metadata.LocalizedWorksheets.Any())
                return;

            foreach (var language in languages)
            {
                var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                if (lWorksheet == null)
                    continue;

                tvChannelAttributeManager.CurrentLanguage = language;
                tvChannelAttributeManager.ReadLocalizedFromXlsx(lWorksheet, iRow, ExportTvChannelAttribute.TvChannelAttributeCellOffset);

                valueName = tvChannelAttributeManager.GetLocalizedProperty("ValueName").StringValue;
                textPrompt = tvChannelAttributeManager.GetLocalizedProperty("AttributeTextPrompt").StringValue;

                await _localizedEntityService.SaveLocalizedValueAsync(pav, p => p.Name, valueName, language.Id);
                await _localizedEntityService.SaveLocalizedValueAsync(tvChannelAttributeMapping, p => p.TextPrompt, textPrompt, language.Id);

                switch (attributeControlType)
                {
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    case AttributeControlType.MultilineTextbox:
                    case AttributeControlType.TextBox:
                        if (tvChannelAttributeMapping.ValidationRulesAllowed())
                        {
                            var defaultValue = tvChannelAttributeManager.GetLocalizedProperty("DefaultValue")?.StringValue;
                            await _localizedEntityService.SaveLocalizedValueAsync(tvChannelAttributeMapping, p => p.DefaultValue, defaultValue, language.Id);
                        }

                        return;
                }
            }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task ImportSpecificationAttributeAsync(ImportTvChannelMetadata metadata, TvChannel lastLoadedTvChannel, IList<Language> languages, int iRow)
        {
            var specificationAttributeManager = metadata.SpecificationAttributeManager;
            if (!_catalogSettings.ExportImportTvChannelSpecificationAttributes || lastLoadedTvChannel == null || specificationAttributeManager.IsCaption)
                return;

            var attributeTypeId = specificationAttributeManager.GetDefaultProperty("AttributeType").IntValue;
            var allowFiltering = specificationAttributeManager.GetDefaultProperty("AllowFiltering").BooleanValue;
            var specificationAttributeOptionId = specificationAttributeManager.GetDefaultProperty("SpecificationAttributeOptionId").IntValue;
            var tvChannelId = lastLoadedTvChannel.Id;
            var customValue = specificationAttributeManager.GetDefaultProperty("CustomValue").StringValue;
            var displayOrder = specificationAttributeManager.GetDefaultProperty("DisplayOrder").IntValue;
            var showOnTvChannelPage = specificationAttributeManager.GetDefaultProperty("ShowOnTvChannelPage").BooleanValue;

            //if specification attribute option isn't set, try to get first of possible specification attribute option for current specification attribute
            if (specificationAttributeOptionId == 0)
            {
                var specificationAttribute = specificationAttributeManager.GetDefaultProperty("SpecificationAttribute").IntValue;
                specificationAttributeOptionId =
                    (await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(
                        specificationAttribute))
                    .FirstOrDefault()?.Id ?? specificationAttributeOptionId;
            }

            var tvChannelSpecificationAttribute = specificationAttributeOptionId == 0
                ? null
                : (await _specificationAttributeService.GetTvChannelSpecificationAttributesAsync(tvChannelId, specificationAttributeOptionId)).FirstOrDefault();

            var isNew = tvChannelSpecificationAttribute == null;

            if (isNew)
                tvChannelSpecificationAttribute = new TvChannelSpecificationAttribute();

            if (attributeTypeId != (int)SpecificationAttributeType.Option)
                //we allow filtering only for "Option" attribute type
                allowFiltering = false;

            //we don't allow CustomValue for "Option" attribute type
            if (attributeTypeId == (int)SpecificationAttributeType.Option)
                customValue = null;

            tvChannelSpecificationAttribute.AttributeTypeId = attributeTypeId;
            tvChannelSpecificationAttribute.SpecificationAttributeOptionId = specificationAttributeOptionId;
            tvChannelSpecificationAttribute.TvChannelId = tvChannelId;
            tvChannelSpecificationAttribute.CustomValue = customValue;
            tvChannelSpecificationAttribute.AllowFiltering = allowFiltering;
            tvChannelSpecificationAttribute.ShowOnTvChannelPage = showOnTvChannelPage;
            tvChannelSpecificationAttribute.DisplayOrder = displayOrder;

            if (isNew)
                await _specificationAttributeService.InsertTvChannelSpecificationAttributeAsync(tvChannelSpecificationAttribute);
            else
                await _specificationAttributeService.UpdateTvChannelSpecificationAttributeAsync(tvChannelSpecificationAttribute);

            if (!metadata.LocalizedWorksheets.Any())
                return;

            foreach (var language in languages)
            {
                var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                if (lWorksheet == null)
                    continue;

                specificationAttributeManager.CurrentLanguage = language;
                specificationAttributeManager.ReadLocalizedFromXlsx(lWorksheet, iRow, ExportTvChannelAttribute.TvChannelAttributeCellOffset);

                customValue = specificationAttributeManager.GetLocalizedProperty("CustomValue").StringValue;
                await _localizedEntityService.SaveLocalizedValueAsync(tvChannelSpecificationAttribute, p => p.CustomValue, customValue, language.Id);
            }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task<string> DownloadFileAsync(string urlString, IList<string> downloadedFiles)
        {
            if (string.IsNullOrEmpty(urlString))
                return string.Empty;

            if (!Uri.IsWellFormedUriString(urlString, UriKind.Absolute))
                return urlString;

            if (!_catalogSettings.ExportImportAllowDownloadImages)
                return string.Empty;

            //ensure that temp directory is created
            var tempDirectory = _fileProvider.MapPath(ExportImportDefaults.UploadsTempPath);
            _fileProvider.CreateDirectory(tempDirectory);

            var fileName = _fileProvider.GetFileName(urlString);
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            var filePath = _fileProvider.Combine(tempDirectory, fileName);
            try
            {
                var client = _httpClientFactory.CreateClient(TvProgHttpDefaults.DefaultHttpClient);
                var fileData = await client.GetByteArrayAsync(urlString);
                await using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                    fs.Write(fileData, 0, fileData.Length);

                downloadedFiles?.Add(filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("Download image failed", ex);
            }

            return string.Empty;
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task<ImportTvChannelMetadata> PrepareImportTvChannelDataAsync(IXLWorkbook workbook, IList<Language> languages)
        {
            //the columns
            var metadata = GetWorkbookMetadata<TvChannel>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<TvChannel, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var tvChannelAttributeProperties = new[]
            {
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeId"),
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeName"),
                new PropertyByName<ExportTvChannelAttribute, Language>("DefaultValue"),
                new PropertyByName<ExportTvChannelAttribute, Language>("ValidationMinLength"),
                new PropertyByName<ExportTvChannelAttribute, Language>("ValidationMaxLength"),
                new PropertyByName<ExportTvChannelAttribute, Language>("ValidationFileAllowedExtensions"),
                new PropertyByName<ExportTvChannelAttribute, Language>("ValidationFileMaximumSize"),
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeTextPrompt"),
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeIsRequired"),
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeControlType"),
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeDisplayOrder"),
                new PropertyByName<ExportTvChannelAttribute, Language>("TvChannelAttributeValueId"),
                new PropertyByName<ExportTvChannelAttribute, Language>("ValueName"),
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeValueType"),
                new PropertyByName<ExportTvChannelAttribute, Language>("AssociatedTvChannelId"),
                new PropertyByName<ExportTvChannelAttribute, Language>("ColorSquaresRgb"),
                new PropertyByName<ExportTvChannelAttribute, Language>("ImageSquaresPictureId"),
                new PropertyByName<ExportTvChannelAttribute, Language>("PriceAdjustment"),
                new PropertyByName<ExportTvChannelAttribute, Language>("PriceAdjustmentUsePercentage"),
                new PropertyByName<ExportTvChannelAttribute, Language>("WeightAdjustment"),
                new PropertyByName<ExportTvChannelAttribute, Language>("Cost"),
                new PropertyByName<ExportTvChannelAttribute, Language>("UserEntersQty"),
                new PropertyByName<ExportTvChannelAttribute, Language>("Quantity"),
                new PropertyByName<ExportTvChannelAttribute, Language>("IsPreSelected"),
                new PropertyByName<ExportTvChannelAttribute, Language>("DisplayOrder"),
                new PropertyByName<ExportTvChannelAttribute, Language>("PictureId")
            };

            var tvChannelAttributeLocalizedProperties = new[]
            {
                new PropertyByName<ExportTvChannelAttribute, Language>("DefaultValue"),
                new PropertyByName<ExportTvChannelAttribute, Language>("AttributeTextPrompt"),
                new PropertyByName<ExportTvChannelAttribute, Language>("ValueName")
            };

            var tvChannelAttributeManager = new PropertyManager<ExportTvChannelAttribute, Language>(tvChannelAttributeProperties, _catalogSettings, tvChannelAttributeLocalizedProperties, languages);

            var specificationAttributeProperties = new[]
            {
                new PropertyByName<ExportSpecificationAttribute, Language>("AttributeType", (p, l) => p.AttributeTypeId),
                new PropertyByName<ExportSpecificationAttribute, Language>("SpecificationAttribute", (p, l) => p.SpecificationAttributeId),
                new PropertyByName<ExportSpecificationAttribute, Language>("CustomValue", (p, l) => p.CustomValue),
                new PropertyByName<ExportSpecificationAttribute, Language>("SpecificationAttributeOptionId", (p, l) => p.SpecificationAttributeOptionId),
                new PropertyByName<ExportSpecificationAttribute, Language>("AllowFiltering", (p, l) => p.AllowFiltering),
                new PropertyByName<ExportSpecificationAttribute, Language>("ShowOnTvChannelPage", (p, l) => p.ShowOnTvChannelPage),
                new PropertyByName<ExportSpecificationAttribute, Language>("DisplayOrder", (p, l) => p.DisplayOrder)
            };

            var specificationAttributeLocalizedProperties = new[]
            {
                new PropertyByName<ExportSpecificationAttribute, Language>("CustomValue")
            };

            var specificationAttributeManager = new PropertyManager<ExportSpecificationAttribute, Language>(specificationAttributeProperties, _catalogSettings, specificationAttributeLocalizedProperties, languages);

            var endRow = 2;
            var allCategories = new List<string>();
            var allSku = new List<string>();

            var tempProperty = manager.GetDefaultProperty("Categories");
            var categoryCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            tempProperty = manager.GetDefaultProperty("SKU");
            var skuCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            var allManufacturers = new List<string>();
            tempProperty = manager.GetDefaultProperty("Manufacturers");
            var manufacturerCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            var allStores = new List<string>();
            tempProperty = manager.GetDefaultProperty("LimitedToStores");
            var limitedToStoresCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            if (_catalogSettings.ExportImportUseDropdownlistsForAssociatedEntities)
            {
                tvChannelAttributeManager.SetSelectList("AttributeControlType", await AttributeControlType.TextBox.ToSelectListAsync(useLocalization: false));
                tvChannelAttributeManager.SetSelectList("AttributeValueType", await AttributeValueType.Simple.ToSelectListAsync(useLocalization: false));

                specificationAttributeManager.SetSelectList("AttributeType", await SpecificationAttributeType.Option.ToSelectListAsync(useLocalization: false));
                specificationAttributeManager.SetSelectList("SpecificationAttribute", (await _specificationAttributeService
                    .GetSpecificationAttributesAsync())
                    .Select(sa => sa as BaseEntity)
                    .ToSelectList(p => (p as SpecificationAttribute)?.Name ?? string.Empty));

                manager.SetSelectList("TvChannelType", await TvChannelType.SimpleTvChannel.ToSelectListAsync(useLocalization: false));
                manager.SetSelectList("GiftCardType", await GiftCardType.Virtual.ToSelectListAsync(useLocalization: false));
                manager.SetSelectList("DownloadActivationType",
                    await DownloadActivationType.Manually.ToSelectListAsync(useLocalization: false));
                manager.SetSelectList("ManageInventoryMethod",
                    await ManageInventoryMethod.DontManageStock.ToSelectListAsync(useLocalization: false));
                manager.SetSelectList("LowStockActivity",
                    await LowStockActivity.Nothing.ToSelectListAsync(useLocalization: false));
                manager.SetSelectList("BackorderMode", await BackorderMode.NoBackorders.ToSelectListAsync(useLocalization: false));
                manager.SetSelectList("RecurringCyclePeriod",
                    await RecurringTvChannelCyclePeriod.Days.ToSelectListAsync(useLocalization: false));
                manager.SetSelectList("RentalPricePeriod", await RentalPricePeriod.Days.ToSelectListAsync(useLocalization: false));

                manager.SetSelectList("Vendor",
                    (await _vendorService.GetAllVendorsAsync(showHidden: true)).Select(v => v as BaseEntity)
                        .ToSelectList(p => (p as Vendor)?.Name ?? string.Empty));
                manager.SetSelectList("TvChannelTemplate",
                    (await _tvChannelTemplateService.GetAllTvChannelTemplatesAsync()).Select(pt => pt as BaseEntity)
                        .ToSelectList(p => (p as TvChannelTemplate)?.Name ?? string.Empty));
                manager.SetSelectList("DeliveryDate",
                    (await _dateRangeService.GetAllDeliveryDatesAsync()).Select(dd => dd as BaseEntity)
                        .ToSelectList(p => (p as DeliveryDate)?.Name ?? string.Empty));
                manager.SetSelectList("TvChannelAvailabilityRange",
                    (await _dateRangeService.GetAllTvChannelAvailabilityRangesAsync()).Select(range => range as BaseEntity)
                        .ToSelectList(p => (p as TvChannelAvailabilityRange)?.Name ?? string.Empty));
                manager.SetSelectList("TaxCategory",
                    (await _taxCategoryService.GetAllTaxCategoriesAsync()).Select(tc => tc as BaseEntity)
                        .ToSelectList(p => (p as TaxCategory)?.Name ?? string.Empty));
                manager.SetSelectList("BasepriceUnit",
                    (await _measureService.GetAllMeasureWeightsAsync()).Select(mw => mw as BaseEntity)
                        .ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
                manager.SetSelectList("BasepriceBaseUnit",
                    (await _measureService.GetAllMeasureWeightsAsync()).Select(mw => mw as BaseEntity)
                        .ToSelectList(p => (p as MeasureWeight)?.Name ?? string.Empty));
            }

            var allAttributeIds = new List<int>();
            var allSpecificationAttributeOptionIds = new List<int>();

            var attributeIdCellNum = 1 + ExportTvChannelAttribute.TvChannelAttributeCellOffset;
            var specificationAttributeOptionIdCellNum =
                specificationAttributeManager.GetIndex("SpecificationAttributeOptionId") +
                ExportTvChannelAttribute.TvChannelAttributeCellOffset;

            var tvChannelsInFile = new List<int>();

            //find end of data
            var typeOfExportedAttribute = ExportedAttributeType.NotSpecified;
            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                    .Select(property => defaultWorksheet.Row(endRow).Cell(property.PropertyOrderPosition))
                    .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString()));

                if (allColumnsAreEmpty)
                    break;

                if (new[] { 1, 2 }.Select(cellNum => defaultWorksheet.Row(endRow).Cell(cellNum))
                        .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString())) &&
                    defaultWorksheet.Row(endRow).OutlineLevel == 0)
                {
                    var cellValue = defaultWorksheet.Row(endRow).Cell(attributeIdCellNum).Value;
                    await SetOutLineForTvChannelAttributeRowAsync(cellValue, defaultWorksheet, endRow);
                    await SetOutLineForSpecificationAttributeRowAsync(cellValue, defaultWorksheet, endRow);
                }

                if (defaultWorksheet.Row(endRow).OutlineLevel != 0)
                {
                    var newTypeOfExportedAttribute = GetTypeOfExportedAttribute(defaultWorksheet, metadata.LocalizedWorksheets, tvChannelAttributeManager, specificationAttributeManager, endRow);

                    //skip caption row
                    if (newTypeOfExportedAttribute != ExportedAttributeType.NotSpecified && newTypeOfExportedAttribute != typeOfExportedAttribute)
                    {
                        typeOfExportedAttribute = newTypeOfExportedAttribute;
                        endRow++;
                        continue;
                    }

                    switch (typeOfExportedAttribute)
                    {
                        case ExportedAttributeType.TvChannelAttribute:
                            tvChannelAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, endRow,
                                ExportTvChannelAttribute.TvChannelAttributeCellOffset);
                            if (int.TryParse((defaultWorksheet.Row(endRow).Cell(attributeIdCellNum).Value ?? string.Empty).ToString(), out var aid))
                            {
                                allAttributeIds.Add(aid);
                            }

                            break;
                        case ExportedAttributeType.SpecificationAttribute:
                            specificationAttributeManager.ReadDefaultFromXlsx(defaultWorksheet, endRow, ExportTvChannelAttribute.TvChannelAttributeCellOffset);

                            if (int.TryParse((defaultWorksheet.Row(endRow).Cell(specificationAttributeOptionIdCellNum).Value ?? string.Empty).ToString(), out var saoid))
                            {
                                allSpecificationAttributeOptionIds.Add(saoid);
                            }

                            break;
                    }

                    endRow++;
                    continue;
                }

                if (categoryCellNum > 0)
                {
                    var categoryIds = defaultWorksheet.Row(endRow).Cell(categoryCellNum).Value?.ToString() ?? string.Empty;

                    if (!string.IsNullOrEmpty(categoryIds))
                        allCategories.AddRange(categoryIds
                            .Split(new[] { ";", ">>" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                            .Distinct());
                }

                if (skuCellNum > 0)
                {
                    var sku = defaultWorksheet.Row(endRow).Cell(skuCellNum).Value?.ToString() ?? string.Empty;

                    if (!string.IsNullOrEmpty(sku))
                        allSku.Add(sku);
                }

                if (manufacturerCellNum > 0)
                {
                    var manufacturerIds = defaultWorksheet.Row(endRow).Cell(manufacturerCellNum).Value?.ToString() ??
                                          string.Empty;
                    if (!string.IsNullOrEmpty(manufacturerIds))
                        allManufacturers.AddRange(manufacturerIds
                            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                }

                if (limitedToStoresCellNum > 0)
                {
                    var storeIds = defaultWorksheet.Row(endRow).Cell(limitedToStoresCellNum).Value?.ToString() ??
                                          string.Empty;
                    if (!string.IsNullOrEmpty(storeIds))
                        allStores.AddRange(storeIds
                            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                }

                //counting the number of tvChannels
                tvChannelsInFile.Add(endRow);

                endRow++;
            }

            //performance optimization, the check for the existence of the categories in one SQL request
            var notExistingCategories = await _categoryService.GetNotExistingCategoriesAsync(allCategories.ToArray());
            if (notExistingCategories.Any())
            {
                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Import.CategoriesDontExist"), string.Join(", ", notExistingCategories)));
            }

            //performance optimization, the check for the existence of the manufacturers in one SQL request
            var notExistingManufacturers = await _manufacturerService.GetNotExistingManufacturersAsync(allManufacturers.ToArray());
            if (notExistingManufacturers.Any())
            {
                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Import.ManufacturersDontExist"), string.Join(", ", notExistingManufacturers)));
            }

            //performance optimization, the check for the existence of the tvChannel attributes in one SQL request
            var notExistingTvChannelAttributes = await _tvChannelAttributeService.GetNotExistingAttributesAsync(allAttributeIds.ToArray());
            if (notExistingTvChannelAttributes.Any())
            {
                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Import.TvChannelAttributesDontExist"), string.Join(", ", notExistingTvChannelAttributes)));
            }

            //performance optimization, the check for the existence of the specification attribute options in one SQL request
            var notExistingSpecificationAttributeOptions = await _specificationAttributeService.GetNotExistingSpecificationAttributeOptionsAsync(allSpecificationAttributeOptionIds.Where(saoId => saoId != 0).ToArray());
            if (notExistingSpecificationAttributeOptions.Any())
            {
                throw new ArgumentException($"The following specification attribute option ID(s) don't exist - {string.Join(", ", notExistingSpecificationAttributeOptions)}");
            }

            //performance optimization, the check for the existence of the stores in one SQL request
            var notExistingStores = await _storeService.GetNotExistingStoresAsync(allStores.ToArray());
            if (notExistingStores.Any())
            {
                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Import.StoresDontExist"), string.Join(", ", notExistingStores)));
            }

            return new ImportTvChannelMetadata
            {
                EndRow = endRow,
                Manager = manager,
                Properties = defaultProperties,
                TvChannelsInFile = tvChannelsInFile,
                TvChannelAttributeManager = tvChannelAttributeManager,
                DefaultWorksheet = defaultWorksheet,
                LocalizedWorksheets = metadata.LocalizedWorksheets,
                SpecificationAttributeManager = specificationAttributeManager,
                SkuCellNum = skuCellNum,
                AllSku = allSku
            };
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task ImportTvChannelsFromSplitedXlsxAsync(IXLWorksheet worksheet, ImportTvChannelMetadata metadata)
        {
            foreach (var path in SplitTvChannelFile(worksheet, metadata))
            {
                using var scope = _serviceScopeFactory.CreateScope();
                // Resolve
                var importManager = EngineContext.Current.Resolve<IImportManager>(scope);

                using var sr = new StreamReader(path);
                await importManager.ImportTvChannelsFromXlsxAsync(sr.BaseStream);

                try
                {
                    _fileProvider.DeleteFile(path);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private IList<string> SplitTvChannelFile(IXLWorksheet worksheet, ImportTvChannelMetadata metadata)
        {
            var fileIndex = 1;
            var fileName = Guid.NewGuid().ToString();
            var endCell = metadata.Properties.Max(p => p.PropertyOrderPosition);

            var filePaths = new List<string>();

            while (true)
            {
                var curIndex = fileIndex * _catalogSettings.ExportImportTvChannelsCountInOneFile;

                var startRow = metadata.TvChannelsInFile[(fileIndex - 1) * _catalogSettings.ExportImportTvChannelsCountInOneFile];

                var endRow = metadata.CountTvChannelsInFile > curIndex + 1
                    ? metadata.TvChannelsInFile[curIndex - 1]
                    : metadata.EndRow;

                var filePath = $"{_fileProvider.MapPath(ExportImportDefaults.UploadsTempPath)}/{fileName}_part_{fileIndex}.xlsx";

                CopyDataToNewFile(metadata, worksheet, filePath, startRow, endRow, endCell);

                filePaths.Add(filePath);
                fileIndex += 1;

                if (endRow == metadata.EndRow)
                    break;
            }

            return filePaths;
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        private async Task<(ImportOrderMetadata, IXLWorksheet)> PrepareImportOrderDataAsync(IXLWorkbook workbook)
        {
            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<Order>(workbook, languages);
            var worksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;

            var manager = new PropertyManager<Order, Language>(defaultProperties, _catalogSettings);

            var orderItemProperties = new[]
            {
                new PropertyByName<OrderItem, Language>("OrderItemGuid"),
                new PropertyByName<OrderItem, Language>("Name"),
                new PropertyByName<OrderItem, Language>("Sku"),
                new PropertyByName<OrderItem, Language>("PriceExclTax"),
                new PropertyByName<OrderItem, Language>("PriceInclTax"),
                new PropertyByName<OrderItem, Language>("Quantity"),
                new PropertyByName<OrderItem, Language>("DiscountExclTax"),
                new PropertyByName<OrderItem, Language>("DiscountInclTax"),
                new PropertyByName<OrderItem, Language>("TotalExclTax"),
                new PropertyByName<OrderItem, Language>("TotalInclTax")
            };

            var orderItemManager = new PropertyManager<OrderItem, Language>(orderItemProperties, _catalogSettings);

            var endRow = 2;
            var allOrderGuids = new List<Guid>();

            var tempProperty = manager.GetDefaultProperty("OrderGuid");
            var orderGuidCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            tempProperty = manager.GetDefaultProperty("UserGuid");
            var userGuidCellNum = tempProperty?.PropertyOrderPosition ?? -1;

            manager.SetSelectList("OrderStatus", await OrderStatus.Cancelled.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("ShippingStatus", await ShippingStatus.Delivered.ToSelectListAsync(useLocalization: false));
            manager.SetSelectList("PaymentStatus", await PaymentStatus.Authorized.ToSelectListAsync(useLocalization: false));

            var allUserGuids = new List<Guid>();

            var allOrderItemSkus = new List<string>();

            var countOrdersInFile = 0;

            //find end of data
            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                    .Select(property => worksheet.Row(endRow).Cell(property.PropertyOrderPosition))
                    .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString()));

                if (allColumnsAreEmpty)
                    break;

                if (worksheet.Row(endRow).OutlineLevel != 0)
                {
                    orderItemManager.ReadDefaultFromXlsx(worksheet, endRow, 2);

                    //skip caption row
                    if (!orderItemManager.IsCaption)
                    {
                        allOrderItemSkus.Add(orderItemManager.GetDefaultProperty("Sku").StringValue);
                    }

                    endRow++;
                    continue;
                }

                if (orderGuidCellNum > 0)
                {
                    var orderGuidString = worksheet.Row(endRow).Cell(orderGuidCellNum).Value?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(orderGuidString) && Guid.TryParse(orderGuidString, out var orderGuid))
                        allOrderGuids.Add(orderGuid);
                }

                if (userGuidCellNum > 0)
                {
                    var userGuidString = worksheet.Row(endRow).Cell(userGuidCellNum).Value?.ToString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(userGuidString) && Guid.TryParse(userGuidString, out var userGuid))
                        allUserGuids.Add(userGuid);
                }

                //counting the number of orders
                countOrdersInFile++;

                endRow++;
            }

            //performance optimization, the check for the existence of the users in one SQL request
            var notExistingUserGuids = await _userService.GetNotExistingUsersAsync(allUserGuids.ToArray());
            if (notExistingUserGuids.Any())
            {
                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Orders.Import.UsersDontExist"), string.Join(", ", notExistingUserGuids)));
            }

            //performance optimization, the check for the existence of the order items in one SQL request
            var notExistingTvChannelSkus = await _tvChannelService.GetNotExistingTvChannelsAsync(allOrderItemSkus.ToArray());
            if (notExistingTvChannelSkus.Any())
            {
                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Orders.Import.TvChannelsDontExist"), string.Join(", ", notExistingTvChannelSkus)));
            }

            return (new ImportOrderMetadata
            {
                EndRow = endRow,
                Manager = manager,
                Properties = defaultProperties,
                CountOrdersInFile = countOrdersInFile,
                OrderItemManager = orderItemManager,
                AllOrderGuids = allOrderGuids,
                AllUserGuids = allUserGuids
            }, worksheet);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task ImportOrderItemAsync(PropertyManager<OrderItem, Language> orderItemManager, Order lastLoadedOrder)
        {
            if (lastLoadedOrder == null || orderItemManager.IsCaption)
                return;

            var orderItemGuid = Guid.TryParse(orderItemManager.GetDefaultProperty("OrderItemGuid").StringValue, out var guidValue) ? guidValue : Guid.NewGuid();
            var sku = orderItemManager.GetDefaultProperty("Sku").StringValue;
            var priceExclTax = orderItemManager.GetDefaultProperty("PriceExclTax").DecimalValue;
            var priceInclTax = orderItemManager.GetDefaultProperty("PriceInclTax").DecimalValue;
            var quantity = orderItemManager.GetDefaultProperty("Quantity").IntValue;
            var discountExclTax = orderItemManager.GetDefaultProperty("DiscountExclTax").DecimalValue;
            var discountInclTax = orderItemManager.GetDefaultProperty("DiscountInclTax").DecimalValue;
            var totalExclTax = orderItemManager.GetDefaultProperty("TotalExclTax").DecimalValue;
            var totalInclTax = orderItemManager.GetDefaultProperty("TotalInclTax").DecimalValue;

            var orderItemTvChannel = await _tvChannelService.GetTvChannelBySkuAsync(sku);
            var orderItem = (await _orderService.GetOrderItemsAsync(lastLoadedOrder.Id)).FirstOrDefault(f => f.OrderItemGuid == orderItemGuid);

            if (orderItem == null)
            {
                //insert order item
                orderItem = new OrderItem
                {
                    DiscountAmountExclTax = discountExclTax,
                    DiscountAmountInclTax = discountInclTax,
                    OrderId = lastLoadedOrder.Id,
                    OrderItemGuid = orderItemGuid,
                    PriceExclTax = totalExclTax,
                    PriceInclTax = totalInclTax,
                    TvChannelId = orderItemTvChannel.Id,
                    Quantity = quantity,
                    OriginalTvChannelCost = orderItemTvChannel.TvChannelCost,
                    UnitPriceExclTax = priceExclTax,
                    UnitPriceInclTax = priceInclTax
                };
                await _orderService.InsertOrderItemAsync(orderItem);
            }
            else
            {
                //update order item
                orderItem.DiscountAmountExclTax = discountExclTax;
                orderItem.DiscountAmountInclTax = discountInclTax;
                orderItem.OrderId = lastLoadedOrder.Id;
                orderItem.PriceExclTax = totalExclTax;
                orderItem.PriceInclTax = totalInclTax;
                orderItem.Quantity = quantity;
                orderItem.UnitPriceExclTax = priceExclTax;
                orderItem.UnitPriceInclTax = priceInclTax;
                await _orderService.UpdateOrderItemAsync(orderItem);
            }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected async Task ImportTvChannelLocalizedAsync(TvChannel tvChannel, ImportTvChannelMetadata metadata, int iRow, IList<Language> languages)
        {
            if (metadata.LocalizedWorksheets.Any())
            {
                var manager = metadata.Manager;
                foreach (var language in languages)
                {
                    var lWorksheet = metadata.LocalizedWorksheets.FirstOrDefault(ws => ws.Name.Equals(language.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
                    if (lWorksheet == null)
                        continue;

                    manager.CurrentLanguage = language;
                    manager.ReadLocalizedFromXlsx(lWorksheet, iRow);

                    foreach (var property in manager.GetLocalizedProperties)
                    {
                        string localizedName = null;

                        switch (property.PropertyName)
                        {
                            case "Name":
                                localizedName = property.StringValue;
                                await _localizedEntityService.SaveLocalizedValueAsync(tvChannel, p => p.Name, localizedName, language.Id);
                                break;
                            case "ShortDescription":
                                await _localizedEntityService.SaveLocalizedValueAsync(tvChannel, p => p.ShortDescription, property.StringValue, language.Id);
                                break;
                            case "FullDescription":
                                await _localizedEntityService.SaveLocalizedValueAsync(tvChannel, p => p.FullDescription, property.StringValue, language.Id);
                                break;
                            case "MetaKeywords":
                                await _localizedEntityService.SaveLocalizedValueAsync(tvChannel, p => p.MetaKeywords, property.StringValue, language.Id);
                                break;
                            case "MetaDescription":
                                await _localizedEntityService.SaveLocalizedValueAsync(tvChannel, p => p.MetaDescription, property.StringValue, language.Id);
                                break;
                            case "MetaTitle":
                                await _localizedEntityService.SaveLocalizedValueAsync(tvChannel, p => p.MetaTitle, property.StringValue, language.Id);
                                break;
                            case "SeName":
                                //search engine name
                                var localizedSeName = await _urlRecordService.ValidateSeNameAsync(tvChannel, property.StringValue, localizedName, false);
                                await _urlRecordService.SaveSlugAsync(tvChannel, localizedSeName, language.Id);
                                break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get excel workbook metadata
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="workbook">Excel workbook</param>
        /// <param name="languages">Languages</param>
        /// <returns>Workbook metadata</returns>
        public static WorkbookMetadata<T> GetWorkbookMetadata<T>(IXLWorkbook workbook, IList<Language> languages)
        {
            // get the first worksheet in the workbook
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                throw new TvProgException("No worksheet found");

            var properties = new List<PropertyByName<T, Language>>();
            var localizedProperties = new List<PropertyByName<T, Language>>();
            var localizedWorksheets = new List<IXLWorksheet>();

            var poz = 1;
            while (true)
            {
                try
                {
                    var cell = worksheet.Row(1).Cell(poz);

                    if (string.IsNullOrEmpty(cell?.Value?.ToString()))
                        break;

                    poz += 1;
                    properties.Add(new PropertyByName<T, Language>(cell.Value.ToString()));
                }
                catch
                {
                    break;
                }
            }

            foreach (var ws in workbook.Worksheets.Skip(1))
                if (languages.Any(l => l.UniqueSeoCode.Equals(ws.Name, StringComparison.InvariantCultureIgnoreCase)))
                    localizedWorksheets.Add(ws);

            if (localizedWorksheets.Any())
            {
                // get the first worksheet in the workbook
                var localizedWorksheet = localizedWorksheets.First();

                poz = 1;
                while (true)
                {
                    try
                    {
                        var cell = localizedWorksheet.Row(1).Cell(poz);

                        if (string.IsNullOrEmpty(cell?.Value?.ToString()))
                            break;

                        poz += 1;
                        localizedProperties.Add(new PropertyByName<T, Language>(cell.Value.ToString()));
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            return new WorkbookMetadata<T>()
            {
                DefaultProperties = properties,
                LocalizedProperties = localizedProperties,
                DefaultWorksheet = worksheet,
                LocalizedWorksheets = localizedWorksheets
            };
        }

        /// <summary>
        /// Import tvChannels from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task ImportTvChannelsFromXlsxAsync(Stream stream)
        {
            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            using var workbook = new XLWorkbook(stream);
            var downloadedFiles = new List<string>();

            var metadata = await PrepareImportTvChannelDataAsync(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;

            if (_catalogSettings.ExportImportSplitTvChannelsFile && metadata.CountTvChannelsInFile > _catalogSettings.ExportImportTvChannelsCountInOneFile)
            {
                await ImportTvChannelsFromSplitedXlsxAsync(defaultWorksheet, metadata);
                return;
            }

            //performance optimization, load all tvChannels by SKU in one SQL request
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            var allTvChannelsBySku = await _tvChannelService.GetTvChannelsBySkuAsync(metadata.AllSku.ToArray(), currentVendor?.Id ?? 0);

            //validate maximum number of tvChannels per vendor
            if (_vendorSettings.MaximumTvChannelNumber > 0 &&
                currentVendor != null)
            {
                var newTvChannelsCount = metadata.CountTvChannelsInFile - allTvChannelsBySku.Count;
                if (await _tvChannelService.GetNumberOfTvChannelsByVendorIdAsync(currentVendor.Id) + newTvChannelsCount > _vendorSettings.MaximumTvChannelNumber)
                    throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.ExceededMaximumNumber"), _vendorSettings.MaximumTvChannelNumber));
            }

            //performance optimization, load all categories IDs for tvChannels in one SQL request
            var allTvChannelsCategoryIds = await _categoryService.GetTvChannelCategoryIdsAsync(allTvChannelsBySku.Select(p => p.Id).ToArray());

            //performance optimization, load all categories in one SQL request
            Dictionary<CategoryKey, Category> allCategories;
            try
            {
                var allCategoryList = await _categoryService.GetAllCategoriesAsync(showHidden: true);

                allCategories = await allCategoryList
                    .ToDictionaryAwaitAsync(async c => await CategoryKey.CreateCategoryKeyAsync(c, _categoryService, allCategoryList, _storeMappingService), c => new ValueTask<Category>(c));
            }
            catch (ArgumentException)
            {
                //categories with the same name are not supported in the same category level
                throw new ArgumentException(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Import.CategoriesWithSameNameNotSupported"));
            }

            //performance optimization, load all manufacturers IDs for tvChannels in one SQL request
            var allTvChannelsManufacturerIds = await _manufacturerService.GetTvChannelManufacturerIdsAsync(allTvChannelsBySku.Select(p => p.Id).ToArray());

            //performance optimization, load all manufacturers in one SQL request
            var allManufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true);

            //performance optimization, load all stores in one SQL request
            var allStores = await _storeService.GetAllStoresAsync();

            //tvChannel to import images
            var tvChannelPictureMetadata = new List<TvChannelPictureMetadata>();

            TvChannel lastLoadedTvChannel = null;
            var typeOfExportedAttribute = ExportedAttributeType.NotSpecified;

            for (var iRow = 2; iRow < metadata.EndRow; iRow++)
            {
                if (defaultWorksheet.Row(iRow).OutlineLevel != 0)
                {
                    if (lastLoadedTvChannel == null)
                        continue;

                    var newTypeOfExportedAttribute = GetTypeOfExportedAttribute(defaultWorksheet, metadata.LocalizedWorksheets, metadata.TvChannelAttributeManager, metadata.SpecificationAttributeManager, iRow);

                    //skip caption row
                    if (newTypeOfExportedAttribute != ExportedAttributeType.NotSpecified &&
                        newTypeOfExportedAttribute != typeOfExportedAttribute)
                    {
                        typeOfExportedAttribute = newTypeOfExportedAttribute;
                        continue;
                    }

                    switch (typeOfExportedAttribute)
                    {
                        case ExportedAttributeType.TvChannelAttribute:
                            await ImportTvChannelAttributeAsync(metadata, lastLoadedTvChannel, languages, iRow);
                            break;
                        case ExportedAttributeType.SpecificationAttribute:
                            await ImportSpecificationAttributeAsync(metadata, lastLoadedTvChannel, languages, iRow);
                            break;
                        case ExportedAttributeType.NotSpecified:
                        default:
                            continue;
                    }

                    continue;
                }

                metadata.Manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var tvChannel = metadata.SkuCellNum > 0 ? allTvChannelsBySku.FirstOrDefault(p => p.Sku == metadata.Manager.GetDefaultProperty("SKU").StringValue) : null;

                var isNew = tvChannel == null;

                tvChannel ??= new TvChannel();

                //some of previous values
                var previousStockQuantity = tvChannel.StockQuantity;
                var previousWarehouseId = tvChannel.WarehouseId;
                var prevTotalStockQuantity = await _tvChannelService.GetTotalStockQuantityAsync(tvChannel);

                if (isNew)
                    tvChannel.CreatedOnUtc = DateTime.UtcNow;

                foreach (var property in metadata.Manager.GetDefaultProperties)
                {
                    switch (property.PropertyName)
                    {
                        case "TvChannelType":
                            tvChannel.TvChannelTypeId = property.IntValue;
                            break;
                        case "ParentGroupedTvChannelId":
                            tvChannel.ParentGroupedTvChannelId = property.IntValue;
                            break;
                        case "VisibleIndividually":
                            tvChannel.VisibleIndividually = property.BooleanValue;
                            break;
                        case "Name":
                            tvChannel.Name = property.StringValue;
                            break;
                        case "ShortDescription":
                            tvChannel.ShortDescription = property.StringValue;
                            break;
                        case "FullDescription":
                            tvChannel.FullDescription = property.StringValue;
                            break;
                        case "Vendor":
                            //vendor can't change this field
                            if (currentVendor == null)
                                tvChannel.VendorId = property.IntValue;
                            break;
                        case "TvChannelTemplate":
                            tvChannel.TvChannelTemplateId = property.IntValue;
                            break;
                        case "ShowOnHomepage":
                            //vendor can't change this field
                            if (currentVendor == null)
                                tvChannel.ShowOnHomepage = property.BooleanValue;
                            break;
                        case "DisplayOrder":
                            //vendor can't change this field
                            if (currentVendor == null)
                                tvChannel.DisplayOrder = property.IntValue;
                            break;
                        case "MetaKeywords":
                            tvChannel.MetaKeywords = property.StringValue;
                            break;
                        case "MetaDescription":
                            tvChannel.MetaDescription = property.StringValue;
                            break;
                        case "MetaTitle":
                            tvChannel.MetaTitle = property.StringValue;
                            break;
                        case "AllowUserReviews":
                            tvChannel.AllowUserReviews = property.BooleanValue;
                            break;
                        case "Published":
                            tvChannel.Published = property.BooleanValue;
                            break;
                        case "SKU":
                            tvChannel.Sku = property.StringValue;
                            break;
                        case "ManufacturerPartNumber":
                            tvChannel.ManufacturerPartNumber = property.StringValue;
                            break;
                        case "Gtin":
                            tvChannel.Gtin = property.StringValue;
                            break;
                        case "IsGiftCard":
                            tvChannel.IsGiftCard = property.BooleanValue;
                            break;
                        case "GiftCardType":
                            tvChannel.GiftCardTypeId = property.IntValue;
                            break;
                        case "OverriddenGiftCardAmount":
                            tvChannel.OverriddenGiftCardAmount = property.DecimalValue;
                            break;
                        case "RequireOtherTvChannels":
                            tvChannel.RequireOtherTvChannels = property.BooleanValue;
                            break;
                        case "RequiredTvChannelIds":
                            tvChannel.RequiredTvChannelIds = property.StringValue;
                            break;
                        case "AutomaticallyAddRequiredTvChannels":
                            tvChannel.AutomaticallyAddRequiredTvChannels = property.BooleanValue;
                            break;
                        case "IsDownload":
                            tvChannel.IsDownload = property.BooleanValue;
                            break;
                        case "DownloadId":
                            tvChannel.DownloadId = property.IntValue;
                            break;
                        case "UnlimitedDownloads":
                            tvChannel.UnlimitedDownloads = property.BooleanValue;
                            break;
                        case "MaxNumberOfDownloads":
                            tvChannel.MaxNumberOfDownloads = property.IntValue;
                            break;
                        case "DownloadActivationType":
                            tvChannel.DownloadActivationTypeId = property.IntValue;
                            break;
                        case "HasSampleDownload":
                            tvChannel.HasSampleDownload = property.BooleanValue;
                            break;
                        case "SampleDownloadId":
                            tvChannel.SampleDownloadId = property.IntValue;
                            break;
                        case "HasUserAgreement":
                            tvChannel.HasUserAgreement = property.BooleanValue;
                            break;
                        case "UserAgreementText":
                            tvChannel.UserAgreementText = property.StringValue;
                            break;
                        case "IsRecurring":
                            tvChannel.IsRecurring = property.BooleanValue;
                            break;
                        case "RecurringCycleLength":
                            tvChannel.RecurringCycleLength = property.IntValue;
                            break;
                        case "RecurringCyclePeriod":
                            tvChannel.RecurringCyclePeriodId = property.IntValue;
                            break;
                        case "RecurringTotalCycles":
                            tvChannel.RecurringTotalCycles = property.IntValue;
                            break;
                        case "IsRental":
                            tvChannel.IsRental = property.BooleanValue;
                            break;
                        case "RentalPriceLength":
                            tvChannel.RentalPriceLength = property.IntValue;
                            break;
                        case "RentalPricePeriod":
                            tvChannel.RentalPricePeriodId = property.IntValue;
                            break;
                        case "IsShipEnabled":
                            tvChannel.IsShipEnabled = property.BooleanValue;
                            break;
                        case "IsFreeShipping":
                            tvChannel.IsFreeShipping = property.BooleanValue;
                            break;
                        case "ShipSeparately":
                            tvChannel.ShipSeparately = property.BooleanValue;
                            break;
                        case "AdditionalShippingCharge":
                            tvChannel.AdditionalShippingCharge = property.DecimalValue;
                            break;
                        case "DeliveryDate":
                            tvChannel.DeliveryDateId = property.IntValue;
                            break;
                        case "IsTaxExempt":
                            tvChannel.IsTaxExempt = property.BooleanValue;
                            break;
                        case "TaxCategory":
                            tvChannel.TaxCategoryId = property.IntValue;
                            break;
                        case "IsTelecommunicationsOrBroadcastingOrElectronicServices":
                            tvChannel.IsTelecommunicationsOrBroadcastingOrElectronicServices = property.BooleanValue;
                            break;
                        case "ManageInventoryMethod":
                            tvChannel.ManageInventoryMethodId = property.IntValue;
                            break;
                        case "TvChannelAvailabilityRange":
                            tvChannel.TvChannelAvailabilityRangeId = property.IntValue;
                            break;
                        case "UseMultipleWarehouses":
                            tvChannel.UseMultipleWarehouses = property.BooleanValue;
                            break;
                        case "WarehouseId":
                            tvChannel.WarehouseId = property.IntValue;
                            break;
                        case "StockQuantity":
                            tvChannel.StockQuantity = property.IntValue;
                            break;
                        case "DisplayStockAvailability":
                            tvChannel.DisplayStockAvailability = property.BooleanValue;
                            break;
                        case "DisplayStockQuantity":
                            tvChannel.DisplayStockQuantity = property.BooleanValue;
                            break;
                        case "MinStockQuantity":
                            tvChannel.MinStockQuantity = property.IntValue;
                            break;
                        case "LowStockActivity":
                            tvChannel.LowStockActivityId = property.IntValue;
                            break;
                        case "NotifyAdminForQuantityBelow":
                            tvChannel.NotifyAdminForQuantityBelow = property.IntValue;
                            break;
                        case "BackorderMode":
                            tvChannel.BackorderModeId = property.IntValue;
                            break;
                        case "AllowBackInStockSubscriptions":
                            tvChannel.AllowBackInStockSubscriptions = property.BooleanValue;
                            break;
                        case "OrderMinimumQuantity":
                            tvChannel.OrderMinimumQuantity = property.IntValue;
                            break;
                        case "OrderMaximumQuantity":
                            tvChannel.OrderMaximumQuantity = property.IntValue;
                            break;
                        case "AllowedQuantities":
                            tvChannel.AllowedQuantities = property.StringValue;
                            break;
                        case "AllowAddingOnlyExistingAttributeCombinations":
                            tvChannel.AllowAddingOnlyExistingAttributeCombinations = property.BooleanValue;
                            break;
                        case "NotReturnable":
                            tvChannel.NotReturnable = property.BooleanValue;
                            break;
                        case "DisableBuyButton":
                            tvChannel.DisableBuyButton = property.BooleanValue;
                            break;
                        case "DisableWishlistButton":
                            tvChannel.DisableWishlistButton = property.BooleanValue;
                            break;
                        case "AvailableForPreOrder":
                            tvChannel.AvailableForPreOrder = property.BooleanValue;
                            break;
                        case "PreOrderAvailabilityStartDateTimeUtc":
                            tvChannel.PreOrderAvailabilityStartDateTimeUtc = property.DateTimeNullable;
                            break;
                        case "CallForPrice":
                            tvChannel.CallForPrice = property.BooleanValue;
                            break;
                        case "Price":
                            tvChannel.Price = property.DecimalValue;
                            break;
                        case "OldPrice":
                            tvChannel.OldPrice = property.DecimalValue;
                            break;
                        case "TvChannelCost":
                            tvChannel.TvChannelCost = property.DecimalValue;
                            break;
                        case "UserEntersPrice":
                            tvChannel.UserEntersPrice = property.BooleanValue;
                            break;
                        case "MinimumUserEnteredPrice":
                            tvChannel.MinimumUserEnteredPrice = property.DecimalValue;
                            break;
                        case "MaximumUserEnteredPrice":
                            tvChannel.MaximumUserEnteredPrice = property.DecimalValue;
                            break;
                        case "BasepriceEnabled":
                            tvChannel.BasepriceEnabled = property.BooleanValue;
                            break;
                        case "BasepriceAmount":
                            tvChannel.BasepriceAmount = property.DecimalValue;
                            break;
                        case "BasepriceUnit":
                            tvChannel.BasepriceUnitId = property.IntValue;
                            break;
                        case "BasepriceBaseAmount":
                            tvChannel.BasepriceBaseAmount = property.DecimalValue;
                            break;
                        case "BasepriceBaseUnit":
                            tvChannel.BasepriceBaseUnitId = property.IntValue;
                            break;
                        case "MarkAsNew":
                            tvChannel.MarkAsNew = property.BooleanValue;
                            break;
                        case "MarkAsNewStartDateTimeUtc":
                            tvChannel.MarkAsNewStartDateTimeUtc = property.DateTimeNullable;
                            break;
                        case "MarkAsNewEndDateTimeUtc":
                            tvChannel.MarkAsNewEndDateTimeUtc = property.DateTimeNullable;
                            break;
                        case "Weight":
                            tvChannel.Weight = property.DecimalValue;
                            break;
                        case "Length":
                            tvChannel.Length = property.DecimalValue;
                            break;
                        case "Width":
                            tvChannel.Width = property.DecimalValue;
                            break;
                        case "Height":
                            tvChannel.Height = property.DecimalValue;
                            break;
                        case "IsLimitedToStores":
                            tvChannel.LimitedToStores = property.BooleanValue;
                            break;
                    }
                }

                //set some default values if not specified
                if (isNew && metadata.Properties.All(p => p.PropertyName != "TvChannelType"))
                    tvChannel.TvChannelType = TvChannelType.SimpleTvChannel;
                if (isNew && metadata.Properties.All(p => p.PropertyName != "VisibleIndividually"))
                    tvChannel.VisibleIndividually = true;
                if (isNew && metadata.Properties.All(p => p.PropertyName != "Published"))
                    tvChannel.Published = true;

                //sets the current vendor for the new tvChannel
                if (isNew && currentVendor != null)
                    tvChannel.VendorId = currentVendor.Id;

                tvChannel.UpdatedOnUtc = DateTime.UtcNow;

                if (isNew)
                    await _tvChannelService.InsertTvChannelAsync(tvChannel);
                else
                    await _tvChannelService.UpdateTvChannelAsync(tvChannel);

                //quantity change history
                if (isNew || previousWarehouseId == tvChannel.WarehouseId)
                {
                    await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannel, tvChannel.StockQuantity - previousStockQuantity, tvChannel.StockQuantity,
                        tvChannel.WarehouseId, await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.ImportTvChannel.Edit"));
                }
                //warehouse is changed 
                else
                {
                    //compose a message
                    var oldWarehouseMessage = string.Empty;
                    if (previousWarehouseId > 0)
                    {
                        var oldWarehouse = await _shippingService.GetWarehouseByIdAsync(previousWarehouseId);
                        if (oldWarehouse != null)
                            oldWarehouseMessage = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.Old"), oldWarehouse.Name);
                    }

                    var newWarehouseMessage = string.Empty;
                    if (tvChannel.WarehouseId > 0)
                    {
                        var newWarehouse = await _shippingService.GetWarehouseByIdAsync(tvChannel.WarehouseId);
                        if (newWarehouse != null)
                            newWarehouseMessage = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.New"), newWarehouse.Name);
                    }

                    var message = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.ImportTvChannel.EditWarehouse"), oldWarehouseMessage, newWarehouseMessage);

                    //record history
                    await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannel, -previousStockQuantity, 0, previousWarehouseId, message);
                    await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannel, tvChannel.StockQuantity, tvChannel.StockQuantity, tvChannel.WarehouseId, message);
                }

                if (!isNew &&
                    tvChannel.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                    tvChannel.BackorderMode == BackorderMode.NoBackorders &&
                    tvChannel.AllowBackInStockSubscriptions &&
                    await _tvChannelService.GetTotalStockQuantityAsync(tvChannel) > 0 &&
                    prevTotalStockQuantity <= 0 &&
                    tvChannel.Published &&
                    !tvChannel.Deleted)
                {
                    await _backInStockSubscriptionService.SendNotificationsToSubscribersAsync(tvChannel);
                }

                var tempProperty = metadata.Manager.GetDefaultProperty("SeName");

                //search engine name
                var seName = tempProperty?.StringValue ?? (isNew ? string.Empty : await _urlRecordService.GetSeNameAsync(tvChannel, 0));
                await _urlRecordService.SaveSlugAsync(tvChannel, await _urlRecordService.ValidateSeNameAsync(tvChannel, seName, tvChannel.Name, true), 0);

                //save tvChannel localized data
                await ImportTvChannelLocalizedAsync(tvChannel, metadata, iRow, languages);

                tempProperty = metadata.Manager.GetDefaultProperty("Categories");

                if (tempProperty != null)
                {
                    var categoryList = tempProperty.StringValue;

                    //category mappings
                    var categories = isNew || !allTvChannelsCategoryIds.ContainsKey(tvChannel.Id) ? Array.Empty<int>() : allTvChannelsCategoryIds[tvChannel.Id];

                    var storesIds = tvChannel.LimitedToStores
                        ? (await _storeMappingService.GetStoresIdsWithAccessAsync(tvChannel)).ToList()
                        : new List<int>();

                    var importedCategories = await categoryList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(categoryName => new CategoryKey(categoryName, storesIds))
                        .SelectAwait(async categoryKey =>
                        {
                            var rez = (allCategories.ContainsKey(categoryKey) ? allCategories[categoryKey].Id : allCategories.Values.FirstOrDefault(c => c.Name == categoryKey.Key)?.Id) ??
                                      allCategories.FirstOrDefault(p =>
                                    p.Key.Key.Equals(categoryKey.Key, StringComparison.InvariantCultureIgnoreCase))
                                .Value?.Id;

                            if (!rez.HasValue && int.TryParse(categoryKey.Key, out var id))
                                rez = id;

                            if (!rez.HasValue)
                                //database doesn't contain the imported category
                                throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Import.DatabaseNotContainCategory"), categoryKey.Key));

                            return rez.Value;
                        }).ToListAsync();

                    foreach (var categoryId in importedCategories)
                    {
                        if (categories.Any(c => c == categoryId))
                            continue;

                        var tvChannelCategory = new TvChannelCategory
                        {
                            TvChannelId = tvChannel.Id,
                            CategoryId = categoryId,
                            IsFeaturedTvChannel = false,
                            DisplayOrder = 1
                        };
                        await _categoryService.InsertTvChannelCategoryAsync(tvChannelCategory);
                    }

                    //delete tvChannel categories
                    var deletedTvChannelCategories = await categories.Where(categoryId => !importedCategories.Contains(categoryId))
                        .SelectAwait(async categoryId => (await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(tvChannel.Id, true)).FirstOrDefault(pc => pc.CategoryId == categoryId)).Where(pc => pc != null).ToListAsync();

                    foreach (var deletedTvChannelCategory in deletedTvChannelCategories)
                        await _categoryService.DeleteTvChannelCategoryAsync(deletedTvChannelCategory);
                }

                tempProperty = metadata.Manager.GetDefaultProperty("Manufacturers");
                if (tempProperty != null)
                {
                    var manufacturerList = tempProperty.StringValue;

                    //manufacturer mappings
                    var manufacturers = isNew || !allTvChannelsManufacturerIds.ContainsKey(tvChannel.Id) ? Array.Empty<int>() : allTvChannelsManufacturerIds[tvChannel.Id];
                    var importedManufacturers = manufacturerList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => allManufacturers.FirstOrDefault(m => m.Name == x.Trim())?.Id ?? int.Parse(x.Trim())).ToList();
                    foreach (var manufacturerId in importedManufacturers)
                    {
                        if (manufacturers.Any(c => c == manufacturerId))
                            continue;

                        var tvChannelManufacturer = new TvChannelManufacturer
                        {
                            TvChannelId = tvChannel.Id,
                            ManufacturerId = manufacturerId,
                            IsFeaturedTvChannel = false,
                            DisplayOrder = 1
                        };
                        await _manufacturerService.InsertTvChannelManufacturerAsync(tvChannelManufacturer);
                    }

                    //delete tvChannel manufacturers
                    var deletedTvChannelsManufacturers = await manufacturers.Where(manufacturerId => !importedManufacturers.Contains(manufacturerId))
                        .SelectAwait(async manufacturerId => (await _manufacturerService.GetTvChannelManufacturersByTvChannelIdAsync(tvChannel.Id)).First(pc => pc.ManufacturerId == manufacturerId)).ToListAsync();
                    foreach (var deletedTvChannelManufacturer in deletedTvChannelsManufacturers)
                        await _manufacturerService.DeleteTvChannelManufacturerAsync(deletedTvChannelManufacturer);
                }

                tempProperty = metadata.Manager.GetDefaultProperty("TvChannelTags");
                if (tempProperty != null)
                {
                    var tvChannelTags = tempProperty.StringValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();

                    //searching existing tvChannel tags by their id
                    var tvChannelTagIds = tvChannelTags.Where(pt => int.TryParse(pt, out var _)).Select(int.Parse);

                    var tvChannelTagsByIds = (await _tvChannelTagService.GetAllTvChannelTagsByTvChannelIdAsync(tvChannel.Id)).Where(pt => tvChannelTagIds.Contains(pt.Id)).ToList();

                    tvChannelTags.AddRange(tvChannelTagsByIds.Select(pt => pt.Name));
                    var filter = tvChannelTagsByIds.Select(pt => pt.Id.ToString()).ToList();

                    //tvChannel tag mappings
                    await _tvChannelTagService.UpdateTvChannelTagsAsync(tvChannel, tvChannelTags.Where(pt => !filter.Contains(pt)).ToArray());
                }

                tempProperty = metadata.Manager.GetDefaultProperty("LimitedToStores");
                if (tempProperty != null)
                {
                    var limitedToStoresList = tempProperty.StringValue;

                    var importedStores = tvChannel.LimitedToStores ? limitedToStoresList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => allStores.FirstOrDefault(store => store.Name == x.Trim())?.Id ?? int.Parse(x.Trim())).ToList() : new List<int>();

                    await _tvChannelService.UpdateTvChannelStoreMappingsAsync(tvChannel, importedStores);
                }

                var picture1 = await DownloadFileAsync(metadata.Manager.GetDefaultProperty("Picture1")?.StringValue, downloadedFiles);
                var picture2 = await DownloadFileAsync(metadata.Manager.GetDefaultProperty("Picture2")?.StringValue, downloadedFiles);
                var picture3 = await DownloadFileAsync(metadata.Manager.GetDefaultProperty("Picture3")?.StringValue, downloadedFiles);

                tvChannelPictureMetadata.Add(new TvChannelPictureMetadata
                {
                    TvChannelItem = tvChannel,
                    Picture1Path = picture1,
                    Picture2Path = picture2,
                    Picture3Path = picture3,
                    IsNew = isNew
                });

                lastLoadedTvChannel = tvChannel;

                //update "HasTierPrices" and "HasDiscountsApplied" properties
                //_tvChannelService.UpdateHasTierPricesProperty(tvChannel);
                //_tvChannelService.UpdateHasDiscountsApplied(tvChannel);
            }

            if (_mediaSettings.ImportTvChannelImagesUsingHash && await _pictureService.IsStoreInDbAsync())
                await ImportTvChannelImagesUsingHashAsync(tvChannelPictureMetadata, allTvChannelsBySku);
            else
                await ImportTvChannelImagesUsingServicesAsync(tvChannelPictureMetadata);

            foreach (var downloadedFile in downloadedFiles)
            {
                if (!_fileProvider.FileExists(downloadedFile))
                    continue;

                try
                {
                    _fileProvider.DeleteFile(downloadedFile);
                }
                catch
                {
                    // ignored
                }
            }

            //activity log
            await _userActivityService.InsertActivityAsync("ImportTvChannels", string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportTvChannels"), metadata.CountTvChannelsInFile));
        }

        /// <summary>
        /// Import newsletter subscribers from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of imported subscribers
        /// </returns>
        public virtual async Task<int> ImportNewsletterSubscribersFromTxtAsync(Stream stream)
        {
            var count = 0;
            using (var reader = new StreamReader(stream))
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var tmp = line.Split(',');

                    if (tmp.Length > 3)
                        throw new TvProgException("Wrong file format");

                    var isActive = true;

                    var store = await _storeContext.GetCurrentStoreAsync();
                    var storeId = store.Id;

                    //"email" field specified
                    var email = tmp[0].Trim();

                    if (!CommonHelper.IsValidEmail(email))
                        continue;

                    //"active" field specified
                    if (tmp.Length >= 2)
                        isActive = bool.Parse(tmp[1].Trim());

                    //"storeId" field specified
                    if (tmp.Length == 3)
                        storeId = int.Parse(tmp[2].Trim());

                    //import
                    var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(email, storeId);
                    if (subscription != null)
                    {
                        subscription.Email = email;
                        subscription.Active = isActive;
                        await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
                    }
                    else
                    {
                        subscription = new NewsLetterSubscription
                        {
                            Active = isActive,
                            CreatedOnUtc = DateTime.UtcNow,
                            Email = email,
                            StoreId = storeId,
                            NewsLetterSubscriptionGuid = Guid.NewGuid()
                        };
                        await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(subscription);
                    }

                    count++;
                }

            await _userActivityService.InsertActivityAsync("ImportNewsLetterSubscriptions",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportNewsLetterSubscriptions"), count));

            return count;
        }

        /// <summary>
        /// Import states from TXT file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="writeLog">Indicates whether to add logging</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of imported states
        /// </returns>
        public virtual async Task<int> ImportStatesFromTxtAsync(Stream stream, bool writeLog = true)
        {
            var count = 0;
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    var tmp = line.Split(',');

                    if (tmp.Length != 5)
                        throw new TvProgException("Wrong file format");

                    //parse
                    var countryTwoLetterIsoCode = tmp[0].Trim();
                    var name = tmp[1].Trim();
                    var abbreviation = tmp[2].Trim();
                    var published = bool.Parse(tmp[3].Trim());
                    var displayOrder = int.Parse(tmp[4].Trim());

                    var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(countryTwoLetterIsoCode);
                    if (country == null)
                    {
                        //country cannot be loaded. skip
                        continue;
                    }

                    //import
                    var states = await _stateProvinceService.GetStateProvincesByCountryIdAsync(country.Id, showHidden: true);
                    var state = states.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

                    if (state != null)
                    {
                        state.Abbreviation = abbreviation;
                        state.Published = published;
                        state.DisplayOrder = displayOrder;
                        await _stateProvinceService.UpdateStateProvinceAsync(state);
                    }
                    else
                    {
                        state = new StateProvince
                        {
                            CountryId = country.Id,
                            Name = name,
                            Abbreviation = abbreviation,
                            Published = published,
                            DisplayOrder = displayOrder
                        };
                        await _stateProvinceService.InsertStateProvinceAsync(state);
                    }

                    count++;
                }
            }

            //activity log
            if (writeLog)
            {
                await _userActivityService.InsertActivityAsync("ImportStates",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportStates"), count));
            }

            return count;
        }

        /// <summary>
        /// Import manufacturers from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task ImportManufacturersFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<Manufacturer>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<Manufacturer, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;
            var setSeName = defaultProperties.Any(p => p.PropertyName == "SeName");

            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                    .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                    .All(cell => cell?.Value == null || string.IsNullOrEmpty(cell.Value.ToString()));

                if (allColumnsAreEmpty)
                    break;

                manager.ReadDefaultFromXlsx(defaultWorksheet, iRow);

                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manager.GetDefaultProperty("Id").IntValue);

                var isNew = manufacturer == null;

                manufacturer ??= new Manufacturer();

                if (isNew)
                {
                    manufacturer.CreatedOnUtc = DateTime.UtcNow;

                    //default values
                    manufacturer.PageSize = _catalogSettings.DefaultManufacturerPageSize;
                    manufacturer.PageSizeOptions = _catalogSettings.DefaultManufacturerPageSizeOptions;
                    manufacturer.Published = true;
                    manufacturer.AllowUsersToSelectPageSize = true;
                }

                var seName = string.Empty;

                foreach (var property in manager.GetDefaultProperties)
                {
                    switch (property.PropertyName)
                    {
                        case "Name":
                            manufacturer.Name = property.StringValue;
                            break;
                        case "Description":
                            manufacturer.Description = property.StringValue;
                            break;
                        case "ManufacturerTemplateId":
                            manufacturer.ManufacturerTemplateId = property.IntValue;
                            break;
                        case "MetaKeywords":
                            manufacturer.MetaKeywords = property.StringValue;
                            break;
                        case "MetaDescription":
                            manufacturer.MetaDescription = property.StringValue;
                            break;
                        case "MetaTitle":
                            manufacturer.MetaTitle = property.StringValue;
                            break;
                        case "Picture":
                            var picture = await LoadPictureAsync(manager.GetDefaultProperty("Picture").StringValue, manufacturer.Name, isNew ? null : (int?)manufacturer.PictureId);

                            if (picture != null)
                                manufacturer.PictureId = picture.Id;

                            break;
                        case "PageSize":
                            manufacturer.PageSize = property.IntValue;
                            break;
                        case "AllowUsersToSelectPageSize":
                            manufacturer.AllowUsersToSelectPageSize = property.BooleanValue;
                            break;
                        case "PageSizeOptions":
                            manufacturer.PageSizeOptions = property.StringValue;
                            break;
                        case "PriceRangeFiltering":
                            manufacturer.PriceRangeFiltering = property.BooleanValue;
                            break;
                        case "PriceFrom":
                            manufacturer.PriceFrom = property.DecimalValue;
                            break;
                        case "PriceTo":
                            manufacturer.PriceTo = property.DecimalValue;
                            break;
                        case "AutomaticallyCalculatePriceRange":
                            manufacturer.ManuallyPriceRange = property.BooleanValue;
                            break;
                        case "Published":
                            manufacturer.Published = property.BooleanValue;
                            break;
                        case "DisplayOrder":
                            manufacturer.DisplayOrder = property.IntValue;
                            break;
                        case "SeName":
                            seName = property.StringValue;
                            break;
                    }
                }

                manufacturer.UpdatedOnUtc = DateTime.UtcNow;

                if (isNew)
                    await _manufacturerService.InsertManufacturerAsync(manufacturer);
                else
                    await _manufacturerService.UpdateManufacturerAsync(manufacturer);

                //search engine name
                if (setSeName)
                    await _urlRecordService.SaveSlugAsync(manufacturer, await _urlRecordService.ValidateSeNameAsync(manufacturer, seName, manufacturer.Name, true), 0);

                //save manufacturer localized data
                await ImportManufaturerLocalizedAsync(manufacturer, metadata, manager, iRow, languages);

                iRow++;
            }

            //activity log
            await _userActivityService.InsertActivityAsync("ImportManufacturers",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportManufacturers"), iRow - 2));
        }

        /// <summary>
        /// Import categories from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task ImportCategoriesFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);

            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);

            //the columns
            var metadata = GetWorkbookMetadata<Category>(workbook, languages);
            var defaultWorksheet = metadata.DefaultWorksheet;
            var defaultProperties = metadata.DefaultProperties;
            var localizedProperties = metadata.LocalizedProperties;

            var manager = new PropertyManager<Category, Language>(defaultProperties, _catalogSettings, localizedProperties, languages);

            var iRow = 2;
            var setSeName = defaultProperties.Any(p => p.PropertyName == "SeName");

            //performance optimization, load all categories in one SQL request
            var allCategories = await (await _categoryService
                .GetAllCategoriesAsync(showHidden: true))
                .GroupByAwait(async c => await _categoryService.GetFormattedBreadCrumbAsync(c))
                .ToDictionaryAsync(c => c.Key, c => c.FirstAsync());

            var saveNextTime = new List<int>();

            while (true)
            {
                var allColumnsAreEmpty = manager.GetDefaultProperties
                    .Select(property => defaultWorksheet.Row(iRow).Cell(property.PropertyOrderPosition))
                    .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString()));

                if (allColumnsAreEmpty)
                    break;

                //get category by data in xlsx file if it possible, or create new category
                var (category, isNew, currentCategoryBreadCrumb) = await GetCategoryFromXlsxAsync(manager, defaultWorksheet, iRow, allCategories);

                //update category by data in xlsx file
                var (seName, isParentCategoryExists) = await UpdateCategoryByXlsxAsync(category, manager, allCategories, isNew);

                if (isParentCategoryExists)
                {
                    //if parent category exists in database then save category into database
                    await SaveCategoryAsync(isNew, category, allCategories, currentCategoryBreadCrumb, setSeName, seName);

                    //save category localized data
                    await ImportCategoryLocalizedAsync(category, metadata, manager, iRow, languages);
                }
                else
                {
                    //if parent category doesn't exists in database then try save category into database next time
                    saveNextTime.Add(iRow);
                }

                iRow++;
            }

            var needSave = saveNextTime.Any();

            while (needSave)
            {
                var remove = new List<int>();

                //try to save unsaved categories
                foreach (var rowId in saveNextTime)
                {
                    //get category by data in xlsx file if it possible, or create new category
                    var (category, isNew, currentCategoryBreadCrumb) = await GetCategoryFromXlsxAsync(manager, defaultWorksheet, rowId, allCategories);
                    //update category by data in xlsx file
                    var (seName, isParentCategoryExists) = await UpdateCategoryByXlsxAsync(category, manager, allCategories, isNew);

                    if (!isParentCategoryExists)
                        continue;

                    //if parent category exists in database then save category into database
                    await SaveCategoryAsync(isNew, category, allCategories, currentCategoryBreadCrumb, setSeName, seName);

                    //save category localized data
                    await ImportCategoryLocalizedAsync(category, metadata, manager, rowId, languages);

                    remove.Add(rowId);
                }

                saveNextTime.RemoveAll(item => remove.Contains(item));

                needSave = remove.Any() && saveNextTime.Any();
            }

            //activity log
            await _userActivityService.InsertActivityAsync("ImportCategories",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportCategories"), iRow - 2 - saveNextTime.Count));

            if (!saveNextTime.Any())
                return;

            var categoriesName = new List<string>();

            foreach (var rowId in saveNextTime)
            {
                manager.ReadDefaultFromXlsx(defaultWorksheet, rowId);
                categoriesName.Add(manager.GetDefaultProperty("Name").StringValue);
            }

            throw new ArgumentException(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Import.CategoriesArentImported"), string.Join(", ", categoriesName)));
        }

        /// <summary>
        /// Import orders from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task ImportOrdersFromXlsxAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            var downloadedFiles = new List<string>();

            (var metadata, var worksheet) = await PrepareImportOrderDataAsync(workbook);

            //performance optimization, load all orders by guid in one SQL request
            var allOrdersByGuids = await _orderService.GetOrdersByGuidsAsync(metadata.AllOrderGuids.ToArray());

            //performance optimization, load all users by guid in one SQL request
            var allUsersByGuids = await _userService.GetUsersByGuidsAsync(metadata.AllUserGuids.ToArray());

            Order lastLoadedOrder = null;

            for (var iRow = 2; iRow < metadata.EndRow; iRow++)
            {
                //imports tvChannel attributes
                if (worksheet.Row(iRow).OutlineLevel != 0)
                {
                    if (lastLoadedOrder == null)
                        continue;

                    metadata.OrderItemManager.ReadDefaultFromXlsx(worksheet, iRow, 2);

                    //skip caption row
                    if (!metadata.OrderItemManager.IsCaption)
                    {
                        await ImportOrderItemAsync(metadata.OrderItemManager, lastLoadedOrder);
                    }
                    continue;
                }

                metadata.Manager.ReadDefaultFromXlsx(worksheet, iRow);

                var order = allOrdersByGuids.FirstOrDefault(p => p.OrderGuid == metadata.Manager.GetDefaultProperty("OrderGuid").GuidValue);

                var isNew = order == null;

                order ??= new Order();

                Address orderBillingAddress = null;
                Address orderAddress = null;

                if (isNew)
                    order.CreatedOnUtc = DateTime.UtcNow;
                else
                {
                    orderBillingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
                    orderAddress = await _addressService.GetAddressByIdAsync((order.PickupInStore ? order.PickupAddressId : order.ShippingAddressId) ?? 0);
                }

                orderBillingAddress ??= new Address();
                orderAddress ??= new Address();

                var user = allUsersByGuids.FirstOrDefault(p => p.UserGuid.ToString() == metadata.Manager.GetDefaultProperty("UserGuid").StringValue);

                foreach (var property in metadata.Manager.GetDefaultProperties)
                {
                    switch (property.PropertyName)
                    {
                        case "StoreId":
                            if (await _storeService.GetStoreByIdAsync(property.IntValue) is Store orderStore)
                                order.StoreId = property.IntValue;
                            else
                                order.StoreId = (await _storeContext.GetCurrentStoreAsync())?.Id ?? 0;
                            break;
                        case "OrderGuid":
                            order.OrderGuid = property.GuidValue;
                            break;
                        case "UserId":
                            order.UserId = user?.Id ?? 0;
                            break;
                        case "OrderStatus":
                            order.OrderStatus = (OrderStatus)property.PropertyValue;
                            break;
                        case "PaymentStatus":
                            order.PaymentStatus = (PaymentStatus)property.PropertyValue;
                            break;
                        case "ShippingStatus":
                            order.ShippingStatus = (ShippingStatus)property.PropertyValue;
                            break;
                        case "OrderSubtotalInclTax":
                            order.OrderSubtotalInclTax = property.DecimalValue;
                            break;
                        case "OrderSubtotalExclTax":
                            order.OrderSubtotalExclTax = property.DecimalValue;
                            break;
                        case "OrderSubTotalDiscountInclTax":
                            order.OrderSubTotalDiscountInclTax = property.DecimalValue;
                            break;
                        case "OrderSubTotalDiscountExclTax":
                            order.OrderSubTotalDiscountExclTax = property.DecimalValue;
                            break;
                        case "OrderShippingInclTax":
                            order.OrderShippingInclTax = property.DecimalValue;
                            break;
                        case "OrderShippingExclTax":
                            order.OrderShippingExclTax = property.DecimalValue;
                            break;
                        case "PaymentMethodAdditionalFeeInclTax":
                            order.PaymentMethodAdditionalFeeInclTax = property.DecimalValue;
                            break;
                        case "PaymentMethodAdditionalFeeExclTax":
                            order.PaymentMethodAdditionalFeeExclTax = property.DecimalValue;
                            break;
                        case "TaxRates":
                            order.TaxRates = property.StringValue;
                            break;
                        case "OrderTax":
                            order.OrderTax = property.DecimalValue;
                            break;
                        case "OrderTotal":
                            order.OrderTotal = property.DecimalValue;
                            break;
                        case "RefundedAmount":
                            order.RefundedAmount = property.DecimalValue;
                            break;
                        case "OrderDiscount":
                            order.OrderDiscount = property.DecimalValue;
                            break;
                        case "CurrencyRate":
                            order.CurrencyRate = property.DecimalValue;
                            break;
                        case "UserCurrencyCode":
                            order.UserCurrencyCode = property.StringValue;
                            break;
                        case "AffiliateId":
                            order.AffiliateId = property.IntValue;
                            break;
                        case "PaymentMethodSystemName":
                            order.PaymentMethodSystemName = property.StringValue;
                            break;
                        case "ShippingPickupInStore":
                            order.PickupInStore = property.BooleanValue;
                            break;
                        case "ShippingMethod":
                            order.ShippingMethod = property.StringValue;
                            break;
                        case "ShippingRateComputationMethodSystemName":
                            order.ShippingRateComputationMethodSystemName = property.StringValue;
                            break;
                        case "CustomValuesXml":
                            order.CustomValuesXml = property.StringValue;
                            break;
                        case "VatNumber":
                            order.VatNumber = property.StringValue;
                            break;
                        case "CreatedOnUtc":
                            order.CreatedOnUtc = DateTime.TryParse(property.StringValue, out var createdOnUtc) ? createdOnUtc : DateTime.UtcNow;
                            break;
                        case "BillingFirstName":
                            orderBillingAddress.FirstName = property.StringValue;
                            break;
                        case "BillingLastName":
                            orderBillingAddress.LastName = property.StringValue;
                            break;
                        case "BillingMiddleName":
                            orderBillingAddress.MiddleName = property.StringValue;
                            break;
                        case "BillingPhoneNumber":
                            orderBillingAddress.PhoneNumber = property.StringValue;
                            break;
                        case "BillingEmail":
                            orderBillingAddress.Email = property.StringValue;
                            break;
                        case "BillingFaxNumber":
                            orderBillingAddress.FaxNumber = property.StringValue;
                            break;
                        case "BillingCompany":
                            orderBillingAddress.Company = property.StringValue;
                            break;
                        case "BillingAddress1":
                            orderBillingAddress.Address1 = property.StringValue;
                            break;
                        case "BillingAddress2":
                            orderBillingAddress.Address2 = property.StringValue;
                            break;
                        case "BillingCity":
                            orderBillingAddress.City = property.StringValue;
                            break;
                        case "BillingCounty":
                            orderBillingAddress.County = property.StringValue;
                            break;
                        case "BillingStateProvinceAbbreviation":
                            if (await _stateProvinceService.GetStateProvinceByAbbreviationAsync(property.StringValue) is StateProvince billingState)
                                orderBillingAddress.StateProvinceId = billingState.Id;
                            break;
                        case "BillingZipPostalCode":
                            orderBillingAddress.ZipPostalCode = property.StringValue;
                            break;
                        case "BillingCountryCode":
                            if (await _countryService.GetCountryByTwoLetterIsoCodeAsync(property.StringValue) is Country billingCountry)
                                orderBillingAddress.CountryId = billingCountry.Id;
                            break;
                        case "ShippingFirstName":
                            orderAddress.FirstName = property.StringValue;
                            break;
                        case "ShippingLastName":
                            orderAddress.LastName = property.StringValue;
                            break;
                        case "ShippingMiddleName":
                            orderAddress.MiddleName = property.StringValue;
                            break;
                        case "ShippingPhoneNumber":
                            orderAddress.PhoneNumber = property.StringValue;
                            break;
                        case "ShippingEmail":
                            orderAddress.Email = property.StringValue;
                            break;
                        case "ShippingFaxNumber":
                            orderAddress.FaxNumber = property.StringValue;
                            break;
                        case "ShippingCompany":
                            orderAddress.Company = property.StringValue;
                            break;
                        case "ShippingAddress1":
                            orderAddress.Address1 = property.StringValue;
                            break;
                        case "ShippingAddress2":
                            orderAddress.Address2 = property.StringValue;
                            break;
                        case "ShippingCity":
                            orderAddress.City = property.StringValue;
                            break;
                        case "ShippingCounty":
                            orderAddress.County = property.StringValue;
                            break;
                        case "ShippingStateProvinceAbbreviation":
                            if (await _stateProvinceService.GetStateProvinceByAbbreviationAsync(property.StringValue) is StateProvince shippingState)
                                orderAddress.StateProvinceId = shippingState.Id;
                            break;
                        case "ShippingZipPostalCode":
                            orderAddress.ZipPostalCode = property.StringValue;
                            break;
                        case "ShippingCountryCode":
                            if (await _countryService.GetCountryByTwoLetterIsoCodeAsync(property.StringValue) is Country shippingCountry)
                                orderAddress.CountryId = shippingCountry.Id;
                            break;
                    }
                }

                //check order address field values from excel
                if (string.IsNullOrWhiteSpace(orderAddress.FirstName) && string.IsNullOrWhiteSpace(orderAddress.LastName) && string.IsNullOrWhiteSpace(orderAddress.Email))
                    orderAddress = null;

                //insert or update billing address
                if (orderBillingAddress.Id == 0)
                {
                    await _addressService.InsertAddressAsync(orderBillingAddress);
                    order.BillingAddressId = orderBillingAddress.Id;
                }
                else
                    await _addressService.UpdateAddressAsync(orderBillingAddress);

                //insert or update shipping/pickup address
                if (orderAddress != null)
                {
                    if (orderAddress.Id == 0)
                    {
                        await _addressService.InsertAddressAsync(orderAddress);

                        if (order.PickupInStore)
                            order.PickupAddressId = orderAddress.Id;
                        else
                            order.ShippingAddressId = orderAddress.Id;
                    }
                    else
                        await _addressService.UpdateAddressAsync(orderAddress);
                }
                else
                    order.ShippingAddressId = null;

                //set some default values if not specified
                if (isNew)
                {
                    //user language
                    var userLanguage = await _languageService.GetLanguageByIdAsync(user?.LanguageId ?? 0);
                    if (userLanguage == null || !userLanguage.Published)
                        userLanguage = await _workContext.GetWorkingLanguageAsync();
                    order.UserLanguageId = userLanguage.Id;

                    //tax display type
                    order.UserTaxDisplayType = _taxSettings.TaxDisplayType;

                    //set other default values
                    order.AllowStoringCreditCardNumber = false;
                    order.AuthorizationTransactionCode = string.Empty;
                    order.AuthorizationTransactionId = string.Empty;
                    order.AuthorizationTransactionResult = string.Empty;
                    order.CaptureTransactionId = string.Empty;
                    order.CaptureTransactionResult = string.Empty;
                    order.CardCvv2 = string.Empty;
                    order.CardExpirationMonth = string.Empty;
                    order.CardExpirationYear = string.Empty;
                    order.CardName = string.Empty;
                    order.CardNumber = string.Empty;
                    order.CardType = string.Empty;
                    order.UserIp = string.Empty;
                    order.CustomOrderNumber = string.Empty;
                    order.MaskedCreditCardNumber = string.Empty;
                    order.RefundedAmount = decimal.Zero;
                    order.SubscriptionTransactionId = string.Empty;

                    await _orderService.InsertOrderAsync(order);

                    //generate and set custom order number
                    order.CustomOrderNumber = _customNumberFormatter.GenerateOrderCustomNumber(order);
                    await _orderService.UpdateOrderAsync(order);
                }
                else
                    await _orderService.UpdateOrderAsync(order);

                lastLoadedOrder = order;
            }

            //activity log
            await _userActivityService.InsertActivityAsync("ImportOrders", string.Format(await _localizationService.GetResourceAsync("ActivityLog.ImportOrders"), metadata.CountOrdersInFile));
        }

        #endregion

        #region Nested classes

        protected partial class TvChannelPictureMetadata
        {
            public TvChannel TvChannelItem { get; set; }

            public string Picture1Path { get; set; }

            public string Picture2Path { get; set; }

            public string Picture3Path { get; set; }

            public bool IsNew { get; set; }
        }

        public partial class CategoryKey
        {
            /// <returns>Задача представляет асинхронную операцию</returns>
            public static async Task<CategoryKey> CreateCategoryKeyAsync(Category category, ICategoryService categoryService, IList<Category> allCategories, IStoreMappingService storeMappingService)
            {
                return new CategoryKey(await categoryService.GetFormattedBreadCrumbAsync(category, allCategories), category.LimitedToStores ? (await storeMappingService.GetStoresIdsWithAccessAsync(category)).ToList() : new List<int>())
                {
                    Category = category
                };
            }

            public CategoryKey(string key, List<int> storesIds = null)
            {
                Key = key.Trim();
                StoresIds = storesIds ?? new List<int>();
            }

            public List<int> StoresIds { get; }

            public Category Category { get; private set; }

            public string Key { get; }

            public bool Equals(CategoryKey y)
            {
                if (y == null)
                    return false;

                if (Category != null && y.Category != null)
                    return Category.Id == y.Category.Id;

                if ((StoresIds.Any() || y.StoresIds.Any())
                    && (StoresIds.All(id => !y.StoresIds.Contains(id)) || y.StoresIds.All(id => !StoresIds.Contains(id))))
                    return false;

                return Key.Equals(y.Key);
            }

            public override int GetHashCode()
            {
                if (!StoresIds.Any())
                    return Key.GetHashCode();

                var storesIds = StoresIds.Select(id => id.ToString())
                    .Aggregate(string.Empty, (all, current) => all + current);

                return $"{storesIds}_{Key}".GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var other = obj as CategoryKey;
                return other?.Equals(other) ?? false;
            }
        }

        #endregion
    }
}