using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product product attribute mapping configuration
    /// </summary>
    public partial class ProductAttributeMappingMap : TvProgEntityTypeConfiguration<ProductAttributeMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductAttributeMapping> builder)
        {
            builder.HasTableName(TvProgMappingDefaults.ProductProductAttributeTable);

            builder.Property(productattributemapping => productattributemapping.ProductId);
            builder.Property(productattributemapping => productattributemapping.ProductAttributeId);
            builder.Property(productattributemapping => productattributemapping.TextPrompt);
            builder.Property(productattributemapping => productattributemapping.IsRequired);
            builder.Property(productattributemapping => productattributemapping.AttributeControlTypeId);
            builder.Property(productattributemapping => productattributemapping.DisplayOrder);
            builder.Property(productattributemapping => productattributemapping.ValidationMinLength);
            builder.Property(productattributemapping => productattributemapping.ValidationMaxLength);
            builder.Property(productattributemapping => productattributemapping.ValidationFileAllowedExtensions);
            builder.Property(productattributemapping => productattributemapping.ValidationFileMaximumSize);
            builder.Property(productattributemapping => productattributemapping.DefaultValue);
            builder.Property(productattributemapping => productattributemapping.ConditionAttributeXml);

            builder.Ignore(pam => pam.AttributeControlType);
        }

        #endregion
    }
}