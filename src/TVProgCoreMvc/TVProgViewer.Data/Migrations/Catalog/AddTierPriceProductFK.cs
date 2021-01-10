using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 01:09:03:8051844")]
    public class AddTierPriceProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(TierPrice), 
                nameof(TierPrice.ProductId), 
                nameof(Product), 
                nameof(Product.Id), 
                Rule.Cascade);
        }
        
        #endregion
    }
}