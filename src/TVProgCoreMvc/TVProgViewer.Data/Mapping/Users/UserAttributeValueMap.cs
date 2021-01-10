using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Data.Mapping.Users
{
    /// <summary>
    /// Represents a User attribute value mapping configuration
    /// </summary>
    public partial class UserAttributeValueMap : TvProgEntityTypeConfiguration<UserAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<UserAttributeValue> builder)
        {
            builder.HasTableName(nameof(UserAttributeValue));

            builder.Property(value => value.Name).HasLength(400).IsNullable(false);
            builder.Property(Userattributevalue => Userattributevalue.UserAttributeId);
            builder.Property(Userattributevalue => Userattributevalue.IsPreSelected);
            builder.Property(Userattributevalue => Userattributevalue.DisplayOrder);
        }

        #endregion
    }
}