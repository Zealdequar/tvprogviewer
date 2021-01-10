using System;
using System.Linq;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Helpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Media;
using TVProgViewer.Services.Orders;
using TVProgViewer.Services.Seo;
using TVProgViewer.WebUI.Models.Order;

namespace TVProgViewer.WebUI.Factories
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
        private readonly IProductService _productService;
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
            IProductService productService,
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
            _productService = productService;
            _returnRequestService = returnRequestService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _orderSettings = orderSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the order item model
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>Order item model</returns>
        public virtual SubmitReturnRequestModel.OrderItemModel PrepareSubmitReturnRequestOrderItemModel(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            var order = _orderService.GetOrderById(orderItem.OrderId);
            var product = _productService.GetProductById(orderItem.ProductId);

            var model = new SubmitReturnRequestModel.OrderItemModel
            {
                Id = orderItem.Id,
                ProductId = product.Id,
                ProductName = _localizationService.GetLocalized(product, x => x.Name),
                ProductSeName = _urlRecordService.GetSeName(product),
                AttributeInfo = orderItem.AttributeDescription,
                Quantity = orderItem.Quantity
            };

            //unit price
            if (order.UserTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax
                var unitPriceInclTaxInUserCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                model.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInUserCurrency, true, order.UserCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax
                var unitPriceExclTaxInUserCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                model.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInUserCurrency, true, order.UserCurrencyCode, _workContext.WorkingLanguage, false);
            }

            return model;
        }

        /// <summary>
        /// Prepare the submit return request model
        /// </summary>
        /// <param name="model">Submit return request model</param>
        /// <param name="order">Order</param>
        /// <returns>Submit return request model</returns>
        public virtual SubmitReturnRequestModel PrepareSubmitReturnRequestModel(SubmitReturnRequestModel model,
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
            model.AvailableReturnReasons = _returnRequestService.GetAllReturnRequestReasons()
                .Select(rrr => new SubmitReturnRequestModel.ReturnRequestReasonModel
                {
                    Id = rrr.Id,
                    Name = _localizationService.GetLocalized(rrr, x => x.Name)
                }).ToList();

            //return actions
            model.AvailableReturnActions = _returnRequestService.GetAllReturnRequestActions()
                .Select(rra => new SubmitReturnRequestModel.ReturnRequestActionModel
                {
                    Id = rra.Id,
                    Name = _localizationService.GetLocalized(rra, x => x.Name)
                })
                .ToList();

            //returnable products
            var orderItems = _orderService.GetOrderItems(order.Id, isNotReturnable: false);
            foreach (var orderItem in orderItems)
            {
                var orderItemModel = PrepareSubmitReturnRequestOrderItemModel(orderItem);
                model.Items.Add(orderItemModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the user return requests model
        /// </summary>
        /// <returns>User return requests model</returns>
        public virtual UserReturnRequestsModel PrepareUserReturnRequestsModel()
        {
            var model = new UserReturnRequestsModel();

            var returnRequests = _returnRequestService.SearchReturnRequests(_storeContext.CurrentStore.Id, _workContext.CurrentUser.Id);
            foreach (var returnRequest in returnRequests)
            {
                var orderItem = _orderService.GetOrderItemById(returnRequest.OrderItemId);
                if (orderItem != null)
                {
                    var product = _productService.GetProductById(orderItem.ProductId);

                    var download = _downloadService.GetDownloadById(returnRequest.UploadedFileId);

                    var itemModel = new UserReturnRequestsModel.ReturnRequestModel
                    {
                        Id = returnRequest.Id,
                        CustomNumber = returnRequest.CustomNumber,
                        ReturnRequestStatus = _localizationService.GetLocalizedEnum(returnRequest.ReturnRequestStatus),
                        ProductId = product.Id,
                        ProductName = _localizationService.GetLocalized(product, x => x.Name),
                        ProductSeName = _urlRecordService.GetSeName(product),
                        Quantity = returnRequest.Quantity,
                        ReturnAction = returnRequest.RequestedAction,
                        ReturnReason = returnRequest.ReasonForReturn,
                        Comments = returnRequest.UserComments,
                        UploadedFileGuid = download?.DownloadGuid ?? Guid.Empty,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc),
                    };
                    model.Items.Add(itemModel);
                }
            }

            return model;
        }

        #endregion
    }
}