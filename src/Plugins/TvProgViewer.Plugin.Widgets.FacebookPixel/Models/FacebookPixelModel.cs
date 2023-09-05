using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Widgets.FacebookPixel.Models
{
    /// <summary>
    /// Represents a Facebook Pixel model
    /// </summary>
    public record FacebookPixelModel : BaseTvProgEntityModel
    {
        #region Ctor

        public FacebookPixelModel()
        {
            AvailableStores = new List<SelectListItem>();
            CustomEventSearchModel = new CustomEventSearchModel();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelId")]
        public string PixelId { get; set; }
        
        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.AccessToken")]
        [DataType(DataType.Password)]
        public string AccessToken { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.DisableForUsersNotAcceptingCookieConsent")]
        public bool DisableForUsersNotAcceptingCookieConsent { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.PixelScriptEnabled")]
        public bool PixelScriptEnabled { get; set; }
        
        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.ConversionsApiEnabled")]
        public bool ConversionsApiEnabled { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.Store")]
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public bool HideStoresList { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.UseAdvancedMatching")]
        public bool UseAdvancedMatching { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.PassUserProperties")]
        public bool PassUserProperties { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackPageView")]
        public bool TrackPageView { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackAddToCart")]
        public bool TrackAddToCart { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackPurchase")]
        public bool TrackPurchase { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackViewContent")]
        public bool TrackViewContent { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackAddToWishlist")]
        public bool TrackAddToWishlist { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackInitiateCheckout")]
        public bool TrackInitiateCheckout { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackSearch")]
        public bool TrackSearch { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackContact")]
        public bool TrackContact { get; set; }

        [TvProgResourceDisplayName("Plugins.Widgets.FacebookPixel.Configuration.Fields.TrackCompleteRegistration")]
        public bool TrackCompleteRegistration { get; set; }

        public bool HideCustomEventsSearch { get; set; }

        public CustomEventSearchModel CustomEventSearchModel { get; set; }

        #endregion
    }
}