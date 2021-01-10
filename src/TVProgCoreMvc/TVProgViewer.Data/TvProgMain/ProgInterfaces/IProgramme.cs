using System;

namespace TVProgViewer.Data.TvProgMain.ProgInterfaces
{
    /// <summary>
    /// Спецификация для телепередачи
    /// </summary>
    public interface IProgramme
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        long ProgrammeId { get; }

        /// <summary>
        /// Номер канала
        /// </summary>
        int ChannelNumber { get; }

        /// <summary>
        /// Название канала
        /// </summary>
        string ChannelName { get; }

        /// <summary>
        /// Название файла логотипа телеканала
        /// </summary>
        string ChannelContent { get; }

        /// <summary>
        /// Начало телепередачи
        /// </summary>
        DateTimeOffset Start { get; }

        /// <summary>
        /// Окончание телепередачи
        /// </summary>
        DateTimeOffset Stop { get; }

        /// <summary>
        /// Осталось секунд до начала телепередачи
        /// </summary>
        int? Remain { get; }

        /// <summary>
        /// Название телепередачи
        /// </summary>
        string TelecastTitle { get; }

        /// <summary>
        /// Название файла для анонса
        /// </summary>
        string AnonsContent { get; }

        /// <summary>
        /// Анонс телепередачи
        /// </summary>
        string TelecastDescr { get; }

        /// <summary>
        /// Категория телепередачи
        /// </summary>
        string TelecastCategory { get; }
    }
}
