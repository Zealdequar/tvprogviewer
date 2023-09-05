using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Gdpr;

namespace TvProgViewer.Data.Mapping.Builders.Gdpr
{
    /// <summary>
    /// Represents a GDPR consent entity builder
    /// </summary>
    public partial class GdprConsentBuilder : TvProgEntityBuilder<GdprConsent>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(GdprConsent.Message)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}