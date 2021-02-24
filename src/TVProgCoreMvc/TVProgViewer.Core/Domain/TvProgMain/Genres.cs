using System;

namespace TVProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Жанры
    /// </summary>
    public partial class Genres: BaseEntity
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
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Название жанра
        /// </summary>
        public string GenreName { get; set; }
        
        /// <summary>
        /// Видимость
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public DateTime? DeleteDate { get; set; }
    }
}
