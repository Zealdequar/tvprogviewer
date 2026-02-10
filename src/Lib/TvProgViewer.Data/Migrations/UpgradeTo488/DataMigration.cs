using FluentMigrator;
using System;
using System.Linq;
using TvProgViewer.Core.Domain.Users;
namespace TvProgViewer.Data.Migrations.UpgradeTo488
{
    [TvProgMigration("2026-01-12 20:25:00", "4.88.0 Data Update", UpdateMigrationType.Data, MigrationProcessType.Update)]
    public class DataMigration: Migration
    {
        private readonly ITvProgDataProvider _dataProvider;
        public DataMigration(ITvProgDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Собрать выражения для наката
        /// </summary>
        public override void Up()
        {
            // Добавить новую роль:
            if (!_dataProvider.GetTable<UserRole>().Any(ur => string.Compare(ur.SystemName, TvProgUserDefaults.TvGreenDataRoleName, true) == 0))
            {
                var tvGreenDataRole = _dataProvider.InsertEntity(
                    new UserRole()
                    {
                        Name = "TvGreenData",
                        SystemName = "TvGreenData",
                        FreeShipping = false,
                        TaxExempt = false,
                        Active = true,
                        IsSystemRole = true,
                        EnablePasswordLifetime = false,
                        OverrideTaxDisplayType = false,
                        DefaultTaxDisplayTypeId = 0,
                        PurchasedWithTvChannelId = 0
                    });
            }
            
            // Добавить новые операции:
            if (!_dataProvider.GetTable<UserGreenDataOperations>().Any(ugdo => string.Compare(ugdo.Name, TvProgUserDefaults.GetProgrammeStatusOperationName, true) == 0))
            {
                var userGreenDataGetProgrammeStatusOperation = _dataProvider.InsertEntity(
                    new UserGreenDataOperations()
                    {
                        Name = "GetProgrammeStatus",
                        Active = true
                    });
            }
            if (!_dataProvider.GetTable<UserGreenDataOperations>().Any(ugdo => string.Compare(ugdo.Name, TvProgUserDefaults.GetAllChannelsOperationName, true) == 0))
            {
                var userGreenDataGetAllChannelsOperation = _dataProvider.InsertEntity(
                    new UserGreenDataOperations()
                    {
                        Name = "GetAllChannels",
                        Active = true
                    });
            }
            if (!_dataProvider.GetTable<UserGreenDataOperations>().Any(ugdo => string.Compare(ugdo.Name, TvProgUserDefaults.GetAllProgrammesOperationName, true) == 0))
            {
                var userGreenDataGetAllProgrammesOperation = _dataProvider.InsertEntity(
                    new UserGreenDataOperations()
                    {
                        Name = "GetAllProgrammes",
                        Active = true
                    });
            }
        }

        /// <summary>
        /// Собрать выражения для отката
        /// </summary>
        public override void Down()
        {
            // Добавить, если необходимо
        }
    }
}
