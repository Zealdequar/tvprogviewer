using FluentMigrator;
using TVProgViewer.Core.Domain.Affiliates;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Affiliates
{
    [TvProgMigration("2019/11/19 11:24:16:2551771")]
    public class AddAffiliateAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Affiliate), 
                nameof(Affiliate.AddressId), 
                nameof(Address), 
                nameof(Address.Id));
        }

        #endregion
    }
}