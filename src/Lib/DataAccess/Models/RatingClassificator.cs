using System;

namespace TvProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесено на TvProgViewer.Core.Domain
    /// </summary>
    public partial class RatingClassificator
    {
        public long RatingClassificatorId { get; set; }
        public long Rid { get; set; }
        public long? Uid { get; set; }
        public string ContainPhrases { get; set; }
        public string NonContainPhrases { get; set; }
        public int? OrderCol { get; set; }
        public DateTime? DeleteAfterDate { get; set; }

        public virtual Ratings R { get; set; }
        public virtual SystemUsers U { get; set; }
    }
}
