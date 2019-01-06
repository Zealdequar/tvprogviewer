using System;

namespace TVProgViewer.BusinessLogic.ProgObjs
{
    /// <summary>
    /// Период программы телепередач
    /// </summary>
    public struct ProgPeriod
    {
        /// <summary>
        /// Время начала первой телепередачи
        /// </summary>
        public DateTimeOffset dtStart;

        /// <summary>
        /// Время окончания последней телепередачи
        /// </summary>
        public DateTimeOffset dtEnd;
    }
}
