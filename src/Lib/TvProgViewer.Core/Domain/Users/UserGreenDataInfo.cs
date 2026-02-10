using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvProgViewer.Core.Domain.Users
{
    /// <summary>
    /// Представляет инфо для пользователя из GreenData
    /// </summary>
    public partial class UserGreenDataInfo: BaseEntity
    {
        /// <summary>
        /// Получает или устанавливает пользовательский идентификатор
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Получает или устанавливает операцию, которую вызвала GreenData
        /// </summary>
        public int OperationId { get; set; }

        /// <summary>
        /// Получает или устанавливает количество вызвов операции из GreenData
        /// </summary>
        public long OperationRaiseQty { get; set; }

        /// <summary>
        /// Получает или устанавливает крайнюю дата и время вызова операции
        /// </summary>
        public DateTime? LastOperationRaiseOnUtc { get; set; }
    }
}
