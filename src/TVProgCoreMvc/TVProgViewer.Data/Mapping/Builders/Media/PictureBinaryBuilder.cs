using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Media;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Media
{
    /// <summary>
    /// Represents a picture binary entity builder
    /// </summary>
    public partial class PictureBinaryBuilder : TvProgEntityBuilder<PictureBinary>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(PictureBinary.PictureId)).AsInt32().ForeignKey<Picture>();
        }

        #endregion
    }
}