using System;
using System.Globalization;
using System.Linq;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Helpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Orders;
using TVProgViewer.Services.Payments;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Orders;
using TVProgViewer.Web.Framework.Models.Extensions;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the recurring payment model factory implementation
    /// </summary>
    public partial class RecurringPaymentModelFactory : IRecurringPaymentModelFactory
    {
        #region Fields

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public RecurringPaymentModelFactory(IDateTimeHelper dateTimeHelper,
            IUserService userService,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentService paymentService,
            IWorkContext workContext)
        {
            _dateTimeHelper = dateTimeHelper;
            _userService = userService;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentService = paymentService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare recurring payment history search model
        /// </summary>
        /// <param name="searchModel">Recurring payment history search model</param>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>Recurring payment history search model</returns>
        protected virtual RecurringPaymentHistorySearchModel PrepareRecurringPaymentHistorySearchModel(RecurringPaymentHistorySearchModel searchModel,
            RecurringPayment recurringPayment)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            searchModel.RecurringPaymentId = recurringPayment.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare recurring payment search model
        /// </summary>
        /// <param name="searchModel">Recurring payment search model</param>
        /// <returns>Recurring payment search model</returns>
        public virtual RecurringPaymentSearchModel PrepareRecurringPaymentSearchModel(RecurringPaymentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged recurring payment list model
        /// </summary>
        /// <param name="searchModel">Recurring payment search model</param>
        /// <returns>Recurring payment list model</returns>
        public virtual RecurringPaymentListModel PrepareRecurringPaymentListModel(RecurringPaymentSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get recurringPayments
            var recurringPayments = _orderService.SearchRecurringPayments(showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new RecurringPaymentListModel().PrepareToGrid(searchModel, recurringPayments, () =>
            {
                return recurringPayments.Select(recurringPayment =>
                {
                    //fill in model values from the entity
                    var recurringPaymentModel = recurringPayment.ToModel<RecurringPaymentModel>();

                    var order = _orderService.GetOrderById(recurringPayment.InitialOrderId);
                    var user = _userService.GetUserById(order.UserId);

                    //convert dates to the user time
                    if (_orderProcessingService.GetNextPaymentDate(recurringPayment) is DateTime nextPaymentDate)
                    {
                        recurringPaymentModel.NextPaymentDate = _dateTimeHelper
                            .ConvertToUserTime(nextPaymentDate, DateTimeKind.Utc).ToString(CultureInfo.InvariantCulture);
                        recurringPaymentModel.CyclesRemaining = _orderProcessingService.GetCyclesRemaining(recurringPayment);
                    }

                    recurringPaymentModel.StartDate = _dateTimeHelper
                        .ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(CultureInfo.InvariantCulture);

                    //fill in additional values (not existing in the entity)
                    recurringPaymentModel.UserId = user.Id;
                    recurringPaymentModel.InitialOrderId = order.Id;
                    recurringPaymentModel.CyclePeriodStr = _localizationService.GetLocalizedEnum(recurringPayment.CyclePeriod);
                    recurringPaymentModel.UserEmail = _userService.IsRegistered(user)
                        ? user.Email : _localizationService.GetResource("Admin.Users.Guest");

                    return recurringPaymentModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare recurring payment model
        /// </summary>
        /// <param name="model">Recurring payment model</param>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Recurring payment model</returns>
        public virtual RecurringPaymentModel PrepareRecurringPaymentModel(RecurringPaymentModel model,
            RecurringPayment recurringPayment, bool excludeProperties = false)
        {
            if (recurringPayment == null)
                return model;

            //fill in model values from the entity
            if (model == null)
                model = recurringPayment.ToModel<RecurringPaymentModel>();

            var order = _orderService.GetOrderById(recurringPayment.InitialOrderId);
            var user = _userService.GetUserById(order.UserId);

            //convert dates to the user time
            if (_orderProcessingService.GetNextPaymentDate(recurringPayment) is DateTime nextPaymentDate)
            {
                model.NextPaymentDate = _dateTimeHelper.ConvertToUserTime(nextPaymentDate, DateTimeKind.Utc).ToString(CultureInfo.InvariantCulture);
                model.CyclesRemaining = _orderProcessingService.GetCyclesRemaining(recurringPayment);
            }
            model.StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(CultureInfo.InvariantCulture);

            model.UserId = user.Id;
            model.InitialOrderId = order.Id;
            model.UserEmail = _userService.IsRegistered(user)
                ? user.Email : _localizationService.GetResource("Admin.Users.Guest");
            model.PaymentType = _localizationService.GetLocalizedEnum(_paymentService
                .GetRecurringPaymentType(order.PaymentMethodSystemName));
            model.CanCancelRecurringPayment = _orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentUser, recurringPayment);
            
            //prepare nested search model
            PrepareRecurringPaymentHistorySearchModel(model.RecurringPaymentHistorySearchModel, recurringPayment);

            return model;
        }

        /// <summary>
        /// Prepare paged recurring payment history list model
        /// </summary>
        /// <param name="searchModel">Recurring payment history search model</param>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>Recurring payment history list model</returns>
        public virtual RecurringPaymentHistoryListModel PrepareRecurringPaymentHistoryListModel(RecurringPaymentHistorySearchModel searchModel,
            RecurringPayment recurringPayment)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            //get recurring payments history
            var recurringPayments = _orderService.GetRecurringPaymentHistory(recurringPayment)
                .OrderBy(historyEntry => historyEntry.CreatedOnUtc).ToList()
                .ToPagedList(searchModel);

            //prepare list model
            var model = new RecurringPaymentHistoryListModel().PrepareToGrid(searchModel, recurringPayments, () =>
            {
                return recurringPayments.Select(historyEntry =>
                {
                    //fill in model values from the entity
                    var historyModel = historyEntry.ToModel<RecurringPaymentHistoryModel>();

                    //convert dates to the user time
                    historyModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(historyEntry.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    var order = _orderService.GetOrderById(historyEntry.OrderId);
                    if (order == null)
                        return historyModel;

                    historyModel.OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus);
                    historyModel.PaymentStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus);
                    historyModel.ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus);
                    historyModel.CustomOrderNumber = order.CustomOrderNumber;

                    return historyModel;
                });
            });

            return model;
        }

        #endregion
    }
}