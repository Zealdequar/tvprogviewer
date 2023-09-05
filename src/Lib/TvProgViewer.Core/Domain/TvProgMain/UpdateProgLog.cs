using System;

namespace TvProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Лог обновлений телеканалов и телепередач
    /// </summary>
    public partial class UpdateProgLog: BaseEntity
    {
        /// <summary>
        /// Идентификатор веб-ресурса для обновления
        /// </summary>
        public int WebResourceId { get; set; }

        /// <summary>
        /// Дата и время начала обновления
        /// </summary>
        public DateTime TsUpdateStart { get; set; }

        /// <summary>
        /// Дата и время окончания обновления
        /// </summary>
        public DateTime TsUpdateEnd { get; set; }

        /// <summary>
        /// Сколько секунд заняло обновление
        /// </summary>
        public int? UdtElapsedSec { get; set; }

        /// <summary>
        /// Минимальная дата и время эфира
        /// </summary>
        public DateTime MinProgDate { get; set; }

        /// <summary>
        /// Максимальная дата и время эфира
        /// </summary>
        public DateTime MaxProgDate { get; set; }

        /// <summary>
        /// Количество телеканалов
        /// </summary>
        public int QtyChans { get; set; }

        /// <summary>
        /// Количество телепередач
        /// </summary>
        public int QtyProgrammes { get; set; }

        /// <summary>
        /// Количество новых телеканалов
        /// </summary>
        public int QtyNewChans { get; set; }

        /// <summary>
        /// Количество новых телепередач
        /// </summary>
        public int QtyNewProgrammes { get; set; }

        /// <summary>
        /// Индицирует, успешно ли завершилось обновление
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Сообщение об ошибке в случае IsSuccess = false
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
