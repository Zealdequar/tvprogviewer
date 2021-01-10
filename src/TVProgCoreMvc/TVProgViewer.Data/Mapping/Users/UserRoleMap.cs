using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Data.Mapping.Users
{
    /// <summary>
    /// Represents a User role mapping configuration
    /// </summary>
    public partial class UserRoleMap : TvProgEntityTypeConfiguration<UserRole>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<UserRole> builder)
        {
            builder.HasTableName(nameof(UserRole));

            builder.Property(role => role.Name).HasLength(255).IsNullable(false);
            builder.Property(role => role.SystemName).HasLength(255);
            builder.Property(role => role.FreeShipping);
            builder.Property(role => role.TaxExempt);
            builder.Property(role => role.Active);
            builder.Property(role => role.IsSystemRole);
            builder.Property(role => role.EnablePasswordLifetime);
            builder.Property(role => role.OverrideTaxDisplayType);
            builder.Property(role => role.DefaultTaxDisplayTypeId);
            builder.Property(role => role.PurchasedWithProductId);
        }

        #endregion
    }
}