using System;

namespace TvProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Телеканалы
    /// </summary>
    public partial class Channels : BaseEntity
    {
        /// <summary>
        /// Идентификатор провайдера
        /// </summary>
        public int TvProgProviderId { get; set; }

        /// <summary>
        /// Внутренний идентификатор
        /// </summary>
        public int? InternalId { get; set; }

        /// <summary>
        /// Идентификатор изображения
        /// </summary>
        public int? IconId { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Название телеканала
        /// </summary>
        public string TitleChannel { get; set; }

        /// <summary>
        /// Ссылка на пиктограмму телеканала
        /// </summary>
        public string IconWebSrc { get; set; }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public DateTime? Deleted { get; set; }

        /// <summary>
        /// Порядок сортировки
        /// </summary>
        public int SysOrderCol { get; set; }
    }
}
