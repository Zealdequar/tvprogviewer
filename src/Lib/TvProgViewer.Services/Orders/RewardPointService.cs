﻿using System;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Data;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;

namespace TvProgViewer.Services.Orders
{
    /// <summary>
    /// Reward point service
    /// </summary>
    public partial class RewardPointService : IRewardPointService
    {
        #region Fields

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<RewardPointsHistory> _rewardPointsHistoryRepository;
        private readonly RewardPointsSettings _rewardPointsSettings;

        #endregion

        #region Ctor

        public RewardPointService(IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IRepository<RewardPointsHistory> rewardPointsHistoryRepository,
            RewardPointsSettings rewardPointsSettings)
        {
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _rewardPointsHistoryRepository = rewardPointsHistoryRepository;
            _rewardPointsSettings = rewardPointsSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get query to load reward points history
        /// </summary>
        /// <param name="userId">User identifier; pass 0 to load all records</param>
        /// <param name="storeId">Store identifier; pass null to load all records</param>
        /// <param name="showNotActivated">Whether to load reward points that did not yet activated</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the query to load reward points history
        /// </returns>
        protected virtual async Task<IQueryable<RewardPointsHistory>> GetRewardPointsQueryAsync(int userId, int? storeId, bool showNotActivated = false)
        {
            var query = _rewardPointsHistoryRepository.Table;

            //filter by user
            if (userId > 0)
                query = query.Where(historyEntry => historyEntry.UserId == userId);

            //filter by store
            if (!_rewardPointsSettings.PointsAccumulatedForAllStores && storeId > 0)
                query = query.Where(historyEntry => historyEntry.StoreId == storeId);

            //whether to show only the points that already activated
            if (!showNotActivated) 
                query = query.Where(historyEntry => historyEntry.CreatedOnUtc < DateTime.UtcNow);

            //update points balance
            await UpdateRewardPointsBalanceAsync(query);

            return query;
        }

        /// <summary>
        /// Update reward points balance if necessary
        /// </summary>
        /// <param name="query">Input query</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task UpdateRewardPointsBalanceAsync(IQueryable<RewardPointsHistory> query)
        {
            //get expired points
            var nowUtc = DateTime.UtcNow;
            var expiredPoints = query
                .Where(historyEntry => historyEntry.EndDateUtc < nowUtc && historyEntry.ValidPoints > 0)
                .OrderBy(historyEntry => historyEntry.CreatedOnUtc).ThenBy(historyEntry => historyEntry.Id).ToList();

            //reduce the balance for these points
            foreach (var historyEntry in expiredPoints)
            {
                await InsertRewardPointsHistoryEntryAsync(new RewardPointsHistory
                {
                    UserId = historyEntry.UserId,
                    StoreId = historyEntry.StoreId,
                    Points = -historyEntry.ValidPoints.Value,
                    Message = string.Format(await _localizationService.GetResourceAsync("RewardPoints.Expired"),
                        await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc)),
                    CreatedOnUtc = historyEntry.EndDateUtc.Value
                });

                historyEntry.ValidPoints = 0;
                await UpdateRewardPointsHistoryEntryAsync(historyEntry);
            }

            //get has not yet activated points, but it's time to do it
            var notActivatedPoints = query
                .Where(historyEntry => !historyEntry.PointsBalance.HasValue && historyEntry.CreatedOnUtc < nowUtc)
                .OrderBy(historyEntry => historyEntry.CreatedOnUtc).ThenBy(historyEntry => historyEntry.Id).ToList();
            if (!notActivatedPoints.Any())
                return;

            //get current points balance
            //LINQ to entities does not support Last method, thus order by desc and use First one
            var currentPointsBalance = query
                .OrderByDescending(historyEntry => historyEntry.CreatedOnUtc).ThenByDescending(historyEntry => historyEntry.Id)
                .FirstOrDefault(historyEntry => historyEntry.PointsBalance.HasValue)
                ?.PointsBalance ?? 0;

            //update appropriate records
            foreach (var historyEntry in notActivatedPoints)
            {
                currentPointsBalance += historyEntry.Points;
                historyEntry.PointsBalance = currentPointsBalance;
                await UpdateRewardPointsHistoryEntryAsync(historyEntry);
            }
        }

        /// <summary>
        /// Insert the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InsertRewardPointsHistoryEntryAsync(RewardPointsHistory rewardPointsHistory)
        {
            await _rewardPointsHistoryRepository.InsertAsync(rewardPointsHistory);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load reward point history records
        /// </summary>
        /// <param name="userId">User identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier; pass null to load all records</param>
        /// <param name="showNotActivated">A value indicating whether to show reward points that did not yet activated</param>
        /// <param name="orderGuid">Order Guid; pass null to load all record</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the reward point history records
        /// </returns>
        public virtual async Task<IPagedList<RewardPointsHistory>> GetRewardPointsHistoryAsync(int userId = 0, int? storeId = null,
            bool showNotActivated = false, Guid? orderGuid = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = await GetRewardPointsQueryAsync(userId, storeId, showNotActivated);

            if (orderGuid.HasValue)
                query = query.Where(historyEntry => historyEntry.UsedWithOrder == orderGuid.Value);

            query = query.OrderByDescending(historyEntry => historyEntry.CreatedOnUtc)
                .ThenByDescending(historyEntry => historyEntry.Id);

            //return paged reward points history
            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets reward points balance
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the balance
        /// </returns>
        public virtual async Task<int> GetRewardPointsBalanceAsync(int userId, int storeId)
        {
            var query = (await GetRewardPointsQueryAsync(userId, storeId))
                .OrderByDescending(historyEntry => historyEntry.CreatedOnUtc).ThenByDescending(historyEntry => historyEntry.Id);

            //return point balance of the first actual history entry
            return (await query.FirstOrDefaultAsync())?.PointsBalance ?? 0;
        }

        /// <summary>
        /// Add reward points history record
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="points">Number of points to add</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="message">Message</param>
        /// <param name="usedWithOrder">The order for which points were redeemed (spent) as a payment</param>
        /// <param name="usedAmount">Used amount</param>
        /// <param name="activatingDate">Date and time of activating reward points; pass null to immediately activating</param>
        /// <param name="endDate">Date and time when the reward points will no longer be valid; pass null to add date termless points</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the reward points history entry identifier
        /// </returns>
        public virtual async Task<int> AddRewardPointsHistoryEntryAsync(User user, int points, int storeId, string message = "",
            Order usedWithOrder = null, decimal usedAmount = 0M, DateTime? activatingDate = null, DateTime? endDate = null)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (storeId == 0)
                throw new ArgumentException("Store ID should be valid");

            if (points < 0 && endDate.HasValue)
                throw new ArgumentException("End date is available only for positive points amount");

            //insert new history entry
            var newHistoryEntry = new RewardPointsHistory
            {
                UserId = user.Id,
                StoreId = storeId,
                Points = points,
                PointsBalance = activatingDate.HasValue ? null : (int?)(await GetRewardPointsBalanceAsync(user.Id, storeId) + points),
                UsedAmount = usedAmount,
                Message = message,
                CreatedOnUtc = activatingDate ?? DateTime.UtcNow,
                EndDateUtc = endDate,
                ValidPoints = points > 0 ? (int?)points : null,
                UsedWithOrder = usedWithOrder?.OrderGuid
            };
            await InsertRewardPointsHistoryEntryAsync(newHistoryEntry);

            //reduce valid points of previous entries
            if (points >= 0) 
                return newHistoryEntry.Id;

            var withValidPoints = (await GetRewardPointsQueryAsync(user.Id, storeId))
                .Where(historyEntry => historyEntry.ValidPoints > 0)
                .OrderBy(historyEntry => historyEntry.CreatedOnUtc).ThenBy(historyEntry => historyEntry.Id).ToList();
            foreach (var historyEntry in withValidPoints)
            {
                points += historyEntry.ValidPoints.Value;
                historyEntry.ValidPoints = Math.Max(points, 0);
                await UpdateRewardPointsHistoryEntryAsync(historyEntry);

                if (points >= 0)
                    break;
            }

            return newHistoryEntry.Id;
        }

        /// <summary>
        /// Gets a reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistoryId">Reward point history entry identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the reward point history entry
        /// </returns>
        public virtual async Task<RewardPointsHistory> GetRewardPointsHistoryEntryByIdAsync(int rewardPointsHistoryId)
        {
            return await _rewardPointsHistoryRepository.GetByIdAsync(rewardPointsHistoryId);
        }

        /// <summary>
        /// Update the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateRewardPointsHistoryEntryAsync(RewardPointsHistory rewardPointsHistory)
        {
            await _rewardPointsHistoryRepository.UpdateAsync(rewardPointsHistory);
        }

        /// <summary>
        /// Delete the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteRewardPointsHistoryEntryAsync(RewardPointsHistory rewardPointsHistory)
        {
            await _rewardPointsHistoryRepository.DeleteAsync(rewardPointsHistory);
        }

        #endregion
    }
}