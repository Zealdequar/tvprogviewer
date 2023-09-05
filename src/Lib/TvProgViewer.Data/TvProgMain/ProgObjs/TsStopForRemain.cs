using System;
using System.Runtime.Serialization;

namespace TvProgViewer.Data.TvProgMain.ProgObjs
{
    /// <summary>
    /// Контракт (DTO) для расчета телепередачи, следующей после окончания предыдущей
    /// </summary>
    [DataContract]
    public class TsStopForRemain
    {
        /// <summary>
        /// Время окончания телепередачи
        /// </summary>
        [DataMember]
        public DateTime TsStopMoAfter { get; set; }

        /// <summary>
        /// Идентификатор телеканала
        /// </summary>
        [DataMember]
        public int Cid { get; set; }
    }
}
