using Azure.Storage.Blobs.Models;
using FluentMigrator.Builders.Create.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.TvProgMain;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Data.Extensions;

namespace TvProgViewer.Data.Mapping.Builders.Users
{
    /// <summary>
    /// Представляет постоитель для маппинга пользователей и каналов
    /// </summary>
    public partial class UserChannelMappingBuilder: TvProgEntityBuilder<UserChannelMapping>
    {
        #region Методы
        /// <summary>
        /// Применяет конфигурацию сущности
        /// </summary>
        /// <param name="table">Создаёт строителя табличного выражения</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(UserChannelMapping), nameof(UserChannelMapping.ChannelId)))
                    .AsInt32().ForeignKey<Channels>().PrimaryKey()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(UserChannelMapping), nameof(UserChannelMapping.UserId)))
                    .AsInt32().ForeignKey<User>().PrimaryKey()
                .WithColumn(nameof(UserChannelMapping.DisplayName)).AsString(255).Nullable();
        }

        #endregion
    }
}
