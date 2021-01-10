using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Data.Mapping.Users
{
    /// <summary>
    /// Represents a User attribute mapping configuration
    /// </summary>
    public partial class UserAttributeMap : TvProgEntityTypeConfiguration<UserAttribute>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<UserAttribute> builder)
        {
            builder.HasTableName(nameof(UserAttribute));

            builder.Property(attribute => attribute.Name).HasLength(400).IsNullable(false);
            builder.Property(Userattribute => Userattribute.IsRequired);
            builder.Property(Userattribute => Userattribute.AttributeControlTypeId);
            builder.Property(Userattribute => Userattribute.DisplayOrder);

            builder.Ignore(attribute => attribute.AttributeControlType);
        }

        #endregion
    }
}