using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvProgViewer.Data.TvProgMain.ProgInterfaces
{
    /// <summary>
    /// Спецификация для телеканала
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// Идентификатор канала
        /// </summary>
        int ChannelId { get; }

        /// <summary>
        /// Нужно ли отображать?
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// Системное название
        /// </summary>
        string SystemTitle { get; }

        /// <summary>
        /// Миграционный идентификатор
        /// </summary>
        int? InternalId { get; }

        /// <summary>
        /// Порядковый номер
        /// </summary>
        int OrderCol { get; }

        /// <summary>
        /// Пользовательское название
        /// </summary>
        string UserTitle { get; }

        /// <summary>
        /// Название файла для крупного логотипа
        /// </summary>
        string FileNameOrig { get; }

        /// <summary>
        /// Название файла для маленького логотипа
        /// </summary>
        string FileName25 { get; }

        /// <summary>
        /// Смещение времени относительно Гринвича
        /// </summary>
        string Diff { get; }

        /// <summary>
        /// Частота вещания (устаревшее, необходимо для аналогового тв-тюнера)
        /// </summary>
        int Freq { get; }
    }
}
