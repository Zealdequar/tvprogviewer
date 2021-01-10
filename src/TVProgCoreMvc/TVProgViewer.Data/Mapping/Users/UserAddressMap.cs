using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Data.Mapping.Users
{
    /// <summary>
    /// Represents a User-address mapping configuration
    /// </summary>
    public partial class UserAddressMap : TvProgEntityTypeConfiguration<UserAddressMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<UserAddressMapping> builder)
        {
            builder.HasTableName(TvProgMappingDefaults.UserAddressesTable);
            builder.HasPrimaryKey(mapping => new { mapping.UserId, mapping.AddressId });

            builder.Property(mapping => mapping.UserId).HasColumnName("User_Id");
            builder.Property(mapping => mapping.AddressId).HasColumnName("Address_Id");

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}