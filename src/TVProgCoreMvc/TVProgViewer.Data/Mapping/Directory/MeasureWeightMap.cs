using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Directory
{
    /// <summary>
    /// Represents a measure weight mapping configuration
    /// </summary>
    public partial class MeasureWeightMap : TvProgEntityTypeConfiguration<MeasureWeight>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<MeasureWeight> builder)
        {
            builder.HasTableName(nameof(MeasureWeight));

            builder.Property(weight => weight.Name).HasLength(100).IsNullable(false);
            builder.Property(weight => weight.SystemKeyword).HasLength(100).IsNullable(false);
            builder.Property(weight => weight.Ratio).HasDecimal(18, 8);
            builder.Property(measureweight => measureweight.DisplayOrder);
        }

        #endregion
    }
}