using System.Drawing;
using System.Runtime.Serialization;
using TVProgViewer.Data.TvProgMain.ProgInterfaces;

namespace TVProgViewer.Data.TvProgMain.ProgObjs
{
    /// <summary>
    /// Контракт (DTO) для телеканала
    /// </summary>
    [DataContract]
    public class Channel: IChannel
    {

        
        #region Автосвойства

        /// <summary>
        /// Нужно ли отображать канал
        /// </summary>
        [DataMember]
        public bool Visible { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Порядковый номер
        /// </summary>
        [DataMember]
        public uint Number { get; set; }

        /// <summary>
        /// Пользовательское название
        /// </summary>
        [DataMember]
        public string UserSyn { get; set; }

        /// <summary>
        /// Эмблема телеканала
        /// </summary>
        // [DataMember]
        //  public Image Emblem { get; set; }

        /// <summary>
        /// Смещение времени относительно Гринвича
        /// </summary>
        [DataMember]
        public string Diff { get; set; }

        /// <summary>
        /// Пиктограмма канала
        /// </summary>
        [DataMember]
        public long IconId { get; set; }

        /// <summary>
        /// Намиенование канала
        /// </summary>
        [DataMember]
        public string TitleChannel { get; set; }

        #region IChannel Members

        /// <summary>
        /// Идентификатор канала
        /// </summary>
        [DataMember]
        public int ChannelId { get; }

        /// <summary>
        /// Системное название
        /// </summary>
        [DataMember]
        public string SystemTitle { get; }

        /// <summary>
        /// Миграционный идентификатор
        /// </summary>
        [DataMember]
        public int? InternalId { get; }

        /// <summary>
        /// Порядковый номер
        /// </summary>
        [DataMember]
        public int OrderCol { get; }

        /// <summary>
        /// Пользовательское название
        /// </summary>
        [DataMember]
        public string UserTitle { get; }

        /// <summary>
        /// Название файла для крупного логотипа
        /// </summary>
        [DataMember]
        public string FileNameOrig { get; }

        /// <summary>
        /// Название файла для маленького логотипа
        /// </summary>
        [DataMember]
        public string FileName25 { get; }

        /// <summary>
        /// Частота вещания (устаревшее, необходимо для аналогового тв-тюнера)
        /// </summary>
        [DataMember]
        public int Freq { get; }

        #endregion

        #endregion

    }
}
