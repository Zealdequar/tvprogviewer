using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Seo;
using TvProgViewer.WebUI.Models.Order;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the return request model factory
    /// </summary>
    public partial class ReturnRequestModelFactory : IReturnRequestModelFactory
    {
        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITvChannelService _tvchannelService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;

        #endregion

        #region Ctor

        public ReturnRequestModelFactory(ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            IOrderService orderService,
            IPriceFormatter priceFormatter,
            ITvChannelService tvchannelService,
            IReturnRequestService returnRequestService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            OrderSettings orderSettings)
        {
            _currencyService = currencyService;
            _dateTimeHelper = dateTimeHelper;
            _downloadService = downloadService;
            _localizationService = localizationService;
            _orderService = orderService;
            _priceFormatter = priceFormatter;
            _tvchannelService = tvchannelService;
            _returnRequestService = returnRequestService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _orderSettings = orderSettings;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Prepare the submit return request model
        /// </summary>
        /// <param name="model">Submit return request model</param>
        /// <param name="order">Order</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the submit return request model
        /// </returns>
        public virtual async Task<SubmitReturnRequestModel> PrepareSubmitReturnRequestModelAsync(SubmitReturnRequestModel model,
            Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.OrderId = order.Id;
            model.AllowFiles = _orderSettings.ReturnRequestsAllowFiles;
            model.CustomOrderNumber = order.CustomOrderNumber;

            //return reasons
            model.AvailableReturnReasons = await (await _returnRequestService.GetAllReturnRequestReasonsAsync())
                .SelectAwait(async rrr => new SubmitReturnRequestModel.ReturnRequestReasonModel
                {
                    Id = rrr.Id,
                    Name = await _localizationService.GetLocalizedAsync(rrr, x => x.Name)
                }).ToListAsync();

            //return actions
            model.AvailableReturnActions = await (await _returnRequestService.GetAllReturnRequestActionsAsync())
                .SelectAwait(async rra => new SubmitReturnRequestModel.ReturnRequestActionModel
                {
                    Id = rra.Id,
                    Name = await _localizationService.GetLocalizedAsync(rra, x => x.Name)
                })
                .ToListAsync();

            //returnable tvchannels
            model.Items = await PrepareSubmitReturnRequestOrderItemModelsAsync(order);

            return model;
        }

        /// <summary>
        /// Prepare the user return requests model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user return requests model
        /// </returns>
        public virtual async Task<UserReturnRequestsModel> PrepareUserReturnRequestsModelAsync()
        {
            var model = new UserReturnRequestsModel();
            var store = await _storeContext.GetCurrentStoreAsync();
            var user = await _workContext.GetCurrentUserAsync();
            var returnRequests = await _returnRequestService.SearchReturnRequestsAsync(store.Id, user.Id);
            
            foreach (var returnRequest in returnRequests)
            {
                var orderItem = await _orderService.GetOrderItemByIdAsync(returnRequest.OrderItemId);
                if (orderItem != null)
                {
                    var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(orderItem.TvChannelId);

                    var download = await _downloadService.GetDownloadByIdAsync(returnRequest.UploadedFileId);

                    var itemModel = new UserReturnRequestsModel.ReturnRequestModel
                    {
                        Id = returnRequest.Id,
                        CustomNumber = returnRequest.CustomNumber,
                        ReturnRequestStatus = await _localizationService.GetLocalizedEnumAsync(returnRequest.ReturnRequestStatus),
                        TvChannelId = tvchannel.Id,
                        TvChannelName = await _localizationService.GetLocalizedAsync(tvchannel, x => x.Name),
                        TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvchannel),
                        Quantity = returnRequest.Quantity,
                        ReturnAction = returnRequest.RequestedAction,
                        ReturnReason = returnRequest.ReasonForReturn,
                        Comments = returnRequest.UserComments,
                        UploadedFileGuid = download?.DownloadGuid ?? Guid.Empty,
                        CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(returnRequest.CreatedOnUtc, DateTimeKind.Utc),
                    };
                    model.Items.Add(itemModel);
                }
            }

            return model;
        }

        /// <summary>
        /// Prepares the order item models for return request by specified order.
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// The <see cref="Task"/> containing the <see cref="IList{SubmitReturnRequestModel.OrderItemModel}"/>
        /// </returns>
        protected virtual async Task<IList<SubmitReturnRequestModel.OrderItemModel>> PrepareSubmitReturnRequestOrderItemModelsAsync(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var models = new List<SubmitReturnRequestModel.OrderItemModel>();

            var returnRequestAvailability = await _returnRequestService.GetReturnRequestAvailabilityAsync(order.Id);
            if (returnRequestAvailability?.IsAllowed == true)
            {
                foreach (var returnableOrderItem in returnRequestAvailability.ReturnableOrderItems)
                {
                    if (returnableOrderItem.AvailableQuantityForReturn == 0)
                        continue;

                    var orderItem = returnableOrderItem.OrderItem;
                    var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(orderItem.TvChannelId);

                    var model = new SubmitReturnRequestModel.OrderItemModel
                    {
                        Id = orderItem.Id,
                        TvChannelId = tvchannel.Id,
                        TvChannelName = await _localizationService.GetLocalizedAsync(tvchannel, x => x.Name),
                        TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvchannel),
                        AttributeInfo = orderItem.AttributeDescription,
                        Quantity = returnableOrderItem.AvailableQuantityForReturn
                    };

                    var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;

                    //unit price
                    if (order.UserTaxDisplayType == TaxDisplayType.IncludingTax)
                    {
                        //including tax
                        var unitPriceInclTaxInUserCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                        model.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, true);
                    }
                    else
                    {
                        //excluding tax
                        var unitPriceExclTaxInUserCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                        model.UnitPrice = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInUserCurrency, true, order.UserCurrencyCode, languageId, false);
                    }

                    models.Add(model);
                }
            }

            return models;
        }

        #endregion
    }
}