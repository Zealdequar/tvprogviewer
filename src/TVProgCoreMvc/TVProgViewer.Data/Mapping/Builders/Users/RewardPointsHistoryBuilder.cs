using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Users
{
    /// <summary>
    /// Represents a reward points history entity builder
    /// </summary>
    public partial class RewardPointsHistoryBuilder : TvProgEntityBuilder<RewardPointsHistory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(RewardPointsHistory.UserId)).AsInt32().ForeignKey<User>();
        }

        #endregion
    }
}