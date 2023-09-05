using System;

namespace TvProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Представляет телепередачу
    /// </summary>
    public partial class Programmes : BaseEntity
    {
        /// <summary>
        /// Идентификатор типа телепрограммы
        /// </summary>
        public int TypeProgId { get; set; }
        
        /// <summary>
        /// Идентификатор телеканала
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// Внутренний идентификатор канала
        /// </summary>
        public int? InternalChanId { get; set; }

        /// <summary>
        /// Начало телепередачи
        /// </summary>
        public DateTime TsStart { get; set; }

        /// <summary>
        ///  Завершение телепередачи
        /// </summary>
        public DateTime TsStop { get; set; }

        /// <summary>
        /// Начало телепередачи по Москве
        /// </summary>
        public DateTime TsStartMo { get; set; }

        /// <summary>
        /// Окончание телепередачи по Москве
        /// </summary>
        public DateTime TsStopMo { get; set; }

        /// <summary>
        /// Название телепередачи
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Анонс телепередачи
        /// </summary>
        public string Descr { get; set; }

        /// <summary>
        /// Категория телепередачи
        /// </summary>
        public string Category { get; set; }
    }
}
