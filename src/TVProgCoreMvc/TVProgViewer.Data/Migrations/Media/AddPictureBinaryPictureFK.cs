using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Media;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Media
{
    [TvProgMigration("2019/11/19 05:01:09:5631609")]
    public class AddPictureBinaryPictureFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(PictureBinary),
                nameof(PictureBinary.PictureId),
                nameof(Picture),
                nameof(Picture.Id),
                Rule.Cascade);
        }

        #endregion
    }
}