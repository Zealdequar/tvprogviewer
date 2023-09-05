using System.Data;
using FluentMigrator.Builders.Create.Table;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Forums
{
    /// <summary>
    /// Represents a private message entity builder
    /// </summary>
    public partial class PrivateMessageBuilder : TvProgEntityBuilder<PrivateMessage>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(PrivateMessage.Subject)).AsString(450).NotNullable()
                .WithColumn(nameof(PrivateMessage.Text)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(PrivateMessage.FromUserId)).AsInt32().ForeignKey<User>().OnDelete(Rule.None)
                .WithColumn(nameof(PrivateMessage.ToUserId)).AsInt32().ForeignKey<User>().OnDelete(Rule.None);
        }

        #endregion
    }
}