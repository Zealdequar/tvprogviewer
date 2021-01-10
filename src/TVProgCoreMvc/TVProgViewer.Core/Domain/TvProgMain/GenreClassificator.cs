using System;

namespace TVProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Жанровый классификатор
    /// </summary>
    public partial class GenreClassificator: BaseEntity
    {
        /// <summary>
        /// Идентификатор жанра
        /// </summary>
        public int GenreId { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Содержит фразу
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
        /// Дата, после которой необходимо удалить классификацию
        /// </summary>
        public DateTime? DeleteAfterDate { get; set; }

    }
}
