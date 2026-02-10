using System;

namespace TvProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Сопоставление для передач и категорий
    /// </summary>
    public class ProgrammesTvProgCategoryMapping: BaseEntity
    {
        /// <summary>
        /// Получает или устанавливает идентификатор передачи
        /// </summary>
        public int ProgrammesId { get; set; }

        /// <summary>
        /// Получает или устанавливает идентификатор категории для передачи
        /// </summary>
        public int TvProgCategoryId { get; set; }
    }
}
