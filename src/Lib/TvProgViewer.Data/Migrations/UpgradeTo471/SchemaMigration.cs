using FluentMigrator;
using TvProgViewer.Core.Domain.Common;

namespace TvProgViewer.Data.Migrations.UpgradeTo471
{
    [TvProgMigration("2023-11-19 00:00:00", "SchemaMigration for 4.71.0", MigrationProcessType.Update)]
    public class SchemaMigration: Migration
    {
        /// <summary>
        /// Собирает все Up выражения миграции
        /// </summary>
        public override void Up()
        {
            // Добавление колонок
            var addressTableName = nameof(Address);

            var middleNameUserColumnName = nameof(Address.MiddleName);

            if (!Schema.Table(addressTableName).Column(middleNameUserColumnName).Exists())
            {
                Alter.Table(addressTableName)
                    .AddColumn(middleNameUserColumnName).AsString(1000).Nullable();
            }
        }

        public override void Down()
        {
            // добавление логики отката, если необходима 
        }
    }
}
