using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Vendors;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Vendors;
using TvProgViewer.Web.Framework.Factories;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the vendor model factory implementation
    /// </summary>
    public partial class VendorModelFactory : IVendorModelFactory
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorAttributeParser _vendorAttributeParser;
        private readonly IVendorAttributeService _vendorAttributeService;
        private readonly IVendorService _vendorService;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public VendorModelFactory(CurrencySettings currencySettings,
            ICurrencyService currencyService,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IUrlRecordService urlRecordService,
            IVendorAttributeParser vendorAttributeParser,
            IVendorAttributeService vendorAttributeService,
            IVendorService vendorService,
            VendorSettings vendorSettings)
        {
            _currencySettings = currencySettings;
            _currencyService = currencyService;
            _addressModelFactory = addressModelFactory;
            _addressService = addressService;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _urlRecordService = urlRecordService;
            _vendorAttributeParser = vendorAttributeParser;
            _vendorAttributeService = vendorAttributeService;
            _vendorService = vendorService;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare vendor associated user models
        /// </summary>
        /// <param name="models">List of vendor associated user models</param>
        /// <param name="vendor">Vendor</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task PrepareAssociatedUserModelsAsync(IList<VendorAssociatedUserModel> models, Vendor vendor)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            var associatedUsers = await _userService.GetAllUsersAsync(vendorId: vendor.Id);
            foreach (var user in associatedUsers)
            {
                models.Add(new VendorAssociatedUserModel
                {
                    Id = user.Id,
                    Email = user.Email
                });
            }
        }

        /// <summary>
        /// Prepare vendor attribute models
        /// </summary>
        /// <param name="models">List of vendor attribute models</param>
        /// <param name="vendor">Vendor</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task PrepareVendorAttributeModelsAsync(IList<VendorModel.VendorAttributeModel> models, Vendor vendor)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //get available vendor attributes
            var vendorAttributes = await _vendorAttributeService.GetAllVendorAttributesAsync();
            foreach (var attribute in vendorAttributes)
            {
                var attributeModel = new VendorModel.VendorAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _vendorAttributeService.GetVendorAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new VendorModel.VendorAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                if (vendor != null)
                {
                    var selectedVendorAttributes = await _genericAttributeService.GetAttributeAsync<string>(vendor, TvProgVendorDefaults.VendorAttributes);
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                            {
                                if (!string.IsNullOrEmpty(selectedVendorAttributes))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = await _vendorAttributeParser.ParseVendorAttributeValuesAsync(selectedVendorAttributes);
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
                                if (!string.IsNullOrEmpty(selectedVendorAttributes))
                                {
                                    var enteredText = _vendorAttributeParser.ParseValues(selectedVendorAttributes, attribute.Id);
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
        /// Prepare vendor note search model
        /// </summary>
        /// <param name="searchModel">Vendor note search model</param>
        /// <param name="vendor">Vendor</param>
        /// <returns>Vendor note search model</returns>
        protected virtual VendorNoteSearchModel PrepareVendorNoteSearchModel(VendorNoteSearchModel searchModel, Vendor vendor)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            searchModel.VendorId = vendor.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare vendor search model
        /// </summary>
        /// <param name="searchModel">Vendor search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor search model
        /// </returns>
        public virtual Task<VendorSearchModel> PrepareVendorSearchModelAsync(VendorSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged vendor list model
        /// </summary>
        /// <param name="searchModel">Vendor search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor list model
        /// </returns>
        public virtual async Task<VendorListModel> PrepareVendorListModelAsync(VendorSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get vendors
            var vendors = await _vendorService.GetAllVendorsAsync(showHidden: true,
                name: searchModel.SearchName,
                email: searchModel.SearchEmail,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new VendorListModel().PrepareToGridAsync(searchModel, vendors, () =>
            {
                //fill in model values from the entity
                return vendors.SelectAwait(async vendor =>
                {
                    var vendorModel = vendor.ToModel<VendorModel>();

                    vendorModel.SeName = await _urlRecordService.GetSeNameAsync(vendor, 0, true, false);

                    return vendorModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare vendor model
        /// </summary>
        /// <param name="model">Vendor model</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor model
        /// </returns>
        public virtual async Task<VendorModel> PrepareVendorModelAsync(VendorModel model, Vendor vendor, bool excludeProperties = false)
        {
            Func<VendorLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (vendor != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = vendor.ToModel<VendorModel>();
                    model.SeName = await _urlRecordService.GetSeNameAsync(vendor, 0, true, false);
                }

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(vendor, entity => entity.Name, languageId, false, false);
                    locale.Description = await _localizationService.GetLocalizedAsync(vendor, entity => entity.Description, languageId, false, false);
                    locale.MetaKeywords = await _localizationService.GetLocalizedAsync(vendor, entity => entity.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = await _localizationService.GetLocalizedAsync(vendor, entity => entity.MetaDescription, languageId, false, false);
                    locale.MetaTitle = await _localizationService.GetLocalizedAsync(vendor, entity => entity.MetaTitle, languageId, false, false);
                    locale.SeName = await _urlRecordService.GetSeNameAsync(vendor, languageId, false, false);
                };

                //prepare associated users
                await PrepareAssociatedUserModelsAsync(model.AssociatedUsers, vendor);

                //prepare nested search models
                PrepareVendorNoteSearchModel(model.VendorNoteSearchModel, vendor);
            }

            //set default values for the new model
            if (vendor == null)
            {
                model.PageSize = 6;
                model.Active = true;
                model.AllowUsersToSelectPageSize = true;
                model.PageSizeOptions = _vendorSettings.DefaultVendorPageSizeOptions;
                model.PriceRangeFiltering = true;
                model.ManuallyPriceRange = true;
                model.PriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom;
                model.PriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo;
            }

            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare model vendor attributes
            await PrepareVendorAttributeModelsAsync(model.VendorAttributes, vendor);

            //prepare address model
            var address = await _addressService.GetAddressByIdAsync(vendor?.AddressId ?? 0);
            if (!excludeProperties && address != null)
                model.Address = address.ToModel(model.Address);
            await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);

            return model;
        }

        /// <summary>
        /// Prepare paged vendor note list model
        /// </summary>
        /// <param name="searchModel">Vendor note search model</param>
        /// <param name="vendor">Vendor</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor note list model
        /// </returns>
        public virtual async Task<VendorNoteListModel> PrepareVendorNoteListModelAsync(VendorNoteSearchModel searchModel, Vendor vendor)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            //get vendor notes
            var vendorNotes = await _vendorService.GetVendorNotesByVendorAsync(vendor.Id, searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = await new VendorNoteListModel().PrepareToGridAsync(searchModel, vendorNotes, () =>
            {
                //fill in model values from the entity
                return vendorNotes.SelectAwait(async note =>
                {
                    //fill in model values from the entity        
                    var vendorNoteModel = note.ToModel<VendorNoteModel>();

                    //convert dates to the user time
                    vendorNoteModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(note.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    vendorNoteModel.Note = _vendorService.FormatVendorNoteText(note);

                    return vendorNoteModel;
                });
            });

            return model;
        }

        #endregion
    }
}