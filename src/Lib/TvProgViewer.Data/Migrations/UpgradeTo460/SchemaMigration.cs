using System.Data;
using FluentMigrator;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Migrations.UpgradeTo460
{
    [TvProgMigration("2022-07-20 00:00:00", "SchemaMigration for 4.60.0", MigrationProcessType.Update)]
    public class SchemaMigration : Migration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            // add column
            var userTableName = nameof(User);

            var firstNameUserColumnName = nameof(User.FirstName);
            var lastNameUserColumnName = nameof(User.LastName);
            var genderUserColumnName = nameof(User.Gender);
            var dobUserColumnName = nameof(User.BirthDate);
            var companyUserColumnName = nameof(User.Company);
            var address1UserColumnName = nameof(User.StreetAddress);
            var address2UserColumnName = nameof(User.StreetAddress2);
            var zipUserColumnName = nameof(User.ZipPostalCode);
            var cityUserColumnName = nameof(User.City);
            var countyUserColumnName = nameof(User.County);
            var countryIdUserColumnName = nameof(User.CountryId);
            var stateIdUserColumnName = nameof(User.StateProvinceId);
            var phoneUserColumnName = nameof(User.SmartPhone);
            var faxUserColumnName = nameof(User.Fax);
            var vatNumberUserColumnName = nameof(User.VatNumber);
            var vatNumberStatusIdUserColumnName = nameof(User.VatNumberStatusId);
            var timeZoneIdUserColumnName = nameof(User.GmtZone);
            var attributeXmlUserColumnName = nameof(User.CustomUserAttributesXML);
            var currencyIdUserColumnName = nameof(User.CurrencyId);
            var languageIdUserColumnName = nameof(User.LanguageId);
            var taxDisplayTypeIdUserColumnName = nameof(User.TaxDisplayTypeId);

            if (!Schema.Table(userTableName).Column(firstNameUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(firstNameUserColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(userTableName).Column(lastNameUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(lastNameUserColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(userTableName).Column(genderUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(genderUserColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(userTableName).Column(dobUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(dobUserColumnName).AsDateTime2().Nullable();
            }
            if (!Schema.Table(userTableName).Column(companyUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(companyUserColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(userTableName).Column(address1UserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(address1UserColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(userTableName).Column(address2UserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(address2UserColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(userTableName).Column(zipUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(zipUserColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(userTableName).Column(cityUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(cityUserColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(userTableName).Column(countyUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(countyUserColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(userTableName).Column(countryIdUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(countryIdUserColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
            }
            if (!Schema.Table(userTableName).Column(stateIdUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(stateIdUserColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
            }
            if (!Schema.Table(userTableName).Column(phoneUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(phoneUserColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(userTableName).Column(faxUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(faxUserColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(userTableName).Column(vatNumberUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(vatNumberUserColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(userTableName).Column(vatNumberStatusIdUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(vatNumberStatusIdUserColumnName).AsInt32().NotNullable().SetExistingRowsTo((int)VatNumberStatus.Unknown);
            }
            if (!Schema.Table(userTableName).Column(timeZoneIdUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(timeZoneIdUserColumnName).AsString(1000).Nullable();
            }
            if (!Schema.Table(userTableName).Column(attributeXmlUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(attributeXmlUserColumnName).AsString(int.MaxValue).Nullable();
            }
            if (!Schema.Table(userTableName).Column(currencyIdUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(currencyIdUserColumnName).AsInt32().ForeignKey<Currency>(onDelete: Rule.SetNull).Nullable();
            }
            if (!Schema.Table(userTableName).Column(languageIdUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(languageIdUserColumnName).AsInt32().ForeignKey<Language>(onDelete: Rule.SetNull).Nullable();
            }
            if (!Schema.Table(userTableName).Column(taxDisplayTypeIdUserColumnName).Exists())
            {
                Alter.Table(userTableName)
                    .AddColumn(taxDisplayTypeIdUserColumnName).AsInt32().Nullable();
            }

            //5705
            var discountTableName = nameof(Discount);
            var isActiveDiscountColumnName = nameof(Discount.IsActive);

            if (!Schema.Table(discountTableName).Column(isActiveDiscountColumnName).Exists())
            {
                Alter.Table(discountTableName)
                    .AddColumn(isActiveDiscountColumnName).AsBoolean().NotNullable().SetExistingRowsTo(true);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
