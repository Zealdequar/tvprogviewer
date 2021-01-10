using System;
using System.Collections.Generic;

namespace TVProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесено на TVProgViewer.Core.Domain
    /// </summary>
    public partial class MediaPic
    {
        public MediaPic()
        {
            Channels = new HashSet<Channels>();
            Genres = new HashSet<Genres>();
            Ratings = new HashSet<Ratings>();
            UserChannels = new HashSet<UserChannels>();
        }

        public long IconId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string ContentCoding { get; set; }
        public int Length { get; set; }
        public int? Length25 { get; set; }
        public bool IsSystem { get; set; }
        public string PathOrig { get; set; }
        public string Path25 { get; set; }

        public virtual ICollection<Channels> Channels { get; set; }
        public virtual ICollection<Genres> Genres { get; set; }
        public virtual ICollection<Ratings> Ratings { get; set; }
        public virtual ICollection<UserChannels> UserChannels { get; set; }
    }
}
