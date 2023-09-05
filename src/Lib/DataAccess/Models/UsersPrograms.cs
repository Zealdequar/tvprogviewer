using System;
using System.Collections.Generic;

namespace TvProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесено на TvProgViewer.Core.Domain
    /// </summary>
    public partial class UsersPrograms
    {
        public long UserProgramsId { get; set; }
        public long Uid { get; set; }
        public int Ucid { get; set; }
        public long Pid { get; set; }
        public long? Gid { get; set; }
        public long? Rid { get; set; }
        public string Anons { get; set; }
        public bool Remind { get; set; }

        public virtual Genres G { get; set; }
        public virtual Programmes P { get; set; }
        public virtual Ratings R { get; set; }
        public virtual SystemUsers U { get; set; }
        public virtual UserChannels Uc { get; set; }
    }
}
