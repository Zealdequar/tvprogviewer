using System;
using System.Collections.Generic;

namespace TVProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесено на TVProgViewer.Core.Domain
    /// </summary>
    public partial class UserChannels
    {
        public UserChannels()
        {
            UsersPrograms = new HashSet<UsersPrograms>();
        }

        public int UserChannelId { get; set; }
        public long Uid { get; set; }
        public int Cid { get; set; }
        public long? IconId { get; set; }
        public string DisplayName { get; set; }
        public int? OrderCol { get; set; }

        public virtual Channels C { get; set; }
        public virtual MediaPic Icon { get; set; }
        public virtual SystemUsers U { get; set; }
        public virtual ICollection<UsersPrograms> UsersPrograms { get; set; }
    }
}
