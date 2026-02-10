using System;

namespace TvProgViewer.Core.Domain.TvProgMain
{
    /// <summary>
    /// Представляет телепередачу
    /// </summary>
    public partial class Programmes : BaseEntity
    {
        /// <summary>
        /// Получает или устанавливает идентификатор типа телепрограммы
        /// </summary>
        public int TypeProgId { get; set; }
        
        /// <summary>
        /// Получает или устанавливает идентификатор телеканала
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// Получает или устанавливает внутренний идентификатор канала
        /// </summary>
        public int? InternalChanId { get; set; }

        /// <summary>
        /// Получает или устанавливает начало телепередачи
        /// </summary>
        public DateTime TsStart { get; set; }

        /// <summary>
        ///  Получает или устанавливает завершение телепередачи
        /// </summary>
        public DateTime TsStop { get; set; }

        /// <summary>
        /// Получает или устанавливает начало телепередачи по Москве
        /// </summary>
        public DateTime TsStartMo { get; set; }

        /// <summary>
        /// Получает или устанавливает окончание телепередачи по Москве
        /// </summary>
        public DateTime TsStopMo { get; set; }

        /// <summary>
        /// Получает или устанавливает название телепередачи
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Получает или устанавливает анонс телепередачи
        /// </summary>
        public string Descr { get; set; }

        /// <summary>
        /// Получает или устанавливает категорию телепередачи (устаревшее)
        /// </summary>
        public string Category { get; set; }
    }
}
