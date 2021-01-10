using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Common;

namespace TVProgViewer.Data.Mapping.Common
{
    /// <summary>
    /// Represents an address attribute mapping configuration
    /// </summary>
    public partial class AddressAttributeMap : TvProgEntityTypeConfiguration<AddressAttribute>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<AddressAttribute> builder)
        {
            builder.HasTableName(nameof(AddressAttribute));

            builder.Property(attribute => attribute.Name).HasLength(400).IsNullable(false);
            builder.Property(addressattribute => addressattribute.IsRequired);
            builder.Property(addressattribute => addressattribute.AttributeControlTypeId);
            builder.Property(addressattribute => addressattribute.DisplayOrder);

            builder.Ignore(attribute => attribute.AttributeControlType);
        }

        #endregion
    }
}