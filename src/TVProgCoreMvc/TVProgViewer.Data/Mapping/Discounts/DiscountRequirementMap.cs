using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Discounts;

namespace TVProgViewer.Data.Mapping.Discounts
{
    /// <summary>
    /// Represents a discount requirement mapping configuration
    /// </summary>
    public partial class DiscountRequirementMap : TvProgEntityTypeConfiguration<DiscountRequirement>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<DiscountRequirement> builder)
        {
            builder.HasTableName(nameof(DiscountRequirement));

            builder.Property(requirement => requirement.DiscountId);
            builder.Property(requirement => requirement.DiscountRequirementRuleSystemName);
            builder.Property(requirement => requirement.ParentId);
            builder.Property(requirement => requirement.InteractionTypeId);
            builder.Property(requirement => requirement.IsGroup);

            builder.Ignore(requirement => requirement.InteractionType);
        }

        #endregion
    }
}