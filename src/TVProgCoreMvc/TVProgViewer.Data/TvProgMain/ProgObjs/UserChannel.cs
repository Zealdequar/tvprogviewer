using System.Runtime.Serialization;
using TVProgViewer.Data.TvProgMain.ProgInterfaces;

namespace TVProgViewer.Data.TvProgMain.ProgObjs
{
    /// <summary>
    /// Контракт (DTO) для пользовательского телеканала
    /// </summary>
    [DataContract]
    public class UserChannel : IChannel
    {
        /// <summary>
        /// Идентификатор пользовательского телеканала
        /// </summary>
        [DataMember]
        public int UserChannelId { get; set; }

        /// <summary>
        /// Системный идентификатор
        /// </summary>
        [DataMember]
        public int ChannelId { get; set; }

        /// <summary>
        /// Идентификатор провайдера телепередач
        /// </summary>
        [DataMember]
        public int TVProgViewerId { get; set; }

        /// <summary>
        /// Внутренний (миграционный) идентификатор
        /// </summary>
        [DataMember]
        public int? InternalId { get; set; }

        /// <summary>
        /// Идентификатор пиктограммы телеканала
        /// </summary>
        [DataMember]
        public long? IconId { get; set; }

        /// <summary>
        /// Названия файла крупной пиктограммы телеканала
        /// </summary>
        [DataMember]
        public string FileNameOrig { get; set; }

        /// <summary>
        /// Названия файла крупной пиктограммы телеканала
        /// </summary>
        [DataMember]
        public string FileName25 { get; set; }

        /// <summary>
        /// Название телеканала
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Адрес пиктограммы телеканала
        /// </summary>
        [DataMember]
        public string ImageWebSrc { get; set; }


        #region IChannel Members

        private bool _visible;
        private string _diff;

        /// <summary>
        /// Нужно ли отображать?
        /// </summary>
        public bool Visible
        {
            get
            {
                return true;
            }
            set
            {
                _visible = value;
            }
        }

        /// <summary>
        /// Системное название
        /// </summary>
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
        public int OrderCol { get; set; }

        /// <summary>
        /// Пользовательское название
        /// </summary>
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
        /// Частота (устаревшее поле, использовалось для работы с аналоговым тв-тюнером)
        /// </summary>
        public int Freq { get; set; }

        #endregion
    }
}
