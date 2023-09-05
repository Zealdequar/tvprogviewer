using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Plugin.Tax.Avalara.Models.EntityUseCode;
using TvProgViewer.Plugin.Tax.Avalara.Services;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Tax;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.WebUI.Areas.Admin.Models.Users;
using TvProgViewer.WebUI.Areas.Admin.Models.Orders;
using TvProgViewer.Web.Framework.Components;
using TvProgViewer.Web.Framework.Infrastructure;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Tax.Avalara.Components
{
    /// <summary>
    /// Represents a view component to render an additional field on user details, user role details, product details, checkout attribute details views
    /// </summary>
    public class EntityUseCodeViewComponent : TvProgViewComponent
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IUserService _userService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ITaxPluginManager _taxPluginManager;

        #endregion

        #region Ctor

        public EntityUseCodeViewComponent(AvalaraTaxManager avalaraTaxManager,
            ICheckoutAttributeService checkoutAttributeService,
            IUserService userService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IProductService productService,
            IStaticCacheManager staticCacheManager,
            ITaxPluginManager taxPluginManager)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _checkoutAttributeService = checkoutAttributeService;
            _userService = userService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _productService = productService;
            _staticCacheManager = staticCacheManager;
            _taxPluginManager = taxPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke the widget view component
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="additionalData">Additional parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            //ensure that model is passed
            if (additionalData is not BaseTvProgEntityModel entityModel)
                return Content(string.Empty);

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return Content(string.Empty);

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content(string.Empty);

            //ensure that it's a proper widget zone
            if (!widgetZone.Equals(AdminWidgetZones.UserDetailsBlock) &&
                !widgetZone.Equals(AdminWidgetZones.UserRoleDetailsTop) &&
                !widgetZone.Equals(AdminWidgetZones.ProductDetailsBlock) &&
                !widgetZone.Equals(AdminWidgetZones.CheckoutAttributeDetailsBlock))
            {
                return Content(string.Empty);
            }

            //get Avalara pre-defined entity use codes
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(AvalaraTaxDefaults.EntityUseCodesCacheKey);
            var cachedEntityUseCodes = await _staticCacheManager.GetAsync(cacheKey, async () => await _avalaraTaxManager.GetEntityUseCodesAsync());

            var entityUseCodes = cachedEntityUseCodes?.Select(useCode => new SelectListItem
            {
                Value = useCode.code,
                Text = $"{useCode.name} ({string.Join(", ", useCode.validCountries)})"
            }).ToList() ?? new List<SelectListItem>();

            //add the special item for 'undefined' with empty guid value
            var defaultValue = Guid.Empty.ToString();
            entityUseCodes.Insert(0, new SelectListItem
            {
                Value = defaultValue,
                Text = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.Fields.EntityUseCode.None")
            });

            //prepare model
            var model = new EntityUseCodeModel
            {
                Id = entityModel.Id,
                EntityUseCodes = entityUseCodes
            };

            //get entity by the model identifier
            BaseEntity entity = null;
            if (widgetZone.Equals(AdminWidgetZones.UserDetailsBlock))
            {
                model.PrecedingElementId = nameof(UserModel.IsTaxExempt);
                entity = await _userService.GetUserByIdAsync(entityModel.Id);
            }

            if (widgetZone.Equals(AdminWidgetZones.UserRoleDetailsTop))
            {
                model.PrecedingElementId = nameof(UserRoleModel.TaxExempt);
                entity = await _userService.GetUserRoleByIdAsync(entityModel.Id);
            }

            if (widgetZone.Equals(AdminWidgetZones.ProductDetailsBlock))
            {
                model.PrecedingElementId = nameof(ProductModel.IsTaxExempt);
                entity = await _productService.GetProductByIdAsync(entityModel.Id);
            }

            if (widgetZone.Equals(AdminWidgetZones.CheckoutAttributeDetailsBlock))
            {
                model.PrecedingElementId = nameof(CheckoutAttributeModel.IsTaxExempt);
                entity = await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(entityModel.Id);
            }

            //try to get previously saved entity use code
            model.AvalaraEntityUseCode = entity == null ? defaultValue :
                await _genericAttributeService.GetAttributeAsync<string>(entity, AvalaraTaxDefaults.EntityUseCodeAttribute);

            return View("~/Plugins/Tax.Avalara/Views/EntityUseCode/EntityUseCode.cshtml", model);
        }

        #endregion
    }
}