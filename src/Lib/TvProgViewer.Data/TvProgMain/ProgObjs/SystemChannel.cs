using System;
using System.Runtime.Serialization;
using TvProgViewer.Data.TvProgMain.ProgInterfaces;

namespace TvProgViewer.Data.TvProgMain.ProgObjs
{
    /// <summary>
    /// Контракт (DTO) для системного телеканала
    /// </summary>
    [DataContract]
    public class SystemChannel : IChannel
    {
        /// <summary>
        /// Пользовательский идентификатор телеканала
        /// </summary>
        [DataMember]
        public int UserChannelId { get; set; }

        /// <summary>
        /// Системный идентификатор канала
        /// </summary>
        [DataMember]
        public int ChannelId { get; set; }

        /// <summary>
        /// Идентификатор провайдера телепрограммы
        /// </summary>
        [DataMember]
        public int TvProgViewerId { get; set; }

        /// <summary>
        /// Внутренний идентификатор канала
        /// </summary>
        [DataMember]
        public int? InternalId { get; set; }

        /// <summary>
        /// Идентификатор пиктограммы
        /// </summary>
        [DataMember]
        public long? IconId { get; set; }

        /// <summary>
        /// Название файла крупной пиктограммы
        /// </summary>
        [DataMember]
        public string FileNameOrig { get; set; }

        /// <summary>
        /// Название файла маленькой пиктограммы
        /// </summary>
        [DataMember]
        public string FileName25 { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Адрес размещения пиктограммы
        /// </summary>
        [DataMember]
        public string ImageWebSrc { get; set; }

        #region IChannel Members

        private string _diff;

        /// <summary>
        /// Нужно ли отображать?
        /// </summary>
        [DataMember]
        public bool Visible { get; set; }

        /// <summary>
        /// Системное название
        /// </summary>
        [DataMember]
        public string SystemTitle
        {
            get
            {
                return Title;
            }
            set
            {
                Title = value;
            }
        }

        /// <summary>
        /// Порядковый номер телеканала
        /// </summary>
        [DataMember]
        public int SysOrderCol { get; set; }

        /// <summary>
        /// Пользовательское название телеканала
        /// </summary>
        [DataMember]
        public string UserTitle
        {
            get
            {
                return Title;
            }
            set
            {
                Title = value;
            }
        }

        /// <summary>
        /// Смещение времени относительно Гринвича
        /// </summary>
        [DataMember]
        public string Diff
        {
            get
            {
                return "+03:00";
            }
            set
            {
                _diff = value;
            }
        }

        /// <summary>
        /// Частота телеканала (устаревшее, использовалось для работы с аналоговым тв-тюнером)
        /// </summary>
        public int Freq { get; set; }

        #endregion
    }
}
