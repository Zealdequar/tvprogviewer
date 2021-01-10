using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Seo;

namespace TVProgViewer.Data.Mapping.Seo
{
    /// <summary>
    /// Represents an URL record mapping configuration
    /// </summary>
    public partial class UrlRecordMap : TvProgEntityTypeConfiguration<UrlRecord>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<UrlRecord> builder)
        {
            builder.HasTableName(nameof(UrlRecord));

            builder.Property(record => record.EntityName).HasLength(400).IsNullable(false);
            builder.Property(record => record.Slug).HasLength(400).IsNullable(false);
            builder.Property(urlrecord => urlrecord.EntityId);
            builder.Property(urlrecord => urlrecord.IsActive);
            builder.Property(urlrecord => urlrecord.LanguageId);
        }

        #endregion
    }
}