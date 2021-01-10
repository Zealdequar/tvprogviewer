using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Gdpr;

namespace TVProgViewer.Data.Mapping.Gdpr
{
    /// <summary>
    /// Represents a GDPR log mapping configuration
    /// </summary>
    public partial class GdprLogMap : TvProgEntityTypeConfiguration<GdprLog>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<GdprLog> builder)
        {
            builder.HasTableName(nameof(GdprLog));

            builder.Property(gdprlog => gdprlog.UserId);
            builder.Property(gdprlog => gdprlog.ConsentId);
            builder.Property(gdprlog => gdprlog.UserInfo);
            builder.Property(gdprlog => gdprlog.RequestTypeId);
            builder.Property(gdprlog => gdprlog.RequestDetails);
            builder.Property(gdprlog => gdprlog.CreatedOnUtc);

            builder.Ignore(gdpr => gdpr.RequestType);
        }

        #endregion
    }
}