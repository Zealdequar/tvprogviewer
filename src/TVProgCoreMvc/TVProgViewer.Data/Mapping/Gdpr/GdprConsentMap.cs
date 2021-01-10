using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Gdpr;

namespace TVProgViewer.Data.Mapping.Gdpr
{
    /// <summary>
    /// Represents a GDPR consent mapping configuration
    /// </summary>
    public partial class GdprConsentMap : TvProgEntityTypeConfiguration<GdprConsent>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<GdprConsent> builder)
        {
            builder.HasTableName(nameof(GdprConsent));

            builder.Property(gdpr => gdpr.Message).IsNullable(false);

            builder.Property(gdpr => gdpr.IsRequired);
            builder.Property(gdpr => gdpr.RequiredMessage);
            builder.Property(gdpr => gdpr.DisplayDuringRegistration);
            builder.Property(gdpr => gdpr.DisplayOnUserInfoPage);
            builder.Property(gdpr => gdpr.DisplayOrder);
        }

        #endregion
    }
}