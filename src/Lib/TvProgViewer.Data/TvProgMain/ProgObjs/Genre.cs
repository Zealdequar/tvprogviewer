using System;
using System.Runtime.Serialization;

namespace TvProgViewer.Data.TvProgMain.ProgObjs
{
    /// <summary>
    /// Контракт (DTO) для жанра
    /// </summary>
    [DataContract]
    public class Genre
    {
        /// <summary>
        /// Идентификатор жанра
        /// </summary>
        [DataMember]
        public int GenreId { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [DataMember]
        public int? Uid { get; set; }

        /// <summary>
        /// Идентификатор пиктограммы
        /// </summary>
        [DataMember]
        public int? IconId { get; set; }

        /// <summary>
        /// Путь к пиктограмме жанра
        /// </summary>
        [DataMember]
        public string GenrePath { get; set; }

        /// <summary>
        /// Дата создания жанра
        /// </summary>
        [DataMember]
        public DateTimeOffset CreateDate { get; set; }

        /// <summary>
        /// Название жанра
        /// </summary>
        [DataMember]
        public string GenreName { get; set; }

        /// <summary>
        /// Видимость жанра
        /// </summary>
        [DataMember]
        public bool Visible { get; set; }

        /// <summary>
        /// Дата удаления жанра
        /// </summary>
        [DataMember]
        public DateTimeOffset? DeleteDate { get; set; }
    }
}
