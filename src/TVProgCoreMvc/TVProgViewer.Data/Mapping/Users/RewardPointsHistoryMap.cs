using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Users
{
    /// <summary>
    /// Represents a reward points history mapping configuration
    /// </summary>
    public partial class RewardPointsHistoryMap : TvProgEntityTypeConfiguration<RewardPointsHistory>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<RewardPointsHistory> builder)
        {
            builder.HasTableName(nameof(RewardPointsHistory));

            builder.Property(historyEntry => historyEntry.UsedAmount).HasDecimal();

            builder.Property(historyEntry => historyEntry.UserId);
            builder.Property(historyEntry => historyEntry.StoreId);
            builder.Property(historyEntry => historyEntry.Points);
            builder.Property(historyEntry => historyEntry.PointsBalance);
            builder.Property(historyEntry => historyEntry.Message);
            builder.Property(historyEntry => historyEntry.CreatedOnUtc);
            builder.Property(historyEntry => historyEntry.EndDateUtc);
            builder.Property(historyEntry => historyEntry.ValidPoints);
            builder.Property(historyEntry => historyEntry.OrderId);
        }

        #endregion
    }
}