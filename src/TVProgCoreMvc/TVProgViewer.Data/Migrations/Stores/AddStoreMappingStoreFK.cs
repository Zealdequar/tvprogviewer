using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Stores
{
    [TvProgMigration("2019/11/19 05:46:03:9005655")]
    public class AddStoreMappingStoreFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(StoreMapping),
                nameof(StoreMapping.StoreId),
                nameof(Store),
                nameof(Store.Id),
                Rule.Cascade);
        }

        #endregion
    }
}