using System;
using System.Runtime.Serialization;
using static TVProgViewer.Data.TvProgMain.Enums;

namespace TVProgViewer.Data.TvProgMain.Updater
{
    /// <summary>
    /// Контракт (DTO) для ресурсов телепрограммы
    /// </summary>
    [DataContract]
    public class WebResource
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [DataMember]
        public int WebResourceId { get; set; }

        /// <summary>
        /// Формат телепрограммы
        /// </summary>
        [DataMember]
        public TypeProg SourceType { get; set; }

        /// <summary>
        /// Название файла для сохранения
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        /// Описание ресурса
        /// </summary>
        [DataMember]
        public string ResourceName { get; set; }

        /// <summary>
        /// Адрес ресурса в интернете
        /// </summary>
        [DataMember]
        public Uri WrUri { get; set; }

        /// <summary>
        /// XML-представление источника для загрузки в БД
        /// </summary>
        public string xmlDoc { get; set; }

        /// <summary>
        /// Синдикация содержимого
        /// </summary>
        [DataMember]
        public string Rss { get; set; }
    }
}
