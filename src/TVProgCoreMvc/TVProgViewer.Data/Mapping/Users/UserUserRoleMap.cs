using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Data.Mapping.Users
{
    /// <summary>
    /// Represents a User-User role mapping configuration
    /// </summary>
    public partial class UserUserRoleMap : TvProgEntityTypeConfiguration<UserUserRoleMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<UserUserRoleMapping> builder)
        {
            builder.HasTableName(TvProgMappingDefaults.UserUserRoleTable);
            builder.HasPrimaryKey(mapping => new { mapping.UserId, mapping.UserRoleId });

            builder.Property(mapping => mapping.UserId).HasColumnName("User_Id");
            builder.Property(mapping => mapping.UserRoleId).HasColumnName("UserRole_Id");

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}