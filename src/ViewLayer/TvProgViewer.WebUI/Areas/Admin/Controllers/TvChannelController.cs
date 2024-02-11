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
        private readonly ITvChannelAttributeFormatter _tvchannelAttributeFormatter;
        private readonly ITvChannelAttributeParser _tvchannelAttributeParser;
        private readonly ITvChannelAttributeService _tvchannelAttributeService;
        private readonly ITvChannelModelFactory _tvchannelModelFactory;
        private readonly ITvChannelService _tvchannelService;
        private readonly ITvChannelTagService _tvchannelTagService;
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
            ITvChannelAttributeFormatter tvchannelAttributeFormatter,
            ITvChannelAttributeParser tvchannelAttributeParser,
            ITvChannelAttributeService tvchannelAttributeService,
            ITvChannelModelFactory tvchannelModelFactory,
            ITvChannelService tvchannelService,
            ITvChannelTagService tvchannelTagService,
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
            _tvchannelAttributeFormatter = tvchannelAttributeFormatter;
            _tvchannelAttributeParser = tvchannelAttributeParser;
            _tvchannelAttributeService = tvchannelAttributeService;
            _tvchannelModelFactory = tvchannelModelFactory;
            _tvchannelService = tvchannelService;
            _tvchannelTagService = tvchannelTagService;
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

        protected virtual async Task UpdateLocalesAsync(TvChannel tvchannel, TvChannelModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(tvchannel,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(tvchannel,
                    x => x.ShortDescription,
                    localized.ShortDescription,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(tvchannel,
                    x => x.FullDescription,
                    localized.FullDescription,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(tvchannel,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(tvchannel,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValueAsync(tvchannel,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(tvchannel, localized.SeName, localized.Name, false);
                await _urlRecordService.SaveSlugAsync(tvchannel, seName, localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(TvChannelTag tvchannelTag, TvChannelTagModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(tvchannelTag,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                var seName = await _urlRecordService.ValidateSeNameAsync(tvchannelTag, string.Empty, localized.Name, false);
                await _urlRecordService.SaveSlugAsync(tvchannelTag, seName, localized.LanguageId);
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

        protected virtual async Task UpdatePictureSeoNamesAsync(TvChannel tvchannel)
        {
            foreach (var pp in await _tvchannelService.GetTvChannelPicturesByTvChannelIdAsync(tvchannel.Id))
                await _pictureService.SetSeoFilenameAsync(pp.PictureId, await _pictureService.GetPictureSeNameAsync(tvchannel.Name));
        }

        protected virtual async Task SaveTvChannelAclAsync(TvChannel tvchannel, TvChannelModel model)
        {
            tvchannel.SubjectToAcl = model.SelectedUserRoleIds.Any();
            await _tvchannelService.UpdateTvChannelAsync(tvchannel);

            var existingAclRecords = await _aclService.GetAclRecordsAsync(tvchannel);
            var allUserRoles = await _userService.GetAllUserRolesAsync(true);
            foreach (var userRole in allUserRoles)
            {
                if (model.SelectedUserRoleIds.Contains(userRole.Id))
                {
                    //new role
                    if (!existingAclRecords.Any(acl => acl.UserRoleId == userRole.Id))
                        await _aclService.InsertAclRecordAsync(tvchannel, userRole.Id);
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

        protected virtual async Task SaveCategoryMappingsAsync(TvChannel tvchannel, TvChannelModel model)
        {
            var existingTvChannelCategories = await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(tvchannel.Id, true);

            //delete categories
            foreach (var existingTvChannelCategory in existingTvChannelCategories)
                if (!model.SelectedCategoryIds.Contains(existingTvChannelCategory.CategoryId))
                    await _categoryService.DeleteTvChannelCategoryAsync(existingTvChannelCategory);

            //add categories
            foreach (var categoryId in model.SelectedCategoryIds)
            {
                if (_categoryService.FindTvChannelCategory(existingTvChannelCategories, tvchannel.Id, categoryId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingCategoryMapping = await _categoryService.GetTvChannelCategoriesByCategoryIdAsync(categoryId, showHidden: true);
                    if (existingCategoryMapping.Any())
                        displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;
                    await _categoryService.InsertTvChannelCategoryAsync(new TvChannelCategory
                    {
                        TvChannelId = tvchannel.Id,
                        CategoryId = categoryId,
                        DisplayOrder = displayOrder
                    });
                }
            }
        }

        protected virtual async Task SaveManufacturerMappingsAsync(TvChannel tvchannel, TvChannelModel model)
        {
            var existingTvChannelManufacturers = await _manufacturerService.GetTvChannelManufacturersByTvChannelIdAsync(tvchannel.Id, true);

            //delete manufacturers
            foreach (var existingTvChannelManufacturer in existingTvChannelManufacturers)
                if (!model.SelectedManufacturerIds.Contains(existingTvChannelManufacturer.ManufacturerId))
                    await _manufacturerService.DeleteTvChannelManufacturerAsync(existingTvChannelManufacturer);

            //add manufacturers
            foreach (var manufacturerId in model.SelectedManufacturerIds)
            {
                if (_manufacturerService.FindTvChannelManufacturer(existingTvChannelManufacturers, tvchannel.Id, manufacturerId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingManufacturerMapping = await _manufacturerService.GetTvChannelManufacturersByManufacturerIdAsync(manufacturerId, showHidden: true);
                    if (existingManufacturerMapping.Any())
                        displayOrder = existingManufacturerMapping.Max(x => x.DisplayOrder) + 1;
                    await _manufacturerService.InsertTvChannelManufacturerAsync(new TvChannelManufacturer
                    {
                        TvChannelId = tvchannel.Id,
                        ManufacturerId = manufacturerId,
                        DisplayOrder = displayOrder
                    });
                }
            }
        }

        protected virtual async Task SaveDiscountMappingsAsync(TvChannel tvchannel, TvChannelModel model)
        {
            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToSkus, showHidden: true, isActive: null);

            foreach (var discount in allDiscounts)
            {
                if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (await _tvchannelService.GetDiscountAppliedToTvChannelAsync(tvchannel.Id, discount.Id) is null)
                        await _tvchannelService.InsertDiscountTvChannelMappingAsync(new DiscountTvChannelMapping { EntityId = tvchannel.Id, DiscountId = discount.Id });
                }
                else
                {
                    //remove discount
                    if (await _tvchannelService.GetDiscountAppliedToTvChannelAsync(tvchannel.Id, discount.Id) is DiscountTvChannelMapping discountTvChannelMapping)
                        await _tvchannelService.DeleteDiscountTvChannelMappingAsync(discountTvChannelMapping);
                }
            }

            await _tvchannelService.UpdateTvChannelAsync(tvchannel);
            await _tvchannelService.UpdateHasDiscountsAppliedAsync(tvchannel);
        }

        protected virtual async Task<string> GetAttributesXmlForTvChannelAttributeCombinationAsync(IFormCollection form, List<string> warnings, int tvchannelId)
        {
            var attributesXml = string.Empty;

            //get tvchannel attribute mappings (exclude non-combinable attributes)
            var attributes = (await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannelId))
                .Where(tvchannelAttributeMapping => !tvchannelAttributeMapping.IsNonCombinable()).ToList();

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
                                attributesXml = _tvchannelAttributeParser.AddTvChannelAttribute(attributesXml,
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
                                    attributesXml = _tvchannelAttributeParser.AddTvChannelAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        //load read-only (already server-side selected) values
                        var attributeValues = await _tvchannelAttributeService.GetTvChannelAttributeValuesAsync(attribute.Id);
                        foreach (var selectedAttributeId in attributeValues
                            .Where(v => v.IsPreSelected)
                            .Select(v => v.Id)
                            .ToList())
                        {
                            attributesXml = _tvchannelAttributeParser.AddTvChannelAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                        }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        ctrlAttributes = form[controlId];
                        if (!string.IsNullOrEmpty(ctrlAttributes))
                        {
                            var enteredText = ctrlAttributes.ToString().Trim();
                            attributesXml = _tvchannelAttributeParser.AddTvChannelAttribute(attributesXml,
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
                            attributesXml = _tvchannelAttributeParser.AddTvChannelAttribute(attributesXml,
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
                                attributesXml = _tvchannelAttributeParser.AddTvChannelAttribute(attributesXml,
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
                var conditionMet = await _tvchannelAttributeParser.IsConditionMetAsync(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _tvchannelAttributeParser.RemoveTvChannelAttribute(attributesXml, attribute);
                }
            }

            return attributesXml;
        }

        protected virtual string[] ParseTvChannelTags(string tvchannelTags)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(tvchannelTags))
                return result.ToArray();

            var values = tvchannelTags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var val in values)
                if (!string.IsNullOrEmpty(val.Trim()))
                    result.Add(val.Trim());

            return result.ToArray();
        }

        protected virtual async Task SaveTvChannelWarehouseInventoryAsync(TvChannel tvchannel, TvChannelModel model)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

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

                var existingPwI = (await _tvchannelService.GetAllTvChannelWarehouseInventoryRecordsAsync(tvchannel.Id)).FirstOrDefault(x => x.WarehouseId == warehouse.Id);
                if (existingPwI != null)
                {
                    if (used)
                    {
                        var previousStockQuantity = existingPwI.StockQuantity;

                        //update existing record
                        existingPwI.StockQuantity = stockQuantity;
                        existingPwI.ReservedQuantity = reservedQuantity;
                        await _tvchannelService.UpdateTvChannelWarehouseInventoryAsync(existingPwI);

                        //quantity change history
                        await _tvchannelService.AddStockQuantityHistoryEntryAsync(tvchannel, existingPwI.StockQuantity - previousStockQuantity, existingPwI.StockQuantity,
                            existingPwI.WarehouseId, message);
                    }
                    else
                    {
                        //delete. no need to store record for qty 0
                        await _tvchannelService.DeleteTvChannelWarehouseInventoryAsync(existingPwI);

                        //quantity change history
                        await _tvchannelService.AddStockQuantityHistoryEntryAsync(tvchannel, -existingPwI.StockQuantity, 0, existingPwI.WarehouseId, message);
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
                        TvChannelId = tvchannel.Id,
                        StockQuantity = stockQuantity,
                        ReservedQuantity = reservedQuantity
                    };

                    await _tvchannelService.InsertTvChannelWarehouseInventoryAsync(existingPwI);

                    //quantity change history
                    await _tvchannelService.AddStockQuantityHistoryEntryAsync(tvchannel, existingPwI.StockQuantity, existingPwI.StockQuantity,
                        existingPwI.WarehouseId, message);
                }
            }
        }

        protected virtual async Task SaveConditionAttributesAsync(TvChannelAttributeMapping tvchannelAttributeMapping,
            TvChannelAttributeConditionModel model, IFormCollection form)
        {
            string attributesXml = null;
            if (model.EnableCondition)
            {
                var attribute = await _tvchannelAttributeService.GetTvChannelAttributeMappingByIdAsync(model.SelectedTvChannelAttributeId);
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
                                attributesXml = _tvchannelAttributeParser.AddTvChannelAttribute(null, attribute,
                                    selectedAttributeId > 0 ? selectedAttributeId.ToString() : string.Empty);
                            }
                            else
                            {
                                //for conditions we should empty values save even when nothing is selected
                                //otherwise "attributesXml" will be empty
                                //hence we won't be able to find a selected attribute
                                attributesXml = _tvchannelAttributeParser.AddTvChannelAttribute(null,
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

                                    attributesXml = _tvchannelAttributeParser.AddTvChannelAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                                    anyValueSelected = true;
                                }

                                if (!anyValueSelected)
                                {
                                    //for conditions we should save empty values even when nothing is selected
                                    //otherwise "attributesXml" will be empty
                                    //hence we won't be able to find a selected attribute
                                    attributesXml = _tvchannelAttributeParser.AddTvChannelAttribute(null,
                                        attribute, string.Empty);
                                }
                            }
                            else
                            {
                                //for conditions we should save empty values even when nothing is selected
                                //otherwise "attributesXml" will be empty
                                //hence we won't be able to find a selected attribute
                                attributesXml = _tvchannelAttributeParser.AddTvChannelAttribute(null,
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

            tvchannelAttributeMapping.ConditionAttributeXml = attributesXml;
            await _tvchannelAttributeService.UpdateTvChannelAttributeMappingAsync(tvchannelAttributeMapping);
        }

        protected virtual async Task GenerateAttributeCombinationsAsync(TvChannel tvchannel, IList<int> allowedAttributeIds = null)
        {
            var allAttributesXml = await _tvchannelAttributeParser.GenerateAllCombinationsAsync(tvchannel, true, allowedAttributeIds);
            foreach (var attributesXml in allAttributesXml)
            {
                var existingCombination = await _tvchannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvchannel, attributesXml);

                //already exists?
                if (existingCombination != null)
                    continue;

                //new one
                var warnings = new List<string>();
                warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentUserAsync(),
                    ShoppingCartType.ShoppingCart, tvchannel, 1, attributesXml, true, true, true));
                if (warnings.Count != 0)
                    continue;

                //save combination
                var combination = new TvChannelAttributeCombination
                {
                    TvChannelId = tvchannel.Id,
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
                await _tvchannelAttributeService.InsertTvChannelAttributeCombinationAsync(combination);
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
            var model = await _tvchannelModelFactory.PrepareTvChannelSearchModelAsync(new TvChannelSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelList(TvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-tvchannel-by-sku")]
        public virtual async Task<IActionResult> GoToSku(TvChannelSearchModel searchModel)
        {
            //try to load a tvchannel entity, if not found, then try to load a tvchannel attribute combination
            var tvchannelId = (await _tvchannelService.GetTvChannelBySkuAsync(searchModel.GoDirectlyToSku))?.Id
                ?? (await _tvchannelAttributeService.GetTvChannelAttributeCombinationBySkuAsync(searchModel.GoDirectlyToSku))?.TvChannelId;

            if (tvchannelId != null)
                return RedirectToAction("Edit", "TvChannel", new { id = tvchannelId });

            //not found
            return await List();
        }

        public virtual async Task<IActionResult> Create(bool showtour = false)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //validate maximum number of tvchannels per vendor
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (_vendorSettings.MaximumTvChannelNumber > 0 && currentVendor != null
                && await _tvchannelService.GetNumberOfTvChannelsByVendorIdAsync(currentVendor.Id) >= _vendorSettings.MaximumTvChannelNumber)
            {
                _notificationService.ErrorNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.ExceededMaximumNumber"),
                    _vendorSettings.MaximumTvChannelNumber));
                return RedirectToAction("List");
            }

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelModelAsync(new TvChannelModel(), null);

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

            //validate maximum number of tvchannels per vendor
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (_vendorSettings.MaximumTvChannelNumber > 0 && currentVendor != null
                && await _tvchannelService.GetNumberOfTvChannelsByVendorIdAsync(currentVendor.Id) >= _vendorSettings.MaximumTvChannelNumber)
            {
                _notificationService.ErrorNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.ExceededMaximumNumber"),
                    _vendorSettings.MaximumTvChannelNumber));
                return RedirectToAction("List");
            }

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his tvchannels
                if (currentVendor != null)
                    model.VendorId = currentVendor.Id;

                //vendors cannot edit "Show on home page" property
                if (currentVendor != null && model.ShowOnHomepage)
                    model.ShowOnHomepage = false;

                //tvchannel
                var tvchannel = model.ToEntity<TvChannel>();
                tvchannel.CreatedOnUtc = DateTime.UtcNow;
                tvchannel.UpdatedOnUtc = DateTime.UtcNow;
                await _tvchannelService.InsertTvChannelAsync(tvchannel);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(tvchannel, model.SeName, tvchannel.Name, true);
                await _urlRecordService.SaveSlugAsync(tvchannel, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(tvchannel, model);

                //categories
                await SaveCategoryMappingsAsync(tvchannel, model);

                //manufacturers
                await SaveManufacturerMappingsAsync(tvchannel, model);

                //ACL (user roles)
                await SaveTvChannelAclAsync(tvchannel, model);

                //stores
                await _tvchannelService.UpdateTvChannelStoreMappingsAsync(tvchannel, model.SelectedStoreIds);

                //discounts
                await SaveDiscountMappingsAsync(tvchannel, model);

                //tags
                await _tvchannelTagService.UpdateTvChannelTagsAsync(tvchannel, ParseTvChannelTags(model.TvChannelTags));

                //warehouses
                await SaveTvChannelWarehouseInventoryAsync(tvchannel, model);

                //quantity change history
                await _tvchannelService.AddStockQuantityHistoryEntryAsync(tvchannel, tvchannel.StockQuantity, tvchannel.StockQuantity, tvchannel.WarehouseId,
                    await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Edit"));

                //activity log
                await _userActivityService.InsertActivityAsync("AddNewTvChannel",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewTvChannel"), tvchannel.Name), tvchannel);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = tvchannel.Id });
            }

            //prepare model
            model = await _tvchannelModelFactory.PrepareTvChannelModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(id);
            if (tvchannel == null || tvchannel.Deleted)
                return RedirectToAction("List");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelModelAsync(null, tvchannel);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(TvChannelModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(model.Id);
            if (tvchannel == null || tvchannel.Deleted)
                return RedirectToAction("List");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List");

            //check if the tvchannel quantity has been changed while we were editing the tvchannel
            //and if it has been changed then we show error notification
            //and redirect on the editing page without data saving
            if (tvchannel.StockQuantity != model.LastStockQuantity)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Fields.StockQuantity.ChangedWarning"));
                return RedirectToAction("Edit", new { id = tvchannel.Id });
            }

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his tvchannels
                if (currentVendor != null)
                    model.VendorId = currentVendor.Id;

                //we do not validate maximum number of tvchannels per vendor when editing existing tvchannels (only during creation of new tvchannels)
                //vendors cannot edit "Show on home page" property
                if (currentVendor != null && model.ShowOnHomepage != tvchannel.ShowOnHomepage)
                    model.ShowOnHomepage = tvchannel.ShowOnHomepage;

                //some previously used values
                var prevTotalStockQuantity = await _tvchannelService.GetTotalStockQuantityAsync(tvchannel);
                var prevDownloadId = tvchannel.DownloadId;
                var prevSampleDownloadId = tvchannel.SampleDownloadId;
                var previousStockQuantity = tvchannel.StockQuantity;
                var previousWarehouseId = tvchannel.WarehouseId;
                var previousTvChannelType = tvchannel.TvChannelType;

                //tvchannel
                tvchannel = model.ToEntity(tvchannel);

                tvchannel.UpdatedOnUtc = DateTime.UtcNow;
                await _tvchannelService.UpdateTvChannelAsync(tvchannel);

                //remove associated tvchannels
                if (previousTvChannelType == TvChannelType.GroupedTvChannel && tvchannel.TvChannelType == TvChannelType.SimpleTvChannel)
                {
                    var store = await _storeContext.GetCurrentStoreAsync();
                    var storeId = store?.Id ?? 0;
                    var vendorId = currentVendor?.Id ?? 0;

                    var associatedTvChannels = await _tvchannelService.GetAssociatedTvChannelsAsync(tvchannel.Id, storeId, vendorId);
                    foreach (var associatedTvChannel in associatedTvChannels)
                    {
                        associatedTvChannel.ParentGroupedTvChannelId = 0;
                        await _tvchannelService.UpdateTvChannelAsync(associatedTvChannel);
                    }
                }

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeNameAsync(tvchannel, model.SeName, tvchannel.Name, true);
                await _urlRecordService.SaveSlugAsync(tvchannel, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(tvchannel, model);

                //tags
                await _tvchannelTagService.UpdateTvChannelTagsAsync(tvchannel, ParseTvChannelTags(model.TvChannelTags));

                //warehouses
                await SaveTvChannelWarehouseInventoryAsync(tvchannel, model);

                //categories
                await SaveCategoryMappingsAsync(tvchannel, model);

                //manufacturers
                await SaveManufacturerMappingsAsync(tvchannel, model);

                //ACL (user roles)
                await SaveTvChannelAclAsync(tvchannel, model);

                //stores
                await _tvchannelService.UpdateTvChannelStoreMappingsAsync(tvchannel, model.SelectedStoreIds);

                //discounts
                await SaveDiscountMappingsAsync(tvchannel, model);

                //picture seo names
                await UpdatePictureSeoNamesAsync(tvchannel);

                //back in stock notifications
                if (tvchannel.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                    tvchannel.BackorderMode == BackorderMode.NoBackorders &&
                    tvchannel.AllowBackInStockSubscriptions &&
                    await _tvchannelService.GetTotalStockQuantityAsync(tvchannel) > 0 &&
                    prevTotalStockQuantity <= 0 &&
                    tvchannel.Published &&
                    !tvchannel.Deleted)
                {
                    await _backInStockSubscriptionService.SendNotificationsToSubscribersAsync(tvchannel);
                }

                //delete an old "download" file (if deleted or updated)
                if (prevDownloadId > 0 && prevDownloadId != tvchannel.DownloadId)
                {
                    var prevDownload = await _downloadService.GetDownloadByIdAsync(prevDownloadId);
                    if (prevDownload != null)
                        await _downloadService.DeleteDownloadAsync(prevDownload);
                }

                //delete an old "sample download" file (if deleted or updated)
                if (prevSampleDownloadId > 0 && prevSampleDownloadId != tvchannel.SampleDownloadId)
                {
                    var prevSampleDownload = await _downloadService.GetDownloadByIdAsync(prevSampleDownloadId);
                    if (prevSampleDownload != null)
                        await _downloadService.DeleteDownloadAsync(prevSampleDownload);
                }

                //quantity change history
                if (previousWarehouseId != tvchannel.WarehouseId)
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
                    if (tvchannel.WarehouseId > 0)
                    {
                        var newWarehouse = await _shippingService.GetWarehouseByIdAsync(tvchannel.WarehouseId);
                        if (newWarehouse != null)
                            newWarehouseMessage = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.New"), newWarehouse.Name);
                    }

                    var message = string.Format(await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse"), oldWarehouseMessage, newWarehouseMessage);

                    //record history
                    await _tvchannelService.AddStockQuantityHistoryEntryAsync(tvchannel, -previousStockQuantity, 0, previousWarehouseId, message);
                    await _tvchannelService.AddStockQuantityHistoryEntryAsync(tvchannel, tvchannel.StockQuantity, tvchannel.StockQuantity, tvchannel.WarehouseId, message);
                }
                else
                {
                    await _tvchannelService.AddStockQuantityHistoryEntryAsync(tvchannel, tvchannel.StockQuantity - previousStockQuantity, tvchannel.StockQuantity,
                        tvchannel.WarehouseId, await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Edit"));
                }

                //activity log
                await _userActivityService.InsertActivityAsync("EditTvChannel",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditTvChannel"), tvchannel.Name), tvchannel);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = tvchannel.Id });
            }

            //prepare model
            model = await _tvchannelModelFactory.PrepareTvChannelModelAsync(model, tvchannel, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(id);
            if (tvchannel == null)
                return RedirectToAction("List");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List");

            await _tvchannelService.DeleteTvChannelAsync(tvchannel);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteTvChannel",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteTvChannel"), tvchannel.Name), tvchannel);

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
            await _tvchannelService.DeleteTvChannelsAsync((await _tvchannelService.GetTvChannelsByIdsAsync(selectedIds.ToArray()))
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
                var originalTvChannel = await _tvchannelService.GetTvChannelByIdAsync(copyModel.Id);

                //a vendor should have access only to his tvchannels
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
        public virtual async Task<IActionResult> SkuReservedWarning(int tvchannelId, string sku)
        {
            string message;

            //check whether tvchannel with passed SKU already exists
            var tvchannelBySku = await _tvchannelService.GetTvChannelBySkuAsync(sku);
            if (tvchannelBySku != null)
            {
                if (tvchannelBySku.Id == tvchannelId)
                    return Json(new { Result = string.Empty });

                message = string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Fields.Sku.Reserved"), tvchannelBySku.Name);
                return Json(new { Result = message });
            }

            //check whether combination with passed SKU already exists
            var combinationBySku = await _tvchannelAttributeService.GetTvChannelAttributeCombinationBySkuAsync(sku);
            if (combinationBySku == null)
                return Json(new { Result = string.Empty });

            message = string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Fields.Sku.Reserved"),
                (await _tvchannelService.GetTvChannelByIdAsync(combinationBySku.TvChannelId))?.Name);

            return Json(new { Result = message });
        }

        #endregion

        #region Required tvchannels

        [HttpPost]
        public virtual async Task<IActionResult> LoadTvChannelFriendlyNames(string tvchannelIds)
        {
            var result = string.Empty;

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return Json(new { Text = result });

            if (string.IsNullOrWhiteSpace(tvchannelIds))
                return Json(new { Text = result });

            var ids = new List<int>();
            var rangeArray = tvchannelIds
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            foreach (var str1 in rangeArray)
            {
                if (int.TryParse(str1, out var tmp1))
                    ids.Add(tmp1);
            }

            var tvchannels = await _tvchannelService.GetTvChannelsByIdsAsync(ids.ToArray());
            for (var i = 0; i <= tvchannels.Count - 1; i++)
            {
                result += tvchannels[i].Name;
                if (i != tvchannels.Count - 1)
                    result += ", ";
            }

            return Json(new { Text = result });
        }

        public virtual async Task<IActionResult> RequiredTvChannelAddPopup()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //prepare model
            var model = await _tvchannelModelFactory.PrepareAddRequiredTvChannelSearchModelAsync(new AddRequiredTvChannelSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RequiredTvChannelAddPopupList(AddRequiredTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvchannelModelFactory.PrepareAddRequiredTvChannelListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Related tvchannels

        [HttpPost]
        public virtual async Task<IActionResult> RelatedTvChannelList(RelatedTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareRelatedTvChannelListModelAsync(searchModel, tvchannel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RelatedTvChannelUpdate(RelatedTvChannelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a related tvchannel with the specified id
            var relatedTvChannel = await _tvchannelService.GetRelatedTvChannelByIdAsync(model.Id)
                ?? throw new ArgumentException("No related tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(relatedTvChannel.TvChannelId1);
                if (tvchannel != null && tvchannel.VendorId != currentVendor.Id)
                    return Content("This is not your tvchannel");
            }

            relatedTvChannel.DisplayOrder = model.DisplayOrder;
            await _tvchannelService.UpdateRelatedTvChannelAsync(relatedTvChannel);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> RelatedTvChannelDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a related tvchannel with the specified id
            var relatedTvChannel = await _tvchannelService.GetRelatedTvChannelByIdAsync(id)
                ?? throw new ArgumentException("No related tvchannel found with the specified id");

            var tvchannelId = relatedTvChannel.TvChannelId1;

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
                if (tvchannel != null && tvchannel.VendorId != currentVendor.Id)
                    return Content("This is not your tvchannel");
            }

            await _tvchannelService.DeleteRelatedTvChannelAsync(relatedTvChannel);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> RelatedTvChannelAddPopup(int tvchannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //prepare model
            var model = await _tvchannelModelFactory.PrepareAddRelatedTvChannelSearchModelAsync(new AddRelatedTvChannelSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RelatedTvChannelAddPopupList(AddRelatedTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvchannelModelFactory.PrepareAddRelatedTvChannelListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> RelatedTvChannelAddPopup(AddRelatedTvChannelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            var selectedTvChannels = await _tvchannelService.GetTvChannelsByIdsAsync(model.SelectedTvChannelIds.ToArray());
            if (selectedTvChannels.Any())
            {
                var existingRelatedTvChannels = await _tvchannelService.GetRelatedTvChannelsByTvChannelId1Async(model.TvChannelId, showHidden: true);
                var currentVendor = await _workContext.GetCurrentVendorAsync();
                foreach (var tvchannel in selectedTvChannels)
                {
                    //a vendor should have access only to his tvchannels
                    if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                        continue;

                    if (_tvchannelService.FindRelatedTvChannel(existingRelatedTvChannels, model.TvChannelId, tvchannel.Id) != null)
                        continue;

                    await _tvchannelService.InsertRelatedTvChannelAsync(new RelatedTvChannel
                    {
                        TvChannelId1 = model.TvChannelId,
                        TvChannelId2 = tvchannel.Id,
                        DisplayOrder = 1
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddRelatedTvChannelSearchModel());
        }

        #endregion

        #region Cross-sell tvchannels

        [HttpPost]
        public virtual async Task<IActionResult> CrossSellTvChannelList(CrossSellTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareCrossSellTvChannelListModelAsync(searchModel, tvchannel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CrossSellTvChannelDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a cross-sell tvchannel with the specified id
            var crossSellTvChannel = await _tvchannelService.GetCrossSellTvChannelByIdAsync(id)
                ?? throw new ArgumentException("No cross-sell tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(crossSellTvChannel.TvChannelId1);
                if (tvchannel != null && tvchannel.VendorId != currentVendor.Id)
                    return Content("This is not your tvchannel");
            }

            await _tvchannelService.DeleteCrossSellTvChannelAsync(crossSellTvChannel);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> CrossSellTvChannelAddPopup(int tvchannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //prepare model
            var model = await _tvchannelModelFactory.PrepareAddCrossSellTvChannelSearchModelAsync(new AddCrossSellTvChannelSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CrossSellTvChannelAddPopupList(AddCrossSellTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvchannelModelFactory.PrepareAddCrossSellTvChannelListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> CrossSellTvChannelAddPopup(AddCrossSellTvChannelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            var selectedTvChannels = await _tvchannelService.GetTvChannelsByIdsAsync(model.SelectedTvChannelIds.ToArray());
            if (selectedTvChannels.Any())
            {
                var existingCrossSellTvChannels = await _tvchannelService.GetCrossSellTvChannelsByTvChannelId1Async(model.TvChannelId, showHidden: true);
                var currentVendor = await _workContext.GetCurrentVendorAsync();
                foreach (var tvchannel in selectedTvChannels)
                {
                    //a vendor should have access only to his tvchannels
                    if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                        continue;

                    if (_tvchannelService.FindCrossSellTvChannel(existingCrossSellTvChannels, model.TvChannelId, tvchannel.Id) != null)
                        continue;

                    await _tvchannelService.InsertCrossSellTvChannelAsync(new CrossSellTvChannel
                    {
                        TvChannelId1 = model.TvChannelId,
                        TvChannelId2 = tvchannel.Id
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddCrossSellTvChannelSearchModel());
        }

        #endregion

        #region Associated tvchannels

        [HttpPost]
        public virtual async Task<IActionResult> AssociatedTvChannelList(AssociatedTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareAssociatedTvChannelListModelAsync(searchModel, tvchannel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociatedTvChannelUpdate(AssociatedTvChannelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get an associated tvchannel with the specified id
            var associatedTvChannel = await _tvchannelService.GetTvChannelByIdAsync(model.Id)
                ?? throw new ArgumentException("No associated tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && associatedTvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            associatedTvChannel.DisplayOrder = model.DisplayOrder;
            await _tvchannelService.UpdateTvChannelAsync(associatedTvChannel);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociatedTvChannelDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get an associated tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(id)
                ?? throw new ArgumentException("No associated tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            tvchannel.ParentGroupedTvChannelId = 0;
            await _tvchannelService.UpdateTvChannelAsync(tvchannel);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> AssociatedTvChannelAddPopup(int tvchannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //prepare model
            var model = await _tvchannelModelFactory.PrepareAddAssociatedTvChannelSearchModelAsync(new AddAssociatedTvChannelSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociatedTvChannelAddPopupList(AddAssociatedTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvchannelModelFactory.PrepareAddAssociatedTvChannelListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> AssociatedTvChannelAddPopup(AddAssociatedTvChannelModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            var selectedTvChannels = await _tvchannelService.GetTvChannelsByIdsAsync(model.SelectedTvChannelIds.ToArray());

            var tryToAddSelfGroupedTvChannel = selectedTvChannels
                .Select(p => p.Id)
                .Contains(model.TvChannelId);

            if (selectedTvChannels.Any())
            {
                foreach (var tvchannel in selectedTvChannels)
                {
                    if (tvchannel.Id == model.TvChannelId)
                        continue;

                    //a vendor should have access only to his tvchannels
                    var currentVendor = await _workContext.GetCurrentVendorAsync();
                    if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                        continue;

                    tvchannel.ParentGroupedTvChannelId = model.TvChannelId;
                    await _tvchannelService.UpdateTvChannelAsync(tvchannel);
                }
            }

            if (tryToAddSelfGroupedTvChannel)
            {
                _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.AssociatedTvChannels.TryToAddSelfGroupedTvChannel"));

                var addAssociatedTvChannelSearchModel = await _tvchannelModelFactory.PrepareAddAssociatedTvChannelSearchModelAsync(new AddAssociatedTvChannelSearchModel());
                //set current tvchannel id
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
        public virtual async Task<IActionResult> TvChannelPictureAdd(int tvchannelId, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            if (tvchannelId == 0)
                throw new ArgumentException();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            var files = form.Files.ToList();
            if (!files.Any())
                return Json(new { success = false });

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List");
            try
            {
                foreach (var file in files)
                {
                    //insert picture
                    var picture = await _pictureService.InsertPictureAsync(file);

                    await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(tvchannel.Name));

                    await _tvchannelService.InsertTvChannelPictureAsync(new TvChannelPicture
                    {
                        PictureId = picture.Id,
                        TvChannelId = tvchannel.Id,
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

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelPictureListModelAsync(searchModel, tvchannel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelPictureUpdate(TvChannelPictureModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel picture with the specified id
            var tvchannelPicture = await _tvchannelService.GetTvChannelPictureByIdAsync(model.Id)
                ?? throw new ArgumentException("No tvchannel picture found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelPicture.TvChannelId);
                if (tvchannel != null && tvchannel.VendorId != currentVendor.Id)
                    return Content("This is not your tvchannel");
            }

            //try to get a picture with the specified id
            var picture = await _pictureService.GetPictureByIdAsync(tvchannelPicture.PictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            await _pictureService.UpdatePictureAsync(picture.Id,
                await _pictureService.LoadPictureBinaryAsync(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute,
                model.OverrideTitleAttribute);

            tvchannelPicture.DisplayOrder = model.DisplayOrder;
            await _tvchannelService.UpdateTvChannelPictureAsync(tvchannelPicture);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelPictureDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel picture with the specified id
            var tvchannelPicture = await _tvchannelService.GetTvChannelPictureByIdAsync(id)
                ?? throw new ArgumentException("No tvchannel picture found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelPicture.TvChannelId);
                if (tvchannel != null && tvchannel.VendorId != currentVendor.Id)
                    return Content("This is not your tvchannel");
            }

            var pictureId = tvchannelPicture.PictureId;
            await _tvchannelService.DeleteTvChannelPictureAsync(tvchannelPicture);

            //try to get a picture with the specified id
            var picture = await _pictureService.GetPictureByIdAsync(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            await _pictureService.DeletePictureAsync(picture);

            return new NullJsonResult();
        }

        #endregion

        #region TvChannel videos

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelVideoAdd(int tvchannelId, [Validate] TvChannelVideoModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            if (tvchannelId == 0)
                throw new ArgumentException();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

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

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List");
            try
            {
                var video = new Video
                {
                    VideoUrl = videoUrl
                };

                //insert video
                await _videoService.InsertVideoAsync(video);

                await _tvchannelService.InsertTvChannelVideoAsync(new TvChannelVideo
                {
                    VideoId = video.Id,
                    TvChannelId = tvchannel.Id,
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

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelVideoListModelAsync(searchModel, tvchannel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelVideoUpdate([Validate] TvChannelVideoModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel picture with the specified id
            var tvchannelVideo = await _tvchannelService.GetTvChannelVideoByIdAsync(model.Id)
                ?? throw new ArgumentException("No tvchannel video found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelVideo.TvChannelId);
                if (tvchannel != null && tvchannel.VendorId != currentVendor.Id)
                    return Content("This is not your tvchannel");
            }

            //try to get a video with the specified id
            var video = await _videoService.GetVideoByIdAsync(tvchannelVideo.VideoId)
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

            tvchannelVideo.DisplayOrder = model.DisplayOrder;
            await _tvchannelService.UpdateTvChannelVideoAsync(tvchannelVideo);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelVideoDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel video with the specified id
            var tvchannelVideo = await _tvchannelService.GetTvChannelVideoByIdAsync(id)
                ?? throw new ArgumentException("No tvchannel video found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelVideo.TvChannelId);
                if (tvchannel != null && tvchannel.VendorId != currentVendor.Id)
                    return Content("This is not your tvchannel");
            }

            var videoId = tvchannelVideo.VideoId;
            await _tvchannelService.DeleteTvChannelVideoAsync(tvchannelVideo);

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

            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(model.TvChannelId);
            if (tvchannel == null)
            {
                _notificationService.ErrorNotification("No tvchannel found with the specified id");
                return RedirectToAction("List");
            }

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
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
                    new { tvchannelId = psa.TvChannelId, specificationId = psa.Id });

            //select an appropriate card
            SaveSelectedCardName("tvchannel-specification-attributes");
            return RedirectToAction("Edit", new { id = model.TvChannelId });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelSpecAttrList(TvChannelSpecificationAttributeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelSpecificationAttributeListModelAsync(searchModel, tvchannel);

            return Json(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> TvChannelSpecAttrUpdate(AddSpecificationAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel specification attribute with the specified id
            var psa = await _specificationAttributeService.GetTvChannelSpecificationAttributeByIdAsync(model.SpecificationId);
            if (psa == null)
            {
                //select an appropriate card
                SaveSelectedCardName("tvchannel-specification-attributes");
                _notificationService.ErrorNotification("No tvchannel specification attribute found with the specified id");

                return RedirectToAction("Edit", new { id = model.TvChannelId });
            }

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null
                && (await _tvchannelService.GetTvChannelByIdAsync(psa.TvChannelId)).VendorId != currentVendor.Id)
            {
                _notificationService.ErrorNotification("This is not your tvchannel");

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
                    new { tvchannelId = psa.TvChannelId, specificationId = model.SpecificationId });
            }

            //select an appropriate card
            SaveSelectedCardName("tvchannel-specification-attributes");

            return RedirectToAction("Edit", new { id = psa.TvChannelId });
        }

        public virtual async Task<IActionResult> TvChannelSpecAttributeAddOrEdit(int tvchannelId, int? specificationId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            if (await _tvchannelService.GetTvChannelByIdAsync(tvchannelId) == null)
            {
                _notificationService.ErrorNotification("No tvchannel found with the specified id");
                return RedirectToAction("List");
            }

            //try to get a tvchannel specification attribute with the specified id
            try
            {
                var model = await _tvchannelModelFactory.PrepareAddSpecificationAttributeModelAsync(tvchannelId, specificationId);
                return View(model);
            }
            catch (Exception ex)
            {
                await _notificationService.ErrorNotificationAsync(ex);

                //select an appropriate card
                SaveSelectedCardName("tvchannel-specification-attributes");
                return RedirectToAction("Edit", new { id = tvchannelId });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelSpecAttrDelete(AddSpecificationAttributeModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel specification attribute with the specified id
            var psa = await _specificationAttributeService.GetTvChannelSpecificationAttributeByIdAsync(model.SpecificationId);
            if (psa == null)
            {
                //select an appropriate card
                SaveSelectedCardName("tvchannel-specification-attributes");
                _notificationService.ErrorNotification("No tvchannel specification attribute found with the specified id");
                return RedirectToAction("Edit", new { id = model.TvChannelId });
            }

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && (await _tvchannelService.GetTvChannelByIdAsync(psa.TvChannelId)).VendorId != currentVendor.Id)
            {
                _notificationService.ErrorNotification("This is not your tvchannel");
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
            var model = await _tvchannelModelFactory.PrepareTvChannelTagSearchModelAsync(new TvChannelTagSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelTags(TvChannelTagSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelTags))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelTagListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelTagDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelTags))
                return AccessDeniedView();

            //try to get a tvchannel tag with the specified id
            var tag = await _tvchannelTagService.GetTvChannelTagByIdAsync(id)
                ?? throw new ArgumentException("No tvchannel tag found with the specified id");

            await _tvchannelTagService.DeleteTvChannelTagAsync(tag);

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

            var tags = await _tvchannelTagService.GetTvChannelTagsByIdsAsync(selectedIds.ToArray());
            await _tvchannelTagService.DeleteTvChannelTagsAsync(tags);

            return Json(new { Result = true });
        }

        public virtual async Task<IActionResult> EditTvChannelTag(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelTags))
                return AccessDeniedView();

            //try to get a tvchannel tag with the specified id
            var tvchannelTag = await _tvchannelTagService.GetTvChannelTagByIdAsync(id);
            if (tvchannelTag == null)
                return RedirectToAction("List");

            //prepare tag model
            var model = await _tvchannelModelFactory.PrepareTvChannelTagModelAsync(null, tvchannelTag);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditTvChannelTag(TvChannelTagModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelTags))
                return AccessDeniedView();

            //try to get a tvchannel tag with the specified id
            var tvchannelTag = await _tvchannelTagService.GetTvChannelTagByIdAsync(model.Id);
            if (tvchannelTag == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                tvchannelTag.Name = model.Name;
                await _tvchannelTagService.UpdateTvChannelTagAsync(tvchannelTag);

                //locales
                await UpdateLocalesAsync(tvchannelTag, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannelTags.Updated"));

                return continueEditing ? RedirectToAction("EditTvChannelTag", new { id = tvchannelTag.Id }) : RedirectToAction("TvChannelTags");
            }

            //prepare model
            model = await _tvchannelModelFactory.PrepareTvChannelTagModelAsync(model, tvchannelTag, true);

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

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelOrderListModelAsync(searchModel, tvchannel);

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

            //a vendor should have access only to his tvchannels
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

            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(0,
                categoryIds: categoryIds,
                manufacturerIds: new List<int> { model.SearchManufacturerId },
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                tvchannelType: model.SearchTvChannelTypeId > 0 ? (TvChannelType?)model.SearchTvChannelTypeId : null,
                keywords: model.SearchTvChannelName,
                showHidden: true,
                overridePublished: overridePublished);

            try
            {
                byte[] bytes;
                await using (var stream = new MemoryStream())
                {
                    await _pdfService.PrintTvChannelsToPdfAsync(stream, tvchannels);
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

            //a vendor should have access only to his tvchannels
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

            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(0,
                categoryIds: categoryIds,
                manufacturerIds: new List<int> { model.SearchManufacturerId },
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                tvchannelType: model.SearchTvChannelTypeId > 0 ? (TvChannelType?)model.SearchTvChannelTypeId : null,
                keywords: model.SearchTvChannelName,
                showHidden: true,
                overridePublished: overridePublished);

            try
            {
                var xml = await _exportManager.ExportTvChannelsToXmlAsync(tvchannels);

                return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "tvchannels.xml");
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

            var tvchannels = new List<TvChannel>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                tvchannels.AddRange(await _tvchannelService.GetTvChannelsByIdsAsync(ids));
            }
            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                tvchannels = tvchannels.Where(p => p.VendorId == currentVendor.Id).ToList();
            }

            try
            {
                var xml = await _exportManager.ExportTvChannelsToXmlAsync(tvchannels);
                return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "tvchannels.xml");
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

            //a vendor should have access only to his tvchannels
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

            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(0,
                categoryIds: categoryIds,
                manufacturerIds: new List<int> { model.SearchManufacturerId },
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                tvchannelType: model.SearchTvChannelTypeId > 0 ? (TvChannelType?)model.SearchTvChannelTypeId : null,
                keywords: model.SearchTvChannelName,
                showHidden: true,
                overridePublished: overridePublished);

            try
            {
                var bytes = await _exportManager.ExportTvChannelsToXlsxAsync(tvchannels);

                return File(bytes, MimeTypes.TextXlsx, "tvchannels.xlsx");
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

            var tvchannels = new List<TvChannel>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                tvchannels.AddRange(await _tvchannelService.GetTvChannelsByIdsAsync(ids));
            }
            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                tvchannels = tvchannels.Where(p => p.VendorId == currentVendor.Id).ToList();
            }

            try
            {
                var bytes = await _exportManager.ExportTvChannelsToXlsxAsync(tvchannels);

                return File(bytes, MimeTypes.TextXlsx, "tvchannels.xlsx");
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
                //a vendor can not import tvchannels
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

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTierPriceListModelAsync(searchModel, tvchannel);

            return Json(model);
        }

        public virtual async Task<IActionResult> TierPriceCreatePopup(int tvchannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTierPriceModelAsync(new TierPriceModel(), tvchannel, null);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> TierPriceCreatePopup(TierPriceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(model.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            if (ModelState.IsValid)
            {
                //fill entity from model
                var tierPrice = model.ToEntity<TierPrice>();
                tierPrice.TvChannelId = tvchannel.Id;
                tierPrice.UserRoleId = model.UserRoleId > 0 ? model.UserRoleId : (int?)null;

                await _tvchannelService.InsertTierPriceAsync(tierPrice);

                //update "HasTierPrices" property
                await _tvchannelService.UpdateHasTierPricesPropertyAsync(tvchannel);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvchannelModelFactory.PrepareTierPriceModelAsync(model, tvchannel, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> TierPriceEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = await _tvchannelService.GetTierPriceByIdAsync(id);
            if (tierPrice == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tierPrice.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTierPriceModelAsync(null, tvchannel, tierPrice);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TierPriceEditPopup(TierPriceModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = await _tvchannelService.GetTierPriceByIdAsync(model.Id);
            if (tierPrice == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tierPrice.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            if (ModelState.IsValid)
            {
                //fill entity from model
                tierPrice = model.ToEntity(tierPrice);
                tierPrice.UserRoleId = model.UserRoleId > 0 ? model.UserRoleId : (int?)null;
                await _tvchannelService.UpdateTierPriceAsync(tierPrice);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvchannelModelFactory.PrepareTierPriceModelAsync(model, tvchannel, tierPrice, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TierPriceDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = await _tvchannelService.GetTierPriceByIdAsync(id)
                ?? throw new ArgumentException("No tier price found with the specified id");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tierPrice.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            await _tvchannelService.DeleteTierPriceAsync(tierPrice);

            //update "HasTierPrices" property
            await _tvchannelService.UpdateHasTierPricesPropertyAsync(tvchannel);

            return new NullJsonResult();
        }

        #endregion

        #region TvChannel attributes

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeMappingList(TvChannelAttributeMappingSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelAttributeMappingListModelAsync(searchModel, tvchannel);

            return Json(model);
        }

        public virtual async Task<IActionResult> TvChannelAttributeMappingCreate(int tvchannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("This is not your tvchannel"));
                return RedirectToAction("List");
            }

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelAttributeMappingModelAsync(new TvChannelAttributeMappingModel(), tvchannel, null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> TvChannelAttributeMappingCreate(TvChannelAttributeMappingModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(model.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("This is not your tvchannel"));
                return RedirectToAction("List");
            }

            //ensure this attribute is not mapped yet
            if ((await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannel.Id))
                .Any(x => x.TvChannelAttributeId == model.TvChannelAttributeId))
            {
                //redisplay form
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.AlreadyExists"));

                model = await _tvchannelModelFactory.PrepareTvChannelAttributeMappingModelAsync(model, tvchannel, null, true);

                return View(model);
            }

            //insert mapping
            var tvchannelAttributeMapping = model.ToEntity<TvChannelAttributeMapping>();

            await _tvchannelAttributeService.InsertTvChannelAttributeMappingAsync(tvchannelAttributeMapping);
            await UpdateLocalesAsync(tvchannelAttributeMapping, model);

            //predefined values
            var predefinedValues = await _tvchannelAttributeService.GetPredefinedTvChannelAttributeValuesAsync(model.TvChannelAttributeId);
            foreach (var predefinedValue in predefinedValues)
            {
                var pav = new TvChannelAttributeValue
                {
                    TvChannelAttributeMappingId = tvchannelAttributeMapping.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = predefinedValue.Name,
                    PriceAdjustment = predefinedValue.PriceAdjustment,
                    PriceAdjustmentUsePercentage = predefinedValue.PriceAdjustmentUsePercentage,
                    WeightAdjustment = predefinedValue.WeightAdjustment,
                    Cost = predefinedValue.Cost,
                    IsPreSelected = predefinedValue.IsPreSelected,
                    DisplayOrder = predefinedValue.DisplayOrder
                };
                await _tvchannelAttributeService.InsertTvChannelAttributeValueAsync(pav);

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
                return RedirectToAction("Edit", new { id = tvchannel.Id });
            }

            return RedirectToAction("TvChannelAttributeMappingEdit", new { id = tvchannelAttributeMapping.Id });
        }

        public virtual async Task<IActionResult> TvChannelAttributeMappingEdit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel attribute mapping with the specified id
            var tvchannelAttributeMapping = await _tvchannelAttributeService.GetTvChannelAttributeMappingByIdAsync(id)
                ?? throw new ArgumentException("No tvchannel attribute mapping found with the specified id");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("This is not your tvchannel"));
                return RedirectToAction("List");
            }

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelAttributeMappingModelAsync(null, tvchannel, tvchannelAttributeMapping);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> TvChannelAttributeMappingEdit(TvChannelAttributeMappingModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel attribute mapping with the specified id
            var tvchannelAttributeMapping = await _tvchannelAttributeService.GetTvChannelAttributeMappingByIdAsync(model.Id)
                ?? throw new ArgumentException("No tvchannel attribute mapping found with the specified id");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("This is not your tvchannel"));
                return RedirectToAction("List");
            }

            //ensure this attribute is not mapped yet
            if ((await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannel.Id))
                .Any(x => x.TvChannelAttributeId == model.TvChannelAttributeId && x.Id != tvchannelAttributeMapping.Id))
            {
                //redisplay form
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.AlreadyExists"));

                model = await _tvchannelModelFactory.PrepareTvChannelAttributeMappingModelAsync(model, tvchannel, tvchannelAttributeMapping, true);

                return View(model);
            }

            //fill entity from model
            tvchannelAttributeMapping = model.ToEntity(tvchannelAttributeMapping);
            await _tvchannelAttributeService.UpdateTvChannelAttributeMappingAsync(tvchannelAttributeMapping);

            await UpdateLocalesAsync(tvchannelAttributeMapping, model);

            await SaveConditionAttributesAsync(tvchannelAttributeMapping, model.ConditionModel, form);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Updated"));

            if (!continueEditing)
            {
                //select an appropriate card
                SaveSelectedCardName("tvchannel-tvchannel-attributes");
                return RedirectToAction("Edit", new { id = tvchannel.Id });
            }

            return RedirectToAction("TvChannelAttributeMappingEdit", new { id = tvchannelAttributeMapping.Id });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeMappingDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel attribute mapping with the specified id
            var tvchannelAttributeMapping = await _tvchannelAttributeService.GetTvChannelAttributeMappingByIdAsync(id)
                ?? throw new ArgumentException("No tvchannel attribute mapping found with the specified id");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            //check if existed combinations contains the specified attribute
            var existedCombinations = await _tvchannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvchannel.Id);
            if (existedCombinations?.Any() == true)
            {
                foreach (var combination in existedCombinations)
                {
                    var mappings = await _tvchannelAttributeParser
                        .ParseTvChannelAttributeMappingsAsync(combination.AttributesXml);
                    
                    if (mappings?.Any(m => m.Id == tvchannelAttributeMapping.Id) == true)
                    {
                        _notificationService.ErrorNotification(
                            string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.AlreadyExistsInCombination"),
                                await _tvchannelAttributeFormatter.FormatAttributesAsync(tvchannel, combination.AttributesXml, await _workContext.GetCurrentUserAsync(), await _storeContext.GetCurrentStoreAsync(), ", ")));

                        return RedirectToAction("TvChannelAttributeMappingEdit", new { id = tvchannelAttributeMapping.Id });
                    }
                }
            }

            await _tvchannelAttributeService.DeleteTvChannelAttributeMappingAsync(tvchannelAttributeMapping);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Deleted"));

            //select an appropriate card
            SaveSelectedCardName("tvchannel-tvchannel-attributes");
            return RedirectToAction("Edit", new { id = tvchannelAttributeMapping.TvChannelId });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeValueList(TvChannelAttributeValueSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //try to get a tvchannel attribute mapping with the specified id
            var tvchannelAttributeMapping = await _tvchannelAttributeService.GetTvChannelAttributeMappingByIdAsync(searchModel.TvChannelAttributeMappingId)
                ?? throw new ArgumentException("No tvchannel attribute mapping found with the specified id");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelAttributeValueListModelAsync(searchModel, tvchannelAttributeMapping);

            return Json(model);
        }

        public virtual async Task<IActionResult> TvChannelAttributeValueCreatePopup(int tvchannelAttributeMappingId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel attribute mapping with the specified id
            var tvchannelAttributeMapping = await _tvchannelAttributeService.GetTvChannelAttributeMappingByIdAsync(tvchannelAttributeMappingId)
                ?? throw new ArgumentException("No tvchannel attribute mapping found with the specified id");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelAttributeValueModelAsync(new TvChannelAttributeValueModel(), tvchannelAttributeMapping, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeValueCreatePopup(TvChannelAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel attribute mapping with the specified id
            var tvchannelAttributeMapping = await _tvchannelAttributeService.GetTvChannelAttributeMappingByIdAsync(model.TvChannelAttributeMappingId);
            if (tvchannelAttributeMapping == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            if (tvchannelAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares)
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
            if (tvchannelAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares && model.ImageSquaresPictureId == 0)
            {
                ModelState.AddModelError(string.Empty, "Image is required");
            }

            if (ModelState.IsValid)
            {
                //fill entity from model
                var pav = model.ToEntity<TvChannelAttributeValue>();

                pav.Quantity = model.UserEntersQty ? 1 : model.Quantity;

                await _tvchannelAttributeService.InsertTvChannelAttributeValueAsync(pav);
                await UpdateLocalesAsync(pav, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvchannelModelFactory.PrepareTvChannelAttributeValueModelAsync(model, tvchannelAttributeMapping, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> TvChannelAttributeValueEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel attribute value with the specified id
            var tvchannelAttributeValue = await _tvchannelAttributeService.GetTvChannelAttributeValueByIdAsync(id);
            if (tvchannelAttributeValue == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvchannel attribute mapping with the specified id
            var tvchannelAttributeMapping = await _tvchannelAttributeService.GetTvChannelAttributeMappingByIdAsync(tvchannelAttributeValue.TvChannelAttributeMappingId);
            if (tvchannelAttributeMapping == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelAttributeValueModelAsync(null, tvchannelAttributeMapping, tvchannelAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeValueEditPopup(TvChannelAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel attribute value with the specified id
            var tvchannelAttributeValue = await _tvchannelAttributeService.GetTvChannelAttributeValueByIdAsync(model.Id);
            if (tvchannelAttributeValue == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvchannel attribute mapping with the specified id
            var tvchannelAttributeMapping = await _tvchannelAttributeService.GetTvChannelAttributeMappingByIdAsync(tvchannelAttributeValue.TvChannelAttributeMappingId);
            if (tvchannelAttributeMapping == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            if (tvchannelAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares)
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
            if (tvchannelAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares && model.ImageSquaresPictureId == 0)
            {
                ModelState.AddModelError(string.Empty, "Image is required");
            }

            if (ModelState.IsValid)
            {
                //fill entity from model
                tvchannelAttributeValue = model.ToEntity(tvchannelAttributeValue);
                tvchannelAttributeValue.Quantity = model.UserEntersQty ? 1 : model.Quantity;
                await _tvchannelAttributeService.UpdateTvChannelAttributeValueAsync(tvchannelAttributeValue);

                await UpdateLocalesAsync(tvchannelAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvchannelModelFactory.PrepareTvChannelAttributeValueModelAsync(model, tvchannelAttributeMapping, tvchannelAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeValueDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel attribute value with the specified id
            var tvchannelAttributeValue = await _tvchannelAttributeService.GetTvChannelAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No tvchannel attribute value found with the specified id");

            //try to get a tvchannel attribute mapping with the specified id
            var tvchannelAttributeMapping = await _tvchannelAttributeService.GetTvChannelAttributeMappingByIdAsync(tvchannelAttributeValue.TvChannelAttributeMappingId)
                ?? throw new ArgumentException("No tvchannel attribute mapping found with the specified id");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelAttributeMapping.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            //check if existed combinations contains the specified attribute value
            var existedCombinations = await _tvchannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvchannel.Id);
            if (existedCombinations?.Any() == true)
            {
                foreach (var combination in existedCombinations)
                {
                    var attributeValues = await _tvchannelAttributeParser.ParseTvChannelAttributeValuesAsync(combination.AttributesXml);
                    
                    if (attributeValues.Where(attribute => attribute.Id == id).Any())
                    {
                        return Conflict(string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Values.AlreadyExistsInCombination"),
                            await _tvchannelAttributeFormatter.FormatAttributesAsync(tvchannel, combination.AttributesXml, await _workContext.GetCurrentUserAsync(), await _storeContext.GetCurrentStoreAsync(), ", ")));
                    }
                }
            }

            await _tvchannelAttributeService.DeleteTvChannelAttributeValueAsync(tvchannelAttributeValue);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> AssociateTvChannelToAttributeValuePopup()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //prepare model
            var model = await _tvchannelModelFactory.PrepareAssociateTvChannelToAttributeValueSearchModelAsync(new AssociateTvChannelToAttributeValueSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociateTvChannelToAttributeValuePopupList(AssociateTvChannelToAttributeValueSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvchannelModelFactory.PrepareAssociateTvChannelToAttributeValueListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> AssociateTvChannelToAttributeValuePopup([Bind(Prefix = nameof(AssociateTvChannelToAttributeValueModel))] AssociateTvChannelToAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel with the specified id
            var associatedTvChannel = await _tvchannelService.GetTvChannelByIdAsync(model.AssociatedToTvChannelId);
            if (associatedTvChannel == null)
                return Content("Cannot load a tvchannel");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && associatedTvChannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            ViewBag.RefreshPage = true;
            ViewBag.tvchannelId = associatedTvChannel.Id;
            ViewBag.tvchannelName = associatedTvChannel.Name;

            return View(new AssociateTvChannelToAttributeValueSearchModel());
        }

        //action displaying notification (warning) to a store owner when associating some tvchannel
        public virtual async Task<IActionResult> AssociatedTvChannelGetWarnings(int tvchannelId)
        {
            var associatedTvChannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
            if (associatedTvChannel == null)
                return Json(new { Result = string.Empty });

            //attributes
            if (await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(associatedTvChannel.Id) is IList<TvChannelAttributeMapping> mapping && mapping.Any())
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

            //downloadable tvchannel
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

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelAttributeCombinationListModelAsync(searchModel, tvchannel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeCombinationDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = await _tvchannelAttributeService.GetTvChannelAttributeCombinationByIdAsync(id)
                ?? throw new ArgumentException("No tvchannel attribute combination found with the specified id");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(combination.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            await _tvchannelAttributeService.DeleteTvChannelAttributeCombinationAsync(combination);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> TvChannelAttributeCombinationCreatePopup(int tvchannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
            if (tvchannel == null)
                return RedirectToAction("List", "TvChannel");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelAttributeCombinationModelAsync(new TvChannelAttributeCombinationModel(), tvchannel, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeCombinationCreatePopup(int tvchannelId, TvChannelAttributeCombinationModel model, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
            if (tvchannel == null)
                return RedirectToAction("List", "TvChannel");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //attributes
            var warnings = new List<string>();
            var attributesXml = await GetAttributesXmlForTvChannelAttributeCombinationAsync(form, warnings, tvchannel.Id);

            //check whether the attribute value is specified
            if (string.IsNullOrEmpty(attributesXml))
                warnings.Add(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Alert.FailedValue"));

            warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentUserAsync(),
                ShoppingCartType.ShoppingCart, tvchannel, 1, attributesXml, true));

            //check whether the same attribute combination already exists
            var existingCombination = await _tvchannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvchannel, attributesXml);
            if (existingCombination != null)
                warnings.Add(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.AlreadyExists"));

            if (!warnings.Any())
            {
                //save combination
                var combination = model.ToEntity<TvChannelAttributeCombination>();

                //fill attributes
                combination.AttributesXml = attributesXml;

                await _tvchannelAttributeService.InsertTvChannelAttributeCombinationAsync(combination);

                //quantity change history
                await _tvchannelService.AddStockQuantityHistoryEntryAsync(tvchannel, combination.StockQuantity, combination.StockQuantity,
                    message: await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvchannelModelFactory.PrepareTvChannelAttributeCombinationModelAsync(model, tvchannel, null, true);
            model.Warnings = warnings;

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> TvChannelAttributeCombinationGeneratePopup(int tvchannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId);
            if (tvchannel == null)
                return RedirectToAction("List", "TvChannel");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelAttributeCombinationModelAsync(new TvChannelAttributeCombinationModel(), tvchannel, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeCombinationGeneratePopup(IFormCollection form, TvChannelAttributeCombinationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(model.TvChannelId);
            if (tvchannel == null)
                return RedirectToAction("List", "TvChannel");

            var allowedAttributeIds = form.Keys.Where(key => key.Contains("attribute_value_"))
                .Select(key => int.TryParse(form[key], out var id) ? id : 0).Where(id => id > 0).ToList();

            var requiredAttributeNames = await (await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannel.Id))
                .Where(pam => pam.IsRequired)
                .Where(pam => !pam.IsNonCombinable())
                .WhereAwait(async pam => !(await _tvchannelAttributeService.GetTvChannelAttributeValuesAsync(pam.Id)).Any(v => allowedAttributeIds.Any(id => id == v.Id)))
                .SelectAwait(async pam => (await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(pam.TvChannelAttributeId)).Name).ToListAsync();

            if (requiredAttributeNames.Any())
            {
                model = await _tvchannelModelFactory.PrepareTvChannelAttributeCombinationModelAsync(model, tvchannel, null, true);
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

            await GenerateAttributeCombinationsAsync(tvchannel, allowedAttributeIds);

            ViewBag.RefreshPage = true;

            return View(new TvChannelAttributeCombinationModel());
        }

        public virtual async Task<IActionResult> TvChannelAttributeCombinationEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = await _tvchannelAttributeService.GetTvChannelAttributeCombinationByIdAsync(id);
            if (combination == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(combination.TvChannelId);
            if (tvchannel == null)
                return RedirectToAction("List", "TvChannel");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareTvChannelAttributeCombinationModelAsync(null, tvchannel, combination);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelAttributeCombinationEditPopup(TvChannelAttributeCombinationModel model, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = await _tvchannelAttributeService.GetTvChannelAttributeCombinationByIdAsync(model.Id);
            if (combination == null)
                return RedirectToAction("List", "TvChannel");

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(combination.TvChannelId);
            if (tvchannel == null)
                return RedirectToAction("List", "TvChannel");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return RedirectToAction("List", "TvChannel");

            //attributes
            var warnings = new List<string>();
            var attributesXml = await GetAttributesXmlForTvChannelAttributeCombinationAsync(form, warnings, tvchannel.Id);

            //check whether the attribute value is specified
            if (string.IsNullOrEmpty(attributesXml))
                warnings.Add(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Alert.FailedValue"));

            warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentUserAsync(),
                ShoppingCartType.ShoppingCart, tvchannel, 1, attributesXml, true));

            //check whether the same attribute combination already exists
            var existingCombination = await _tvchannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvchannel, attributesXml);
            if (existingCombination != null && existingCombination.Id != model.Id && existingCombination.AttributesXml.Equals(attributesXml))
                warnings.Add(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.AlreadyExists"));

            if (!warnings.Any() && ModelState.IsValid)
            {
                var previousStockQuantity = combination.StockQuantity;

                //save combination
                //fill entity from model
                combination = model.ToEntity(combination);
                combination.AttributesXml = attributesXml;

                await _tvchannelAttributeService.UpdateTvChannelAttributeCombinationAsync(combination);

                //quantity change history
                await _tvchannelService.AddStockQuantityHistoryEntryAsync(tvchannel, combination.StockQuantity - previousStockQuantity, combination.StockQuantity,
                    message: await _localizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvchannelModelFactory.PrepareTvChannelAttributeCombinationModelAsync(model, tvchannel, combination, true);
            model.Warnings = warnings;

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GenerateAllAttributeCombinations(int tvchannelId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannels))
                return AccessDeniedView();

            //try to get a tvchannel with the specified id
            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            await GenerateAttributeCombinationsAsync(tvchannel);

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

            var tvchannelEditorSettings = await _settingService.LoadSettingAsync<TvChannelEditorSettings>();
            tvchannelEditorSettings = model.TvChannelEditorSettingsModel.ToSettings(tvchannelEditorSettings);
            await _settingService.SaveSettingAsync(tvchannelEditorSettings);

            //tvchannel list
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

            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(searchModel.TvChannelId)
                ?? throw new ArgumentException("No tvchannel found with the specified id");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && tvchannel.VendorId != currentVendor.Id)
                return Content("This is not your tvchannel");

            //prepare model
            var model = await _tvchannelModelFactory.PrepareStockQuantityHistoryListModelAsync(searchModel, tvchannel);

            return Json(model);
        }

        #endregion

        #endregion
    }
}