using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Data.Mapping.Builders;
using TvProgViewer.Plugin.Shipping.FixedByWeightByTotal.Domain;

namespace TvProgViewer.Plugin.Shipping.FixedByWeightByTotal.Data
{
    public class ShippingByWeightByTotalRecordBuilder : TvProgEntityBuilder<ShippingByWeightByTotalRecord>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ShippingByWeightByTotalRecord.WeightFrom))
                .AsDecimal(18, 4)
                .WithColumn(nameof(ShippingByWeightByTotalRecord.WeightTo))
                .AsDecimal(18, 4)
                .WithColumn(nameof(ShippingByWeightByTotalRecord.OrderSubtotalFrom))
                .AsDecimal(18, 4)
                .WithColumn(nameof(ShippingByWeightByTotalRecord.OrderSubtotalTo))
                .AsDecimal(18, 4)
                .WithColumn(nameof(ShippingByWeightByTotalRecord.AdditionalFixedCost))
                .AsDecimal(18, 4)
                .WithColumn(nameof(ShippingByWeightByTotalRecord.PercentageRateOfSubtotal))
                .AsDecimal(18, 4)
                .WithColumn(nameof(ShippingByWeightByTotalRecord.RatePerWeightUnit))
                .AsDecimal(18, 4)
                .WithColumn(nameof(ShippingByWeightByTotalRecord.LowerWeightLimit))
                .AsDecimal(18, 4)
                .WithColumn(nameof(ShippingByWeightByTotalRecord.Zip))
                .AsString(400)
                .Nullable();
        }
    }
}