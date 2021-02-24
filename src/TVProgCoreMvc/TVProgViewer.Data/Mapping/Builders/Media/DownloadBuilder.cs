using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Media;

namespace TVProgViewer.Data.Mapping.Builders.Media
{
    /// <summary>
    /// Represents a download entity builder
    /// </summary>
    public partial class DownloadBuilder : TvProgEntityBuilder<Download>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
        }

        #endregion
    }
}