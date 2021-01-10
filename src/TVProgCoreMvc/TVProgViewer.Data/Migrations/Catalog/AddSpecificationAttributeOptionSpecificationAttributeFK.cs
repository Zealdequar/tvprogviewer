using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 01:02:16:6619708")]
    public class AddSpecificationAttributeOptionSpecificationAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(SpecificationAttributeOption), 
                nameof(SpecificationAttributeOption.SpecificationAttributeId), 
                nameof(SpecificationAttribute), 
                nameof(SpecificationAttribute.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}