using System;
using System.Runtime.Serialization;
using TVProgViewer.BusinessLogic.ProgInterfaces;

namespace TVProgViewer.BusinessLogic.ProgObjs
{
    /// <summary>
    /// Контракт (DTO) для системных телепередач
    /// </summary>
    [DataContract]
    public class SystemProgramme : IProgramme
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [DataMember]
        public long ProgrammesID { get; set; }

        /// <summary>
        /// Системный идентификатор телеканала
        /// </summary>
        [DataMember]
        public int CID { get; set; }

        /// <summary>
        /// Название файла пиктограмы телеканала
        /// </summary>
        [DataMember]
        public string ChannelContent { get; set; }

        /// <summary>
        /// Внутренний (миграционный) идентификатор телеканала
        /// </summary>
        [DataMember]
        public int? InternalChanID { get; set; }

        /// <summary>
        /// Время начала телепередачи
        /// </summary>
        [DataMember]
        public DateTimeOffset TsStart { get; set; }

        /// <summary>
        /// Осталось секунд до телепередачи
        /// </summary>
        [DataMember]
        public int Remain { get; set; }

        /// <summary>
        /// Устаревшее поле, используемое в предыдущих версиях TVProgViewer
        /// </summary>
        public DateTime NextRemain
        {
            get
            {
                return new DateTime(1, 1, 1).AddSeconds(Remain);
            }
        }

        /// <summary>
        /// Текст анонса телепередачи
        /// </summary>
        [DataMember]
        public string TelecastDescr { get; set; }

        /// <summary>
        /// Название файла для пиктограммы анонса
        /// </summary>
        [DataMember]
        public string AnonsContent { get; set; }

        /// <summary>
        /// Категория
        /// </summary>
        [DataMember]
        public string Category { get; set; }

        /// <summary>
        /// Начало телепередачи (время мск)
        /// </summary>
        [DataMember]
        public DateTime TsStartMO { get; set; }

        /// <summary>
        /// Завершение телепередачи (время мск)
        /// </summary>
        [DataMember]
        public DateTime TsStopMO { get; set; }

        /// <summary>
        /// Название рейтинга
        /// </summary>
        [DataMember]
        public string RatingName { get; set; }

        /// <summary>
        /// Название файла пиктограммы рейтинга
        /// </summary>
        [DataMember]
        public string RatingContent { get; set; }
        
        /// <summary>
        /// Идентификатор жанра
        /// </summary>
        public long? GenreID { get; set; }

        /// <summary>
        /// Идентификатор рейтинга
        /// </summary>
        public long? RatingID { get; set; }
        /// <summary>
        /// Название жанра
        /// </summary>
        [DataMember]
        public string GenreName { get; set; }

        /// <summary>
        /// Название файла пиктограммы жанра
        /// </summary>
        [DataMember]
        public string GenreContent { get; set; }

        /// <summary>
        /// Путь к пиктограмме жанра
        /// </summary>
        [DataMember]
        public string GenrePath { get; set; }
        
        /// <summary>
        /// День и месяц показа
        /// </summary>
        [DataMember]
        public string DayMonth { get; set; }

        #region IProgramme Members

        /// <summary>
        /// Идентификатор телепрограммы
        /// </summary>
        [DataMember]
        public long ProgrammeID { get; set; }

        /// <summary>
        /// Номер канала
        /// </summary>
        [DataMember]
        public int ChannelNumber { get; set; }

        /// <summary>
        /// Название канала
        /// </summary>
        [DataMember]
        public string ChannelName { get; set; }

        /// <summary>
        /// Время начала телепередачи
        /// </summary>
        [DataMember]
        public DateTimeOffset Start { get; set; }

        /// <summary>
        /// Время окончания телепередачи
        /// </summary>
        [DataMember]
        public DateTimeOffset Stop { get; set; }

        /// <summary>
        /// Название телепередачи
        /// </summary>
        [DataMember]
        public string TelecastTitle { get; set; }

        /// <summary>
        /// Категория телепредачи
        /// </summary>
        [DataMember]
        public string TelecastCategory { get; set; }

        /// <summary>
        /// Порядковый номер канала
        /// </summary>
        [DataMember]
        public int? OrderCol { get; set; }

        #endregion
    }
}
