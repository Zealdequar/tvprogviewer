using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Security;

namespace TVProgViewer.Data.Mapping.Security
{
    /// <summary>
    /// Represents a permission record-User role mapping configuration
    /// </summary>
    public partial class PermissionRecordUserRoleMap : TvProgEntityTypeConfiguration<PermissionRecordUserRoleMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<PermissionRecordUserRoleMapping> builder)
        {
            builder.HasTableName(TvProgMappingDefaults.PermissionRecordRoleTable);
            builder.HasPrimaryKey(mapping => new
            {
                mapping.PermissionRecordId,
                mapping.UserRoleId
            });

            builder.Property(mapping => mapping.PermissionRecordId).HasColumnName("PermissionRecord_Id");
            builder.Property(mapping => mapping.UserRoleId).HasColumnName("UserRole_Id");

            builder.Ignore(mapping => mapping.Id);
        }

        #endregion
    }
}