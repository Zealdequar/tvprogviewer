namespace TvProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Пользователи связаны с телепрограммой
    /// </summary>
    public partial class UsersPrograms: BaseEntity
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Идентификатор пользовательского телеканала
        /// </summary>
        public int UserChannelId { get; set; }
        
        /// <summary>
        /// Идентификатор телепередачи
        /// </summary>
        public long ProgrammesId { get; set; }

        /// <summary>
        /// Идентифкатор жанра
        /// </summary>
        public long? GenreId { get; set; }

        /// <summary>
        /// Идентификатор рейтинга
        /// </summary>
        public long? RatingId { get; set; }

        /// <summary>
        /// Анонс телепередачи
        /// </summary>
        public string Anons { get; set; }

        /// <summary>
        /// Индицирует, нужно ли напомнить о телепередаче
        /// </summary>
        public bool Remind { get; set; }
    }
}
