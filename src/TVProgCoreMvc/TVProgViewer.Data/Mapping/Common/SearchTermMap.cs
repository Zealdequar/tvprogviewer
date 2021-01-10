using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Common;

namespace TVProgViewer.Data.Mapping.Common
{
    /// <summary>
    /// Represents a search term mapping configuration
    /// </summary>
    public partial class SearchTermMap : TvProgEntityTypeConfiguration<SearchTerm>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<SearchTerm> builder)
        {
            builder.HasTableName(nameof(SearchTerm));

            builder.Property(searchTerm => searchTerm.Keyword);
            builder.Property(searchTerm => searchTerm.StoreId);
            builder.Property(searchTerm => searchTerm.Count);
        }

        #endregion
    }
}