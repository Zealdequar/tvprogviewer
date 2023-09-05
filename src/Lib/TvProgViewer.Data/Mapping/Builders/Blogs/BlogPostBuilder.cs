using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Blogs
{
    /// <summary>
    /// Represents a blog post entity builder
    /// </summary>
    public partial class BlogPostBuilder : TvProgEntityBuilder<BlogPost>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(BlogPost.Title)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(BlogPost.Body)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(BlogPost.MetaKeywords)).AsString(400).Nullable()
                .WithColumn(nameof(BlogPost.MetaTitle)).AsString(400).Nullable()
                .WithColumn(nameof(BlogPost.LanguageId)).AsInt32().ForeignKey<Language>();
        }

        #endregion
    }
}