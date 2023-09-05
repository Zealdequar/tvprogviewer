using System;
using System.Collections.Generic;

namespace TvProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесено на TvProgViewer.Core.Domain
    /// </summary>
    public partial class SearchSettings
    {
        public int SearchSettingsId { get; set; }
        public long Uid { get; set; }
        public bool LoadSettings { get; set; }
        public string Match { get; set; }
        public string NotMatch { get; set; }
        public bool? InAnons { get; set; }
        public DateTime? TsFinalFrom { get; set; }
        public DateTime? TsFinalTo { get; set; }
        public int? TrackBarFrom { get; set; }
        public int? TrackBarTo { get; set; }

        public virtual SystemUsers U { get; set; }
    }
}
