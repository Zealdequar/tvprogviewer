using FluentMigrator;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037686")]
    public class AddGenericAttributeEntityIdKeyGroupIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_GenericAttribute_EntityId_and_KeyGroup", nameof(GenericAttribute), i => i.Ascending(),
                nameof(GenericAttribute.EntityId), nameof(GenericAttribute.KeyGroup));
        }

        #endregion
    }
}