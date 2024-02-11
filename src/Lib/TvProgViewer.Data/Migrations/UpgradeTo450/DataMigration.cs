using System;
using System.Linq;
using FluentMigrator;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.ScheduleTasks;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Data.Mapping;
using TvProgViewer.Core.Domain.Orders;

namespace TvProgViewer.Data.Migrations.UpgradeTo450
{
    [TvProgMigration("2021-04-23 00:00:00", "4.50.0", UpdateMigrationType.Data, MigrationProcessType.Update)]
    public class DataMigration : Migration
    {
        private readonly ITvProgDataProvider _dataProvider;

        public DataMigration(ITvProgDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            // add column
            var shipmentTableName = nameof(Shipment);
            var collectedDateUtcColumnName = "ReadyForPickupDateUtc";

            if (!Schema.Table(shipmentTableName).Column(collectedDateUtcColumnName).Exists())
            {
                Alter.Table(shipmentTableName)
                    .AddColumn(collectedDateUtcColumnName).AsDateTime2().Nullable();
            }

            // add message template
            if (!_dataProvider.GetTable<MessageTemplate>().Any(pr => string.Compare(pr.Name, MessageTemplateSystemNames.ShipmentReadyForPickupUserNotification, true) == 0))
            {
                var messageTemplate = _dataProvider.InsertEntity(
                    new MessageTemplate
                    {
                        Name = MessageTemplateSystemNames.ShipmentReadyForPickupUserNotification,
                        Subject = "Your order from %Store.Name% has been %if (!%Order.IsCompletelyReadyForPickup%) partially endif%ready for pickup.",
                        Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\"> %Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.UserFullName%!,{Environment.NewLine}<br />{Environment.NewLine}Good news! You order has been%if (!%Order.IsCompletelyReadyForPickup%) partially endif%ready for pickup.{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a href=\"%Order.OrderURLForUser%\" target=\"_blank\">%Order.OrderURLForUser%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% TvChannels ready for pickup:{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Shipment.TvChannel(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                        IsActive = true,
                        EmailAccountId = _dataProvider.GetTable<EmailAccount>().FirstOrDefault()?.Id ?? 0
                    }
                );
            }
            //#5547
            var scheduleTaskTableName = NameCompatibilityManager.GetTableName(typeof(ScheduleTask));

            //add column
            if (!Schema.Table(scheduleTaskTableName).Column(nameof(ScheduleTask.LastEnabledUtc)).Exists())
            {
                Alter.Table(scheduleTaskTableName)
                    .AddColumn(nameof(ScheduleTask.LastEnabledUtc)).AsDateTime2().Nullable();
            }
            else
            {
                Alter.Table(scheduleTaskTableName).AlterColumn(nameof(ScheduleTask.LastEnabledUtc)).AsDateTime2().Nullable();
            }

            //#5939
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "SalesSummaryReport", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var salesSummaryReportPermission = _dataProvider.InsertEntity(
                    new PermissionRecord
                    {
                        Name = "Admin area. Access sales summary report",
                        SystemName = "SalesSummaryReport",
                        Category = "Orders"
                    }
                );

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<UserRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == TvProgUserDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordUserRoleMapping
                    {
                        UserRoleId = adminRole.Id,
                        PermissionRecordId = salesSummaryReportPermission.Id
                    }
                );
            }

            //add column
            var returnRequestTableName = NameCompatibilityManager.GetTableName(typeof(ReturnRequest));
            var returnedQuantityColumnName = "ReturnedQuantity";

            if (!Schema.Table(returnRequestTableName).Column(returnedQuantityColumnName).Exists())
            {
                Alter.Table(returnRequestTableName)
                    .AddColumn(returnedQuantityColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
            }

            //#6053
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageAppSettings", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(
                    new PermissionRecord
                    {
                        Name = "Admin area. Manage App Settings",
                        SystemName = "ManageAppSettings",
                        Category = "Configuration"
                    }
                );

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<UserRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == TvProgUserDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordUserRoleMapping
                    {
                        UserRoleId = adminRole.Id,
                        PermissionRecordId = manageConnectionStringPermission.Id
                    }
                );
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
