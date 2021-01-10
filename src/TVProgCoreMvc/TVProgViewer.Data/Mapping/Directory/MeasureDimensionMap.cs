using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Directory
{
    /// <summary>
    /// Represents a measure dimension mapping configuration
    /// </summary>
    public partial class MeasureDimensionMap : TvProgEntityTypeConfiguration<MeasureDimension>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<MeasureDimension> builder)
        {
            builder.HasTableName(nameof(MeasureDimension));

            builder.Property(dimension => dimension.Name).HasLength(100).IsNullable(false);
            builder.Property(dimension => dimension.SystemKeyword).HasLength(100).IsNullable(false);
            builder.Property(dimension => dimension.Ratio).HasDecimal(18, 8);
            builder.Property(measuredimension => measuredimension.DisplayOrder);
        }

        #endregion
    }
}