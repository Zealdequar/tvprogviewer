using System;

namespace TVProgViewer.Data.TvProgMain.ProgObjs
{
    /// <summary>
    /// Период программы телепередач
    /// </summary>
    public struct ProgPeriod
    {
        /// <summary>
        /// Время начала первой телепередачи
        /// </summary>
        public DateTimeOffset? dtStart;

        /// <summary>
        /// Время окончания последней телепередачи
        /// </summary>
        public DateTimeOffset? dtEnd;
    }
}
