using System;

namespace TvProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Классификатор рейтингов
    /// </summary>
    public partial class RatingClassificator: BaseEntity
    {
        /// <summary>
        /// Идентификатор рейтинга
        /// </summary>
        public int RatingId { get; set; }

        /// <summary>
        /// Идентификатор пользователя; если null, тогда для всех пользователей
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Содедржит фразу
        /// </summary>
        public string ContainPhrases { get; set; }

        /// <summary>
        /// Не содержит фразу
        /// </summary>
        public string NonContainPhrases { get; set; }

        /// <summary>
        /// Порядок сортировки
        /// </summary>
        public int? OrderCol { get; set; }

        /// <summary>
        /// Удалить после наступления даты
        /// </summary>
        public DateTime? DeleteAfterDate { get; set; }
    }
}
