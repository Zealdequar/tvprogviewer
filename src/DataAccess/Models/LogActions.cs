using System;
using System.Collections.Generic;

namespace TVProgViewer.DataAccess.Models
{
    /// <summary>
    /// Есть альтернатива на TVProgViewer.Core.Domain
    /// </summary>
    public partial class LogActions
    {
        public int LogId { get; set; }
        public string Login { get; set; }
        public DateTimeOffset? TsAction { get; set; }
        public short TypeAction { get; set; }
        public string MessageAction { get; set; }
        public string Ip { get; set; }
        public string UserAgent { get; set; }
    }
}
