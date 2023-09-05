namespace TvProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Пользователи связаны с каналами
    /// </summary>
    public partial class UserChannels: BaseEntity
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Идентификатор телеканала
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// Идентификатор пиктограммы
        /// </summary>
        public long? IconId { get; set; }

        /// <summary>
        /// Пользовательское название телеканала
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Порядок сортировки
        /// </summary>
        public int? OrderCol { get; set; }
    }
}
