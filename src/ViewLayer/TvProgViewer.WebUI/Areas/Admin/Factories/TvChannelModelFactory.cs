using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Discounts;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Shipping;
using TvProgViewer.Services.Stores;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.WebUI.Areas.Admin.Models.Common;
using TvProgViewer.WebUI.Areas.Admin.Models.Orders;
using TvProgViewer.Web.Framework.Extensions;
using TvProgViewer.Web.Framework.Factories;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the tvChannel model factory implementation
    /// </summary>
    public partial class TvChannelModelFactory : ITvChannelModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IAddressService _addressService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDiscountService _discountService;
        private readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IManufacturerService _manufacturerService;
        private readonly IMeasureService _measureService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly ITvChannelAttributeFormatter _tvChannelAttributeFormatter;
        private readonly ITvChannelAttributeParser _tvChannelAttributeParser;
        private readonly ITvChannelAttributeService _tvChannelAttributeService;
        private readonly ITvChannelService _tvChannelService;
        private readonly ITvChannelTagService _tvChannelTagService;
        private readonly ITvChannelTemplateService _tvChannelTemplateService;
        private readonly ISettingModelFactory _settingModelFactory;
        private readonly ISettingService _settingService;
        private readonly IShipmentService _shipmentService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVideoService _videoService;
        private readonly IWorkContext _workContext;
        private readonly MeasureSettings _measureSettings;
        private readonly TvProgHttpClient _nopHttpClient;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public TvChannelModelFactory(CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IAddressService addressService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            IDiscountSupportedModelFactory discountSupportedModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IManufacturerService manufacturerService,
            IMeasureService measureService,
            IOrderService orderService,
            IPictureService pictureService,
            ITvChannelAttributeFormatter tvChannelAttributeFormatter,
            ITvChannelAttributeParser tvChannelAttributeParser,
            ITvChannelAttributeService tvChannelAttributeService,
            ITvChannelService tvChannelService,
            ITvChannelTagService tvChannelTagService,
            ITvChannelTemplateService tvChannelTemplateService,
            ISettingModelFactory settingModelFactory,
            ISettingService settingService,
            IShipmentService shipmentService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            ISpecificationAttributeService specificationAttributeService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreContext storeContext,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IVideoService videoService,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            TvProgHttpClient nopHttpClient,
            TaxSettings taxSettings,
            VendorSettings vendorSettings)
        {
            _catalogSettings = catalogSettings;
            _currencySettings = currencySettings;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _addressService = addressService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _discountService = discountService;
            _discountSupportedModelFactory = discountSupportedModelFactory;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _manufacturerService = manufacturerService;
            _measureService = measureService;
            _orderService = orderService;
            _pictureService = pictureService;
            _tvChannelAttributeFormatter = tvChannelAttributeFormatter;
            _tvChannelAttributeParser = tvChannelAttributeParser;
            _tvChannelAttributeService = tvChannelAttributeService;
            _tvChannelService = tvChannelService;
            _tvChannelTagService = tvChannelTagService;
            _tvChannelTemplateService = tvChannelTemplateService;
            _settingModelFactory = settingModelFactory;
            _settingService = settingService;
            _shipmentService = shipmentService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _specificationAttributeService = specificationAttributeService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _storeContext = storeContext;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _videoService = videoService;
            _workContext = workContext;
            _measureSettings = measureSettings;
            _nopHttpClient = nopHttpClient;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task<string> GetSpecificationAttributeNameAsync(SpecificationAttribute specificationAttribute)
        {
            var name = specificationAttribute.Name;

            if (specificationAttribute.SpecificationAttributeGroupId.HasValue)
            {
                var group = await _specificationAttributeService.GetSpecificationAttributeGroupByIdAsync(specificationAttribute.SpecificationAttributeGroupId.Value);
                if (group != null)
                    name = string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.SpecificationAttributes.NameFormat"), group.Name, name);
            }

            return name;
        }

        /// <summary>
        /// Prepare copy tvChannel model
        /// </summary>
        /// <param name="model">Copy tvChannel model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the copy tvChannel model
        /// </returns>
        protected virtual async Task<CopyTvChannelModel> PrepareCopyTvChannelModelAsync(CopyTvChannelModel model, TvChannel tvChannel)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.Id = tvChannel.Id;
            model.Name = string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Copy.Name.New"), tvChannel.Name);
            model.Published = true;
            model.CopyMultimedia = true;

            return model;
        }

        /// <summary>
        /// Prepare tvChannel warehouse inventory models
        /// </summary>
        /// <param name="models">List of tvChannel warehouse inventory models</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task PrepareTvChannelWarehouseInventoryModelsAsync(IList<TvChannelWarehouseInventoryModel> models, TvChannel tvChannel)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            foreach (var warehouse in await _shippingService.GetAllWarehousesAsync())
            {
                var model = new TvChannelWarehouseInventoryModel
                {
                    WarehouseId = warehouse.Id,
                    WarehouseName = warehouse.Name
                };

                if (tvChannel != null)
                {
                    var tvChannelWarehouseInventory = (await _tvChannelService.GetAllTvChannelWarehouseInventoryRecordsAsync(tvChannel.Id))?.FirstOrDefault(inventory => inventory.WarehouseId == warehouse.Id);
                    if (tvChannelWarehouseInventory != null)
                    {
                        model.WarehouseUsed = true;
                        model.StockQuantity = tvChannelWarehouseInventory.StockQuantity;
                        model.ReservedQuantity = tvChannelWarehouseInventory.ReservedQuantity;
                        model.PlannedQuantity = await _shipmentService.GetQuantityInShipmentsAsync(tvChannel, tvChannelWarehouseInventory.WarehouseId, true, true);
                    }
                }

                models.Add(model);
            }
        }

        /// <summary>
        /// Prepare tvChannel attribute mapping validation rules string
        /// </summary>
        /// <param name="attributeMapping">TvChannel attribute mapping</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the validation rules string
        /// </returns>
        protected virtual async Task<string> PrepareTvChannelAttributeMappingValidationRulesStringAsync(TvChannelAttributeMapping attributeMapping)
        {
            if (!attributeMapping.ValidationRulesAllowed())
                return string.Empty;

            var validationRules = new StringBuilder(string.Empty);
            if (attributeMapping.ValidationMinLength.HasValue)
            {
                validationRules.AppendFormat("{0}: {1}<br />",
                    await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.ValidationRules.MinLength"),
                    attributeMapping.ValidationMinLength);
            }

            if (attributeMapping.ValidationMaxLength.HasValue)
            {
                validationRules.AppendFormat("{0}: {1}<br />",
                    await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.ValidationRules.MaxLength"),
                    attributeMapping.ValidationMaxLength);
            }

            if (!string.IsNullOrEmpty(attributeMapping.ValidationFileAllowedExtensions))
            {
                validationRules.AppendFormat("{0}: {1}<br />",
                    await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.ValidationRules.FileAllowedExtensions"),
                    WebUtility.HtmlEncode(attributeMapping.ValidationFileAllowedExtensions));
            }

            if (attributeMapping.ValidationFileMaximumSize.HasValue)
            {
                validationRules.AppendFormat("{0}: {1}<br />",
                    await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.ValidationRules.FileMaximumSize"),
                    attributeMapping.ValidationFileMaximumSize);
            }

            if (!string.IsNullOrEmpty(attributeMapping.DefaultValue))
            {
                validationRules.AppendFormat("{0}: {1}<br />",
                    await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.ValidationRules.DefaultValue"),
                    WebUtility.HtmlEncode(attributeMapping.DefaultValue));
            }

            return validationRules.ToString();
        }

        /// <summary>
        /// Prepare tvChannel attribute condition model
        /// </summary>
        /// <param name="model">TvChannel attribute condition model</param>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task PrepareTvChannelAttributeConditionModelAsync(TvChannelAttributeConditionModel model,
            TvChannelAttributeMapping tvChannelAttributeMapping)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (tvChannelAttributeMapping == null)
                throw new ArgumentNullException(nameof(tvChannelAttributeMapping));

            model.TvChannelAttributeMappingId = tvChannelAttributeMapping.Id;
            model.EnableCondition = !string.IsNullOrEmpty(tvChannelAttributeMapping.ConditionAttributeXml);

            //pre-select attribute and values
            var selectedPva = (await _tvChannelAttributeParser
                .ParseTvChannelAttributeMappingsAsync(tvChannelAttributeMapping.ConditionAttributeXml))
                .FirstOrDefault();

            var attributes = (await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannelAttributeMapping.TvChannelId))
                //ignore non-combinable attributes (should have selectable values)
                .Where(x => x.CanBeUsedAsCondition())
                //ignore this attribute (it cannot depend on itself)
                .Where(x => x.Id != tvChannelAttributeMapping.Id)
                .ToList();
            foreach (var attribute in attributes)
            {
                var attributeModel = new TvChannelAttributeConditionModel.TvChannelAttributeModel
                {
                    Id = attribute.Id,
                    TvChannelAttributeId = attribute.TvChannelAttributeId,
                    Name = (await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(attribute.TvChannelAttributeId)).Name,
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _tvChannelAttributeService.GetTvChannelAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new TvChannelAttributeConditionModel.TvChannelAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }

                    //pre-select attribute and value
                    if (selectedPva != null && attribute.Id == selectedPva.Id)
                    {
                        //attribute
                        model.SelectedTvChannelAttributeId = selectedPva.Id;

                        //values
                        switch (attribute.AttributeControlType)
                        {
                            case AttributeControlType.DropdownList:
                            case AttributeControlType.RadioList:
                            case AttributeControlType.Checkboxes:
                            case AttributeControlType.ColorSquares:
                            case AttributeControlType.ImageSquares:
                                if (!string.IsNullOrEmpty(tvChannelAttributeMapping.ConditionAttributeXml))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues =
                                        await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(tvChannelAttributeMapping
                                            .ConditionAttributeXml);
                                    foreach (var attributeValue in selectedValues)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                                item.IsPreSelected = true;
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

                model.TvChannelAttributes.Add(attributeModel);
            }
        }

        /// <summary>
        /// Prepare related tvChannel search model
        /// </summary>
        /// <param name="searchModel">Related tvChannel search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Related tvChannel search model</returns>
        protected virtual RelatedTvChannelSearchModel PrepareRelatedTvChannelSearchModel(RelatedTvChannelSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            searchModel.TvChannelId = tvChannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare cross-sell tvChannel search model
        /// </summary>
        /// <param name="searchModel">Cross-sell tvChannel search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Cross-sell tvChannel search model</returns>
        protected virtual CrossSellTvChannelSearchModel PrepareCrossSellTvChannelSearchModel(CrossSellTvChannelSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            searchModel.TvChannelId = tvChannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare associated tvChannel search model
        /// </summary>
        /// <param name="searchModel">Associated tvChannel search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Associated tvChannel search model</returns>
        protected virtual AssociatedTvChannelSearchModel PrepareAssociatedTvChannelSearchModel(AssociatedTvChannelSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            searchModel.TvChannelId = tvChannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvChannel picture search model
        /// </summary>
        /// <param name="searchModel">TvChannel picture search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>TvChannel picture search model</returns>
        protected virtual TvChannelPictureSearchModel PrepareTvChannelPictureSearchModel(TvChannelPictureSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            searchModel.TvChannelId = tvChannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvChannel video search model
        /// </summary>
        /// <param name="searchModel">TvChannel video search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>TvChannel video search model</returns>
        protected virtual TvChannelVideoSearchModel PrepareTvChannelVideoSearchModel(TvChannelVideoSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            searchModel.TvChannelId = tvChannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvChannel order search model
        /// </summary>
        /// <param name="searchModel">TvChannel order search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>TvChannel order search model</returns>
        protected virtual TvChannelOrderSearchModel PrepareTvChannelOrderSearchModel(TvChannelOrderSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            searchModel.TvChannelId = tvChannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tier price search model
        /// </summary>
        /// <param name="searchModel">Tier price search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Tier price search model</returns>
        protected virtual TierPriceSearchModel PrepareTierPriceSearchModel(TierPriceSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            searchModel.TvChannelId = tvChannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare stock quantity history search model
        /// </summary>
        /// <param name="searchModel">Stock quantity history search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the stock quantity history search model
        /// </returns>
        protected virtual async Task<StockQuantityHistorySearchModel> PrepareStockQuantityHistorySearchModelAsync(StockQuantityHistorySearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            searchModel.TvChannelId = tvChannel.Id;

            //prepare available warehouses
            await _baseAdminModelFactory.PrepareWarehousesAsync(searchModel.AvailableWarehouses);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvChannel attribute mapping search model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute mapping search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>TvChannel attribute mapping search model</returns>
        protected virtual TvChannelAttributeMappingSearchModel PrepareTvChannelAttributeMappingSearchModel(TvChannelAttributeMappingSearchModel searchModel,
            TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            searchModel.TvChannelId = tvChannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvChannel attribute value search model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute value search model</param>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>TvChannel attribute value search model</returns>
        protected virtual TvChannelAttributeValueSearchModel PrepareTvChannelAttributeValueSearchModel(TvChannelAttributeValueSearchModel searchModel,
            TvChannelAttributeMapping tvChannelAttributeMapping)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannelAttributeMapping == null)
                throw new ArgumentNullException(nameof(tvChannelAttributeMapping));

            searchModel.TvChannelAttributeMappingId = tvChannelAttributeMapping.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvChannel attribute combination search model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute combination search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>TvChannel attribute combination search model</returns>
        protected virtual TvChannelAttributeCombinationSearchModel PrepareTvChannelAttributeCombinationSearchModel(
            TvChannelAttributeCombinationSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            searchModel.TvChannelId = tvChannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvChannel specification attribute search model
        /// </summary>
        /// <param name="searchModel">TvChannel specification attribute search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>TvChannel specification attribute search model</returns>
        protected virtual TvChannelSpecificationAttributeSearchModel PrepareTvChannelSpecificationAttributeSearchModel(
            TvChannelSpecificationAttributeSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            searchModel.TvChannelId = tvChannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare tvChannel search model
        /// </summary>
        /// <param name="searchModel">TvChannel search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel search model
        /// </returns>
        public virtual async Task<TvChannelSearchModel> PrepareTvChannelSearchModelAsync(TvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his tvChannels
            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
            searchModel.AllowVendorsToImportTvChannels = _vendorSettings.AllowVendorsToImportTvChannels;

            var licenseCheckModel = new LicenseCheckModel();
            try
            {
                var result = await _nopHttpClient.GetLicenseCheckDetailsAsync();
                if (!string.IsNullOrEmpty(result))
                {
                    licenseCheckModel = JsonConvert.DeserializeObject<LicenseCheckModel>(result);
                    if (licenseCheckModel.DisplayWarning == false && licenseCheckModel.BlockPages == false)
                        await _settingService.SetSettingAsync($"{nameof(AdminAreaSettings)}.{nameof(AdminAreaSettings.CheckLicense)}", false);
                }
            }
            catch { }
            searchModel.LicenseCheckModel = licenseCheckModel;

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available tvChannel types
            await _baseAdminModelFactory.PrepareTvChannelTypesAsync(searchModel.AvailableTvChannelTypes);

            //prepare available warehouses
            await _baseAdminModelFactory.PrepareWarehousesAsync(searchModel.AvailableWarehouses);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.List.SearchPublished.All")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.List.SearchPublished.PublishedOnly")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.List.SearchPublished.UnpublishedOnly")
            });

            //prepare grid
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged tvChannel list model
        /// </summary>
        /// <param name="searchModel">TvChannel search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel list model
        /// </returns>
        public virtual async Task<TvChannelListModel> PrepareTvChannelListModelAsync(TvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter comments
            var overridePublished = searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1);
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;
            var categoryIds = new List<int> { searchModel.SearchCategoryId };
            if (searchModel.SearchIncludeSubCategories && searchModel.SearchCategoryId > 0)
            {
                var childCategoryIds = await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: searchModel.SearchCategoryId, showHidden: true);
                categoryIds.AddRange(childCategoryIds);
            }

            //get tvChannels
            var tvChannels = await _tvChannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: categoryIds,
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                warehouseId: searchModel.SearchWarehouseId,
                tvChannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
                overridePublished: overridePublished);

            //prepare list model
            var model = await new TvChannelListModel().PrepareToGridAsync(searchModel, tvChannels, () =>
            {
                return tvChannels.SelectAwait(async tvChannel =>
                {
                    //fill in model values from the entity
                    var tvChannelModel = tvChannel.ToModel<TvChannelModel>();

                    //little performance optimization: ensure that "FullDescription" is not returned
                    tvChannelModel.FullDescription = string.Empty;

                    //fill in additional values (not existing in the entity)
                    tvChannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvChannel, 0, true, false);
                    var defaultTvChannelPicture = (await _pictureService.GetPicturesByTvChannelIdAsync(tvChannel.Id, 1)).FirstOrDefault();
                    (tvChannelModel.PictureThumbnailUrl, _) = await _pictureService.GetPictureUrlAsync(defaultTvChannelPicture, 75);
                    tvChannelModel.TvChannelTypeName = await _localizationService.GetLocalizedEnumAsync(tvChannel.TvChannelType);
                    if (tvChannel.TvChannelType == TvChannelType.SimpleTvChannel && tvChannel.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                        tvChannelModel.StockQuantityStr = (await _tvChannelService.GetTotalStockQuantityAsync(tvChannel)).ToString();

                    return tvChannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvChannel model
        /// </summary>
        /// <param name="model">TvChannel model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel model
        /// </returns>
        public virtual async Task<TvChannelModel> PrepareTvChannelModelAsync(TvChannelModel model, TvChannel tvChannel, bool excludeProperties = false)
        {
            Func<TvChannelLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (tvChannel != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = tvChannel.ToModel<TvChannelModel>();
                    model.SeName = await _urlRecordService.GetSeNameAsync(tvChannel, 0, true, false);
                }

                var parentGroupedTvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannel.ParentGroupedTvChannelId);
                if (parentGroupedTvChannel != null)
                {
                    model.AssociatedToTvChannelId = tvChannel.ParentGroupedTvChannelId;
                    model.AssociatedToTvChannelName = parentGroupedTvChannel.Name;
                }

                model.LastStockQuantity = tvChannel.StockQuantity;
                model.TvChannelTags = string.Join(", ", (await _tvChannelTagService.GetAllTvChannelTagsByTvChannelIdAsync(tvChannel.Id)).Select(tag => tag.Name));
                model.TvChannelAttributesExist = (await _tvChannelAttributeService.GetAllTvChannelAttributesAsync()).Any();

                model.CanCreateCombinations = await (await _tvChannelAttributeService
                    .GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id)).AnyAwaitAsync(async pam => (await _tvChannelAttributeService.GetTvChannelAttributeValuesAsync(pam.Id)).Any());

                if (!excludeProperties)
                {
                    model.SelectedCategoryIds = (await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(tvChannel.Id, true))
                        .Select(tvChannelCategory => tvChannelCategory.CategoryId).ToList();
                    model.SelectedManufacturerIds = (await _manufacturerService.GetTvChannelManufacturersByTvChannelIdAsync(tvChannel.Id, true))
                        .Select(tvChannelManufacturer => tvChannelManufacturer.ManufacturerId).ToList();
                }

                //prepare copy tvChannel model
                await PrepareCopyTvChannelModelAsync(model.CopyTvChannelModel, tvChannel);

                //prepare nested search model
                PrepareRelatedTvChannelSearchModel(model.RelatedTvChannelSearchModel, tvChannel);
                PrepareCrossSellTvChannelSearchModel(model.CrossSellTvChannelSearchModel, tvChannel);
                PrepareAssociatedTvChannelSearchModel(model.AssociatedTvChannelSearchModel, tvChannel);
                PrepareTvChannelPictureSearchModel(model.TvChannelPictureSearchModel, tvChannel);
                PrepareTvChannelVideoSearchModel(model.TvChannelVideoSearchModel, tvChannel);
                PrepareTvChannelSpecificationAttributeSearchModel(model.TvChannelSpecificationAttributeSearchModel, tvChannel);
                PrepareTvChannelOrderSearchModel(model.TvChannelOrderSearchModel, tvChannel);
                PrepareTierPriceSearchModel(model.TierPriceSearchModel, tvChannel);
                await PrepareStockQuantityHistorySearchModelAsync(model.StockQuantityHistorySearchModel, tvChannel);
                PrepareTvChannelAttributeMappingSearchModel(model.TvChannelAttributeMappingSearchModel, tvChannel);
                PrepareTvChannelAttributeCombinationSearchModel(model.TvChannelAttributeCombinationSearchModel, tvChannel);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(tvChannel, entity => entity.Name, languageId, false, false);
                    locale.FullDescription = await _localizationService.GetLocalizedAsync(tvChannel, entity => entity.FullDescription, languageId, false, false);
                    locale.ShortDescription = await _localizationService.GetLocalizedAsync(tvChannel, entity => entity.ShortDescription, languageId, false, false);
                    locale.MetaKeywords = await _localizationService.GetLocalizedAsync(tvChannel, entity => entity.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = await _localizationService.GetLocalizedAsync(tvChannel, entity => entity.MetaDescription, languageId, false, false);
                    locale.MetaTitle = await _localizationService.GetLocalizedAsync(tvChannel, entity => entity.MetaTitle, languageId, false, false);
                    locale.SeName = await _urlRecordService.GetSeNameAsync(tvChannel, languageId, false, false);
                };
            }

            //set default values for the new model
            if (tvChannel == null)
            {
                model.MaximumUserEnteredPrice = 1000;
                model.MaxNumberOfDownloads = 10;
                model.RecurringCycleLength = 100;
                model.RecurringTotalCycles = 10;
                model.RentalPriceLength = 1;
                model.StockQuantity = 10000;
                model.NotifyAdminForQuantityBelow = 1;
                model.OrderMinimumQuantity = 1;
                model.OrderMaximumQuantity = 10000;
                model.TaxCategoryId = _taxSettings.DefaultTaxCategoryId;
                model.UnlimitedDownloads = true;
                model.IsShipEnabled = true;
                model.AllowUserReviews = true;
                model.Published = true;
                model.VisibleIndividually = true;
            }

            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
            model.BaseWeightIn = (await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId)).Name;
            model.BaseDimensionIn = (await _measureService.GetMeasureDimensionByIdAsync(_measureSettings.BaseDimensionId)).Name;
            model.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
            model.HasAvailableSpecificationAttributes =
                (await _specificationAttributeService.GetSpecificationAttributesWithOptionsAsync()).Any();

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare editor settings
            model.TvChannelEditorSettingsModel = await _settingModelFactory.PrepareTvChannelEditorSettingsModelAsync();

            //prepare available tvChannel templates
            await _baseAdminModelFactory.PrepareTvChannelTemplatesAsync(model.AvailableTvChannelTemplates, false);

            //prepare available tvChannel types
            var tvChannelTemplates = await _tvChannelTemplateService.GetAllTvChannelTemplatesAsync();
            foreach (var tvChannelType in Enum.GetValues(typeof(TvChannelType)).OfType<TvChannelType>())
            {
                model.TvChannelsTypesSupportedByTvChannelTemplates.Add((int)tvChannelType, new List<SelectListItem>());
                foreach (var template in tvChannelTemplates)
                {
                    var list = (IList<int>)TypeDescriptor.GetConverter(typeof(List<int>)).ConvertFrom(template.IgnoredTvChannelTypes) ?? new List<int>();
                    if (string.IsNullOrEmpty(template.IgnoredTvChannelTypes) || !list.Contains((int)tvChannelType))
                    {
                        model.TvChannelsTypesSupportedByTvChannelTemplates[(int)tvChannelType].Add(new SelectListItem
                        {
                            Text = template.Name,
                            Value = template.Id.ToString()
                        });
                    }
                }
            }

            //prepare available delivery dates
            await _baseAdminModelFactory.PrepareDeliveryDatesAsync(model.AvailableDeliveryDates,
                defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Fields.DeliveryDate.None"));

            //prepare available tvChannel availability ranges
            await _baseAdminModelFactory.PrepareTvChannelAvailabilityRangesAsync(model.AvailableTvChannelAvailabilityRanges,
                defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Fields.TvChannelAvailabilityRange.None"));

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(model.AvailableVendors,
                defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Fields.Vendor.None"));

            //prepare available tax categories
            await _baseAdminModelFactory.PrepareTaxCategoriesAsync(model.AvailableTaxCategories);

            //prepare available warehouses
            await _baseAdminModelFactory.PrepareWarehousesAsync(model.AvailableWarehouses,
                defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Fields.Warehouse.None"));
            await PrepareTvChannelWarehouseInventoryModelsAsync(model.TvChannelWarehouseInventoryModels, tvChannel);

            //prepare available base price units
            var availableMeasureWeights = (await _measureService.GetAllMeasureWeightsAsync())
                .Select(weight => new SelectListItem { Text = weight.Name, Value = weight.Id.ToString() }).ToList();
            model.AvailableBasepriceUnits = availableMeasureWeights;
            model.AvailableBasepriceBaseUnits = availableMeasureWeights;

            //prepare model categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(model.AvailableCategories, false);
            foreach (var categoryItem in model.AvailableCategories)
            {
                categoryItem.Selected = int.TryParse(categoryItem.Value, out var categoryId)
                    && model.SelectedCategoryIds.Contains(categoryId);
            }

            //prepare model manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(model.AvailableManufacturers, false);
            foreach (var manufacturerItem in model.AvailableManufacturers)
            {
                manufacturerItem.Selected = int.TryParse(manufacturerItem.Value, out var manufacturerId)
                    && model.SelectedManufacturerIds.Contains(manufacturerId);
            }

            //prepare model discounts
            var availableDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToSkus, showHidden: true, isActive: null);
            await _discountSupportedModelFactory.PrepareModelDiscountsAsync(model, tvChannel, availableDiscounts, excludeProperties);

            //prepare model user roles
            await _aclSupportedModelFactory.PrepareModelUserRolesAsync(model, tvChannel, excludeProperties);

            //prepare model stores
            await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, tvChannel, excludeProperties);

            var tvChannelTags = await _tvChannelTagService.GetAllTvChannelTagsAsync();
            var tvChannelTagsSb = new StringBuilder();
            tvChannelTagsSb.Append("var initialTvChannelTags = [");
            for (var i = 0; i < tvChannelTags.Count; i++)
            {
                var tag = tvChannelTags[i];
                tvChannelTagsSb.Append('\'');
                tvChannelTagsSb.Append(JavaScriptEncoder.Default.Encode(tag.Name));
                tvChannelTagsSb.Append('\'');
                if (i != tvChannelTags.Count - 1)
                    tvChannelTagsSb.Append(',');
            }
            tvChannelTagsSb.Append(']');

            model.InitialTvChannelTags = tvChannelTagsSb.ToString();

            return model;
        }

        /// <summary>
        /// Prepare required tvChannel search model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Required tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the required tvChannel search model to add to the tvChannel
        /// </returns>
        public virtual async Task<AddRequiredTvChannelSearchModel> PrepareAddRequiredTvChannelSearchModelAsync(AddRequiredTvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available tvChannel types
            await _baseAdminModelFactory.PrepareTvChannelTypesAsync(searchModel.AvailableTvChannelTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare required tvChannel list model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Required tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the required tvChannel list model to add to the tvChannel
        /// </returns>
        public virtual async Task<AddRequiredTvChannelListModel> PrepareAddRequiredTvChannelListModelAsync(AddRequiredTvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;

            //get tvChannels
            var tvChannels = await _tvChannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                tvChannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddRequiredTvChannelListModel().PrepareToGridAsync(searchModel, tvChannels, () =>
            {
                return tvChannels.SelectAwait(async tvChannel =>
                {
                    var tvChannelModel = tvChannel.ToModel<TvChannelModel>();

                    tvChannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvChannel, 0, true, false);

                    return tvChannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged related tvChannel list model
        /// </summary>
        /// <param name="searchModel">Related tvChannel search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvChannel list model
        /// </returns>
        public virtual async Task<RelatedTvChannelListModel> PrepareRelatedTvChannelListModelAsync(RelatedTvChannelSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            //get related tvChannels
            var relatedTvChannels = (await _tvChannelService
                .GetRelatedTvChannelsByTvChannelId1Async(tvChannelId1: tvChannel.Id, showHidden: true)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new RelatedTvChannelListModel().PrepareToGridAsync(searchModel, relatedTvChannels, () =>
            {
                return relatedTvChannels.SelectAwait(async relatedTvChannel =>
                {
                    //fill in model values from the entity
                    var relatedTvChannelModel = relatedTvChannel.ToModel<RelatedTvChannelModel>();

                    //fill in additional values (not existing in the entity)
                    relatedTvChannelModel.TvChannel2Name = (await _tvChannelService.GetTvChannelByIdAsync(relatedTvChannel.TvChannelId2))?.Name;

                    return relatedTvChannelModel;
                });
            });
            return model;
        }

        /// <summary>
        /// Prepare related tvChannel search model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Related tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvChannel search model to add to the tvChannel
        /// </returns>
        public virtual async Task<AddRelatedTvChannelSearchModel> PrepareAddRelatedTvChannelSearchModelAsync(AddRelatedTvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available tvChannel types
            await _baseAdminModelFactory.PrepareTvChannelTypesAsync(searchModel.AvailableTvChannelTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged related tvChannel list model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Related tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvChannel list model to add to the tvChannel
        /// </returns>
        public virtual async Task<AddRelatedTvChannelListModel> PrepareAddRelatedTvChannelListModelAsync(AddRelatedTvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;

            //get tvChannels
            var tvChannels = await _tvChannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                tvChannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddRelatedTvChannelListModel().PrepareToGridAsync(searchModel, tvChannels, () =>
            {
                return tvChannels.SelectAwait(async tvChannel =>
                {
                    var tvChannelModel = tvChannel.ToModel<TvChannelModel>();

                    tvChannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvChannel, 0, true, false);

                    return tvChannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged cross-sell tvChannel list model
        /// </summary>
        /// <param name="searchModel">Cross-sell tvChannel search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvChannel list model
        /// </returns>
        public virtual async Task<CrossSellTvChannelListModel> PrepareCrossSellTvChannelListModelAsync(CrossSellTvChannelSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            //get cross-sell tvChannels
            var crossSellTvChannels = (await _tvChannelService
                .GetCrossSellTvChannelsByTvChannelId1Async(tvChannelId1: tvChannel.Id, showHidden: true)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new CrossSellTvChannelListModel().PrepareToGridAsync(searchModel, crossSellTvChannels, () =>
            {
                return crossSellTvChannels.SelectAwait(async crossSellTvChannel =>
                {
                    //fill in model values from the entity
                    var crossSellTvChannelModel = new CrossSellTvChannelModel
                    {
                        Id = crossSellTvChannel.Id,
                        TvChannelId2 = crossSellTvChannel.TvChannelId2
                    };

                    //fill in additional values (not existing in the entity)
                    crossSellTvChannelModel.TvChannel2Name = (await _tvChannelService.GetTvChannelByIdAsync(crossSellTvChannel.TvChannelId2))?.Name;

                    return crossSellTvChannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare cross-sell tvChannel search model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Cross-sell tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvChannel search model to add to the tvChannel
        /// </returns>
        public virtual async Task<AddCrossSellTvChannelSearchModel> PrepareAddCrossSellTvChannelSearchModelAsync(AddCrossSellTvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available tvChannel types
            await _baseAdminModelFactory.PrepareTvChannelTypesAsync(searchModel.AvailableTvChannelTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged crossSell tvChannel list model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">CrossSell tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the crossSell tvChannel list model to add to the tvChannel
        /// </returns>
        public virtual async Task<AddCrossSellTvChannelListModel> PrepareAddCrossSellTvChannelListModelAsync(AddCrossSellTvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;

            //get tvChannels
            var tvChannels = await _tvChannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                tvChannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddCrossSellTvChannelListModel().PrepareToGridAsync(searchModel, tvChannels, () =>
            {
                return tvChannels.SelectAwait(async tvChannel =>
                {
                    var tvChannelModel = tvChannel.ToModel<TvChannelModel>();

                    tvChannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvChannel, 0, true, false);

                    return tvChannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged associated tvChannel list model
        /// </summary>
        /// <param name="searchModel">Associated tvChannel search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the associated tvChannel list model
        /// </returns>
        public virtual async Task<AssociatedTvChannelListModel> PrepareAssociatedTvChannelListModelAsync(AssociatedTvChannelSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var vendor = await _workContext.GetCurrentVendorAsync();
            //get associated tvChannels
            var associatedTvChannels = (await _tvChannelService.GetAssociatedTvChannelsAsync(showHidden: true,
                parentGroupedTvChannelId: tvChannel.Id,
                vendorId: vendor?.Id ?? 0)).ToPagedList(searchModel);

            //prepare grid model
            var model = new AssociatedTvChannelListModel().PrepareToGrid(searchModel, associatedTvChannels, () =>
            {
                return associatedTvChannels.Select(associatedTvChannel =>
                {
                    var associatedTvChannelModel = associatedTvChannel.ToModel<AssociatedTvChannelModel>();
                    associatedTvChannelModel.TvChannelName = associatedTvChannel.Name;

                    return associatedTvChannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare associated tvChannel search model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Associated tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the associated tvChannel search model to add to the tvChannel
        /// </returns>
        public virtual async Task<AddAssociatedTvChannelSearchModel> PrepareAddAssociatedTvChannelSearchModelAsync(AddAssociatedTvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available tvChannel types
            await _baseAdminModelFactory.PrepareTvChannelTypesAsync(searchModel.AvailableTvChannelTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged associated tvChannel list model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Associated tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the associated tvChannel list model to add to the tvChannel
        /// </returns>
        public virtual async Task<AddAssociatedTvChannelListModel> PrepareAddAssociatedTvChannelListModelAsync(AddAssociatedTvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;

            //get tvChannels
            var tvChannels = await _tvChannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                tvChannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddAssociatedTvChannelListModel().PrepareToGridAsync(searchModel, tvChannels, () =>
            {
                return tvChannels.SelectAwait(async tvChannel =>
                {
                    //fill in model values from the entity
                    var tvChannelModel = tvChannel.ToModel<TvChannelModel>();

                    //fill in additional values (not existing in the entity)
                    tvChannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvChannel, 0, true, false);
                    var parentGroupedTvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannel.ParentGroupedTvChannelId);

                    if (parentGroupedTvChannel == null)
                        return tvChannelModel;

                    tvChannelModel.AssociatedToTvChannelId = tvChannel.ParentGroupedTvChannelId;
                    tvChannelModel.AssociatedToTvChannelName = parentGroupedTvChannel.Name;

                    return tvChannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged tvChannel picture list model
        /// </summary>
        /// <param name="searchModel">TvChannel picture search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel picture list model
        /// </returns>
        public virtual async Task<TvChannelPictureListModel> PrepareTvChannelPictureListModelAsync(TvChannelPictureSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            //get tvChannel pictures
            var tvChannelPictures = (await _tvChannelService.GetTvChannelPicturesByTvChannelIdAsync(tvChannel.Id)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new TvChannelPictureListModel().PrepareToGridAsync(searchModel, tvChannelPictures, () =>
            {
                return tvChannelPictures.SelectAwait(async tvChannelPicture =>
                {
                    //fill in model values from the entity
                    var tvChannelPictureModel = tvChannelPicture.ToModel<TvChannelPictureModel>();

                    //fill in additional values (not existing in the entity)
                    var picture = (await _pictureService.GetPictureByIdAsync(tvChannelPicture.PictureId))
                        ?? throw new Exception("Picture cannot be loaded");

                    tvChannelPictureModel.PictureUrl = (await _pictureService.GetPictureUrlAsync(picture)).Url;

                    tvChannelPictureModel.OverrideAltAttribute = picture.AltAttribute;
                    tvChannelPictureModel.OverrideTitleAttribute = picture.TitleAttribute;

                    return tvChannelPictureModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged tvChannel video list model
        /// </summary>
        /// <param name="searchModel">TvChannel video search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel video list model
        /// </returns>
        public virtual async Task<TvChannelVideoListModel> PrepareTvChannelVideoListModelAsync(TvChannelVideoSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            //get tvChannel videos
            var tvChannelVideos = (await _tvChannelService.GetTvChannelVideosByTvChannelIdAsync(tvChannel.Id)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new TvChannelVideoListModel().PrepareToGridAsync(searchModel, tvChannelVideos, () =>
            {
                return tvChannelVideos.SelectAwait(async tvChannelVideo =>
                {
                    //fill in model values from the entity
                    var tvChannelVideoModel = tvChannelVideo.ToModel<TvChannelVideoModel>();

                    //fill in additional values (not existing in the entity)
                    var video = (await _videoService.GetVideoByIdAsync(tvChannelVideo.VideoId))
                        ?? throw new Exception("Video cannot be loaded");

                    tvChannelVideoModel.VideoUrl = video.VideoUrl;

                    return tvChannelVideoModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged tvChannel specification attribute list model
        /// </summary>
        /// <param name="searchModel">TvChannel specification attribute search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel specification attribute list model
        /// </returns>
        public virtual async Task<TvChannelSpecificationAttributeListModel> PrepareTvChannelSpecificationAttributeListModelAsync(
            TvChannelSpecificationAttributeSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            //get tvChannel specification attributes
            var tvChannelSpecificationAttributes = (await _specificationAttributeService
                .GetTvChannelSpecificationAttributesAsync(tvChannel.Id)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new TvChannelSpecificationAttributeListModel().PrepareToGridAsync(searchModel, tvChannelSpecificationAttributes, () =>
            {
                return tvChannelSpecificationAttributes.SelectAwait(async attribute =>
                {
                    //fill in model values from the entity
                    var tvChannelSpecificationAttributeModel = attribute.ToModel<TvChannelSpecificationAttributeModel>();

                    var specAttributeOption = await _specificationAttributeService
                        .GetSpecificationAttributeOptionByIdAsync(attribute.SpecificationAttributeOptionId);
                    var specAttribute = await _specificationAttributeService
                        .GetSpecificationAttributeByIdAsync(specAttributeOption.SpecificationAttributeId);

                    //fill in additional values (not existing in the entity)
                    tvChannelSpecificationAttributeModel.AttributeTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeType);

                    tvChannelSpecificationAttributeModel.AttributeId = specAttribute.Id;
                    tvChannelSpecificationAttributeModel.AttributeName = await GetSpecificationAttributeNameAsync(specAttribute);
                    var currentLanguage = await _workContext.GetWorkingLanguageAsync();

                    switch (attribute.AttributeType)
                    {
                        case SpecificationAttributeType.Option:
                            tvChannelSpecificationAttributeModel.ValueRaw = WebUtility.HtmlEncode(specAttributeOption.Name);
                            tvChannelSpecificationAttributeModel.SpecificationAttributeOptionId = specAttributeOption.Id;
                            break;
                        case SpecificationAttributeType.CustomText:
                            tvChannelSpecificationAttributeModel.ValueRaw = WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(attribute, x => x.CustomValue, currentLanguage?.Id));
                            break;
                        case SpecificationAttributeType.CustomHtmlText:
                            tvChannelSpecificationAttributeModel.ValueRaw = await _localizationService
                                .GetLocalizedAsync(attribute, x => x.CustomValue, currentLanguage?.Id);
                            break;
                        case SpecificationAttributeType.Hyperlink:
                            tvChannelSpecificationAttributeModel.ValueRaw = attribute.CustomValue;
                            break;
                    }

                    return tvChannelSpecificationAttributeModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged tvChannel specification attribute model
        /// </summary>
        /// <param name="tvChannelId">TvChannel id</param>
        /// <param name="specificationId">Specification attribute id</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel specification attribute model
        /// </returns>
        public virtual async Task<AddSpecificationAttributeModel> PrepareAddSpecificationAttributeModelAsync(int tvChannelId, int? specificationId)
        {
            if (!specificationId.HasValue)
            {
                return new AddSpecificationAttributeModel
                {
                    AvailableAttributes = await (await _specificationAttributeService.GetSpecificationAttributesWithOptionsAsync())
                        .SelectAwait(async attributeWithOption =>
                        {
                            var attributeName = await GetSpecificationAttributeNameAsync(attributeWithOption);

                            return new SelectListItem(attributeName, attributeWithOption.Id.ToString());
                        }).ToListAsync(),
                    TvChannelId = tvChannelId,
                    Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync<AddSpecificationAttributeLocalizedModel>()
                };
            }

            var attribute = await _specificationAttributeService.GetTvChannelSpecificationAttributeByIdAsync(specificationId.Value);

            if (attribute == null)
            {
                throw new ArgumentException("No specification attribute found with the specified id");
            }

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && (await _tvChannelService.GetTvChannelByIdAsync(attribute.TvChannelId)).VendorId != currentVendor.Id)
                throw new UnauthorizedAccessException("This is not your tvChannel");

            var specAttributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(attribute.SpecificationAttributeOptionId);
            var specAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(specAttributeOption.SpecificationAttributeId);

            var model = attribute.ToModel<AddSpecificationAttributeModel>();
            model.SpecificationId = attribute.Id;
            model.AttributeId = specAttribute.Id;
            model.AttributeTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeType);
            model.AttributeName = specAttribute.Name;

            model.AvailableAttributes = await (await _specificationAttributeService.GetSpecificationAttributesWithOptionsAsync())
                .SelectAwait(async attributeWithOption =>
                {
                    var attributeName = await GetSpecificationAttributeNameAsync(attributeWithOption);

                    return new SelectListItem(attributeName, attributeWithOption.Id.ToString());
                })
                .ToListAsync();

            model.AvailableOptions = (await _specificationAttributeService
                .GetSpecificationAttributeOptionsBySpecificationAttributeAsync(model.AttributeId))
                .Select(option => new SelectListItem { Text = option.Name, Value = option.Id.ToString() })
                .ToList();

            switch (attribute.AttributeType)
            {
                case SpecificationAttributeType.Option:
                    model.ValueRaw = WebUtility.HtmlEncode(specAttributeOption.Name);
                    model.SpecificationAttributeOptionId = specAttributeOption.Id;
                    break;
                case SpecificationAttributeType.CustomText:
                    model.Value = WebUtility.HtmlDecode(attribute.CustomValue);
                    break;
                case SpecificationAttributeType.CustomHtmlText:
                    model.ValueRaw = attribute.CustomValue;
                    break;
                case SpecificationAttributeType.Hyperlink:
                    model.Value = attribute.CustomValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(attribute.AttributeType));
            }
            
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(
                async (AddSpecificationAttributeLocalizedModel locale, int languageId) =>
                {
                    switch (attribute.AttributeType)
                    {
                        case SpecificationAttributeType.CustomHtmlText:
                            locale.ValueRaw = await _localizationService.GetLocalizedAsync(attribute, entity => entity.CustomValue, languageId, false, false);
                            break;
                        case SpecificationAttributeType.CustomText:
                            locale.Value = await _localizationService.GetLocalizedAsync(attribute, entity => entity.CustomValue, languageId, false, false);
                            break;
                        case SpecificationAttributeType.Option:
                            break;
                        case SpecificationAttributeType.Hyperlink:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });

            return model;
        }

        /// <summary>
        /// Prepare tvChannel tag search model
        /// </summary>
        /// <param name="searchModel">TvChannel tag search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tag search model
        /// </returns>
        public virtual Task<TvChannelTagSearchModel> PrepareTvChannelTagSearchModelAsync(TvChannelTagSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged tvChannel tag list model
        /// </summary>
        /// <param name="searchModel">TvChannel tag search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tag list model
        /// </returns>
        public virtual async Task<TvChannelTagListModel> PrepareTvChannelTagListModelAsync(TvChannelTagSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tvChannel tags
            var tvChannelTags = (await (await _tvChannelTagService.GetAllTvChannelTagsAsync(tagName: searchModel.SearchTagName))
                .OrderByDescendingAwait(async tag => await _tvChannelTagService.GetTvChannelCountByTvChannelTagIdAsync(tag.Id, storeId: 0, showHidden: true)).ToListAsync())
                .ToPagedList(searchModel);

            //prepare list model
            var model = await new TvChannelTagListModel().PrepareToGridAsync(searchModel, tvChannelTags, () =>
            {
                return tvChannelTags.SelectAwait(async tag =>
                {
                    //fill in model values from the entity
                    var tvChannelTagModel = tag.ToModel<TvChannelTagModel>();

                    //fill in additional values (not existing in the entity)
                    tvChannelTagModel.TvChannelCount = await _tvChannelTagService.GetTvChannelCountByTvChannelTagIdAsync(tag.Id, storeId: 0, showHidden: true);

                    return tvChannelTagModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvChannel tag model
        /// </summary>
        /// <param name="model">TvChannel tag model</param>
        /// <param name="tvChannelTag">TvChannel tag</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tag model
        /// </returns>
        public virtual async Task<TvChannelTagModel> PrepareTvChannelTagModelAsync(TvChannelTagModel model, TvChannelTag tvChannelTag, bool excludeProperties = false)
        {
            Func<TvChannelTagLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (tvChannelTag != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = tvChannelTag.ToModel<TvChannelTagModel>();
                }

                model.TvChannelCount = await _tvChannelTagService.GetTvChannelCountByTvChannelTagIdAsync(tvChannelTag.Id, storeId: 0, showHidden: true);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(tvChannelTag, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged tvChannel order list model
        /// </summary>
        /// <param name="searchModel">TvChannel order search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel order list model
        /// </returns>
        public virtual async Task<TvChannelOrderListModel> PrepareTvChannelOrderListModelAsync(TvChannelOrderSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            //get orders
            var orders = await _orderService.SearchOrdersAsync(tvChannelId: searchModel.TvChannelId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new TvChannelOrderListModel().PrepareToGridAsync(searchModel, orders, () =>
            {
                return orders.SelectAwait(async order =>
                {
                    var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

                    //fill in model values from the entity
                    var orderModel = new OrderModel
                    {
                        Id = order.Id,
                        UserEmail = billingAddress.Email,
                        CustomOrderNumber = order.CustomOrderNumber
                    };

                    //convert dates to the user time
                    orderModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    orderModel.StoreName = (await _storeService.GetStoreByIdAsync(order.StoreId))?.Name ?? "Deleted";
                    orderModel.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                    orderModel.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
                    orderModel.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);

                    return orderModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged tier price list model
        /// </summary>
        /// <param name="searchModel">Tier price search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ier price list model
        /// </returns>
        public virtual async Task<TierPriceListModel> PrepareTierPriceListModelAsync(TierPriceSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            //get tier prices
            var tierPrices = (await _tvChannelService.GetTierPricesByTvChannelAsync(tvChannel.Id))
                .OrderBy(price => price.StoreId).ThenBy(price => price.Quantity).ThenBy(price => price.UserRoleId)
                .ToList().ToPagedList(searchModel);

            //prepare grid model
            var model = await new TierPriceListModel().PrepareToGridAsync(searchModel, tierPrices, () =>
            {
                return tierPrices.SelectAwait(async price =>
                {
                    //fill in model values from the entity
                    var tierPriceModel = price.ToModel<TierPriceModel>();

                    //fill in additional values (not existing in the entity)   
                    tierPriceModel.Store = price.StoreId > 0
                        ? ((await _storeService.GetStoreByIdAsync(price.StoreId))?.Name ?? "Deleted")
                        : await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TierPrices.Fields.Store.All");
                    tierPriceModel.UserRoleId = price.UserRoleId ?? 0;
                    tierPriceModel.UserRole = price.UserRoleId.HasValue
                        ? (await _userService.GetUserRoleByIdAsync(price.UserRoleId.Value))?.Name
                        : await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.TierPrices.Fields.UserRole.All");

                    return tierPriceModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tier price model
        /// </summary>
        /// <param name="model">Tier price model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tierPrice">Tier price</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ier price model
        /// </returns>
        public virtual async Task<TierPriceModel> PrepareTierPriceModelAsync(TierPriceModel model,
            TvChannel tvChannel, TierPrice tierPrice, bool excludeProperties = false)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (tierPrice != null)
            {
                //fill in model values from the entity
                if (model == null)
                    model = tierPrice.ToModel<TierPriceModel>();
            }

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);

            //prepare available user roles
            await _baseAdminModelFactory.PrepareUserRolesAsync(model.AvailableUserRoles);

            return model;
        }

        /// <summary>
        /// Prepare paged stock quantity history list model
        /// </summary>
        /// <param name="searchModel">Stock quantity history search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the stock quantity history list model
        /// </returns>
        public virtual async Task<StockQuantityHistoryListModel> PrepareStockQuantityHistoryListModelAsync(StockQuantityHistorySearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            //get stock quantity history
            var stockQuantityHistory = await _tvChannelService.GetStockQuantityHistoryAsync(tvChannel: tvChannel,
                warehouseId: searchModel.WarehouseId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var currentUser = await _workContext.GetCurrentUserAsync();
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            
            //prepare grid model
            var model = await new StockQuantityHistoryListModel().PrepareToGridAsync(searchModel, stockQuantityHistory, () =>
            {
                return stockQuantityHistory.SelectAwait(async historyEntry =>
                {
                    //fill in model values from the entity
                    var stockQuantityHistoryModel = historyEntry.ToModel<StockQuantityHistoryModel>();

                    //convert dates to the user time
                    stockQuantityHistoryModel.CreatedOn =
                        await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    var combination = await _tvChannelAttributeService.GetTvChannelAttributeCombinationByIdAsync(historyEntry.CombinationId ?? 0);
                    if (combination != null)
                    {
                        stockQuantityHistoryModel.AttributeCombination = await _tvChannelAttributeFormatter
                            .FormatAttributesAsync(tvChannel, combination.AttributesXml, currentUser, currentStore, renderGiftCardAttributes: false);
                    }

                    stockQuantityHistoryModel.WarehouseName = historyEntry.WarehouseId.HasValue
                        ? (await _shippingService.GetWarehouseByIdAsync(historyEntry.WarehouseId.Value))?.Name ?? "Deleted"
                        : await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Fields.Warehouse.None");

                    return stockQuantityHistoryModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged tvChannel attribute mapping list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute mapping search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute mapping list model
        /// </returns>
        public virtual async Task<TvChannelAttributeMappingListModel> PrepareTvChannelAttributeMappingListModelAsync(TvChannelAttributeMappingSearchModel searchModel,
            TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            //get tvChannel attribute mappings
            var tvChannelAttributeMappings = (await _tvChannelAttributeService
                .GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new TvChannelAttributeMappingListModel().PrepareToGridAsync(searchModel, tvChannelAttributeMappings, () =>
            {
                return tvChannelAttributeMappings.SelectAwait(async attributeMapping =>
                {
                    //fill in model values from the entity
                    var tvChannelAttributeMappingModel = attributeMapping.ToModel<TvChannelAttributeMappingModel>();

                    //fill in additional values (not existing in the entity)
                    tvChannelAttributeMappingModel.ConditionString = string.Empty;

                    tvChannelAttributeMappingModel.ValidationRulesString = await PrepareTvChannelAttributeMappingValidationRulesStringAsync(attributeMapping);
                    tvChannelAttributeMappingModel.TvChannelAttribute = (await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(attributeMapping.TvChannelAttributeId))?.Name;
                    tvChannelAttributeMappingModel.AttributeControlType = await _localizationService.GetLocalizedEnumAsync(attributeMapping.AttributeControlType);
                    var conditionAttribute = (await _tvChannelAttributeParser
                        .ParseTvChannelAttributeMappingsAsync(attributeMapping.ConditionAttributeXml))
                        .FirstOrDefault();
                    if (conditionAttribute == null)
                        return tvChannelAttributeMappingModel;

                    var conditionValue = (await _tvChannelAttributeParser
                        .ParseTvChannelAttributeValuesAsync(attributeMapping.ConditionAttributeXml))
                        .FirstOrDefault();
                    if (conditionValue != null)
                    {
                        tvChannelAttributeMappingModel.ConditionString =
                            $"{WebUtility.HtmlEncode((await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(conditionAttribute.TvChannelAttributeId)).Name)}: {WebUtility.HtmlEncode(conditionValue.Name)}";
                    }

                    return tvChannelAttributeMappingModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvChannel attribute mapping model
        /// </summary>
        /// <param name="model">TvChannel attribute mapping model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute mapping model
        /// </returns>
        public virtual async Task<TvChannelAttributeMappingModel> PrepareTvChannelAttributeMappingModelAsync(TvChannelAttributeMappingModel model,
            TvChannel tvChannel, TvChannelAttributeMapping tvChannelAttributeMapping, bool excludeProperties = false)
        {
            Func<TvChannelAttributeMappingLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (tvChannelAttributeMapping != null)
            {
                //fill in model values from the entity
                model ??= new TvChannelAttributeMappingModel
                {
                    Id = tvChannelAttributeMapping.Id
                };

                model.TvChannelAttribute = (await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(tvChannelAttributeMapping.TvChannelAttributeId)).Name;
                model.AttributeControlType = await _localizationService.GetLocalizedEnumAsync(tvChannelAttributeMapping.AttributeControlType);

                if (!excludeProperties)
                {
                    model.TvChannelAttributeId = tvChannelAttributeMapping.TvChannelAttributeId;
                    model.TextPrompt = tvChannelAttributeMapping.TextPrompt;
                    model.IsRequired = tvChannelAttributeMapping.IsRequired;
                    model.AttributeControlTypeId = tvChannelAttributeMapping.AttributeControlTypeId;
                    model.DisplayOrder = tvChannelAttributeMapping.DisplayOrder;
                    model.ValidationMinLength = tvChannelAttributeMapping.ValidationMinLength;
                    model.ValidationMaxLength = tvChannelAttributeMapping.ValidationMaxLength;
                    model.ValidationFileAllowedExtensions = tvChannelAttributeMapping.ValidationFileAllowedExtensions;
                    model.ValidationFileMaximumSize = tvChannelAttributeMapping.ValidationFileMaximumSize;
                    model.DefaultValue = tvChannelAttributeMapping.DefaultValue;
                }

                //prepare condition attributes model
                model.ConditionAllowed = true;
                await PrepareTvChannelAttributeConditionModelAsync(model.ConditionModel, tvChannelAttributeMapping);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.TextPrompt = await _localizationService.GetLocalizedAsync(tvChannelAttributeMapping, entity => entity.TextPrompt, languageId, false, false);
                    locale.DefaultValue = await _localizationService.GetLocalizedAsync(tvChannelAttributeMapping, entity => entity.DefaultValue, languageId, false, false);
                };

                //prepare nested search model
                PrepareTvChannelAttributeValueSearchModel(model.TvChannelAttributeValueSearchModel, tvChannelAttributeMapping);
            }

            model.TvChannelId = tvChannel.Id;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare available tvChannel attributes
            model.AvailableTvChannelAttributes = (await _tvChannelAttributeService.GetAllTvChannelAttributesAsync()).Select(tvChannelAttribute => new SelectListItem
            {
                Text = tvChannelAttribute.Name,
                Value = tvChannelAttribute.Id.ToString()
            }).ToList();

            return model;
        }

        /// <summary>
        /// Prepare paged tvChannel attribute value list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute value search model</param>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute value list model
        /// </returns>
        public virtual async Task<TvChannelAttributeValueListModel> PrepareTvChannelAttributeValueListModelAsync(TvChannelAttributeValueSearchModel searchModel,
            TvChannelAttributeMapping tvChannelAttributeMapping)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannelAttributeMapping == null)
                throw new ArgumentNullException(nameof(tvChannelAttributeMapping));

            //get tvChannel attribute values
            var tvChannelAttributeValues = (await _tvChannelAttributeService
                .GetTvChannelAttributeValuesAsync(tvChannelAttributeMapping.Id)).ToPagedList(searchModel);

            //prepare list model
            var model = await new TvChannelAttributeValueListModel().PrepareToGridAsync(searchModel, tvChannelAttributeValues, () =>
            {
                return tvChannelAttributeValues.SelectAwait(async value =>
                {
                    //fill in model values from the entity
                    var tvChannelAttributeValueModel = value.ToModel<TvChannelAttributeValueModel>();

                    //fill in additional values (not existing in the entity)
                    tvChannelAttributeValueModel.AttributeValueTypeName = await _localizationService.GetLocalizedEnumAsync(value.AttributeValueType);

                    tvChannelAttributeValueModel.Name = tvChannelAttributeMapping.AttributeControlType != AttributeControlType.ColorSquares
                        ? value.Name : $"{value.Name} - {value.ColorSquaresRgb}";
                    if (value.AttributeValueType == AttributeValueType.Simple)
                    {
                        tvChannelAttributeValueModel.PriceAdjustmentStr = value.PriceAdjustment.ToString("G29");
                        if (value.PriceAdjustmentUsePercentage)
                            tvChannelAttributeValueModel.PriceAdjustmentStr += " %";
                        tvChannelAttributeValueModel.WeightAdjustmentStr = value.WeightAdjustment.ToString("G29");
                    }

                    if (value.AttributeValueType == AttributeValueType.AssociatedToTvChannel)
                    {
                        tvChannelAttributeValueModel.AssociatedTvChannelName = (await _tvChannelService.GetTvChannelByIdAsync(value.AssociatedTvChannelId))?.Name ?? string.Empty;
                    }

                    var pictureThumbnailUrl = await _pictureService.GetPictureUrlAsync(value.PictureId, 75, false);
                    //little hack here. Grid is rendered wrong way with <img> without "src" attribute
                    if (string.IsNullOrEmpty(pictureThumbnailUrl))
                        pictureThumbnailUrl = await _pictureService.GetDefaultPictureUrlAsync(targetSize: 1);

                    tvChannelAttributeValueModel.PictureThumbnailUrl = pictureThumbnailUrl;

                    return tvChannelAttributeValueModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvChannel attribute value model
        /// </summary>
        /// <param name="model">TvChannel attribute value model</param>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <param name="tvChannelAttributeValue">TvChannel attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute value model
        /// </returns>
        public virtual async Task<TvChannelAttributeValueModel> PrepareTvChannelAttributeValueModelAsync(TvChannelAttributeValueModel model,
            TvChannelAttributeMapping tvChannelAttributeMapping, TvChannelAttributeValue tvChannelAttributeValue, bool excludeProperties = false)
        {
            if (tvChannelAttributeMapping == null)
                throw new ArgumentNullException(nameof(tvChannelAttributeMapping));

            Func<TvChannelAttributeValueLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (tvChannelAttributeValue != null)
            {
                //fill in model values from the entity
                model ??= new TvChannelAttributeValueModel
                {
                    TvChannelAttributeMappingId = tvChannelAttributeValue.TvChannelAttributeMappingId,
                    AttributeValueTypeId = tvChannelAttributeValue.AttributeValueTypeId,
                    AttributeValueTypeName = await _localizationService.GetLocalizedEnumAsync(tvChannelAttributeValue.AttributeValueType),
                    AssociatedTvChannelId = tvChannelAttributeValue.AssociatedTvChannelId,
                    Name = tvChannelAttributeValue.Name,
                    ColorSquaresRgb = tvChannelAttributeValue.ColorSquaresRgb,
                    DisplayColorSquaresRgb = tvChannelAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares,
                    ImageSquaresPictureId = tvChannelAttributeValue.ImageSquaresPictureId,
                    DisplayImageSquaresPicture = tvChannelAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares,
                    PriceAdjustment = tvChannelAttributeValue.PriceAdjustment,
                    PriceAdjustmentUsePercentage = tvChannelAttributeValue.PriceAdjustmentUsePercentage,
                    WeightAdjustment = tvChannelAttributeValue.WeightAdjustment,
                    Cost = tvChannelAttributeValue.Cost,
                    UserEntersQty = tvChannelAttributeValue.UserEntersQty,
                    Quantity = tvChannelAttributeValue.Quantity,
                    IsPreSelected = tvChannelAttributeValue.IsPreSelected,
                    DisplayOrder = tvChannelAttributeValue.DisplayOrder,
                    PictureId = tvChannelAttributeValue.PictureId
                };

                model.AssociatedTvChannelName = (await _tvChannelService.GetTvChannelByIdAsync(tvChannelAttributeValue.AssociatedTvChannelId))?.Name;

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(tvChannelAttributeValue, entity => entity.Name, languageId, false, false);
                };
            }

            model.TvChannelAttributeMappingId = tvChannelAttributeMapping.Id;
            model.DisplayColorSquaresRgb = tvChannelAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares;
            model.DisplayImageSquaresPicture = tvChannelAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares;

            //set default values for the new model
            if (tvChannelAttributeValue == null)
                model.Quantity = 1;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare picture models
            var tvChannelPictures = await _tvChannelService.GetTvChannelPicturesByTvChannelIdAsync(tvChannelAttributeMapping.TvChannelId);
            model.TvChannelPictureModels = await tvChannelPictures.SelectAwait(async tvChannelPicture => new TvChannelPictureModel
            {
                Id = tvChannelPicture.Id,
                TvChannelId = tvChannelPicture.TvChannelId,
                PictureId = tvChannelPicture.PictureId,
                PictureUrl = await _pictureService.GetPictureUrlAsync(tvChannelPicture.PictureId),
                DisplayOrder = tvChannelPicture.DisplayOrder
            }).ToListAsync();

            return model;
        }

        /// <summary>
        /// Prepare tvChannel model to associate to the tvChannel attribute value
        /// </summary>
        /// <param name="searchModel">TvChannel model to associate to the tvChannel attribute value</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel model to associate to the tvChannel attribute value
        /// </returns>
        public virtual async Task<AssociateTvChannelToAttributeValueSearchModel> PrepareAssociateTvChannelToAttributeValueSearchModelAsync(
            AssociateTvChannelToAttributeValueSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available tvChannel types
            await _baseAdminModelFactory.PrepareTvChannelTypesAsync(searchModel.AvailableTvChannelTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged tvChannel model to associate to the tvChannel attribute value
        /// </summary>
        /// <param name="searchModel">TvChannel model to associate to the tvChannel attribute value</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel model to associate to the tvChannel attribute value
        /// </returns>
        public virtual async Task<AssociateTvChannelToAttributeValueListModel> PrepareAssociateTvChannelToAttributeValueListModelAsync(
            AssociateTvChannelToAttributeValueSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;

            //get tvChannels
            var tvChannels = await _tvChannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                tvChannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AssociateTvChannelToAttributeValueListModel().PrepareToGridAsync(searchModel, tvChannels, () =>
            {
                //fill in model values from the entity
                return tvChannels.SelectAwait(async tvChannel =>
                {
                    var tvChannelModel = tvChannel.ToModel<TvChannelModel>();

                    tvChannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvChannel, 0, true, false);

                    return tvChannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged tvChannel attribute combination list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute combination search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute combination list model
        /// </returns>
        public virtual async Task<TvChannelAttributeCombinationListModel> PrepareTvChannelAttributeCombinationListModelAsync(
            TvChannelAttributeCombinationSearchModel searchModel, TvChannel tvChannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            //get tvChannel attribute combinations
            var tvChannelAttributeCombinations = (await _tvChannelAttributeService
                .GetAllTvChannelAttributeCombinationsAsync(tvChannel.Id)).ToPagedList(searchModel);

            var currentUser = await _workContext.GetCurrentUserAsync();
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            
            //prepare grid model
            var model = await new TvChannelAttributeCombinationListModel().PrepareToGridAsync(searchModel, tvChannelAttributeCombinations, () =>
            {
                return tvChannelAttributeCombinations.SelectAwait(async combination =>
                {
                    //fill in model values from the entity
                    var tvChannelAttributeCombinationModel = combination.ToModel<TvChannelAttributeCombinationModel>();

                    //fill in additional values (not existing in the entity)
                    tvChannelAttributeCombinationModel.AttributesXml = await _tvChannelAttributeFormatter
                        .FormatAttributesAsync(tvChannel, combination.AttributesXml, currentUser, currentStore, "<br />", true, true, true, false);
                    var pictureThumbnailUrl = await _pictureService.GetPictureUrlAsync(combination.PictureId, 75, false);
                    //little hack here. Grid is rendered wrong way with <img> without "src" attribute
                    if (string.IsNullOrEmpty(pictureThumbnailUrl))
                        pictureThumbnailUrl = await _pictureService.GetDefaultPictureUrlAsync(targetSize: 1);

                    tvChannelAttributeCombinationModel.PictureThumbnailUrl = pictureThumbnailUrl;
                    var warnings = (await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(currentUser,
                        ShoppingCartType.ShoppingCart, tvChannel,
                        attributesXml: combination.AttributesXml,
                        ignoreNonCombinableAttributes: true)
                        ).Aggregate(string.Empty, (message, warning) => $"{message}{warning}<br />");
                    tvChannelAttributeCombinationModel.Warnings = new List<string> { warnings };

                    return tvChannelAttributeCombinationModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvChannel attribute combination model
        /// </summary>
        /// <param name="model">TvChannel attribute combination model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelAttributeCombination">TvChannel attribute combination</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute combination model
        /// </returns>
        public virtual async Task<TvChannelAttributeCombinationModel> PrepareTvChannelAttributeCombinationModelAsync(TvChannelAttributeCombinationModel model,
            TvChannel tvChannel, TvChannelAttributeCombination tvChannelAttributeCombination, bool excludeProperties = false)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (tvChannelAttributeCombination != null)
            {
                //fill in model values from the entity
                model ??= new TvChannelAttributeCombinationModel
                {
                    AllowOutOfStockOrders = tvChannelAttributeCombination.AllowOutOfStockOrders,
                    AttributesXml = tvChannelAttributeCombination.AttributesXml,
                    Gtin = tvChannelAttributeCombination.Gtin,
                    Id = tvChannelAttributeCombination.Id,
                    ManufacturerPartNumber = tvChannelAttributeCombination.ManufacturerPartNumber,
                    NotifyAdminForQuantityBelow = tvChannelAttributeCombination.NotifyAdminForQuantityBelow,
                    OverriddenPrice = tvChannelAttributeCombination.OverriddenPrice,
                    PictureId = tvChannelAttributeCombination.PictureId,
                    TvChannelId = tvChannelAttributeCombination.TvChannelId,
                    Sku = tvChannelAttributeCombination.Sku,
                    StockQuantity = tvChannelAttributeCombination.StockQuantity,
                    MinStockQuantity = tvChannelAttributeCombination.MinStockQuantity
                };
            }

            model.TvChannelId = tvChannel.Id;

            //set default values for the new model
            if (tvChannelAttributeCombination == null)
            {
                model.TvChannelId = tvChannel.Id;
                model.StockQuantity = 10000;
                model.NotifyAdminForQuantityBelow = 1;
            }

            //prepare picture models
            var tvChannelPictures = await _tvChannelService.GetTvChannelPicturesByTvChannelIdAsync(tvChannel.Id);
            model.TvChannelPictureModels = await tvChannelPictures.SelectAwait(async tvChannelPicture => new TvChannelPictureModel
            {
                Id = tvChannelPicture.Id,
                TvChannelId = tvChannelPicture.TvChannelId,
                PictureId = tvChannelPicture.PictureId,
                PictureUrl = await _pictureService.GetPictureUrlAsync(tvChannelPicture.PictureId),
                DisplayOrder = tvChannelPicture.DisplayOrder
            }).ToListAsync();

            //prepare tvChannel attribute mappings (exclude non-combinable attributes)
            var attributes = (await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id))
                .Where(tvChannelAttributeMapping => !tvChannelAttributeMapping.IsNonCombinable()).ToList();

            foreach (var attribute in attributes)
            {
                var attributeModel = new TvChannelAttributeCombinationModel.TvChannelAttributeModel
                {
                    Id = attribute.Id,
                    TvChannelAttributeId = attribute.TvChannelAttributeId,
                    Name = (await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(attribute.TvChannelAttributeId)).Name,
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _tvChannelAttributeService.GetTvChannelAttributeValuesAsync(attribute.Id);
                    var preSelectedValue = _tvChannelAttributeParser.ParseValues(model.AttributesXml, attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new TvChannelAttributeCombinationModel.TvChannelAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = preSelectedValue.Contains(attributeValue.Id.ToString())
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                model.TvChannelAttributes.Add(attributeModel);
            }

            return model;
        }

        #endregion
    }
}