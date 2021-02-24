using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Configuration;

namespace TVProgViewer.Data.Mapping.Builders.Configuration
{
    /// <summary>
    /// Represents a setting entity builder
    /// </summary>
    public partial class SettingBuilder : TvProgEntityBuilder<Setting>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
               .WithColumn(nameof(Setting.Name)).AsString(200).NotNullable()
               .WithColumn(nameof(Setting.Value)).AsString(6000).NotNullable();
        }

        #endregion
    }
}