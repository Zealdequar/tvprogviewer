using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvProgViewer.Core.Domain.Users
{
    /// <summary>
    /// Представляет класс маппинг для пользователей и каналов
    /// </summary>
    public partial class UserChannelMapping: BaseEntity
    {
        /// <summary>
        /// Получает или устанавливает пользовательский идентификатор
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Получает или устанавливает пользовательский телеканал
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// Получает или устанавливает дату и время установки
        /// </summary>

        public DateTime? SetDate { get; set; }
        
        /// <summary>
        /// Получает или устанавливает логотип телеканала
        /// </summary>
        public int? IconId { get; set; }

        /// <summary>
        /// Получает или устанавливает пользовательское наименование телеканала
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Получает или устанавливает пользовательский порядок для сортировки
        /// </summary>
        public int? OrderCol { get; set; }

    }
}
