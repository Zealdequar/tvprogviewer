using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.News;

namespace TVProgViewer.Data.Mapping.News
{
    /// <summary>
    /// Represents a news mapping configuration
    /// </summary>
    public partial class NewsItemMap : TvProgEntityTypeConfiguration<NewsItem>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<NewsItem> builder)
        {
            builder.HasTableName(TvProgMappingDefaults.NewsItemTable);

            builder.Property(newsItem => newsItem.Title).IsNullable(false);
            builder.Property(newsItem => newsItem.Short).IsNullable(false);
            builder.Property(newsItem => newsItem.Full).IsNullable(false);
            builder.Property(newsItem => newsItem.MetaKeywords).HasLength(400);
            builder.Property(newsItem => newsItem.MetaTitle).HasLength(400);
            builder.Property(newsitem => newsitem.LanguageId);
            builder.Property(newsitem => newsitem.Published);
            builder.Property(newsitem => newsitem.StartDateUtc);
            builder.Property(newsitem => newsitem.EndDateUtc);
            builder.Property(newsitem => newsitem.AllowComments);
            builder.Property(newsitem => newsitem.LimitedToStores);
            builder.Property(newsitem => newsitem.MetaDescription);
            builder.Property(newsitem => newsitem.CreatedOnUtc);
        }

        #endregion
    }
}