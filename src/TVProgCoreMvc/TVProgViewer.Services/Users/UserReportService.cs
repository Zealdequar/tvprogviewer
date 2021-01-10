using System;
using System.Linq;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Payments;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Data;
using TVProgViewer.Services.Helpers;

namespace TVProgViewer.Services.Users
{
    /// <summary>
    /// User report service
    /// </summary>
    public partial class UserReportService : IUserReportService
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Order> _orderRepository;

        #endregion

        #region Ctor

        public UserReportService(IUserService UserService,
            IDateTimeHelper dateTimeHelper,
            IRepository<User> UserRepository,
            IRepository<Order> orderRepository)
        {
            _userService = UserService;
            _dateTimeHelper = dateTimeHelper;
            _userRepository = UserRepository;
            _orderRepository = orderRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Получить лучших пользователей
        /// </summary>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Order shipment status; null to load all records</param>
        /// <param name="orderBy">1 - order by order total, 2 - order by number of orders</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Report</returns>
        public virtual IPagedList<BestUserReportLine> GetBestUsersReport(DateTime? createdFromUtc,
            DateTime? createdToUtc, OrderStatus? os, PaymentStatus? ps, ShippingStatus? ss, int orderBy,
            int pageIndex = 0, int pageSize = 214748364)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

            int? shippingStatusId = null;
            if (ss.HasValue)
                shippingStatusId = (int)ss.Value;
            var query1 = from c in _userRepository.Table
                         join o in _orderRepository.Table on c.Id equals o.UserId
                         where (!createdFromUtc.HasValue || createdFromUtc.Value <= o.CreatedOnUtc) &&
                         (!createdToUtc.HasValue || createdToUtc.Value >= o.CreatedOnUtc) &&
                         (!orderStatusId.HasValue || orderStatusId == o.OrderStatusId) &&
                         (!paymentStatusId.HasValue || paymentStatusId == o.PaymentStatusId) &&
                         (!shippingStatusId.HasValue || shippingStatusId == o.ShippingStatusId) &&
                         !o.Deleted &&
                         c.Deleted == null
                         select new { c, o };

            var query2 = from co in query1
                         group co by co.c.Id into g
                         select new
                         {
                             UserId = g.Key,
                             OrderTotal = g.Sum(x => x.o.OrderTotal),
                             OrderCount = g.Count()
                         };
            query2 = orderBy switch
            {
                1 => query2.OrderByDescending(x => x.OrderTotal),
                2 => query2 = query2.OrderByDescending(x => x.OrderCount),
                _ => throw new ArgumentException("Wrong orderBy parameter", nameof(orderBy))
            };

            var tmp = new PagedList<dynamic>(query2, pageIndex, pageSize);
            return new PagedList<BestUserReportLine>(tmp.Select(x => new BestUserReportLine
            {
                UserId = x.UserId,
                OrderTotal = x.OrderTotal,
                OrderCount = x.OrderCount
            }),
                tmp.PageIndex, tmp.PageSize, tmp.TotalCount);
        }

        /// <summary>
        /// Gets a report of Users registered in the last days
        /// </summary>
        /// <param name="days">Users registered in the last days</param>
        /// <returns>Number of registered Users</returns>
        public virtual int GetRegisteredUsersReport(int days)
        {
            var date = _dateTimeHelper.ConvertToUserTime(DateTime.Now).AddDays(-days);

            var registeredUserRole = _userService.GetUserRoleBySystemName(TvProgUserDefaults.RegisteredRoleName);
            if (registeredUserRole == null)
                return 0;

            return _userService.GetAllUsers(
                createdFromUtc: date,
                userRoleIds: new int[] { registeredUserRole.Id }).Count();
        }

        #endregion
    }
}