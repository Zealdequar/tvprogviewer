using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Security;

namespace TVProgViewer.Data.Mapping.Security
{
    /// <summary>
    /// Represents an ACL record mapping configuration
    /// </summary>
    public partial class AclRecordMap : TvProgEntityTypeConfiguration<AclRecord>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<AclRecord> builder)
        {
            builder.HasTableName(nameof(AclRecord));

            builder.Property(record => record.EntityName).HasLength(400).IsNullable(false);
            builder.Property(aclrecord => aclrecord.EntityId);
            builder.Property(aclrecord => aclrecord.UserRoleId);
        }

        #endregion
    }
}