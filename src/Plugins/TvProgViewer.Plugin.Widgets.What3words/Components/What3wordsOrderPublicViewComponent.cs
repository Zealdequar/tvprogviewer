using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Services.Cms;
using TvProgViewer.Services.Common;
using TvProgViewer.Web.Framework.Components;
using TvProgViewer.Web.Framework.Infrastructure;
using TvProgViewer.WebUI.Models.Order;
using TvProgViewer.WebUI.Models.ShoppingCart;

namespace TvProgViewer.Plugin.Widgets.What3words.Components
{
    public class What3wordsOrderPublicViewComponent : TvProgViewComponent
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IWorkContext _workContext;
        private readonly What3wordsSettings _what3WordsSettings;

        #endregion

        #region Ctor

        public What3wordsOrderPublicViewComponent(IAddressService addressService,
            IGenericAttributeService genericAttributeService,
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext,
            What3wordsSettings what3WordsSettings)
        {
            _addressService = addressService;
            _genericAttributeService = genericAttributeService;
            _widgetPluginManager = widgetPluginManager;
            _workContext = workContext;
            _what3WordsSettings = what3WordsSettings;
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
            //ensure that what3words widget is active and enabled
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _widgetPluginManager.IsPluginActiveAsync(What3wordsDefaults.SystemName, user))
                return Content(string.Empty);

            if (!_what3WordsSettings.Enabled)
                return Content(string.Empty);

            var summaryModel = additionalData as ShoppingCartModel.OrderReviewDataModel;
            var detailsModel = additionalData as OrderDetailsModel;
            if (summaryModel is null && detailsModel is null)
                return Content(string.Empty);

            var addressId = 0;
            if (widgetZone.Equals(PublicWidgetZones.OrderSummaryBillingAddress))
                addressId = summaryModel.BillingAddress.Id;
            if (widgetZone.Equals(PublicWidgetZones.OrderSummaryShippingAddress))
                addressId = summaryModel.ShippingAddress.Id;
            if (widgetZone.Equals(PublicWidgetZones.OrderDetailsBillingAddress))
                addressId = detailsModel.BillingAddress.Id;
            if (widgetZone.Equals(PublicWidgetZones.OrderDetailsShippingAddress))
                addressId = detailsModel.ShippingAddress.Id;
            var address = await _addressService.GetAddressByIdAsync(addressId);
            var addressValue = address is not null
                ? await _genericAttributeService.GetAttributeAsync<string>(address, What3wordsDefaults.AddressValueAttribute)
                : null;
            if (string.IsNullOrEmpty(addressValue))
                return Content(string.Empty);

            return View("~/Plugins/Widgets.What3words/Views/PublicOrderAddress.cshtml", addressValue);
        }

        #endregion
    }
}
