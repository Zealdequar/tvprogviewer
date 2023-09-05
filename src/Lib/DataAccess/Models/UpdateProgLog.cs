using System;
using System.Collections.Generic;

namespace TvProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесено на TvProgViewer.Core.Domain
    /// </summary>
    public partial class UpdateProgLog
    {
        public long UpdateProgLogId { get; set; }
        public int Wrid { get; set; }
        public DateTimeOffset TsUpdateStart { get; set; }
        public DateTimeOffset TsUpdateEnd { get; set; }
        public int? UdtElapsedSec { get; set; }
        public DateTimeOffset MinProgDate { get; set; }
        public DateTimeOffset MaxProgDate { get; set; }
        public int QtyChans { get; set; }
        public int QtyProgrammes { get; set; }
        public int QtyNewChans { get; set; }
        public int QtyNewProgrammes { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public virtual WebResources Wr { get; set; }
    }
}
