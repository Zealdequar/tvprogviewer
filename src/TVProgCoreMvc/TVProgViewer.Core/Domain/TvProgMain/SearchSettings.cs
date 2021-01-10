using System;

namespace TVProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Настройки для поиска телепередач
    /// </summary>
    public partial class SearchSettings : BaseEntity
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Сохранять ли параметры фильтра
        /// </summary>
        public bool LoadSettings { get; set; }

        /// <summary>
        /// Совпадает
        /// </summary>
        public string Match { get; set; }

        /// <summary>
        /// Не совпадает
        /// </summary>
        public string NotMatch { get; set; }

        /// <summary>
        /// Искать ли в анонсах
        /// </summary>
        public bool? InAnons { get; set; }

        /// <summary>
        /// С какого времени искать
        /// </summary>
        public DateTime? TsFinalFrom { get; set; }

        /// <summary>
        /// По какое время искать
        /// </summary>
        public DateTime? TsFinalTo { get; set; }

        /// <summary>
        /// Координаты ползунка от
        /// </summary>
        public int? TrackBarFrom { get; set; }

        /// <summary>
        /// Координаты ползунка до
        /// </summary>
        public int? TrackBarTo { get; set; }
    }
}
