using FluentMigrator;
using TvProgViewer.Data;
using TvProgViewer.Data.Mapping;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Plugin.Tax.Avalara.Domain;

namespace TvProgViewer.Plugin.Tax.Avalara.Data
{
    [TvProgMigration("2022-07-13 00:00:02", "Tax.Avalara 2.65. Update datetime type precision", MigrationProcessType.Update)]
    public class MySqlDateTimeWithPrecisionMigration : Migration
    {

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            var dataSettings = DataSettingsManager.LoadSettings();

            //update the types only in MySql 
            if (dataSettings.DataProvider != DataProviderType.MySql)
                return;

            Alter.Table(NameCompatibilityManager.GetTableName(typeof(TaxTransactionLog)))
                 .AlterColumn(NameCompatibilityManager.GetColumnName(typeof(TaxTransactionLog), nameof(TaxTransactionLog.CreatedDateUtc)))
                 .AsCustom("datetime(6)");
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            //nothing
        }
    }
}