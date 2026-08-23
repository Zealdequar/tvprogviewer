using FluentMigrator;
using TvProgViewer.Core.Domain.TvProgMain;
using TvProgViewer.Data.Extensions;
using TvProgViewer.Data.Mapping;

namespace TvProgViewer.Data.Migrations.UpgradeTo490
{
    [TvProgMigration("2026-08-17 00:00:00", "4.90.0 Fix ProgrammesTvProgCategoryMapping schema", MigrationProcessType.Update)]
    public class SchemaMigration : Migration
    {
        /// <summary>
        /// Собрать то, что нужно накатить
        /// </summary>
        public override void Up()
        {
            var tableName = NameCompatibilityManager.GetTableName(typeof(ProgrammesTvProgCategoryMapping));
            var idColumnName = nameof(ProgrammesTvProgCategoryMapping.Id);
            var tvProgCategoryColumn = NameCompatibilityManager.GetColumnName(typeof(ProgrammesTvProgCategoryMapping), nameof(ProgrammesTvProgCategoryMapping.TvProgCategoryId));
            var programmesColumn = NameCompatibilityManager.GetColumnName(typeof(ProgrammesTvProgCategoryMapping), nameof(ProgrammesTvProgCategoryMapping.ProgrammesId));
            var uniqueIndexName = "UQ_ProgrammesTvProgCategoryMapping_TvProgCategoryId_ProgrammesId";

            // Таблица могла быть создана со старой составной первичной ключом (без Identity-колонки Id).
            // Добавляем Identity-колонку на месте, не пересоздавая таблицу, чтобы не конфликтовать с уже
            // существующими индексами внешних ключей (IX_<таблица>_<колонка>).
            if (!Schema.Table(tableName).Column(idColumnName).Exists())
            {
                Alter.Table(tableName).AddColumn(idColumnName).AsInt32().Identity();
            }

            // Уникальный индекс по паре (категория, передача) — защита от дубликатов сопоставлений.
            // Создаём независимо и идемпотентно (проверка на наличие), чтобы корректно отрабатывать
            // при повторных запусках миграции.
            if (!Schema.Table(tableName).Index(uniqueIndexName).Exists())
            {
                Create.Index(uniqueIndexName)
                    .OnTable(tableName)
                    .OnColumn(tvProgCategoryColumn).Ascending()
                    .OnColumn(programmesColumn).Ascending()
                    .WithOptions().Unique();
            }
        }

        /// <summary>
        /// Собрать то, что нужно откатить
        /// </summary>
        public override void Down()
        {
            // Ничего откатывать не надо
        }
    }
}
