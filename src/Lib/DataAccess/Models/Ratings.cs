using System;
using System.Collections.Generic;

namespace TvProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесён на TvProgViewer.Core.Domain
    /// </summary>
    public partial class Ratings
    {
        public Ratings()
        {
            RatingClassificator = new HashSet<RatingClassificator>();
            UsersPrograms = new HashSet<UsersPrograms>();
        }

        public long RatingId { get; set; }
        public long? Uid { get; set; }
        public long? IconId { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string RatingName { get; set; }
        public bool Visible { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }

        public virtual MediaPic Icon { get; set; }
        public virtual SystemUsers U { get; set; }
        public virtual ICollection<RatingClassificator> RatingClassificator { get; set; }
        public virtual ICollection<UsersPrograms> UsersPrograms { get; set; }
    }
}
