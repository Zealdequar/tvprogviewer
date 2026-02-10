using System;

namespace TvProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Представляет категорию передачи
    /// </summary>
    public partial class TvProgCategory: BaseEntity
    {
        /// <summary>
        /// Получает или устанавливает наименование категории
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Получает или устанавливает системный идентификатор
        /// </summary>
        public string Ident { get; set; }

        /// <summary>
        /// Получает или устанавливает признак «Действует»?
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Получает или устанавливает дату/время создания (UTC)
        /// </summary>
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Получает или устанавливает порядок (для отображения)
        /// </summary>
        public int? Order { get; set; }
    }
}
