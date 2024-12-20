﻿using System;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Orders;

namespace TvProgViewer.Services.Orders
{
    /// <summary>
    /// Reward point service interface
    /// </summary>
    public partial interface IRewardPointService
    {
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
        Task<IPagedList<RewardPointsHistory>> GetRewardPointsHistoryAsync(int userId = 0, int? storeId = null,
            bool showNotActivated = false, Guid? orderGuid = null, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets reward points balance
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the balance
        /// </returns>
        Task<int> GetRewardPointsBalanceAsync(int userId, int storeId);

        /// <summary>
        /// Add reward points history record
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="points">Number of points to add</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="message">Message</param>
        /// <param name="usedWithOrder">the order for which points were redeemed as a payment</param>
        /// <param name="usedAmount">Used amount</param>
        /// <param name="activatingDate">Date and time of activating reward points; pass null to immediately activating</param>
        /// <param name="endDate">Date and time when the reward points will no longer be valid; pass null to add date termless points</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the reward points history entry identifier
        /// </returns>
        Task<int> AddRewardPointsHistoryEntryAsync(User user, int points, int storeId, string message = "",
            Order usedWithOrder = null, decimal usedAmount = 0M, DateTime? activatingDate = null, DateTime? endDate = null);

        /// <summary>
        /// Gets a reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistoryId">Reward point history entry identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the reward point history entry
        /// </returns>
        Task<RewardPointsHistory> GetRewardPointsHistoryEntryByIdAsync(int rewardPointsHistoryId);

        /// <summary>
        /// Updates the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateRewardPointsHistoryEntryAsync(RewardPointsHistory rewardPointsHistory);

        /// <summary>
        /// Delete the reward point history entry
        /// </summary>
        /// <param name="rewardPointsHistory">Reward point history entry</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteRewardPointsHistoryEntryAsync(RewardPointsHistory rewardPointsHistory);
    }
}