using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Http;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Discounts;
using TvProgViewer.Services.ExportImport;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Shipping;
using TvProgViewer.WebUI.Areas.Admin.Factories;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Controllers;
using TvProgViewer.Web.Framework.Mvc;
using TvProgViewer.Web.Framework.Mvc.Filters;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Validators;

namespace TvProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class TvChannelController : BaseAdminController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly ICategoryService _categoryService;
        private readonly ICopyTvChannelService _copyTvChannelService;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly IDiscountService _discountService;
        private readonly IDownloadService _downloadService;
        private readonly IExportManager _exportManager;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IImportManager _importManager;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ITvProgFileProvider _fileProvider;
        private readonly INotificationService _notificationService;
        private readonly IPdfService _pdfService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly ITvChannelAttributeFormatter _tvChannelAttributeFormatter;
        private readonly ITvChannelAttributeParser _tvChannelAttributeParser;
        private readonly ITvChannelAttributeService _tvChannelAttributeService;
        private readonly ITvChannelModelFactory _tvChannelModelFactory;
        private readonly ITvChannelService _tvChannelService;
        private readonly ITvChannelTagService _tvChannelTagService;
        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVideoService _videoService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public TvChannelController(IAclService aclService,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            ICategoryService categoryService,
            ICopyTvChannelService copyTvChannelService,
            IUserActivityService userActivityService,
            IUserService userService,
            IDiscountService discountService,
            IDownloadService downloadService,
            IExportManager exportManager,
            IGenericAttributeService genericAttributeService,
            IHttpClientFactory httpClientFactory,
            IImportManager importManager,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IManufacturerService manufacturerService,
            ITvProgFileProvider fileProvider,
            INotificationService notificationService,
            IPdfService pdfService,
            IPermissionService permissionService,
            IPictureService pictureService,
            ITvChannelAttributeFormatter tvChannelAttributeFormatter,
            ITvChannelAttributeParser tvChannelAttributeParser,
            ITvChannelAttributeService tvChannelAttributeService,
            ITvChannelModelFactory tvChannelModelFactory,
            ITvChannelService tvChannelService,
            ITvChannelTagService tvChannelTagService,
            ISettingService settingService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            ISpecificationAttributeService specificationAttributeService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IVideoService videoService,
            IWebHelper webHelper,
            IWorkContext workContext,
            VendorSettings vendorSettings)
        {
            _aclService = aclService;
            _backInStockSubscriptionService = backInStockSubscriptionService;
            _categoryService = categoryService;
            _copyTvChannelService = copyTvChannelService;
            _userActivityService = userActivityService;
            _userService = userService;
            _discountService = discountService;
            _downloadService = downloadService;
            _exportManager = exportManager;
            _genericAttributeService = genericAttributeService;
            _httpClientFactory = httpClientFactory;
            _importManager = importManager;
            _languageService = languageService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _manufacturerService = manufacturerService;
            _fileProvider = fileProvider;
            _notificationService = notificationService;
            _pdfService = pdfService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _tvChannelAttributeFormatter = tvChannelAttributeFormatter;
            _tvChannelAttributeParser = tvChannelAttributeParser;
            _tvChannelAttributeService = tvChannelAttributeService;
            _tvChannelModelFactory = tvChannelModelFactory;
            _tvChannelService = tvChannelService;
            _tvChannelTagService = tvChannelTagService;
            _settingService = settingService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _specificationAttributeService = specificationAttributeService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _videoService = videoService;
            _webHelper = webHelper;
            _workContext = workContext;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(TvChannel tvChannel, TvChannelModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(tvChannel,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(tvChannel,
                    x => x.ShortDescription,
                    localized.ShortDescription,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(tvChannel,
                    x => x.FullDescription,
                    localized.FullDescription,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(tvChannel,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(tvChannel,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(tvChannel,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(tvChannel, localized.SeName, localized.Name, false);
                await _urlRecordService.SaveSlugAsync(tvChannel, seName, localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(TvChannelTag tvChannelTag, TvChannelTagModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(tvChannelTag,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                var seName = await _urlRecordService.ValidateSeNameAsync(tvChannelTag, string.Empty, localized.Name, false);
                await _urlRecordService.SaveSlugAsync(tvChannelTag, seName, localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(TvChannelAttributeMapping pam, TvChannelAttributeMappingModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(pam,
                    x => x.TextPrompt,
                    localized.TextPrompt,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(pam,
                    x => x.DefaultValue,
                    localized.DefaultValue,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(TvChannelAttributeValue pav, TvChannelAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(pav,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdatePictureSeoNamesAsync(TvChannel tvChannel)
        {
            foreach (var pp in await _tvChannelService.GetTvChannelPicturesByTvChannelIdAsync(tvChannel.Id))
                await _pictureService.SetSeoFilenameAsync(pp.PictureId, await _pictureService.GetPictureSeNameAsync(tvChannel.Name));
        }

        protected virtual async Task SaveTvChannelAclAsync(TvChannel tvChannel, TvChannelModel model)
        {
            tvChannel.SubjectToAcl = model.SelectedUserRoleIds.Any();
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            var existingAclRecords = await _aclService.GetAclRecordsAsync(tvChannel);
            var allUserRoles = await _userService.GetAllUserRolesAsync(true);
            foreach (var userRole in allUserRoles)
            {
                if (model.SelectedUserRoleIds.Contains(userRole.Id))
                {
                    //new role
                    if (!existingAclRecords.Any(acl => acl.UserRoleId == userRole.Id))
                        await _aclService.InsertAclRecordAsync(tvChannel, userRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.UserRoleId == userRole.Id);
                    if (aclRecordToDelete != null)
                        await _aclService.DeleteAclRecordAsync(aclRecordToDelete);
                }
            }
        }

        protected virtual async Task SaveCategoryMappingsAsync(TvChannel tvChannel, TvChannelModel model)
        {
            var existingTvChannelCategories = await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(tvChannel.Id, true);

            //delete categories
            foreach (var existingTvChannelCategory in existingTvChannelCategories)
                if (!model.SelectedCategoryIds.Contains(existingTvChannelCategory.CategoryId))
                    await _categoryService.DeleteTvChannelCategoryAsync(existingTvChannelCategory);

            //add categories
            foreach (var categoryId in model.SelectedCategoryIds)
            {
                if (_categoryService.FindTvChannelCategory(existingTvChannelCategories, tvChannel.Id, categoryId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingCategoryMapping = await _categoryService.GetTvChannelCategoriesByCategoryIdAsync(categoryId, showHidden: true);
                    if (existingCategoryMapping.Any())
                        displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;
                    await _categoryService.InsertTvChannelCategoryAsync(new TvChannelCategory
                    {
                        TvChannelId = tvChannel.Id,
                        CategoryId = categoryId,
                        DisplayOrder = displayOrder
                    });
                }
            }
        }

        protected virtual async Task SaveManufacturerMappingsAsync(TvChannel tvChannel, TvChannelModel model)
        {
            var existingTvChannelManufacturers = await _manufacturerService.GetTvChannelManufacturersByTvChannelIdAsync(tvChannel.Id, true);

            //delete manufacturers
            foreach (var existingTvChannelManufacturer in existingTvChannelManufacturers)
                if (!model.SelectedManufacturerIds.Contains(existingTvChannelManufacturer.ManufacturerId))
                    await _manufacturerService.DeleteTvChannelManufacturerAsync(existingTvChannelManufacturer);

            //add manufacturers
            foreach (var manufacturerId in model.SelectedManufacturerIds)
            {
                if (_manufacturerService.FindTvChannelManufacturer(existingTvChannelManufacturers, tvChannel.Id, manufacturerId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingManufacturerMapping = await _manufacturerService.GetTvChannelManufacturersByManufacturerIdAsync(manufacturerId, showHidden: true);
                    if (existingManufacturerMapping.Any())
                        displayOrder = existingManufacturerMapping.Max(x => x.DisplayOrder) + 1;
                    await _manufacturerService.InsertTvChannelManufacturerAsync(new TvChannelManufacturer
                    {
                        TvChannelId = tvChannel.Id,
                        ManufacturerId = manufacturerId,
                        DisplayOrder = displayOrder
                    });
                }
            }
        }

        protected virtual async Task SaveDiscountMappingsAsync(TvChannel tvChannel, TvChannelModel model)
        {
            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToSkus, showHidden: true, isActive: null);

            foreach (var discount in allDiscounts)
            {
                if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (await _tvChannelService.GetDiscountAppliedToTvChannelAsync(tvChannel.Id, discount.Id) is null)
                        await _tvChannelService.InsertDiscountTvChannelMappingAsync(new DiscountTvChannelMapping { EntityId = tvChannel.Id, DiscountId = discount.Id });
                }
                else
                {
                    //remove discount
                    if (await _tvChannelService.GetDiscountAppliedToTvChannelAsync(tvChannel.Id, discount.Id) is DiscountTvChannelMapping discountTvChannelMapping)
                        await _tvChannelService.DeleteDiscountTvChannelMappingAsync(discountTvChannelMapping);
                }
            }

            await _tvChannelService.UpdateTvChannelAsync(tvChannel);
            await _tvChannelService.UpdateHasDiscountsAppliedAsync(tvChannel);
        }

        protected virtual async Task<string> GetAttributesXmlForTvChannelAttributeCombinationAsync(IFormCollection form, List<string> warnings, int tvChannelId)
        {
            var attributesXml = string.Empty;

            //get tvChannel attribute mappings (exclude non-combinable attributes)
            var attributes = (await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannelId))
                .Where(tvChannelAttributeMapping => !tvChannelAttributeMapping.IsNonCombinable()).ToList();

            foreach (var attribute in attributes)
            {
                var controlId = $"{TvProgCatalogDefaults.TvChannelAttributePrefix}{attribute.Id}";
                StringValues ctrlAttributes;

                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        ctrlAttributes = form[controlId];
                        if (!string.IsNullOrEmpty(ctrlAttributes))
                        {
                            var selectedAttributeId = int.Parse(ctrlAttributes);
                            if (selectedAttributeId > 0)
                                attributesXml = _tvChannelAttributeParser.AddTvChannelAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                        }

                        break;
                    case AttributeControlType.Checkboxes:
                        var cblAttributes = form[controlId].ToString();
                        if (!string.IsNullOrEmpty(cblAttributes))
                        {
                            foreach (var item in cblAttributes.Split(new[] { ',' },
                                StringSplitOptions.RemoveEmptyEntries))
                            {
                                var selectedAttributeId = int.Parse(item);
                                if (selectedAttributeId > 0)
                                    attributesXml = _tvChannelAttributeParser.AddTvChannelAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        //load read-only (already server-side selected) values
                        var attributeValues = await _tvChannelAttributeService.GetTvChannelAttributeValuesAsync(attribute.Id);
                        foreach (var selectedAttributeId in attributeValues
                            .Where(v => v.IsPreSelected)
                            .Select(v => v.Id)
                            .ToList())
                        {
                            attributesXml = _tvChannelAttributeParser.AddTvChannelAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                        }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        ctrlAttributes = form[controlId];
                        if (!string.IsNullOrEmpty(ctrlAttributes))
                        {
                            var enteredText = ctrlAttributes.ToString().Trim();
                            attributesXml = _tvChannelAttributeParser.AddTvChannelAttribute(attributesXml,
                                attribute, enteredText);
                        }

                        break;
                    case AttributeControlType.Datepicker:
                        var date = form[controlId + "_day"];
                        var month = form[controlId + "_month"];
                        var year = form[controlId + "_year"];
                        DateTime? selectedDate = null;
                        try
                        {
                            selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(date));
                        }
                        catch
                        {
                            //ignore any exception
                        }

                        if (selectedDate.HasValue)
                        {
                            attributesXml = _tvChannelAttributeParser.AddTvChannelAttribute(attributesXml,
                                attribute, selectedDate.Value.ToString("D"));
                        }

                        break;
                    case AttributeControlType.FileUpload:
                        var httpPostedFile = Request.Form.Files[controlId];
                        if (!string.IsNullOrEmpty(httpPostedFile?.FileName))
                        {
                            var fileSizeOk = true;
                            if (attribute.ValidationFileMaximumSize.HasValue)
                            {
                                //compare in bytes
                                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                                if (httpPostedFile.Length > maxFileSizeBytes)
                                {
                                    warnings.Add(string.Format(
                                        await _localizationService.GetResourceAsync("ShoppingCart.MaximumUploadedFileSize"),
                                        attribute.ValidationFileMaximumSize.Value));
                                    fileSizeOk = false;
                                }
                            }

                            if (fileSizeOk)
                            {
                                //save an uploaded file
                                var download = new Download
                                {
                                    DownloadGuid = Guid.NewGuid(),
                                    UseDownloadUrl = false,
                                    DownloadUrl = string.Empty,
                                    DownloadBinary = await _downloadService.GetDownloadBitsAsync(httpPostedFile),
                                    ContentType = httpPostedFile.ContentType,
                                    Filename = _fileProvider.GetFileNameWithoutExtension(httpPostedFile.FileName),
                                    Extension = _fileProvider.GetFileExtension(httpPostedFile.FileName),
                                    IsNew = true
                                };
                                await _downloadService.InsertDownloadAsync(download);

                                //save attribute
                                attributesXml = _tvChannelAttributeParser.AddTvChannelAttribute(attributesXml,
                                    attribute, download.DownloadGuid.ToString());
                            }
                        }

                        break;
                    default:
                        break;
                }
            }

            //validate conditional attributes (if specified)
            foreach (var attribute in attributes)
            {
                var conditionMet = await _tvChannelAttributeParser.IsConditionMetAsync(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _tvChannelAttributeParser.RemoveTvChannelAttribute(attributesXml, attribute);
                }
            }

            return attributesXml;
        }

        protected virtual string[] ParseTvChannelTags(string tvChannelTags)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(tvChannelTags))
                return result.ToArray();

            var values = tvChannelTags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var val in values)
                if (!string.IsNullOrEmpty(val.Trim()))
                    result.Add(val.Trim());

            return result.ToArray();
        }

        protected virtual async Task SaveTvChannelWarehouseInventoryAsync(TvChannel tvChannel, TvChannelModel model)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (model.ManageInventoryMethodId != (int)ManageInventoryMethod.ManageStock)
                return;

            if (!model.UseMultipleWarehouses)
                return;

            var warehouses = await _shippingService.GetAllWarehousesAsync();

            var formData = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());

            foreach (var warehouse in warehouses)
            {
                //parse stock quantity
                var stockQuantity = 0;
                foreach (var formKey in formData.Keys)
                {
                    if (!formKey.Equals($"warehouse_qty_{warehouse.Id}", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    _ = int.TryParse(formData[formKey], out stockQuantity);
                    break;
                }

                //parse reserved quantity
                var reservedQuantity = 0;
                foreach (var formKey in formData.Keys)
                    if (formKey.Equals($"warehouse_reserved_{warehouse.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _ = int.TryParse(formData[formKey], out reservedQuantity);
                        break;
                    }

                //parse "used" field
                var used = false;
                foreach (var formKey in formData.Keys)
                    if (formKey.Equals($"warehouse_used_{warehouse.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _ = int.TryParse(formData[formKey], out var tmp);
                        used = tmp == warehouse.Id;
                        break;
                    }

                //quantity change history message
                var message = $"{await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.MultipleWarehouses")} {await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Edit")}";

                var existingPwI = (await _tvChannelService.GetAllTvChannelWarehouseInventoryRecordsAsync(tvChannel.Id)).FirstOrDefault(x => x.WarehouseId == warehouse.Id);
                if (existingPwI != null)
                {
                    if (used)
                    {
                        var previousStockQuantity = existingPwI.StockQuantity;

                        //update existing record
                        existingPwI.StockQuantity = stockQuantity;
                        existingPwI.ReservedQuantity = reservedQuantity;
                        await _tvChannelService.UpdateTvChannelWarehouseInventoryAsync(existingPwI);

                        //quantity change history
                        await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannel, existingPwI.StockQuantity - previousStockQuantity, existingPwI.StockQuantity,
                            existingPwI.WarehouseId, message);
                    }
                    else
                    {
                        //delete. no need to store record for qty 0
                        await _tvChannelService.DeleteTvChannelWarehouseInventoryAsync(existingPwI);

                        //quantity change history
                        await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannel, -existingPwI.StockQuantity, 0, existingPwI.WarehouseId, message);
                    }
                }
                else
                {
                    if (!used)
                        continue;

                    //no need to insert a record for qty 0
                    existingPwI = new TvChannelWarehouseInventory
                    {
                        WarehouseId = warehouse.Id,
                        TvChannelId = tvChannel.Id,
                        StockQuantity = stockQuantity,
                        ReservedQuantity = reservedQuantity
                    };

                    await _tvChannelService.InsertTvChannelWarehouseInventoryAsync(existingPwI);

                    //quantity change history
                    await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannel, existingPwI.StockQuantity, existingPwI.StockQuantity,
                        existingPwI.WarehouseId, message);
                }
            }
        }

        protected virtual async Task SaveConditionAttributesAsync(TvChannelAttributeMapping tvChannelAttributeMapping,
            TvChannelAttributeConditionModel model, IFormCollection form)
        {
            string attributesXml = null;
            if (model.EnableCondition)
            {
                var attribute = await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(model.SelectedTvChannelAttributeId);
                if (attribute != null)
                {
                    var controlId = $"{TvProgCatalogDefaults.TvChannelAttributePrefix}{attribute.Id}";
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                //for conditions we should empty values save even when nothing is selected
                                //otherwise "attributesXml" will be empty
                                //hence we won't be able to find a selected attribute
                                attributesXml = _tvChannelAttributeParser.AddTvChannelAttribute(null, attribute,
                                    selectedAttributeId > 0 ? selectedAttributeId.ToString() : string.Empty);
                            }
                            else
                            {
                                //for conditions we should empty values save even when nothing is selected
                                //otherwise "attributesXml" will be empty
                                //hence we won't be able to find a selected attribute
                                attributesXml = _tvChannelAttributeParser.AddTvChannelAttribute(null,
                                    attribute, string.Empty);
                            }

                            break;
                        case AttributeControlType.Checkboxes:
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                var anyValueSelected = false;
                                foreach (var item in cblAttributes.ToString()
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId <= 0)
                                        continue;

                                    attributesXml = _tvChannelAttributeParser.AddTvChannelAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                                    anyValueSelected = true;
                                }

                                if (!anyValueSelected)
                                {
                                    //for conditions we should save empty values even when nothing is selected
                                    //otherwise "attributesXml" will be empty
                                    //hence we won't be able to find a selected attribute
                                    attributesXml = _tvChannelAttributeParser.AddTvChannelAttribute(null,
                                        attribute, string.Empty);
                                }
                            }
                            else
                            {
                                //for conditions we should save empty values even when nothing is selected
                                //otherwise "attributesXml" will be empty
                                //hence we won't be able to find a selected attribute
                                attributesXml = _tvChannelAttributeParser.AddTvChannelAttribute(null,
                                    attribute, string.Empty);
                            }

                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                        case AttributeControlType.Datepicker:
                        case AttributeControlType.FileUpload:
                        default:
                            //these attribute types are supported as conditions
                            break;
                    }
                }
            }

            tvChannelAttributeMapping.ConditionAttributeXml = attributesXml;
            await _tvChannelAttributeService.UpdateTvChannelAttributeMappingAsync(tvChannelAttributeMapping);
        }

        protected virtual async Task GenerateAttributeCombinationsAsync(TvChannel tvChannel, IList<int> allowedAttributeIds = null)
        {
            var allAttributesXml = await _tvChannelAttributeParser.GenerateAllCombinationsAsync(tvChannel, true, allowedAttributeIds);
            foreach (var attributesXml in allAttributesXml)
            {
                var existingCombination = await _tvChannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvChannel, attributesXml);

                //already exists?
                if (existingCombination != null)
                    continue;

                //new one
                var warnings = new List<string>();
                warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentUserAsync(),
                    ShoppingCartType.ShoppingCart, tvChannel, 1, attributesXml, true, true, true));
                if (warnings.Count != 0)
                    continue;

                //save combination
                var combination = new TvChannelAttributeCombination
                {
                    TvChannelId = tvChannel.Id,
                    AttributesXml = attributesXml,
                    StockQuantity = 0,
                    AllowOutOfStockOrders = false,
                    Sku = null,
                    ManufacturerPartNumber = null,
                    Gtin = null,
                    OverriddenPrice = null,
                    NotifyAdminForQuantityBelow = 1,
                    PictureId = 0
                };
                await _tvChannelAttributeService.InsertTvChannelAttributeCombinationAsync(combination);
            }
        }

        protected virtual async Task PingVideoUrlAsync(string videoUrl)
        {
            var path = videoUrl.StartsWith("/") ? $"{_webHelper.GetStoreLocation()}{videoUrl.TrimStart('/')}" : videoUrl;

            var client = _httpClientFactory.CreateClient(TvProgHttpDefaults.DefaultHttpClient);
            await client.GetStringAsync(path);
        }

        #endregion

        #region Methods

        #region TvChannel list / create / edit / delete

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelSearchModelAsync(new TvChannelSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelList(TvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-tvchannel-by-sku")]
        public virtual async Task<IActionResult> GoToSku(TvChannelSearchModel searchModel)
        {
            //try to load a tvChannel entity, if not found, then try to load a tvChannel attribute combination
            var tvChannelId = (await _tvChannelService.GetTvChannelBySkuAsync(searchModel.GoDirectlyToSku))?.Id
                ?? (await _tvChannelAttributeService.GetTvChannelAttributeCombinationBySkuAsync(searchModel.GoDirectlyToSku))?.TvChannelId;

            if (tvChannelId != null)
                return RedirectToAction("Edit", "TvChannel", new { id = tvChannelId });

            //not found
            return await List();
        }

        public virtual async Task<IActionResult> Create(bool showtour = false)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //validate maximum number of tvChannels per vendor
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (_vendorSettings.MaximumTvChannelNumber > 0 && currentVendor != null
                && await _tvChannelService.GetNumberOfTvChannelsByVendorIdAsync(currentVendor.Id) >= _vendorSettings.MaximumTvChannelNumber)
            {
                _notificationService.ErrorNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.ExceededMaximumNumber"),
                    _vendorSettings.MaximumTvChannelNumber));
                return RedirectToAction("List");
            }

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelModelAsync(new TvChannelModel(), null);

            //show configuration tour
            if (showtour)
            {
                var user = await _workContext.GetCurrentUserAsync();
                var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, TvProgUserDefaults.HideConfigurationStepsAttribute);
                var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, TvProgUserDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(TvChannelModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //validate maximum number of tvChannels per vendor
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (_vendorSettings.MaximumTvChannelNumber > 0 && currentVendor != null
                && await _tvChannelService.GetNumberOfTvChannelsByVendorIdAsync(currentVendor.Id) >= _vendorSettings.MaximumTvChannelNumber)
            {
                _notificationService.ErrorNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.ExceededMaximumNumber"),
                    _vendorSettings.MaximumTvChannelNumber));
                return RedirectToAction("List");
            }

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his tvChannels
                if (currentVendor != null)
                    model.VendorId = currentVendor.Id;

                //vendors cannot edit "Show on home page" property
                if (currentVendor != null && model.ShowOnHomepage)
                    model.ShowOnHomepage = false;

                //tvChannel
                var tvChannel = model.ToEntity<TvChannel>();
                tvChannel.CreatedOnUtc = DateTime.UtcNow;
                tvChannel.UpdatedOnUtc = DateTime.UtcNow;
                await _tvChannelService.InsertTvChannelAsync(tvChannel);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(tvChannel, model.SeName, tvChannel.Name, true);
                await _urlRecordService.SaveSlugAsync(tvChannel, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(tvChannel, model);

                //categories
                await SaveCategoryMappingsAsync(tvChannel, model);

                //manufacturers
                await SaveManufacturerMappingsAsync(tvChannel, model);

                //ACL (user roles)
                await SaveTvChannelAclAsync(tvChannel, model);

                //stores
                await _tvChannelService.UpdateTvChannelStoreMappingsAsync(tvChannel, model.SelectedStoreIds);

                //discounts
                await SaveDiscountMappingsAsync(tvChannel, model);

                //tags
                await _tvChannelTagService.UpdateTvChannelTagsAsync(tvChannel, ParseTvChannelTags(model.TvChannelTags));

                //warehouses
                await SaveTvChannelWarehouseInventoryAsync(tvChannel, model);

                //quantity change history
                await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannel, tvChannel.StockQuantity, tvChannel.StockQuantity, tvChannel.WarehouseId,
                    await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Edit"));

                //activity log
                await _userActivityService.InsertActivityAsync("AddNewTvChannel",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewTvChannel"), tvChannel.Name), tvChannel);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = tvChannel.Id });
            }

            //prepare model
            model = await _tvChannelModelFactory.PrepareTvChannelModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(id);
            if (tvChannel == null || tvChannel.Deleted)
                return RedirectToAction("List");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelModelAsync(null, tvChannel);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(TvChannelModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(model.Id);
            if (tvChannel == null || tvChannel.Deleted)
                return RedirectToAction("List");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List");

            //check if the tvChannel quantity has been changed while we were editing the tvChannel
            //and if it has been changed then we show error notification
            //and redirect on the editing page without data saving
            if (tvChannel.StockQuantity != model.LastStockQuantity)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Fields.StockQuantity.ChangedWarning"));
                return RedirectToAction("Edit", new { id = tvChannel.Id });
            }

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his tvChannels
                if (currentVendor != null)
                    model.VendorId = currentVendor.Id;

                //we do not validate maximum number of tvChannels per vendor when editing existing tvChannels (only during creation of new tvChannels)
                //vendors cannot edit "Show on home page" property
                if (currentVendor != null && model.ShowOnHomepage != tvChannel.ShowOnHomepage)
                    model.ShowOnHomepage = tvChannel.ShowOnHomepage;

                //some previously used values
                var prevTotalStockQuantity = await _tvChannelService.GetTotalStockQuantityAsync(tvChannel);
                var prevDownloadId = tvChannel.DownloadId;
                var prevSampleDownloadId = tvChannel.SampleDownloadId;
                var previousStockQuantity = tvChannel.StockQuantity;
                var previousWarehouseId = tvChannel.WarehouseId;
                var previousTvChannelType = tvChannel.TvChannelType;

                //tvChannel
                tvChannel = model.ToEntity(tvChannel);

                tvChannel.UpdatedOnUtc = DateTime.UtcNow;
                await _tvChannelService.UpdateTvChannelAsync(tvChannel);

                //remove associated tvChannels
                if (previousTvChannelType == TvChannelType.GroupedTvChannel && tvChannel.TvChannelType == TvChannelType.SimpleTvChannel)
                {
                    var store = await _storeContext.GetCurrentStoreAsync();
                    var storeId = store?.Id ?? 0;
                    var vendorId = currentVendor?.Id ?? 0;

                    var associatedTvChannels = await _tvChannelService.GetAssociatedTvChannelsAsync(tvChannel.Id, storeId, vendorId);
                    foreach (var associatedTvChannel in associatedTvChannels)
                    {
                        associatedTvChannel.ParentGroupedTvChannelId = 0;
                        await _tvChannelService.UpdateTvChannelAsync(associatedTvChannel);
                    }
                }

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(tvChannel, model.SeName, tvChannel.Name, true);
                await _urlRecordService.SaveSlugAsync(tvChannel, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(tvChannel, model);

                //tags
                await _tvChannelTagService.UpdateTvChannelTagsAsync(tvChannel, ParseTvChannelTags(model.TvChannelTags));

                //warehouses
                await SaveTvChannelWarehouseInventoryAsync(tvChannel, model);

                //categories
                await SaveCategoryMappingsAsync(tvChannel, model);

                //manufacturers
                await SaveManufacturerMappingsAsync(tvChannel, model);

                //ACL (user roles)
                await SaveTvChannelAclAsync(tvChannel, model);

                //stores
                await _tvChannelService.UpdateTvChannelStoreMappingsAsync(tvChannel, model.SelectedStoreIds);

                //discounts
                await SaveDiscountMappingsAsync(tvChannel, model);

                //picture seo names
                await UpdatePictureSeoNamesAsync(tvChannel);

                //back in stock notifications
                if (tvChannel.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                    tvChannel.BackorderMode == BackorderMode.NoBackorders &&
                    tvChannel.AllowBackInStockSubscriptions &&
                    await _tvChannelService.GetTotalStockQuantityAsync(tvChannel) > 0 &&
                    prevTotalStockQuantity <= 0 &&
                    tvChannel.Published &&
                    !tvChannel.Deleted)
                {
                    await _backInStockSubscriptionService.SendNotificationsToSubscribersAsync(tvChannel);
                }

                //delete an old "download" file (if deleted or updated)
                if (prevDownloadId > 0 && prevDownloadId != tvChannel.DownloadId)
                {
                    var prevDownload = await _downloadService.GetDownloadByIdAsync(prevDownloadId);
                    if (prevDownload != null)
                        await _downloadService.DeleteDownloadAsync(prevDownload);
                }

                //delete an old "sample download" file (if deleted or updated)
                if (prevSampleDownloadId > 0 && prevSampleDownloadId != tvChannel.SampleDownloadId)
                {
                    var prevSampleDownload = await _downloadService.GetDownloadByIdAsync(prevSampleDownloadId);
                    if (prevSampleDownload != null)
                        await _downloadService.DeleteDownloadAsync(prevSampleDownload);
                }

                //quantity change history
                if (previousWarehouseId != tvChannel.WarehouseId)
                {
                    //warehouse is changed 
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

                    var message = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse"), oldWarehouseMessage, newWarehouseMessage);

                    //record history
                    await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannel, -previousStockQuantity, 0, previousWarehouseId, message);
                    await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannel, tvChannel.StockQuantity, tvChannel.StockQuantity, tvChannel.WarehouseId, message);
                }
                else
                {
                    await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannel, tvChannel.StockQuantity - previousStockQuantity, tvChannel.StockQuantity,
                        tvChannel.WarehouseId, await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Edit"));
                }

                //activity log
                await _userActivityService.InsertActivityAsync("EditTvChannel",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditTvChannel"), tvChannel.Name), tvChannel);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = tvChannel.Id });
            }

            //prepare model
            model = await _tvChannelModelFactory.PrepareTvChannelModelAsync(model, tvChannel, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(id);
            if (tvChannel == null)
                return RedirectToAction("List");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List");

            await _tvChannelService.DeleteTvChannelAsync(tvChannel);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteTvChannel",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteTvChannel"), tvChannel.Name), tvChannel);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var currentVendor = await _workContext.GetCurrentVendorAsync();
            await _tvChannelService.DeleteTvChannelsAsync((await _tvChannelService.GetTvChannelsByIdsAsync(selectedIds.ToArray()))
                .Where(p => currentVendor == null || p.VendorId == currentVendor.Id).ToList());

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> CopyTvChannel(TvChannelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            var copyModel = model.CopyTvChannelModel;
            try
            {
                var originalTvChannel = await _tvChannelService.GetTvChannelByIdAsync(copyModel.Id);

                //a vendor should have access only to his tvChannels
                var currentVendor = await _workContext.GetCurrentVendorAsync();
                if (currentVendor != null && originalTvChannel.VendorId != currentVendor.Id)
                    return RedirectToAction("List");

                var newTvChannel = await _copyTvChannelService.CopyTvChannelAsync(originalTvChannel, copyModel.Name, copyModel.Published, copyModel.CopyMultimedia);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Copied"));

                return RedirectToAction("Edit", new { id = newTvChannel.Id });
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = copyModel.Id });
            }
        }

        //action displaying notification (warning) to a store owner that entered SKU already exists
        public virtual async Task<IActionResult> SkuReservedWarning(int tvChannelId, string sku)
        {
            string message;

            //check whether tvChannel with passed SKU already exists
            var tvChannelBySku = await _tvChannelService.GetTvChannelBySkuAsync(sku);
            if (tvChannelBySku != null)
            {
                if (tvChannelBySku.Id == tvChannelId)
                    return Json(new { Result = string.Empty });

                message = string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Fields.Sku.Reserved"), tvChannelBySku.Name);
                return Json(new { Result = message });
            }

            //check whether combination with passed SKU already exists
            var combinationBySku = await _tvChannelAttributeService.GetTvChannelAttributeCombinationBySkuAsync(sku);
            if (combinationBySku == null)
                return Json(new { Result = string.Empty });

            message = string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Fields.Sku.Reserved"),
                (await _tvChannelService.GetTvChannelByIdAsync(combinationBySku.TvChannelId))?.Name);

            return Json(new { Result = message });
        }

        #endregion

        #region Required tvChannels

        [HttpPost]
        public virtual async Task<IActionResult> LoadTvChannelFriendlyNames(string tvChannelIds)
        {
            var result = string.Empty;

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return Json(new { Text = result });

            if (string.IsNullOrWhiteSpace(tvChannelIds))
                return Json(new { Text = result });

            var ids = new List<int>();
            var rangeArray = tvChannelIds
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            foreach (var str1 in rangeArray)
            {
                if (int.TryParse(str1, out var tmp1))
                    ids.Add(tmp1);
            }

            var tvChannels = await _tvChannelService.GetTvChannelsByIdsAsync(ids.ToArray());
            for (var i = 0; i <= tvChannels.Count - 1; i++)
            {
                result += tvChannels[i].Name;
                if (i != tvChannels.Count - 1)
                    result += ", ";
            }

            return Json(new { Text = result });
        }

        public virtual async Task<IActionResult> RequiredTvChannelAddPopup()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //prepare model
            var model = await _tvChannelModelFactory.PrepareAddRequiredTvChannelSearchModelAsync(new AddRequiredTvChannelSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RequiredTvChannelAddPopupList(AddRequiredTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvChannelModelFactory.PrepareAddRequiredTvChannelListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Related tvChannels

        [HttpPost]
        public virtual async Task<IActionResult> RelatedTvChannelList(RelatedTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareRelatedTvChannelListModelAsync(searchModel, tvChannel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RelatedTvChannelUpdate(RelatedTvChannelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a related tvChannel with the specified id
            var relatedTvChannel = await _tvChannelService.GetRelatedTvChannelByIdAsync(model.Id)
                ?? throw new ArgumentException("No related tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(relatedTvChannel.TvChannelId1);
                if (tvChannel != null && tvChannel.VendorId != currentVendor.Id)
                    return Content("This is not your tvChannel");
            }

            relatedTvChannel.DisplayOrder = model.DisplayOrder;
            await _tvChannelService.UpdateRelatedTvChannelAsync(relatedTvChannel);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> RelatedTvChannelDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a related tvChannel with the specified id
            var relatedTvChannel = await _tvChannelService.GetRelatedTvChannelByIdAsync(id)
                ?? throw new ArgumentException("No related tvChannel found with the specified id");

            var tvChannelId = relatedTvChannel.TvChannelId1;

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
                if (tvChannel != null && tvChannel.VendorId != currentVendor.Id)
                    return Content("This is not your tvChannel");
            }

            await _tvChannelService.DeleteRelatedTvChannelAsync(relatedTvChannel);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> RelatedTvChannelAddPopup(int tvChannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //prepare model
            var model = await _tvChannelModelFactory.PrepareAddRelatedTvChannelSearchModelAsync(new AddRelatedTvChannelSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RelatedTvChannelAddPopupList(AddRelatedTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvChannelModelFactory.PrepareAddRelatedTvChannelListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> RelatedTvChannelAddPopup(AddRelatedTvChannelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            var selectedTvChannels = await _tvChannelService.GetTvChannelsByIdsAsync(model.SelectedTvChannelIds.ToArray());
            if (selectedTvChannels.Any())
            {
                var existingRelatedTvChannels = await _tvChannelService.GetRelatedTvChannelsByTvChannelId1Async(model.TvChannelId, showHidden: true);
                var currentVendor = await _workContext.GetCurrentVendorAsync();
                foreach (var tvChannel in selectedTvChannels)
                {
                    //a vendor should have access only to his tvChannels
                    if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                        continue;

                    if (_tvChannelService.FindRelatedTvChannel(existingRelatedTvChannels, model.TvChannelId, tvChannel.Id) != null)
                        continue;

                    await _tvChannelService.InsertRelatedTvChannelAsync(new RelatedTvChannel
                    {
                        TvChannelId1 = model.TvChannelId,
                        TvChannelId2 = tvChannel.Id,
                        DisplayOrder = 1
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddRelatedTvChannelSearchModel());
        }

        #endregion

        #region Cross-sell tvChannels

        [HttpPost]
        public virtual async Task<IActionResult> CrossSellTvChannelList(CrossSellTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareCrossSellTvChannelListModelAsync(searchModel, tvChannel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CrossSellTvChannelDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a cross-sell tvChannel with the specified id
            var crossSellTvChannel = await _tvChannelService.GetCrossSellTvChannelByIdAsync(id)
                ?? throw new ArgumentException("No cross-sell tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(crossSellTvChannel.TvChannelId1);
                if (tvChannel != null && tvChannel.VendorId != currentVendor.Id)
                    return Content("This is not your tvChannel");
            }

            await _tvChannelService.DeleteCrossSellTvChannelAsync(crossSellTvChannel);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> CrossSellTvChannelAddPopup(int tvChannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //prepare model
            var model = await _tvChannelModelFactory.PrepareAddCrossSellTvChannelSearchModelAsync(new AddCrossSellTvChannelSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CrossSellTvChannelAddPopupList(AddCrossSellTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvChannelModelFactory.PrepareAddCrossSellTvChannelListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> CrossSellTvChannelAddPopup(AddCrossSellTvChannelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            var selectedTvChannels = await _tvChannelService.GetTvChannelsByIdsAsync(model.SelectedTvChannelIds.ToArray());
            if (selectedTvChannels.Any())
            {
                var existingCrossSellTvChannels = await _tvChannelService.GetCrossSellTvChannelsByTvChannelId1Async(model.TvChannelId, showHidden: true);
                var currentVendor = await _workContext.GetCurrentVendorAsync();
                foreach (var tvChannel in selectedTvChannels)
                {
                    //a vendor should have access only to his tvChannels
                    if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                        continue;

                    if (_tvChannelService.FindCrossSellTvChannel(existingCrossSellTvChannels, model.TvChannelId, tvChannel.Id) != null)
                        continue;

                    await _tvChannelService.InsertCrossSellTvChannelAsync(new CrossSellTvChannel
                    {
                        TvChannelId1 = model.TvChannelId,
                        TvChannelId2 = tvChannel.Id
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddCrossSellTvChannelSearchModel());
        }

        #endregion

        #region Associated tvChannels

        [HttpPost]
        public virtual async Task<IActionResult> AssociatedTvChannelList(AssociatedTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareAssociatedTvChannelListModelAsync(searchModel, tvChannel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociatedTvChannelUpdate(AssociatedTvChannelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get an associated tvChannel with the specified id
            var associatedTvChannel = await _tvChannelService.GetTvChannelByIdAsync(model.Id)
                ?? throw new ArgumentException("No associated tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && associatedTvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            associatedTvChannel.DisplayOrder = model.DisplayOrder;
            await _tvChannelService.UpdateTvChannelAsync(associatedTvChannel);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociatedTvChannelDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get an associated tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(id)
                ?? throw new ArgumentException("No associated tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            tvChannel.ParentGroupedTvChannelId = 0;
            await _tvChannelService.UpdateTvChannelAsync(tvChannel);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> AssociatedTvChannelAddPopup(int tvChannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //prepare model
            var model = await _tvChannelModelFactory.PrepareAddAssociatedTvChannelSearchModelAsync(new AddAssociatedTvChannelSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociatedTvChannelAddPopupList(AddAssociatedTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvChannelModelFactory.PrepareAddAssociatedTvChannelListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> AssociatedTvChannelAddPopup(AddAssociatedTvChannelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            var selectedTvChannels = await _tvChannelService.GetTvChannelsByIdsAsync(model.SelectedTvChannelIds.ToArray());

            var tryToAddSelfGroupedTvChannel = selectedTvChannels
                .Select(p => p.Id)
                .Contains(model.TvChannelId);

            if (selectedTvChannels.Any())
            {
                foreach (var tvChannel in selectedTvChannels)
                {
                    if (tvChannel.Id == model.TvChannelId)
                        continue;

                    //a vendor should have access only to his tvChannels
                    var currentVendor = await _workContext.GetCurrentVendorAsync();
                    if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                        continue;

                    tvChannel.ParentGroupedTvChannelId = model.TvChannelId;
                    await _tvChannelService.UpdateTvChannelAsync(tvChannel);
                }
            }

            if (tryToAddSelfGroupedTvChannel)
            {
                _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.AssociatedTvChannels.TryToAddSelfGroupedTvChannel"));

                var addAssociatedTvChannelSearchModel = await _tvChannelModelFactory.PrepareAddAssociatedTvChannelSearchModelAsync(new AddAssociatedTvChannelSearchModel());
                //set current tvChannel id
                addAssociatedTvChannelSearchModel.TvChannelId = model.TvChannelId;

                ViewBag.RefreshPage = true;

                return View(addAssociatedTvChannelSearchModel);
            }

            ViewBag.RefreshPage = true;

            ViewBag.ClosePage = true;

            return View(new AddAssociatedTvChannelSearchModel());
        }

        #endregion

        #region TvChannel pictures

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> TvChannelPictureAdd(int tvChannelId, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            if (tvChannelId == 0)
                throw new ArgumentException();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            var files = form.Files.ToList();
            if (!files.Any())
                return Json(new { success = false });

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List");
            try
            {
                foreach (var file in files)
                {
                    //insert picture
                    var picture = await _pictureService.InsertPictureAsync(file);

                    await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(tvChannel.Name));

                    await _tvChannelService.InsertTvChannelPictureAsync(new TvChannelPicture
                    {
                        PictureId = picture.Id,
                        TvChannelId = tvChannel.Id,
                        DisplayOrder = 0
                    });
                }
            }
            catch (Exception exc)
            {
                return Json(new 
                    { 
                        success = false, 
                        message = $"{await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Multimedia.Pictures.Alert.PictureAdd")} {exc.Message}", 
                    });
            }
            
            return Json(new { success = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelPictureList(TvChannelPictureSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelPictureListModelAsync(searchModel, tvChannel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelPictureUpdate(TvChannelPictureModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel picture with the specified id
            var tvChannelPicture = await _tvChannelService.GetTvChannelPictureByIdAsync(model.Id)
                ?? throw new ArgumentException("No tvChannel picture found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelPicture.TvChannelId);
                if (tvChannel != null && tvChannel.VendorId != currentVendor.Id)
                    return Content("This is not your tvChannel");
            }

            //try to get a picture with the specified id
            var picture = await _pictureService.GetPictureByIdAsync(tvChannelPicture.PictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            await _pictureService.UpdatePictureAsync(picture.Id,
                await _pictureService.LoadPictureBinaryAsync(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute,
                model.OverrideTitleAttribute);

            tvChannelPicture.DisplayOrder = model.DisplayOrder;
            await _tvChannelService.UpdateTvChannelPictureAsync(tvChannelPicture);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelPictureDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel picture with the specified id
            var tvChannelPicture = await _tvChannelService.GetTvChannelPictureByIdAsync(id)
                ?? throw new ArgumentException("No tvChannel picture found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelPicture.TvChannelId);
                if (tvChannel != null && tvChannel.VendorId != currentVendor.Id)
                    return Content("This is not your tvChannel");
            }

            var pictureId = tvChannelPicture.PictureId;
            await _tvChannelService.DeleteTvChannelPictureAsync(tvChannelPicture);

            //try to get a picture with the specified id
            var picture = await _pictureService.GetPictureByIdAsync(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            await _pictureService.DeletePictureAsync(picture);

            return new NullJsonResult();
        }

        #endregion

        #region TvChannel videos

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelVideoAdd(int tvChannelId, [Validate] TvChannelVideoModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            if (tvChannelId == 0)
                throw new ArgumentException();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            var videoUrl = model.VideoUrl.TrimStart('~');

            try
            {
                await PingVideoUrlAsync(videoUrl);
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    error = $"{await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Multimedia.Videos.Alert.VideoAdd")} {exc.Message}",
                });
            }

            if (!ModelState.IsValid) 
                return ErrorJson(ModelState.SerializeErrors());

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List");
            try
            {
                var video = new Video
                {
                    VideoUrl = videoUrl
                };

                //insert video
                await _videoService.InsertVideoAsync(video);

                await _tvChannelService.InsertTvChannelVideoAsync(new TvChannelVideo
                {
                    VideoId = video.Id,
                    TvChannelId = tvChannel.Id,
                    DisplayOrder = model.DisplayOrder
                });
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    error = $"{await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Multimedia.Videos.Alert.VideoAdd")} {exc.Message}",
                });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelVideoList(TvChannelVideoSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelVideoListModelAsync(searchModel, tvChannel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelVideoUpdate([Validate] TvChannelVideoModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel picture with the specified id
            var tvChannelVideo = await _tvChannelService.GetTvChannelVideoByIdAsync(model.Id)
                ?? throw new ArgumentException("No tvChannel video found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelVideo.TvChannelId);
                if (tvChannel != null && tvChannel.VendorId != currentVendor.Id)
                    return Content("This is not your tvChannel");
            }

            //try to get a video with the specified id
            var video = await _videoService.GetVideoByIdAsync(tvChannelVideo.VideoId)
                ?? throw new ArgumentException("No video found with the specified id");

            var videoUrl = model.VideoUrl.TrimStart('~');

            try
            {
                await PingVideoUrlAsync(videoUrl);
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    error = $"{await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Multimedia.Videos.Alert.VideoUpdate")} {exc.Message}",
                });
            }

            video.VideoUrl = videoUrl;

            await _videoService.UpdateVideoAsync(video);

            tvChannelVideo.DisplayOrder = model.DisplayOrder;
            await _tvChannelService.UpdateTvChannelVideoAsync(tvChannelVideo);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelVideoDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel video with the specified id
            var tvChannelVideo = await _tvChannelService.GetTvChannelVideoByIdAsync(id)
                ?? throw new ArgumentException("No tvChannel video found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelVideo.TvChannelId);
                if (tvChannel != null && tvChannel.VendorId != currentVendor.Id)
                    return Content("This is not your tvChannel");
            }

            var videoId = tvChannelVideo.VideoId;
            await _tvChannelService.DeleteTvChannelVideoAsync(tvChannelVideo);

            //try to get a video with the specified id
            var video = await _videoService.GetVideoByIdAsync(videoId)
                ?? throw new ArgumentException("No video found with the specified id");

            await _videoService.DeleteVideoAsync(video);

            return new NullJsonResult();
        }

        #endregion

        #region TvChannel specification attributes

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> TvChannelSpecificationAttributeAdd(AddSpecificationAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(model.TvChannelId);
            if (tvChannel == null)
            {
                _notificationService.ErrorNotification("No tvChannel found with the specified id");
                return RedirectToAction("List");
            }

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
            {
                return RedirectToAction("List");
            }

            //we allow filtering only for "Option" attribute type
            if (model.AttributeTypeId != (int)SpecificationAttributeType.Option)
                model.AllowFiltering = false;

            //we don't allow CustomValue for "Option" attribute type
            if (model.AttributeTypeId == (int)SpecificationAttributeType.Option)
                model.ValueRaw = null;

            //store raw html if field allow this
            if (model.AttributeTypeId == (int)SpecificationAttributeType.CustomText
                || model.AttributeTypeId == (int)SpecificationAttributeType.Hyperlink)
                model.ValueRaw = model.Value;

            var psa = model.ToEntity<TvChannelSpecificationAttribute>();
            psa.CustomValue = model.ValueRaw;
            await _specificationAttributeService.InsertTvChannelSpecificationAttributeAsync(psa);

            switch (psa.AttributeType)
            {
                case SpecificationAttributeType.CustomText:
                    foreach (var localized in model.Locales)
                    {
                        await _localizedEntityService.SaveLocalizedValueAsync(psa,
                            x => x.CustomValue,
                            localized.Value,
                            localized.LanguageId);
                    }

                    break;
                case SpecificationAttributeType.CustomHtmlText:
                    foreach (var localized in model.Locales)
                    {
                        await _localizedEntityService.SaveLocalizedValueAsync(psa,
                            x => x.CustomValue,
                            localized.ValueRaw,
                            localized.LanguageId);
                    }

                    break;
                case SpecificationAttributeType.Option:
                    break;
                case SpecificationAttributeType.Hyperlink:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (continueEditing)
                return RedirectToAction("TvChannelSpecAttributeAddOrEdit",
                    new { tvChannelId = psa.TvChannelId, specificationId = psa.Id });

            //select an appropriate card
            SaveSelectedCardName("tvchannel-specification-attributes");
            return RedirectToAction("Edit", new { id = model.TvChannelId });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelSpecAttrList(TvChannelSpecificationAttributeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelSpecificationAttributeListModelAsync(searchModel, tvChannel);

            return Json(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> TvChannelSpecAttrUpdate(AddSpecificationAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel specification attribute with the specified id
            var psa = await _specificationAttributeService.GetTvChannelSpecificationAttributeByIdAsync(model.SpecificationId);
            if (psa == null)
            {
                //select an appropriate card
                SaveSelectedCardName("tvchannel-specification-attributes");
                _notificationService.ErrorNotification("No tvChannel specification attribute found with the specified id");

                return RedirectToAction("Edit", new { id = model.TvChannelId });
            }

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null
                && (await _tvChannelService.GetTvChannelByIdAsync(psa.TvChannelId)).VendorId != currentVendor.Id)
            {
                _notificationService.ErrorNotification("This is not your tvChannel");

                return RedirectToAction("List");
            }

            //we allow filtering and change option only for "Option" attribute type
            //save localized values for CustomHtmlText and CustomText
            switch (model.AttributeTypeId)
            {
                case (int)SpecificationAttributeType.Option:
                    psa.AllowFiltering = model.AllowFiltering;
                    psa.SpecificationAttributeOptionId = model.SpecificationAttributeOptionId;

                    break;
                case (int)SpecificationAttributeType.CustomHtmlText:
                    psa.CustomValue = model.ValueRaw;
                    foreach (var localized in model.Locales)
                    {
                        await _localizedEntityService.SaveLocalizedValueAsync(psa,
                            x => x.CustomValue,
                            localized.ValueRaw,
                            localized.LanguageId);
                    }

                    break;
                case (int)SpecificationAttributeType.CustomText:
                    psa.CustomValue = model.Value;
                    foreach (var localized in model.Locales)
                    {
                        await _localizedEntityService.SaveLocalizedValueAsync(psa,
                            x => x.CustomValue,
                            localized.Value,
                            localized.LanguageId);
                    }

                    break;
                default:
                    psa.CustomValue = model.Value;

                    break;
            }

            psa.ShowOnTvChannelPage = model.ShowOnTvChannelPage;
            psa.DisplayOrder = model.DisplayOrder;
            await _specificationAttributeService.UpdateTvChannelSpecificationAttributeAsync(psa);

            if (continueEditing)
            {
                return RedirectToAction("TvChannelSpecAttributeAddOrEdit",
                    new { tvChannelId = psa.TvChannelId, specificationId = model.SpecificationId });
            }

            //select an appropriate card
            SaveSelectedCardName("tvchannel-specification-attributes");

            return RedirectToAction("Edit", new { id = psa.TvChannelId });
        }

        public virtual async Task<IActionResult> TvChannelSpecAttributeAddOrEdit(int tvChannelId, int? specificationId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            if (await _tvChannelService.GetTvChannelByIdAsync(tvChannelId) == null)
            {
                _notificationService.ErrorNotification("No tvChannel found with the specified id");
                return RedirectToAction("List");
            }

            //try to get a tvChannel specification attribute with the specified id
            try
            {
                var model = await _tvChannelModelFactory.PrepareAddSpecificationAttributeModelAsync(tvChannelId, specificationId);
                return View(model);
            }
            catch (Exception ex)
            {
                await _notificationService.ErrorNotificationAsync(ex);

                //select an appropriate card
                SaveSelectedCardName("tvchannel-specification-attributes");
                return RedirectToAction("Edit", new { id = tvChannelId });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelSpecAttrDelete(AddSpecificationAttributeModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel specification attribute with the specified id
            var psa = await _specificationAttributeService.GetTvChannelSpecificationAttributeByIdAsync(model.SpecificationId);
            if (psa == null)
            {
                //select an appropriate card
                SaveSelectedCardName("tvchannel-specification-attributes");
                _notificationService.ErrorNotification("No tvChannel specification attribute found with the specified id");
                return RedirectToAction("Edit", new { id = model.TvChannelId });
            }

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && (await _tvChannelService.GetTvChannelByIdAsync(psa.TvChannelId)).VendorId != currentVendor.Id)
            {
                _notificationService.ErrorNotification("This is not your tvChannel");
                return RedirectToAction("List", new { id = model.TvChannelId });
            }

            await _specificationAttributeService.DeleteTvChannelSpecificationAttributeAsync(psa);

            //select an appropriate card
            SaveSelectedCardName("tvchannel-specification-attributes");

            return RedirectToAction("Edit", new { id = psa.TvChannelId });
        }

        #endregion

        #region TvChannel tags

        public virtual async Task<IActionResult> TvChannelTags()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelTags))
                return AccessDeniedView();

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelTagSearchModelAsync(new TvChannelTagSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelTags(TvChannelTagSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelTags))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelTagListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelTagDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelTags))
                return AccessDeniedView();

            //try to get a tvChannel tag with the specified id
            var tag = await _tvChannelTagService.GetTvChannelTagByIdAsync(id)
                ?? throw new ArgumentException("No tvChannel tag found with the specified id");

            await _tvChannelTagService.DeleteTvChannelTagAsync(tag);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannelTags.Deleted"));

            return RedirectToAction("TvChannelTags");
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelTagsDelete(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelTags))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var tags = await _tvChannelTagService.GetTvChannelTagsByIdsAsync(selectedIds.ToArray());
            await _tvChannelTagService.DeleteTvChannelTagsAsync(tags);

            return Json(new { Result = true });
        }

        public virtual async Task<IActionResult> EditTvChannelTag(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelTags))
                return AccessDeniedView();

            //try to get a tvChannel tag with the specified id
            var tvChannelTag = await _tvChannelTagService.GetTvChannelTagByIdAsync(id);
            if (tvChannelTag == null)
                return RedirectToAction("List");

            //prepare tag model
            var model = await _tvChannelModelFactory.PrepareTvChannelTagModelAsync(null, tvChannelTag);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditTvChannelTag(TvChannelTagModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelTags))
                return AccessDeniedView();

            //try to get a tvChannel tag with the specified id
            var tvChannelTag = await _tvChannelTagService.GetTvChannelTagByIdAsync(model.Id);
            if (tvChannelTag == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                tvChannelTag.Name = model.Name;
                await _tvChannelTagService.UpdateTvChannelTagAsync(tvChannelTag);

                //locales
                await UpdateLocalesAsync(tvChannelTag, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannelTags.Updated"));

                return continueEditing ? RedirectToAction("EditTvChannelTag", new { id = tvChannelTag.Id }) : RedirectToAction("TvChannelTags");
            }

            //prepare model
            model = await _tvChannelModelFactory.PrepareTvChannelTagModelAsync(model, tvChannelTag, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Purchased with order

        [HttpPost]
        public virtual async Task<IActionResult> PurchasedWithOrders(TvChannelOrderSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelOrderListModelAsync(searchModel, tvChannel);

            return Json(model);
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("DownloadCatalogPDF")]
        [FormValueRequired("download-catalog-pdf")]
        public virtual async Task<IActionResult> DownloadCatalogAsPdf(TvChannelSearchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                model.SearchVendorId = currentVendor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: model.SearchCategoryId, showHidden: true));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var tvChannels = await _tvChannelService.SearchTvChannelsAsync(0,
                categoryIds: categoryIds,
                manufacturerIds: new List<int> { model.SearchManufacturerId },
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                tvChannelType: model.SearchTvChannelTypeId > 0 ? (TvChannelType?)model.SearchTvChannelTypeId : null,
                keywords: model.SearchTvChannelName,
                showHidden: true,
                overridePublished: overridePublished);

            try
            {
                byte[] bytes;
                await using (var stream = new MemoryStream())
                {
                    await _pdfService.PrintTvChannelsToPdfAsync(stream, tvChannels);
                    bytes = stream.ToArray();
                }

                return File(bytes, MimeTypes.ApplicationPdf, "pdfcatalog.pdf");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("ExportToXml")]
        [FormValueRequired("exportxml-all")]
        public virtual async Task<IActionResult> ExportXmlAll(TvChannelSearchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                model.SearchVendorId = currentVendor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: model.SearchCategoryId, showHidden: true));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var tvChannels = await _tvChannelService.SearchTvChannelsAsync(0,
                categoryIds: categoryIds,
                manufacturerIds: new List<int> { model.SearchManufacturerId },
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                tvChannelType: model.SearchTvChannelTypeId > 0 ? (TvChannelType?)model.SearchTvChannelTypeId : null,
                keywords: model.SearchTvChannelName,
                showHidden: true,
                overridePublished: overridePublished);

            try
            {
                var xml = await _exportManager.ExportTvChannelsToXmlAsync(tvChannels);

                return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "tvChannels.xml");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportXmlSelected(string selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            var tvChannels = new List<TvChannel>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                tvChannels.AddRange(await _tvChannelService.GetTvChannelsByIdsAsync(ids));
            }
            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                tvChannels = tvChannels.Where(p => p.VendorId == currentVendor.Id).ToList();
            }

            try
            {
                var xml = await _exportManager.ExportTvChannelsToXmlAsync(tvChannels);
                return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "tvChannels.xml");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("ExportToExcel")]
        [FormValueRequired("exportexcel-all")]
        public virtual async Task<IActionResult> ExportExcelAll(TvChannelSearchModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                model.SearchVendorId = currentVendor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: model.SearchCategoryId, showHidden: true));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var tvChannels = await _tvChannelService.SearchTvChannelsAsync(0,
                categoryIds: categoryIds,
                manufacturerIds: new List<int> { model.SearchManufacturerId },
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                tvChannelType: model.SearchTvChannelTypeId > 0 ? (TvChannelType?)model.SearchTvChannelTypeId : null,
                keywords: model.SearchTvChannelName,
                showHidden: true,
                overridePublished: overridePublished);

            try
            {
                var bytes = await _exportManager.ExportTvChannelsToXlsxAsync(tvChannels);

                return File(bytes, MimeTypes.TextXlsx, "tvChannels.xlsx");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);

                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportExcelSelected(string selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            var tvChannels = new List<TvChannel>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                tvChannels.AddRange(await _tvChannelService.GetTvChannelsByIdsAsync(ids));
            }
            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                tvChannels = tvChannels.Where(p => p.VendorId == currentVendor.Id).ToList();
            }

            try
            {
                var bytes = await _exportManager.ExportTvChannelsToXlsxAsync(tvChannels);

                return File(bytes, MimeTypes.TextXlsx, "tvChannels.xlsx");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportExcel(IFormFile importexcelfile)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            if (await _workContext.GetCurrentVendorAsync() != null && !_vendorSettings.AllowVendorsToImportTvChannels)
                //a vendor can not import tvChannels
                return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                {
                    await _importManager.ImportTvChannelsFromXlsxAsync(importexcelfile.OpenReadStream());
                }
                else
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
                    
                    return RedirectToAction("List");
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Imported"));
                
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Tier prices

        [HttpPost]
        public virtual async Task<IActionResult> TierPriceList(TierPriceSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTierPriceListModelAsync(searchModel, tvChannel);

            return Json(model);
        }

        public virtual async Task<IActionResult> TierPriceCreatePopup(int tvChannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTierPriceModelAsync(new TierPriceModel(), tvChannel, null);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> TierPriceCreatePopup(TierPriceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(model.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            if (ModelState.IsValid)
            {
                //fill entity from model
                var tierPrice = model.ToEntity<TierPrice>();
                tierPrice.TvChannelId = tvChannel.Id;
                tierPrice.UserRoleId = model.UserRoleId > 0 ? model.UserRoleId : (int?)null;

                await _tvChannelService.InsertTierPriceAsync(tierPrice);

                //update "HasTierPrices" property
                await _tvChannelService.UpdateHasTierPricesPropertyAsync(tvChannel);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvChannelModelFactory.PrepareTierPriceModelAsync(model, tvChannel, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> TierPriceEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = await _tvChannelService.GetTierPriceByIdAsync(id);
            if (tierPrice == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tierPrice.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTierPriceModelAsync(null, tvChannel, tierPrice);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TierPriceEditPopup(TierPriceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = await _tvChannelService.GetTierPriceByIdAsync(model.Id);
            if (tierPrice == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tierPrice.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            if (ModelState.IsValid)
            {
                //fill entity from model
                tierPrice = model.ToEntity(tierPrice);
                tierPrice.UserRoleId = model.UserRoleId > 0 ? model.UserRoleId : (int?)null;
                await _tvChannelService.UpdateTierPriceAsync(tierPrice);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvChannelModelFactory.PrepareTierPriceModelAsync(model, tvChannel, tierPrice, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TierPriceDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = await _tvChannelService.GetTierPriceByIdAsync(id)
                ?? throw new ArgumentException("No tier price found with the specified id");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tierPrice.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            await _tvChannelService.DeleteTierPriceAsync(tierPrice);

            //update "HasTierPrices" property
            await _tvChannelService.UpdateHasTierPricesPropertyAsync(tvChannel);

            return new NullJsonResult();
        }

        #endregion

        #region TvChannel attributes

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeMappingList(TvChannelAttributeMappingSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelAttributeMappingListModelAsync(searchModel, tvChannel);

            return Json(model);
        }

        public virtual async Task<IActionResult> TvChannelAttributeMappingCreate(int tvChannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("This is not your tvChannel"));
                return RedirectToAction("List");
            }

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelAttributeMappingModelAsync(new TvChannelAttributeMappingModel(), tvChannel, null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> TvChannelAttributeMappingCreate(TvChannelAttributeMappingModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(model.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("This is not your tvChannel"));
                return RedirectToAction("List");
            }

            //ensure this attribute is not mapped yet
            if ((await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id))
                .Any(x => x.TvChannelAttributeId == model.TvChannelAttributeId))
            {
                //redisplay form
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.AlreadyExists"));

                model = await _tvChannelModelFactory.PrepareTvChannelAttributeMappingModelAsync(model, tvChannel, null, true);

                return View(model);
            }

            //insert mapping
            var tvChannelAttributeMapping = model.ToEntity<TvChannelAttributeMapping>();

            await _tvChannelAttributeService.InsertTvChannelAttributeMappingAsync(tvChannelAttributeMapping);
            await UpdateLocalesAsync(tvChannelAttributeMapping, model);

            //predefined values
            var predefinedValues = await _tvChannelAttributeService.GetPredefinedTvChannelAttributeValuesAsync(model.TvChannelAttributeId);
            foreach (var predefinedValue in predefinedValues)
            {
                var pav = new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = tvChannelAttributeMapping.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = predefinedValue.Name,
                    PriceAdjustment = predefinedValue.PriceAdjustment,
                    PriceAdjustmentUsePercentage = predefinedValue.PriceAdjustmentUsePercentage,
                    WeightAdjustment = predefinedValue.WeightAdjustment,
                    Cost = predefinedValue.Cost,
                    IsPreSelected = predefinedValue.IsPreSelected,
                    DisplayOrder = predefinedValue.DisplayOrder
                };
                await _tvChannelAttributeService.InsertTvChannelAttributeValueAsync(pav);

                //locales
                var languages = await _languageService.GetAllLanguagesAsync(true);

                //localization
                foreach (var lang in languages)
                {
                    var name = await _localizationService.GetLocalizedAsync(predefinedValue, x => x.Name, lang.Id, false, false);
                    if (!string.IsNullOrEmpty(name))
                        await _localizedEntityService.SaveLocalizedValueAsync(pav, x => x.Name, name, lang.Id);
                }
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Added"));

            if (!continueEditing)
            {
                //select an appropriate card
                SaveSelectedCardName("tvchannel-tvchannel-attributes");
                return RedirectToAction("Edit", new { id = tvChannel.Id });
            }

            return RedirectToAction("TvChannelAttributeMappingEdit", new { id = tvChannelAttributeMapping.Id });
        }

        public virtual async Task<IActionResult> TvChannelAttributeMappingEdit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel attribute mapping with the specified id
            var tvChannelAttributeMapping = await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(id)
                ?? throw new ArgumentException("No tvChannel attribute mapping found with the specified id");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("This is not your tvChannel"));
                return RedirectToAction("List");
            }

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelAttributeMappingModelAsync(null, tvChannel, tvChannelAttributeMapping);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> TvChannelAttributeMappingEdit(TvChannelAttributeMappingModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel attribute mapping with the specified id
            var tvChannelAttributeMapping = await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(model.Id)
                ?? throw new ArgumentException("No tvChannel attribute mapping found with the specified id");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("This is not your tvChannel"));
                return RedirectToAction("List");
            }

            //ensure this attribute is not mapped yet
            if ((await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id))
                .Any(x => x.TvChannelAttributeId == model.TvChannelAttributeId && x.Id != tvChannelAttributeMapping.Id))
            {
                //redisplay form
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.AlreadyExists"));

                model = await _tvChannelModelFactory.PrepareTvChannelAttributeMappingModelAsync(model, tvChannel, tvChannelAttributeMapping, true);

                return View(model);
            }

            //fill entity from model
            tvChannelAttributeMapping = model.ToEntity(tvChannelAttributeMapping);
            await _tvChannelAttributeService.UpdateTvChannelAttributeMappingAsync(tvChannelAttributeMapping);

            await UpdateLocalesAsync(tvChannelAttributeMapping, model);

            await SaveConditionAttributesAsync(tvChannelAttributeMapping, model.ConditionModel, form);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Updated"));

            if (!continueEditing)
            {
                //select an appropriate card
                SaveSelectedCardName("tvchannel-tvchannel-attributes");
                return RedirectToAction("Edit", new { id = tvChannel.Id });
            }

            return RedirectToAction("TvChannelAttributeMappingEdit", new { id = tvChannelAttributeMapping.Id });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeMappingDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel attribute mapping with the specified id
            var tvChannelAttributeMapping = await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(id)
                ?? throw new ArgumentException("No tvChannel attribute mapping found with the specified id");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            //check if existed combinations contains the specified attribute
            var existedCombinations = await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvChannel.Id);
            if (existedCombinations?.Any() == true)
            {
                foreach (var combination in existedCombinations)
                {
                    var mappings = await _tvChannelAttributeParser
                        .ParseTvChannelAttributeMappingsAsync(combination.AttributesXml);
                    
                    if (mappings?.Any(m => m.Id == tvChannelAttributeMapping.Id) == true)
                    {
                        _notificationService.ErrorNotification(
                            string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.AlreadyExistsInCombination"),
                                await _tvChannelAttributeFormatter.FormatAttributesAsync(tvChannel, combination.AttributesXml, await _workContext.GetCurrentUserAsync(), await _storeContext.GetCurrentStoreAsync(), ", ")));

                        return RedirectToAction("TvChannelAttributeMappingEdit", new { id = tvChannelAttributeMapping.Id });
                    }
                }
            }

            await _tvChannelAttributeService.DeleteTvChannelAttributeMappingAsync(tvChannelAttributeMapping);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Deleted"));

            //select an appropriate card
            SaveSelectedCardName("tvchannel-tvchannel-attributes");
            return RedirectToAction("Edit", new { id = tvChannelAttributeMapping.TvChannelId });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeValueList(TvChannelAttributeValueSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvChannel attribute mapping with the specified id
            var tvChannelAttributeMapping = await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(searchModel.TvChannelAttributeMappingId)
                ?? throw new ArgumentException("No tvChannel attribute mapping found with the specified id");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelAttributeValueListModelAsync(searchModel, tvChannelAttributeMapping);

            return Json(model);
        }

        public virtual async Task<IActionResult> TvChannelAttributeValueCreatePopup(int tvChannelAttributeMappingId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel attribute mapping with the specified id
            var tvChannelAttributeMapping = await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(tvChannelAttributeMappingId)
                ?? throw new ArgumentException("No tvChannel attribute mapping found with the specified id");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelAttributeValueModelAsync(new TvChannelAttributeValueModel(), tvChannelAttributeMapping, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeValueCreatePopup(TvChannelAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel attribute mapping with the specified id
            var tvChannelAttributeMapping = await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(model.TvChannelAttributeMappingId);
            if (tvChannelAttributeMapping == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            if (tvChannelAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (string.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError(string.Empty, "Color is required");
                try
                {
                    //ensure color is valid (can be instantiated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                }
            }

            //ensure a picture is uploaded
            if (tvChannelAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares && model.ImageSquaresPictureId == 0)
            {
                ModelState.AddModelError(string.Empty, "Image is required");
            }

            if (ModelState.IsValid)
            {
                //fill entity from model
                var pav = model.ToEntity<TvChannelAttributeValue>();

                pav.Quantity = model.UserEntersQty ? 1 : model.Quantity;

                await _tvChannelAttributeService.InsertTvChannelAttributeValueAsync(pav);
                await UpdateLocalesAsync(pav, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvChannelModelFactory.PrepareTvChannelAttributeValueModelAsync(model, tvChannelAttributeMapping, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> TvChannelAttributeValueEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel attribute value with the specified id
            var tvChannelAttributeValue = await _tvChannelAttributeService.GetTvChannelAttributeValueByIdAsync(id);
            if (tvChannelAttributeValue == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvChannel attribute mapping with the specified id
            var tvChannelAttributeMapping = await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(tvChannelAttributeValue.TvChannelAttributeMappingId);
            if (tvChannelAttributeMapping == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelAttributeValueModelAsync(null, tvChannelAttributeMapping, tvChannelAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeValueEditPopup(TvChannelAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel attribute value with the specified id
            var tvChannelAttributeValue = await _tvChannelAttributeService.GetTvChannelAttributeValueByIdAsync(model.Id);
            if (tvChannelAttributeValue == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvChannel attribute mapping with the specified id
            var tvChannelAttributeMapping = await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(tvChannelAttributeValue.TvChannelAttributeMappingId);
            if (tvChannelAttributeMapping == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            if (tvChannelAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (string.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError(string.Empty, "Color is required");
                try
                {
                    //ensure color is valid (can be instantiated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                }
            }

            //ensure a picture is uploaded
            if (tvChannelAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares && model.ImageSquaresPictureId == 0)
            {
                ModelState.AddModelError(string.Empty, "Image is required");
            }

            if (ModelState.IsValid)
            {
                //fill entity from model
                tvChannelAttributeValue = model.ToEntity(tvChannelAttributeValue);
                tvChannelAttributeValue.Quantity = model.UserEntersQty ? 1 : model.Quantity;
                await _tvChannelAttributeService.UpdateTvChannelAttributeValueAsync(tvChannelAttributeValue);

                await UpdateLocalesAsync(tvChannelAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvChannelModelFactory.PrepareTvChannelAttributeValueModelAsync(model, tvChannelAttributeMapping, tvChannelAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeValueDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel attribute value with the specified id
            var tvChannelAttributeValue = await _tvChannelAttributeService.GetTvChannelAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No tvChannel attribute value found with the specified id");

            //try to get a tvChannel attribute mapping with the specified id
            var tvChannelAttributeMapping = await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(tvChannelAttributeValue.TvChannelAttributeMappingId)
                ?? throw new ArgumentException("No tvChannel attribute mapping found with the specified id");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            //check if existed combinations contains the specified attribute value
            var existedCombinations = await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvChannel.Id);
            if (existedCombinations?.Any() == true)
            {
                foreach (var combination in existedCombinations)
                {
                    var attributeValues = await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(combination.AttributesXml);
                    
                    if (attributeValues.Where(attribute => attribute.Id == id).Any())
                    {
                        return Conflict(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.AlreadyExistsInCombination"),
                            await _tvChannelAttributeFormatter.FormatAttributesAsync(tvChannel, combination.AttributesXml, await _workContext.GetCurrentUserAsync(), await _storeContext.GetCurrentStoreAsync(), ", ")));
                    }
                }
            }

            await _tvChannelAttributeService.DeleteTvChannelAttributeValueAsync(tvChannelAttributeValue);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> AssociateTvChannelToAttributeValuePopup()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //prepare model
            var model = await _tvChannelModelFactory.PrepareAssociateTvChannelToAttributeValueSearchModelAsync(new AssociateTvChannelToAttributeValueSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociateTvChannelToAttributeValuePopupList(AssociateTvChannelToAttributeValueSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvChannelModelFactory.PrepareAssociateTvChannelToAttributeValueListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> AssociateTvChannelToAttributeValuePopup([Bind(Prefix = nameof(AssociateTvChannelToAttributeValueModel))] AssociateTvChannelToAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel with the specified id
            var associatedTvChannel = await _tvChannelService.GetTvChannelByIdAsync(model.AssociatedToTvChannelId);
            if (associatedTvChannel == null)
                return Content("Cannot load a tvChannel");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && associatedTvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            ViewBag.RefreshPage = true;
            ViewBag.tvChannelId = associatedTvChannel.Id;
            ViewBag.tvChannelName = associatedTvChannel.Name;

            return View(new AssociateTvChannelToAttributeValueSearchModel());
        }

        //action displaying notification (warning) to a store owner when associating some tvChannel
        public virtual async Task<IActionResult> AssociatedTvChannelGetWarnings(int tvChannelId)
        {
            var associatedTvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
            if (associatedTvChannel == null)
                return Json(new { Result = string.Empty });

            //attributes
            if (await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(associatedTvChannel.Id) is IList<TvChannelAttributeMapping> mapping && mapping.Any())
            {
                if (mapping.Any(attribute => attribute.IsRequired))
                    return Json(new { Result = await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.AssociatedTvChannel.HasRequiredAttributes") });

                return Json(new { Result = await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.AssociatedTvChannel.HasAttributes") });
            }

            //gift card
            if (associatedTvChannel.IsGiftCard)
            {
                return Json(new { Result = await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.AssociatedTvChannel.GiftCard") });
            }

            //downloadable tvChannel
            if (associatedTvChannel.IsDownload)
            {
                return Json(new { Result = await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.Fields.AssociatedTvChannel.Downloadable") });
            }

            return Json(new { Result = string.Empty });
        }

        #endregion

        #region TvChannel attribute combinations

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeCombinationList(TvChannelAttributeCombinationSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelAttributeCombinationListModelAsync(searchModel, tvChannel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeCombinationDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = await _tvChannelAttributeService.GetTvChannelAttributeCombinationByIdAsync(id)
                ?? throw new ArgumentException("No tvChannel attribute combination found with the specified id");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(combination.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            await _tvChannelAttributeService.DeleteTvChannelAttributeCombinationAsync(combination);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> TvChannelAttributeCombinationCreatePopup(int tvChannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
            if (tvChannel == null)
                return RedirectToAction("List", "TvChannel");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelAttributeCombinationModelAsync(new TvChannelAttributeCombinationModel(), tvChannel, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeCombinationCreatePopup(int tvChannelId, TvChannelAttributeCombinationModel model, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
            if (tvChannel == null)
                return RedirectToAction("List", "TvChannel");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //attributes
            var warnings = new List<string>();
            var attributesXml = await GetAttributesXmlForTvChannelAttributeCombinationAsync(form, warnings, tvChannel.Id);

            //check whether the attribute value is specified
            if (string.IsNullOrEmpty(attributesXml))
                warnings.Add(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Alert.FailedValue"));

            warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentUserAsync(),
                ShoppingCartType.ShoppingCart, tvChannel, 1, attributesXml, true));

            //check whether the same attribute combination already exists
            var existingCombination = await _tvChannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvChannel, attributesXml);
            if (existingCombination != null)
                warnings.Add(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.AlreadyExists"));

            if (!warnings.Any())
            {
                //save combination
                var combination = model.ToEntity<TvChannelAttributeCombination>();

                //fill attributes
                combination.AttributesXml = attributesXml;

                await _tvChannelAttributeService.InsertTvChannelAttributeCombinationAsync(combination);

                //quantity change history
                await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannel, combination.StockQuantity, combination.StockQuantity,
                    message: await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvChannelModelFactory.PrepareTvChannelAttributeCombinationModelAsync(model, tvChannel, null, true);
            model.Warnings = warnings;

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> TvChannelAttributeCombinationGeneratePopup(int tvChannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
            if (tvChannel == null)
                return RedirectToAction("List", "TvChannel");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelAttributeCombinationModelAsync(new TvChannelAttributeCombinationModel(), tvChannel, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeCombinationGeneratePopup(IFormCollection form, TvChannelAttributeCombinationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(model.TvChannelId);
            if (tvChannel == null)
                return RedirectToAction("List", "TvChannel");

            var allowedAttributeIds = form.Keys.Where(key => key.Contains("attribute_value_"))
                .Select(key => int.TryParse(form[key], out var id) ? id : 0).Where(id => id > 0).ToList();

            var requiredAttributeNames = await (await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id))
                .Where(pam => pam.IsRequired)
                .Where(pam => !pam.IsNonCombinable())
                .WhereAwait(async pam => !(await _tvChannelAttributeService.GetTvChannelAttributeValuesAsync(pam.Id)).Any(v => allowedAttributeIds.Any(id => id == v.Id)))
                .SelectAwait(async pam => (await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(pam.TvChannelAttributeId)).Name).ToListAsync();

            if (requiredAttributeNames.Any())
            {
                model = await _tvChannelModelFactory.PrepareTvChannelAttributeCombinationModelAsync(model, tvChannel, null, true);
                var pavModels = model.TvChannelAttributes.SelectMany(pa => pa.Values)
                    .Where(v => allowedAttributeIds.Any(id => id == v.Id))
                    .ToList();
                foreach(var pavModel in pavModels)
                {
                    pavModel.Checked = "checked";
                }
                
                model.Warnings.Add(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.SelectRequiredAttributes"), string.Join(", ", requiredAttributeNames)));

                return View(model);
            }

            await GenerateAttributeCombinationsAsync(tvChannel, allowedAttributeIds);

            ViewBag.RefreshPage = true;

            return View(new TvChannelAttributeCombinationModel());
        }

        public virtual async Task<IActionResult> TvChannelAttributeCombinationEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = await _tvChannelAttributeService.GetTvChannelAttributeCombinationByIdAsync(id);
            if (combination == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(combination.TvChannelId);
            if (tvChannel == null)
                return RedirectToAction("List", "TvChannel");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareTvChannelAttributeCombinationModelAsync(null, tvChannel, combination);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeCombinationEditPopup(TvChannelAttributeCombinationModel model, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = await _tvChannelAttributeService.GetTvChannelAttributeCombinationByIdAsync(model.Id);
            if (combination == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(combination.TvChannelId);
            if (tvChannel == null)
                return RedirectToAction("List", "TvChannel");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //attributes
            var warnings = new List<string>();
            var attributesXml = await GetAttributesXmlForTvChannelAttributeCombinationAsync(form, warnings, tvChannel.Id);

            //check whether the attribute value is specified
            if (string.IsNullOrEmpty(attributesXml))
                warnings.Add(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Alert.FailedValue"));

            warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentUserAsync(),
                ShoppingCartType.ShoppingCart, tvChannel, 1, attributesXml, true));

            //check whether the same attribute combination already exists
            var existingCombination = await _tvChannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvChannel, attributesXml);
            if (existingCombination != null && existingCombination.Id != model.Id && existingCombination.AttributesXml.Equals(attributesXml))
                warnings.Add(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.AlreadyExists"));

            if (!warnings.Any() && ModelState.IsValid)
            {
                var previousStockQuantity = combination.StockQuantity;

                //save combination
                //fill entity from model
                combination = model.ToEntity(combination);
                combination.AttributesXml = attributesXml;

                await _tvChannelAttributeService.UpdateTvChannelAttributeCombinationAsync(combination);

                //quantity change history
                await _tvChannelService.AddStockQuantityHistoryEntryAsync(tvChannel, combination.StockQuantity - previousStockQuantity, combination.StockQuantity,
                    message: await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvChannelModelFactory.PrepareTvChannelAttributeCombinationModelAsync(model, tvChannel, combination, true);
            model.Warnings = warnings;

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GenerateAllAttributeCombinations(int tvChannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvChannel with the specified id
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            await GenerateAttributeCombinationsAsync(tvChannel);

            return Json(new { Success = true });
        }

        #endregion

        #region TvChannel editor settings

        [HttpPost]
        public virtual async Task<IActionResult> SaveTvChannelEditorSettings(TvChannelModel model, string returnUrl = "")
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //vendors cannot manage these settings
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            var tvChannelEditorSettings = await _settingService.LoadSettingAsync<TvChannelEditorSettings>();
            tvChannelEditorSettings = model.TvChannelEditorSettingsModel.ToSettings(tvChannelEditorSettings);
            await _settingService.SaveSettingAsync(tvChannelEditorSettings);

            //tvChannel list
            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("List");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("List");

            return Redirect(returnUrl);
        }

        #endregion

        #region Stock quantity history

        [HttpPost]
        public virtual async Task<IActionResult> StockQuantityHistory(StockQuantityHistorySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvChannel found with the specified id");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvChannel");

            //prepare model
            var model = await _tvChannelModelFactory.PrepareStockQuantityHistoryListModelAsync(searchModel, tvChannel);

            return Json(model);
        }

        #endregion

        #endregion
    }
}