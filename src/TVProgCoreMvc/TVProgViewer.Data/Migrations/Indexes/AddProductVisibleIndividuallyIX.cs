using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/13 09:36:08:9037706")]
    public class AddProductVisibleIndividuallyIX : AutoReversingMigration
    {
        #region Methods      

        public override void Up()
        {
            Create.Index("IX_Product_VisibleIndividually").OnTable(nameof(Product))
                .OnColumn(nameof(Product.VisibleIndividually)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}