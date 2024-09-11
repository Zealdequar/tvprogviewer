using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Events;
using TvProgViewer.Services.Events;
using TvProgViewer.Services.Messages;
using TvProgViewer.Web.Framework;
using TvProgViewer.Web.Framework.Events;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.Catalog;

namespace TvProgViewer.Plugin.Widgets.FacebookPixel.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<UserRegisteredEvent>,
        IConsumer<EntityInsertedEvent<ShoppingCartItem>>,
        IConsumer<MessageTokensAddedEvent<Token>>,
        IConsumer<ModelPreparedEvent<BaseTvProgModel>>,
        IConsumer<OrderPlacedEvent>,
        IConsumer<PageRenderingEvent>,
        IConsumer<TvChannelSearchEvent>
    {
        #region Fields

        private readonly FacebookPixelService _facebookPixelService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Ctor

        public EventConsumer(FacebookPixelService facebookPixelService,
            IHttpContextAccessor httpContextAccessor)
        {
            _facebookPixelService = facebookPixelService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle shopping cart item inserted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
        {
            if (eventMessage?.Entity != null)
                await _facebookPixelService.SendAddToCartEventAsync(eventMessage.Entity);
        }

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            if (eventMessage?.Order != null)
                await _facebookPixelService.SendPurchaseEventAsync(eventMessage.Order);
        }

        /// <summary>
        /// Handle tvChannel details model prepared event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ModelPreparedEvent<BaseTvProgModel> eventMessage)
        {
            if (eventMessage?.Model is TvChannelDetailsModel tvChannelDetailsModel)
                await _facebookPixelService.SendViewContentEventAsync(tvChannelDetailsModel);
        }

        /// <summary>
        /// Handle page rendering event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(PageRenderingEvent eventMessage)
        {
            var routeName = eventMessage.GetRouteName() ?? string.Empty;
            if (routeName == FacebookPixelDefaults.CheckoutRouteName || routeName == FacebookPixelDefaults.CheckoutOnePageRouteName)
                await _facebookPixelService.SendInitiateCheckoutEventAsync();

            if (_httpContextAccessor.HttpContext.GetRouteValue("area") is not string area || area != AreaNames.Admin)
                await _facebookPixelService.SendPageViewEventAsync();
        }

        /// <summary>
        /// Handle tvChannel search event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(TvChannelSearchEvent eventMessage)
        {
            if (eventMessage?.SearchTerm != null)
                await _facebookPixelService.SendSearchEventAsync(eventMessage.SearchTerm);
        }

        /// <summary>
        /// Handle message token added event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(MessageTokensAddedEvent<Token> eventMessage)
        {
            if (eventMessage?.Message?.Name == MessageTemplateSystemNames.ContactUsMessage)
                await _facebookPixelService.SendContactEventAsync();
        }

        /// <summary>
        /// Handle user registered event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(UserRegisteredEvent eventMessage)
        {
            if (eventMessage?.User != null)
                await _facebookPixelService.SendCompleteRegistrationEventAsync();
        }

        #endregion
    }
}