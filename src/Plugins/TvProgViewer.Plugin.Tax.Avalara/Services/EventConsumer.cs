using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Events;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Events;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Tax;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.WebUI.Areas.Admin.Models.Users;
using TvProgViewer.WebUI.Areas.Admin.Models.Orders;
using TvProgViewer.Web.Framework.Events;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.User;

namespace TvProgViewer.Plugin.Tax.Avalara.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<UserActivatedEvent>,
        IConsumer<UserPermanentlyDeleted>,
        IConsumer<EntityDeletedEvent<Order>>,
        IConsumer<ModelPreparedEvent<BaseTvProgModel>>,
        IConsumer<ModelReceivedEvent<BaseTvProgModel>>,
        IConsumer<OrderStatusChangedEvent>,
        IConsumer<OrderPlacedEvent>,
        IConsumer<OrderRefundedEvent>,
        IConsumer<OrderVoidedEvent>
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly AvalaraTaxSettings _avalaraTaxSettings;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IUserService _userService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly ITaxPluginManager _taxPluginManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public EventConsumer(AvalaraTaxManager avalaraTaxManager,
            AvalaraTaxSettings avalaraTaxSettings,
            ICheckoutAttributeService checkoutAttributeService,
            IUserService userService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IProductService productService,
            ITaxPluginManager taxPluginManager,
            IWorkContext workContext)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _avalaraTaxSettings = avalaraTaxSettings;
            _checkoutAttributeService = checkoutAttributeService;
            _userService = userService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _productService = productService;
            _taxPluginManager = taxPluginManager;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle user activated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(UserActivatedEvent eventMessage)
        {
            if (eventMessage.User is null)
                return;

            //ensure that Avalara tax provider is active for the passed user, since it's the event from the public area
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, eventMessage.User))
                return;

            if (!_avalaraTaxSettings.EnableCertificates)
                return;

            //create user
            await _avalaraTaxManager.CreateOrUpdateUserAsync(eventMessage.User);
        }

        /// <summary>
        /// Handle user permanently deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(UserPermanentlyDeleted eventMessage)
        {
            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return;

            if (!_avalaraTaxSettings.EnableCertificates)
                return;

            //delete user
            //await _avalaraTaxManager.DeleteUserAsync(eventMessage.UserId);
        }

        /// <summary>
        /// Handle model prepared event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ModelPreparedEvent<BaseTvProgModel> eventMessage)
        {
            var userModel = eventMessage.Model as UserInfoModel;
            var navigationModel = eventMessage.Model as UserNavigationModel;
            if (userModel is null && navigationModel is null)
                return;

            //ensure that Avalara tax provider is active for the passed user, since it's the event from the public area
            var user = await _workContext.GetCurrentUserAsync();
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, user))
                return;

            if (!_avalaraTaxSettings.EnableCertificates)
                return;

            if (navigationModel is not null)
            {
                //ACL
                if (_avalaraTaxSettings.UserRoleIds.Any())
                {
                    var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
                    if (!userRoleIds.Intersect(_avalaraTaxSettings.UserRoleIds).Any())
                        return;
                }

                var infoItem = navigationModel.UserNavigationItems.FirstOrDefault(item => item.Tab == (int)UserNavigationEnum.Info);
                var position = navigationModel.UserNavigationItems.IndexOf(infoItem) + 1;
                navigationModel.UserNavigationItems.Insert(position, new UserNavigationItemModel
                {
                    RouteName = AvalaraTaxDefaults.ExemptionCertificatesRouteName,
                    ItemClass = AvalaraTaxDefaults.ExemptionCertificatesMenuClassName,
                    Tab = AvalaraTaxDefaults.ExemptionCertificatesMenuTab,
                    Title = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.ExemptionCertificates")
                });
            }

            if (userModel is not null && !_avalaraTaxSettings.AllowEditUser)
                await _avalaraTaxManager.CreateOrUpdateUserAsync(user);
        }

        /// <summary>
        /// Handle model received event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ModelReceivedEvent<BaseTvProgModel> eventMessage)
        {
            //get entity by received model
            var entity = eventMessage.Model switch
            {
                UserModel userModel => (BaseEntity)await _userService.GetUserByIdAsync(userModel.Id),
                UserRoleModel userRoleModel => await _userService.GetUserRoleByIdAsync(userRoleModel.Id),
                ProductModel productModel => await _productService.GetProductByIdAsync(productModel.Id),
                CheckoutAttributeModel checkoutAttributeModel => await _checkoutAttributeService.GetCheckoutAttributeByIdAsync(checkoutAttributeModel.Id),
                _ => null
            };
            if (entity == null)
                return;

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return;

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return;

            //whether there is a form value for the entity use code
            if (_httpContextAccessor.HttpContext.Request.Form.TryGetValue(AvalaraTaxDefaults.EntityUseCodeAttribute, out var entityUseCodeValue)
                && !StringValues.IsNullOrEmpty(entityUseCodeValue))
            {
                //save attribute
                var entityUseCode = !entityUseCodeValue.ToString().Equals(Guid.Empty.ToString()) ? entityUseCodeValue.ToString() : null;
                await _genericAttributeService.SaveAttributeAsync(entity, AvalaraTaxDefaults.EntityUseCodeAttribute, entityUseCode);
            }
        }

        /// <summary>
        /// Handle order placed event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            if (eventMessage.Order == null)
                return;

            //ensure that Avalara tax provider is active for the passed user, since it's the event from the public area
            var user = await _userService.GetUserByIdAsync(eventMessage.Order.UserId);
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName, user))
                return;

            //create tax transaction
            await _avalaraTaxManager.CreateOrderTaxTransactionAsync(eventMessage.Order);
        }

        /// <summary>
        /// Handle order refunded event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderRefundedEvent eventMessage)
        {
            if (eventMessage.Order == null)
                return;

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return;

            //refund tax transaction
            await _avalaraTaxManager.RefundTaxTransactionAsync(eventMessage.Order, eventMessage.Amount);
        }

        /// <summary>
        /// Handle order voided event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderVoidedEvent eventMessage)
        {
            if (eventMessage.Order == null)
                return;

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return;

            //void tax transaction
            await _avalaraTaxManager.VoidTaxTransactionAsync(eventMessage.Order);
        }

        /// <summary>
        /// Handle order cancelled event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderStatusChangedEvent eventMessage)
        {
            if (eventMessage.Order == null)
                return;

            if (eventMessage.Order.OrderStatus != OrderStatus.Cancelled)
                return;

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return;

            //void tax transaction
            await _avalaraTaxManager.VoidTaxTransactionAsync(eventMessage.Order);
        }

        /// <summary>
        /// Handle order deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Order> eventMessage)
        {
            if (eventMessage.Entity == null)
                return;

            //ensure that Avalara tax provider is active
            if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
                return;

            //delete tax transaction
            await _avalaraTaxManager.DeleteTaxTransactionAsync(eventMessage.Entity);
        }

        #endregion
    }
}