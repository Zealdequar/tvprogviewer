using System;
using System.Runtime.Serialization;

namespace TvProgViewer.Data.TvProgMain.ProgObjs
{
    [DataContract]
    public class GdProgramme
    {
        /// <summary>
        /// Внутренний (миграционный) идентификатор телеканала
        /// </summary>
        [DataMember]
        public int? InternalChanId { get; set; }

        /// <summary>
        /// Начало телепередачи (время мск)
        /// </summary>
        [DataMember]
        public DateTime TsStartMo { get; set; }

        /// <summary>
        /// Завершение телепередачи (время мск)
        /// </summary>
        [DataMember]
        public DateTime TsStopMo { get; set; }

        /// <summary>
        /// Название телепередачи
        /// </summary>
        [DataMember]
        public string TelecastTitle { get; set; }

        /// <summary>
        /// Категория
        /// </summary>
        [DataMember]
        public string Category { get; set; }
    }
}
