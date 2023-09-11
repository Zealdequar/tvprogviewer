using System;
using System.Collections.Generic;

namespace TvProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесено на TvProgViewer.Core.Domain
    /// </summary>
    public partial class Channels
    {
        public Channels()
        {
            Programmes = new HashSet<Programmes>();
            UserChannels = new HashSet<UserChannels>();
        }

        public int ChannelId { get; set; }
        public int TvprogProviderId { get; set; }
        public int? InternalId { get; set; }
        public long? IconId { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string TitleChannel { get; set; }
        public string IconWebSrc { get; set; }
        public DateTime? Deleted { get; set; }
        public int? SysOrderCol { get; set; }
        public virtual MediaPic Icon { get; set; }
        public virtual TvprogProviders TvprogProvider { get; set; }
        public virtual ICollection<Programmes> Programmes { get; set; }
        public virtual ICollection<UserChannels> UserChannels { get; set; }
    }
}
