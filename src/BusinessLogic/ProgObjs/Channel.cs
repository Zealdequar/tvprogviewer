using System.Drawing;
using System.Runtime.Serialization;

namespace TVProgViewer.BusinessLogic.ProgObjs
{
    /// <summary>
    /// Контракт (DTO) для телеканала
    /// </summary>
    [DataContract]
    public class Channel
    {

        #region Конструктор
        public Channel(bool visible, string name, uint number, string userSyn, Image emblem, string diff)
        {
            Visible = visible;
            Name = name;
            Number = number;
            UserSyn = userSyn;
            Emblem = emblem;
            Diff = diff;
        }
        #endregion

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
        [DataMember]
        public Image Emblem { get; set; }

        /// <summary>
        /// Смещение времени относительно Гринвича
        /// </summary>
        [DataMember]
        public string Diff { get; set; }

        #endregion

    }
}
