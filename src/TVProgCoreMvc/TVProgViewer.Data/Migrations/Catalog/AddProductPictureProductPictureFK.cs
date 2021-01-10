using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Media;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Mapping;

namespace TVProgViewer.Data.Migrations.Catalog
{
    [TvProgMigration("2019/11/19 12:19:26:2625749")]
    public class AddProductPictureProductPictureFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(TvProgMappingDefaults.ProductPictureTable,
                nameof(ProductPicture.PictureId),
                nameof(Picture),
                nameof(Picture.Id),
                Rule.Cascade);
        }

        #endregion
    }
}