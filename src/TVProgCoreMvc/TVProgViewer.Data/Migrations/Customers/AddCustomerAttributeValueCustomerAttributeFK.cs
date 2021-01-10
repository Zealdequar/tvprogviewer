using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Users
{
    [TvProgMigration("2019/11/19 02:22:30:4308129")]
    public class AddUserAttributeValueUserAttributeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(UserAttributeValue),
                nameof(UserAttributeValue.UserAttributeId),
                nameof(UserAttribute),
                nameof(UserAttribute.Id),
                Rule.Cascade);
        }

        #endregion
    }
}