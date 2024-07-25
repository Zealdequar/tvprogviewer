using System.Runtime.Serialization;

namespace TvProgViewer.Data.TvProgMain.ProgObjs
{
    [DataContract]
    public class GdChannel
    {
        /// <summary>
        /// Внутренний (миграционный) идентификатор
        /// </summary>
        [DataMember]
        public int? InternalId { get; set; }

        /// <summary>
        /// Название телеканала
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Порядковый номер телеканала
        /// </summary>
        [DataMember]
        public int SysOrderCol { get; set; }

        /// <summary>
        /// Пользовательский рейтинг
        /// </summary>
        [DataMember]
        public int? UserRating { get; set; }
    }
}
