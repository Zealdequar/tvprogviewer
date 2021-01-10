using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Orders;

namespace TVProgViewer.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a return request action mapping configuration
    /// </summary>
    public partial class ReturnRequestActionMap : TvProgEntityTypeConfiguration<ReturnRequestAction>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ReturnRequestAction> builder)
        {
            builder.HasTableName(nameof(ReturnRequestAction));

            builder.Property(action => action.Name).HasLength(400).IsNullable(false);
            builder.Property(action => action.DisplayOrder);
        }

        #endregion
    }
}