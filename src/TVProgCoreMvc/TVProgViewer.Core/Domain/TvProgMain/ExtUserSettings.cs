namespace TVProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Дополнительные пользовательские настройки
    /// </summary>
    public partial class ExtUserSettings: BaseEntity
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Тип провайдера программы телепередач
        /// </summary>
        public int TvProgProviderId { get; set; }

        /// <summary>
        /// Неотмеченные телеканалы
        /// </summary>
        public int? UncheckedChannels { get; set; }
    }
}
