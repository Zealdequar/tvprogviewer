using FluentMigrator;
using System;
using System.Linq;
using TvProgViewer.Core.Domain.ScheduleTasks;
using TvProgViewer.Core.Domain.Users;

namespace TvProgViewer.Data.Migrations.UpgradeTo472
{
    [TvProgMigration("2024-01-14 14:38:00", "4.72.0 Data Update", UpdateMigrationType.Data, MigrationProcessType.Update)]
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
            if (!_dataProvider.GetTable<UserRole>().Any(ur => string.Compare(ur.SystemName, TvProgUserDefaults.TvGuestsRoleName, true) == 0))
            {
                var tvGuestRole = _dataProvider.InsertEntity(
                    new UserRole()
                    {
                        Name = "TvGuests",
                        SystemName = "TvGuests",
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

            // Добавить новое задание
            if (!_dataProvider.GetTable<ScheduleTask>().Any(st => string.Compare(st.Name, "Перерасчёт пользовательского рейтинга телеканалов", true) == 0))
            {
                var channelRatingTask = _dataProvider.InsertEntity(
                    new ScheduleTask()
                    {
                        Name = "Перерасчёт пользовательского рейтинга телеканалов",
                        Type = "TvProgViewer.Services.TvProgMain.ChannelRatingTask, TvProgViewer.Services",
                        Seconds = 86400,
                        LastEnabledUtc = DateTime.UtcNow,
                        Enabled = true,
                        StopOnError = false,
                        LastStartUtc = null,
                        LastEndUtc = null,
                        LastSuccessUtc= null
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
