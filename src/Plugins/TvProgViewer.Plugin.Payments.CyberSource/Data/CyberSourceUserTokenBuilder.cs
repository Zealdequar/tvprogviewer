using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Data.Extensions;
using TvProgViewer.Data.Mapping.Builders;
using TvProgViewer.Plugin.Payments.CyberSource.Domain;

namespace TvProgViewer.Plugin.Payments.CyberSource.Data
{
    /// <summary>
    /// Represents a user token entity builder
    /// </summary>
    public class CyberSourceUserTokenBuilder : TvProgEntityBuilder<CyberSourceUserToken>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CyberSourceUserToken.UserId)).AsInt32().NotNullable().ForeignKey<User>()
                .WithColumn(nameof(CyberSourceUserToken.SubscriptionId)).AsString(100).NotNullable()
                .WithColumn(nameof(CyberSourceUserToken.LastFourDigitOfCard)).AsString(4).NotNullable()
                .WithColumn(nameof(CyberSourceUserToken.FirstSixDigitOfCard)).AsString(6).Nullable()
                .WithColumn(nameof(CyberSourceUserToken.CardExpirationYear)).AsString(4).NotNullable()
                .WithColumn(nameof(CyberSourceUserToken.CardExpirationMonth)).AsString(2).NotNullable()
                .WithColumn(nameof(CyberSourceUserToken.ThreeDigitCardType)).AsString(3).Nullable()
                .WithColumn(nameof(CyberSourceUserToken.InstrumentIdentifier)).AsString(100).Nullable()
                .WithColumn(nameof(CyberSourceUserToken.InstrumentIdentifierStatus)).AsString(100).Nullable()
                .WithColumn(nameof(CyberSourceUserToken.CyberSourceUserId)).AsString(100).Nullable()
                .WithColumn(nameof(CyberSourceUserToken.TransactionId)).AsString(100).Nullable();
        }

        #endregion
    }
}