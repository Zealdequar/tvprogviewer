using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a tvChannel video mapping entity builder
    /// </summary>
    public partial class TvChannelVideoBuilder : TvProgEntityBuilder<TvChannelVideo>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TvChannelVideo.VideoId)).AsInt32().ForeignKey<Video>()
                .WithColumn(nameof(TvChannelVideo.TvChannelId)).AsInt32().ForeignKey<TvChannel>();
        }

        #endregion
    }
}
