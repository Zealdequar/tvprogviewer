using System;
using System.Runtime.Serialization;
using TvProgViewer.Data.TvProgMain.ProgInterfaces;

namespace TvProgViewer.Data.TvProgMain.ProgObjs
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
        public long ProgrammesId { get; set; }

        /// <summary>
        /// Системный идентификатор телеканала
        /// </summary>
        [DataMember]
        public int Cid { get; set; }

        /// <summary>
        /// Название файла пиктограмы телеканала
        /// </summary>
        [DataMember]
        public string ChannelContent { get; set; }

        /// <summary>
        /// Внутренний (миграционный) идентификатор телеканала
        /// </summary>
        [DataMember]
        public int? InternalChanId { get; set; }

        /// <summary>
        /// Время начала телепередачи
        /// </summary>
        [DataMember]
        public DateTimeOffset TsStart { get; set; }

        /// <summary>
        /// Осталось секунд до телепередачи
        /// </summary>
        [DataMember]
        public int? Remain { get; set; }

        /// <summary>
        /// Устаревшее поле, используемое в предыдущих версиях TvProgViewer
        /// </summary>
        public DateTime NextRemain
        {
            get
            {
                return new DateTime(1, 1, 1).AddSeconds((int)Remain);
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
        public DateTime TsStartMo { get; set; }

        /// <summary>
        /// Завершение телепередачи (время мск)
        /// </summary>
        [DataMember]
        public DateTime TsStopMo { get; set; }

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
        public long? GenreId { get; set; }

        /// <summary>
        /// Идентификатор рейтинга
        /// </summary>
        public long? RatingId { get; set; }
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
        public long ProgrammeId { get; set; }

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
        public int OrderCol { get; set; }

        #endregion
    }
}
