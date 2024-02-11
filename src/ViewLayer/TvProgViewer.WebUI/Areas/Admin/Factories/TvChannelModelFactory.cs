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
    /// Represents the tvchannel model factory implementation
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
        private readonly ITvChannelAttributeFormatter _tvchannelAttributeFormatter;
        private readonly ITvChannelAttributeParser _tvchannelAttributeParser;
        private readonly ITvChannelAttributeService _tvchannelAttributeService;
        private readonly ITvChannelService _tvchannelService;
        private readonly ITvChannelTagService _tvchannelTagService;
        private readonly ITvChannelTemplateService _tvchannelTemplateService;
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
            ITvChannelAttributeFormatter tvchannelAttributeFormatter,
            ITvChannelAttributeParser tvchannelAttributeParser,
            ITvChannelAttributeService tvchannelAttributeService,
            ITvChannelService tvchannelService,
            ITvChannelTagService tvchannelTagService,
            ITvChannelTemplateService tvchannelTemplateService,
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
            _tvchannelAttributeFormatter = tvchannelAttributeFormatter;
            _tvchannelAttributeParser = tvchannelAttributeParser;
            _tvchannelAttributeService = tvchannelAttributeService;
            _tvchannelService = tvchannelService;
            _tvchannelTagService = tvchannelTagService;
            _tvchannelTemplateService = tvchannelTemplateService;
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

        /// <returns>A task that represents the asynchronous operation</returns>
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
        /// Prepare copy tvchannel model
        /// </summary>
        /// <param name="model">Copy tvchannel model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the copy tvchannel model
        /// </returns>
        protected virtual async Task<CopyTvChannelModel> PrepareCopyTvChannelModelAsync(CopyTvChannelModel model, TvChannel tvchannel)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.Id = tvchannel.Id;
            model.Name = string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannels.Copy.Name.New"), tvchannel.Name);
            model.Published = true;
            model.CopyMultimedia = true;

            return model;
        }

        /// <summary>
        /// Prepare tvchannel warehouse inventory models
        /// </summary>
        /// <param name="models">List of tvchannel warehouse inventory models</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareTvChannelWarehouseInventoryModelsAsync(IList<TvChannelWarehouseInventoryModel> models, TvChannel tvchannel)
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

                if (tvchannel != null)
                {
                    var tvchannelWarehouseInventory = (await _tvchannelService.GetAllTvChannelWarehouseInventoryRecordsAsync(tvchannel.Id))?.FirstOrDefault(inventory => inventory.WarehouseId == warehouse.Id);
                    if (tvchannelWarehouseInventory != null)
                    {
                        model.WarehouseUsed = true;
                        model.StockQuantity = tvchannelWarehouseInventory.StockQuantity;
                        model.ReservedQuantity = tvchannelWarehouseInventory.ReservedQuantity;
                        model.PlannedQuantity = await _shipmentService.GetQuantityInShipmentsAsync(tvchannel, tvchannelWarehouseInventory.WarehouseId, true, true);
                    }
                }

                models.Add(model);
            }
        }

        /// <summary>
        /// Prepare tvchannel attribute mapping validation rules string
        /// </summary>
        /// <param name="attributeMapping">TvChannel attribute mapping</param>
        /// <returns>
        /// A task that represents the asynchronous operation
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
        /// Prepare tvchannel attribute condition model
        /// </summary>
        /// <param name="model">TvChannel attribute condition model</param>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PrepareTvChannelAttributeConditionModelAsync(TvChannelAttributeConditionModel model,
            TvChannelAttributeMapping tvchannelAttributeMapping)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (tvchannelAttributeMapping == null)
                throw new ArgumentNullException(nameof(tvchannelAttributeMapping));

            model.TvChannelAttributeMappingId = tvchannelAttributeMapping.Id;
            model.EnableCondition = !string.IsNullOrEmpty(tvchannelAttributeMapping.ConditionAttributeXml);

            //pre-select attribute and values
            var selectedPva = (await _tvchannelAttributeParser
                .ParseTvChannelAttributeMappingsAsync(tvchannelAttributeMapping.ConditionAttributeXml))
                .FirstOrDefault();

            var attributes = (await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannelAttributeMapping.TvChannelId))
                //ignore non-combinable attributes (should have selectable values)
                .Where(x => x.CanBeUsedAsCondition())
                //ignore this attribute (it cannot depend on itself)
                .Where(x => x.Id != tvchannelAttributeMapping.Id)
                .ToList();
            foreach (var attribute in attributes)
            {
                var attributeModel = new TvChannelAttributeConditionModel.TvChannelAttributeModel
                {
                    Id = attribute.Id,
                    TvChannelAttributeId = attribute.TvChannelAttributeId,
                    Name = (await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(attribute.TvChannelAttributeId)).Name,
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _tvchannelAttributeService.GetTvChannelAttributeValuesAsync(attribute.Id);
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
                                if (!string.IsNullOrEmpty(tvchannelAttributeMapping.ConditionAttributeXml))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues =
                                        await _tvchannelAttributeParser.ParseTvChannelAttributeValuesAsync(tvchannelAttributeMapping
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
        /// Prepare related tvchannel search model
        /// </summary>
        /// <param name="searchModel">Related tvchannel search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Related tvchannel search model</returns>
        protected virtual RelatedTvChannelSearchModel PrepareRelatedTvChannelSearchModel(RelatedTvChannelSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            searchModel.TvChannelId = tvchannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare cross-sell tvchannel search model
        /// </summary>
        /// <param name="searchModel">Cross-sell tvchannel search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Cross-sell tvchannel search model</returns>
        protected virtual CrossSellTvChannelSearchModel PrepareCrossSellTvChannelSearchModel(CrossSellTvChannelSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            searchModel.TvChannelId = tvchannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare associated tvchannel search model
        /// </summary>
        /// <param name="searchModel">Associated tvchannel search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Associated tvchannel search model</returns>
        protected virtual AssociatedTvChannelSearchModel PrepareAssociatedTvChannelSearchModel(AssociatedTvChannelSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            searchModel.TvChannelId = tvchannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvchannel picture search model
        /// </summary>
        /// <param name="searchModel">TvChannel picture search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>TvChannel picture search model</returns>
        protected virtual TvChannelPictureSearchModel PrepareTvChannelPictureSearchModel(TvChannelPictureSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            searchModel.TvChannelId = tvchannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvchannel video search model
        /// </summary>
        /// <param name="searchModel">TvChannel video search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>TvChannel video search model</returns>
        protected virtual TvChannelVideoSearchModel PrepareTvChannelVideoSearchModel(TvChannelVideoSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            searchModel.TvChannelId = tvchannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvchannel order search model
        /// </summary>
        /// <param name="searchModel">TvChannel order search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>TvChannel order search model</returns>
        protected virtual TvChannelOrderSearchModel PrepareTvChannelOrderSearchModel(TvChannelOrderSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            searchModel.TvChannelId = tvchannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tier price search model
        /// </summary>
        /// <param name="searchModel">Tier price search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Tier price search model</returns>
        protected virtual TierPriceSearchModel PrepareTierPriceSearchModel(TierPriceSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            searchModel.TvChannelId = tvchannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare stock quantity history search model
        /// </summary>
        /// <param name="searchModel">Stock quantity history search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the stock quantity history search model
        /// </returns>
        protected virtual async Task<StockQuantityHistorySearchModel> PrepareStockQuantityHistorySearchModelAsync(StockQuantityHistorySearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            searchModel.TvChannelId = tvchannel.Id;

            //prepare available warehouses
            await _baseAdminModelFactory.PrepareWarehousesAsync(searchModel.AvailableWarehouses);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvchannel attribute mapping search model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute mapping search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>TvChannel attribute mapping search model</returns>
        protected virtual TvChannelAttributeMappingSearchModel PrepareTvChannelAttributeMappingSearchModel(TvChannelAttributeMappingSearchModel searchModel,
            TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            searchModel.TvChannelId = tvchannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvchannel attribute value search model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute value search model</param>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>TvChannel attribute value search model</returns>
        protected virtual TvChannelAttributeValueSearchModel PrepareTvChannelAttributeValueSearchModel(TvChannelAttributeValueSearchModel searchModel,
            TvChannelAttributeMapping tvchannelAttributeMapping)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannelAttributeMapping == null)
                throw new ArgumentNullException(nameof(tvchannelAttributeMapping));

            searchModel.TvChannelAttributeMappingId = tvchannelAttributeMapping.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvchannel attribute combination search model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute combination search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>TvChannel attribute combination search model</returns>
        protected virtual TvChannelAttributeCombinationSearchModel PrepareTvChannelAttributeCombinationSearchModel(
            TvChannelAttributeCombinationSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            searchModel.TvChannelId = tvchannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare tvchannel specification attribute search model
        /// </summary>
        /// <param name="searchModel">TvChannel specification attribute search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>TvChannel specification attribute search model</returns>
        protected virtual TvChannelSpecificationAttributeSearchModel PrepareTvChannelSpecificationAttributeSearchModel(
            TvChannelSpecificationAttributeSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            searchModel.TvChannelId = tvchannel.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare tvchannel search model
        /// </summary>
        /// <param name="searchModel">TvChannel search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel search model
        /// </returns>
        public virtual async Task<TvChannelSearchModel> PrepareTvChannelSearchModelAsync(TvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his tvchannels
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

            //prepare available tvchannel types
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
        /// Prepare paged tvchannel list model
        /// </summary>
        /// <param name="searchModel">TvChannel search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel list model
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

            //get tvchannels
            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: categoryIds,
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                warehouseId: searchModel.SearchWarehouseId,
                tvchannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
                overridePublished: overridePublished);

            //prepare list model
            var model = await new TvChannelListModel().PrepareToGridAsync(searchModel, tvchannels, () =>
            {
                return tvchannels.SelectAwait(async tvchannel =>
                {
                    //fill in model values from the entity
                    var tvchannelModel = tvchannel.ToModel<TvChannelModel>();

                    //little performance optimization: ensure that "FullDescription" is not returned
                    tvchannelModel.FullDescription = string.Empty;

                    //fill in additional values (not existing in the entity)
                    tvchannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvchannel, 0, true, false);
                    var defaultTvChannelPicture = (await _pictureService.GetPicturesByTvChannelIdAsync(tvchannel.Id, 1)).FirstOrDefault();
                    (tvchannelModel.PictureThumbnailUrl, _) = await _pictureService.GetPictureUrlAsync(defaultTvChannelPicture, 75);
                    tvchannelModel.TvChannelTypeName = await _localizationService.GetLocalizedEnumAsync(tvchannel.TvChannelType);
                    if (tvchannel.TvChannelType == TvChannelType.SimpleTvChannel && tvchannel.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                        tvchannelModel.StockQuantityStr = (await _tvchannelService.GetTotalStockQuantityAsync(tvchannel)).ToString();

                    return tvchannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvchannel model
        /// </summary>
        /// <param name="model">TvChannel model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel model
        /// </returns>
        public virtual async Task<TvChannelModel> PrepareTvChannelModelAsync(TvChannelModel model, TvChannel tvchannel, bool excludeProperties = false)
        {
            Func<TvChannelLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (tvchannel != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = tvchannel.ToModel<TvChannelModel>();
                    model.SeName = await _urlRecordService.GetSeNameAsync(tvchannel, 0, true, false);
                }

                var parentGroupedTvChannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannel.ParentGroupedTvChannelId);
                if (parentGroupedTvChannel != null)
                {
                    model.AssociatedToTvChannelId = tvchannel.ParentGroupedTvChannelId;
                    model.AssociatedToTvChannelName = parentGroupedTvChannel.Name;
                }

                model.LastStockQuantity = tvchannel.StockQuantity;
                model.TvChannelTags = string.Join(", ", (await _tvchannelTagService.GetAllTvChannelTagsByTvChannelIdAsync(tvchannel.Id)).Select(tag => tag.Name));
                model.TvChannelAttributesExist = (await _tvchannelAttributeService.GetAllTvChannelAttributesAsync()).Any();

                model.CanCreateCombinations = await (await _tvchannelAttributeService
                    .GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannel.Id)).AnyAwaitAsync(async pam => (await _tvchannelAttributeService.GetTvChannelAttributeValuesAsync(pam.Id)).Any());

                if (!excludeProperties)
                {
                    model.SelectedCategoryIds = (await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(tvchannel.Id, true))
                        .Select(tvchannelCategory => tvchannelCategory.CategoryId).ToList();
                    model.SelectedManufacturerIds = (await _manufacturerService.GetTvChannelManufacturersByTvChannelIdAsync(tvchannel.Id, true))
                        .Select(tvchannelManufacturer => tvchannelManufacturer.ManufacturerId).ToList();
                }

                //prepare copy tvchannel model
                await PrepareCopyTvChannelModelAsync(model.CopyTvChannelModel, tvchannel);

                //prepare nested search model
                PrepareRelatedTvChannelSearchModel(model.RelatedTvChannelSearchModel, tvchannel);
                PrepareCrossSellTvChannelSearchModel(model.CrossSellTvChannelSearchModel, tvchannel);
                PrepareAssociatedTvChannelSearchModel(model.AssociatedTvChannelSearchModel, tvchannel);
                PrepareTvChannelPictureSearchModel(model.TvChannelPictureSearchModel, tvchannel);
                PrepareTvChannelVideoSearchModel(model.TvChannelVideoSearchModel, tvchannel);
                PrepareTvChannelSpecificationAttributeSearchModel(model.TvChannelSpecificationAttributeSearchModel, tvchannel);
                PrepareTvChannelOrderSearchModel(model.TvChannelOrderSearchModel, tvchannel);
                PrepareTierPriceSearchModel(model.TierPriceSearchModel, tvchannel);
                await PrepareStockQuantityHistorySearchModelAsync(model.StockQuantityHistorySearchModel, tvchannel);
                PrepareTvChannelAttributeMappingSearchModel(model.TvChannelAttributeMappingSearchModel, tvchannel);
                PrepareTvChannelAttributeCombinationSearchModel(model.TvChannelAttributeCombinationSearchModel, tvchannel);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(tvchannel, entity => entity.Name, languageId, false, false);
                    locale.FullDescription = await _localizationService.GetLocalizedAsync(tvchannel, entity => entity.FullDescription, languageId, false, false);
                    locale.ShortDescription = await _localizationService.GetLocalizedAsync(tvchannel, entity => entity.ShortDescription, languageId, false, false);
                    locale.MetaKeywords = await _localizationService.GetLocalizedAsync(tvchannel, entity => entity.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = await _localizationService.GetLocalizedAsync(tvchannel, entity => entity.MetaDescription, languageId, false, false);
                    locale.MetaTitle = await _localizationService.GetLocalizedAsync(tvchannel, entity => entity.MetaTitle, languageId, false, false);
                    locale.SeName = await _urlRecordService.GetSeNameAsync(tvchannel, languageId, false, false);
                };
            }

            //set default values for the new model
            if (tvchannel == null)
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

            //prepare available tvchannel templates
            await _baseAdminModelFactory.PrepareTvChannelTemplatesAsync(model.AvailableTvChannelTemplates, false);

            //prepare available tvchannel types
            var tvchannelTemplates = await _tvchannelTemplateService.GetAllTvChannelTemplatesAsync();
            foreach (var tvchannelType in Enum.GetValues(typeof(TvChannelType)).OfType<TvChannelType>())
            {
                model.TvChannelsTypesSupportedByTvChannelTemplates.Add((int)tvchannelType, new List<SelectListItem>());
                foreach (var template in tvchannelTemplates)
                {
                    var list = (IList<int>)TypeDescriptor.GetConverter(typeof(List<int>)).ConvertFrom(template.IgnoredTvChannelTypes) ?? new List<int>();
                    if (string.IsNullOrEmpty(template.IgnoredTvChannelTypes) || !list.Contains((int)tvchannelType))
                    {
                        model.TvChannelsTypesSupportedByTvChannelTemplates[(int)tvchannelType].Add(new SelectListItem
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

            //prepare available tvchannel availability ranges
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
            await PrepareTvChannelWarehouseInventoryModelsAsync(model.TvChannelWarehouseInventoryModels, tvchannel);

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
            await _discountSupportedModelFactory.PrepareModelDiscountsAsync(model, tvchannel, availableDiscounts, excludeProperties);

            //prepare model user roles
            await _aclSupportedModelFactory.PrepareModelUserRolesAsync(model, tvchannel, excludeProperties);

            //prepare model stores
            await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, tvchannel, excludeProperties);

            var tvchannelTags = await _tvchannelTagService.GetAllTvChannelTagsAsync();
            var tvchannelTagsSb = new StringBuilder();
            tvchannelTagsSb.Append("var initialTvChannelTags = [");
            for (var i = 0; i < tvchannelTags.Count; i++)
            {
                var tag = tvchannelTags[i];
                tvchannelTagsSb.Append('\'');
                tvchannelTagsSb.Append(JavaScriptEncoder.Default.Encode(tag.Name));
                tvchannelTagsSb.Append('\'');
                if (i != tvchannelTags.Count - 1)
                    tvchannelTagsSb.Append(',');
            }
            tvchannelTagsSb.Append(']');

            model.InitialTvChannelTags = tvchannelTagsSb.ToString();

            return model;
        }

        /// <summary>
        /// Prepare required tvchannel search model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Required tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the required tvchannel search model to add to the tvchannel
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

            //prepare available tvchannel types
            await _baseAdminModelFactory.PrepareTvChannelTypesAsync(searchModel.AvailableTvChannelTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare required tvchannel list model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Required tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the required tvchannel list model to add to the tvchannel
        /// </returns>
        public virtual async Task<AddRequiredTvChannelListModel> PrepareAddRequiredTvChannelListModelAsync(AddRequiredTvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;

            //get tvchannels
            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                tvchannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddRequiredTvChannelListModel().PrepareToGridAsync(searchModel, tvchannels, () =>
            {
                return tvchannels.SelectAwait(async tvchannel =>
                {
                    var tvchannelModel = tvchannel.ToModel<TvChannelModel>();

                    tvchannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvchannel, 0, true, false);

                    return tvchannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged related tvchannel list model
        /// </summary>
        /// <param name="searchModel">Related tvchannel search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the related tvchannel list model
        /// </returns>
        public virtual async Task<RelatedTvChannelListModel> PrepareRelatedTvChannelListModelAsync(RelatedTvChannelSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            //get related tvchannels
            var relatedTvChannels = (await _tvchannelService
                .GetRelatedTvChannelsByTvChannelId1Async(tvchannelId1: tvchannel.Id, showHidden: true)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new RelatedTvChannelListModel().PrepareToGridAsync(searchModel, relatedTvChannels, () =>
            {
                return relatedTvChannels.SelectAwait(async relatedTvChannel =>
                {
                    //fill in model values from the entity
                    var relatedTvChannelModel = relatedTvChannel.ToModel<RelatedTvChannelModel>();

                    //fill in additional values (not existing in the entity)
                    relatedTvChannelModel.TvChannel2Name = (await _tvchannelService.GetTvChannelByIdAsync(relatedTvChannel.TvChannelId2))?.Name;

                    return relatedTvChannelModel;
                });
            });
            return model;
        }

        /// <summary>
        /// Prepare related tvchannel search model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Related tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the related tvchannel search model to add to the tvchannel
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

            //prepare available tvchannel types
            await _baseAdminModelFactory.PrepareTvChannelTypesAsync(searchModel.AvailableTvChannelTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged related tvchannel list model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Related tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the related tvchannel list model to add to the tvchannel
        /// </returns>
        public virtual async Task<AddRelatedTvChannelListModel> PrepareAddRelatedTvChannelListModelAsync(AddRelatedTvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;

            //get tvchannels
            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                tvchannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddRelatedTvChannelListModel().PrepareToGridAsync(searchModel, tvchannels, () =>
            {
                return tvchannels.SelectAwait(async tvchannel =>
                {
                    var tvchannelModel = tvchannel.ToModel<TvChannelModel>();

                    tvchannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvchannel, 0, true, false);

                    return tvchannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged cross-sell tvchannel list model
        /// </summary>
        /// <param name="searchModel">Cross-sell tvchannel search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the cross-sell tvchannel list model
        /// </returns>
        public virtual async Task<CrossSellTvChannelListModel> PrepareCrossSellTvChannelListModelAsync(CrossSellTvChannelSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            //get cross-sell tvchannels
            var crossSellTvChannels = (await _tvchannelService
                .GetCrossSellTvChannelsByTvChannelId1Async(tvchannelId1: tvchannel.Id, showHidden: true)).ToPagedList(searchModel);

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
                    crossSellTvChannelModel.TvChannel2Name = (await _tvchannelService.GetTvChannelByIdAsync(crossSellTvChannel.TvChannelId2))?.Name;

                    return crossSellTvChannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare cross-sell tvchannel search model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Cross-sell tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the cross-sell tvchannel search model to add to the tvchannel
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

            //prepare available tvchannel types
            await _baseAdminModelFactory.PrepareTvChannelTypesAsync(searchModel.AvailableTvChannelTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged crossSell tvchannel list model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">CrossSell tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the crossSell tvchannel list model to add to the tvchannel
        /// </returns>
        public virtual async Task<AddCrossSellTvChannelListModel> PrepareAddCrossSellTvChannelListModelAsync(AddCrossSellTvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;

            //get tvchannels
            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                tvchannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddCrossSellTvChannelListModel().PrepareToGridAsync(searchModel, tvchannels, () =>
            {
                return tvchannels.SelectAwait(async tvchannel =>
                {
                    var tvchannelModel = tvchannel.ToModel<TvChannelModel>();

                    tvchannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvchannel, 0, true, false);

                    return tvchannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged associated tvchannel list model
        /// </summary>
        /// <param name="searchModel">Associated tvchannel search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the associated tvchannel list model
        /// </returns>
        public virtual async Task<AssociatedTvChannelListModel> PrepareAssociatedTvChannelListModelAsync(AssociatedTvChannelSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var vendor = await _workContext.GetCurrentVendorAsync();
            //get associated tvchannels
            var associatedTvChannels = (await _tvchannelService.GetAssociatedTvChannelsAsync(showHidden: true,
                parentGroupedTvChannelId: tvchannel.Id,
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
        /// Prepare associated tvchannel search model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Associated tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the associated tvchannel search model to add to the tvchannel
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

            //prepare available tvchannel types
            await _baseAdminModelFactory.PrepareTvChannelTypesAsync(searchModel.AvailableTvChannelTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged associated tvchannel list model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Associated tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the associated tvchannel list model to add to the tvchannel
        /// </returns>
        public virtual async Task<AddAssociatedTvChannelListModel> PrepareAddAssociatedTvChannelListModelAsync(AddAssociatedTvChannelSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;

            //get tvchannels
            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                tvchannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddAssociatedTvChannelListModel().PrepareToGridAsync(searchModel, tvchannels, () =>
            {
                return tvchannels.SelectAwait(async tvchannel =>
                {
                    //fill in model values from the entity
                    var tvchannelModel = tvchannel.ToModel<TvChannelModel>();

                    //fill in additional values (not existing in the entity)
                    tvchannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvchannel, 0, true, false);
                    var parentGroupedTvChannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannel.ParentGroupedTvChannelId);

                    if (parentGroupedTvChannel == null)
                        return tvchannelModel;

                    tvchannelModel.AssociatedToTvChannelId = tvchannel.ParentGroupedTvChannelId;
                    tvchannelModel.AssociatedToTvChannelName = parentGroupedTvChannel.Name;

                    return tvchannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged tvchannel picture list model
        /// </summary>
        /// <param name="searchModel">TvChannel picture search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel picture list model
        /// </returns>
        public virtual async Task<TvChannelPictureListModel> PrepareTvChannelPictureListModelAsync(TvChannelPictureSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            //get tvchannel pictures
            var tvchannelPictures = (await _tvchannelService.GetTvChannelPicturesByTvChannelIdAsync(tvchannel.Id)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new TvChannelPictureListModel().PrepareToGridAsync(searchModel, tvchannelPictures, () =>
            {
                return tvchannelPictures.SelectAwait(async tvchannelPicture =>
                {
                    //fill in model values from the entity
                    var tvchannelPictureModel = tvchannelPicture.ToModel<TvChannelPictureModel>();

                    //fill in additional values (not existing in the entity)
                    var picture = (await _pictureService.GetPictureByIdAsync(tvchannelPicture.PictureId))
                        ?? throw new Exception("Picture cannot be loaded");

                    tvchannelPictureModel.PictureUrl = (await _pictureService.GetPictureUrlAsync(picture)).Url;

                    tvchannelPictureModel.OverrideAltAttribute = picture.AltAttribute;
                    tvchannelPictureModel.OverrideTitleAttribute = picture.TitleAttribute;

                    return tvchannelPictureModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged tvchannel video list model
        /// </summary>
        /// <param name="searchModel">TvChannel video search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel video list model
        /// </returns>
        public virtual async Task<TvChannelVideoListModel> PrepareTvChannelVideoListModelAsync(TvChannelVideoSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            //get tvchannel videos
            var tvchannelVideos = (await _tvchannelService.GetTvChannelVideosByTvChannelIdAsync(tvchannel.Id)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new TvChannelVideoListModel().PrepareToGridAsync(searchModel, tvchannelVideos, () =>
            {
                return tvchannelVideos.SelectAwait(async tvchannelVideo =>
                {
                    //fill in model values from the entity
                    var tvchannelVideoModel = tvchannelVideo.ToModel<TvChannelVideoModel>();

                    //fill in additional values (not existing in the entity)
                    var video = (await _videoService.GetVideoByIdAsync(tvchannelVideo.VideoId))
                        ?? throw new Exception("Video cannot be loaded");

                    tvchannelVideoModel.VideoUrl = video.VideoUrl;

                    return tvchannelVideoModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged tvchannel specification attribute list model
        /// </summary>
        /// <param name="searchModel">TvChannel specification attribute search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel specification attribute list model
        /// </returns>
        public virtual async Task<TvChannelSpecificationAttributeListModel> PrepareTvChannelSpecificationAttributeListModelAsync(
            TvChannelSpecificationAttributeSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            //get tvchannel specification attributes
            var tvchannelSpecificationAttributes = (await _specificationAttributeService
                .GetTvChannelSpecificationAttributesAsync(tvchannel.Id)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new TvChannelSpecificationAttributeListModel().PrepareToGridAsync(searchModel, tvchannelSpecificationAttributes, () =>
            {
                return tvchannelSpecificationAttributes.SelectAwait(async attribute =>
                {
                    //fill in model values from the entity
                    var tvchannelSpecificationAttributeModel = attribute.ToModel<TvChannelSpecificationAttributeModel>();

                    var specAttributeOption = await _specificationAttributeService
                        .GetSpecificationAttributeOptionByIdAsync(attribute.SpecificationAttributeOptionId);
                    var specAttribute = await _specificationAttributeService
                        .GetSpecificationAttributeByIdAsync(specAttributeOption.SpecificationAttributeId);

                    //fill in additional values (not existing in the entity)
                    tvchannelSpecificationAttributeModel.AttributeTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeType);

                    tvchannelSpecificationAttributeModel.AttributeId = specAttribute.Id;
                    tvchannelSpecificationAttributeModel.AttributeName = await GetSpecificationAttributeNameAsync(specAttribute);
                    var currentLanguage = await _workContext.GetWorkingLanguageAsync();

                    switch (attribute.AttributeType)
                    {
                        case SpecificationAttributeType.Option:
                            tvchannelSpecificationAttributeModel.ValueRaw = WebUtility.HtmlEncode(specAttributeOption.Name);
                            tvchannelSpecificationAttributeModel.SpecificationAttributeOptionId = specAttributeOption.Id;
                            break;
                        case SpecificationAttributeType.CustomText:
                            tvchannelSpecificationAttributeModel.ValueRaw = WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(attribute, x => x.CustomValue, currentLanguage?.Id));
                            break;
                        case SpecificationAttributeType.CustomHtmlText:
                            tvchannelSpecificationAttributeModel.ValueRaw = await _localizationService
                                .GetLocalizedAsync(attribute, x => x.CustomValue, currentLanguage?.Id);
                            break;
                        case SpecificationAttributeType.Hyperlink:
                            tvchannelSpecificationAttributeModel.ValueRaw = attribute.CustomValue;
                            break;
                    }

                    return tvchannelSpecificationAttributeModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged tvchannel specification attribute model
        /// </summary>
        /// <param name="tvchannelId">TvChannel id</param>
        /// <param name="specificationId">Specification attribute id</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel specification attribute model
        /// </returns>
        public virtual async Task<AddSpecificationAttributeModel> PrepareAddSpecificationAttributeModelAsync(int tvchannelId, int? specificationId)
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
                    TvChannelId = tvchannelId,
                    Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync<AddSpecificationAttributeLocalizedModel>()
                };
            }

            var attribute = await _specificationAttributeService.GetTvChannelSpecificationAttributeByIdAsync(specificationId.Value);

            if (attribute == null)
            {
                throw new ArgumentException("No specification attribute found with the specified id");
            }

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && (await _tvchannelService.GetTvChannelByIdAsync(attribute.TvChannelId)).VendorId != currentVendor.Id)
                throw new UnauthorizedAccessException("This is not your tvchannel");

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
        /// Prepare tvchannel tag search model
        /// </summary>
        /// <param name="searchModel">TvChannel tag search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel tag search model
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
        /// Prepare paged tvchannel tag list model
        /// </summary>
        /// <param name="searchModel">TvChannel tag search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel tag list model
        /// </returns>
        public virtual async Task<TvChannelTagListModel> PrepareTvChannelTagListModelAsync(TvChannelTagSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tvchannel tags
            var tvchannelTags = (await (await _tvchannelTagService.GetAllTvChannelTagsAsync(tagName: searchModel.SearchTagName))
                .OrderByDescendingAwait(async tag => await _tvchannelTagService.GetTvChannelCountByTvChannelTagIdAsync(tag.Id, storeId: 0, showHidden: true)).ToListAsync())
                .ToPagedList(searchModel);

            //prepare list model
            var model = await new TvChannelTagListModel().PrepareToGridAsync(searchModel, tvchannelTags, () =>
            {
                return tvchannelTags.SelectAwait(async tag =>
                {
                    //fill in model values from the entity
                    var tvchannelTagModel = tag.ToModel<TvChannelTagModel>();

                    //fill in additional values (not existing in the entity)
                    tvchannelTagModel.TvChannelCount = await _tvchannelTagService.GetTvChannelCountByTvChannelTagIdAsync(tag.Id, storeId: 0, showHidden: true);

                    return tvchannelTagModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvchannel tag model
        /// </summary>
        /// <param name="model">TvChannel tag model</param>
        /// <param name="tvchannelTag">TvChannel tag</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel tag model
        /// </returns>
        public virtual async Task<TvChannelTagModel> PrepareTvChannelTagModelAsync(TvChannelTagModel model, TvChannelTag tvchannelTag, bool excludeProperties = false)
        {
            Func<TvChannelTagLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (tvchannelTag != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = tvchannelTag.ToModel<TvChannelTagModel>();
                }

                model.TvChannelCount = await _tvchannelTagService.GetTvChannelCountByTvChannelTagIdAsync(tvchannelTag.Id, storeId: 0, showHidden: true);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(tvchannelTag, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged tvchannel order list model
        /// </summary>
        /// <param name="searchModel">TvChannel order search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel order list model
        /// </returns>
        public virtual async Task<TvChannelOrderListModel> PrepareTvChannelOrderListModelAsync(TvChannelOrderSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            //get orders
            var orders = await _orderService.SearchOrdersAsync(tvchannelId: searchModel.TvChannelId,
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
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ier price list model
        /// </returns>
        public virtual async Task<TierPriceListModel> PrepareTierPriceListModelAsync(TierPriceSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            //get tier prices
            var tierPrices = (await _tvchannelService.GetTierPricesByTvChannelAsync(tvchannel.Id))
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
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tierPrice">Tier price</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ier price model
        /// </returns>
        public virtual async Task<TierPriceModel> PrepareTierPriceModelAsync(TierPriceModel model,
            TvChannel tvchannel, TierPrice tierPrice, bool excludeProperties = false)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

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
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the stock quantity history list model
        /// </returns>
        public virtual async Task<StockQuantityHistoryListModel> PrepareStockQuantityHistoryListModelAsync(StockQuantityHistorySearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            //get stock quantity history
            var stockQuantityHistory = await _tvchannelService.GetStockQuantityHistoryAsync(tvchannel: tvchannel,
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
                    var combination = await _tvchannelAttributeService.GetTvChannelAttributeCombinationByIdAsync(historyEntry.CombinationId ?? 0);
                    if (combination != null)
                    {
                        stockQuantityHistoryModel.AttributeCombination = await _tvchannelAttributeFormatter
                            .FormatAttributesAsync(tvchannel, combination.AttributesXml, currentUser, currentStore, renderGiftCardAttributes: false);
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
        /// Prepare paged tvchannel attribute mapping list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute mapping search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute mapping list model
        /// </returns>
        public virtual async Task<TvChannelAttributeMappingListModel> PrepareTvChannelAttributeMappingListModelAsync(TvChannelAttributeMappingSearchModel searchModel,
            TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            //get tvchannel attribute mappings
            var tvchannelAttributeMappings = (await _tvchannelAttributeService
                .GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannel.Id)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new TvChannelAttributeMappingListModel().PrepareToGridAsync(searchModel, tvchannelAttributeMappings, () =>
            {
                return tvchannelAttributeMappings.SelectAwait(async attributeMapping =>
                {
                    //fill in model values from the entity
                    var tvchannelAttributeMappingModel = attributeMapping.ToModel<TvChannelAttributeMappingModel>();

                    //fill in additional values (not existing in the entity)
                    tvchannelAttributeMappingModel.ConditionString = string.Empty;

                    tvchannelAttributeMappingModel.ValidationRulesString = await PrepareTvChannelAttributeMappingValidationRulesStringAsync(attributeMapping);
                    tvchannelAttributeMappingModel.TvChannelAttribute = (await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(attributeMapping.TvChannelAttributeId))?.Name;
                    tvchannelAttributeMappingModel.AttributeControlType = await _localizationService.GetLocalizedEnumAsync(attributeMapping.AttributeControlType);
                    var conditionAttribute = (await _tvchannelAttributeParser
                        .ParseTvChannelAttributeMappingsAsync(attributeMapping.ConditionAttributeXml))
                        .FirstOrDefault();
                    if (conditionAttribute == null)
                        return tvchannelAttributeMappingModel;

                    var conditionValue = (await _tvchannelAttributeParser
                        .ParseTvChannelAttributeValuesAsync(attributeMapping.ConditionAttributeXml))
                        .FirstOrDefault();
                    if (conditionValue != null)
                    {
                        tvchannelAttributeMappingModel.ConditionString =
                            $"{WebUtility.HtmlEncode((await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(conditionAttribute.TvChannelAttributeId)).Name)}: {WebUtility.HtmlEncode(conditionValue.Name)}";
                    }

                    return tvchannelAttributeMappingModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvchannel attribute mapping model
        /// </summary>
        /// <param name="model">TvChannel attribute mapping model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute mapping model
        /// </returns>
        public virtual async Task<TvChannelAttributeMappingModel> PrepareTvChannelAttributeMappingModelAsync(TvChannelAttributeMappingModel model,
            TvChannel tvchannel, TvChannelAttributeMapping tvchannelAttributeMapping, bool excludeProperties = false)
        {
            Func<TvChannelAttributeMappingLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (tvchannelAttributeMapping != null)
            {
                //fill in model values from the entity
                model ??= new TvChannelAttributeMappingModel
                {
                    Id = tvchannelAttributeMapping.Id
                };

                model.TvChannelAttribute = (await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(tvchannelAttributeMapping.TvChannelAttributeId)).Name;
                model.AttributeControlType = await _localizationService.GetLocalizedEnumAsync(tvchannelAttributeMapping.AttributeControlType);

                if (!excludeProperties)
                {
                    model.TvChannelAttributeId = tvchannelAttributeMapping.TvChannelAttributeId;
                    model.TextPrompt = tvchannelAttributeMapping.TextPrompt;
                    model.IsRequired = tvchannelAttributeMapping.IsRequired;
                    model.AttributeControlTypeId = tvchannelAttributeMapping.AttributeControlTypeId;
                    model.DisplayOrder = tvchannelAttributeMapping.DisplayOrder;
                    model.ValidationMinLength = tvchannelAttributeMapping.ValidationMinLength;
                    model.ValidationMaxLength = tvchannelAttributeMapping.ValidationMaxLength;
                    model.ValidationFileAllowedExtensions = tvchannelAttributeMapping.ValidationFileAllowedExtensions;
                    model.ValidationFileMaximumSize = tvchannelAttributeMapping.ValidationFileMaximumSize;
                    model.DefaultValue = tvchannelAttributeMapping.DefaultValue;
                }

                //prepare condition attributes model
                model.ConditionAllowed = true;
                await PrepareTvChannelAttributeConditionModelAsync(model.ConditionModel, tvchannelAttributeMapping);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.TextPrompt = await _localizationService.GetLocalizedAsync(tvchannelAttributeMapping, entity => entity.TextPrompt, languageId, false, false);
                    locale.DefaultValue = await _localizationService.GetLocalizedAsync(tvchannelAttributeMapping, entity => entity.DefaultValue, languageId, false, false);
                };

                //prepare nested search model
                PrepareTvChannelAttributeValueSearchModel(model.TvChannelAttributeValueSearchModel, tvchannelAttributeMapping);
            }

            model.TvChannelId = tvchannel.Id;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare available tvchannel attributes
            model.AvailableTvChannelAttributes = (await _tvchannelAttributeService.GetAllTvChannelAttributesAsync()).Select(tvchannelAttribute => new SelectListItem
            {
                Text = tvchannelAttribute.Name,
                Value = tvchannelAttribute.Id.ToString()
            }).ToList();

            return model;
        }

        /// <summary>
        /// Prepare paged tvchannel attribute value list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute value search model</param>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute value list model
        /// </returns>
        public virtual async Task<TvChannelAttributeValueListModel> PrepareTvChannelAttributeValueListModelAsync(TvChannelAttributeValueSearchModel searchModel,
            TvChannelAttributeMapping tvchannelAttributeMapping)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannelAttributeMapping == null)
                throw new ArgumentNullException(nameof(tvchannelAttributeMapping));

            //get tvchannel attribute values
            var tvchannelAttributeValues = (await _tvchannelAttributeService
                .GetTvChannelAttributeValuesAsync(tvchannelAttributeMapping.Id)).ToPagedList(searchModel);

            //prepare list model
            var model = await new TvChannelAttributeValueListModel().PrepareToGridAsync(searchModel, tvchannelAttributeValues, () =>
            {
                return tvchannelAttributeValues.SelectAwait(async value =>
                {
                    //fill in model values from the entity
                    var tvchannelAttributeValueModel = value.ToModel<TvChannelAttributeValueModel>();

                    //fill in additional values (not existing in the entity)
                    tvchannelAttributeValueModel.AttributeValueTypeName = await _localizationService.GetLocalizedEnumAsync(value.AttributeValueType);

                    tvchannelAttributeValueModel.Name = tvchannelAttributeMapping.AttributeControlType != AttributeControlType.ColorSquares
                        ? value.Name : $"{value.Name} - {value.ColorSquaresRgb}";
                    if (value.AttributeValueType == AttributeValueType.Simple)
                    {
                        tvchannelAttributeValueModel.PriceAdjustmentStr = value.PriceAdjustment.ToString("G29");
                        if (value.PriceAdjustmentUsePercentage)
                            tvchannelAttributeValueModel.PriceAdjustmentStr += " %";
                        tvchannelAttributeValueModel.WeightAdjustmentStr = value.WeightAdjustment.ToString("G29");
                    }

                    if (value.AttributeValueType == AttributeValueType.AssociatedToTvChannel)
                    {
                        tvchannelAttributeValueModel.AssociatedTvChannelName = (await _tvchannelService.GetTvChannelByIdAsync(value.AssociatedTvChannelId))?.Name ?? string.Empty;
                    }

                    var pictureThumbnailUrl = await _pictureService.GetPictureUrlAsync(value.PictureId, 75, false);
                    //little hack here. Grid is rendered wrong way with <img> without "src" attribute
                    if (string.IsNullOrEmpty(pictureThumbnailUrl))
                        pictureThumbnailUrl = await _pictureService.GetDefaultPictureUrlAsync(targetSize: 1);

                    tvchannelAttributeValueModel.PictureThumbnailUrl = pictureThumbnailUrl;

                    return tvchannelAttributeValueModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvchannel attribute value model
        /// </summary>
        /// <param name="model">TvChannel attribute value model</param>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <param name="tvchannelAttributeValue">TvChannel attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute value model
        /// </returns>
        public virtual async Task<TvChannelAttributeValueModel> PrepareTvChannelAttributeValueModelAsync(TvChannelAttributeValueModel model,
            TvChannelAttributeMapping tvchannelAttributeMapping, TvChannelAttributeValue tvchannelAttributeValue, bool excludeProperties = false)
        {
            if (tvchannelAttributeMapping == null)
                throw new ArgumentNullException(nameof(tvchannelAttributeMapping));

            Func<TvChannelAttributeValueLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (tvchannelAttributeValue != null)
            {
                //fill in model values from the entity
                model ??= new TvChannelAttributeValueModel
                {
                    TvChannelAttributeMappingId = tvchannelAttributeValue.TvChannelAttributeMappingId,
                    AttributeValueTypeId = tvchannelAttributeValue.AttributeValueTypeId,
                    AttributeValueTypeName = await _localizationService.GetLocalizedEnumAsync(tvchannelAttributeValue.AttributeValueType),
                    AssociatedTvChannelId = tvchannelAttributeValue.AssociatedTvChannelId,
                    Name = tvchannelAttributeValue.Name,
                    ColorSquaresRgb = tvchannelAttributeValue.ColorSquaresRgb,
                    DisplayColorSquaresRgb = tvchannelAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares,
                    ImageSquaresPictureId = tvchannelAttributeValue.ImageSquaresPictureId,
                    DisplayImageSquaresPicture = tvchannelAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares,
                    PriceAdjustment = tvchannelAttributeValue.PriceAdjustment,
                    PriceAdjustmentUsePercentage = tvchannelAttributeValue.PriceAdjustmentUsePercentage,
                    WeightAdjustment = tvchannelAttributeValue.WeightAdjustment,
                    Cost = tvchannelAttributeValue.Cost,
                    UserEntersQty = tvchannelAttributeValue.UserEntersQty,
                    Quantity = tvchannelAttributeValue.Quantity,
                    IsPreSelected = tvchannelAttributeValue.IsPreSelected,
                    DisplayOrder = tvchannelAttributeValue.DisplayOrder,
                    PictureId = tvchannelAttributeValue.PictureId
                };

                model.AssociatedTvChannelName = (await _tvchannelService.GetTvChannelByIdAsync(tvchannelAttributeValue.AssociatedTvChannelId))?.Name;

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(tvchannelAttributeValue, entity => entity.Name, languageId, false, false);
                };
            }

            model.TvChannelAttributeMappingId = tvchannelAttributeMapping.Id;
            model.DisplayColorSquaresRgb = tvchannelAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares;
            model.DisplayImageSquaresPicture = tvchannelAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares;

            //set default values for the new model
            if (tvchannelAttributeValue == null)
                model.Quantity = 1;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare picture models
            var tvchannelPictures = await _tvchannelService.GetTvChannelPicturesByTvChannelIdAsync(tvchannelAttributeMapping.TvChannelId);
            model.TvChannelPictureModels = await tvchannelPictures.SelectAwait(async tvchannelPicture => new TvChannelPictureModel
            {
                Id = tvchannelPicture.Id,
                TvChannelId = tvchannelPicture.TvChannelId,
                PictureId = tvchannelPicture.PictureId,
                PictureUrl = await _pictureService.GetPictureUrlAsync(tvchannelPicture.PictureId),
                DisplayOrder = tvchannelPicture.DisplayOrder
            }).ToListAsync();

            return model;
        }

        /// <summary>
        /// Prepare tvchannel model to associate to the tvchannel attribute value
        /// </summary>
        /// <param name="searchModel">TvChannel model to associate to the tvchannel attribute value</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel model to associate to the tvchannel attribute value
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

            //prepare available tvchannel types
            await _baseAdminModelFactory.PrepareTvChannelTypesAsync(searchModel.AvailableTvChannelTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged tvchannel model to associate to the tvchannel attribute value
        /// </summary>
        /// <param name="searchModel">TvChannel model to associate to the tvchannel attribute value</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel model to associate to the tvchannel attribute value
        /// </returns>
        public virtual async Task<AssociateTvChannelToAttributeValueListModel> PrepareAssociateTvChannelToAttributeValueListModelAsync(
            AssociateTvChannelToAttributeValueSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
                searchModel.SearchVendorId = currentVendor.Id;

            //get tvchannels
            var tvchannels = await _tvchannelService.SearchTvChannelsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                tvchannelType: searchModel.SearchTvChannelTypeId > 0 ? (TvChannelType?)searchModel.SearchTvChannelTypeId : null,
                keywords: searchModel.SearchTvChannelName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AssociateTvChannelToAttributeValueListModel().PrepareToGridAsync(searchModel, tvchannels, () =>
            {
                //fill in model values from the entity
                return tvchannels.SelectAwait(async tvchannel =>
                {
                    var tvchannelModel = tvchannel.ToModel<TvChannelModel>();

                    tvchannelModel.SeName = await _urlRecordService.GetSeNameAsync(tvchannel, 0, true, false);

                    return tvchannelModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged tvchannel attribute combination list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute combination search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute combination list model
        /// </returns>
        public virtual async Task<TvChannelAttributeCombinationListModel> PrepareTvChannelAttributeCombinationListModelAsync(
            TvChannelAttributeCombinationSearchModel searchModel, TvChannel tvchannel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            //get tvchannel attribute combinations
            var tvchannelAttributeCombinations = (await _tvchannelAttributeService
                .GetAllTvChannelAttributeCombinationsAsync(tvchannel.Id)).ToPagedList(searchModel);

            var currentUser = await _workContext.GetCurrentUserAsync();
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            
            //prepare grid model
            var model = await new TvChannelAttributeCombinationListModel().PrepareToGridAsync(searchModel, tvchannelAttributeCombinations, () =>
            {
                return tvchannelAttributeCombinations.SelectAwait(async combination =>
                {
                    //fill in model values from the entity
                    var tvchannelAttributeCombinationModel = combination.ToModel<TvChannelAttributeCombinationModel>();

                    //fill in additional values (not existing in the entity)
                    tvchannelAttributeCombinationModel.AttributesXml = await _tvchannelAttributeFormatter
                        .FormatAttributesAsync(tvchannel, combination.AttributesXml, currentUser, currentStore, "<br />", true, true, true, false);
                    var pictureThumbnailUrl = await _pictureService.GetPictureUrlAsync(combination.PictureId, 75, false);
                    //little hack here. Grid is rendered wrong way with <img> without "src" attribute
                    if (string.IsNullOrEmpty(pictureThumbnailUrl))
                        pictureThumbnailUrl = await _pictureService.GetDefaultPictureUrlAsync(targetSize: 1);

                    tvchannelAttributeCombinationModel.PictureThumbnailUrl = pictureThumbnailUrl;
                    var warnings = (await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(currentUser,
                        ShoppingCartType.ShoppingCart, tvchannel,
                        attributesXml: combination.AttributesXml,
                        ignoreNonCombinableAttributes: true)
                        ).Aggregate(string.Empty, (message, warning) => $"{message}{warning}<br />");
                    tvchannelAttributeCombinationModel.Warnings = new List<string> { warnings };

                    return tvchannelAttributeCombinationModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tvchannel attribute combination model
        /// </summary>
        /// <param name="model">TvChannel attribute combination model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelAttributeCombination">TvChannel attribute combination</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute combination model
        /// </returns>
        public virtual async Task<TvChannelAttributeCombinationModel> PrepareTvChannelAttributeCombinationModelAsync(TvChannelAttributeCombinationModel model,
            TvChannel tvchannel, TvChannelAttributeCombination tvchannelAttributeCombination, bool excludeProperties = false)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (tvchannelAttributeCombination != null)
            {
                //fill in model values from the entity
                model ??= new TvChannelAttributeCombinationModel
                {
                    AllowOutOfStockOrders = tvchannelAttributeCombination.AllowOutOfStockOrders,
                    AttributesXml = tvchannelAttributeCombination.AttributesXml,
                    Gtin = tvchannelAttributeCombination.Gtin,
                    Id = tvchannelAttributeCombination.Id,
                    ManufacturerPartNumber = tvchannelAttributeCombination.ManufacturerPartNumber,
                    NotifyAdminForQuantityBelow = tvchannelAttributeCombination.NotifyAdminForQuantityBelow,
                    OverriddenPrice = tvchannelAttributeCombination.OverriddenPrice,
                    PictureId = tvchannelAttributeCombination.PictureId,
                    TvChannelId = tvchannelAttributeCombination.TvChannelId,
                    Sku = tvchannelAttributeCombination.Sku,
                    StockQuantity = tvchannelAttributeCombination.StockQuantity,
                    MinStockQuantity = tvchannelAttributeCombination.MinStockQuantity
                };
            }

            model.TvChannelId = tvchannel.Id;

            //set default values for the new model
            if (tvchannelAttributeCombination == null)
            {
                model.TvChannelId = tvchannel.Id;
                model.StockQuantity = 10000;
                model.NotifyAdminForQuantityBelow = 1;
            }

            //prepare picture models
            var tvchannelPictures = await _tvchannelService.GetTvChannelPicturesByTvChannelIdAsync(tvchannel.Id);
            model.TvChannelPictureModels = await tvchannelPictures.SelectAwait(async tvchannelPicture => new TvChannelPictureModel
            {
                Id = tvchannelPicture.Id,
                TvChannelId = tvchannelPicture.TvChannelId,
                PictureId = tvchannelPicture.PictureId,
                PictureUrl = await _pictureService.GetPictureUrlAsync(tvchannelPicture.PictureId),
                DisplayOrder = tvchannelPicture.DisplayOrder
            }).ToListAsync();

            //prepare tvchannel attribute mappings (exclude non-combinable attributes)
            var attributes = (await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannel.Id))
                .Where(tvchannelAttributeMapping => !tvchannelAttributeMapping.IsNonCombinable()).ToList();

            foreach (var attribute in attributes)
            {
                var attributeModel = new TvChannelAttributeCombinationModel.TvChannelAttributeModel
                {
                    Id = attribute.Id,
                    TvChannelAttributeId = attribute.TvChannelAttributeId,
                    Name = (await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(attribute.TvChannelAttributeId)).Name,
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _tvchannelAttributeService.GetTvChannelAttributeValuesAsync(attribute.Id);
                    var preSelectedValue = _tvchannelAttributeParser.ParseValues(model.AttributesXml, attribute.Id);
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