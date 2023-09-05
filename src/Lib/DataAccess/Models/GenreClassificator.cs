using System;
using System.Collections.Generic;

namespace TvProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесено на TvProgViewer.Core.Domain
    /// </summary>
    public partial class GenreClassificator
    {
        public long GenreClassificatorId { get; set; }
        public long Gid { get; set; }
        public long? Uid { get; set; }
        public string ContainPhrases { get; set; }
        public string NonContainPhrases { get; set; }
        public int? OrderCol { get; set; }
        public DateTime? DeleteAfterDate { get; set; }

        public virtual Genres G { get; set; }
        public virtual SystemUsers U { get; set; }
    }
}
