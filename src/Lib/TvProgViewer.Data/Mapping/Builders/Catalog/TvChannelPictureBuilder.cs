using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvChannel picture entity builder
    /// </summary>
    public partial class TvChannelPictureBuilder : TvProgEntityBuilder<TvChannelPicture>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TvChannelPicture.PictureId)).AsInt32().ForeignKey<Picture>()
                .WithColumn(nameof(TvChannelPicture.TvChannelId)).AsInt32().ForeignKey<TvChannel>();
        }

        #endregion
    }
}