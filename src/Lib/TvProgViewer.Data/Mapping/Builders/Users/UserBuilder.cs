using System.Data;
using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Users
{
    /// <summary>
    /// Represents a user entity builder
    /// </summary>
    public partial class UserBuilder : TvProgEntityBuilder<User>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(User.Username)).AsString(1000).Nullable()
                .WithColumn(nameof(User.Email)).AsString(1000).Nullable()
                .WithColumn(nameof(User.EmailToRevalidate)).AsString(1000).Nullable()
                .WithColumn(nameof(User.FirstName)).AsString(1000).Nullable()
                .WithColumn(nameof(User.LastName)).AsString(1000).Nullable()
                .WithColumn(nameof(User.Gender)).AsString(1000).Nullable()
                .WithColumn(nameof(User.Company)).AsString(1000).Nullable()
                .WithColumn(nameof(User.StreetAddress)).AsString(1000).Nullable()
                .WithColumn(nameof(User.StreetAddress2)).AsString(1000).Nullable()
                .WithColumn(nameof(User.ZipPostalCode)).AsString(1000).Nullable()
                .WithColumn(nameof(User.City)).AsString(1000).Nullable()
                .WithColumn(nameof(User.County)).AsString(1000).Nullable()
                .WithColumn(nameof(User.SmartPhone)).AsString(1000).Nullable()
                .WithColumn(nameof(User.Fax)).AsString(1000).Nullable()
                .WithColumn(nameof(User.VatNumber)).AsString(1000).Nullable()
                .WithColumn(nameof(User.GmtZone)).AsString(1000).Nullable()
                .WithColumn(nameof(User.CustomUserAttributesXML)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(User.BirthDate)).AsDateTime2().Nullable()
                .WithColumn(nameof(User.SystemName)).AsString(400).Nullable()
                .WithColumn(nameof(User.CurrencyId)).AsInt32().ForeignKey<Currency>(onDelete: Rule.SetNull).Nullable()
                .WithColumn(nameof(User.LanguageId)).AsInt32().ForeignKey<Language>(onDelete: Rule.SetNull).Nullable()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(User), nameof(User.BillingAddressId))).AsInt32().ForeignKey<Address>(onDelete: Rule.None).Nullable()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(User), nameof(User.ShippingAddressId))).AsInt32().ForeignKey<Address>(onDelete: Rule.None).Nullable();
        }

        #endregion
    }
}