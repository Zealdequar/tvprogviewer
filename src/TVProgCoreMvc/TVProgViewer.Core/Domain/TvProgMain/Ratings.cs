using System;

namespace TVProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Рейтинги
    /// </summary>
    public partial class Ratings: BaseEntity
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int? UserId { get; set; }
        
        /// <summary>
        /// Идентификатор пиктограммы
        /// </summary>
        public int? IconId { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTimeOffset CreateDate { get; set; }

        /// <summary>
        /// Наименование рейтинга
        /// </summary>
        public string RatingName { get; set; }

        /// <summary>
        /// Индицирует, видим ли рейтинг
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public DateTimeOffset? DeleteDate { get; set; }
    }
}
