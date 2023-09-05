using System;
using System.Collections.Generic;

namespace TvProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесено на TvProgViewer.Core.Domain
    /// </summary>
    public partial class Genres
    {
        public Genres()
        {
            GenreClassificator = new HashSet<GenreClassificator>();
            UsersPrograms = new HashSet<UsersPrograms>();
        }

        public long GenreId { get; set; }
        public long? Uid { get; set; }
        public long? IconId { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string GenreName { get; set; }
        public bool Visible { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }

        public virtual MediaPic Icon { get; set; }
        public virtual SystemUsers U { get; set; }
        public virtual ICollection<GenreClassificator> GenreClassificator { get; set; }
        public virtual ICollection<UsersPrograms> UsersPrograms { get; set; }
    }
}
