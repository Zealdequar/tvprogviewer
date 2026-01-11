using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TvProgViewer.Data.TvProgMain.ProgObjs
{
    [DataContract]
    public class GdProgrammeStatus
    {
        /// <summary>
        /// Количество передач в программе
        /// </summary>
        [DataMember]
        public int Qty { get; set; }

        /// <summary>
        /// Дата/время последней передачи
        /// </summary>
        [DataMember]
        public DateTime? MaxDateTime { get; set;}
    }
}
