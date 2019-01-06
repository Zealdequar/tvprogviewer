using System;
using System.Runtime.Serialization;

namespace TVProgViewer.BusinessLogic.ProgObjs
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
        public DateTime TsStopMOAfter { get; set; }
        
        /// <summary>
        /// Идентификатор телеканала
        /// </summary>
        [DataMember]
        public int CID { get; set; }
    }
}
