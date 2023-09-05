using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentMigrator;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Configuration;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Logging;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.ScheduleTasks;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Shipping;

namespace TvProgViewer.Data.Migrations.UpgradeTo460
{
    [TvProgMigration("2022-07-20 00:00:01", "4.60.0", UpdateMigrationType.Data, MigrationProcessType.Update)]
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
            //#4601 user attribute values to user table column values
            var attributeKeys = new[] { nameof(User.FirstName), nameof(User.LastName), nameof(User.Gender),
                nameof(User.Company), nameof(User.StreetAddress), nameof(User.StreetAddress2), nameof(User.ZipPostalCode),
                nameof(User.City), nameof(User.County), nameof(User.SmartPhone), nameof(User.Fax), nameof(User.VatNumber),
                nameof(User.GmtZone), nameof(User.CustomUserAttributesXML), nameof(User.CountryId),
                nameof(User.StateProvinceId), nameof(User.VatNumberStatusId), nameof(User.CurrencyId), nameof(User.LanguageId),
                nameof(User.TaxDisplayTypeId), nameof(User.BirthDate)};

            var languages = _dataProvider.GetTable<Language>().ToList();
            var currencies = _dataProvider.GetTable<Currency>().ToList();
            var userRole = _dataProvider.GetTable<UserRole>().FirstOrDefault(cr => cr.SystemName == TvProgUserDefaults.RegisteredRoleName);
            var userRoleId = userRole?.Id ?? 0;

            var query =
                from c in _dataProvider.GetTable<User>()
                join crm in _dataProvider.GetTable<UserUserRoleMapping>() on c.Id equals crm.UserId
                where !c.Deleted && (userRoleId == 0 || crm.UserRoleId == userRoleId)
                select c;

            var pageIndex = 0;
            var pageSize = 500;

            int castToInt(string value)
            {
                return int.TryParse(value, out var result) ? result : default;
            }

            string castToString(string value)
            {
                return value;
            }

            DateTime? castToDateTime(string value)
            {
                return DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var dateOfBirth)
                    ? dateOfBirth
                    : default;
            }

            T getAttributeValue<T>(IList<GenericAttribute> attributes, string key, Func<string, T> castTo, int maxLength = 1000)
            {
                var str = CommonHelper.EnsureMaximumLength(attributes.FirstOrDefault(ga => ga.Key == key)?.Value, maxLength);

                return castTo(str);
            }

            while (true)
            {
                var users = query.ToPagedListAsync(pageIndex++, pageSize).Result;

                if (!users.Any())
                    break;

                var userIds = users.Select(c => c.Id).ToList();
                var genericAttributes = _dataProvider.GetTable<GenericAttribute>()
                    .Where(ga => ga.KeyGroup == nameof(User) && userIds.Contains(ga.EntityId) && attributeKeys.Contains(ga.Key)).ToList();

                if (!genericAttributes.Any())
                    continue;

                foreach (var user in users)
                {
                    var userAttributes = genericAttributes.Where(ga => ga.EntityId == user.Id).ToList();
                    if (!userAttributes.Any())
                        continue;

                    user.FirstName = getAttributeValue(userAttributes, nameof(User.FirstName), castToString);
                    user.LastName = getAttributeValue(userAttributes, nameof(User.LastName), castToString);
                    user.Gender = getAttributeValue(userAttributes, nameof(User.Gender), castToString);
                    user.Company = getAttributeValue(userAttributes, nameof(User.Company), castToString);
                    user.StreetAddress = getAttributeValue(userAttributes, nameof(User.StreetAddress), castToString);
                    user.StreetAddress2 = getAttributeValue(userAttributes, nameof(User.StreetAddress2), castToString);
                    user.ZipPostalCode = getAttributeValue(userAttributes, nameof(User.ZipPostalCode), castToString);
                    user.City = getAttributeValue(userAttributes, nameof(User.City), castToString);
                    user.County = getAttributeValue(userAttributes, nameof(User.County), castToString);
                    user.SmartPhone = getAttributeValue(userAttributes, nameof(User.SmartPhone), castToString);
                    user.Fax = getAttributeValue(userAttributes, nameof(User.Fax), castToString);
                    user.VatNumber = getAttributeValue(userAttributes, nameof(User.VatNumber), castToString);
                    user.GmtZone = getAttributeValue(userAttributes, nameof(User.GmtZone), castToString);
                    user.CustomUserAttributesXML = getAttributeValue<string>(userAttributes, nameof(User.CustomUserAttributesXML), castToString, int.MaxValue);
                    user.CountryId = getAttributeValue(userAttributes, nameof(User.CountryId), castToInt);
                    user.StateProvinceId = getAttributeValue(userAttributes, nameof(User.StateProvinceId), castToInt);
                    user.VatNumberStatusId = getAttributeValue(userAttributes, nameof(User.VatNumberStatusId), castToInt);
                    user.CurrencyId = currencies.FirstOrDefault(c => c.Id == getAttributeValue(userAttributes, nameof(User.CurrencyId), castToInt))?.Id;
                    user.LanguageId = languages.FirstOrDefault(l => l.Id == getAttributeValue(userAttributes, nameof(User.LanguageId), castToInt))?.Id;
                    user.TaxDisplayTypeId = getAttributeValue(userAttributes, nameof(User.TaxDisplayTypeId), castToInt);
                    user.BirthDate = getAttributeValue(userAttributes, nameof(User.BirthDate), castToDateTime);
                }

                _dataProvider.UpdateEntities(users);
                _dataProvider.BulkDeleteEntities(genericAttributes);
            }

            //#3777 new activity log types
            var activityLogTypeTable = _dataProvider.GetTable<ActivityLogType>();

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ImportNewsLetterSubscriptions", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ImportNewsLetterSubscriptions",
                        Enabled = true,
                        Name = "Newsletter subscriptions were imported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportUsers", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportUsers",
                        Enabled = true,
                        Name = "Users were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportCategories", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportCategories",
                        Enabled = true,
                        Name = "Categories were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportManufacturers", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportManufacturers",
                        Enabled = true,
                        Name = "Manufacturers were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportProducts", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportProducts",
                        Enabled = true,
                        Name = "Products were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportOrders", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportOrders",
                        Enabled = true,
                        Name = "Orders were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportStates", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportStates",
                        Enabled = true,
                        Name = "States were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportNewsLetterSubscriptions", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportNewsLetterSubscriptions",
                        Enabled = true,
                        Name = "Newsletter subscriptions were exported"
                    }
                );

            //#5809
            if (!_dataProvider.GetTable<ScheduleTask>().Any(st => string.Compare(st.Type, "TvProg.Services.Gdpr.DeleteInactiveUsersTask, TvProg.Services", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(
                    new ScheduleTask
                    {
                        Name = "Delete inactive users (GDPR)",
                        //24 hours
                        Seconds = 86400,
                        Type = "TvProg.Services.Gdpr.DeleteInactiveUsersTask, TvProg.Services",
                        Enabled = false,
                        StopOnError = false
                    }
                );
            }

            //#5607
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "EnableMultiFactorAuthentication", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var multifactorAuthenticationPermissionRecord = _dataProvider.InsertEntity(
                    new PermissionRecord
                    {
                        SystemName = "EnableMultiFactorAuthentication",
                        Name = "Security. Enable Multi-factor authentication",
                        Category = "Security"
                    }
                );

                var forceMultifactorAuthentication = _dataProvider.GetTable<Setting>()
                    .FirstOrDefault(s =>
                        string.Compare(s.Name, "MultiFactorAuthenticationSettings.ForceMultifactorAuthentication", StringComparison.InvariantCultureIgnoreCase) == 0 &&
                        string.Compare(s.Value, "True", StringComparison.InvariantCultureIgnoreCase) == 0)
                    is not null;

                var userRoles = _dataProvider.GetTable<UserRole>();
                if (!forceMultifactorAuthentication)
                    userRoles = userRoles.Where(cr => cr.SystemName == TvProgUserDefaults.AdministratorsRoleName || cr.SystemName == TvProgUserDefaults.RegisteredRoleName);

                foreach (var role in userRoles.ToList())
                {
                    _dataProvider.InsertEntity(
                        new PermissionRecordUserRoleMapping
                        {
                            UserRoleId = role.Id,
                            PermissionRecordId = multifactorAuthenticationPermissionRecord.Id
                        }
                    );
                }
            }

            var lastEnabledUtc = DateTime.UtcNow;
            if (!_dataProvider.GetTable<ScheduleTask>().Any(st => string.Compare(st.Type, "TvProg.Services.Common.ResetLicenseCheckTask, TvProg.Services", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                _dataProvider.InsertEntity(new ScheduleTask
                {
                    Name = "ResetLicenseCheckTask",
                    Seconds = 2073600,
                    Type = "TvProg.Services.Common.ResetLicenseCheckTask, TvProg.Services",
                    Enabled = true,
                    LastEnabledUtc = lastEnabledUtc,
                    StopOnError = false
                });
            }

            //#3651
            if (!_dataProvider.GetTable<MessageTemplate>().Any(mt => string.Compare(mt.Name, MessageTemplateSystemNames.OrderProcessingUserNotification, StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var manageConnectionStringPermission = _dataProvider.InsertEntity(
                    new MessageTemplate
                    {
                        Name = MessageTemplateSystemNames.OrderProcessingUserNotification,
                        Subject = "%Store.Name%. Your order is processing",
                        Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Order.UserFullName%,{Environment.NewLine}<br />{Environment.NewLine}Your order is processing. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForUser%\">%Order.OrderURLForUser%</a>{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                        IsActive = false,
                        EmailAccountId = _dataProvider.GetTable<EmailAccount>().FirstOrDefault()?.Id ?? 0
                    }
                );
            }

            //#6395
            var paRange = _dataProvider.GetTable<ProductAvailabilityRange>().FirstOrDefault(par => string.Compare(par.Name, "2 week", StringComparison.InvariantCultureIgnoreCase) == 0);
            if (paRange is not null)
            {
                paRange.Name = "2 weeks";
                _dataProvider.UpdateEntity(paRange);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
